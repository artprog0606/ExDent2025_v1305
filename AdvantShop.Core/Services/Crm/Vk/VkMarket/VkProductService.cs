using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Crm.Vk.VkMarket.Models;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using VkNet;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket
{
    public class VkProductService
    {
        private readonly VkMarketApiService _apiService;

        public VkProductService()
        {
            _apiService = new VkMarketApiService();
        }

        public VkProduct Get(long id)
        {
            return SQLDataAccess
                .Query<VkProduct>("Select * From Vk.VkProduct Where Id=@id", new {id})
                .FirstOrDefault();
        }

        public long GetIdByOfferId(int offerId)
        {
            return SQLDataAccess
                .Query<long>("Select top(1) id From Vk.VkProduct Where OfferId=@offerId", new { offerId })
                .FirstOrDefault();
        }
        
        public List<VkProduct> GetList()
        {
            return SQLDataAccess.Query<VkProduct>("Select * From Vk.VkProduct").ToList();
        }

        public void Add(VkProduct product)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into Vk.VkProduct (Id, ProductId, OfferId, MainPhotoId, PhotoIds, AlbumId, PhotosMapIds, CurrentVkProduct, ModifiedDate) " +
                "Values (@Id, @ProductId, @OfferId, @MainPhotoId, @PhotoIds, @AlbumId, @PhotosMapIds, @CurrentVkProduct, getdate()) ",
                CommandType.Text,
                new SqlParameter("@Id", product.Id),
                new SqlParameter("@ProductId", product.ProductId),
                new SqlParameter("@OfferId", product.OfferId),
                new SqlParameter("@MainPhotoId", product.MainPhotoId),
                new SqlParameter("@PhotoIds", product.PhotoIdsList != null ? String.Join(",", product.PhotoIdsList) : ""),
                new SqlParameter("@AlbumId", product.AlbumId),
                new SqlParameter("@PhotosMapIds", product.PhotosMapIds ?? ""),
                new SqlParameter("@CurrentVkProduct", JsonConvert.SerializeObject(product)));
        }

        public void Update(VkProduct product)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update Vk.VkProduct " +
                "Set MainPhotoId=@MainPhotoId, PhotoIds=@PhotoIds, AlbumId=@AlbumId, PhotosMapIds=@PhotosMapIds, CurrentVkProduct=@CurrentVkProduct, ModifiedDate=getdate() " +
                "Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", product.Id),
                new SqlParameter("@MainPhotoId", product.MainPhotoId),
                new SqlParameter("@PhotoIds", product.PhotoIdsList != null ? String.Join(",", product.PhotoIdsList) : ""),
                new SqlParameter("@AlbumId", product.AlbumId),
                new SqlParameter("@PhotosMapIds", product.PhotosMapIds ?? ""),
                new SqlParameter("@CurrentVkProduct", JsonConvert.SerializeObject(product)));
        }

        public void UpdateGroupId(long id, long itemGroupId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update Vk.VkProduct Set ItemGroupId=@ItemGroupId Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", id),
                new SqlParameter("@ItemGroupId", itemGroupId));
        }

        public void Delete(long id)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From Vk.VkProduct Where Id=@Id", CommandType.Text,
                new SqlParameter("@Id", id));
        }

        public void DeleteByAlbum(long albumId)
        {
            var vkProducts =
                SQLDataAccess.Query<VkProduct>("Select Id, ProductId From Vk.VkProduct Where AlbumId=@albumId", new { albumId }).ToList();
            
            if (vkProducts.Count == 0)
                return;

            var vk = _apiService.Auth();
            var groupId = -SettingsVk.Group.Id;

            foreach (var p in vkProducts)
            {
                _apiService.DeleteProduct(vk, groupId, p.Id);
                Delete(p.Id);
            }
        }

        public void DeleteByAlbumAndNotInList(VkApi vk, long albumId, List<long> ids)
        {
            var vkProducts =
                SQLDataAccess.Query<VkProduct>(
                    "Select Id, ProductId From Vk.VkProduct " +
                    "Where AlbumId=@albumId " + (ids == null || ids.Count == 0 ? "" : "and Id not in (" + String.Join(", ", ids) + ")"),
                    new {albumId})
                    .ToList();

            if (vkProducts.Count == 0)
                return;
            
            var groupId = -SettingsVk.Group.Id;

            foreach (var p in vkProducts)
            {
                _apiService.DeleteProduct(vk, groupId, p.Id);
                Delete(p.Id);
            }
        }

        public void DeleteByNotExistAlbum(VkApi vk)
        {
            var vkProducts =
                SQLDataAccess.Query<VkProduct>(
                    "Select VkProduct.Id, VkProduct.ProductId From Vk.VkProduct " +
                    "Left Join Vk.VkCategory On VkCategory.VkId = VkProduct.AlbumId " +
                    "Where VkCategory.Id Is null")
                    .ToList();

            if (vkProducts.Count == 0)
                return;

            var groupId = -SettingsVk.Group.Id;

            foreach (var p in vkProducts)
            {
                _apiService.DeleteProduct(vk, groupId, p.Id);
                Delete(p.Id);
            }
        }

        public void DeleteAllProducts()
        {
            var vk = _apiService.Auth();
            var groupId = -SettingsVk.Group.Id;

            var productService = new VkProductService();

            foreach (var product in _apiService.GetProducts(vk, groupId, null))
            {
                try
                {
                    var p = productService.Get(product.Id.Value);
                    if (p != null && p.ItemGroupId != null && p.ItemGroupId != 0)
                    {
                        var res = _apiService.UnGroupItems(p.ItemGroupId.Value);
                        Thread.Sleep(350);
                    }

                    var result = _apiService.DeleteProduct(vk, groupId, product.Id.Value);
                    if (result && p != null)
                    {
                        productService.Delete(p.Id);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }
        }
    }
}
