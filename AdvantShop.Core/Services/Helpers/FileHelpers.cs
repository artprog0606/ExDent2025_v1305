//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Primitives;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Images.WebpDowngrader;
using AdvantShop.Saas;
using AdvantShop.Statistic;
using ByteSizeLib;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Debug = AdvantShop.Diagnostics.Debug;
using Encoder = System.Drawing.Imaging.Encoder;

// class for work with file, managed
namespace AdvantShop.Helpers
{
    [Obsolete("Use EFileType")]
    public enum EAdvantShopFileTypes
    {
        Image,
        Favicon,
        Zip,
        Catalog,
        FileInRootFolder,
        TaskAttachment,
        LeadAttachment,
        BookingAttachment,
        TemplateDocx,
        Photo,
        Svg
    }

    [Flags]
    public enum EFileType
    {
        Image = 0b1 << 0,
        Favicon = 0b1 << 1,
        Svg = 0b1 << 2,
        Video = 0b1 << 3,
        Audio = 0b1 << 4,
        Archive = 0b1 << 5,
        ZipArchive = 0b1 << 6,
        Html = 0b1 << 7,
        Xml = 0b1 << 8,
        Csv = 0b1 << 9,
        Text = 0b1 << 10,
        Document = 0b1 << 11,
        Font = 0b1 << 12,
        ModernImage = 0b1 << 13,
    }

    public class FileHelpers
    {
        private static readonly Dictionary<EFileType, List<string>> AllowedFileExtensions =
            new Dictionary<EFileType, List<string>>
            {
                {
                    EFileType.Image,
                    new List<string> { ".jpg", ".jpeg", ".gif", ".png", ".bmp" }
                },
                {
                    EFileType.ModernImage,
                    new List<string> { ".webp" }
                },
                {
                    EFileType.Favicon,
                    new List<string> { ".ico", ".gif", ".png" }
                },
                {
                    EFileType.Video,
                    new List<string> { ".mp4", ".avi", ".mov", ".3gpp", ".mpeg-4" }
                },
                {
                    EFileType.Audio,
                    new List<string> { ".mp3", ".wav", ".aiff", ".ape", ".flac", ".ogg" }
                },
                {
                    EFileType.Svg,
                    new List<string> { ".svg" }
                },
                {
                    EFileType.ZipArchive,
                    new List<string> { ".zip" }
                },
                {
                    EFileType.Archive,
                    new List<string> { ".zip", ".arj", ".rar", ".cab", ".tar", ".lzh" }
                },
                {
                    EFileType.Html,
                    new List<string> { ".html", ".htm" }
                },
                {
                    EFileType.Xml,
                    new List<string> { ".xml", ".yml", ".yaml" }
                },
                {
                    EFileType.Csv,
                    new List<string> { ".csv", ".txt" }
                },
                {
                    EFileType.Text,
                    new List<string> { ".txt" }
                },
                {
                    EFileType.Document,
                    new List<string> { ".txt", ".pdf", ".doc", ".docx", ".xml", ".xls", ".xlsx", ".rtf" }
                },
                {
                    EFileType.Font,
                    new List<string> { ".woff", ".woff2" }
                }
            };

        public static bool CheckFileExtensionByType(string fileName, EFileType allowedTypes)
        {
            if (fileName.Contains("?"))
                fileName = fileName.Split('?')[0];

            var fileExt = Path.GetExtension(fileName.ToLower());

            return Enum.GetValues(typeof(EFileType))
                .Cast<EFileType>()
                .Any(type => allowedTypes.HasFlag(type) && AllowedFileExtensions[type].Contains(fileExt));
        }

