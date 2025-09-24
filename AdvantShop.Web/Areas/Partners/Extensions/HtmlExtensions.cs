using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AdvantShop.CMS;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.InplaceEditor;
using AdvantShop.Customers;
using AdvantShop.ViewModel.StaticBlock;

namespace AdvantShop.Areas.Partners.Extensions
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString PartnerStaticBlock(this HtmlHelper helper, string key, string cssClass = null, bool onlyContent = false)
        {
            var sb = StaticBlockService.GetPagePartByKeyWithCache(key);

            if (sb == null || !sb.Enabled)
                return MvcHtmlString.Create("");

            if (onlyContent)
                return MvcHtmlString.Create(sb.Content);

            var content = sb.Content;
            var canUseInplace = InplaceEditorService.CanUseInplace(RoleAction.Store);

            if (content.IsNotEmpty() && canUseInplace)
            {
                content = InplaceEditorService.PrepareContent(content);
            }

            var sbModel = new StaticBlockViewModel()
            {
                CssClass = cssClass,
                InplaceAttributes = canUseInplace ? helper.InplacePartnerStaticBlock(sb.StaticBlockId) : new HtmlString(""),
                OnlyContent = onlyContent,
                Content = content,
                Key = sb.Key,
                CanUseInplace = canUseInplace
            };

            return helper.Partial("_StaticBlock", sbModel);
        }
    }
}