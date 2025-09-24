﻿using System.Web;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Web.Admin.Models.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace AdvantShop.Web.Admin.Handlers.Attachments
{
    public class UploadAttachmentsHandler
    {
        protected readonly int? _objId;

        public UploadAttachmentsHandler(int? objId)
        {
            _objId = objId;
        }

        public UploadAttachmentsResult[] Execute<T>() where T : Attachment, new()
        {
            var result = new List<UploadAttachmentsResult>();
            for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                result.Add(AddAttachment<T>(HttpContext.Current.Request.Files[i]));
            }

            return result.ToArray();
        }

        public UploadAttachmentsResult[] Validate<T>() where T : Attachment, new()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request.Files.Count == 0)
                return new[] { new UploadAttachmentsResult { Result = false, Error = LocalizationService.GetResource("Admin.Attachments.FileNotFound") } };

            var type = new T().Type;
            
            // файлы не добавляются ни физически, ни в базу, суммируем размеры файлов
            int filesLength = 0;

            var result = new List<UploadAttachmentsResult>();
            for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                var file = HttpContext.Current.Request.Files[i];
                result.Add(ValidateFile<T>(file, type, ref filesLength));
            }

            return result.ToArray();
        }

        private UploadAttachmentsResult ValidateFile<T>(HttpPostedFile file, AttachmentType type, ref int filesLength) where T : Attachment, new()
        {
            if (file != null && file.ContentLength > 0)
            {
                if (!AttachmentService.CheckFileExtension(file.FileName, type))
                    return new UploadAttachmentsResult
                    {
                        Error = LocalizationService.GetResource("InvalidFileExtension"),
                        Attachment = new AttachmentModel { FileName = file.FileName, OriginFileName = file.FileName }
                    };

                filesLength += file.ContentLength;
                if (FileHelpers.FileStorageLimitReached(filesLength))
                    return new UploadAttachmentsResult
                    {
                        Error = LocalizationService.GetResource("Admin.Attachments.FileStorageLimitReached"),
                        Attachment = new AttachmentModel { FileName = file.FileName }
                    };

                return new UploadAttachmentsResult { Result = true, Attachment = new AttachmentModel { FileName = file.FileName, OriginFileName = file.FileName } };
            }

            return new UploadAttachmentsResult { Error = LocalizationService.GetResource("Admin.Attachments.FileNotFound") };
        }

        protected virtual UploadAttachmentsResult AddAttachment<T>(HttpPostedFile file) where T : Attachment, new()
        {
            if (!_objId.HasValue)
                return new UploadAttachmentsResult() {Error = "No object to attach file"};

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
            var validateResult = ValidateFile<T>(file, attachment.Type, ref filesLength);
            if (!validateResult.Result)
                return validateResult;

            var existAttachments = AttachmentService.GetAttachments<T>(_objId.Value);

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
                        FileName = attachment.FileName,
                        OriginFileName = attachment.OriginFileName,
                        FilePath = attachment.Path,
                        FilePathAdmin = attachment.PathAdmin,
                        FileSize = attachment.FileSizeFormatted
                    },
                };
            }

            return new UploadAttachmentsResult()
            {
                Result = false,
                Error = LocalizationService.GetResource("Admin.Attachments.FileNotFound")
            };
        }

        private string GetValidFileName(string fileName)
        {
            return Guid.NewGuid() + Path.GetExtension(fileName);
        }

    }
}
