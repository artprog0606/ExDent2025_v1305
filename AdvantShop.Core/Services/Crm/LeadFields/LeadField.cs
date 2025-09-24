using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Core.Services.Crm.LeadFields
{
    public class LeadField
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public LeadFieldType FieldType { get; set; }
        public int SortOrder { get; set; }
        public bool Required { get; set; }
        public bool Enabled { get; set; }
        public int SalesFunnelId { get; set; }
    }

    public class LeadFieldWithValue : LeadField
    {
        private string _value;
        public string Value
        {
            get
            {
                if (FieldType == LeadFieldType.Date)
                {
                    var dt = _value.TryParseDateTime(true);
                    return dt.HasValue ? dt.Value.ToString("yyyy-MM-dd") : null;
                }
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public string ValueDateFormat
        {
            get
            {
                if (FieldType == LeadFieldType.Date)
                {
                    var dt = _value.TryParseDateTime(true);
                    return dt.HasValue ? dt.Value.ToString("dd.MM.yyyy") : null;
                }
                return null;
            }
        }

        public List<SelectListItem> Values
        {
            get
            {
                if (FieldType == LeadFieldType.Select)
                {
                    var list = new List<SelectListItem>();

                    var items = LeadFieldService.GetLeadFieldValues(Id);
                    if (items != null && items.Count > 0)
                    {
                        list.AddRange(items.Select(x => new SelectListItem()
                        {
                            Text = x.Value,
                            Value = x.Value,
                            Selected = x.Value == Value
                        }));
                    }

                    return list;
                }

                return new List<SelectListItem>();
            }
        }
    }
}
