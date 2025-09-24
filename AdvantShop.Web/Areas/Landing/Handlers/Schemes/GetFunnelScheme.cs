using System.Collections.Generic;
using System.Linq;
using AdvantShop.App.Landing.Domain.Schemes;
using AdvantShop.App.Landing.Extensions;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.Core.Services.Landing.Templates;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.FilePath;

namespace AdvantShop.App.Landing.Handlers.Schemes
{
    public class GetFunnelScheme
    {
        #region Ctor

        private readonly int _siteId;
        private readonly LpService _lpService;
        private readonly LpBlockService _blockService;

        private readonly FunnelSchemeResult _model;

        private readonly List<string> _btnsList = new List<string>();

        public GetFunnelScheme(int siteId)
        {
            _siteId = siteId;

            _lpService = new LpService();
            _blockService = new LpBlockService();

            _model = new FunnelSchemeResult()
            {
                Nodes = new List<FunnelSchemeNode>(),
                Edges = new List<FunnelSchemeEdge>()
            };
        }

        #endregion

        public FunnelSchemeResult Execute()
        {
            var pages = _lpService.GetList(_siteId);

            var page = pages.OrderByDescending(x => x.IsMain).FirstOrDefault();
            if (page == null)
                return _model;

            GetPageNode(null, page, pages, 0, 0);

            var thanksPage = _model.Nodes.FirstOrDefault(x => x.Type == FunnelSchemeType.ThanksPage);
            if (thanksPage != null)
            {
                foreach (var node in _model.Nodes.Where(x => x.NeedLinkToThanksPage))
                {
                    if (!ContainsKeyValue(node.Id, thanksPage.Id))
                    {
                        _model.Edges.Add(new FunnelSchemeEdge() {Source = node.Id, Target = thanksPage.Id});
                    }
                }

                var maxLevel = _model.Nodes.Where(x => x.Type != FunnelSchemeType.ThanksPage).Max(x => x.Level);
                thanksPage.Level = maxLevel + 1;
                thanksPage.SubLevel = 0;
            }

            return _model;
        }
        
        private void GetPageNode(string fromId, Lp page, List<Lp> pages, int level, int sublevel)
        {
            var product = page.ProductId != null ? ProductService.GetProduct(page.ProductId.Value) : null;
            var photo = product != null
                ? FoldersHelper.GetImageProductPath(ProductImageType.Small,
                    !string.IsNullOrEmpty(product.Photo) ? product.Photo : "", false)
                : null;

            var pageNode = new FunnelSchemeNode()
            {
                Id = page.Id.ToString(),
                Title = page.Name,
                Name = product != null ? product.Name : null,
                Content = photo != null ? photo : null,
                IsMain = page.IsMain,
                Type = FunnelSchemeType.Page,
                Level = level,
                SubLevel = sublevel
            };

            _model.Nodes.Add(pageNode);

            if (fromId != null)
                _model.Edges.Add(new FunnelSchemeEdge() {Source = fromId, Target = pageNode.Id});

            GetLinkNodes(pageNode, page, pages, level+1, 0);
        }

        private void GetLinkNodes(FunnelSchemeNode parentNode, Lp page, List<Lp> pages, int level, int sublevel)
        {
            foreach (var block in _blockService.GetList(page.Id))
            {
                var button = block.TryGetSetting<LpButton>("button");
                if (button == null)
                    continue;

                GetNodesByButton(parentNode, button, page, pages, level, ref sublevel);

                var button2 = block.TryGetSetting<LpButton>("button2");
                if (button2 == null)
                    continue;

                GetNodesByButton(parentNode, button2, page, pages, level, ref sublevel);
            }
        }


