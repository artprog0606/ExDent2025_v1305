using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.TemplatesDocx;
using AdvantShop.Helpers;
using AdvantShop.Localization;

namespace AdvantShop.Web.Admin.Models.Settings.TemplatesDocxSettings
{
    public class TemplatesDocxModel : IValidatableObject
    {
        public TemplatesDocxModel()
        {
            
        }

        public int Id { get; set; }
        public TemplateDocxType Type { get; set; }
        public string TaxTypeFormatted { get { return Type.Localize(); } }
        public string Name { get; set; }
        public string FileName { get; set; }
        public int SortOrder { get; set; }
        public bool DebugMode { get; set; }
        public long FileSize { get; set; }

        public string FileSizeFormatted
        {
            get { return FileHelpers.FileSize(FileSize); }
        }
        public DateTime DateCreated { get; set; }
        public string DateCreatedFormatted { get { return Culture.ConvertDate(DateCreated); } }
        public DateTime DateModified { get; set; }
        public string DateModifiedFormatted { get { return Culture.ConvertDate(DateModified); } }

        public string PathAdmin
        {
            get { return TemplatesDocxServices.GetPath(FileName, true); }
        }


        public static explicit operator TemplatesDocxModel(TemplateDocx template)
        {
            return new TemplatesDocxModel()
            {
                Id = template.Id,
                Type = template.Type,
                Name = template.Name,
                FileName = template.FileName,
                FileSize = template.FileSize,
                SortOrder = template.SortOrder,
                DebugMode = template.DebugMode,
                DateCreated = template.DateCreated,
                DateModified = template.DateModified,
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                yield return new ValidationResult("Укажите название");
            }
        }
    }
}
