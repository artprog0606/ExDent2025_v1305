using System.Drawing;
using System.Web.Mvc;
using AdvantShop.CMS;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Cms.Menus;
using AdvantShop.Web.Admin.Models.Cms.Menus;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Cms
{
    [Auth(RoleAction.Store, RoleAction.Settings)]
    public partial class MenusController : BaseAdminController
    {
        [SalesChannel(ESalesChannelType.Store)]
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Menus.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.MenusCtrl);

            return View();
        }
        
        public JsonResult MenusTree(MenusTree model)
        {
            return Json(new GetMenusTree(model).Execute());
        }
        
        public JsonResult GetMenuItem(int menuItemId)
        {
            var item = MenuService.GetMenuItemById(menuItemId);
            if (item == null)
                return JsonError();

            return Json(new MenuItemModel()
            {
                MenuItemId = item.MenuItemID,
                MenuItemParentId = item.MenuItemParentID,
                MenuItemName = item.MenuItemName,
                MenuItemIcon = item.MenuItemIcon,
                MenuItemUrlPath = item.MenuItemUrlPath,
                MenuItemUrlType = item.MenuItemUrlType,
                SortOrder = item.SortOrder,
                ShowMode = item.ShowMode,
                Enabled = item.Enabled,
                Blank = item.Blank,
                NoFollow = item.NoFollow,
                MenuType = item.MenuType,
                HasChild = item.HasChild
            });
        }
        

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddMenuItem(MenuItemModel model)
        {
            return ProcessJsonResult(new SaveMenuItem(model));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateMenuItem(MenuItemModel model)
        {
            return ProcessJsonResult(new SaveMenuItem(model));            
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteMenuItem(int menuItemId, EMenuType menuType)
        {
            DeleteSubMenuItems(menuItemId, menuType);
            return JsonOk();
        }

        private void DeleteSubMenuItems(int menuItemId, EMenuType menuType)
        {
            foreach (var id in MenuService.GetAllChildIdByParent(menuItemId, menuType))
            {
                if (id != menuItemId)
                    DeleteSubMenuItems(id, menuType);

                MenuService.DeleteMenuItemById(id);
            }
        }

        public JsonResult GetLinkUrl(MenuLinkModel model)
        {
            var url = "";

            switch (model.Type)
            {
                case EMenuItemUrlType.Product:
                    url = UrlService.GetLinkDB(ParamType.Product, model.ProductId);
                    break;
                case EMenuItemUrlType.Category:
                    url = UrlService.GetLinkDB(ParamType.Category, model.CategoryId);
                    break;
                case EMenuItemUrlType.StaticPage:
                    url = UrlService.GetLinkDB(ParamType.StaticPage, model.StaticPageId);
                    break;
                case EMenuItemUrlType.News:
                    url = UrlService.GetLinkDB(ParamType.News, model.NewsId);
                    break;
                case EMenuItemUrlType.Brand:
                    url = UrlService.GetLinkDB(ParamType.Brand, model.BrandId);
                    break;
            }
            return Json(new { result = true, url = url });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeMenuSortOrder(int itemId, int? prevItemId, int? nextItemId, int? parentItemId)
        {
            var result = new ChangeMenuItemSortOrder(itemId, prevItemId, nextItemId, parentItemId).Execute();
            return Json(result);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadIcon(int? itemId, int iconWidth, int iconHeight)
        {
            var iconSize = new Size(iconWidth, iconHeight);
            var result = new UploadMenuIcon(itemId, iconSize).Execute();
            return Json(result);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteIcon(int? itemId, string menuItemIcon)
        {
            if (!itemId.HasValue && menuItemIcon.IsNullOrEmpty())
                return JsonError();

            if (itemId.HasValue)
                MenuService.DeleteMenuItemIconById(itemId.Value);
            else
                FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, menuItemIcon));

            return JsonOk();
        }

    }
}
