//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using Debug = AdvantShop.Diagnostics.Debug;

namespace AdvantShop.Catalog
{
    public class PhotoService
    {
        public static Photo GetPhoto(int photoId)
        {
            return SQLDataAccess.ExecuteReadOne<Photo>("SELECT TOP 1 * FROM [Catalog].[Photo] WHERE [PhotoID] = @PhotoID",
                CommandType.Text, GetPhotoFromReader, new SqlParameter("@PhotoID", photoId));
        }



        public static Photo GetProductPhoto(int productID, string originalName)
        {
            return
                SQLDataAccess.ExecuteReadOne<Photo>(
                    "SELECT top 1 * FROM [Catalog].[Photo] WHERE [objId] = @objId and Type=@Type and OriginName=@OriginName",
                    CommandType.Text, GetPhotoFromReader,
                    new SqlParameter("@objId", productID),
                    new SqlParameter("@Type", PhotoType.Product.ToString()),
                    new SqlParameter("OriginName", originalName));
        }



        public static ProductPhoto GetMainProductPhoto(int productId, int? colorId = null)
        {

            var photo = SQLDataAccess.ExecuteReadOne<ProductPhoto>(
                string.Format(
                    "SELECT top 1 * FROM [Catalog].[Photo] WHERE [objId] = @productId and type=@type {0} ORDER BY main desc, [PhotoSortOrder], PhotoID",
                    colorId != null ? "and (colorID=@colorID or colorID is Null)" : ""),
                CommandType.Text,
                reader => GetPhotoFromReader<ProductPhoto>(reader, PhotoType.Product),
                new SqlParameter("@productId", productId),
                new SqlParameter("@type", PhotoType.Product.ToString()),
                new SqlParameter("@colorID", colorId ?? (object)DBNull.Value));

            return photo ?? new ProductPhoto();
        }

        /// <summary>
        /// return list of photos by type
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Photo> GetPhotos(int objId, PhotoType type)
        {
            var list = SQLDataAccess.ExecuteReadIEnumerable<Photo>("SELECT * FROM [Catalog].[Photo] WHERE [objId] = @objId and type=@type  ORDER BY [PhotoSortOrder]",
                                                                    CommandType.Text, GetPhotoFromReader,
                                                                    new SqlParameter("@objId", objId),
                                                                    new SqlParameter("@type", type.ToString()));
            return list;
        }

        public static IEnumerable<Photo> GetAllPhotos(PhotoType type)
        {
            return
                SQLDataAccess.ExecuteReadIEnumerable("SELECT * FROM [Catalog].[Photo] WHERE type=@type", CommandType.Text,
                                                     GetPhotoFromReader, new SqlParameter("@type", type.ToString()));
        }

        public static IEnumerable<string> GetNamePhotos(int objId, PhotoType type, bool allNames = false)
        {
            if (allNames)
                return SQLDataAccess.ExecuteReadIEnumerable<string>("SELECT PhotoName FROM [Catalog].[Photo] WHERE type=@type",
                                                                    CommandType.Text, reader => SQLDataHelper.GetString(reader, "PhotoName"),
                                                                    new SqlParameter("@type", type.ToString()));

            return SQLDataAccess.ExecuteReadIEnumerable<string>("SELECT PhotoName FROM [Catalog].[Photo] WHERE [objId] = @objId and type=@type",
                                                                    CommandType.Text, reader => SQLDataHelper.GetString(reader, "PhotoName"),
                                                                    new SqlParameter("@objId", objId),
                                                                    new SqlParameter("@type", type.ToString()));
        }

        /// <summary>
        /// return count of photos by type
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int GetCountPhotos(int objId, PhotoType type)
        {
            if (objId == 0)
                return SQLDataAccess.ExecuteScalar<int>("SELECT Count(*) FROM [Catalog].[Photo] WHERE type=@type",
                                                                  CommandType.Text, new SqlParameter("@type", type.ToString()));

            var res = SQLDataAccess.ExecuteScalar<int>("SELECT Count(*) FROM [Catalog].[Photo] WHERE [objId] = @objId and type=@type",
                                                                    CommandType.Text, new SqlParameter("@objId", objId), new SqlParameter("@type", type.ToString()));
            return res;
        }

