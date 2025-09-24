using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Models.Attachments;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Web;

namespace AdvantShop.Handlers.Common
{
    public class UploadAttachmentsWithResizePhotoHandler<T> : UploadAttachmentsHandler<T>
         where T : Attachment, new()
    {
        private readonly int _maxWidth;
        private readonly int _maxHeight;

        public UploadAttachmentsWithResizePhotoHandler(int? objId, int maxWidth, int maxHeight) : base(objId)
        {
            _maxWidth = maxWidth;
            _maxHeight = maxHeight;
        }

        protected override UploadAttachmentsResult AddAttachment(HttpPostedFile file)
        {
            if (FileHelpers.CheckFileExtensionByType(file.FileName, EFileType.Image | EFileType.Favicon | EFileType.Svg))
            {
                using (Image image = Image.FromStream(file.InputStream))
                {
                    var tempDirectoryPath = $"{FoldersHelper.GetPathAbsolut(FolderType.ApplicationTempData, null)}AttachmentTemp/{typeof(T).Name}/{_objId}";
                    var tempFilePath = $"{tempDirectoryPath}/{file.FileName}";
                    FileHelpers.CreateDirectory(tempDirectoryPath);
                    FileHelpers.SaveResizePhotoFile(tempFilePath, _maxWidth, _maxHeight, image);
                    if (!File.Exists(tempFilePath))
                    {
                        FileHelpers.DeleteDirectory(tempDirectoryPath);
                        return new UploadAttachmentsResult
                        {
                            Error = LocalizationService.GetResource("Common.Attachments.CannotResizePhotoFile"),
                            Attachment = new AttachmentModel { FileName = file.FileName }
                        };
                    }
                    var newFile = ConstructHttpPostedFile(tempFilePath, file.FileName, file.ContentType);
                    FileHelpers.DeleteDirectory(tempDirectoryPath);
                    return base.AddAttachment(newFile);
                }
            }
            return base.AddAttachment(file);
        }

        private HttpPostedFile ConstructHttpPostedFile(string tempFilePath, string fileName, string contentType)
        {
            byte[] buffer = new byte[2048];
            int bytesRead;
            using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read))
            {
                int fileSize = (int)fileStream.Length;

                // Get the System.Web assembly reference
                Assembly systemWebAssembly = typeof(HttpPostedFileBase).Assembly;
                // Get the types of the two internal types we need
                Type typeHttpRawUploadedContent = systemWebAssembly.GetType("System.Web.HttpRawUploadedContent");
                Type typeHttpInputStream = systemWebAssembly.GetType("System.Web.HttpInputStream");
                // Prepare the signatures of the constructors we want.
                Type[] uploadedParams = { typeof(int), typeof(int) };
                Type[] streamParams = { typeHttpRawUploadedContent, typeof(int), typeof(int) };
                Type[] parameters = { typeof(string), typeof(string), typeHttpInputStream };
                // Create an HttpRawUploadedContent instance
                object uploadedContent = typeHttpRawUploadedContent
                                        .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, uploadedParams,
                                             null)
                                        .Invoke(new object[] { fileSize, buffer.Length });

                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    // Call the AddBytes method
                    typeHttpRawUploadedContent
                     .GetMethod("AddBytes", BindingFlags.NonPublic | BindingFlags.Instance)
                     .Invoke(uploadedContent, new object[] { buffer, 0, bytesRead });
                }

                // This is necessary if you will be using the returned content (ie to Save)
                typeHttpRawUploadedContent
                 .GetMethod("DoneAddingBytes", BindingFlags.NonPublic | BindingFlags.Instance)
                 .Invoke(uploadedContent, null);
                // Create an HttpInputStream instance
                object stream = (Stream)typeHttpInputStream
                                        .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, streamParams,
                                           null)
                                        .Invoke(new object[] { uploadedContent, 0, fileSize });
                // Create an HttpPostedFile instance
                HttpPostedFile postedFile = (HttpPostedFile)typeof(HttpPostedFile)
                                                            .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
                                                               null, parameters, null)
                                                            .Invoke(new object[] { fileName, contentType, stream });

                return postedFile;
            }
        }
    }
}
