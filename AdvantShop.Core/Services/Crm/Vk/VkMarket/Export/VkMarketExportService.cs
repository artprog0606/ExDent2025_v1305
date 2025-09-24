using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.Vk.VkMarket.Models;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using Newtonsoft.Json;
using VkNet;
using VkNet.Exception;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket.Export
{
    public class VkMarketExportService : VkMarketBaseExportService
    {
        private Dictionary<long, bool> _productInVk;
        private int _captchaNeededExceptionCount;

        public VkMarketExportService()
        {
        }

        public bool StartExport()
        {
            if (VkMarketExportState.IsRun)
                return false;

            VkMarketExportState.Start();
            var result = Export();
            VkMarketExportState.Stop();

            Track.TrackService.TrackEvent(!Track.TrackService.TrackEventIsCommitted(Track.ETrackEvent.Shop_Vk_FirstExportProducts)
                ? Track.ETrackEvent.Shop_Vk_FirstExportProducts
                : Track.ETrackEvent.Shop_Vk_ExportProducts);

            VkMarketExportState.DeleteExpiredLogs();

            return result;
        }

        private bool Export()
        {
            try
            {
                if (VkProgress.IsProcessing())
                    return false;
                
                VkProgress.Start();
                
                var vk = _apiService.Auth();
                if (vk == null)
                {
                    VkMarketExportState.WriteLog("Не удалось авторизоваться в vk. Пересоздайте подключение к ВКонтакте заново.");
                    return false;
                }
                
                _captchaNeededExceptionCount = 0;

                var categories = _categoryService.GetList();
                var albums = _apiService.GetAllAlbums(vk);

                var totalProductsCount = categories.Sum(x =>
                    _settings.ExportMode == VkExportMode.GrouppingOffers
                        ? GetProductsCount(x.Id)
                        : GetProductsCountForModeProductWithouOffers(x.Id));

                CheckGroupingApi(vk);
                CreateColorSizeProperties();

                VkProgress.Set(totalProductsCount);

                var list = new List<VkProductSimple>();

                foreach (var vkCategory in categories)
                {
                    CheckCategoryExistInVk(vkCategory, albums, vk);

                    var ids = new List<long>();
                    var products =
                        _settings.ExportMode == VkExportMode.GrouppingOffers
                            ? GetProducts(vkCategory.Id).ToList()
                            : GetProductsForModeProductWithouOffers(vkCategory.Id).ToList();

                    FillsProductExistInVk(vk, products);

                    foreach (var product in products)
                    {
                        if (!VkMarketExportState.IsRun)
                            return false;

                        try
                        {
                            var vkProduct = ExportProduct(product, vk, vkCategory);
                            if (vkProduct != null)
                                list.Add(new VkProductSimple(vkProduct));
                        }
                        catch (VkApiException vkEx)
                        {
                            Debug.Log.Warn(vkEx);
                            VkMarketExportState.WriteLog($"Товар {product.ProductArtNo} \"{product.Name}\" не обработан: {(vkEx.ErrorCode != 0 ? vkEx.ErrorCode + " " : "")}{vkEx.Message}");

                            if (vkEx.Message != null)
                            {
                                var error = vkEx.Message.ToLower();
                                
                                if (error.Contains("captcha needed"))
                                {
                                    _captchaNeededExceptionCount++;

                                    if (_captchaNeededExceptionCount == 5)
                                    {
                                        VkProgress.Error(LocalizationService.GetResource("Core.VkMarket.Export.CaptchaNeededException"));
                                        return false;
                                    }
                                }

                                if (error.Contains("too many requests per second"))
                                    Thread.Sleep(3000);

                                if (error.Contains("flood control: too much captcha requests"))
                                    return false;
                                
                                if (error.Contains("too many items in album"))
                                    break;
                            }
                        }
                        catch (BlException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            Debug.Log.Warn(ex);
                            VkMarketExportState.WriteLog($"Ошибка при экспорте товара {product.ProductArtNo}: {ex.Message}");
                        }

                        if (product.VkProductId != 0)
                            ids.Add(product.VkProductId);
                        
                        VkProgress.Inc();
                    }

                    _productService.DeleteByAlbumAndNotInList(vk, vkCategory.VkId, ids);
                }
                _productService.DeleteByNotExistAlbum(vk);


                // group products
                GroupProducts(list);                
            }
            catch (Exception ex)
            {
                Debug.Log.Info(ex);
                VkMarketExportState.WriteLog("Ошибка при экспорте");

                return false;
            }
            finally
            {
                VkProgress.Stop();
                FileHelpers.DeleteFilesFromImageTempInBackground();
            }

            return true;
        }


        private VkProduct ExportProduct(VkExportProductModel product, VkApi vk, VkCategory vkCategory)
        {
            var name = product.Name +
                        (!product.Main && _settings.AddSizeAndColorInName
                            ? (!string.IsNullOrEmpty(product.ColorName) ? " " + product.ColorName : "") +
                              (!string.IsNullOrEmpty(product.SizeName) ? " " + product.SizeName : "")
                            : "") +
                        (_settings.ConsiderMinimalAmount && product.MinAmount.HasValue
                            ? " - " + product.MinAmount.Value + " " + (product.Unit.IsNullOrEmpty() ? "шт." : product.Unit)
                            : "");
           
            if (name.Length >= 90)
                name = name.Substring(0, 90);

            var discountPrice = product.Discount != 0 ? product.Price * product.Discount / 100 : product.DiscountAmount;
            var price = GetPrice(product, discountPrice);
            var oldPrice = discountPrice > 0 ? GetOldPrice(product) : 0;
            var description = GetDescription(product);

            var vkProduct = new VkProduct()
            {
                Id = product.VkProductId,
                OwnerId = _settings.OwnerId,
                ProductId = product.ProductId,
                OfferId = product.OfferId,
                Name = name,
                Description = description,
                Price = price > 0 ? price : 0.01m,
                OldPrice = oldPrice > 0 && oldPrice > price ? oldPrice : default(decimal?),
                Amount =
                    _settings.ExportAmount == VkExportAmount.Yes
                        ? (product.Amount > 0
                            ? product.Amount
                            : (_settings.ExportPreorderProducts && product.AllowPreOrder ? 100 : 0))
                        : default(float?),
                CategoryId = vkCategory.VkCategoryId,
                Deleted = false,
                AlbumId = vkCategory.VkId,

                Weight = product.Weight,
                Widht = product.Width,
                Height = product.Height,
                Length = product.Length,

                Sku = (product.OfferArtNo ?? "").Reduce(50)
            };

            if (_settings.ExportMode == VkExportMode.GrouppingOffers)
            {
                vkProduct.ItemGroupId = product.ItemGroupId;
                vkProduct.IsMainVariant = product.Main;

                if (product.SizeId != 0 || product.ColorId != 0)
                {
                    vkProduct.VariantIds = GetVariants(product);
                }
            }

            var isNew = product.VkProductId == 0;

            if (!isNew) // дополнительно проверям существование в вк (потому что его могли удалить в вк)
            {
                var exist = _productInVk.ContainsKey(product.VkProductId) && _productInVk[product.VkProductId];
                if (!exist)
                {
                    isNew = true;
                    _productService.Delete(product.VkProductId);
                }
            }

            if (isNew)
            {
                var vp = _productService.GetIdByOfferId(product.OfferId); // чтобы не выгружать дубли в вк
                if (vp != 0)
                    return null;

                var photoMapIds = AddUpdatePhotos(product, vk, null);
                if (photoMapIds == null || photoMapIds.Count == 0)
                {
                    VkMarketExportState.WriteLog("У товара {0} нет ни одной фотографии, поэтому он не будет загружен", product.ProductArtNo);
                    return null;
                }

                vkProduct.PhotosMapIds = JsonConvert.SerializeObject(photoMapIds);

                vkProduct.MainPhotoId = photoMapIds[0].VkPhotoId;
                vkProduct.PhotoIdsList = photoMapIds.Count > 1 ? photoMapIds.Skip(1).Select(x => x.VkPhotoId) : null;

                Thread.Sleep(100);

                vkProduct.Id = product.VkProductId = _apiService.AddProduct(vk, vkProduct);

                if (vkProduct.Id != 0)
                    _productService.Add(vkProduct);
            }
            else
            {
                var photoMap = !string.IsNullOrEmpty(product.PhotosMapIds)
                    ? JsonConvert.DeserializeObject<List<VkPhotoMap>>(product.PhotosMapIds)
                    : null;

                if (photoMap == null) // delete photos
                {
                    var photoIds =
                        !string.IsNullOrEmpty(product.VkPhotoIds)
                            ? product.VkPhotoIds.Split(',').Select(x => x.TryParseLong()).Where(x => x != 0)
                            : null;

                    if (photoIds != null)
                    {
                        var groupId = _settings.OwnerId < 0 ? -_settings.OwnerId : _settings.OwnerId;

                        foreach (var photoId in photoIds)
                            _apiService.DeletePhoto(vk, photoId, groupId);
                    }
                }
                
                var photoMapIds = AddUpdatePhotos(product, vk, photoMap);

                if (photoMapIds == null || photoMapIds.Count == 0)
                {
                    VkMarketExportState.WriteLog("У товара {0} нет ни одной фотографии, поэтому он не будет обновлен", product.ProductArtNo);
                    return null;
                }

                vkProduct.PhotosMapIds = JsonConvert.SerializeObject(photoMapIds);
                vkProduct.MainPhotoId = photoMapIds[0].VkPhotoId;
                vkProduct.PhotoIdsList = photoMapIds.Select(x => x.VkPhotoId).Take(4);

                if (IsVkProductChanged(vkProduct, product.CurrentVkProduct))
                {
                    Thread.Sleep(100);

                    if (_apiService.UpdateProduct(vk, vkProduct))
                    {
                        _productService.Update(vkProduct);
                    }
                }
            }

            Thread.Sleep(50);

            return vkProduct;
        }

        private void GroupProducts(List<VkProductSimple> products)
        {
            if (_settings.ExportMode != VkExportMode.GrouppingOffers)
                return;

            foreach (var product in products)
            {
                if (product.IsGrouped)
                    continue;

                var groupProducts = products.Where(x => x.ProductId == product.ProductId).ToList();
                if (groupProducts.Count < 2)
                    continue;

                if (!groupProducts.Any(x => x.ItemGroupId == null || x.ItemGroupId == 0))
                    continue;

                var maxVariantsCount = groupProducts.Select(x => x.VariantIds.Count).Max();

                // Группируем только те товары, у которых кол-во цвет/размер совпадает.
                // Может не совпадать, если не смогли добавить вариант (ограничение 30 вариантов на св-во)
                var ids = groupProducts.Where(x => x.VariantIds.Count == maxVariantsCount).Select(x => x.Id).Distinct().ToList();
                if (ids.Count < 2)
                    continue;

                var itemGroupId = groupProducts.Select(x => x.ItemGroupId).FirstOrDefault(x => x != null && x != 0);

                var groupId = _apiService.GroupItems(ids, itemGroupId);
                if (groupId == 0)
                    continue;

                foreach (var groupProduct in groupProducts)
                {
                    _productService.UpdateGroupId(groupProduct.Id, groupId);

                    groupProduct.IsGrouped = true;
                }
            }
        }

        /// <summary>
        /// Заполняем Dictionary long, bool для проверки существует ли товар в ВКонтакте 
        /// </summary>
        private void FillsProductExistInVk(VkApi vk, List<VkExportProductModel> products)
        {
            var ids = products.Select(x => x.VkProductId).Where(x => x != 0).ToList();
            if (ids.Count == 0)
                return;

            _productInVk = new Dictionary<long, bool>(ids.Count);
            foreach (var id in ids)
                _productInVk.Add(id, false);

            var offset = 0;

            while (true)
            {
                var itemIds = ids.Skip(offset).Take(100).Select(x => _settings.OwnerId + "_" + x).ToList();
                if (itemIds.Count == 0)
                    break;

                var markets = vk.Markets.GetById(itemIds).ToList();

                foreach (var market in markets)
                    if (market != null && _productInVk.ContainsKey(market.Id.Value))
                        _productInVk[market.Id.Value] = market.Availability == VkNet.Enums.ProductAvailability.Available;

                Thread.Sleep(300);
                offset += 100;
            }
        }

        /// <summary>
        /// Сравниваем текущий vkProduct с тем, что отправляли в прошлый раз
        /// </summary>
        private bool IsVkProductChanged(VkProduct vkProduct, string currentVkProductJson)
        {
            if (string.IsNullOrEmpty(currentVkProductJson))
                return true;

            try
            {
                var currentVkProduct = JsonConvert.DeserializeObject<VkProduct>(currentVkProductJson);

                if (currentVkProduct != null && Equals(vkProduct, currentVkProduct))
                    return false;
            }
            catch (Exception ex)
            {
                Debug.Log.Error("IsVkProductChanged: " + currentVkProductJson, ex);
            }

            return true;
        }
    }
}
