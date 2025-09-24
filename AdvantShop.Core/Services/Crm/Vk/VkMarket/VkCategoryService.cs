using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Crm.Vk.VkMarket.Import;
using AdvantShop.Core.Services.Crm.Vk.VkMarket.Models;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket
{
    public class VkCategoryService
    {
        private readonly VkMarketApiService _apiService;

        public VkCategoryService()
        {
            _apiService = new VkMarketApiService();
        }

        public List<VkCategory> GetList()
        {
            return SQLDataAccess.Query<VkCategory>("Select * From Vk.VkCategory Order by SortOrder").ToList();
        }

        public VkCategory Get(int id)
        {
            return SQLDataAccess.Query<VkCategory>("Select * From Vk.VkCategory Where Id=@id", new {id}).FirstOrDefault();
        }
        
        public VkCategory GetByVkId(long vkId)
        {
            return SQLDataAccess.Query<VkCategory>("Select * From Vk.VkCategory Where VkId=@vkId", new { vkId }).FirstOrDefault();
        }

        public void Add(VkCategory category)
        {
            if (category.VkId == 0)
                category.VkId = _apiService.AddAlbum(category.Name);
            
            category.Id =
                SQLDataAccess.ExecuteScalar<int>(
                    "Insert Into Vk.VkCategory (VkId, VkCategoryId, Name, SortOrder) Values (@VkId, @VkCategoryId, @Name, @SortOrder); " +
                    "Select scope_identity();", 
                    CommandType.Text,
                    new SqlParameter("@VkId", category.VkId),
                    new SqlParameter("@VkCategoryId", category.VkCategoryId),
                    new SqlParameter("@Name", category.Name ?? ""),
                    new SqlParameter("@SortOrder", category.SortOrder));
        }

        public void Update(VkCategory category)
        {
            var c = Get(category.Id);
            if (c != null && c.Name != category.Name)
                _apiService.UpdateAlbum(category.VkId, category.Name);

            UpdateDb(category);
        }

        public void UpdateDb(VkCategory category)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update Vk.VkCategory SET VkId=@VkId, VkCategoryId=@VkCategoryId, Name=@Name, SortOrder=@SortOrder Where Id=@Id", CommandType.Text,
                new SqlParameter("@Id", category.Id),
                new SqlParameter("@VkId", category.VkId),
                new SqlParameter("@VkCategoryId", category.VkCategoryId),
                new SqlParameter("@Name", category.Name ?? ""),
                new SqlParameter("@SortOrder", category.SortOrder));
        }

        public void Delete(int id, long vkId)
        {
            try
            {
                _apiService.DeleteAlbum(vkId);

                new VkProductService().DeleteByAlbum(vkId);

                SQLDataAccess.ExecuteNonQuery(
                    "Delete From Vk.VkCategory Where Id=@Id", CommandType.Text,
                    new SqlParameter("@Id", id));
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }


        public List<Category> GetLinkedCategories(int id)
        {
            return
                SQLDataAccess.Query<Category>(
                    "Select CategoryId, Name From Catalog.Category " +
                    "Where CategoryId in (Select CategoryId From Vk.VkCategory_Category Where VkCategoryId=@id)",
                    new { id })
                    .ToList();
        }

        public void AddLink(int categoryId, int id)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into Vk.VkCategory_Category (CategoryId, VkCategoryId) Values (@CategoryId, @VkCategoryId) ",
                CommandType.Text,
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@VkCategoryId", id));
        }

        public void RemoveLinks(int id)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From Vk.VkCategory_Category Where VkCategoryId=@VkCategoryId",
                CommandType.Text,
                new SqlParameter("@VkCategoryId", id));
        }


        #region Import

        public VkCategoryImport GetCategoryImport(long albumId)
        {
            return SQLDataAccess.Query<VkCategoryImport>("Select * From Vk.VkCategoryImport Where albumId=@albumId", new { albumId }).FirstOrDefault();
        }

        public void AddCategoryImport(int categoryId, long albumId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into Vk.VkCategoryImport (categoryId, albumId) Values (@categoryId, @albumId) ",
                CommandType.Text,
                new SqlParameter("@categoryId", categoryId),
                new SqlParameter("@albumId", albumId));
        }

        #endregion
    }
}
