using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Customers;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Landing.Forms
{
    public class LpFormField
    {
        public string Title { get; set; }
        public string TitleCrm { get; set; }

        public ELpFormFieldType FieldType { get; set; }

        public string Type
        {
            get { return FieldType.FieldType().ToString().ToLower(); }
        }

        public int? CustomFieldId { get; set; }

        [JsonIgnore]
        private CustomerFieldWithValue _customerField;

        [JsonIgnore]
        public CustomerFieldWithValue CustomerField
        {
            get
            {
                if (FieldType != ELpFormFieldType.CustomerField || CustomFieldId == null)
                    return null;
                
                return _customerField ?? (_customerField = CustomerFieldService.GetCustomerFieldsWithValue(CustomFieldId.Value));
            }
            set { _customerField = value; }
        }

        [JsonIgnore]
        private LeadFieldWithValue _leadField;
        [JsonIgnore]
        public LeadFieldWithValue LeadField
        {
            get
            {
                if (FieldType != ELpFormFieldType.LeadField || CustomFieldId == null)
                    return null;

                return _leadField ?? (_leadField = LeadFieldService.GetLeadFieldWithValue(CustomFieldId.Value));
            }
            set { _leadField = value; }
        }

        public bool Required { get; set; }
    }


    public enum ELpFormFieldType
    {
        [Localize("App.Landing.Domain.Forms.ELpFormFieldType.None")] None = 0,
        [Localize("App.Landing.Domain.Forms.ELpFormFieldType.LastName"), FieldType(EFieldType.Text)] LastName = 1,
        [Localize("App.Landing.Domain.Forms.ELpFormFieldType.FirstName"), FieldType(EFieldType.Text)] FirstName = 2,
        [Localize("App.Landing.Domain.Forms.ELpFormFieldType.Patronymic"), FieldType(EFieldType.Text)] Patronymic = 3,
        [Localize("App.Landing.Domain.Forms.ELpFormFieldType.CustomerField")] CustomerField = 4,
        [Localize("App.Landing.Domain.Forms.ELpFormFieldType.Email"), FieldType(EFieldType.Text)] Email = 5,
        [Localize("App.Landing.Domain.Forms.ELpFormFieldType.Phone"), FieldType(EFieldType.Tel)] Phone = 6,
        [Localize("App.Landing.Domain.Forms.ELpFormFieldType.Country"), FieldType(EFieldType.Text)] Country = 7,
        [Localize("App.Landing.Domain.Forms.ELpFormFieldType.Region"), FieldType(EFieldType.Text)] Region = 8,
        [Localize("App.Landing.Domain.Forms.ELpFormFieldType.City"), FieldType(EFieldType.Text)] City = 9,
        [Localize("App.Landing.Domain.Forms.ELpFormFieldType.Comment"), FieldType(EFieldType.TextArea)] Comment = 10,
        [Localize("App.Landing.Domain.Forms.ELpFormFieldType.TextArea"), FieldType(EFieldType.TextArea)] TextArea = 11,
        [Localize("App.Landing.Domain.Forms.ELpFormFieldType.Address"), FieldType(EFieldType.Text)] Address = 12,
        [Localize("App.Landing.Domain.Forms.ELpFormFieldType.LeadField")] LeadField = 13,
        [Localize("App.Landing.Domain.Forms.ELpFormFieldType.Picture"), FieldType(EFieldType.Picture)] Picture = 14,
        [Localize("App.Landing.Domain.Forms.ELpFormFieldType.Birthday"), FieldType(EFieldType.Date)] Birthday = 15,
        [Localize("App.Landing.Domain.Forms.ELpFormFieldType.FileArchive"), FieldType(EFieldType.FileArchive)] FileArchive = 16,
    }
}