        public static Photo GetPhotoFromReader(SqlDataReader reader)
        {
            return new Photo(
                SQLDataHelper.GetInt(reader, "PhotoId"),
                SQLDataHelper.GetInt(reader, "ObjId"),
                (PhotoType)Enum.Parse(typeof(PhotoType), SQLDataHelper.GetString(reader, "Type"), true))
            {
                Description = SQLDataHelper.GetString(reader, "Description"),
                ModifiedDate = SQLDataHelper.GetDateTime(reader, "ModifiedDate"),
                PhotoName = SQLDataHelper.GetString(reader, "PhotoName"),
                PhotoNameSize1 = SQLDataHelper.GetString(reader, "PhotoNameSize1"),
                PhotoNameSize2 = SQLDataHelper.GetString(reader, "PhotoNameSize2"),
                OriginName = SQLDataHelper.GetString(reader, "OriginName"),
                PhotoSortOrder = SQLDataHelper.GetInt(reader, "PhotoSortOrder"),
                Main = SQLDataHelper.GetBoolean(reader, "Main"),
                ColorID = SQLDataHelper.GetNullableInt(reader, "ColorID"),
                PhotoCategoryId = SQLDataHelper.GetNullableInt(reader, "PhotoCategoryId")
            };
        }

        /// <summary>
        /// add new photo, return new photo new name
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public static string AddPhoto(Photo photo)
        {
            if (photo.Type == PhotoType.Product && SaasDataService.IsSaasEnabled &&
                GetCountPhotos(photo.ObjId, photo.Type) >= SaasDataService.CurrentSaasData.PhotosCount)
            {
                return string.Empty;
            }

            var photoTemp =
                 SQLDataAccess.ExecuteReadOne<Photo>("[Catalog].[sp_AddPhoto]", CommandType.StoredProcedure,
                     GetPhotoFromReader,
                     new SqlParameter("@ObjId", photo.ObjId),
                     new SqlParameter("@PhotoName", photo.PhotoName ?? (object)DBNull.Value),
                     new SqlParameter("@Description", photo.Description ?? string.Empty),
                     new SqlParameter("@OriginName", photo.OriginName ?? ""),
                     new SqlParameter("@Type", photo.Type.ToString()),
                     new SqlParameter("@Extension", Path.GetExtension(photo.OriginName) ?? ""),
                     new SqlParameter("@ColorID", photo.ColorID ?? (object)DBNull.Value),
                     new SqlParameter("@PhotoSortOrder", photo.PhotoSortOrder),
                     new SqlParameter("@PhotoNameSize1", !string.IsNullOrEmpty(photo.PhotoNameSize1) ? photo.PhotoNameSize1 : (object)DBNull.Value),
                     new SqlParameter("@PhotoNameSize2", !string.IsNullOrEmpty(photo.PhotoNameSize2) ? photo.PhotoNameSize2 : (object)DBNull.Value),
                     new SqlParameter("@PhotoCategoryId", photo.PhotoCategoryId ?? (object)DBNull.Value));


            photo.PhotoId = photoTemp.PhotoId;
            photo.PhotoName = photoTemp.PhotoName;

            return photoTemp.PhotoName;
        }

        // Должна называться WithPhotoName, т.к. OriginName и в обычной функции передается,
        // а тут именно PhotoName присваивается, а не генерируется
        public static int AddPhotoWithOrignName(Photo ph)
        {
            AddPhoto(ph);

            return ph.PhotoId;
        }

        public static string GetPathByPhotoId(int id)
        {
            return SQLDataAccess.ExecuteScalar<string>("SELECT [PhotoName] FROM [Catalog].[Photo] WHERE [PhotoId] = @PhotoId", CommandType.Text, new SqlParameter("@PhotoId", id));
        }

        public static Photo GetPhotoByObjId(int objId, PhotoType type)
        {
            return SQLDataAccess.ExecuteReadOne<Photo>("SELECT TOP 1 * FROM [Catalog].[Photo] WHERE [ObjId] = @ObjId and type=@type ORDER BY Main desc, [PhotoSortOrder]",
                                                        CommandType.Text, GetPhotoFromReader,
                                                        new SqlParameter("@ObjId", objId),
                                                        new SqlParameter("@type", type.ToString()));
        }

        public static T GetPhotoByObjId<T>(int objId, PhotoType type) where T : Photo, new()
        {
            var photo =
                SQLDataAccess.ExecuteReadOne<T>("SELECT TOP 1 *  FROM [Catalog].[Photo] WHERE [ObjId] = @ObjId and type=@type ORDER BY Main desc, [PhotoSortOrder]",
                    CommandType.Text,
                    reader => GetPhotoFromReader<T>(reader, type),
                    new SqlParameter("@ObjId", objId),
                    new SqlParameter("@type", type.ToString()));

            return photo ?? new T();
        }

