using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.CMS;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;

namespace AdvantShop.Web.Admin.Models.Cms.Menus
{
    public class MenuItemModel : IValidatableObject
    {
        public int MenuItemId { get; set; }
        public int MenuItemParentId { get; set; }
        public string MenuItemName { get; set; }
        public string MenuItemIcon { get; set; }
        public string MenuItemUrlPath { get; set; }
        public EMenuItemUrlType MenuItemUrlType { get; set; }
        public int SortOrder { get; set; }
        public EMenuItemShowMode ShowMode { get; set; }
        public bool Enabled { get; set; }
        public bool Blank { get; set; }
        public bool NoFollow { get; set; }
        public EMenuType MenuType { get; set; }
        public bool HasChild { get; set; }

        public string MenuItemIconPath
        {
            get { return MenuItemIcon.IsNotEmpty() ? UrlService.GetAbsoluteLink(FoldersHelper.GetPath(FolderType.MenuIcons, MenuItemIcon, false)) : null; }
        }

        public string MenuItemParentName
        {
            get
            {
                if (MenuItemParentId == 0)
                    return LocalizationService.GetResource("Admin.Menus.RootElement");

                var parent = MenuService.GetMenuItemById(MenuItemParentId);
                if (parent != null)
                    return parent.MenuItemName;

                return null;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(MenuItemName))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Menus.Errors.NameRequired"), new[] { "MenuItemName" });
            }
        }
    }
}
