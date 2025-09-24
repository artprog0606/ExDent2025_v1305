using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Admin;
using System;
using System.Collections.Generic;

namespace AdvantShop.Areas.Api.Models.Customers
{
    public class CustomersFilterModel : BaseFilterModel<Guid>
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int? ManagerId { get; set; }
        public string City { get; set; }
        public string LastOrderNumber { get; set; }
        public float? OrdersCountFrom { get; set; }
        public float? OrdersCountTo { get; set; }        
        public float? OrdersSumFrom { get; set; }
        public float? OrdersSumTo { get; set; }
        public string RegistrationDateTimeFrom { get; set; }
        public string RegistrationDateTimeTo { get; set; }
        public string LastOrderDateTimeFrom { get; set; }
        public string LastOrderDateTimeTo { get; set; }
        public int? AverageCheckFrom { get; set; }
        public int? AverageCheckTo { get; set; }

        /// <summary>
        /// Тип в соц. сети
        /// </summary>
        public string SocialType { get; set; }

        public bool? Subscription { get; set; }
        public bool? HasBonusCard { get; set; }

        public Dictionary<string, CustomerFiledFilterModel> CustomerFields { get; set; }

        public List<int> Tags { get; set; }

        public int? CustomerSegment { get; set; }

        public bool? Extended { get; set; }
    }

    public class CustomerFiledFilterModel
    {
        public string Value { get; set; }
        public string ValueExact { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}