using System;
using System.Collections.Generic;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Shared.Common;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Customers
{
    public class CustomersFilterModel : BaseFilterModel<Guid>
    {
        public Role? Role { get; set; }
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int? ManagerId { get; set; }
        public string LastOrderNumber { get; set; }
        public float? OrdersCountFrom { get; set; }
        public float? OrdersCountTo { get; set; }
        public string Location { get; set; }
        public string Country { get; set; }
        public float? OrderSumFrom { get; set; }
        public float? OrderSumTo { get; set; }
        public string RegistrationDateTimeFrom { get; set; }
        public string RegistrationDateTimeTo { get; set; }
        public int? LastOrderFrom { get; set; }
        public int? LastOrderTo { get; set; }

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
        public CustomerType? CustomerType { get; set; }
    }
}
