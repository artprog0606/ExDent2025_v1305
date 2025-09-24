using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Customers;

namespace AdvantShop.Web.Admin.Models.Triggers
{
    public class FieldSelectItem
    {
        public int type { get; set; }
        public string typeStr { get; set; }
        public string name { get; set; }
        public int? objId { get; set; }
        public string fieldType { get; set; }
        public string groupName { get; set; }
        public int sortOrder { get; set; }

        public static explicit operator FieldSelectItem(Enum fieldType)
        {
            var fieldTypeValue = fieldType.FieldTypeValue();

            return new FieldSelectItem
            {
                type = Convert.ToInt32(fieldType),
                typeStr = fieldType.ToString().ToLower(),
                name = fieldType.Localize(),
                fieldType = fieldTypeValue.Type.ToString().ToLower(),
                groupName = fieldTypeValue.GroupName,
                sortOrder = fieldTypeValue.SortOrder
            };
        }

        public static explicit operator FieldSelectItem(CustomerField customerField)
        {
            var item = new FieldSelectItem
            {
                type = (int) EOrderFieldType.CustomerField,
                typeStr = EOrderFieldType.CustomerField.ToString().ToLower(),
                name = customerField.Name,
                objId = customerField.Id,
                groupName = "Пользовательские поля",
                sortOrder = ((int) EOrderFieldType.CustomerField)*10,
                fieldType = customerField.FieldType.FieldType().ToString().ToLower()
            };
            return item;
        }

        public static explicit operator FieldSelectItem(LeadField leadField)
        {
            var salesFunnel = SalesFunnelService.Get(leadField.SalesFunnelId);
            var item = new FieldSelectItem
            {
                type = (int)ELeadFieldType.LeadField,
                typeStr = ELeadFieldType.LeadField.ToString().ToLower(),
                name = leadField.Name,
                objId = leadField.Id,
                groupName = (salesFunnel != null ? salesFunnel.Name + ". ": null) + "Доп. поля",
                sortOrder = ((int)ELeadFieldType.LeadField) * 10,
                fieldType = leadField.FieldType.FieldType().ToString().ToLower()
            };
            return item;
        }
    }
}