using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Crm.BusinessProcesses;

namespace AdvantShop.Core.Services.Crm.LeadFields
{
    public enum LeadFieldType
    {
        [Localize("Core.Crm.LeadFields.ELeadFieldType.Select"), FieldType(EFieldType.Select)]
        Select = 0,
        [Localize("Core.Crm.LeadFields.ELeadFieldType.Text"), FieldType(EFieldType.Text)]
        Text = 1,
        [Localize("Core.Crm.LeadFields.ELeadFieldType.Number"), FieldType(EFieldType.Number)]
        Number = 2,
        [Localize("Core.Crm.LeadFields.ELeadFieldType.TextArea"), FieldType(EFieldType.Text)]
        TextArea = 3,
        [Localize("Core.Crm.LeadFields.ELeadFieldType.Date"), FieldType(EFieldType.Date)]
        Date = 4
    }
}
