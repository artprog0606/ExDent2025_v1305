using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Crm.Ok.OkMarket
{
    public class OkMarketApiService
    {
        public readonly string _applicationPublicKey;
        public readonly string _applicationAccessToken;
        public readonly string _applicationSessionSecretKey;
        public readonly string _groupId;

        public OkMarketApiService()
        {
            _applicationPublicKey = SettingsOk.ApplicationPublicKey;
            _applicationAccessToken = SettingsOk.ApplicationAccessToken;
            _applicationSessionSecretKey = SettingsOk.ApplicationSessionSecretKey;
            _groupId = SettingsOk.GroupId;
        }

        public bool IsActive()
        {
            return !string.IsNullOrEmpty(_applicationPublicKey) && !string.IsNullOrEmpty(_applicationAccessToken)
                                                                && !string.IsNullOrEmpty(_applicationSessionSecretKey);
        }

        public void DeActivate()
        {
            SettingsOk.ApplicationPublicKey = null;
            SettingsOk.ApplicationAccessToken = null;
            SettingsOk.ApplicationSessionSecretKey = null;
        }

        #region Group
        public OkMarketApiGetGroupResponse GetUserGroups()
        {
            try
            {
                var request = new OkMarketApiRequest
                {
                    application_key = _applicationPublicKey,
                    access_token = _applicationAccessToken,
                    format = "json",
                    method = "group.getUserGroupsV2"
                };
                request.UpdateSinature(_applicationSessionSecretKey);
                var result = RequestHelper.MakeRequest<OkMarketApiGetGroupResponse>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true), method: ERequestMethod.GET);

                return result;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
                return new OkMarketApiGetGroupResponse
                {
                    ErrorMsg = ex.Message
                };
            }

        }

        public List<OkMarketGroup> GetGroupsInfo(IEnumerable<string> groupIds)
        {
            try
            {
                var request = new OkMarketApiRequest
                {
                    application_key = _applicationPublicKey,
                    access_token = _applicationAccessToken,
                    format = "json",
                    method = "group.getInfo",
                    uids = groupIds.Aggregate((x, y) => x + "," + y),
                    fields = "UID,NAME,CATALOG_CREATE_ALLOWED,PRODUCT_CREATE_ALLOWED,BUSINESS"
                };
                request.UpdateSinature(_applicationSessionSecretKey);
                var result = RequestHelper.MakeRequest<List<OkMarketGroup>>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true), method: ERequestMethod.GET);
                if (result == null)
                    return new List<OkMarketGroup>();
                result.ForEach(x => x.GroupId = x.UID);
                return result.Where(x => x.Attributes != null && x.Attributes.Flags.Contains("pc") && x.Attributes.Flags.Contains("cca")).ToList();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
                return new List<OkMarketGroup>();
            }
        }
        #endregion

        #region Catalog
        public long? AddCatalog(string name, string photoId = null)
        {
            try
            {
                var request = new OkMarketApiRequest()
                {
                    application_key = _applicationPublicKey,
                    access_token = _applicationAccessToken,
                    format = "json",
                    gid = _groupId,
                    method = "market.addCatalog",
                    name = name
                };
                if (photoId != null)
                    request.photo_id = photoId;
                request.UpdateSinature(_applicationSessionSecretKey);
                var result = RequestHelper.MakeRequest<OkMarketApiAddCatalogResponse>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true),
                    method: ERequestMethod.POST, contentType: ERequestContentType.FormUrlencoded);

                if (!result.Success)
                {
                    Debug.Log.Warn("OKMarketAPI error: " + result.ErrorMsg);
                    //return null;
                    throw new BlException(result.FullErrorMsg);
                }
                return result.CatalogId;
            }
            catch (BlException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
                return null;
            }
        }

        public List<OkMarketCatalog> GetCatalogsByGroup(int count = 0)
        {
            try
            {
                var request = new OkMarketApiRequest()
                {
                    application_key = _applicationPublicKey,
                    gid = _groupId,
                    access_token = _applicationAccessToken,
                    format = "json",
                    method = "market.getCatalogsByGroup",
                    count = count == 0 || count > 100 ? 100 : count,
                    fields = "ID,NAME,SIZE",
                };
                request.UpdateSinature(_applicationSessionSecretKey);
                var result = RequestHelper.MakeRequest<OkMarketApiGetCatalogResponse>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true), method: ERequestMethod.GET);

                if (!result.Success)
                {
                    Debug.Log.Warn("OKMarketAPI error: " + result.ErrorMsg);
                    return new List<OkMarketCatalog>();
                }
                return result.Catalogs;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
                return new List<OkMarketCatalog>();
            }
        }

        public OkMarketCatalog GetCatalog(long catalogId)
        {
            try
            {
                var request = new OkMarketApiRequest()
                {
                    application_key = _applicationPublicKey,
                    gid = _groupId,
                    access_token = _applicationAccessToken,
                    format = "json",
                    method = "market.getCatalogsByIds",
                    catalog_ids = catalogId.ToString(),
                    fields = "ID,NAME,SIZE",
                };
                request.UpdateSinature(_applicationSessionSecretKey);
                var result = RequestHelper.MakeRequest<OkMarketApiGetCatalogResponse>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true), method: ERequestMethod.GET);

                if (!result.Success)
                {
                    Debug.Log.Warn("OKMarketAPI error: " + result.ErrorMsg);
                    return null;
                }
                return result.Catalogs.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
                return null;
            }
        }

        public void EditCatalog(long catalogId, string name)
        {
            try
            {
                var request = new OkMarketApiRequest()
                {
                    application_key = _applicationPublicKey,
                    gid = _groupId,
                    access_token = _applicationAccessToken,
                    format = "json",
                    method = "market.editCatalog",
                    catalog_id = catalogId.ToString(),
                    name = name
                };
                request.UpdateSinature(_applicationSessionSecretKey);
                var result = RequestHelper.MakeRequest<OkMarketApiBaseResponse>("https://api.ok.ru/fb.do", request.ToString("&", codeActtachmet: true),
                    method: ERequestMethod.POST, contentType: ERequestContentType.FormUrlencoded);

                if (!result.Success)
                {
                    Debug.Log.Warn("OKMarketAPI error: " + result.ErrorMsg);
                    throw new BlException(result.FullErrorMsg);
                }   
            }
            catch (BlException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
            }
        }

        public void DeleteCatalog(long catalogId)
        {
            try
            {
                var request = new OkMarketApiRequest()
                {
                    application_key = _applicationPublicKey,
                    gid = _groupId,
                    access_token = _applicationAccessToken,
                    format = "json",
                    method = "market.deleteCatalog",
                    catalog_id = catalogId.ToString(),
                    delete_products = true
                };
                request.UpdateSinature(_applicationSessionSecretKey);
                var result = RequestHelper.MakeRequest<OkMarketApiBaseResponse>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true),
                    method: ERequestMethod.POST, contentType: ERequestContentType.FormUrlencoded);

                if (!result.Success)
                {
                    Debug.Log.Warn("OKMarketAPI error: " + result.ErrorMsg);
                    throw new BlException(result.FullErrorMsg);
                }  
            }
            catch (BlException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
            }
        }

        public void DeleteCatalogWithPhotos(long catalogId)
        {
            try
            {
                foreach (var productId in GetProductIdsByCatalog(catalogId))
                {
                    DeleteProductWithPhotos(productId.TryParseLong());
                }
                DeleteCatalog(catalogId);
            }
            catch (BlException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
            }
            
        }
        #endregion

        #region Product
        public long? AddProduct(OkMarketProduct product, string catalogIds = null)
        {
            try
            {
                var request = new OkMarketApiRequest()
                {
                    application_key = _applicationPublicKey,
                    gid = _groupId,
                    access_token = _applicationAccessToken,
                    format = "json",
                    method = "market.add",
                    type = "GROUP_PRODUCT",
                    attachment = JsonConvert.SerializeObject(product),
                    catalog_ids = catalogIds
                };
                request.UpdateSinature(_applicationSessionSecretKey);
                var result = RequestHelper.MakeRequest<OkMarketApiAddProductResponse>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true),
                    method: ERequestMethod.POST, contentType: ERequestContentType.FormUrlencoded);

                if (!result.Success)
                {
                    Debug.Log.Warn("OKMarketAPI error: " + result.ErrorMsg);
                    return null;
                }
                return result.ProductId;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
                return null;
            }
        }

        public List<string> GetProductIdsByGroup(int count = 0)
        {
            try
            {
                var gettingByAnchor = count == 0;

                var request = new OkMarketApiRequest()
                {
                    application_key = _applicationPublicKey,
                    gid = _groupId,
                    access_token = _applicationAccessToken,
                    format = "json",
                    method = "market.getProducts",
                    tab = "PRODUCTS",
                    count = gettingByAnchor ? 100 : count,
                    fields = "ID"
                };
                request.UpdateSinature(_applicationSessionSecretKey);
                var result = RequestHelper.MakeRequest<OkMarketApiGetProductsResponse>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true), method: ERequestMethod.GET);

                if (!result.Success)
                {
                    Debug.Log.Warn("OKMarketAPI error: " + result.ErrorMsg);
                    return new List<string>();
                }
                if (result.Success && gettingByAnchor && result.HasMore)
                {
                    var products = new List<OkMarketProduct>();
                    while (result.Success && result.HasMore)
                    {
                        products.AddRange(result.ShortProducts);
                        request.anchor = result.Anchor;
                        request.UpdateSinature(_applicationSessionSecretKey);
                        result = RequestHelper.MakeRequest<OkMarketApiGetProductsResponse>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true), method: ERequestMethod.GET);
                    }
                    if (!result.Success)
                    {
                        Debug.Log.Warn("OKMarketAPI error: " + result.ErrorMsg);
                        return new List<string>();
                    }
                    if (result.ShortProducts != null)
                        products.AddRange(result.ShortProducts);
                    return products.Select(x => x.Id).ToList();
                }
                if (result.ShortProducts == null)
                    return new List<string>();
                return result.ShortProducts.Select(x => x.Id).ToList();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
                return new List<string>();
            }
        }

        public List<string> GetProductIdsByCatalog(long catalogId, int count = 0)
        {
            try
            {
                var gettingByAnchor = count == 0;

                var request = new OkMarketApiRequest()
                {
                    application_key = _applicationPublicKey,
                    access_token = _applicationAccessToken,
                    gid = _groupId,
                    format = "json",
                    method = "market.getByCatalog",
                    count = gettingByAnchor ? 100 : count,
                    fields = "ID",
                    catalog_id = catalogId.ToString()
                };
                request.UpdateSinature(_applicationSessionSecretKey);
                var result = RequestHelper.MakeRequest<OkMarketApiGetProductsResponse>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true), method: ERequestMethod.GET);

                if (!result.Success)
                {
                    Debug.Log.Warn("OKMarketAPI error: " + result.ErrorMsg);
                    //return new List<string>();
                    throw new BlException(result.FullErrorMsg);
                }
                if (result.Success && gettingByAnchor && result.HasMore)
                {
                    var products = new List<OkMarketProduct>();
                    while (result.Success && result.HasMore)
                    {
                        products.AddRange(result.ShortProducts);
                        request.anchor = result.Anchor;
                        request.UpdateSinature(_applicationSessionSecretKey);
                        result = RequestHelper.MakeRequest<OkMarketApiGetProductsResponse>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true), method: ERequestMethod.GET);
                    }
                    if (!result.Success)
                    {
                        Debug.Log.Warn("OKMarketAPI error: " + result.ErrorMsg);
                        return new List<string>();
                    }
                    if (result.ShortProducts != null)
                        products.AddRange(result.ShortProducts);
                    return products.Select(x => x.Id).ToList();
                }
                if (result.ShortProducts == null)
                    return new List<string>();
                return result.ShortProducts.Select(x => x.Id).ToList();
            }
            catch (BlException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
                return new List<string>();
            }
        }

        public List<OkMarketProduct> GetProductsByIds(long productIds)
        {
            try
            {
                var request = new OkMarketApiRequest()
                {
                    application_key = _applicationPublicKey,
                    access_token = _applicationAccessToken,
                    format = "json",
                    method = "market.getByIds",
                    product_ids = productIds.ToString(),
                    fields = "ID,MEDIA,MEDIA_TYPE,MEDIA_TEXT,MEDIA_PHOTO_REFS"
                };
                request.UpdateSinature(_applicationSessionSecretKey);
                var result = RequestHelper.MakeRequest<OkMarketApiGetProductsResponse>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true), method: ERequestMethod.GET);

                if (!result.Success)
                {
                    Debug.Log.Warn("OKMarketAPI error: " + result.ErrorMsg);
                    return new List<OkMarketProduct>();
                }
                return result.Products ?? new List<OkMarketProduct>();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
                return new List<OkMarketProduct>();
            }
        }

        public void EditProduct(long productId, OkMarketProduct product, string catalogIds = null)
        {
            try
            {
                var request = new OkMarketApiRequest()
                {
                    application_key = _applicationPublicKey,
                    access_token = _applicationAccessToken,
                    format = "json",
                    method = "market.edit",
                    product_id = productId.ToString(),
                    attachment = JsonConvert.SerializeObject(product),
                    catalog_ids = catalogIds
                };
                request.UpdateSinature(_applicationSessionSecretKey);
                var result = RequestHelper.MakeRequest<OkMarketApiBaseResponse>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true),
                    method: ERequestMethod.POST, contentType: ERequestContentType.FormUrlencoded);

                if (!result.Success)
                    Debug.Log.Warn("OKMarketAPI error: " + result.ErrorMsg);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
            }
        }

        public void SetProductStatus(long productId, OkMarketProductStatus status)
        {
            try
            {
                var request = new OkMarketApiRequest()
                {
                    application_key = _applicationPublicKey,
                    access_token = _applicationAccessToken,
                    format = "json",
                    method = "market.setStatus",
                    product_id = productId.ToString(),
                    product_status = status.ToString()
                };
                request.UpdateSinature(_applicationSessionSecretKey);
                RequestHelper.MakeRequest<OkMarketApiBaseResponse>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true),
                    method: ERequestMethod.POST, contentType: ERequestContentType.FormUrlencoded);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
            }
        }

        public void DeleteProduct(long productId)
        {
            try
            {
                var request = new OkMarketApiRequest()
                {
                    application_key = _applicationPublicKey,
                    access_token = _applicationAccessToken,
                    format = "json",
                    method = "market.delete",
                    product_id = productId.ToString()
                };
                request.UpdateSinature(_applicationSessionSecretKey);
                var result = RequestHelper.MakeRequest<OkMarketApiBaseResponse>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true),
                    method: ERequestMethod.POST, contentType: ERequestContentType.FormUrlencoded);
                if (!result.Success && !result.ErrorMsg.Contains("NOT_FOUND"))
                {
                    Debug.Log.Warn("OKMarketAPI error: " + result.ErrorMsg);
                    throw new BlException(result.FullErrorMsg);
                }
            }
            catch (BlException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
            }
        }

        public void DeleteProductWithPhotos(long productId)
        {
            try
            {
                var product = GetProductsByIds(productId).FirstOrDefault();
                if (product == null)
                    return;
                var productModel = new OkMarketProductModel(product);
                if (productModel.OkPhotoIdsList != null)
                    foreach (var photoId in productModel.OkPhotoIdsList)
                        DeletePhoto(photoId);
                DeleteProduct(productId);
            }
            catch (BlException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
            }
        }
        #endregion

        #region Photo
        private string GetAbsolutePhotoPath(Photo photo, out bool tempPhoto)
        {
            tempPhoto = false;
            var photoPath = FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Big, photo.PhotoName);

            if (photo.PhotoName.Contains("://"))
            {
                tempPhoto = true;
                string photoName;
                if (photo.PhotoName.Contains("cs71.advantshop.net"))  // https://cs71.advantshop.net/15705.jpg
                {
                    photoName = photoPath.Split('/').LastOrDefault();
                    photoPath = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoName);

                    var name = photo.PhotoName.Split('/').LastOrDefault();
                    var url = photo.PhotoName.Replace(name, "") + "pictures/product/big/" + name.Replace(".", "_big.");

                    if (!FileHelpers.DownloadRemoteImageFile(url, photoPath))
                        return null;
                }
                else
                {
                    photoName = Guid.NewGuid() + "_" + photo.PhotoName.Split('/').LastOrDefault();
                    photoPath = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoName);

                    if (!FileHelpers.DownloadRemoteImageFile(photo.PhotoName, photoPath))
                        return null;
                }
            }

            return photoPath;
        }

        public Dictionary<string, OkMarketPhotoToken> UploadPhotos(List<ProductPhoto> photos, string uploadUrl, bool usePlaceHolders = false, List<string> photoIds = null)
        {
            var result = new Dictionary<string, OkMarketPhotoToken>();

            if (usePlaceHolders == true && (photoIds.Count == 0 && photos.Count != photoIds.Count))
            {
                return result;
            }

            try
            {
                using (var wc = new WebClient())
                {
                    var i = 0;
                    foreach (var photo in photos)
                    {
                        var filePath = GetAbsolutePhotoPath(photo, out var tempPhoto);
                        if (filePath == null)
                        {
                            return result;
                        }

                        var requestUrl = usePlaceHolders
                            ? uploadUrl.Replace("${photoIds}", photoIds[i]).Replace("+", "%2B")
                            : uploadUrl;
                        i++;

                        var response = Encoding.ASCII.GetString(wc.UploadFile(requestUrl, filePath));

                        if (tempPhoto)
                            FileHelpers.DeleteFile(filePath);

                        var desResponse = JsonConvert.DeserializeObject<OkMarketApiPhotoUploadResponse>(response);
                        if (desResponse != null && desResponse.Success)
                        {
                            var token = desResponse.Photos.FirstOrDefault();
                            result.Add(token.Key, token.Value);
                        }
                        else
                        {
                            return result;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
                return new Dictionary<string, OkMarketPhotoToken>();
            }
        }

        public string GetUploadUrl(out List<string> photoIds, int count = 1, bool usePlaceHolders = false)
        {
            try
            {
                var request = new OkMarketApiRequest()
                {
                    application_key = _applicationPublicKey,
                    access_token = _applicationAccessToken,
                    method = "photosV2.getUploadUrl",
                    format = "json",
                    gid = _groupId,
                    count = count,
                    placeholders = usePlaceHolders
                };
                request.UpdateSinature(_applicationSessionSecretKey);
                var result = RequestHelper.MakeRequest<OkMarketApiPhotoUploadResponse>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true), method: ERequestMethod.GET);

                if (!result.Success)
                {
                    Debug.Log.Warn("OKMarketAPI error: " + result.ErrorMsg);
                    photoIds = null;
                    return null;
                }
                photoIds = result.PhotoIds;
                return result.UploadUrl;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
                photoIds = null;
                return null;
            }
        }

        public string GetPhotoUrl(string photoId)
        {
            try
            {
                var request = new OkMarketApiRequest()
                {
                    application_key = _applicationPublicKey,
                    access_token = _applicationAccessToken,
                    method = "photos.getPhotoInfo",
                    format = "json",
                    gid = _groupId,
                    photo_id = photoId
                };
                request.UpdateSinature(_applicationSessionSecretKey);
                var result = RequestHelper.MakeRequest<OkMarketApiGetPhotoResponse>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true), method: ERequestMethod.GET);

                if (!result.Success)
                {
                    Debug.Log.Warn("OKMarketAPI error: " + result.ErrorMsg);
                    return null;
                }
                return result.Photo.PhotoUrl;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
                return null;
            }
        }

        public void DeletePhoto(string photoId)
        {
            try
            {
                var request = new OkMarketApiRequest()
                {
                    application_key = _applicationPublicKey,
                    access_token = _applicationAccessToken,
                    format = "json",
                    method = "photos.deletePhoto",
                    gid = _groupId,
                    photo_id = photoId
                };
                request.UpdateSinature(_applicationSessionSecretKey);
                var result = RequestHelper.MakeRequest<bool>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true),
                    method: ERequestMethod.POST, contentType: ERequestContentType.FormUrlencoded);
                if (!result)
                {
                    Debug.Log.Warn("OKMarketAPI error: [DeletePhoto]" + photoId);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
            }
        }
        #endregion

        #region Users
        public OkUser GetUserInfo(string userId)
        {
            try
            {
                var request = new OkMarketApiRequest()
                {
                    application_key = _applicationPublicKey,
                    access_token = _applicationAccessToken,
                    format = "json",
                    method = "users.getInfo",
                    uids = userId,
                    fields = "FIRST_NAME,LAST_NAME,PIC128X128"
                };
                request.UpdateSinature(_applicationSessionSecretKey);
                var result = RequestHelper.MakeRequest<List<OkMarketUser>>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true), method: ERequestMethod.GET);

                if (result == null || result.Count == 0)
                {
                    return null;
                }
                var user = result.FirstOrDefault();
                return new OkUser()
                {
                    Id = userId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Photo = user.Photo
                };
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
                return null;
            }
        }
        #endregion

        #region URL
        public string GetIdFromUrl(string url)
        {
            try
            {
                var request = new OkMarketApiRequest()
                {
                    application_key = _applicationPublicKey,
                    access_token = _applicationAccessToken,
                    format = "json",
                    method = "url.getInfo",
                    url = url
                };
                request.UpdateSinature(_applicationSessionSecretKey);
                var result = RequestHelper.MakeRequest<OKMarketApiGetIdFromUrlResponse>("https://api.ok.ru/fb.do", data: request.ToString("&", codeActtachmet: true), method: ERequestMethod.GET);

                if(result.Type != "UNKNOWN")
                {
                    return result.Id != 0 ? result.Id.ToString() : result.IdString;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
                return null;
            }
        }
        #endregion
    }
}