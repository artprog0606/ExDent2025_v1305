using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Web.Admin.Models.Cms.Menus;


namespace AdvantShop.Web.Admin.Handlers.Cms.Menus
{
    public class GetMenusTree
    {
        private readonly string _id;
        private readonly int _idSelected;
        private readonly EMenuType _type;
        private readonly bool _showRoot;
        private readonly bool _showActions;
        private readonly int? _excludeId;
        private readonly bool _levelLimitation;

        bool isMobile = SettingsDesign.IsMobileTemplate;

        public GetMenusTree(MenusTree model)
        {
            _id = model.Id;
            _idSelected = model.SelectedId != null ? model.SelectedId.Value : -1;
            _type = model.MenuType;
            _showRoot = model.ShowRoot;
            _showActions = model.ShowActions;
            _excludeId = model.ExcludeId;
            _levelLimitation = model.LevelLimitation;
        }

        public List<AdminMenuTreeViewItem> Execute()
        {
            if (_showRoot)
            {
                return new List<AdminMenuTreeViewItem>()
                    {
                        new AdminMenuTreeViewItem()
                        {
                            id = "0",
                            parent = "#",
                            text = String.Format("<span class=\"jstree-advantshop-name\">{0}</span>", "Корень"),
                                name = "Корень",
                                children = true,
                                state = new AdminMenuTreeViewItemState()
                                {
                                    opened = true,
                                    selected = _idSelected == 0 || _idSelected == -1
                                },
                            li_attr = new Dictionary<string, string>() {
                                { "data-tree-id", "menuItemId_" +  0}
                            },
                        }
                    };
            }


            var menuItems = MenuService.GetAllMenuItems(_id.TryParseInt(), _type);

            if (_excludeId != null && _excludeId != 0)
                menuItems = menuItems.Where(x => x.ItemId != _excludeId).ToList();
            

            var itemOpen = 0;

            if (menuItems.Any(x => x.ItemId != _idSelected))
            {
                var item = MenuService.GetMenuItemById(_idSelected);
                if (item != null)
                {
                    int i = 20;
                    while (item.MenuItemParentID > 0 && i > 0)
                    {
                        if (menuItems.Any(x => x.ItemId == item.MenuItemParentID))
                        {
                            itemOpen = item.MenuItemParentID;
                            break;
                        }
                        item = MenuService.GetMenuItemById(item.MenuItemParentID);
                        i--;
                    }
                }
            }

            return menuItems.Select(x => new AdminMenuTreeViewItem()
            {
                id = x.ItemId.ToString(),
                parent = _id == "#" && x.ItemParentId == 0 ? "#" : x.ItemParentId.ToString(),
                text = _showActions 
                            ? String.Format( !isMobile ?
                                "<span class=\"jstree-advantshop-grab\"><icon-move /></span> " +
                                "<span class=\"jstree-advantshop-name\">{0}</span> " +
                                "<span class=\"jstree-advantshop-count\"> " +
                                    "<menu-item-actions data-id='{1}' data-type='{2}'></menu-item-actions> " +
                                "</span>" : 
                                "<i class=\"jstree-icon jstree-ocl\" role=\"presentation\"></i>"+
                                "<div class=\"navigation-item navigation-item--horizontal card card--middle swipe-line__content card--padding-top-bottom-none\">" +
                                    "<div class=\"navigation-item__icon drop-handle\">" +
                                        "<icon-move>" +
                                            "<svg class=\"icon-move\" version=\"1.0\" xmlns=\"http://www.w3.org/2000/svg\" width=\"10.000000pt\" height=\"20.000000pt\" viewBox=\"0 0 10.000000 20.000000\" preserveAspectRatio=\"xMidYMid meet\">" +
                                                "<g transform = \"translate(0.000000,20.000000) scale(0.100000,-0.100000)\" fill=\"#c4c4c4\" stroke=\"none\">" +
                                                    "<path d=\"M10 175 c0 -8 7 -15 15 -15 8 0 15 7 15 15 0 8 -7 15 -15 15 -8 0 -15 -7 -15 -15z\"></path>" + 
                                                    "<path d=\"M60 175 c0 -8 7 -15 15 -15 8 0 15 7 15 15 0 8 -7 15 -15 15 -8 0 -15 -7 -15 -15z\"></path> " + 
                                                    "<path d=\"M10 125 c0 -8 7 -15 15 -15 8 0 15 7 15 15 0 8 -7 15 -15 15 -8 0 -15 -7 -15 -15z\"></path>" +
                                                    "<path d=\"M60 125 c0 -8 7 -15 15 -15 8 0 15 7 15 15 0 8 -7 15 -15 15 -8 0 -15 -7 -15 -15z\"></path>" +
                                                    "<path d=\"M10 75 c0 -8 7 -15 15 -15 8 0 15 7 15 15 0 8 -7 15 -15 15 -8 0 -15 -7 -15 -15z\"></path>" +
                                                    "<path d=\"M60 75 c0 -8 7 -15 15 -15 8 0 15 7 15 15 0 8 -7 15 -15 15 -8 0 -15 -7 -15 -15z\"></path> " +
                                                    "<path d=\"M10 25 c0 -8 7 -15 15 -15 8 0 15 7 15 15 0 8 -7 15 -15 15 -8 0 -15 -7 -15 -15z\" ></path>" +
                                                    "<path d=\"M60 25 c0 -8 7 -15 15 -15 8 0 15 7 15 15 0 8 -7 15 -15 15 -8 0 -15 -7 -15 -15z\"></path>" +
                                                "/g>" + 
                                            "</svg>" +
                                        "</icon-move>"+
                                    "</div>" +
                                    "<div class=\" jstree-swipe-line-grab-text jstree-advantshop-name\">{0}</div>" + 
                                    "<menu-item-edit-link data-name='{0}' data-id='{1}' data-type='{2}'></menu-item-edit-link>" +
                                "</div><div data-swipe-line-right class=\"swipe-line__right swipe-line__btn-group\" style=\"width:74px;\"><menu-item-actions data-id='{1}' data-type='{2}'></menu-item-actions></div>",
                                x.Name, x.ItemId, _type)
                            : String.Format(
                                "<span class=\"jstree-advantshop-grab\"><icon-move /></span> " +
                                "<span class=\"jstree-advantshop-name\">{0}</span> ", x.Name),
                name = x.Name,
                children = _type == EMenuType.Mobile || (_levelLimitation && _type == EMenuType.Admin) ? false : x.HasChild,
                li_attr = new Dictionary<string, string>() {
                    { "class", "jstree-advantshop-li-grable" +  (isMobile ? " jstree-swipe-line swipe-line drop-item": "")},
                    { "data-tree-id", "menuItemId_" +  x.ItemId},
                },
                a_attr = new Dictionary<string,string>()
                {
                    {"class", "" +  (isMobile ? "jstree-anchor-swipe-line swipe-line__inner": "")},
                    {"data-swipe-line",""},
                    {"data-disabled-swipe-element","drop-handle"}
                },
                state = new AdminMenuTreeViewItemState()
                {
                    opened = x.ItemId == itemOpen,
                    selected = x.ItemId == _idSelected
                }
            }).ToList();
        }
    }
}