        private void GetNodesByButton(FunnelSchemeNode parentNode, LpButton button, Lp page, List<Lp> pages, int level, ref int sublevel)
        {
            if (button.Action == "CheckoutUpsell")
            {
                var funnelId = button.ActionUpsellLpId.TryParseInt();

                var item = new FunnelSchemeNode()
                {
                    Name = "Оформление заказа",
                    Type = FunnelSchemeType.LinkedNode,
                    Level = level,
                    SubLevel = sublevel
                };

                var add = funnelId != 0 && !ContainsKeyValue(parentNode.Id, item.Id);

                var isUpsellDownsell = page.PageType == LpTemplatePageType.UpsellFirst ||
                                       page.PageType == LpTemplatePageType.UpsellSecond ||
                                       page.PageType == LpTemplatePageType.Downsell;

                if (isUpsellDownsell)
                {
                    item.Id = (button.Text + page.Id).GetHashCode().ToString();
                    item.Name = "Кнопка: " + button.Text;
                    item.Type = FunnelSchemeType.LinkedButton;
                }

                if (!ContainsKeyValue(parentNode.Id, item.Id))
                {
                    if (isUpsellDownsell && _btnsList.Contains(parentNode.Id))
                        return;

                    var linkedPage = pages.FirstOrDefault(x => x.Id == funnelId);

                    if (isUpsellDownsell && linkedPage == null)
                        item.NeedLinkToThanksPage = true;

                    AddNode(item, parentNode.Id);
                    sublevel++;

                    if (linkedPage != null)
                    {
                        GetPageNode(item.Id, linkedPage, pages, level + 1, 0);
                    }
                    else
                    {
                        _btnsList.Add(parentNode.Id);
                    }
                }

                if (add || (funnelId == 0 && !isUpsellDownsell))
                {
                    GetTriggerOnOrderCreated(item, level + 1, sublevel);
                }
            }
            else if (button.Action == "Url")
            {
                var actionUrlLpId = button.ActionUrlLpId.TryParseInt(true);
                if (actionUrlLpId != null && actionUrlLpId != 0)
                {
                    var linkedPage = pages.FirstOrDefault(x => x.Id == actionUrlLpId.Value);
                    if (linkedPage != null)
                    {
                        var item = new FunnelSchemeNode()
                        {
                            Id = (button.Text + page.Id).GetHashCode().ToString(),
                            Name = "Кнопка: " + button.Text,
                            Type = FunnelSchemeType.LinkedButton,

                            Level = level,
                            SubLevel = sublevel,
                        };
                        
                        if (!ContainsKeyValue(parentNode.Id, item.Id))
                        {
                            AddNode(item, parentNode.Id);
                            sublevel++;

                            GetPageNode(item.Id, linkedPage, pages, level+1, 0);
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(button.ActionUrl))
                {
                    var item = new FunnelSchemeNode()
                    {
                        Id = (button.Text + button.ActionUrl + page.Id).GetHashCode().ToString(),
                        Name = "Кнопка: " + button.Text,
                        Type = FunnelSchemeType.LinkedButton,

                        Level = level,
                        SubLevel = sublevel
                    };

                    if (!ContainsKeyValue(parentNode.Id, item.Id))
                    {
                        AddNode(item, parentNode.Id);
                        sublevel++;
                    }

                    if (button.ActionUrl.Contains("/success"))
                    {
                        var parentId = item.Id;

                        item = new FunnelSchemeNode()
                        {
                            Id = button.ActionUrl.GetHashCode().ToString(),
                            Title = "Спасибо",
                            Name = "Спасибо",
                            Type = FunnelSchemeType.ThanksPage,
                            Level = level,
                            SubLevel = sublevel
                        };

                        if (!_model.Nodes.Any(x => x.Type == FunnelSchemeType.ThanksPage))
                        {
                            level++;
                            AddNode(item, parentId);
                        }
                    }
                }
            }
        }

        private void GetTriggerOnOrderCreated(FunnelSchemeNode parentItem, int level, int sublevel)
        {
            var triggers =
                TriggerRuleService.GetTriggerRules()
                    .Where(x => x.Enabled && x.EventType == ETriggerEventType.OrderCreated)
                    .ToList();

            foreach (var trigger in triggers)
            {
                var triggerItem = new FunnelSchemeNode()
                {
                    Title = "Триггер",
                    Name = trigger.Name,
                    Type = FunnelSchemeType.Trigger,

                    Level = level,
                    SubLevel = sublevel
                };

                AddNode(triggerItem, parentItem.Id);
                level++;

                var actions =
                    TriggerActionService.GetTriggerActions(trigger.Id)
                        .Where(x => x.ActionType == ETriggerActionType.Email || x.ActionType == ETriggerActionType.Sms)
                        .ToList();

                var parentId = triggerItem.Id;

                foreach (var action in actions)
                {
                    if (action.TimeDelay != null && action.TimeDelay.IntervalType != TimeIntervalType.None)
                    {
                        var delayItem = new FunnelSchemeNode()
                        {
                            Name = "через " + action.TimeDelay.Interval + " " + action.TimeDelay.Numeral(),
                            Type = FunnelSchemeType.TriggerDelay,

                            Level = level,
                            SubLevel = sublevel
                        };

                        AddNode(delayItem, parentId);
                        level++;

                        parentId = delayItem.Id;
                    }

                    var actionItem = new FunnelSchemeNode()
                    {
                        Id = action.Id.ToString(),
                        Name = action.ActionType == ETriggerActionType.Email ? "mail" : "sms",
                        Type = action.ActionType == ETriggerActionType.Email ? FunnelSchemeType.Mail : FunnelSchemeType.Sms,

                        Level = level,
                        SubLevel = sublevel
                    };

                    AddNode(actionItem, parentId);
                    level++;

                    parentId = actionItem.Id;
                }
            }
        }

        private bool ContainsKeyValue(string source, string target)
        {
            return _model.Edges.Find(x => x.Source == source && x.Target == target) != null;
        }

        private void AddNode(FunnelSchemeNode node, string sourceId)
        {
            _model.Nodes.Add(node);
            _model.Edges.Add(new FunnelSchemeEdge() { Source = sourceId, Target = node.Id });
        }
    }
}