        [Obsolete("if not use delete in future")]
        private static EFileType MapEAdvantShopFileTypesToEFileType(EAdvantShopFileTypes type)
        {
            switch (type)
            {
                case EAdvantShopFileTypes.Image:
                    return EFileType.Image;
                case EAdvantShopFileTypes.Favicon:
                    return EFileType.Favicon;
                case EAdvantShopFileTypes.Zip:
                    return EFileType.ZipArchive;
                case EAdvantShopFileTypes.Catalog:
                    return EFileType.Csv | EFileType.Text;
                case EAdvantShopFileTypes.FileInRootFolder:
                    return EFileType.Text | EFileType.Csv | EFileType.Html | EFileType.Xml;
                case EAdvantShopFileTypes.TaskAttachment:
                case EAdvantShopFileTypes.LeadAttachment:
                case EAdvantShopFileTypes.BookingAttachment:
                    return EFileType.Text | EFileType.Document | EFileType.Archive |
                           EFileType.Image | EFileType.Favicon;
                case EAdvantShopFileTypes.TemplateDocx:
                    return EFileType.Document;
                case EAdvantShopFileTypes.Photo:
                    return EFileType.Image;
                case EAdvantShopFileTypes.Svg:
                    return EFileType.Svg;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }


        /// <summary>
        /// Delete file if it is exist
        /// </summary>
        /// <param name="fullname"></param>
        public static void DeleteFile(string fullname)
        {
            if (!File.Exists(fullname)) return;

            try
            {
                try
                {
                    File.Delete(fullname);
                }
                catch (IOException)
                {
                    Thread.Sleep(100);
                    File.Delete(fullname);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(fullname, ex);
            }
        }

        public static bool RenameFile(string prevName, string newName)
        {
            if (File.Exists(prevName) && !File.Exists(newName))
            {
                var di = new FileInfo(newName).Directory;
                if (di != null && !di.Exists)
                    di.Create();

                var count = 3;

                for (var i = 0; i < count; i++)
                {
                    try
                    {
                        File.Move(prevName, newName);
                        return true;
                    }
                    catch (IOException ex)
                    {
                        if (i != count - 1)
                        {
                            Thread.Sleep(100);
                            continue;
                        }

                        Debug.Log.Error("prevName - " + prevName + ", newName - " + newName, ex);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error("prevName - " + prevName + ", newName - " + newName, ex);
                        return false;
                    }
                }
            }

            return true;
        }

        public static void ReplaceFile(string fromFile, string resultFile)
        {
            var fileToRevert = resultFile + Guid.NewGuid();
            if (!RenameFile(resultFile, fileToRevert))
                return;

            if (RenameFile(fromFile, resultFile))
            {
                DeleteFile(fileToRevert);
            }
            else
            {
                RenameFile(fileToRevert, resultFile);
            }
        }

        public static long GetFileSize(string fileName)
        {
            if (!File.Exists(fileName))
                return 0;
            try
            {
                var fi = new FileInfo(fileName);
                return fi.Length;
            }
            catch
            {
                //ignore
            }

            return 0;
        }

        private const string BackupFolder = "pictures_deleted";

        private static string GetActualBackupFolder() =>
            Path.Combine(BackupFolder, DateTime.Now.ToString("yyyy-MM-dd"));

        public static void BackupPhoto(string fullname) =>
            BackupFile(fullname, fullname.Replace("pictures", GetActualBackupFolder()));

        public static void BackupFile(string fullname, string backupFullName = null)
        {
            if (!File.Exists(fullname))
                return;

            if (string.IsNullOrWhiteSpace(backupFullName))
                backupFullName = fullname.Replace(
                    SettingsGeneral.AbsolutePath, $"{SettingsGeneral.AbsolutePath}{GetActualBackupFolder()}/");

            var index = backupFullName.LastIndexOf('/');
            if (index > 0)
                CreateDirectory(backupFullName.Substring(0, index));
            else
            {
                index = backupFullName.LastIndexOf('\\');
                if (index > 0)
                    CreateDirectory(backupFullName.Substring(0, index));
            }

            if (File.Exists(backupFullName))
                File.Delete(backupFullName);

            File.Move(fullname, backupFullName);
        }
 

        public static void BackupPhotosDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                return;

            foreach (var dir in Directory.GetDirectories(directory, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dir.Replace("pictures", GetActualBackupFolder()));

            foreach (var file in Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories))
            {
                var newFilePath = file.Replace("pictures", GetActualBackupFolder());
                if (File.Exists(newFilePath))
                    File.Delete(newFilePath);

                File.Move(file, newFilePath);
            }
        }

        public static void CleanBackupPhoto()
        {
            var backupFolderPath = Path.Combine(SettingsGeneral.AbsolutePath, BackupFolder);
            if (Directory.Exists(backupFolderPath) is false)
                return;

            var milestoneDate = DateTime.Today.AddMonths(-1);
            var backupFolderInfo = new DirectoryInfo(backupFolderPath);

            MigrateBackupPhotoFolders(backupFolderInfo);

            foreach (var subDirInfo in backupFolderInfo.GetDirectories())
            {
                if (subDirInfo.Name.TryParseDateTime() < milestoneDate)
                {
                    DeleteDirectory(subDirInfo.FullName);
                }
            }
        }

        private static void MigrateBackupPhotoFolders(DirectoryInfo directoryInfo)
        {
            var subDirsToMigrate = directoryInfo.GetDirectories()
                .Where(x => DateTime.TryParse(x.Name, out _) == false)
                .ToList();

            if (subDirsToMigrate.Count == 0)
                return;

            var backupFolderPath = Path.Combine(SettingsGeneral.AbsolutePath, GetActualBackupFolder());
            Directory.CreateDirectory(backupFolderPath);

            foreach (var subDirectoryInfo in subDirsToMigrate)
            {
                Directory.Move(subDirectoryInfo.FullName, Path.Combine(backupFolderPath, subDirectoryInfo.Name));
            }
        }


        /// <summary>
        /// Delete directory if it's empty
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void DeleteEmptyDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath)) return;

            try
            {
                var files = Directory.GetFiles(directoryPath);
                var dirs = Directory.GetDirectories(directoryPath);

                if (files.Length > 0 || dirs.Length > 0)
                    return;

                try
                {
                    Directory.Delete(directoryPath, false);
                }
                catch (Exception)
                {
                    Thread.Sleep(100);
                    Directory.Delete(directoryPath, true);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(directoryPath, ex);
            }
        }

