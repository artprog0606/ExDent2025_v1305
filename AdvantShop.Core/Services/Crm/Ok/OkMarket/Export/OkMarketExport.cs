using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdvantShop.Core.Services.Crm.Ok.OkMarket.Export
{
    public class OkMarketExport
    {
        private readonly OkMarketApiService _apiService;

        private readonly OkCurrentMarketExportSettings _settings;

        public OkMarketExport()
        {
            _apiService = new OkMarketApiService();

            _settings = new OkCurrentMarketExportSettings()
            {
                Currency = CurrencyService.GetCurrencyByIso3(OkMarketExportSettings.CurrencyIso3 ?? CurrencyService.CurrentCurrency.Iso3),
                ExportDescription = OkMarketExportSettings.ExportDescription,
                ExportLinkToSite = OkMarketExportSettings.ExportLinkToSite,
                ExportProperties = OkMarketExportSettings.ExportProperties,
                ExportUnavailableProducts = OkMarketExportSettings.ExportUnavailableProducts,
                SiteUrl = SettingsMain.SiteUrl.TrimEnd('/'),
                SizeAndColorInDescription = OkMarketExportSettings.SizeAndColorInDescription,
                SizeAndColorInName = OkMarketExportSettings.SizeAndColorInName,
                UpdateProductPhotos = OkMarketExportSettings.UpdateProductPhotos,
                GroupId = SettingsOk.GroupId
            };
        }

        public bool StartExport()
        {
            if (OkMarketExportState.IsRun)
                return false;

            OkMarketExportState.Start();
            var result = Export();
            OkMarketExportState.Stop();

            Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Ok_ExportProducts);

            OkMarketExportState.DeleteExpiredLogs();

            return result;
        }

        public bool Export()
        {
            try
            {
                var groupsInfo = _apiService.GetGroupsInfo(new List<string> { _settings.GroupId });
                if (groupsInfo.Count < 1)
                {
                    OkMarketExportState.WriteLog("OK API error - не удалось получить информацию о группе");
                    return false;
                }
                _settings.IsBusinessGroup = groupsInfo.FirstOrDefault().Business != null ? groupsInfo.FirstOrDefault().Business.Value : false;

                var productsTotal = OkMarketService.GetExportProductsCount();
                OkExportProgress.Start(productsTotal + 1);

                var OkProductIds = _apiService.GetProductIdsByGroup().Select(x => x.TryParseLong()).ToList();
                var OkCatalogs = _apiService.GetCatalogsByGroup();

                foreach (var catalog in OkMarketService.GetCatalogList())
                {
                    CheckCatalogExistInOk(catalog, OkCatalogs);
                    foreach (var product in OkMarketService.GetProducts(catalog.Id))
                    {
                        var id = UploadProduct(product, catalog.OkCatalogId);
                        if (id != 0)
                        {
                            OkProductIds.Remove(id);
                        }

                        OkExportProgress.Inc();
                    }
                }

                DeleteProductsFromOk(OkProductIds);
                var categories = OkCatalogs.Select(x => x.OkCatalogId).ToList();
                DeleteCatalogsFromOk(categories);
                OkExportProgress.Inc();
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
                return false;
            }
        }

        private void CheckCatalogExistInOk(OkMarketCatalog catalog, List<OkMarketCatalog> OkCatalogs)
        {
            try
            {
                if (OkCatalogs != null)
                {
                    var OkCatalog = OkCatalogs.FirstOrDefault(x => x.OkCatalogId == catalog.OkCatalogId);
                    if (OkCatalog != null)
                    {
                        OkCatalogs.Remove(OkCatalog);
                        return;
                    }
                }

                var catalogId = _apiService.AddCatalog(catalog.Name);
                if (!catalogId.HasValue)
                    return;
                catalog.OkCatalogId = catalogId.Value;
                OkMarketService.UpdateCatalog(catalog);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
            }
            
        }

        public long UploadProduct(OkMarketProductModel product, long catalogId)
        {
            var name = product.Name +
                (_settings.SizeAndColorInName
                    ? (!string.IsNullOrEmpty(product.ColorName) ? " " + product.ColorName : "") +
                      (!string.IsNullOrEmpty(product.SizeName) ? " " + product.SizeName : "")
                    : "") +
                (product.MinAmount.HasValue 
                    ? " - " + product.MinAmount.Value + " " + (product.Unit.IsNullOrEmpty() ? "шт." : product.Unit)
                    : "");
            if (name.Length > 100)
                name = name.Substring(0, 100);

            product.Name = name;
            product.Price = GetPrice(product);
            product.Description = GetDescription(product);

            var isNew = product.OkProductId == 0;
            product.OkCatalogId = catalogId;
            if (!isNew)
            {
                var productId = _apiService.GetProductsByIds(product.OkProductId);
                if (productId == null || productId.Count == 0)
                {
                    isNew = true;
                    OkMarketService.DeleteProduct(product.ProductId, product.OkProductId);
                    product.OkPhotoIds = null;
                }
            }
            if (isNew)
            {
                var photoTokens = ExportPhotos(product);
                if (photoTokens == null)
                {
                    OkMarketExportState.WriteLog("Ошибка экспорта фотографий товара - " + product.Name);
                    return 0;
                }
                if (photoTokens.Count == 0 && string.IsNullOrEmpty(product.Description))
                {
                    OkMarketExportState.WriteLog("У продукта нет ни фотографий ни описания - " + product.Name);
                    return 0;
                }

                var productId = _apiService.AddProduct(new OkMarketProduct(product, photoTokens, _settings.IsBusinessGroup), catalogId.ToString());

                if (!productId.HasValue)
                {
                    OkMarketExportState.WriteLog("Ошибка экспорта товара - " + product.Name);
                    return 0;
                }

                product.OkProductId = productId.Value;

                var status = OkMarketProductStatus.ACTIVE;
                if (product.MinAmount.HasValue ? product.Amount < product.MinAmount.Value : product.Amount <= 0)
                    status = _settings.IsBusinessGroup ? OkMarketProductStatus.OUT_OF_STOCK : OkMarketProductStatus.SOLD;
                if (!product.Enabled)
                    status = OkMarketProductStatus.CLOSED;
                _apiService.SetProductStatus(product.OkProductId, status);
                if (photoTokens != null)
                {
                    var OkProducts = _apiService.GetProductsByIds(product.OkProductId);
                    if (OkProducts != null)
                        foreach (var OkProduct in OkProducts)
                        {
                            var photoIds = new List<string>();
                            var media = OkProduct.Media.FirstOrDefault(x => x.Type == "photo");
                            if (media != null && media.PhotoIds != null)
                                foreach (var id in media.PhotoIds)
                                    photoIds.Add(id.Split(':').Last());
                            product.OkPhotoIdsList = photoIds;
                        }
                }
                OkMarketService.AddProduct(product);
                return product.OkProductId;
            }
            else
            {
                if (_settings.UpdateProductPhotos)
                {
                    var photoTokens = ExportPhotos(product);
                    if (photoTokens == null)
                    {
                        OkMarketExportState.WriteLog("Ошибка экспорта фотографий товара - " + product.Name);
                        return 0;
                    }
                    if (photoTokens.Count == 0 && string.IsNullOrEmpty(product.Description))
                    {
                        OkMarketExportState.WriteLog("У продукта нет ни фотографий ни описания - " + product.Name);
                        return 0;
                    }
                    if (product.OkPhotoIdsList != null)
                    {
                        foreach (var photoId in product.OkPhotoIdsList)
                        {
                            _apiService.DeletePhoto(photoId);
                        }
                        product.OkPhotoIdsList = null;
                    }
                    _apiService.EditProduct(product.OkProductId, new OkMarketProduct(product, photoTokens, _settings.IsBusinessGroup), product.OkCatalogId.ToString());
                    if (photoTokens != null)
                    {
                        var OkProducts = _apiService.GetProductsByIds(product.OkProductId);
                        if (OkProducts != null)
                            foreach (var OKProduct in OkProducts)
                            {
                                var photoIds = new List<string>();
                                var media = OKProduct.Media.FirstOrDefault(x => x.Type == "photo");
                                if (media != null && media.PhotoIds != null)
                                    foreach (var id in media.PhotoIds)
                                        photoIds.Add(id.Split(':').Last());
                                product.OkPhotoIdsList = photoIds;
                            }
                    }
                    OkMarketService.UpdateProduct(product);
                }
                else
                {
                    if (product.OkPhotoIds == null && string.IsNullOrEmpty(product.Description))
                    {
                        OkMarketExportState.WriteLog("У продукта нет ни фотографий ни описания - " + product.Name);
                        return 0;
                    }
                    _apiService.EditProduct(product.OkProductId, new OkMarketProduct(product, businessGroup: _settings.IsBusinessGroup), product.OkCatalogId.ToString());
                }
                var status = OkMarketProductStatus.ACTIVE;
                if (product.MinAmount.HasValue ? product.Amount < product.MinAmount.Value : product.Amount <= 0)
                    status = _settings.IsBusinessGroup ? OkMarketProductStatus.OUT_OF_STOCK : OkMarketProductStatus.SOLD;
                if (!product.Enabled)
                    status = OkMarketProductStatus.CLOSED;
                _apiService.SetProductStatus(product.OkProductId, status);
                return product.OkProductId;
            }
        }

        private float GetPrice(OkMarketProductModel product)
        {
            var discountPrice = product.Discount != 0 ? product.Price * product.Discount / 100 : product.DiscountAmount;
            var price = product.Price - discountPrice;
            price = product.MinAmount.HasValue ? price * product.MinAmount.Value : price;
            var currency = CurrencyService.GetCurrencyByIso3(_settings.Currency.Iso3);
            product.CurrencyName = currency.Iso3;
            return PriceService.RoundPrice(price, currency, product.CurrencyValue);
        }

        private string GetDescription(OkMarketProductModel product)
        {
            var description = "";
            if (_settings.SizeAndColorInDescription)
            {
                var offers = OfferService.GetProductOffers(product.ProductId)
                    .Where(x => (x.BasePrice > 0 && x.Amount > 0) || _settings.ExportUnavailableProducts)
                    .ToList();

                if (offers.Count > 0)
                {
                    var colors = offers.Where(x => x.ColorID != null && x.Color != null).Select(x => x.Color.ColorName).Distinct().ToList();
                    if (colors.Count > 0)
                        description += SettingsCatalog.ColorsHeader + ": " + string.Join(", ", colors);

                    var sizes = offers.Where(x => x.SizeID != null && x.Size != null).Select(x => x.Size.SizeName).Distinct().ToList();
                    if (sizes.Count > 0)
                        description += (description == "" ? "" : "\n") + SettingsCatalog.SizesHeader + ": " + string.Join(", ", sizes);
                }
            }

            if (_settings.ExportLinkToSite == OkMarketAddLinkToSiteMode.Top)
                description += (description == "" ? "" : "\n\n") + GetLink(product);

            switch (_settings.ExportDescription)
            {
                case OkMarketShowDescriptionMode.Full:
                    description += !string.IsNullOrEmpty(product.Description) ? (description == "" ? "" : "\n\n") + RemoveHtml(product.Description) : "";
                    break;
                case OkMarketShowDescriptionMode.Short:
                    description += !string.IsNullOrEmpty(product.BriefDescription) ? (description == "" ? "" : "\n\n") + RemoveHtml(product.BriefDescription) : "";
                    break;
            }

            if (_settings.ExportProperties)
            {
                var properties = GetProperties(product);

                if (properties != "")
                {
                    properties = properties.Remove(properties.Length - 1);
                    description += (description == "" ? "" : "\n\n") + properties;
                }
            }

            if (_settings.ExportLinkToSite == OkMarketAddLinkToSiteMode.Bottom || description.Length < 10)
                description += (description == "" ? "" : "\n\n") + GetLink(product);

            if (description.Length > 3600)
                description = description.Substring(0, 3600);

            return description;
        }

        private string GetLink(OkMarketProductModel product)
        {
            var suffix = "";
            if (!product.Main)
            {
                if (product.ColorId != 0)
                    suffix = "?color=" + product.ColorId;

                if (product.SizeId != 0)
                    suffix += (!string.IsNullOrEmpty(suffix) ? "&" : "?") + "size=" + product.SizeId;
            }
            return SettingsMain.SiteUrl.TrimEnd('/') + "/products/" + product.UrlPath + suffix;
        }

        private string RemoveHtml(string val)
        {
            if (string.IsNullOrWhiteSpace(val))
                return "";

            var temp = StringHelper.RemoveHTML(
                    val.Replace("\r\n\t", "")
                        .Replace("\r\n", "")
                        .Replace("</div>", "</div>\n")
                        .Replace("</p>", "</p>\n")
                        .Replace("</li>", "</li>\n")
                        .Replace("<br />", "<br />\n")
                        );
            temp = Regex.Replace(temp, "\n{2,}", "\n");
            temp = Regex.Replace(temp, "\t{2,}", "\t");
            temp = Regex.Replace(temp, "(\n\t){2,}", "\n\t");
            temp = Regex.Replace(temp, "\n{1,}( )*$", "");
            return temp;
        }

        private string GetProperties(OkMarketProductModel product)
        {
            var properties = PropertyService.GetPropertyValuesByProductId(product.ProductId);
            if (properties != null)
            {
                var sb = new StringBuilder();

                foreach (var propertyValue in properties.Where(x => x.Property.UseInDetails))
                {
                    sb.AppendFormat("{0}: {1}\n", propertyValue.Property.Name, propertyValue.Value);
                }
                return sb.ToString();
            }
            return "";
        }

        /// <summary>
        /// Sending to the server top 5 photos of product sorted by colorID
        /// </summary>
        /// <returns>List of tokens of uploaded photos</returns>
        private List<string> ExportPhotos(OkMarketProductModel product)
        {
            var productPhotos = PhotoService.GetPhotos<ProductPhoto>(product.ProductId, PhotoType.Product);
            var photosToUpload = productPhotos
                                .OrderByDescending(x => x.ColorID == product.ColorId)
                                .ThenByDescending(x => x.ColorID != null)
                                .Take(5);

            if (photosToUpload.Count() == 0)
                return new List<string>();
            List<string> photoIds = null;
            var uploadUrl = _apiService.GetUploadUrl(out photoIds, photosToUpload.Count(), true);
            if (uploadUrl == null)
            {
                return null;
            }
            var photoTokens = _apiService.UploadPhotos(photosToUpload.ToList(), uploadUrl, true, photoIds);
            if (photoTokens == null)
            {
                return null;
            }
            return photoTokens.Select(x => x.Value.Token).ToList();
        }

        private void DeleteProductsFromOk(List<long> productIds)
        {
            foreach (var productId in productIds)
            {
                _apiService.DeleteProductWithPhotos(productId);
            }
        }

        private void DeleteCatalogsFromOk(List<long> catalogIds)
        {
            try
            {
                foreach (var catalogId in catalogIds)
                {
                    _apiService.DeleteCatalog(catalogId);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
            }
        }
    }
}