using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Helpers;
using AdvantShop.Models.Attachments;
using AdvantShop.Web.Infrastructure.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace AdvantShop.Handlers.Common
{
    public class UploadAttachmentsHandler<T> : ICommandHandler<UploadAttachmentsResult[]>
         where T : Attachment, new()
    {
        protected readonly int? _objId;

        public UploadAttachmentsHandler(int? objId)
        {
            _objId = objId;
        }

        public UploadAttachmentsResult[] Execute()
        {
            var result = new List<UploadAttachmentsResult>();
            for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                result.Add(AddAttachment(HttpContext.Current.Request.Files[i]));
            }

            return result.ToArray();
        }

        private UploadAttachmentsResult ValidateFile(HttpPostedFile file, AttachmentType type, ref int filesLength)
        {
            if (file != null && file.ContentLength > 0)
            {
                if (!AttachmentService.CheckFileExtension(file.FileName, type))
                    return new UploadAttachmentsResult
                    {
                        Error = LocalizationService.GetResource("InvalidFileExtension"),
                        Attachment = new AttachmentModel { FileName = file.FileName }
                    };

                filesLength += file.ContentLength;
                if (FileHelpers.FileStorageLimitReached(filesLength))
                    return new UploadAttachmentsResult
                    {
                        Error = LocalizationService.GetResource("Common.Attachments.FileStorageLimitReached"),
                        Attachment = new AttachmentModel { FileName = file.FileName }
                    };

                return new UploadAttachmentsResult { Result = true, Attachment = new AttachmentModel { FileName = file.FileName } };
            }

            return new UploadAttachmentsResult { Error = LocalizationService.GetResource("Common.Attachments.FileNotFound") };
        }

        protected virtual UploadAttachmentsResult AddAttachment(HttpPostedFile file)
        {
            if (!_objId.HasValue)
                return new UploadAttachmentsResult() { Error = "No object to attach file" };

            var attachment = new T()
            {
                ObjId = _objId.Value,
                FileName = GetValidFileName(file.FileName),
                OriginFileName = file.FileName,
                FileSize = file.ContentLength,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
            };
            if (attachment.Type == AttachmentType.None)
                throw new Exception("Could not create an instance of Attachment with AttachmentType None");

            int filesLength = 0;
            var validateResult = ValidateFile(file, attachment.Type, ref filesLength);
            if (!validateResult.Result)
                return validateResult;

            var existAttachments = AttachmentService.GetAttachments<T>(_objId.Value);
            if (existAttachments.Count == 10)
                return new UploadAttachmentsResult()
                {
                    Result = false,
                    Error = LocalizationService.GetResource("Common.Attachments.ExceededLimit"),
                    Attachment = new AttachmentModel { FileName = attachment.OriginFileName }
                };

            while (existAttachments.Any(x => x.FileName.Equals(attachment.FileName, StringComparison.OrdinalIgnoreCase)))
                attachment.FileName = GetValidFileName(attachment.FileName);

            attachment.Id = AttachmentService.AddAttachment(attachment);

            if (attachment.Id != 0)
            {
                FileHelpers.SaveFile(attachment.PathAbsolut, file.InputStream);

                return new UploadAttachmentsResult()
                {
                    Result = true,
                    Attachment = new AttachmentModel
                    {
                        Id = attachment.Id,
                        ObjId = attachment.ObjId,
                        FileName = attachment.OriginFileName
                    },
                };
            }

            return new UploadAttachmentsResult()
            {
                Result = false,
                Error = LocalizationService.GetResource("Common.Attachments.FileNotFound")
            };
        }

        private string GetValidFileName(string fileName)
        {
            return Guid.NewGuid() + Path.GetExtension(fileName);
        }
    }
}
