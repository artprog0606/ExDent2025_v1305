using AdvantShop.Web.Infrastructure.Admin.Buttons;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.ViewModels.Shared.Common
{
    public class HeaderViewModel
    {
        public HeaderViewModel()
        {
            EnabledBack = true;
            Controls = new List<IButton>();
            Title = "<span ng-bind=\"app.getTitle()\"></span>";
        }

        public bool EnabledBack { get; set; }
        public BackViewModel Back { get; set; }
        public string Title { get; set; }
        
        public string DefaultTitle { get; set; }
        public string SubTitle { get; set; }

        public string CSSClass { get; set; }
        public bool EnabledSearch { get; set; }
        public List<IButton> Controls { get; set; }
        public bool ShowOnlySticky { get; set; }
    }
}