        /// <summary>
        /// Recursively clear directory 
        /// </summary>
        /// <param name="directoryPath">Directory path</param>
        public static void ClearDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath)) return;

            try
            {
                var files = Directory.GetFiles(directoryPath);
                var dirs = Directory.GetDirectories(directoryPath);

                foreach (var file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }

                foreach (var dir in dirs)
                {
                    ClearDirectory(dir);
                    DeleteEmptyDirectory(dir);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(directoryPath, ex);
            }
        }

        /// <summary>
        /// Recursively clear directory and delete it
        /// </summary>
        public static void DeleteDirectory(string directoryName, bool deleteSelf = true)
        {
            if (!Directory.Exists(directoryName)) return;

            ClearDirectory(directoryName);
            if (deleteSelf)
                DeleteEmptyDirectory(directoryName);
        }

        /// <summary>
        /// Recursively clear directory 
        /// </summary>
        /// <param name="directoryPath">Directory path</param>
        /// <param name="deleteDate">Files before this date will be deleted</param>
        public static void ClearDirectoriesByDate(string directoryPath, DateTime deleteDate)
        {
            if (!Directory.Exists(directoryPath)) return;

            try
            {
                var files = Directory.GetFiles(directoryPath);
                var dirs = Directory.GetDirectories(directoryPath);

                foreach (var file in files)
                {
                    if (File.GetCreationTime(file) < deleteDate)
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                        File.Delete(file);
                    }
                }

                foreach (var dir in dirs)
                    ClearDirectoriesByDate(dir, deleteDate);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(directoryPath, ex);
            }
        }

        public static void CreateDirectory(string strPath)
        {
            if (!Directory.Exists(strPath))
                Directory.CreateDirectory(strPath);
        }

        public static void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs = false,
            bool copyFiles = false)
        {
            var currentDir = new DirectoryInfo(sourceDirName);

            if (!currentDir.Exists)
            {
                return;
            }

            Directory.CreateDirectory(destDirName);

            if (copyFiles)
            {
                var files = currentDir.GetFiles();
                foreach (var file in files)
                {
                    var tempPath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(tempPath, false);
                }
            }

            if (copySubDirs)
            {
                var subDirs = currentDir.GetDirectories();
                foreach (var subDir in subDirs)
                {
                    string tempPath = Path.Combine(destDirName, subDir.Name);
                    CopyDirectory(subDir.FullName, tempPath, true, copyFiles);
                }
            }
        }

        public static long GetDirectorySize(string dirPath)
        {
            long length = 0;

            var dir = new DirectoryInfo(dirPath);
            foreach (var file in dir.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                length += file.Length;
            }

            return length;
        }

        public static void CreateFile(string filename)
        {
            if (!File.Exists(filename))
            {
                File.Create(filename).Dispose();
            }
        }

        public static void DeleteFilesFromPath(string directoryPath)
        {
            if (!Directory.Exists(directoryPath)) return;
            var files = Directory.GetFiles(directoryPath); // prevent loop
            foreach (var file in files)
            {
                DeleteFile(file);
            }
        }

        public static void DeleteFilesFromImageTemp()
        {
            DeleteFilesFromPath(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
        }

        public static void DeleteFilesFromImageTempInBackground()
        {
            Task.Factory.StartNew(DeleteFilesFromImageTemp, TaskCreationOptions.LongRunning);
        }

        public static void UpdateDirectories()
        {
            var pictDirs = new List<string>
            {
                FoldersHelper.GetPathAbsolut(FolderType.Product),
                FoldersHelper.GetPathAbsolut(FolderType.News),
                FoldersHelper.GetPathAbsolut(FolderType.Category),
                FoldersHelper.GetPathAbsolut(FolderType.BrandLogo),
                FoldersHelper.GetPathAbsolut(FolderType.ManagerPhoto),
                FoldersHelper.GetPathAbsolut(FolderType.Carousel),
                FoldersHelper.GetPathAbsolut(FolderType.Color),
                FoldersHelper.GetPathAbsolut(FolderType.ReviewImage),
                FoldersHelper.GetPathAbsolut(FolderType.MobileAppIcon)
            };

            pictDirs.AddRange(FoldersHelper.ProductPhotoPrefix.Select(kvp =>
                FoldersHelper.GetImageProductPathAbsolut(kvp.Key, string.Empty)));
            pictDirs.AddRange(FoldersHelper.CategoryPhotoPrefix.Select(kvp =>
                FoldersHelper.GetImageCategoryPathAbsolut(kvp.Key, string.Empty)));
            pictDirs.AddRange(FoldersHelper.ColorPhotoPrefix.Select(kvp =>
                FoldersHelper.GetImageColorPathAbsolut(kvp.Key, string.Empty)));
            pictDirs.AddRange(FoldersHelper.ReviewPhotoPrefix.Select(kvp =>
                FoldersHelper.GetImageReviewPathAbsolut(kvp.Key, string.Empty)));
            foreach (var directory in pictDirs.Where(dir => !Directory.Exists(dir) && dir.Trim().Length != 0))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public static void SaveFile(string fullname, Stream serverFileStream)
        {
            const int length = 1024;
            var buffer = new Byte[length];
            serverFileStream.Position = 0;
            // write the required bytes
            using (var fs = new FileStream(fullname, FileMode.Create, FileAccess.Write))
            {
                int bytesRead;
                do
                {
                    bytesRead = serverFileStream.Read(buffer, 0, length);
                    fs.Write(buffer, 0, bytesRead);
                } while (bytesRead == length);
            }
        }

        public static void SaveResizePhotoFile(string resultPath, int maxWidth, int maxHeight, Image image,
            long? quality = null, bool isRotated = false)
        {
            UpdateDirectories();

            if (File.Exists(resultPath))
                DeleteFile(resultPath);

            if (!isRotated)
            {
                RotateImageIfNeed(image);
            }

            double resultWidth = image.Width; // 0;
            double resultHeight = image.Height; // 0;

            if (maxHeight != 0 && image.Height > maxHeight)
            {
                resultHeight = maxHeight;
                resultWidth = image.Width * resultHeight / image.Height;
            }

            if (maxWidth != 0 && resultWidth > maxWidth)
            {
                resultHeight = resultHeight * maxWidth / resultWidth; // (resultHeight * resultWidth) / resultHeight;
                resultWidth = maxWidth;
            }

            try
            {
                using (var result = new Bitmap((int)resultWidth, (int)resultHeight))
                {
                    result.MakeTransparent();
                    using (var graphics = Graphics.FromImage(result))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        var fileExt = GetExtensionWithoutException(resultPath);
                        if (fileExt.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                            fileExt.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
                        {
                            graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, image.Width, image.Height));
                        }

                        graphics.DrawImage(image, 0, 0, (int)resultWidth, (int)resultHeight);

                        graphics.Flush();
                        var ext = Path.GetExtension(resultPath);
                        var encoder = GetEncoder(ext);

                        if (!quality.HasValue)
                        {
                            quality = SettingsMain.ImageQuality;
                        }


                        using (var myEncoderParameters = new EncoderParameters(3))
                        {
                            myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality.Value);
                            myEncoderParameters.Param[1] = new EncoderParameter(Encoder.ScanMethod,
                                (int)EncoderValue.ScanMethodInterlaced);
                            myEncoderParameters.Param[2] = new EncoderParameter(Encoder.RenderMethod,
                                (int)EncoderValue.RenderProgressive);

                            using (var stream = new FileStream(resultPath, FileMode.CreateNew))
                            {
                                result.Save(stream, encoder, myEncoderParameters);
                                stream.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error("Error on upload " + resultPath, ex);
            }
        }

        public static void ResizeAllProductPhotos()
        {
            foreach (var photoObj in PhotoService.GetAllPhotos(PhotoType.Product))
            {
                try
                {
                    if (!CommonStatistic.IsRun || CommonStatistic.IsBreaking)
                        return;

                    var photoName = photoObj.PhotoName;

                    if (photoName.StartsWith("http://") || photoName.StartsWith("https://"))
                    {
                        continue;
                    }

                    var originalPath = FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Original, photoName);
                    if (!File.Exists(originalPath))
                    {
                        var bigPath = FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Big, photoName);
                        if (File.Exists(bigPath))
                        {
                            File.Copy(bigPath, originalPath);
                        }
                    }

                    if (File.Exists(originalPath))
                    {
                        if (string.IsNullOrWhiteSpace(originalPath))
                            throw new ArgumentNullException(nameof(originalPath));

                        using (var lStream = new FileStream(originalPath, FileMode.Open, FileAccess.Read))
                        {
                            using (var image = Image.FromStream(lStream))
                            {
                                SaveProductImageUseCompress(photoName, image, true);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                    CommonStatistic.TotalErrorRow++;
                }
                finally
                {
                    CommonStatistic.RowPosition++;
                }
            }
        }
        
        public static void ResizeCategoryPhotos(CategoryImageType type)
        {
            if (!CommonStatistic.IsRun || CommonStatistic.IsBreaking)
                return;
            
            PhotoType photoType;

            switch (type)
            {
                case CategoryImageType.Big:
                    photoType = PhotoType.CategoryBig;
                    break;
                case CategoryImageType.Small:
                    photoType = PhotoType.CategorySmall;
                    break;
                default:
                    throw new ArgumentException("Unsupported type", type.ToString());
            }
            
            CommonStatistic.TotalRow = PhotoService.GetCountPhotos(0, photoType);

            foreach (var photoObj in PhotoService.GetAllPhotos(photoType))
            {
                if (!CommonStatistic.IsRun || CommonStatistic.IsBreaking)
                    return;

                var photoName = photoObj.PhotoName;

                if (photoName.StartsWith("http"))
                {
                    CommonStatistic.RowPosition++;
                    continue;
                }

                try
                {
                    var originalPath = "";
                    
                    switch (type)
                    {
                        case CategoryImageType.Big:
                            originalPath = FoldersHelper.GetImageCategoryPathAbsolut(CategoryImageType.BigOriginal, photoName);
                            break;
                        case CategoryImageType.Small:
                            originalPath = FoldersHelper.GetImageCategoryPathAbsolut(CategoryImageType.SmallOriginal, photoName);
                            break;
                        default:
                            throw new ArgumentNullException(nameof(originalPath));
                    }
                    
                    if (!File.Exists(originalPath))
                    {
                        var bigPath = FoldersHelper.GetImageCategoryPathAbsolut(type, photoName);
                        if (File.Exists(bigPath))
                            File.Copy(bigPath, originalPath);
                    }

                    if (File.Exists(originalPath))
                    {
                        using (var stream = new FileStream(originalPath, FileMode.Open, FileAccess.Read))
                            using (var image = Image.FromStream(stream))
                                SaveCategoryImage(photoName, image, type, true);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                    CommonStatistic.TotalErrorRow++;
                }
                finally
                {
                    CommonStatistic.RowPosition++;
                }
            }
        }


        private static RotateFlipType OrientationToFlipType(short orientation)
        {
            switch (orientation)
            {
                case 1:
                    return RotateFlipType.RotateNoneFlipNone;
                case 2:
                    return RotateFlipType.RotateNoneFlipX;
                case 3:
                    return RotateFlipType.Rotate180FlipNone;
                case 4:
                    return RotateFlipType.Rotate180FlipX;
                case 5:
                    return RotateFlipType.Rotate90FlipX;
                case 6:
                    return RotateFlipType.Rotate90FlipNone;
                case 7:
                    return RotateFlipType.Rotate270FlipX;
                case 8:
                    return RotateFlipType.Rotate270FlipNone;
                default:
                    return RotateFlipType.RotateNoneFlipNone;
            }
        }


        public static bool RotateImageIfNeed(Image image)
        {
            var itemOrientation = image.PropertyItems.FirstOrDefault(x => x.Id == 0x0112);
            if (itemOrientation == null)
                return false;

            var value = BitConverter.ToInt16(itemOrientation.Value, 0);
            var flip = OrientationToFlipType(value);

            if (flip == RotateFlipType.RotateNoneFlipNone)
                return false;

            image.RotateFlip(flip);
            itemOrientation.Value = BitConverter.GetBytes(1); //resert orientation

            return true;
        }

        public static void SaveProductImageUseCompress(string destName, Image image, bool skipOriginal = false)
        {
            UpdateDirectories();

            var isRotated = RotateImageIfNeed(image);

            //не удалять, создаем еще один image из-за багов в формате файла, если сохранять напрямую, выдает исключение GDI+
            using (var img = new Bitmap(image))
            {
                if (!skipOriginal && !Trial.TrialService.IsTrialEnabled && !SettingsGeneral.SkipResizeOriginalPhotos)
                {
                    /* Uncomment if you want save original picture
                    if (image.Width > 3000 || image.Height > 3000)
                    {
                        SaveResizePhotoFile(ProductImageType.Original, img, destName, 100);
                    }
                    else
                    {
                        try
                        {
                            var path = FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Original, destName);
                            image.Save(path);
                        }
                        catch
                        {
                            SaveResizePhotoFile(ProductImageType.Original, img, destName, 100);
                        }
                    }
                     */
                    SaveResizePhotoFile(ProductImageType.Original, img, destName, 100, isRotated);
                }

                //if (!SettingsCatalog.CompressBigImage)
                //{
                //    var path = FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Big, destName);
                //    var size = PhotoService.GetImageMaxSize(ProductImageType.Original);

                //    SaveResizePhotoFile(path, size.Width, size.Height, img, 100, isRotated);

                //    FilesStorageService.IncrementAttachmentsSize(path);
                //}

                ModulesExecuter.ProcessPhoto(img);

                //if (SettingsCatalog.CompressBigImage)
                //{
                SaveResizePhotoFile(ProductImageType.Big, img, destName, isRotated: isRotated);
                //}
                SaveResizePhotoFile(ProductImageType.Middle, img, destName, isRotated: isRotated);
                SaveResizePhotoFile(ProductImageType.Small, img, destName, isRotated: isRotated);
                SaveResizePhotoFile(ProductImageType.XSmall, img, destName, isRotated: isRotated);
            }
        }
        
        public static void SaveResizePhotoFile(ProductImageType type, Image image, string destName, long? quality = null, bool isRotated = false)
        {
            var size = PhotoService.GetImageMaxSize(type);
            var resultPath = FoldersHelper.GetImageProductPathAbsolut(type, destName);

            SaveResizePhotoFile(resultPath, size.Width, size.Height, image, quality, isRotated);

            FilesStorageService.IncrementAttachmentsSize(resultPath);
        }

        public static void SaveCategoryImage(string photoName, Image image, CategoryImageType type, bool skipOriginal = false, long? quality = null)
        {
            UpdateDirectories();
            
            var isRotated = RotateImageIfNeed(image);

            using (var img = new Bitmap(image))
            {
                if (!skipOriginal && !SettingsGeneral.SkipResizeOriginalPhotos)
                {
                    switch (type)
                    {
                        case CategoryImageType.Big:
                            SaveCategoryImage(photoName, img, CategoryImageType.BigOriginal, 100, isRotated);
                            break;
                        case CategoryImageType.Small:
                            SaveCategoryImage(photoName, img, CategoryImageType.SmallOriginal, 100, isRotated);
                            break;
                    }
                }
                
                SaveCategoryImage(photoName, img, type, quality, isRotated);
            }
        }
        
        private static void SaveCategoryImage(string phoName, Image image, CategoryImageType type, long? quality, bool isRotated)
        {
            var size = PhotoService.GetCategoryImageMaxSize(type);
            var resultPath = FoldersHelper.GetImageCategoryPathAbsolut(type, phoName);

            SaveResizePhotoFile(resultPath, size.Width, size.Height, image, quality, isRotated);
        }

        private static ImageCodecInfo GetEncoder(string fileExt)
        {
            fileExt = fileExt.TrimStart(".".ToCharArray()).ToLower().Trim();

            return CacheManager.Get("GetEncoder" + fileExt, () =>
            {
                string mimeType;
                switch (fileExt)
                {
                    case "jpg":
                    case "jpeg":
                        mimeType = "image/jpeg";
                        break;
                    case "png":
                        mimeType = "image/png";
                        break;
                    case "gif":
                        //if need transparency
                        //res = "image/png";
                        mimeType = "image/gif";
                        break;
                    default:
                        mimeType = "image/jpeg";
                        break;
                }

                return ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.MimeType == mimeType);
            });
        }

        public static string TryGetExtensionFromImage(string filePath)
        {
            try
            {
                using (var img = Image.FromFile(filePath))
                {
                    var format = img.RawFormat;

                    if (format.Equals(ImageFormat.Jpeg))
                        return ".jpeg";

                    else if (format.Equals(ImageFormat.Png))
                        return ".png";

                    else if (format.Equals(ImageFormat.Bmp))
                        return ".bmp";

                    else if (format.Equals(ImageFormat.Gif))
                        return ".gif";

                    else if (format.Equals(ImageFormat.Exif))
                        return ".exif";

                    else if (format.Equals(ImageFormat.Tiff))
                        return ".tiff";
                }
            }
            catch
            {
                //ignore
            }

            return "";
        }

        /// <summary>
        /// zip file or folder
        /// </summary>
        /// <param name="inputFolderPath"></param>
        /// <param name="outputPathAndFile"></param>
        /// <param name="password"></param>
        /// <param name="inscludeself">create zip with root folder</param>
        /// <returns></returns>
        public static bool ZipFiles(string inputFolderPath, string outputPathAndFile, string password = null,
            bool inscludeself = false)
        {
            try
            {
                int folderOffset;
                var attr = File.GetAttributes(inputFolderPath);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    if (inscludeself)
                    {
                        folderOffset = Directory.GetParent(inputFolderPath).ToString().Length;
                    }
                    else
                    {
                        folderOffset = inputFolderPath.Length;
                        // find number of chars to remove     // from orginal file path
                        folderOffset += inputFolderPath.EndsWith("\\")
                            ? 0
                            : 1; //remove '\'
                    }
                }
                else
                {
                    folderOffset = Directory.GetParent(inputFolderPath).ToString().Length;
                }

                using (var zipStream = new ZipOutputStream(File.Create(outputPathAndFile))) // create zip stream
                {
                    if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
                    zipStream.SetLevel(4); // //0-9, 9 being the highest level of compression

                    if (attr.HasFlag(FileAttributes.Directory))
                        CompressFolder(inputFolderPath, zipStream, folderOffset, Path.GetFullPath(outputPathAndFile));
                    else
                        CompressFile(inputFolderPath, zipStream, folderOffset);

                    zipStream.IsStreamOwner = true;
                    zipStream.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return false;
            }
        }

        private static void CompressFile(string filename, ZipOutputStream zipStream, int folderOffset)
        {
            FileInfo fi = new FileInfo(filename);
            string entryName = filename.Substring(folderOffset);
            entryName = ZipEntry.CleanName(entryName);
            ZipEntry newEntry = new ZipEntry(entryName);
            newEntry.DateTime = fi.LastWriteTime;
            newEntry.IsUnicodeText = true;
            newEntry.Size = fi.Length;
            zipStream.PutNextEntry(newEntry);
            byte[] buffer = new byte[4096];
            using (FileStream streamReader = File.OpenRead(filename))
            {
                StreamUtils.Copy(streamReader, zipStream, buffer);
            }

            zipStream.CloseEntry();
        }

        private static void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset,
            string excludeFile = null)
        {
            string[] files = Directory.GetFiles(Path.GetFullPath(path));

            if (!string.IsNullOrEmpty(excludeFile))
                files = files.Where(x => !x.Equals(excludeFile, StringComparison.OrdinalIgnoreCase)).ToArray();

            foreach (string filename in files)
            {
                CompressFile(filename, zipStream, folderOffset);
            }

            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset);
            }
        }

        [Obsolete("if not use delete in future")]
        private static List<string> GenerateFileList(string dir, bool recurse)
        {
            var files = new List<string>();

            var attr = File.GetAttributes(dir);
            if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
            {
                files.Add(dir);
                return files;
            }

            bool empty = true;
            foreach (string file in Directory.GetFiles(dir)) // add each file in directory
            {
                files.Add(file);
                empty = false;
            }

            if (empty)
            {
                // if directory is completely empty, add it
                if (Directory.GetDirectories(dir).Length == 0)
                {
                    files.Add(dir + @"/");
                }
            }

            if (recurse)
                foreach (string dirs in Directory.GetDirectories(dir)) // recursive
                {
                    files.AddRange(GenerateFileList(dirs, true));
                }

            return files; // return file list
        }

        public static bool CanUnZipFile(string inputPathOfZipFile)
        {
            int result;
            if (File.Exists(inputPathOfZipFile))
            {
                using (var zipStream = new ZipInputStream(File.OpenRead(inputPathOfZipFile)))
                {
                    zipStream.GetNextEntry();
                    result = zipStream.Available;
                }
            }
            else
            {
                return false;
            }

            return result == 1;
        }

        public static string RemoveInvalidFileNameChars(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !Path.GetInvalidFileNameChars().Any(fileName.Contains))
                return fileName;

            return Path.GetInvalidFileNameChars().Where(item => item.ToString() != "\\" && item.ToString() != "/")
                .Aggregate(fileName, (current, charInvalid) => current.Replace(charInvalid.ToString(), string.Empty));
        }

        public static string RemoveInvalidPathChars(string path)
        {
            List<char> invalidChars = Path.GetInvalidPathChars().ToList();
            //invalidChars.Add(Path.DirectorySeparatorChar);
            //invalidChars.Add(Path.AltDirectorySeparatorChar);
            //invalidChars.Add(Path.PathSeparator);
            //invalidChars.Add(Path.VolumeSeparatorChar);

            if (string.IsNullOrEmpty(path) || !invalidChars.Any(path.Contains))
                return path;

            return invalidChars.Aggregate(path,
                (current, charInvalid) => current.Replace(charInvalid.ToString(), string.Empty));
        }

        private static bool UnZipFileMs(string inputPathOfZipFile, string outputDirPath)
        {
            if (!File.Exists(inputPathOfZipFile) || string.IsNullOrWhiteSpace(outputDirPath))
                return false;

            try
            {
                var file = new FileInfo(inputPathOfZipFile);
                if (file.Length == 0)
                    return false;

                using (var archive = System.IO.Compression.ZipFile.OpenRead(inputPathOfZipFile))
                {
                    foreach (var entry in archive.Entries)
                    {
                        var isDirectory = entry.FullName.EndsWith("/") || entry.FullName.EndsWith("\\");

                        if (isDirectory)
                        {
                            var newDirectory = outputDirPath + @"\" + RemoveInvalidPathChars(entry.FullName);
                            CreateDirectory(newDirectory);
                        }
                        else
                        {
                            var newFile = outputDirPath + @"\" + RemoveInvalidFileNameChars(entry.FullName);
                            DeleteFile(newFile);

                            entry.ExtractToFile(newFile);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return false;
        }

        // Unzip with folders and files
        public static bool UnZipFile(string archiveFilenameIn, string outFolder = null)
        {
            bool result = true;
            string baseDirectory = string.IsNullOrEmpty(outFolder)
                ? Path.GetDirectoryName(archiveFilenameIn)
                : outFolder;
            try
            {
                using (var zipStream = new ZipInputStream(File.OpenRead(archiveFilenameIn)))
                {
                    ZipEntry theEntry;
                    while ((theEntry = zipStream.GetNextEntry()) != null)
                    {
                        if (!theEntry.IsFile)
                        {
                            continue; // Ignore directories
                        }

                        var strNewFile = Path.Combine(baseDirectory, theEntry.Name);
                        string directoryName = Path.GetDirectoryName(strNewFile);
                        string fileName = Path.GetFileName(strNewFile);

                        if (directoryName.Length > 0)
                            Directory.CreateDirectory(directoryName);
                        if (fileName == String.Empty) continue;

                        byte[] buffer = new byte[4096];
                        DeleteFile(strNewFile);
                        using (FileStream streamWriter = File.Create(strNewFile))
                        {
                            StreamUtils.Copy(zipStream, streamWriter, buffer);
                        }
                    }

                    zipStream.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zipStream.Close();
                }
            }
            catch (Exception ex)
            {
                result = false;
                Debug.Log.Error(ex);
            }

            if (!result)
                return UnZipFileMs(archiveFilenameIn, baseDirectory);

            return true;
        }
        
        public static bool CheckFilesExtensionsInZipFile(string archiveFilename, List<string> allowedExtensions, out string errorFileName)
        {
            errorFileName = null;
            try
            {
                using (var zipStream = new ZipInputStream(File.OpenRead(archiveFilename)))
                {
                    ZipEntry entry;
                    while ((entry = zipStream.GetNextEntry()) != null)
                    {
                        if (!entry.IsFile)
                            continue;
                        
                        var extension = Path.GetExtension(entry.Name);

                        if (!allowedExtensions.Contains(extension))
                        {
                            errorFileName = entry.Name.Split('/').LastOrDefault();
                            return false;
                        }
                    }

                    zipStream.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zipStream.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return false;
            }
            
            return true;
        }

        public static bool IsDirectoryHaveFiles(string path)
        {
            if (!Directory.Exists(path)) return false;
            return Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).Any();
        }

        public static bool DownloadRemoteImageFile(string uri, string fileName)
        {
            var result = DownloadImage(uri, fileName);
            return result.IsSuccess; //DownloadRemoteImageFile(uri, fileName, out _);
        }

        public static bool DownloadRemoteImageFile(string uri, string fileName, out string msg)
        {
            var result = DownloadImage(uri, fileName);
            msg = !result.IsSuccess ? result.Error.Message : null;
            return result.IsSuccess;
            /*
            msg = LocalizationService.GetResource("Core.Errors.FailedToAccessFile");

            if (WebpDownGraderService.DetectWebpFromLink(uri))
            {
                try
                {
                    var success = WebpDownGraderService.DowngradeImageByUri(uri, fileName);

                    if (success is false)
                    {
                        return false;
                    }
                }
                catch (NotSupportedException)
                {
                    msg = LocalizationService.GetResource("Core.Errors.UnsupportedImageFormat");
                    return false;
                }
                catch (BlException e)
                {
                    msg = e.Message;
                    return false;
                }

                msg = null;
                return true;
            }
            
            try
            {
                uri = StringHelper.ToPuny(uri);
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";
                request.UserAgent =
                    "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.134 Safari/537.36";
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                var response = (HttpWebResponse)request.GetResponse();
                var responseUrl = response.ResponseUri.ToString().ToLower();

                // Check that the remote file was found. The ContentType 
                // check is performed since a request for a non-existent 
                // image file might be redirected to a 404-page, which would 
                // yield the StatusCode "OK", even though the image was not found. 
                if ((response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Moved || response.StatusCode == HttpStatusCode.Redirect) 
                    && !response.ContentType.EndsWith("webp", StringComparison.OrdinalIgnoreCase) // не поддерживаем webp
                    && !response.ContentType.StartsWith("text/html", StringComparison.OrdinalIgnoreCase)  // валидные ссылки с изображением, но с html-контентом, напр. cloud.mail.ru
                    && (response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase) 
                        || AllowedFileExtensions[EFileType.Image].Any(ext => responseUrl.EndsWith(ext)))
                   )
                {
                    DeleteFile(fileName);

                    var notEmptyFile = false;

                    // if the remote file was found, download it 
                    using (Stream inputStream = response.GetResponseStream())
                    using (Stream outputStream = File.Create(fileName))
                    {
                        var buffer = new byte[4096];
                        int bytesRead;
                        do
                        {
                            bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                            outputStream.Write(buffer, 0, bytesRead);

                            notEmptyFile |= bytesRead != 0;
                        } while (bytesRead != 0);
                    }

                    if (!notEmptyFile)
                        return false;

                    msg = null;
                    return true;
                }

                msg = LocalizationService.GetResource("Core.Errors.UnsupportedImageFormat");
                return false;
            }
            catch (WebException wex)
            {
                var errorResponse = wex.Response as HttpWebResponse;
                //404 Not Found
                if (errorResponse != null && errorResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    msg = LocalizationService.GetResource("Core.Errors.FileNotFound");
                    return false;
                }

                //Other status 401, 503, 403...
                if (wex.Status != WebExceptionStatus.ProtocolError ||
                    wex.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    return false;
                }

                //Other errors WebException. 
                Debug.Log.Error(wex);
                return false;
            }
            catch (Exception ex)
            {
                Debug.Log.Error("Url:" + uri + ",filename:" + fileName + ". " + ex.Message, ex);
                return false;
            }
            */
        }

        public static Result DownloadImage(string uri, string fileName)
        {
            var extension = GetExtension(uri);
            if (extension == ".webp")
                return _DownloadWebpImage(uri, fileName);
            
            var result = _DownloadImage(uri, fileName);
            
            if (!result.IsSuccess && result.Error.Message == "webp error")
                return _DownloadWebpImage(uri, fileName);

            return result;
        }

        private static Result _DownloadImage(string uri, string fileName)
        {
            try
            {
                uri = StringHelper.ToPuny(uri);
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.134 Safari/537.36";
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                var response = (HttpWebResponse)request.GetResponse();
                var responseUrl = response.ResponseUri.ToString().ToLower();

                if (response.StatusCode != HttpStatusCode.OK && 
                    response.StatusCode != HttpStatusCode.Moved && 
                    response.StatusCode != HttpStatusCode.Redirect)
                {
                    return Result.Failure(new Error(LocalizationService.GetResource("Core.Errors.UnsupportedImageFormat")));
                }
                
                // webp обрабатываем отдельно
                if (response.ContentType.EndsWith("webp", StringComparison.OrdinalIgnoreCase))
                {
                    return Result.Failure(new Error("webp error"));
                }

                // валидные ссылки с изображением, но с html-контентом, напр. cloud.mail.ru
                if (response.ContentType.StartsWith("text/html", StringComparison.OrdinalIgnoreCase)  
                    || (!response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase)
                        && !AllowedFileExtensions[EFileType.Image].Any(ext => responseUrl.EndsWith(ext))))
                {
                    return Result.Failure(new Error(LocalizationService.GetResource("Core.Errors.UnsupportedImageFormat")));
                }

                DeleteFile(fileName);

                var notEmptyFile = false;
                
                using (Stream inputStream = response.GetResponseStream())
                using (Stream outputStream = File.Create(fileName))
                {
                    var buffer = new byte[4096];
                    int bytesRead;
                    do
                    {
                        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                        outputStream.Write(buffer, 0, bytesRead);

                        notEmptyFile |= bytesRead != 0;
                    } while (bytesRead != 0);
                }

                if (!notEmptyFile)
                    return Result.Failure(new Error("Не удалось скачать файл"));

                return Result.Success();

            }
            catch (WebException wex)
            {
                var errorResponse = wex.Response as HttpWebResponse;
                //404 Not Found
                if (errorResponse != null && errorResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return Result.Failure(new Error(LocalizationService.GetResource("Core.Errors.FileNotFound")));
                }

                //Other status 401, 503, 403...
                if (wex.Status != WebExceptionStatus.ProtocolError ||
                    wex.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    return Result.Failure(new Error($"Не удалось скачать файл {uri}"));
                }

                //Other errors WebException. 
                Debug.Log.Error(wex);
                
                return Result.Failure(new Error($"Не удалось скачать файл {uri}"));
            }
            catch (Exception ex)
            {
                Debug.Log.Error("Url:" + uri + ",filename:" + fileName + ". " + ex.Message, ex);
                
                return Result.Failure(new Error($"Ошибка при скачивании файла {uri}"));
            }
        }
        
        private static Result _DownloadWebpImage(string uri, string fileName)
        {
            try
            {
                var success = WebpDownGraderService.DowngradeImageByUri(uri, fileName);
                if (success)
                    return Result.Success();
            }
            catch (NotSupportedException)
            {
                return Result.Failure(new Error(LocalizationService.GetResource("Core.Errors.UnsupportedImageFormat")));
            }
            catch (BlException e)
            {
                return Result.Failure(new Error(e.Message));
            }
            return Result.Failure(new Error($"Не удалось скачать webp-файл с {uri}"));
        }

        //todo: remove
        [Obsolete("Use CheckFileExtensionByType")]
        public static bool CheckFileExtension(string fileName, EAdvantShopFileTypes fileType)
        {
            var mappedType = MapEAdvantShopFileTypesToEFileType(fileType);
            return CheckFileExtensionByType(fileName, mappedType);
        }

        public static List<FileInfo> GetFilesInRootDirectory()
        {
            var filesWithPath = Directory.GetFiles(SettingsGeneral.AbsolutePath);
            return filesWithPath.Where(file => !string.IsNullOrEmpty(file) && file.Contains("\\"))
                .Select(file => new FileInfo(file))
                .Where(file => CheckFileExtensionByType(file.Name, EFileType.Html | EFileType.Csv | EFileType.Xml) &&
                               IsNotSystemFile(file.Name)).ToList();
        }

        private static bool IsNotSystemFile(string fileName)
        {
            var systemFiles = new List<string> { "app_offline", "cmsmagazine", "robots.txt" };
            if (!string.IsNullOrEmpty(fileName))
            {
                return systemFiles.All(name => !fileName.Contains(name));
            }

            return true;
        }

        public static string FileSize(string filename)
        {
            var len = new FileInfo(filename).Length;
            return FileSize(len);
        }

        public static string FileSize(long fileLength) => ByteSize.FromBytes(fileLength).ToString();

        public static string GetExtension(string path)
        {
            var _ext = Path.GetExtension(path);

            if (_ext.Contains('?'))
            {
                _ext = _ext.Split('?')[0];
            }

            return _ext;
        }
        
        public static string GetExtensionWithoutException(string path)
        {
            if (path == null)
                return null;
            
            var index = path.IndexOf('?');
            var pathWithoutQuery = index > 0 ? path.Substring(0, index) : path;
            
            int length = pathWithoutQuery.Length;
            int num = length;
            
            while (--num >= 0)
            {
                char ch = pathWithoutQuery[num];
                
                if (ch == '.')
                    return num != length - 1 
                        ? pathWithoutQuery.Substring(num, length - num) 
                        : string.Empty;
                
                if ((int) ch == (int) Path.DirectorySeparatorChar || (int) ch == (int) Path.AltDirectorySeparatorChar || (int) ch == (int) Path.VolumeSeparatorChar)
                    break;
            }
            return string.Empty;
        }

        public static long GetFileStorageSize()
        {
            long filesSize = 0;
            filesSize += AttachmentService.GetAllAttachmentsSize();
            return filesSize;
        }

        public static bool FileStorageLimitReached(int? newFileLength = null) =>
            SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.FileStorageVolume != 0 &&
            GetFileStorageSize() + (newFileLength ?? 0) >
            ByteSize.FromGigaBytes(SaasDataService.CurrentSaasData.FileStorageVolume).Bytes;

        //todo: remove
        [Obsolete("Use GetAllowedFileExtensions with EFileType")]
        public static List<string> GetAllowedFileExtensions(EAdvantShopFileTypes advantShopFileTypes)
        {
            var mappedType = MapEAdvantShopFileTypesToEFileType(advantShopFileTypes);
            return GetAllowedFileExtensions(mappedType).ToList();
        }

        public static HashSet<string> GetAllowedFileExtensions(EFileType allowedTypes)
        {
            var result = new HashSet<string>();
            foreach (EFileType fileType in Enum.GetValues(typeof(EFileType)))
            {
                if (!allowedTypes.HasFlag(fileType))
                    continue;

                foreach (var ext in AllowedFileExtensions[fileType])
                {
                    result.Add(ext);
                }
            }

            return result;
        }

        //todo: remove
        [Obsolete]
        public static string GetFilesHelpText(EAdvantShopFileTypes advantShopFileTypes, string maxFileSize = null)
        {
            var mappedTypes = MapEAdvantShopFileTypesToEFileType(advantShopFileTypes);
            return GetFilesHelpText(mappedTypes, maxFileSize);
        }

        public static string GetFilesHelpText(EFileType allowedFileTypes, string maxFileSize = null)
        {
            var extensions = GetAllowedFileExtensions(allowedFileTypes).AggregateString(", ");

            if (extensions.IsNullOrEmpty())
                return string.Empty;

            return maxFileSize.IsNullOrEmpty()
                ? LocalizationService.GetResourceFormat("Core.FileHelpers.FilesHelpText.Common", extensions)
                : $"{LocalizationService.GetResourceFormat("Core.FileHelpers.FilesHelpText.Common", extensions)}<br/>{LocalizationService.GetResourceFormat("Core.FileHelpers.FilesHelpText.MaxFileSize", maxFileSize)}";
        }

        public static void ResizeMobileAppIcon(string fileName, Image image, string filePath = null)
        {
            using (var img = new Bitmap(image))
            {
                foreach (MobileAppIcon iconSize in Enum.GetValues(typeof(MobileAppIcon)))
                {
                    SaveResizeMobileAppIconFile(iconSize, img, fileName, filePath);
                }
            }
        }

        public static void DeleteMobileAppIcons(string fileName, string filePath = null)
        {
            foreach (MobileAppIcon iconSize in Enum.GetValues(typeof(MobileAppIcon)))
            {
                var resultPath = FoldersHelper.GetMobileAppIconPathAbsolut(iconSize, fileName, filePath);
                DeleteFile(resultPath);
            }

            //delete original photo
            DeleteFile((filePath ?? FoldersHelper.GetPathAbsolut(FolderType.MobileAppIcon)) + fileName);
        }

        private static void SaveResizeMobileAppIconFile(MobileAppIcon type, Image image, string fileName,
            string filePath = null, long quality = 90)
        {
            var size = GetMobileAppIconMaxSize(type);
            var resultPath = FoldersHelper.GetMobileAppIconPathAbsolut(type, fileName, filePath);

            SaveResizePhotoFile(resultPath, size.Width, size.Height, image, quality);

            FilesStorageService.IncrementAttachmentsSize(resultPath);
        }

        public static System.Drawing.Size GetMobileAppIconMaxSize(MobileAppIcon type)
        {
            switch (type)
            {
                case MobileAppIcon.x48:
                    return new System.Drawing.Size(48, 48);
                case MobileAppIcon.x72:
                    return new System.Drawing.Size(72, 72);
                case MobileAppIcon.x96:
                    return new System.Drawing.Size(92, 92);
                case MobileAppIcon.x128:
                    return new System.Drawing.Size(128, 128);
                case MobileAppIcon.x144:
                    return new System.Drawing.Size(144, 144);
                case MobileAppIcon.x152:
                    return new System.Drawing.Size(152, 152);
                case MobileAppIcon.x192:
                    return new System.Drawing.Size(192, 192);
                case MobileAppIcon.x384:
                    return new System.Drawing.Size(384, 384);
                case MobileAppIcon.x512:
                    return new System.Drawing.Size(512, 512);

                default:
                    throw new ArgumentException(@"Parameter must be ProductImageType", "type");
            }
        }

        /// <summary>
        /// Helps to get hash of file
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <returns>Hash string of file content</returns>
        /// <exception cref="ArgumentNullException">If filePath is null or empty</exception>
        /// <exception cref="ArgumentException">is file does not exist</exception>
        public static string GetHashByContent(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            var absoluteFilePath = filePath;
            if (!Path.IsPathRooted(filePath))
            {
                var path = filePath;
                if (!path.StartsWith("~/"))
                {
                    path = "~/" + path.TrimStart('/');
                }

                absoluteFilePath = HostingEnvironment.MapPath(path);
            }

            if (absoluteFilePath == null || !File.Exists(absoluteFilePath))
                throw new ArgumentException("File not exists");

            string hashString;
            using (var fileStream = File.OpenRead(absoluteFilePath))
            {
                using (var sha256 = SHA256.Create())
                {
                    var hashBytes = sha256.ComputeHash(fileStream);
                    hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }

            return hashString;
        }
    }
}