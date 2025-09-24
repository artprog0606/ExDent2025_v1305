using AdvantShop.Core.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.CustomerSegments;
using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Crm.BusinessProcesses
{
    public enum EFieldComparerType
    {
        None = 0,
        Equal = 1,
        Range = 2,
        Flag = 3,
        Contains = 4,
        Products = 5,
        Categories = 6,
        CustomerSegment = 7,
        OrdersPaidSum = 8,
        OrdersCount = 9,
        OrdersPaidCount = 10,
        OpenLeadsInFunnel = 11,
        DealStatus = 12,
    }

    public abstract class FieldComparer
    {
        public virtual EFieldComparerType Type => EFieldComparerType.None;

        public int? FieldObjId { get; set; }

        public int? ValueObjId { get; set; }

        public virtual bool Check(string val)
        {
            return false;
        }

        public virtual bool Check(int val)
        {
            return false;
        }

        public virtual bool Check(int? val)
        {
            return val.HasValue ? Check(val.Value) : false;
        }

        public virtual bool Check(float val)
        {
            return false;
        }

        public virtual bool Check(float? val)
        {
            return val.HasValue ? Check(val.Value) : false;
        }

        public virtual bool Check(bool val)
        {
            return false;
        }

        public virtual bool Check(DateTime val)
        {
            return false;
        }

        public virtual bool Check(DateTime? val)
        {
            return val.HasValue ? Check(val.Value) : false;
        }

        public virtual bool Check(Guid val)
        {
            return false;
        }
    }

    public class FieldEqualityComparer : FieldComparer
    {
        public override EFieldComparerType Type => EFieldComparerType.Equal;

        public string Value { get; set; }

        public override bool Check(string val)
        {
            return (string.IsNullOrEmpty(val) && string.IsNullOrEmpty(Value)) || Value.Equals(val, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Check(int val)
        {
            var intVal = Value.TryParseInt(true); // if Value is not int - return false
            return intVal == val;
        }

        public override bool Check(int? val)
        {
            var intVal = Value.TryParseInt(true); // if Value is not int - return false
            return intVal == val;
        }

        public override bool Check(float val)
        {
            var floatVal = Value.TryParseFloat(true); // if Value is not float - return false
            return floatVal == val;
        }

        public override bool Check(float? val)
        {
            var floatVal = Value.TryParseFloat(true); // if Value is not float - return false
            return floatVal == val;
        }

        public override bool Check(bool val)
        {
            var boolVal = Value.TryParseBool(true); // if Value is not bool - return false
            return boolVal == val;
        }

        public override bool Check(DateTime val)
        {
            var dateTimeVal = Value.TryParseDateTime(true); // if Value is not DateTime - return false
            return dateTimeVal == val;
        }

        public override bool Check(DateTime? val)
        {
            var dateTimeVal = Value.TryParseDateTime(true); // if Value is not DateTime - return false
            return dateTimeVal == val;
        }

        public override bool Check(Guid val)
        {
            var guidVal = Value.TryParseGuid(true);
            return guidVal == val;
        }
    }

    public class FieldRangeComparer : FieldComparer
    {
        public override EFieldComparerType Type => EFieldComparerType.Range;

        public bool? ShowTime { get; set; }

        public bool? OnlyTime { get; set; }

        public float? From { get; set; }
        public float? To { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public string DateFromString =>
            DateFrom.HasValue
                ? (OnlyTime != null && OnlyTime.Value 
                    ? DateFrom.Value.ToString("HH:mm")
                    : DateFrom.Value.ToString(ShowTime != null && ShowTime.Value ? "dd.MM.yyyy HH:mm" : "dd.MM.yyyy"))
                : string.Empty;

        public string DateToString =>
            DateTo.HasValue
                ? (OnlyTime != null && OnlyTime.Value
                    ? DateTo.Value.ToString("HH:mm")
                    : DateTo.Value.ToString(ShowTime != null && ShowTime.Value ? "dd.MM.yyyy HH:mm" : "dd.MM.yyyy"))
                : string.Empty;

        public override bool Check(int val)
        {
            return (!From.HasValue || From.Value < val) && (!To.HasValue || To.Value >= val);
        }

        public override bool Check(float val)
        {
            return (!From.HasValue || From.Value < val) && (!To.HasValue || To.Value >= val);
        }

        public override bool Check(DateTime val)
        {
            if (OnlyTime != null && OnlyTime.Value)
            {
                return (!DateFrom.HasValue || (DateFrom.Value.Hour < val.Hour || (DateFrom.Value.Hour == val.Hour && DateFrom.Value.Minute <= val.Minute))) && 
                       (!DateTo.HasValue || (DateTo.Value.Hour > val.Hour || (DateTo.Value.Hour == val.Hour && DateTo.Value.Minute > val.Minute)));
            }

            return
                ShowTime != null && ShowTime.Value
                    ? (!DateFrom.HasValue || DateFrom.Value < val) && (!DateTo.HasValue || DateTo.Value >= val)
                    : (!DateFrom.HasValue || DateFrom.Value.Date < val.Date) && (!DateTo.HasValue || DateTo.Value.Date >= val.Date);
        }
    }

    public class FieldFlagComparer : FieldComparer
    {
        public override EFieldComparerType Type => EFieldComparerType.Flag;

        public bool Flag { get; set; }

        public override bool Check(string val)
        {
            return Flag == val.TryParseBool(true);
        }

        public override bool Check(bool val)
        {
            return Flag == val;
        }
    }

    public class FieldContainsComparer : FieldComparer
    {
        public override EFieldComparerType Type => EFieldComparerType.Contains;

        public string Value { get; set; }

        public override bool Check(string val)
        {
            return val.IsNotEmpty() && val.Contains(Value, StringComparison.InvariantCultureIgnoreCase);
        }
    }


    public class FieldItemModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class FieldsProductsComparer : FieldComparer
    {
        public override EFieldComparerType Type => EFieldComparerType.Products;

        public List<FieldItemModel> Products { get; set; }

        public override bool Check(int val)
        {
            return Products != null && Products.Count > 0 && Products.Find(x => x.Id == val) != null;
        }
    }

    public class FieldsCategoriesComparer : FieldComparer
    {
        public override EFieldComparerType Type => EFieldComparerType.Categories;

        public List<FieldItemModel> Categories { get; set; }

        public override bool Check(int val)
        {
            return Categories != null && Categories.Count > 0 && Categories.Find(x => x.Id == val) != null;
        }
    }

    public class FieldsCustomerSegmentComparer : FieldComparer
    {
        public override EFieldComparerType Type => EFieldComparerType.CustomerSegment;

        /// <summary>
        /// customer segment id
        /// </summary>
        public int Value { get; set; }

        public override bool Check(Guid val)
        {
            if (Value == 0)
                return false;

            var customerIds = CustomerSegmentService.GetCustomersBySegment(Value);

            return customerIds != null && customerIds.Contains(val);
        }
    }


    public enum EFieldComparerOrdersCustomerSubType
    {
        PaidSum,
        Count,
        PaidCount
    }

    public class FieldsOrdersCustomerSumCountComparer : FieldComparer
    {
        public override EFieldComparerType Type
        {
            get
            {
                switch (SubType)
                {
                    case EFieldComparerOrdersCustomerSubType.PaidSum:
                        return EFieldComparerType.OrdersPaidSum;
                    case EFieldComparerOrdersCustomerSubType.Count:
                        return EFieldComparerType.OrdersCount;
                    case EFieldComparerOrdersCustomerSubType.PaidCount:
                        return EFieldComparerType.OrdersPaidCount;
                }
                throw new NotImplementedException();
            }
        }

        public float? From { get; set; }
        public float? To { get; set; }
        public EFieldComparerOrdersCustomerSubType SubType { get; set; }

        public override bool Check(Guid val)
        {
            var value = 0f;
            switch (SubType)
            {
                case EFieldComparerOrdersCustomerSubType.PaidSum:
                    value = OrderStatisticsService.GetOrdersSum(val, true);
                    break;
                case EFieldComparerOrdersCustomerSubType.Count:
                    value = OrderStatisticsService.GetOrdersCount(val);
                    break;
                case EFieldComparerOrdersCustomerSubType.PaidCount:
                    value = OrderStatisticsService.GetOrdersCount(val, true);
                    break;
            }
            return (!From.HasValue || From.Value <= value) && (!To.HasValue || To.Value >= value);
        }
    }


    public class FieldsOpenLeadsInFunnelComparer : FieldComparer
    {
        public override EFieldComparerType Type => EFieldComparerType.OpenLeadsInFunnel;

        public int SalesFunnelId { get; set; }

        public string SalesFunnelName
        {
            get
            {
                var funnel = SalesFunnelService.Get(SalesFunnelId);
                return funnel != null ? funnel.Name : null;
            }
        }

        public int? DealStatusId { get; set; }

        public string DealStatusName
        {
            get
            {
                var status = DealStatusId != null ? DealStatusService.Get(DealStatusId.Value) : null;
                return status != null ? status.Name : "Любой";
            }
        }

        public override bool Check(Guid customerId)
        {
            var customerLeads = LeadService.GetLeadsByCustomer(customerId).Where(x => x.SalesFunnelId == SalesFunnelId).ToList();
            
            return DealStatusId != null
                ? customerLeads.Any(x => x.DealStatusId == DealStatusId.Value)
                : customerLeads.Any();
        }
    }

    public class FieldsDealStatusComparer : FieldComparer
    {
        public override EFieldComparerType Type => EFieldComparerType.DealStatus;

        public int SalesFunnelId { get; set; }

        public string SalesFunnelName
        {
            get
            {
                var funnel = SalesFunnelService.Get(SalesFunnelId);
                return funnel != null ? funnel.Name : null;
            }
        }

        public int? DealStatusId { get; set; }

        public string DealStatusName
        {
            get
            {
                var status = DealStatusId != null ? DealStatusService.Get(DealStatusId.Value) : null;
                return status != null ? status.Name : "Любой";
            }
        }

        public override bool Check(int dealStatusId)
        {
            return DealStatusId != null && DealStatusId == dealStatusId;
        }
    }
}
