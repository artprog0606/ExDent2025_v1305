using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.App.Landing.Domain.Products;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.App.Landing.Handlers.Inplace;
using AdvantShop.App.Landing.Handlers.Install;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.Core.Services.Landing.Templates;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Screenshot;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Landings;
using AdvantShop.Web.Infrastructure.Handlers;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Landings
{
    public class AddLanding : AbstractCommandHandler<AddingLpResult>
    {
        #region Ctor

        private readonly LpService _lpService;
        private readonly LpSiteService _siteService;
        private readonly LpTemplateService _templateService;
        private readonly LpBlockService _blockService;
        private readonly LpFormService _formService;

        private readonly AddingLpModel _model;
        private readonly List<int> _landingIds = new List<int>();

        private List<LpBlock> addedBlocks = new List<LpBlock>(); 

        public AddLanding(AddingLpModel model)
        {
            _lpService = new LpService();
            _siteService = new LpSiteService();
            _templateService = new LpTemplateService();
            _blockService = new LpBlockService();
            _formService = new LpFormService();

            _model = model;
        }

        #endregion

        protected override AddingLpResult Handle()
        {
            if (string.IsNullOrEmpty(_model.Template))
                _model.Template = "Default";
            
            if (string.IsNullOrEmpty(_model.Name))
            {
                if (_model.ProductId != null)
                {
                    var p = ProductService.GetProduct(_model.ProductId.Value);
                    if (p != null)
                        _model.Name = p.Name;
                }
            }

            if (string.IsNullOrEmpty(_model.Name))
                _model.Name = LocalizationService.GetResource("Admin.Landings.AddLanding.NewFunnel");

            if (_model.Name.ToLower() == "lp")
                throw new BlException(LocalizationService.GetResource("Admin.Landings.AddLanding.AnotherFunnelName"));

            try
            {
                return CreateLandingSite();
            }
            catch (BlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            throw new Exception(LocalizationService.GetResource("Admin.Landings.AddLanding.ErrorCreatingFunnel"));
        }

        private AddingLpResult CreateLandingSite()
        {
            CreatingIsAllowed();
            var template = _templateService.GetTemplate(_model.Template);

            var site = new LpSite()
            {
                Name = StringHelper.RemoveHTML(_model.Name ?? "").Reduce(100),
                Template = _model.Template,
                Enabled = true,
                Url = _siteService.GetAvailableUrl(_model.Url.Reduce(100)),
                ProductId = _model.ProductId
            };
            _siteService.Add(site);

            if (_model.LpType == LpFunnelType.ProductCrossSellDownSell)
            {
                var additionalSalesProductId = _model.AdditionalSalesProductId != null
                    ? _model.AdditionalSalesProductId.Value
                    : _model.ProductId;

                if (additionalSalesProductId != null)
                    _siteService.AddAdditionalSalesProduct(additionalSalesProductId.Value, site.Id);
            }

            Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Funnels_FunnelCreated, site.Template);

            SaveSiteSettings(site.Id, template.SiteSettings);
            
            var resultModel = new AddingLpResult()
            {
                LpUrl = UrlService.GetUrl("lp/" + site.Url + "?inplace=true"),
                AdminUrl = UrlService.GetUrl("adminv2/funnels/site/" + site.Id)
            };

            if (SalesChannelService.IsFirstTimeCreateStore()) {
                resultModel.AdminUrl = UrlService.GetUrl("adminv3/dashboard");
            }

            if (template.Pages == null || template.Pages.Count == 0)
            {
                var lp = new Lp()
                {
                    Name = site.Name,
                    Enabled = true,
                    IsMain = true,
                    LandingSiteId = site.Id,
                    Template = _model.Template
                };
                lp.Url = _lpService.GetAvailableUrl(lp.LandingSiteId, lp.Name);

                _lpService.Add(lp);

                SavePageSettings(lp.Id, null, lp, null);
                
                AddPublicOfferPage(site.Id);

                _lpService.ReGenerateCss(site.Id);

                return resultModel;
            }

            var i = 0;
            var configurationAfter = new LpConfigurationAfter();
            var configuration = new LpConfiguration()
            {
                Template = _model.Template,
                Type = _model.LpType,
                ProductId = _model.ProductId,
                UpsellProductIdFirst = _model.UpsellProductIdFirst,
                UpsellProductIdSecond = _model.UpsellProductIdSecond,
                DownSellProductId = _model.DownSellProductId,
                ProductIds = _model.ProductIds,
                CategoryIds = _model.CategoryIds,
                OfferIds = _model.OfferIds,
                PostActionUrl = GetPostActionUrl(_model.PostAction)
            };

            foreach (var page in template.Pages)
            {
                try
                {
                    if ((page.PageType == LpTemplatePageType.UpsellFirst && configuration.UpsellProductIdFirst == null) ||
                        (page.PageType == LpTemplatePageType.UpsellSecond && configuration.UpsellProductIdSecond == null) ||
                        (page.PageType == LpTemplatePageType.Downsell && configuration.DownSellProductId == null))
                    {
                        continue;
                    }
                    
                    configuration.PageType = page.PageType;
                    configuration.ClearProduct();
                    
                    var lp = AddPage(page, configuration, configurationAfter, site.Id, i == 0);
                    

                    _landingIds.Add(lp.Id);
                    i++;
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }

            if (configurationAfter.UpsellSecondLpId == null)
                configurationAfter.UpsellSecondLpId = configurationAfter.ThankYouPageLpId;

            if (configurationAfter.DownsellLpId == null)
                configurationAfter.DownsellLpId = configurationAfter.ThankYouPageLpId;

            configurationAfter.AddedBlocks = addedBlocks;
            configurationAfter.PostActionUrl = configuration.PostActionUrl;

            // проходим по всем страницам и применяем настройки к блокам, 
            // потому что некоторые настройки мы знаем только когда блоки уже созданы
            foreach (var landingId in _landingIds)
            {
                configurationAfter.LandingId = landingId;

                foreach (var block in _blockService.GetList(landingId))
                {
                    try
                    {
                        var blockSettingsStr = _templateService.PrepareContentAfter("", block.Settings, configuration, configurationAfter);
                        var reSave = blockSettingsStr != block.Settings;

                        var form = _formService.GetByBlock(block.Id);
                        if (form != null)
                        {
                            var formStr = JsonConvert.SerializeObject(form);
                            var formPreparedStr = _templateService.PrepareContentAfter("", formStr, configuration, configurationAfter);
                            if (formStr != formPreparedStr)
                            {
                                form = JsonConvert.DeserializeObject<LpForm>(formPreparedStr);
                                reSave = true;
                            }
                        }

                        if (reSave)
                        {
                            var blockSettings = JsonConvert.DeserializeObject<Dictionary<string, object>>(blockSettingsStr);

                            new SaveBlockSettings(block.Id, blockSettings, form).Execute();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                    }
                }
            }
            
            AddPublicOfferPage(site.Id);

            _lpService.ReGenerateCss(site.Id);

            new ScreenshotService().UpdateFunnelScreenShotInBackground(site);

            return resultModel;
        }

        private Lp AddPage(LpTemplateLandingPageItem page, LpConfiguration configuration, LpConfigurationAfter configurationAfter, int siteId, bool isMain, string url = null)
        {
            var lp = new Lp()
            {
                Name = StringHelper.RemoveHTML(page.Name),
                Enabled = true,
                IsMain = isMain,
                LandingSiteId = siteId,
                Template = _model.Template, //i != 0 ? _model.Template : "Default",
                PageType = page.PageType,
                ProductId = configuration.Product != null ? configuration.Product.ProductId : default(int?)
            };
            lp.Url = _lpService.GetAvailableUrl(lp.LandingSiteId, url ?? lp.Name);

            _lpService.Add(lp);
            
            SavePageSettings(lp.Id, page.LpPageSettings, lp, configuration);

            ConfigureAfter(lp, page.PageType, configurationAfter);

            var sortOrder = 0;

            for (var blockIndex = 0; blockIndex < page.Blocks.Count; blockIndex++)
            {
                try
                {
                    var block = ConfigureBeforeInstallBlock(page.Blocks, page.Blocks[blockIndex], configuration);

                    var result = new InstallBlockHandler(block.Name, lp.Template, lp.Id, sortOrder, configuration).Execute();
                    if (result != null && result.BlockId != 0)
                    {
                        var blockSettingsStr = _templateService.PrepareContent("", JsonConvert.SerializeObject(block.Settings), configuration);
                        var blockSettings = JsonConvert.DeserializeObject<Dictionary<string, object>>(blockSettingsStr);

                        new SaveBlockSettings(result.BlockId, blockSettings, block.Form).Execute();

                        if (block.SubBlocks != null && block.SubBlocks.Count > 0)
                        {
                            foreach (var subBlock in _blockService.GetSubBlocks(result.BlockId))
                            {
                                var subBlockSettings = block.SubBlocks.Find(x => x.Name == subBlock.Name && x.Type == subBlock.Type);
                                if (subBlockSettings == null)
                                    continue;

                                if (subBlock.Type == "html" || subBlock.Type == "text")
                                {
                                    subBlock.ContentHtml = _templateService.PrepareContent(subBlock.ContentHtml, subBlockSettings.Placeholder, configuration);
                                }
                                else if (subBlock.Type == "logo" && !SettingsMain.IsDefaultLogo)
                                {
                                    
                                }
                                else
                                {
                                    subBlock.Settings = _templateService.PrepareContent(subBlock.Settings, JsonConvert.SerializeObject(subBlockSettings.Settings), configuration);
                                }

                                _blockService.UpdateSubBlock(subBlock);
                            }
                        }

                        addedBlocks.Add(new LpBlock() {LandingId = lp.Id, Id = result.BlockId, Name = block.Name});
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }

                sortOrder += 100;
            }

            return lp;
        }

        public void CreatingIsAllowed()
        {
            var template = _templateService.GetTemplate(_model.Template);
            if (template == null)
                throw new BlException("Шаблон не найден");

            if (((template.TemplateType == LpFunnelType.Consulting && template.Key.Equals("Consulting", StringComparison.OrdinalIgnoreCase)) || 
                 (template.TemplateType == LpFunnelType.ServicesOnline && template.Key.Equals("ServicesOnline", StringComparison.OrdinalIgnoreCase)) ||
                 template.Key.Equals("CompanySiteWithBooking", StringComparison.OrdinalIgnoreCase)
                ) && !SettingsMain.BookingActive)
            {
                throw new BlException("Нельзя создать воронку. Не активировано приложение 'Бронирование'. Для активации перейдите по <a href=\"./settingsbooking\" target=\"_blank\">ссылке</a>");
            }

            if (template.TemplateType == LpFunnelType.ProductCrossSellDownSell && template.Key.Equals("ProductCrossSellDownSell", StringComparison.OrdinalIgnoreCase)
                && _model.UpsellProductIdFirst == null && _model.DownSellProductId == null)
            {
                throw new BlException("Нельзя создать воронку. Не выбран товар допродажи");
            }
        }

        private void ConfigureAfter(Lp lp, LpTemplatePageType pageType, LpConfigurationAfter configurationAfter)
        {
            if (pageType == LpTemplatePageType.Main)
                configurationAfter.MainLpId = lp.Id;

            if (pageType == LpTemplatePageType.MainSecond)
                configurationAfter.MainSecondLpId = lp.Id;

            if (pageType == LpTemplatePageType.MainThird)
                configurationAfter.MainThirdLpId = lp.Id;

            if (pageType == LpTemplatePageType.MainFour)
                configurationAfter.MainFourLpId = lp.Id;

            if (pageType == LpTemplatePageType.MainFive)
                configurationAfter.MainFiveLpId = lp.Id;

            if (pageType == LpTemplatePageType.MainSix)
                configurationAfter.MainSixLpId = lp.Id;
            

            if (pageType == LpTemplatePageType.UpsellFirst)
                configurationAfter.UpsellFirstLpId = lp.Id;

            if (pageType == LpTemplatePageType.UpsellSecond)
                configurationAfter.UpsellSecondLpId = lp.Id;

            if (pageType == LpTemplatePageType.Downsell)
                configurationAfter.DownsellLpId = lp.Id;

            if (pageType == LpTemplatePageType.ThankYouPage)
                configurationAfter.ThankYouPageLpId = lp.Id;
        }

        private LpTemplateBlockItem ConfigureBeforeInstallBlock(List<LpTemplateBlockItem> blocks, LpTemplateBlockItem block, LpConfiguration cfg)
        {
            try
            {
                // Если встречам блок productsView и есть товары, то 
                // получаем категории по товарам и создаем блоки с этими категориями и выбранными товарами
                if (block.Name == "productsView")
                {
                    block = PrepareBlockProductsView(blocks, block, cfg);
                }
                else if (block.Name == "productsViewLead" && cfg.Template == "MultyProductsKnowPrice")
                {
                    block = PrepareBlockProductsViewLead(blocks, block, cfg);
                }
                else if (block.Name == "textSingleLeft" && cfg.Template == "MultyProductsShort")
                {
                    PrepareBlockProductsViewOneWithBigPicture(blocks, block, cfg);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return block;
        }


        /// <summary>
        /// Сохраняем настройки страницы
        /// </summary>
        private void SavePageSettings(int lpId, Dictionary<string, string> settings, Lp lp, LpConfiguration configuration)
        {
            if (settings == null)
            {
                LpService.CurrentLanding = lp;
                LPageSettings.PageTitle = StringHelper.RemoveHTML(lp.Name);
                return;
            }

            var service = new LpSettingsService();

            foreach (var key in settings.Keys)
                service.AddOrUpdate(lpId, key, _templateService.PrepareContent("", settings[key] ?? "", configuration));

            CacheManager.RemoveByPattern(LpConstants.LpCachePrefix);
        }

        /// <summary>
        /// Сохраняем настройки сайта
        /// </summary>
        private void SaveSiteSettings(int siteId, Dictionary<string, string> settings)
        {
            if (settings == null)
            {
                LpService.CurrentSiteId = siteId;
                LSiteSettings.FontMain = LSiteSettings.GetDefaultFonts()[0].Name;
                LSiteSettings.LineHeight = "1.2";
                return;
            }

            var service = new LpSiteSettingsService();

            foreach (var key in settings.Keys)
                service.AddOrUpdate(siteId, key, settings[key] ?? "");

            CacheManager.RemoveByPattern(LpConstants.LpCachePrefix);
        }

        private string GetPostActionUrl(PostAction postAction)
        {
            if (postAction == null)
                return null;

            if (postAction.PostActionType == 1 && postAction.PostActionCategoryId != null)
            {
                var category = CategoryService.GetCategory(postAction.PostActionCategoryId.Value);

                return category != null ? "categories/" + category.UrlPath : "";
            }
            if (postAction.PostActionType == 2 && postAction.PostActionFunnelSiteId != null)
            {
                var site = _siteService.Get(postAction.PostActionFunnelSiteId.Value);

                return site != null 
                    ? (!string.IsNullOrEmpty(site.DomainUrl) ? site.DomainUrl : "lp/" + site.Url) 
                    : "";
            }

            if (postAction.PostActionType == 3)
            {
                return postAction.PostActionUrl != null ? postAction.PostActionUrl.Trim() : "";
            }

            return null;
        }

        private LpTemplateBlockItem PrepareBlockProductsView(List<LpTemplateBlockItem> blocks, LpTemplateBlockItem block, LpConfiguration cfg)
        {
            if (cfg.ProductIds != null && cfg.ProductIds.Count > 0)
            {
                var categories = new LpTemplateHelperService().GetCategoriesByProductIds(cfg.ProductIds);
                if (categories.Count > 0)
                {
                    var newBlocks = new List<LpTemplateBlockItem>();

                    if (categories.Count > 1)
                    {
                        foreach (var category in categories.Skip(1))
                            newBlocks.Add(new LpTemplateBlockItem()
                            {
                                Name = block.Name,
                                Form = block.Form != null ? block.Form.DeepClone() : null,
                                SubBlocks = block.SubBlocks != null ? block.SubBlocks.DeepClone() : null,
                                Settings = block.Settings
                            });
                    }

                    var blockStr = JsonConvert.SerializeObject(block);

                    blockStr = blockStr
                        .Replace("#CATEGORY_FOR_PRODUCTSVIEW_NAME_1#", categories.First().Key.Name.Replace("\"", ""))
                        .Replace("\"#PRODUCT_IDS_FOR_PRODUCTSVIEW_1#\"", "[" + String.Join(", ", categories.First().Value) + "]");

                    block = JsonConvert.DeserializeObject<LpTemplateBlockItem>(blockStr);

                    for (var i = 1; i < categories.Count; i++)
                    {
                        blockStr = JsonConvert.SerializeObject(newBlocks[i - 1]);

                        blockStr = blockStr
                            .Replace("#CATEGORY_FOR_PRODUCTSVIEW_NAME_1#", categories.ElementAt(i).Key.Name.Replace("\"", ""))
                            .Replace("\"#PRODUCT_IDS_FOR_PRODUCTSVIEW_1#\"", "[" + String.Join(", ", categories.ElementAt(i).Value) + "]");

                        newBlocks[i - 1] = JsonConvert.DeserializeObject<LpTemplateBlockItem>(blockStr);
                    }

                    var index = blocks.FindIndex(x => x.Name == "productsView");
                    if (index > 0)
                    {
                        if (blocks[index + 1].Name == "productsView")
                            blocks.RemoveAt(index + 1);

                        blocks[index] = block;
                        blocks.InsertRange(index + 1, newBlocks);
                    }

                    cfg.ProductIds = null;
                }
            }
            else if (cfg.CategoryIds != null && cfg.CategoryIds.Count > 0)
            {
                //\"lp-block-products-view\ не нужен так как не паддингов из за него между товарами",
                var blockStr =
                    "{\"Name\":\"productsByCategories\",\"Form\":null,\"Settings\":{\"categories\":#CATEGORIES_BY_IDS#,\"show_title\":true,\"show_subtitle\":false,\"show_video\":true,\"color_scheme\":\"color-scheme--light\",\"classes\":[\"color-scheme--light\",\"block-padding-top--45\",\"block-padding-bottom--45\"],\"show_price\":true,\"multiSelect\":false,\"number_products_in_row\":3,\"show_buybutton\":true,\"darken\":0,\"padding_top\":45,\"padding_bottom\":45,\"mobile_hidden\":false,\"style\":{\"background-position\":\"center center\",\"background-attachment\":\"scroll\"},\"no_cache\":true},\"SubBlocks\":[{\"name\": \"title\",\"type\": \"html\",\"placeholder\": \"#TITLE#\"}]}";

                var categories = new List<LpProductCategory>();
                foreach (var id in cfg.CategoryIds)
                {
                    var category = CategoryService.GetCategory(id);
                    if (category != null)
                    {
                        categories.Add(new LpProductCategory()
                        {
                            CategoryId = category.CategoryId,
                            Name = category.Name.Replace("\"", "")
                        });
                    }
                }

                var blockItemStr =
                    categories.Count > 0
                        ? blockStr.Replace("#CATEGORIES_BY_IDS#", string.Format("[ {{ \"CategoryId\": {0}, \"Name\": \"{1}\" }} ]", categories[0].CategoryId, categories[0].Name))
                                  .Replace("#TITLE#", categories[0].Name)
                        : blockStr.Replace("#CATEGORIES_BY_IDS#", "[]")
                                  .Replace("#TITLE#", "");

                block = JsonConvert.DeserializeObject<LpTemplateBlockItem>(blockItemStr);

                var newBlocks = new List<LpTemplateBlockItem>();

                foreach (var category in categories.Skip(1))
                {
                    blockItemStr =
                        blockStr.Replace("#CATEGORIES_BY_IDS#", string.Format("[ {{ \"CategoryId\": {0}, \"Name\": \"{1}\" }} ]", category.CategoryId, category.Name))
                                .Replace("#TITLE#", category.Name);

                    var newBlock = JsonConvert.DeserializeObject<LpTemplateBlockItem>(blockItemStr);
                    newBlocks.Add(newBlock);
                }

                var index = blocks.FindIndex(x => x.Name == "productsView");
                if (index > 0)
                {
                    if (blocks[index + 1].Name == "productsView")
                        blocks.RemoveAt(index + 1);

                    blocks[index] = block;
                    blocks.InsertRange(index + 1, newBlocks);
                }

                cfg.CategoryIds = null;
            }

            return block;
        }

        private LpTemplateBlockItem PrepareBlockProductsViewLead(List<LpTemplateBlockItem> blocks, LpTemplateBlockItem block, LpConfiguration cfg)
        {
            if (cfg.ProductIds != null && cfg.ProductIds.Count > 0)
            {
                var categories = new LpTemplateHelperService().GetCategoriesByProductIds(cfg.ProductIds);
                if (categories.Count > 0)
                {
                    var newBlocks = new List<LpTemplateBlockItem>();

                    if (categories.Count > 1)
                    {
                        foreach (var category in categories.Skip(1))
                            newBlocks.Add(new LpTemplateBlockItem()
                            {
                                Name = block.Name,
                                Form = block.Form != null ? block.Form.Clone() : null,
                                SubBlocks = block.SubBlocks != null ? block.SubBlocks.DeepClone() : null,
                                Settings = block.Settings
                            });
                    }

                    var blockStr = JsonConvert.SerializeObject(block);

                    blockStr = blockStr
                        .Replace("#CATEGORY_FOR_PRODUCTSVIEW_NAME_1#", categories.First().Key.Name.Replace("\"", ""))
                        .Replace("\"#PRODUCT_IDS_ARRAY#\"", "[" + String.Join(", ", categories.First().Value) + "]");

                    block = JsonConvert.DeserializeObject<LpTemplateBlockItem>(blockStr);

                    for (var i = 1; i < categories.Count; i++)
                    {
                        blockStr = JsonConvert.SerializeObject(newBlocks[i - 1]);

                        blockStr = blockStr
                            .Replace("#CATEGORY_FOR_PRODUCTSVIEW_NAME_1#", categories.ElementAt(i).Key.Name.Replace("\"", ""))
                            .Replace("\"#PRODUCT_IDS_ARRAY#\"", "[" + String.Join(", ", categories.ElementAt(i).Value) + "]");

                        newBlocks[i - 1] = JsonConvert.DeserializeObject<LpTemplateBlockItem>(blockStr);
                    }

                    var index = blocks.FindIndex(x => x.Name == "productsViewLead");
                    if (index > 0)
                    {
                        if (blocks[index + 1].Name == "productsViewLead")
                            blocks.RemoveAt(index + 1);

                        blocks[index] = block;
                        blocks.InsertRange(index + 1, newBlocks);
                    }

                    cfg.ProductIds = null;
                }
            }
            else if (cfg.CategoryIds != null && cfg.CategoryIds.Count > 0)
            {
                var blockStr =
                    "{\"name\":\"productsByCategoriesLead\",\"settings\":{\"categories\":#CATEGORIES_BY_IDS#,\"show_title\":true,\"show_subtitle\":true,\"color_scheme\":\"color-scheme--light\",\"classes\":[\"lp-block-products-by-categories\",\"color-scheme--light\",\"block-padding-top--45\",\"block-padding-bottom--45\"],\"show_price\":true,\"show_description\":false,\"background_image\":null,\"background_color\":null,\"show_buybutton\":true,\"quickview\":true,\"background_settings\":{\"background_fixed\":false,\"parallax\":false},\"number_products_in_row\":3,\"products_count\":12,\"is_lead_button\":true,\"button\":{\"text\":\"Узнать цену\",\"action\":\"Form\",\"action_url\":\"\",\"action_section\":\"\"},\"form\":{\"title\":\"Узнать цену\",\"subTitle\":\"Заполните форму ниже и мы пришлем Вам цены\",\"is_hidden\":true},\"darken\":0,\"padding_top\":45,\"padding_bottom\":45,\"style\":{},\"no_cache\":true},\"form\":{\"title\":\"Узнать цену\",\"subTitle\":\"Заполните форму ниже и мы пришлем Вам цены\",\"buttonText\":\"Отправить\",\"postAction\":2,\"postMessageRedirectLpId\":\"#ACTION_THANKYOUPAGE_LP_ID#\",\"postMessageText\":\"Спасибо за заявку! С Вами свяжется наш менеджер!\",\"fields\":[{\"Title\":\"Имя\",\"TitleCrm\":\"Имя\",\"FieldType\":2,\"Type\":\"text\",\"CustomFieldId\":null,\"Required\":true},{\"Title\":\"Телефон\",\"TitleCrm\":\"Телефон\",\"FieldType\":6,\"Type\":\"tel\",\"CustomFieldId\":null,\"Required\":true},{\"Title\":\"Email\",\"TitleCrm\":\"Email\",\"FieldType\":5,\"Type\":\"text\",\"CustomFieldId\":null,\"Required\":true}],\"showAgreement\":true,\"agreementText\":\"Я согласен на обработку персональных данных\",\"is_hidden\":true},\"SubBlocks\":[{\"name\":\"title\",\"type\":\"html\",\"placeholder\":\"#TITLE#\"}]}";

                var categories = new List<LpProductCategory>();
                foreach (var id in cfg.CategoryIds)
                {
                    var category = CategoryService.GetCategory(id);
                    if (category != null)
                    {
                        categories.Add(new LpProductCategory()
                        {
                            CategoryId = category.CategoryId,
                            Name = category.Name.Replace("\"", "")
                        });
                    }
                }

                var blockItemStr =
                    categories.Count > 0
                        ? blockStr.Replace("#CATEGORIES_BY_IDS#", string.Format("[ {{ \"CategoryId\": {0}, \"Name\": \"{1}\" }} ]", categories[0].CategoryId, categories[0].Name))
                                  .Replace("#TITLE#", categories[0].Name)
                        : blockStr.Replace("#CATEGORIES_BY_IDS#", "[]")
                                  .Replace("#TITLE#", "");

                block = JsonConvert.DeserializeObject<LpTemplateBlockItem>(blockItemStr);

                var newBlocks = new List<LpTemplateBlockItem>();

                foreach (var category in categories.Skip(1))
                {
                    blockItemStr =
                        blockStr.Replace("#CATEGORIES_BY_IDS#", string.Format("[ {{ \"CategoryId\": {0}, \"Name\": \"{1}\" }} ]", category.CategoryId, category.Name))
                                .Replace("#TITLE#", category.Name);

                    var newBlock = JsonConvert.DeserializeObject<LpTemplateBlockItem>(blockItemStr);
                    newBlocks.Add(newBlock);
                }

                var index = blocks.FindIndex(x => x.Name == "productsViewLead");
                if (index > 0)
                {
                    if (blocks[index + 1].Name == "productsViewLead")
                        blocks.RemoveAt(index + 1);

                    blocks[index] = block;
                    blocks.InsertRange(index + 1, newBlocks);
                }

                cfg.CategoryIds = null;
            }

            return block;
        }

        private void PrepareBlockProductsViewOneWithBigPicture(List<LpTemplateBlockItem> blocks, LpTemplateBlockItem block, LpConfiguration cfg)
        {
            if (cfg.ProductIds == null || cfg.ProductIds.Count <= 0)
                return;

            var blockProductsViewOneWithBigPictureStr = "{\"name\":\"productsViewOneWithBigPicture\",\"settings\":{\"product_ids\":[#PRODUCT_ITEMS#],\"show_title\":false,\"show_subtitle\":false,\"color_scheme\":\"color-scheme--light\",\"classes\":[\"lp-block-products-view\",\"lp-block-products-view--one-big-picture\",\"color-scheme--light\",\"block-padding-top--0\",\"block-padding-bottom--0\"],\"show_price\":true,\"hide_photo\":false,\"show_description\":false,\"quickview\":true,\"quickview_description\":\"briefDescription\",\"descriptionOptions\":[{\"value\":\"none\",\"name\":\"Не отображать\",\"$$hashKey\":\"object:173\"},{\"value\":\"briefDescription\",\"name\":\"Отображать краткое описание\",\"$$hashKey\":\"object:174\"},{\"value\":\"fullDescription\",\"name\":\"Отображать полное описание\",\"$$hashKey\":\"object:175\"}],\"multiSelect\":false,\"number_products_in_row\":3,\"show_buybutton\":false,\"darken\":0,\"background_settings\":{\"background_fixed\":false,\"parallax\":false},\"button\":{\"text\":\"Купить\",\"action_upsell_lp_id\":\"\"},\"padding_top\":0,\"padding_bottom\":0,\"mobile_hidden\":false,\"no_cache\":true}}";
            var blockProductsViewStr = "{\"name\":\"productsView\",\"settings\":{\"product_ids\":[#PRODUCT_ITEMS#],\"show_title\":false,\"show_subtitle\":false,\"color_scheme\":\"color-scheme--light\",\"classes\":[\"lp-block-products-view\",\"color-scheme--light\",\"block-padding-top--45\",\"block-padding-bottom--45\"],\"show_price\":true,\"hide_photo\":false,\"show_description\":false,\"quickview\":true,\"quickview_description\":\"briefDescription\",\"descriptionOptions\":[{\"value\":\"none\",\"name\":\"Не отображать\",\"$$hashKey\":\"object:89\"},{\"value\":\"briefDescription\",\"name\":\"Отображать краткое описание\",\"$$hashKey\":\"object:90\"},{\"value\":\"fullDescription\",\"name\":\"Отображать полное описание\",\"$$hashKey\":\"object:91\"}],\"multiSelect\":false,\"number_products_in_row\":3,\"show_buybutton\":false,\"background_settings\":{\"background_fixed\":false,\"parallax\":false},\"button\":{\"text\":\"Купить\",\"action_upsell_lp_id\":\"\"},\"darken\":0,\"padding_top\":45,\"padding_bottom\":45,\"mobile_hidden\":false,\"no_cache\":true}}";

            var newBlocks = new List<LpTemplateBlockItem>();
            var j = 0;

            for (var i = 0; i < cfg.ProductIds.Count; i++)
            {
                if (j%2 == 0)
                {
                    var json = blockProductsViewOneWithBigPictureStr.Replace("#PRODUCT_ITEMS#", cfg.ProductIds[i].ToString());
                    var newBlock = JsonConvert.DeserializeObject<LpTemplateBlockItem>(json);

                    newBlocks.Add(newBlock);
                }
                else
                {
                    var count = cfg.ProductIds.Count - i >= 6 ? 6 : cfg.ProductIds.Count - i;
                    var items = cfg.ProductIds.GetRange(i, count);
                    i += count - 1;

                    var json = blockProductsViewStr.Replace("#PRODUCT_ITEMS#", string.Join(",", items));
                    var newBlock = JsonConvert.DeserializeObject<LpTemplateBlockItem>(json);

                    newBlocks.Add(newBlock);
                }
                j++;
            }

            var index = blocks.FindIndex(x => x.Name == "textSingleLeft");
            blocks.InsertRange(index + 1, newBlocks);

            cfg.ProductIds = null;
        }

        private void AddPublicOfferPage(int siteId)
        {
            var template = _templateService.GetTemplate("PublicOffer");
            if (template == null)
                return;
            
            var i = 0;
            var configurationAfter = new LpConfigurationAfter();
            var configuration = new LpConfiguration()
            {
                Template = _model.Template,
                Type = _model.LpType
            };
            
            foreach (var page in template.Pages)
            {
                AddPage(page, configuration, configurationAfter, siteId, false, "public-offer");
            }
        }
    }
}
