using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Api;
using AdvantShop.Customers;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.Customers
{

    public class GetCustomerResponse : CustomerModel, IApiResponse
    {
        [JsonProperty(Order = 100)]
        public string FcmToken { get; }
        
        public GetCustomerResponse()
        { 
        }

        public GetCustomerResponse(Customer customer)
        {
            Id = customer.Id;
            Email = customer.EMail;
            Phone = customer.StandardPhone;
            FirstName = customer.FirstName ?? "";
            LastName = customer.LastName ?? "";
            Patronymic = customer.Patronymic ?? "";
            Organization = customer.Organization ?? "";
            BirthDay = customer.BirthDay;
            AdminComment = customer.AdminComment ?? "";
            Contact = customer.Contacts != null && customer.Contacts.Count > 0
                ? new CustomerContactModel(customer.Contacts.OrderByDescending(x => x.IsMain).FirstOrDefault())
                : new CustomerContactModel();
            Contacts = customer.Contacts != null && customer.Contacts.Count > 0
                ? customer.Contacts.Select(x => new CustomerContactModel(x)).ToList()
                : new List<CustomerContactModel>();

            ManagerId = customer.ManagerId;
            GroupId = customer.CustomerGroupId;
            Group = new CustomerGroupApi(customer.CustomerGroup);

            CustomerType = customer.CustomerType.ToString();

            IsAgreeForPromotionalNewsletter = customer.IsAgreeForPromotionalNewsletter;

            Fields = CustomerFieldService.GetCustomerFieldsWithValue(customer.Id)
                .Where(x => x.CustomerType == AdvantShop.Customers.CustomerType.All || x.CustomerType == customer.CustomerType)
                .Select(x => new CustomerFieldModel(x))
                .ToList();

            FcmToken = CustomerService.GetFcmToken(customer.Id).Default(null);
        }
    }
}