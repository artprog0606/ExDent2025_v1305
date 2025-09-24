using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Web.Admin.Models.Crm.SalesFunnels
{
    public class LeadFieldModel : LeadField, IValidatableObject
    {
        public LeadFieldModel()
        {
            FieldValues = new List<LeadFieldValue>();
        }

        public LeadFieldModel(LeadField field, bool loadValues = false) : this()
        {
            Id = field.Id;
            Name = field.Name;
            FieldType = field.FieldType;
            SortOrder = field.SortOrder;
            Required = field.Required;
            Enabled = field.Enabled;
            SalesFunnelId = field.SalesFunnelId;
            if (HasValues && loadValues)
                FieldValues = LeadFieldService.GetLeadFieldValues(Id);
        }

        public List<LeadFieldValue> FieldValues { get; set; }

        public string FieldTypeFormatted { get { return FieldType.Localize(); } }
        public bool HasValues { get { return FieldType == LeadFieldType.Select; } }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name.IsNullOrEmpty())
                yield return new ValidationResult(LocalizationService.GetResource("Admin.LeadField.Errors.NameReauired"));
        }
    }
}
