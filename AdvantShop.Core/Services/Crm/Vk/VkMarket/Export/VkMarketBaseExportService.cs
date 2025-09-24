using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Crm.Vk.VkMarket.Models;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using VkNet;
using VkNet.Exception;
using VkNet.Model.Attachments;
using Image = System.Drawing.Image;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket.Export
{
    public class VkMarketBaseExportService
    {
        #region Ctor
        
        protected readonly VkMarketApiService _apiService;
        protected readonly VkCategoryService _categoryService;
        protected readonly VkProductService _productService;

        protected readonly VkCurrentMarketExportSettings _settings;

        protected List<VkResponsePropertyItem> _properties;
        private long _vkPropertyColorId;
        private long _vkPropertySizeId;

        public VkMarketBaseExportService()
        {
            _apiService = new VkMarketApiService();

            _categoryService = new VkCategoryService();
            _productService = new VkProductService();
            
            _settings = new VkCurrentMarketExportSettings()
            {
                ExportUnavailableProducts = VkMarketExportSettings.ExportUnavailableProducts,
                ExportPreorderProducts = VkMarketExportSettings.ExportPreorderProducts,
                AddSizeAndColorInDescription = VkMarketExportSettings.AddSizeAndColorInDescription,
                AddSizeAndColorInName = VkMarketExportSettings.AddSizeAndColorInName,
                ShowDescription = VkMarketExportSettings.ShowDescription,
                AddLinkToSite = VkMarketExportSettings.AddLinkToSite,
                TextBeforeLinkToSite = VkMarketExportSettings.TextBeforeLinkToSite,
                OwnerId = -SettingsVk.Group.Id,
                SiteUrl = SettingsMain.SiteUrl.TrimEnd('/'),
                Currency = CurrencyService.GetCurrencyByIso3(VkMarketSettings.CurrencyIso3 ?? CurrencyService.CurrentCurrency.Iso3),
                ShowProperties = VkMarketExportSettings.ShowProperties,
                ConsiderMinimalAmount = VkMarketExportSettings.ConsiderMinimalAmount,
                ExportMode = VkMarketExportSettings.ExportMode,
                ExportAmount = VkMarketExportSettings.ExportAmount
            };
        }

        #endregion


        #region GetProducts

        protected IEnumerable<VkExportProductModel> GetProducts(int vkCategoryId)
        {
            return SQLDataAccess.Query<VkExportProductModel>(
                "Select top(1000) " +
                "p.ProductId, " +
                "p.Name, " +
                "p.UrlPath, " +
                "p.Description, " +
                "p.BriefDescription, " +
                "p.ArtNo as ProductArtNo, " +
                "p.Discount, " +
                "p.DiscountAmount, " +
                "p.AllowPreOrder, " +
                "p.Enabled, " +
                "p.MinAmount, " +
                "p.Unit, " +

                "o.OfferId, " +
                "o.ColorID, " +
                "o.SizeID, " +
                "o.ArtNo as OfferArtNo, " +
                "o.Price, " +
                "o.Main, " +
                "o.Amount, " +

                "o.Width, " +
                "o.Height, " +
                "o.Length, " +
                "o.Weight, " +

                "ColorName, " +
                "SizeName, " +
                "CurrencyValue, " +

                "VkProduct.Id as VkProductId, " +
                "VkProduct.MainPhotoId as VkMainPhotoId, " +
                "VkProduct.PhotoIds as VkPhotoIds, " +
                "VkProduct.CurrentVkProduct, " +
                "VkProduct.PhotosMapIds, " +

                "VkProduct.ItemGroupId " +

                "From Catalog.Product p " +
                "Left Join Catalog.ProductExt pExt on p.ProductId = pExt.ProductId " +
                "Left Join Catalog.Offer o On o.ProductID = pExt.ProductID " +
                "Inner Join Catalog.ProductCategories pc On pc.ProductID = p.ProductID and pc.Main = 1 " +
                "Left Join Catalog.Category cat On cat.CategoryId = pc.CategoryId " +

                "Left Join Catalog.Color On Color.ColorID = o.ColorID " +
                "Left Join Catalog.Size On Size.SizeID = o.SizeID " +
                "Left Join [Vk].VkProduct On VkProduct.OfferId = o.OfferId " +
                "Inner Join Catalog.Currency On Currency.CurrencyID = p.CurrencyID " +

                "Where (p.Enabled = 1 Or @exportNotAvailable = 1) " +
                "AND (o.Price > 0) " +
                "AND (o.Amount > 0 OR (@exportPreorder = 1 AND p.AllowPreOrder = 1) OR @exportNotAvailable = 1) " +

                "AND pc.CategoryId in (Select CategoryId From [Vk].VkCategory_Category Where VkCategoryId=@vkCategoryId) " +

                "Order By cat.SortOrder, pc.SortOrder, p.ProductId",

                new { vkCategoryId, exportNotAvailable = _settings.ExportUnavailableProducts, exportPreorder = _settings.ExportPreorderProducts });
        }
        
        protected int GetProductsCount(int vkCategoryId)
        {
            var count = SQLDataAccess.Query<int>(
                "Select Count(p.ProductId) " +
                "From Catalog.Product p " +
                "Left Join Catalog.ProductExt pExt on p.ProductId = pExt.ProductId " +
                "Left Join Catalog.Offer o On o.ProductID = pExt.ProductID " +
                "Inner Join Catalog.ProductCategories pc On pc.ProductID = p.ProductID and pc.Main = 1 " +
                "Left Join Catalog.Category cat On cat.CategoryId = pc.CategoryId " +
                "Left Join Catalog.Color On Color.ColorID = o.ColorID " +
                "Left Join Catalog.Size On Size.SizeID = o.SizeID " +
                "Left Join [Vk].VkProduct On VkProduct.OfferId = o.OfferId " +
                "Inner Join Catalog.Currency On Currency.CurrencyID = p.CurrencyID " +
                "Where (p.Enabled = 1 Or @exportNotAvailable = 1) " +
                "AND (o.Price > 0) " +
                "AND (o.Amount > 0 OR (@exportPreorder = 1 AND p.AllowPreOrder = 1) OR @exportNotAvailable = 1) " +

                "AND pc.CategoryId in (Select CategoryId From [Vk].VkCategory_Category Where VkCategoryId=@vkCategoryId) ",

                new { vkCategoryId, exportNotAvailable = _settings.ExportUnavailableProducts, exportPreorder = _settings.ExportPreorderProducts }).FirstOrDefault();

            return count > 1000 ? 1000 : count;
        }

        protected IEnumerable<VkExportProductModel> GetProductsForModeProductWithouOffers(int vkCategoryId)
        {
            return SQLDataAccess.Query<VkExportProductModel>(
                "Select top(1000) " +
                "p.ProductId, " +
                "p.Name, " +
                "p.UrlPath, " +
                "p.Description, " +
                "p.BriefDescription, " +
                "p.ArtNo as ProductArtNo, " +
                "p.Discount, " +
                "p.DiscountAmount, " +
                "p.AllowPreOrder, " +
                "p.Enabled, " +
                "p.MinAmount, " +
                "p.Unit, " +

                "o.OfferId, " +
                "o.ColorID, " +
                "o.SizeID, " +
                "o.ArtNo as OfferArtNo, " +
                "o.Price, " +
                "o.Amount, " +
                "o.Main, " +

                "o.Width, " +
                "o.Height, " +
                "o.Length, " +
                "o.Weight, " +

                "ColorName, " +
                "SizeName, " +
                "CurrencyValue, " +

                "VkProduct.Id as VkProductId, " +
                "VkProduct.MainPhotoId as VkMainPhotoId, " +
                "VkProduct.PhotoIds as VkPhotoIds, " +
                "VkProduct.CurrentVkProduct, " +
                "VkProduct.PhotosMapIds " +

                "From Catalog.Product p " +
                "Left Join Catalog.ProductExt pExt on p.ProductId = pExt.ProductId " +
                "Left Join Catalog.Offer o On o.ProductID = pExt.ProductID " +
                "Inner Join Catalog.ProductCategories pc On pc.ProductID = p.ProductID and pc.Main = 1 " +
                "Left Join Catalog.Category cat On cat.CategoryId = pc.CategoryId " +

                "Left Join Catalog.Color On Color.ColorID = o.ColorID " +
                "Left Join Catalog.Size On Size.SizeID = o.SizeID " +
                "Left Join [Vk].VkProduct On VkProduct.ProductId = p.ProductId " +
                "Inner Join Catalog.Currency On Currency.CurrencyID = p.CurrencyID " +

                "Where (p.Enabled = 1 Or @exportNotAvailable = 1) " +
                "AND (o.Main = 1 Or o.Main is null) " +
                "AND (o.Price > 0) " +
                "AND (o.Amount > 0 OR (@exportPreorder = 1 AND p.AllowPreOrder = 1) OR @exportNotAvailable = 1) " +

                "AND pc.CategoryId in (Select CategoryId From [Vk].VkCategory_Category Where VkCategoryId=@vkCategoryId) " +

                "Order By cat.SortOrder, pc.SortOrder, p.ProductId",

                new { vkCategoryId, exportNotAvailable = _settings.ExportUnavailableProducts, exportPreorder = _settings.ExportPreorderProducts });
        }
        
        protected int GetProductsCountForModeProductWithouOffers(int vkCategoryId)
        {
            var count = SQLDataAccess.Query<int>(
                "Select Count(p.ProductId) " +
                "From Catalog.Product p " +
                "Left Join Catalog.ProductExt pExt on p.ProductId = pExt.ProductId " +
                "Left Join Catalog.Offer o On o.ProductID = pExt.ProductID " +
                "Inner Join Catalog.ProductCategories pc On pc.ProductID = p.ProductID and pc.Main = 1 " +
                "Left Join Catalog.Category cat On cat.CategoryId = pc.CategoryId " +

                "Left Join Catalog.Color On Color.ColorID = o.ColorID " +
                "Left Join Catalog.Size On Size.SizeID = o.SizeID " +
                "Left Join [Vk].VkProduct On VkProduct.ProductId = p.ProductId " +
                "Inner Join Catalog.Currency On Currency.CurrencyID = p.CurrencyID " +

                "Where (p.Enabled = 1 Or @exportNotAvailable = 1) " +
                "AND (o.Main = 1 Or o.Main is null) " +
                "AND (o.Price > 0) " +
                "AND (o.Amount > 0 OR (@exportPreorder = 1 AND p.AllowPreOrder = 1) OR @exportNotAvailable = 1) " +

                "AND pc.CategoryId in (Select CategoryId From [Vk].VkCategory_Category Where VkCategoryId=@vkCategoryId) ",

                new { vkCategoryId, exportNotAvailable = _settings.ExportUnavailableProducts, exportPreorder = _settings.ExportPreorderProducts }).FirstOrDefault();

            return count > 1000 ? 1000 : count;
        }


        #endregion

        #region Properties and Variants

        protected void CreateColorSizeProperties()
        {
            if (_settings.ExportMode != VkExportMode.GrouppingOffers)
                return;

            _properties = _apiService.GetProperties();

            var propColor = _properties.Find(x => x.Title == SettingsCatalog.ColorsHeader || x.Title == "Цвет");

            _vkPropertyColorId = propColor == null
                ? _apiService.AddProperty(SettingsCatalog.ColorsHeader)
                : propColor.Id;

            var propSize = _properties.Find(x => x.Title == SettingsCatalog.SizesHeader || x.Title == "Размер");

            _vkPropertySizeId = propSize == null 
                ? _apiService.AddProperty(SettingsCatalog.SizesHeader) 
                : propSize.Id;

            if (propColor == null || propSize == null)
                _properties = _apiService.GetProperties();
        }

        protected string GetVariants(VkExportProductModel product)
        {
            var variantIds = "";
            VkResponsePropertyItemVariant variant = null;

            if (product.ColorId != 0)
            {
                if (!TryGetVariant(product.ColorName, true, out variant))
                {
                    var variantId = AddVariant(product.ColorName, "", true);
                    if (variantId != 0)
                    {
                        variantIds += variantId.ToString();
                        Thread.Sleep(50);
                    }
                }
                else
                {
                    variantIds += variant.Id.ToString();
                }
            }

            if (product.SizeId != 0)
            {
                if (!TryGetVariant(product.SizeName, false, out variant))
                {
                    var variantId = AddVariant(product.SizeName, "", false);
                    if (variantId != 0)
                    {
                        variantIds += (!string.IsNullOrEmpty(variantIds) ? "," : "") + variantId.ToString();
                        Thread.Sleep(50);
                    }
                }
                else
                {
                    variantIds += (!string.IsNullOrEmpty(variantIds) ? "," : "") + variant.Id.ToString();
                }
            }

            return variantIds;
        }

        protected bool TryGetVariant(string title, bool isColor, out VkResponsePropertyItemVariant variant)
        {
            variant = null;

            var id = isColor ? _vkPropertyColorId : _vkPropertySizeId;

            var prop = _properties.Find(x => x.Id == id);
            if (prop != null)
                variant = prop.Variants.FirstOrDefault(x => x.Title == title);

            return variant != null;
        }

        protected long AddVariant(string title, string value, bool isColor)
        {
            var id = isColor ? _vkPropertyColorId : _vkPropertySizeId;

            var prop = _properties.Find(x => x.Id == id);

            if (prop != null && prop.Variants.Count < 30) // сейчас в Вк ограничение на 30 вариантов у св-ва
            {
                var variantId = _apiService.AddPropertyVariant(prop.Id, title, value);

                prop.Variants.Add(new VkResponsePropertyItemVariant() {Id = variantId, Title = title, Value = value});

                return variantId;
            }
            return 0;
        }

        #endregion

        #region Photo

        public List<VkPhotoMap> AddUpdatePhotos(VkExportProductModel product, VkApi vk, List<VkPhotoMap> photosMap)
        {
            var photos = PhotoService.GetPhotos<ProductPhoto>(product.ProductId, PhotoType.Product);
            if (photos.Count == 0)
                return new List<VkPhotoMap>();

            if (!product.Main && product.ColorId != 0 && photos.Any(x => x.ColorID == product.ColorId))
            {
                photos = photos.Where(x => x.ColorID == product.ColorId).ToList();
            }

            var photo = photos.FirstOrDefault(x => x.ColorID == product.ColorId) ?? photos.FirstOrDefault();
            if (photo == null)
                return new List<VkPhotoMap>();

            if (photosMap == null)
                photosMap = new List<VkPhotoMap>();

            var photoMapIds = new List<VkPhotoMap>();

            try
            {
                var groupId = _settings.OwnerId < 0 ? -_settings.OwnerId : _settings.OwnerId;
                
                if (photosMap.Count > 0 && photosMap[0].StorePhotoId == photo.PhotoId)
                {
                    photoMapIds.Add(photosMap[0]);
                }
                else
                {
                    var photoId = AddPhoto(photo, true, vk, groupId);
                    if (photoId != 0)
                        photoMapIds.Add(new VkPhotoMap() {StorePhotoId = photo.PhotoId, VkPhotoId = photoId});
                }

                foreach (var ph in photos.Where(x => x.PhotoId != photo.PhotoId)
                                         .OrderByDescending(x => x.ColorID == product.ColorId)
                                         .ThenByDescending(x => x.ColorID != null)
                                         .Take(4))
                {
                    var photoMap = photosMap.Find(x => x.StorePhotoId == ph.PhotoId);
                    if (photoMap != null)
                    {
                        photoMapIds.Add(photoMap);
                    }
                    else
                    {
                        var isMain = photoMapIds.Count == 0;

                        var photoId = AddPhoto(ph, isMain, vk, groupId);
                        if (photoId != 0)
                            photoMapIds.Add(new VkPhotoMap() {StorePhotoId = ph.PhotoId, VkPhotoId = photoId});
                    }
                }

                // delete not used
                foreach (var phMap in photosMap)
                {
                    if (!photoMapIds.Any(x => x.StorePhotoId == phMap.StorePhotoId))
                    {
                        _apiService.DeletePhoto(vk, phMap.VkPhotoId, groupId);
                    }
                }
            }
            catch (VkApiException ex)
            {
                VkMarketExportState.WriteLog(product.ProductArtNo + " " + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }

            return photoMapIds;
        }

        private long AddPhoto(ProductPhoto photo, bool mainPhoto, VkApi vk, long groupId)
        {
            try
            {
                var filePath = GetAbsolutePhotoPath(photo);
                if (filePath == null)
                    return 0;

                var vkPhoto = _apiService.AddPhoto(vk, groupId, mainPhoto, filePath);

                return vkPhoto == null || vkPhoto.Id == null ? 0 : vkPhoto.Id.Value;
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
            }
            return 0;
        }
        

        private string GetAbsolutePhotoPath(ProductPhoto photo)
        {
            var photoName = photo.PhotoName;
            var photoPath = FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Big, photo.PhotoName);

            if (photo.PhotoName.Contains("://"))
            {
                if (photo.PhotoName.Contains("cs71.advantshop.net"))  // https://cs71.advantshop.net/15705.jpg
                {
                    photoName = photoPath.Split('/').LastOrDefault();
                    photoPath = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoName);

                    var name = photo.PhotoName.Split('/').LastOrDefault();
                    var url = photo.PhotoName.Replace(name, "") + "pictures/product/big/" + name.Replace(".", "_big.");

                    if (!FileHelpers.DownloadRemoteImageFile(url, photoPath))
                        return null;

                    photoName = photoName.Replace(".", "_tmp.");
                }
                else
                {
                    photoName = Guid.NewGuid() + "_" + photo.PhotoName.Split('?')[0].Split('/').LastOrDefault();
                    photoPath = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoName);

                    if (!FileHelpers.DownloadRemoteImageFile(photo.PhotoName, photoPath))
                        return null;

                    photoName = photoName.Replace(".", "_tmp.");
                }
            }
            
            var tempPhoto = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoName);
            FileHelpers.DeleteFile(tempPhoto);

            using (var image = Image.FromFile(photoPath))
            {
                // если одна из сторон < 400px или соотношение сторон больше чем 1:12
                if (image.Width < 400 || image.Height < 400 || image.Width / image.Height >= 12 || image.Height / image.Width >= 12)
                {
                    if (Resize(image, tempPhoto))
                        return tempPhoto;

                    return null;
                }
            }

            return photoPath;
        }

        private bool Resize(Image image, string resultPath)
        {
            var max = image.Width > image.Height ? image.Width : image.Height;
            if (max < 400)
                max = 400;

            try
            {
                using (var img = new Bitmap(image))
                using (var result = new Bitmap(max, max))
                {
                    result.MakeTransparent();
                    using (var graphics = Graphics.FromImage(result))
                    {
                        graphics.Clear(System.Drawing.Color.White);
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.DrawImage(img, (max - img.Width) / 2, (max - img.Height) / 2, img.Width, img.Height);

                        graphics.Flush();
                        using (var stream = new FileStream(resultPath, FileMode.CreateNew))
                        {
                            result.Save(stream, ImageFormat.Jpeg);
                            stream.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Warn(ex);
                return false;
            }
            return true;
        }

        #endregion

        #region Description

        protected string GetDescription(VkExportProductModel product)
        {
            var description = "";

            if (_settings.AddSizeAndColorInDescription)
            {
                var offers = OfferService.GetProductOffers(product.ProductId)
                    .Where(x => (x.BasePrice > 0 && x.Amount > 0) || product.AllowPreOrder || _settings.ExportUnavailableProducts)
                    .ToList();

                if (offers.Count > 0)
                {
                    var colors = offers.Where(x => x.ColorID != null && x.Color != null).Select(x => x.Color.ColorName).Distinct().ToList();
                    if (colors.Count > 0)
                        description += SettingsCatalog.ColorsHeader + ": " + string.Join(", ", colors) + "\r\n";

                    var sizes = offers.Where(x => x.SizeID != null && x.Size != null).Select(x => x.Size.SizeName).Distinct().ToList();
                    if (sizes.Count > 0)
                        description += SettingsCatalog.SizesHeader + ": " + string.Join(", ", sizes) + "\r\n";
                }
            }

            if (_settings.AddLinkToSite == AddLinkToSiteMode.Top)
                description += _settings.TextBeforeLinkToSite + " " + GetLink(product) + "\r\n";

            switch (_settings.ShowDescription)
            {
                case ShowDescriptionMode.Full:
                    description += RemoveHtml(product.Description) + "\r\n";
                    break;
                case ShowDescriptionMode.Short:
                    description += RemoveHtml(product.BriefDescription) + "\r\n";
                    break;
            }

            if (_settings.ShowProperties)
                description += GetProperties(product) + "\r\n";

            if (_settings.AddLinkToSite == AddLinkToSiteMode.Bottom)
                description += "\r\n" + _settings.TextBeforeLinkToSite + " " + GetLink(product) + "\r\n";
            
            if (description.Length > 3600)
                description = description.Substring(0, 3600);

            return description;
        }

        private string RemoveHtml(string val)
        {
            if (string.IsNullOrWhiteSpace(val))
                return "";

            return
                StringHelper.RemoveHTML(
                    val.Replace("\r\n", "")
                        .Replace("</div>", "</div>\r\n")
                        .Replace("</p>", "</p>\r\n")
                        .Replace("</li>", "</li>\r\n")
                        .Replace("<br/>", "<br/>\r\n")
                        .Replace("<br />", "<br />\r\n")
                        .Replace("<br>", "<br>\r\n"));
        }

        protected string GetLink(VkExportProductModel product)
        {
            var suffix = "";
            if (!product.Main)
            {
                if (product.ColorId != 0)
                    suffix = "?color=" + product.ColorId;

                if (product.SizeId != 0)
                    suffix += (!string.IsNullOrEmpty(suffix) ? "&" : "?") + "size=" + product.SizeId;
            }
            return _settings.SiteUrl + "/products/" + product.UrlPath + suffix;
        }

        private string GetProperties(VkExportProductModel product)
        {
            var properties = PropertyService.GetPropertyValuesByProductId(product.ProductId);
            if (properties != null)
            {
                var sb = new StringBuilder();

                foreach (var propertyValue in properties.Where(x => x.Property.UseInDetails))
                {
                    sb.AppendFormat("{0}: {1}\r\n", 
                        propertyValue.Property.NameDisplayed ?? propertyValue.Property.Name,
                        propertyValue.Value);
                }

                return sb.ToString();
            }
            return "";
        }

        #endregion

        protected decimal GetPrice(VkExportProductModel product, float discountPrice)
        {
            var price = product.Price - discountPrice;
            price = _settings.ConsiderMinimalAmount && product.MinAmount.HasValue 
                ? price * product.MinAmount.Value 
                : price;

            return (decimal)PriceService.RoundPrice(price, _settings.Currency, product.CurrencyValue);
        }

        protected decimal GetOldPrice(VkExportProductModel product)
        {
            var price = product.Price;
            price = _settings.ConsiderMinimalAmount && product.MinAmount.HasValue
                ? price * product.MinAmount.Value
                : price;

            return (decimal)PriceService.RoundPrice(price, _settings.Currency, product.CurrencyValue);
        }

        protected void CheckCategoryExistInVk(VkCategory vkCategory, List<MarketAlbum> albums, VkApi vk)
        {
            try
            {
                var exist = albums != null && albums.Find(x => x.Id != null && x.Id == vkCategory.VkId) != null;
                if (exist)
                    return;

                var albumId = _apiService.AddAlbum(vkCategory.Name, vk);
                if (albumId != 0)
                {
                    vkCategory.VkId = albumId;

                    _categoryService.UpdateDb(vkCategory);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }


        protected void CheckGroupingApi(VkApi vk)
        {
            if (_settings.ExportMode != VkExportMode.GrouppingOffers)
                return;

            if (VkMarketSettings.IsGroupingApi)
                return;

            var groupId = -SettingsVk.Group.Id;

            foreach (var p in _productService.GetList())
            {
                _apiService.DeleteProduct(vk, groupId, p.Id);
                _productService.Delete(p.Id);
            }

            VkMarketSettings.IsGroupingApi = true;
        }
    }
}
