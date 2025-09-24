using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.TemplatesDocx;
using AdvantShop.Core.Services.TemplatesDocx.Templates;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Handlers.Attachments;
using AdvantShop.Web.Admin.Models.Booking.Journal;

namespace AdvantShop.Web.Admin.Handlers.Booking.Journal
{
    public class GenerateTemplatesDocx
    {
        private readonly GenerateTemplatesDocxModel _model;
        public List<string> Errors { get; set; }

        public GenerateTemplatesDocx(GenerateTemplatesDocxModel model)
        {
            _model = model;
            Errors = new List<string>();
        }

        public object Execute()
        {
            if (_model.TemplatesDocx == null || _model.TemplatesDocx.Count <= 0)
            {
                Errors.Add("Укажите шаблоны");
                return null;
            }

            var booking = BookingService.Get(_model.BookingId);
            if (booking == null)
            {
                Errors.Add("Бронь не найдена");
                return null;
            }
            if (!BookingService.CheckAccess(booking))
            {
                Errors.Add(LocalizationService.GetResource("Admin.Booking.NoAccess"));
                return null;
            }

            BookingTemplate bookingTemplate = (BookingTemplate) booking;

            if (_model.Attach)
            {
                if (!BookingService.CheckAccessToEditing(booking))
                {
                    Errors.Add(LocalizationService.GetResource("Admin.Booking.NoAccess"));
                    return null;
                }

                var directoryPath = string.Format("{0}/", FoldersHelper.GetPathAbsolut(FolderType.PriceTemp));

                foreach (var id in _model.TemplatesDocx)
                {
                    var template = TemplatesDocxServices.Get<BookingTemplateDocx>(id);
                    if (template != null)
                    {
                        var templateFile = TemplatesDocxServices.GetPathAbsolut(template);
                        var generateFile = string.Format("{0}{1}{2}", directoryPath, Guid.NewGuid(),
                            Path.GetExtension(templateFile));

                        File.Copy(templateFile, generateFile);

                        TemplatesDocxServices.TemplateFillContent(generateFile, bookingTemplate, isNeedToNoticeAboutErrors: template.DebugMode);

                        var attachFileName = template.Name + Path.GetExtension(templateFile);

                        var existAttachment = AttachmentService.GetAttachments<BookingAttachment>(booking.Id)
                            .FirstOrDefault(x => x.FileName.Equals(attachFileName, StringComparison.OrdinalIgnoreCase));

                        if (existAttachment != null)
                            AttachmentService.DeleteAttachment<BookingAttachment>(existAttachment.Id);

                        AddFileToRequest(HttpContext.Current.Request,
                            attachFileName,
                            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                            File.ReadAllBytes(generateFile));

                        FileHelpers.DeleteFile(generateFile);
                    }
                }

                return new UploadAttachmentsHandler(booking.Id).Execute<BookingAttachment>();
            }
            else
            {
                var directoryPath = string.Format("{0}{1}", FoldersHelper.GetPathAbsolut(FolderType.PriceTemp), Guid.NewGuid());
                FileHelpers.CreateDirectory(directoryPath);

                foreach (var id in _model.TemplatesDocx)
                {
                    var template = TemplatesDocxServices.Get<BookingTemplateDocx>(id);
                    var templateFile = TemplatesDocxServices.GetPathAbsolut(template);
                    var generateFile = string.Format("{0}/{1}{2}", directoryPath, template.Name, Path.GetExtension(templateFile));
                    File.Copy(templateFile, generateFile);

                    TemplatesDocxServices.TemplateFillContent(generateFile, bookingTemplate, isNeedToNoticeAboutErrors: template.DebugMode);
                }

                if (_model.TemplatesDocx.Count == 1)
                {
                    return new Tuple<string, string>(Directory.GetFiles(directoryPath)[0], directoryPath);
                }
                else
                {
                    var zipFilePath = string.Format("{0}/files.zip", directoryPath);
                    if (!FileHelpers.ZipFiles(directoryPath, zipFilePath))
                    {
                        Errors.Add("Не удалось заархивировать файлы");

                        FileHelpers.DeleteDirectory(directoryPath);
                        return null;
                    }

                    return new Tuple<string, string>(zipFilePath, directoryPath);
                }
            }
        }

        private static void AddFileToRequest(HttpRequest request, string fileName, string contentType, byte[] bytes)
        {
            var fileSize = bytes.Length;

            // Because these are internal classes, we can't even reference their types here
            var uploadedContent = Construct(typeof(HttpPostedFile).Assembly,
                "System.Web.HttpRawUploadedContent", fileSize, fileSize);

            InvokeMethod(uploadedContent, "AddBytes", bytes, 0, fileSize);
            InvokeMethod(uploadedContent, "DoneAddingBytes");

            var inputStream = Construct(typeof(HttpPostedFile).Assembly,
                "System.Web.HttpInputStream", uploadedContent, 0, fileSize);

            var postedFile = Construct<HttpPostedFile>(fileName, contentType, inputStream);
            // Accessing request.Files creates an empty collection
            InvokeMethod(request.Files, "AddFile", fileName, postedFile);
        }

        private static object Construct(Assembly assembly, string typeFqn, params object[] args)
        {
            var theType = assembly.GetType(typeFqn);
            return theType
              .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                     args.Select(a => a.GetType()).ToArray(), null)
              .Invoke(args);
        }

        private static T Construct<T>(params object[] args) where T : class
        {
            return Activator.CreateInstance(
                typeof(T),
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                null, args, null) as T;
        }

        private static object InvokeMethod(object o, string methodName,
             params object[] args)
        {
            var mi = o.GetType().GetMethod(methodName,
                     BindingFlags.NonPublic | BindingFlags.Instance);
            if (mi == null) throw new ArgumentOutOfRangeException("methodName",
                string.Format("Method {0} not found", methodName));
            return mi.Invoke(o, args);
        }
    }
}
