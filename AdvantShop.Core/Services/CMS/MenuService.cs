//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.CMS;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.CMS
{
    public class MenuService
    {
        private static void ClearMenuCache()
        {
            CacheManager.RemoveByPattern(CacheNames.MenuPrefix);
        }

        public static void AddMenuItem(AdvMenuItem mItem)
        {
            ClearMenuCache();
            mItem.MenuItemID = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO [CMS].[Menu] (MenuItemParentID, MenuItemName, MenuItemIcon, MenuItemUrlPath, MenuItemUrlType, SortOrder, ShowMode, Enabled, Blank, NoFollow, MenuType) " +
                "VALUES (@MenuItemParentID, @MenuItemName, @MenuItemIcon, @MenuItemUrlPath, @MenuItemUrlType, " +
                "(select isnull(max(SortOrder) + 10, 0) from [CMS].[Menu] where MenuType = @MenuType AND MenuItemParentID " + (mItem.MenuItemParentID == 0 ? "is null" : "= @MenuItemParentID")  + "), " + // sort order
                "@ShowMode, @Enabled, @Blank, @NoFollow, @MenuType); " +
                "SELECT scope_identity();",
                CommandType.Text,
                new SqlParameter("@MenuItemParentID", mItem.MenuItemParentID != 0 ? mItem.MenuItemParentID : (object)DBNull.Value),
                new SqlParameter("@MenuItemName", mItem.MenuItemName),
                new SqlParameter("@MenuItemIcon", mItem.MenuItemIcon.IsNotEmpty() ? mItem.MenuItemIcon : (object)DBNull.Value),
                new SqlParameter("@MenuItemUrlPath", mItem.MenuItemUrlPath),
                new SqlParameter("@MenuItemUrlType", mItem.MenuItemUrlType),
                new SqlParameter("@Blank", mItem.Blank),
                new SqlParameter("@ShowMode", mItem.ShowMode),
                new SqlParameter("@Enabled", mItem.Enabled),
                new SqlParameter("@NoFollow", mItem.NoFollow),
                new SqlParameter("@MenuType", mItem.MenuType));
        }

        private static AdvMenuItem GetMenuItemFromReader(SqlDataReader reader)
        {
            return new AdvMenuItem
            {
                MenuItemID = SQLDataHelper.GetInt(reader, "MenuItemID"),
                MenuItemParentID = SQLDataHelper.GetInt(reader, "MenuItemParentID"),
                MenuItemName = SQLDataHelper.GetString(reader, "MenuItemName"),
                MenuItemIcon = SQLDataHelper.GetString(reader, "MenuItemIcon"),
                MenuItemUrlPath = SQLDataHelper.GetString(reader, "MenuItemUrlPath"),
                MenuItemUrlType = (EMenuItemUrlType)SQLDataHelper.GetInt(reader, "MenuItemUrlType"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                ShowMode = (EMenuItemShowMode)SQLDataHelper.GetInt(reader, "ShowMode"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                Blank = SQLDataHelper.GetBoolean(reader, "Blank"),
                NoFollow = SQLDataHelper.GetBoolean(reader, "NoFollow"),
                MenuType = (EMenuType)SQLDataHelper.GetInt(reader, "MenuType")
            };
        }

        public static AdvMenuItem GetMenuItemById(int mItemId)
        {
            return SQLDataAccess.ExecuteReadOne<AdvMenuItem>(
                "SELECT TOP 1 * FROM [CMS].[Menu] WHERE MenuItemID = @MenuItemID",
                CommandType.Text, GetMenuItemFromReader,
                new SqlParameter("@MenuItemID", mItemId));
        }

        public static List<int> GetParentMenuItems(int mItemId)
        {
            return SQLDataAccess.ExecuteReadColumn<int>(
                "[CMS].[sp_GetParentMenuItemsByItemId]",
                CommandType.StoredProcedure, "MenuItemID",
                new SqlParameter("@MenuItemID", mItemId));
        }

        public static void UpdateMenuItem(AdvMenuItem mItem)
        {
            ClearMenuCache();
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [CMS].[Menu] SET MenuItemParentID=@MenuItemParentID, MenuItemName=@MenuItemName, MenuItemIcon=@MenuItemIcon, MenuItemUrlPath=@MenuItemUrlPath, " +
                "MenuItemUrlType=@MenuItemUrlType, SortOrder=@SortOrder, ShowMode=@ShowMode, Enabled=@Enabled, Blank=@Blank, Nofollow=@NoFollow " +
                "WHERE MenuItemID = @MenuItemID",
                CommandType.Text,
                new SqlParameter("@MenuItemID", mItem.MenuItemID),
                new SqlParameter("@MenuItemParentID", mItem.MenuItemParentID != 0 ? mItem.MenuItemParentID : (object)DBNull.Value),
                new SqlParameter("@MenuItemName", mItem.MenuItemName),
                new SqlParameter("@MenuItemIcon", mItem.MenuItemIcon.IsNotEmpty() ? mItem.MenuItemIcon : (object)DBNull.Value),
                new SqlParameter("@MenuItemUrlPath", mItem.MenuItemUrlPath),
                new SqlParameter("@MenuItemUrlType", mItem.MenuItemUrlType),
                new SqlParameter("@SortOrder", mItem.SortOrder),
                new SqlParameter("@ShowMode", mItem.ShowMode),
                new SqlParameter("@Enabled", mItem.Enabled),
                new SqlParameter("@Blank", mItem.Blank),
                new SqlParameter("@NoFollow", mItem.NoFollow));
        }

        public static void DeleteMenuItemById(int mItemId)
        {
            var item = GetMenuItemById(mItemId);
            if (item == null)
                return;

            DeleteMenuItem(item);
        }

        public static void DeleteMenuItem(AdvMenuItem mItem)
        {
            ClearMenuCache();
            DeleteMenuItemIcon(mItem);

            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [CMS].[Menu] WHERE MenuItemID = @MenuItemID",
                CommandType.Text, new SqlParameter("@MenuItemID", mItem.MenuItemID));
        }

        public static void DeleteMenuItemIcon(AdvMenuItem mItem)
        {
            ClearMenuCache();
            UpdateMenuItemIcon(mItem.MenuItemID, null);

            PhotoService.DeletePhotos(mItem.MenuItemID, PhotoType.MenuIcon);

            // old logic
            if (mItem.MenuItemIcon.IsNotEmpty())
                FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.MenuIcons, mItem.MenuItemIcon));
        }

        public static void DeleteMenuItemIconById(int mItemId)
        {
            var item = GetMenuItemById(mItemId);
            if (item == null)
                return;

            DeleteMenuItemIcon(item);
        }

        public static void UpdateMenuItemIcon(int mItemId, string menuItemIcon)
        {
            ClearMenuCache();
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [CMS].[Menu] SET MenuItemIcon=@MenuItemIcon WHERE MenuItemID = @MenuItemID",
                CommandType.Text,
                new SqlParameter("@MenuItemID", mItemId),
                new SqlParameter("@MenuItemIcon", menuItemIcon.IsNotEmpty() ? menuItemIcon : (object)DBNull.Value));
        }

        /// <summary>
        /// Возвращает сам parentId и child ids
        /// </summary>
        /// <returns></returns>
        public static List<int> GetAllChildIdByParent(int parentId, EMenuType type)
        {
            return SQLDataAccess.ExecuteReadColumn<int>(
                "[CMS].[sp_GetChildMenuItemByParent]",
                CommandType.StoredProcedure,
                "MenuItemID",
                new SqlParameter("@ParentId", parentId),
                new SqlParameter("@MenuType", (int)type));
        }

        public static List<AdvMenuItem> GetChildMenuItemsByParentId(int mItemParentId, EMenuType type)
        {
            return SQLDataAccess.ExecuteReadList<AdvMenuItem>(
                "SELECT p.*, (SELECT COUNT(MenuItemID) FROM [CMS].[Menu] AS c WHERE c.MenuItemParentID = p.MenuItemID and c.Enabled=1) as Child_Count " +
                "FROM [CMS].[Menu] as p WHERE MenuType = @MenuType AND " + 
                (mItemParentId == 0 ? "[MenuItemParentID] IS NULL " : "[MenuItemParentID] = " + mItemParentId) +
                " ORDER BY [SortOrder]",
                CommandType.Text,
                (reader) =>
                {
                    var mItem = GetMenuItemFromReader(reader);
                    if (mItem.MenuItemUrlType != EMenuItemUrlType.Custom && !mItem.MenuItemUrlPath.StartsWith("http:") && !mItem.MenuItemUrlPath.StartsWith("https:"))
                    {
                        mItem.MenuItemUrlPath = UrlService.GetUrl() + mItem.MenuItemUrlPath;
                    }
                    mItem.HasChild = SQLDataHelper.GetInt(reader, "Child_Count") > 0;
                    return mItem;
                },
                new SqlParameter("@MenuType", type));
        }

        public static List<MenuItemModel> GetMenuItems(int parentId, EMenuType type, EMenuItemShowMode showMode)
        {
            return SQLDataAccess.ExecuteReadList(

                "SELECT p.*, (SELECT Count(MenuItemID) FROM [CMS].[Menu] AS c WHERE c.MenuItemParentID = p.MenuItemID and c.Enabled=1) as Child_Count " +
                "FROM [CMS].[Menu] as p WHERE MenuType = @MenuType AND " +
                (parentId == 0 ? "[MenuItemParentID] IS NULL" : "[MenuItemParentID] = " + parentId) +
                " AND (ShowMode = 0 OR ShowMode = @ShowMode) AND Enabled = 1 ORDER BY [SortOrder]",
                CommandType.Text,
                reader =>
                {
                    var item = new MenuItemModel
                    {
                        ItemId = SQLDataHelper.GetInt(reader, "MenuItemID"),
                        ItemParentId = SQLDataHelper.GetInt(reader, "MenuItemParentID"),
                        Name = SQLDataHelper.GetString(reader, "MenuItemName"),
                        UrlPath = SQLDataHelper.GetString(reader, "MenuItemUrlPath"),
                        IconPath = SQLDataHelper.GetString(reader, "MenuItemIcon"),
                        Blank = SQLDataHelper.GetBoolean(reader, "Blank"),
                        NoFollow = SQLDataHelper.GetBoolean(reader, "NoFollow"),
                        MenuType = (EMenuType) SQLDataHelper.GetInt(reader, "MenuType"),
                        HasChild = SQLDataHelper.GetInt(reader, "Child_Count") > 0,
                        MenuItemUrlType = (EMenuItemUrlType) SQLDataHelper.GetInt(reader, "MenuItemUrlType"),
                        DisplaySubItems = false
                    };

                    return item;
                },
                new SqlParameter("@showMode", (int) showMode), 
                new SqlParameter("@MenuType", (int) type));
        }

        public static List<MenuItemModel> GetAllMenuItems(int parentId, EMenuType type)
        {
            return SQLDataAccess.ExecuteReadList(

                "SELECT p.*, (SELECT Count(MenuItemID) FROM [CMS].[Menu] AS c WHERE c.MenuItemParentID = p.MenuItemID) as Child_Count " +
                "FROM [CMS].[Menu] as p WHERE MenuType = @MenuType AND " +
                (parentId == 0 ? "[MenuItemParentID] IS NULL" : "[MenuItemParentID] = " + parentId) +
                " ORDER BY [SortOrder]",
                CommandType.Text,
                reader =>
                {
                    var item = new MenuItemModel
                    {
                        ItemId = SQLDataHelper.GetInt(reader, "MenuItemID"),
                        ItemParentId = SQLDataHelper.GetInt(reader, "MenuItemParentID"),
                        Name = SQLDataHelper.GetString(reader, "MenuItemName"),
                        UrlPath = SQLDataHelper.GetString(reader, "MenuItemUrlPath"),
                        IconPath = SQLDataHelper.GetString(reader, "MenuItemIcon"),
                        Blank = SQLDataHelper.GetBoolean(reader, "Blank"),
                        NoFollow = SQLDataHelper.GetBoolean(reader, "NoFollow"),
                        MenuType = (EMenuType)SQLDataHelper.GetInt(reader, "MenuType"),
                        HasChild = SQLDataHelper.GetInt(reader, "Child_Count") > 0,
                        DisplaySubItems = false
                    };

                    return item;
                },
                new SqlParameter("@MenuType", (int)type));
        }

        public static List<string> GetMenuIcons()
        {
            return SQLDataAccess.ExecuteReadColumn<string>("select MenuItemIcon FROM [CMS].[Menu] where MenuItemIcon<>'' and MenuItemIcon is not null", CommandType.Text, "MenuItemIcon");
        }

        public static List<MenuItemModel> GetMenuItems(EMenuType menuType, EMenuItemShowMode showMode, int cacheTime = 60)
        {
            var cacheName = CacheNames.GetMenuCacheObjectName(menuType, showMode);
            return CacheManager.Get(cacheName, cacheTime, () =>
            {
                var menuItems = GetMenuItems(0, menuType, showMode);

                foreach (var item in menuItems.Where(item => item.HasChild))
                    item.SubItems = GetMenuItems(item.ItemId, menuType, showMode);

                return menuItems;
            });
        }
    }
}