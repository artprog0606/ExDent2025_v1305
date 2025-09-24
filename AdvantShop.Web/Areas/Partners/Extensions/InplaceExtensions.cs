using System;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Core.Services.InplaceEditor;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;

namespace AdvantShop.Areas.Partners.Extensions
{
    public static class PartnerInplaceExtensions
    {
        const string richSimple = "{editorSimple: true}";
        const string richTpl = "data-inplace-rich=\"{4}\" data-inplace-url=\"{3}\" data-inplace-params=\"{{id: {0}, type: '{1}', field: '{2}'}}\" placeholder=\"{5}\"";

        // Static block
        public static HtmlString InplacePartnerStaticBlock(this HtmlHelper helper, int staticBlockId)
        {
            if (!InplaceEditorService.CanUseInplace(RoleAction.Store))
                return new HtmlString("");

            return new HtmlString(String.Format(richTpl, staticBlockId, InplaceType.StaticBlock, StaticBlockInplaceField.Content, UrlService.GetUrl("inplaceeditor/staticblock"), string.Empty, "Нажмите сюда, чтобы добавить описание"));
        }
    }
}