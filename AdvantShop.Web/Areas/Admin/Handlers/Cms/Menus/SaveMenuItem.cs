using System;
using System.IO;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Cms.Menus;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Cms.Menus
{
    public class SaveMenuItem : AbstractCommandHandler
    {
        private readonly MenuItemModel _model;
        private readonly bool _editMode;
        private AdvMenuItem _menuItem;

        public SaveMenuItem(MenuItemModel model)
        {
            _model = model;
            _editMode = _model.MenuItemId != 0;
        }

        protected override void Load()
        {
            _menuItem = _editMode ? MenuService.GetMenuItemById(_model.MenuItemId) : null;
        }

        protected override void Validate()
        {
            if (_editMode && _menuItem == null)
                throw new BlException("Not found");
        }

        protected override void Handle()
        {
            if (!_editMode)
                _menuItem = new AdvMenuItem();
            _menuItem.MenuItemName = _model.MenuItemName;
            _menuItem.MenuItemParentID = _model.MenuItemParentId;
            _menuItem.MenuItemUrlPath = _model.MenuItemUrlPath ?? "";
            _menuItem.SortOrder = _model.SortOrder;
            _menuItem.Blank = _model.Blank;
            _menuItem.Enabled = _model.Enabled;
            _menuItem.MenuItemUrlType = _model.MenuItemUrlType;
            _menuItem.NoFollow = _model.NoFollow;
            _menuItem.MenuType = _model.MenuType;
            _menuItem.ShowMode = _model.ShowMode;

            if (_editMode)
            {
                MenuService.UpdateMenuItem(_menuItem);
            }
            else
            {
                MenuService.AddMenuItem(_menuItem);
                Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Menu_MenuItemCreated);

                if (_model.MenuItemIcon.IsNotEmpty())
                    SaveIconFromTemp();
            }
        }

        private void SaveIconFromTemp()
        {
            var tempPath = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, _model.MenuItemIcon);
                
            if (!File.Exists(tempPath) ||
                !FileHelpers.CheckFileExtensionByType(_model.MenuItemIcon, EFileType.Image)) 
                return;
            
            try
            {
                var photo = new Photo(0, _menuItem.MenuItemID, PhotoType.MenuIcon) { OriginName = _model.MenuItemIcon };
                PhotoService.AddPhoto(photo);
                
                if (photo.PhotoName.IsNotEmpty())
                {
                    var destPath = FoldersHelper.GetPathAbsolut(FolderType.MenuIcons, photo.PhotoName);
                                
                    File.Move(tempPath, destPath);
                                
                    MenuService.UpdateMenuItemIcon(_menuItem.MenuItemID, photo.PhotoName);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message, ex);
            }
        }
    }
}