        public static T GetPhoto<T>(int photoId, PhotoType type) where T : Photo, new()
        {
            var photo =
                SQLDataAccess.ExecuteReadOne<T>("SELECT TOP 1 * FROM [Catalog].[Photo] WHERE [PhotoId] = @PhotoId",
                    CommandType.Text,
                    reader => GetPhotoFromReader<T>(reader, type),
                    new SqlParameter("@PhotoId", photoId));

            return photo ?? new T();
        }

        public static List<T> GetPhotos<T>(int objId, PhotoType type) where T : Photo, new()
        {
            return
                SQLDataAccess.ExecuteReadList<T>(
                    "SELECT * FROM [Catalog].[Photo] WHERE [objId] = @objId and type=@type ORDER BY Main desc, [PhotoSortOrder]",
                    CommandType.Text,
                    reader => GetPhotoFromReader<T>(reader, type),
                    new SqlParameter("@objId", objId),
                    new SqlParameter("@type", type.ToString()));
        }

        public static T GetPhotoFromReader<T>(SqlDataReader reader, PhotoType type) where T : Photo, new()
        {
            return new T()
            {
                PhotoId = SQLDataHelper.GetInt(reader, "PhotoId"),
                ObjId = SQLDataHelper.GetInt(reader, "ObjId"),
                Type = type,

                Description = SQLDataHelper.GetString(reader, "Description"),
                ModifiedDate = SQLDataHelper.GetDateTime(reader, "ModifiedDate"),
                PhotoName = SQLDataHelper.GetString(reader, "PhotoName"),
                PhotoNameSize1 = SQLDataHelper.GetString(reader, "PhotoNameSize1", null),
                PhotoNameSize2 = SQLDataHelper.GetString(reader, "PhotoNameSize2", null),
                OriginName = SQLDataHelper.GetString(reader, "OriginName"),
                PhotoSortOrder = SQLDataHelper.GetInt(reader, "PhotoSortOrder"),
                Main = SQLDataHelper.GetBoolean(reader, "Main"),
                ColorID = SQLDataHelper.GetNullableInt(reader, "ColorID"),
                PhotoCategoryId = SQLDataHelper.GetNullableInt(reader, "PhotoCategoryId")
            };
        }




        #region product

        public static void SetProductMainPhoto(int photoId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_SetProductMainPhoto]", CommandType.StoredProcedure, new SqlParameter("@PhotoId", photoId));
        }

