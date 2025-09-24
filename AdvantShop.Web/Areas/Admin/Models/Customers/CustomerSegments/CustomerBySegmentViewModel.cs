using System;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Localization;

namespace AdvantShop.Web.Admin.Models.Customers.CustomerSegments
{
    public class CustomerBySegmentViewModel
    {
        public int Id { get; set; }

        public Guid CustomerId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Organization { get; set; }


        public string Name
        {
            get { return string.IsNullOrEmpty(CompanyName) ? StringHelper.AggregateStrings(" ", LastName, FirstName, Patronymic) : CompanyName; }
        }

        public int OrdersCount { get; set; }
        public float OrdersSum { get; set; }
        public DateTime RegistrationDateTime { get; set; }
        public string RegistrationDateTimeFormatted { get { return Culture.ConvertDate(RegistrationDateTime); } }

        public DateTime? BirthDay { get; set; }

        public string BirthDayFormatted
        {
            get { return BirthDay != null ? Culture.ConvertDateWithoutHours(BirthDay.Value) : null; }
        }


        public int LastOrderId { get; set; }
        public string LastOrderNumber { get; set; }
        public string Location { get; set; }
        public string ManagerName { get; set; }
        public CustomerType CustomerType { get; set; }
        public string CompanyName { get; set; }
    }
}
