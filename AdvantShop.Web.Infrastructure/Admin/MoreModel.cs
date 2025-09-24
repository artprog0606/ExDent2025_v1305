using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace AdvantShop.Web.Infrastructure.Admin.Buttons
{
    public class MoreButtonModel : ButtonModel
    {
        public MoreButtonModel()
        {
            Items = new List<MoreButtonPopoverItem>();

            Icon = new ButtonIcon()
            {
                SvgName = "more",
                Attributes = new { width = 24, height = 24 },
            };

            ColorType = eColorType.Secondary;
        }

        public List<MoreButtonPopoverItem> Items { get; set; }

        public string NgTemplateId { get; set; }

        public string NgIsOpen { get; set; }

        public bool UseExternalTemplate { get; set; }
    }

    public class MoreButtonPopoverItem : ButtonModel
    {
        public object RowAttributes { get; set; }

        public string RowAttributesSerialized
        {
            get
            {
                if (RowAttributes != null)
                {
                    var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(RowAttributes);
                    var sb = new StringBuilder();
                    foreach (var kvp in attrs)
                    {
                        sb.Append(" ");
                        sb.Append(string.Format("{0}=\"{1}\"", kvp.Key, kvp.Value));
                    }

                    return sb.ToString();
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