        public static void DeletePhotoWithPath(PhotoType type, string photoName)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Catalog].[Photo] WHERE PhotoName = @PhotoName and type=@type",
                                          CommandType.Text,
                                          new SqlParameter("@PhotoName", photoName),
                                          new SqlParameter("@type", type.ToString()));
        }

        /// <summary>
        /// check is product have photo by name
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="originName"></param>
        /// <returns></returns>
        public static bool IsProductHaveThisPhotoByName(int productId, string originName)
        {
            var name = SQLDataAccess.ExecuteScalar<string>(
                    "select top 1 PhotoName from Catalog.Photo where ObjID=@productId and OriginName=@originName and type=@type",
                    CommandType.Text,
                    new SqlParameter("@productId", productId),
                    new SqlParameter("@originName", originName),
                    new SqlParameter("@type", PhotoType.Product.ToString()));

            return name.IsNotEmpty();
        }

        public static void UpdatePhoto(Photo ph)
        {
            SQLDataAccess.ExecuteNonQuery("Update Catalog.Photo set Description=@Description, PhotoSortOrder = @PhotoSortOrder, ColorID=@ColorID, PhotoCategoryId = @PhotoCategoryId Where PhotoID = @PhotoID",
                                            CommandType.Text,
                                            new SqlParameter("@PhotoID", ph.PhotoId),
                                            new SqlParameter("@PhotoSortOrder", ph.PhotoSortOrder),
                                            new SqlParameter("@Description", ph.Description ?? (object)DBNull.Value),
                                            new SqlParameter("@ColorID", ph.ColorID ?? (object)DBNull.Value),
                                            new SqlParameter("@PhotoCategoryId", ph.PhotoCategoryId ?? (object)DBNull.Value)
                                            );
        }

        public static void UpdatePhotoName(Photo ph)
        {
            SQLDataAccess.ExecuteNonQuery("Update Catalog.Photo set PhotoName=@PhotoName Where PhotoID = @PhotoID",
                CommandType.Text,
                new SqlParameter("@PhotoID", ph.PhotoId),
                new SqlParameter("@PhotoName", ph.PhotoName)
            );
        }

        public static void UpdateObjId(int photoId, int objId)
        {
            SQLDataAccess.ExecuteNonQuery("Update Catalog.Photo set [ObjId] = @ObjId Where PhotoID = @PhotoID",
                                            CommandType.Text,
                                            new SqlParameter("@PhotoID", photoId),
                                            new SqlParameter("@ObjId", objId));
        }


        public static void DeleteProductPhoto(int photoId, bool backuping = true)
        {
            var photoName = GetPathByPhotoId(photoId);
            DeleteFile(PhotoType.Product, photoName, backuping);
            DeletePhotoById(photoId);
        }

        public static void DeleteProductPhotos(int productId)
        {
            DeletePhotos(productId, PhotoType.Product);
        }
        #endregion

        public static void DeleteReviewPhoto(int photoId, bool backuping = true)
        {
            var photoName = GetPathByPhotoId(photoId);
            DeleteFile(PhotoType.Review, photoName, backuping);
            DeletePhotoById(photoId);
        }

        public static void DeletePhotos(int objId, PhotoType type, bool backuping = true)
        {
            foreach (var photoName in GetNamePhotos(objId, type))
            {
                DeleteFile(type, photoName, backuping);
            }
            DeletePhotoByOwnerIdAndType(objId, type);
        }

        private static void DeleteFile(PhotoType type, string photoName, bool backuping = true)
        {
            if (photoName.Contains("://"))
                return;

            var backup = backuping && SettingsGeneral.BackupPhotosBeforeDeleting;

            switch (type)
            {
                case PhotoType.Product:

                    FilesStorageService.DecrementAttachmentsSize(FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Big, photoName));
                    FilesStorageService.DecrementAttachmentsSize(FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Middle, photoName));
                    FilesStorageService.DecrementAttachmentsSize(FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Small, photoName));

                    if (backup)
                    {
                        FileHelpers.BackupPhoto(FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Original, photoName));
                        FileHelpers.BackupPhoto(FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Big, photoName));
                        FileHelpers.BackupPhoto(FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Middle, photoName));
                        FileHelpers.BackupPhoto(FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Small, photoName));
                        FileHelpers.BackupPhoto(FoldersHelper.GetImageProductPathAbsolut(ProductImageType.XSmall, photoName));
                    }
                    else
                    {
                        FileHelpers.DeleteFile(FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Original, photoName));
                        FileHelpers.DeleteFile(FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Big, photoName));
                        FileHelpers.DeleteFile(FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Middle, photoName));
                        FileHelpers.DeleteFile(FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Small, photoName));
                        FileHelpers.DeleteFile(FoldersHelper.GetImageProductPathAbsolut(ProductImageType.XSmall, photoName));
                    }
                    break;
                case PhotoType.Brand:
                    if (backup)
                    {
                        FileHelpers.BackupPhoto(FoldersHelper.GetPathAbsolut(FolderType.BrandLogo, photoName));
                    }
                    else
                    {
                        FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.BrandLogo, photoName));
                    }
                    break;
                case PhotoType.Manager:
                    if (backup)
                    {
                        FileHelpers.BackupPhoto(FoldersHelper.GetPathAbsolut(FolderType.ManagerPhoto, photoName));
                    }
                    else
                    {
                        FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.ManagerPhoto, photoName));
                    }
                    break;
                case PhotoType.CategoryBig:
                    if (backup)
                    {
                        FileHelpers.BackupPhoto(FoldersHelper.GetImageCategoryPathAbsolut(CategoryImageType.Big, photoName));
                        FileHelpers.BackupPhoto(FoldersHelper.GetImageCategoryPathAbsolut(CategoryImageType.BigOriginal, photoName));
                    }
                    else
                    {
                        FileHelpers.DeleteFile(FoldersHelper.GetImageCategoryPathAbsolut(CategoryImageType.Big, photoName));
                        FileHelpers.DeleteFile(FoldersHelper.GetImageCategoryPathAbsolut(CategoryImageType.BigOriginal, photoName));
                    }
                    break;
                case PhotoType.CategorySmall:
                    if (backup)
                    {
                        FileHelpers.BackupPhoto(FoldersHelper.GetImageCategoryPathAbsolut(CategoryImageType.Small, photoName));
                        FileHelpers.BackupPhoto(FoldersHelper.GetImageCategoryPathAbsolut(CategoryImageType.SmallOriginal, photoName));
                    }
                    else
                    {
                        FileHelpers.DeleteFile(FoldersHelper.GetImageCategoryPathAbsolut(CategoryImageType.Small, photoName));
                        FileHelpers.DeleteFile(FoldersHelper.GetImageCategoryPathAbsolut(CategoryImageType.SmallOriginal, photoName));
                    }
                    break;
                case PhotoType.CategoryIcon:
                    if (backup)
                    {
                        FileHelpers.BackupPhoto(FoldersHelper.GetImageCategoryPathAbsolut(CategoryImageType.Icon, photoName));
                    }
                    else
                    {
                        FileHelpers.DeleteFile(FoldersHelper.GetImageCategoryPathAbsolut(CategoryImageType.Icon, photoName));
                    }
                    break;

                case PhotoType.Carousel:
                    if (backup)
                    {
                        FileHelpers.BackupPhoto(FoldersHelper.GetPathAbsolut(FolderType.Carousel, photoName));
                    }
                    else
                    {
                        FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Carousel, photoName));
                    }
                    break;
                case PhotoType.News:
                    if (backup)
                    {
                        FileHelpers.BackupPhoto(FoldersHelper.GetPathAbsolut(FolderType.News, photoName));
                    }
                    else
                    {
                        FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.News, photoName));
                    }
                    break;
                case PhotoType.StaticPage:
                    if (backup)
                    {
                        FileHelpers.BackupPhoto(FoldersHelper.GetPathAbsolut(FolderType.StaticPage, photoName));
                    }
                    else
                    {
                        FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.StaticPage, photoName));
                    }
                    break;
                case PhotoType.Shipping:
                    if (backup)
                    {
                        FileHelpers.BackupPhoto(FoldersHelper.GetPathAbsolut(FolderType.ShippingLogo, photoName));
                    }
                    else
                    {
                        FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.ShippingLogo, photoName));
                    }
                    break;
                case PhotoType.Payment:
                    if (backup)
                    {
                        FileHelpers.BackupPhoto(FoldersHelper.GetPathAbsolut(FolderType.PaymentLogo, photoName));
                    }
                    else
                    {
                        FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.PaymentLogo, photoName));
                    }
                    break;

                case PhotoType.MenuIcon:
                    if (backup)
                    {
                        FileHelpers.BackupPhoto(FoldersHelper.GetPathAbsolut(FolderType.MenuIcons, photoName));
                    }
                    else
                    {
                        FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.MenuIcons, photoName));
                    }
                    break;

                case PhotoType.Color:
                    if (backup)
                    {
                        FileHelpers.BackupPhoto(FoldersHelper.GetImageColorPathAbsolut(ColorImageType.Catalog, photoName));
                        FileHelpers.BackupPhoto(FoldersHelper.GetImageColorPathAbsolut(ColorImageType.Details, photoName));
                    }
                    else
                    {
                        FileHelpers.DeleteFile(FoldersHelper.GetImageColorPathAbsolut(ColorImageType.Catalog, photoName));
                        FileHelpers.DeleteFile(FoldersHelper.GetImageColorPathAbsolut(ColorImageType.Details, photoName));
                    }
                    break;

                case PhotoType.Review:
                    if (backup)
                    {
                        FileHelpers.BackupPhoto(FoldersHelper.GetImageReviewPathAbsolut(ReviewImageType.Small, photoName));
                        FileHelpers.BackupPhoto(FoldersHelper.GetImageReviewPathAbsolut(ReviewImageType.Big, photoName));
                    }
                    else
                    {
                        FileHelpers.DeleteFile(FoldersHelper.GetImageReviewPathAbsolut(ReviewImageType.Small, photoName));
                        FileHelpers.DeleteFile(FoldersHelper.GetImageReviewPathAbsolut(ReviewImageType.Big, photoName));
                    }
                    break;
                
                case PhotoType.CarouselApi:
                    if (backup)
                    {
                        FileHelpers.BackupPhoto(FoldersHelper.GetPathAbsolut(FolderType.CarouselApi, photoName));
                    }
                    else
                    {
                        FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.CarouselApi, photoName));
                    }
                    break;

                case PhotoType.CustomOptions:
                    if (backup)
                    {
                        FileHelpers.BackupPhoto(FoldersHelper.GetPathAbsolut(FolderType.CustomOptions, photoName));
                    }
                    else
                    {
                        FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.CustomOptions, photoName));
                    }
                    break;

                case PhotoType.ShippingZones:
                    if (backup)
                    {
                        FileHelpers.BackupPhoto(FoldersHelper.GetPathAbsolut(FolderType.ShippingZones, photoName));
                    }
                    else
                    {
                        FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.ShippingZones, photoName));
                    }
                    break;

                case PhotoType.CityAddressPoints:
                    if (backup)
                    {
                        FileHelpers.BackupPhoto(FoldersHelper.GetPathAbsolut(FolderType.CityAddressPoints, photoName));
                    }
                    else
                    {
                        FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.CityAddressPoints, photoName));
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private static void DeletePhotoById(int photoId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_DeletePhoto]", CommandType.StoredProcedure, new SqlParameter("@PhotoId", photoId));
        }

        private static void DeletePhotoByOwnerIdAndType(int objId, PhotoType type)
        {
            SQLDataAccess.ExecuteNonQuery("Delete FROM [Catalog].[Photo] WHERE [ObjId] = @ObjId and type=@type", CommandType.Text,
                                            new SqlParameter("@objId", objId),
                                            new SqlParameter("@type", type.ToString()));
        }

        public static void DeletePhotoByOwnerIdAndTypeAndColor(int objId, PhotoType type, int? colorId)
        {
            if (colorId != null)
            {
                SQLDataAccess.ExecuteNonQuery("Delete FROM [Catalog].[Photo] WHERE [ObjId] = @ObjId and type=@type and ColorId=@colorId", CommandType.Text,
                                            new SqlParameter("@objId", objId),
                                            new SqlParameter("@type", type.ToString()),
                                            new SqlParameter("@colorId", colorId));
            }
            else
            {
                SQLDataAccess.ExecuteNonQuery("Delete FROM [Catalog].[Photo] WHERE [ObjId] = @ObjId and type=@type and ColorId is null", CommandType.Text,
                                         new SqlParameter("@objId", objId),
                                         new SqlParameter("@type", type.ToString()));
            }
        }



        /// <summary>
        /// Проверяет наличие продукта в базе
        /// </summary>
        /// <param name="fileName">имя файла изображения, ищется продукт с аналогичным артикулом </param>
        /// <returns>ID найденного продукта</returns>
        /// <remarks>если записей не найдено возвращается пустая строка</remarks>
        public static int CheckImageInDataBase(string fileName)
        {
            // без расширения
            int dotPos = fileName.LastIndexOf(".");
            string shortFilename = fileName.Remove(dotPos, fileName.Length - dotPos);

            // 551215_v01_m.jpg
            // Regex regex = new Regex("([\\d\\w^\\-]*)_v([\\d]{2})_m");

            // 8470_1.jpg
            var regex = new Regex("([\\d\\w^\\-]*)_([\\d]*)");
            Match m = regex.Match(shortFilename);

            shortFilename = m.Groups[1].Value;

            return ProductService.GetProductId(shortFilename);
        }

        public static string GetDescription(int photoId)
        {
            if (photoId == 0)
                return string.Empty;
            return SQLDataAccess.ExecuteScalar<string>("SELECT [Description] FROM [Catalog].[Photo] WHERE [PhotoID] = @photoId", CommandType.Text, new SqlParameter("@photoId", photoId));
        }

        public static System.Drawing.Size GetImageMaxSize(ProductImageType type)
        {
            switch (type)
            {
                case ProductImageType.Big:
                    return new System.Drawing.Size(SettingsPictureSize.BigProductImageWidth, SettingsPictureSize.BigProductImageHeight);
                case ProductImageType.Middle:
                    return new System.Drawing.Size(SettingsPictureSize.MiddleProductImageWidth, SettingsPictureSize.MiddleProductImageHeight);
                case ProductImageType.Small:
                    return new System.Drawing.Size(SettingsPictureSize.SmallProductImageWidth, SettingsPictureSize.SmallProductImageHeight);
                case ProductImageType.XSmall:
                    return new System.Drawing.Size(SettingsPictureSize.XSmallProductImageWidth, SettingsPictureSize.XSmallProductImageHeight);
                case ProductImageType.Original:
                    return new System.Drawing.Size(SettingsPictureSize.BigProductImageWidth, SettingsPictureSize.BigProductImageHeight);
                
                default:
                    throw new ArgumentException("Parameter must be ProductImageType", type.ToString());
            }
        }
        
        public static System.Drawing.Size GetCategoryImageMaxSize(CategoryImageType type)
        {
            switch (type)
            {
                case CategoryImageType.Big:
                    return new System.Drawing.Size(SettingsPictureSize.BigCategoryImageWidth, SettingsPictureSize.BigCategoryImageHeight);
                
                case CategoryImageType.BigOriginal:
                    return new System.Drawing.Size(3000, 3000);
                
                case CategoryImageType.Small:
                    return new System.Drawing.Size(SettingsPictureSize.SmallCategoryImageWidth, SettingsPictureSize.SmallCategoryImageHeight);
                
                case CategoryImageType.SmallOriginal:
                    return new System.Drawing.Size(3000, 3000);
                
                case CategoryImageType.Icon:
                    return new System.Drawing.Size(SettingsPictureSize.IconCategoryImageWidth, SettingsPictureSize.IconCategoryImageHeight);
                
                default:
                    throw new ArgumentException($"Unknown CategoryImageType {type}");
            }
        }

        public static string PhotoToString(List<ProductPhoto> productPhotos, string columSeparator, string propertySeparator, bool absolutepath = false)
        {
            var sb = new StringBuilder();
            Color color = null;
            int? colorId = null;

            foreach (var photo in productPhotos.OrderByDescending(ph => ph.Main).ThenBy(ph => ph.ColorID))
            {
                if (colorId != photo.ColorID)
                {
                    colorId = photo.ColorID;
                    color = ColorService.GetColor(colorId);
                }
                sb.Append((absolutepath ? FoldersHelper.GetImageProductPath(ProductImageType.Big, photo.PhotoName, false) : photo.PhotoName) +
                          (color != null ? propertySeparator + color.ColorName : "") + columSeparator);
            }
            return sb.ToString().Trim((columSeparator ?? "").ToCharArray());
        }

        public static bool PhotosFromString(int productId, string photos, string columSeparator,
            string propertySeparator, bool skipOriginal = false, bool downloadRemotePhoto = true)
            => PhotosFromString(productId, photos, columSeparator, propertySeparator, out _, skipOriginal, downloadRemotePhoto);
        
        public static bool PhotosFromString(int productId, string photos, string columSeparator, string propertySeparator, out string[] errors, bool skipOriginal = false, bool downloadRemotePhoto = true)
        {
            if (string.IsNullOrWhiteSpace(columSeparator) || string.IsNullOrWhiteSpace(propertySeparator))
            {
                return _PhotosFromString(productId, photos, ",", ":", out errors, skipOriginal, downloadRemotePhoto);
            }

            return _PhotosFromString(productId, photos, columSeparator, propertySeparator, out errors, skipOriginal, downloadRemotePhoto);
        }

        private static bool _PhotosFromString(int productId, string photos, string photoSeparator, string colorSeparator, out string[] errors, bool skipOriginal = false, bool downloadRemotePhoto = true)
        {
            errors = null;
            var result = true;

            var listErrors = new List<string>();
            var arrPhotos = photos.Split(new[] { photoSeparator, "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < arrPhotos.Length; i++)
            {
                if (SaasDataService.IsSaasEnabled && GetCountPhotos(productId, PhotoType.Product) >= SaasDataService.CurrentSaasData.PhotosCount)
                {
                    errors = new[] { Core.Services.Localization.LocalizationService.GetResource("Core.Photo.LimitProductPhotos") };
                    return false;
                }

                var photo = "";
                var colorName = "";

                var count = arrPhotos[i].Count(c => c.ToString() == colorSeparator);
                if (count > 1)
                {
                    var indexof = arrPhotos[i].LastIndexOf(colorSeparator, StringComparison.Ordinal);

                    photo = indexof > 0 ? arrPhotos[i].Substring(0, indexof) : arrPhotos[i];
                    colorName = arrPhotos[i].Split(colorSeparator).LastOrDefault();
                }
                else if (count == 1)
                {
                    if (arrPhotos[i].Contains("http://") || arrPhotos[i].Contains("https://"))
                    {
                        photo = arrPhotos[i];
                    }
                    else
                    {
                        var photoAndColor = arrPhotos[i].SupperTrim().Split(colorSeparator);

                        photo = photoAndColor[0];

                        if (photoAndColor.Length == 2)
                            colorName = photoAndColor[1];
                    }
                }
                else
                {
                    photo = arrPhotos[i];
                }

                colorName = string.IsNullOrEmpty(colorName) ? colorName : colorName.SupperTrim();
                photo = string.IsNullOrEmpty(photo) ? photo : photo.SupperTrim();

                if (!PhotoFromString(productId, photo, i == 0, colorName, skipOriginal, downloadRemotePhoto, out var errorProcessPhoto))
                {
                    listErrors.Add(Core.Services.Localization.LocalizationService.GetResourceFormat("Core.Photo.NotPhotoProcessed", photo, errorProcessPhoto));
                    result = false;
                }
            }

            if (listErrors.Count > 0)
                errors = listErrors.ToArray();

            return result;
        }

        public static bool PhotoFromString(int productId, string photo, bool isMain, string colorName, bool skipOriginal, bool downloadRemotePhoto,
                                            out string error)
        {
            error = null;
            
            int? colorId = null;
            if (colorName.IsNotEmpty())
            {
                var color = ColorService.GetColor(colorName);
                if (color != null)
                    colorId = color.ColorId;
            }

            // if remote picture we must download it
            if (photo.Contains("http://") || photo.Contains("https://"))
            {
                var uri = new Uri(photo);
                if (!downloadRemotePhoto || IgnoreDownloadPhoto(photo))
                {
                    if (!IsProductHaveThisPhotoByName(productId, photo))
                        ProductService.AddProductPhotoLinkByProductId(productId, uri.AbsoluteUri, string.Empty, isMain, colorId, skipOriginal);
                    return true;
                }

                var photoName = uri.PathAndQuery.Split('?')[0].Trim('/').Replace("/", "-");
                photoName = Path.GetInvalidFileNameChars().Aggregate(photoName, (current, c) => current.Replace(c.ToString(), ""));
                var fileExtension = "";

                if (photoName.Contains("."))
                {
                    fileExtension = photoName.Substring(photoName.LastIndexOf('.'));

                    if (!FileHelpers.CheckFileExtensionByType(fileExtension, EFileType.Image))
                    {
                        photoName = photoName.Replace(fileExtension, ".jpg");
                        fileExtension = ".jpg";
                    }
                }

                if (photoName.Length > 100)
                {
                    if (SettingsCatalog.IsLimitedPhotoNameLength)
                        photoName = photoName.Substring(0, 90) + fileExtension;
                    else
                        photoName = photoName.Length - 245 > 0
                            ? photoName.Substring(photoName.Length - 245)
                            : photoName;
                }

                if (string.IsNullOrWhiteSpace(photoName) || IsProductHaveThisPhotoByName(productId, photoName))
                {
                    if (colorId != null)
                    {
                        var productPhoto = GetProductPhoto(productId, photoName);
                        if (productPhoto != null && productPhoto.ColorID != colorId)
                        {
                            productPhoto.ColorID = colorId;
                            UpdatePhoto(productPhoto);
                        }
                    }

                    return true;
                }

                FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));

                var result = FileHelpers.DownloadImage(photo, FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoName));
                if (!result.IsSuccess)
                {
                    error = result.Error?.Message ?? Core.Services.Localization.LocalizationService.GetResource("Core.Photo.FailedDownload");
                    return false;
                }

                if (string.IsNullOrEmpty(fileExtension))
                {
                    var ext = FileHelpers.TryGetExtensionFromImage(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoName));
                    if (string.IsNullOrEmpty(ext))
                        ext = ".jpg";

                    FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photoName + ext));
                    FileHelpers.RenameFile(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp) + photoName, FoldersHelper.GetPathAbsolut(FolderType.ImageTemp) + photoName + ext);
                    photoName += ext;
                }

                photo = photoName;
            }

            photo = string.IsNullOrEmpty(photo) ? photo : photo.SupperTrim();

            // where temp picture folder
            var fullfilename = FoldersHelper.GetPathAbsolut(FolderType.ImageTemp, photo);
            if (!File.Exists(fullfilename))
                return true;
            
            if (!IsProductHaveThisPhotoByName(productId, Path.GetFileName(photo)))
            {
                ProductService.AddProductPhotoByProductId(productId, fullfilename, string.Empty, isMain, colorId, skipOriginal);
            }
            else
            {
                ProductService.UpdateProductPhotoByProductId(productId, fullfilename, string.Empty, isMain, colorId, skipOriginal);
            }
            
            return true;
        }

        public static string GetNoPhotoPath(ProductImageType type, bool isRelative = false)
        {
            return (!isRelative ? UrlService.GetUrl() : "") + "images/nophoto" + FoldersHelper.ProductPhotoPostfix[type] + ".png";
        }

        public static bool IgnoreDownloadPhoto(string photo)
        {
            if (!photo.Contains("://"))
                return false;

            return photo.Contains(".p5s.ru") ||
                photo.Contains("sima-land.com") ||
                photo.Contains(".netlab.ru") ||
                photo.Contains(".on-advantshop.net") ||
                photo.Contains(".advstatic.ru");
        }
    }
}