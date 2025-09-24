using AdvantShop.Customers;
using AdvantShop.Orders;
using System;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Orders.OrdersEdit
{
    public class ClientInfoModel
    {
        public Customer Customer { get; set; }
        public Order Order { get; set; }
        public List<string> InterestingCategories { get; set; }
        public string CustomerGroup { get; set; }
        public string CustomerId { get; set; }
        public int OrderId { get; set; }
        public string Segment { get; set; }
        public ClientStatistic Statistic { get; set; }

        private string _customerCompanyName;
        public string CustomerCompanyName
        {
            get
            {
                if (!string.IsNullOrEmpty(_customerCompanyName))
                    return _customerCompanyName;
                if (Customer == null || Customer.Id == Guid.Empty || Customer.CustomerType != CustomerType.LegalEntity)
                    return string.Empty;
                var customerField = CustomerFieldService.GetCustomerFieldWithValueByFieldAssignment(Customer.Id, CustomerFieldAssignment.CompanyName);
                _customerCompanyName = customerField?.Value;

                return customerField?.Value;
            }
        }
    }

    public class ClientStatistic
    {
        public string AdminCommentAboutCustomer { get; set; }
        public string CustomerId { get; set; }
        public int OrdersCount { get; set; }
        public int OrderId { get; set; }
        public string RegistrationDuration { get; internal set; }
        public string OrdersSum { get; set; }
        public string AverageCheck { get; set; }
        public string RegistrationDate { get; set; }
    }

}
