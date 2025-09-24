using System;
using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Customers;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKey]
    public class CustomersController : BaseApiController
    {
        // GET api/customers
        [HttpGet]
        public JsonResult Filter(CustomersFilterModel filter) => JsonApi(new GetCustomers(filter));

        // GET api/customers/{id}
        [HttpGet]
        public JsonResult Get(Guid id) => JsonApi(new GetCustomer(id));

        // POST api/customers/add (Put not work in asp.net mvc)
        [HttpPost]
        public JsonResult Add(AddUpdateCustomerModel model, AddUpdateCustomerCommand command) =>
            JsonApi(new AddUpdateCustomer(model, command));

        // POST api/customers/{id}
        [HttpPost]
        public JsonResult Update(Guid id, AddUpdateCustomerModel model, AddUpdateCustomerCommand command) => 
            JsonApi(new AddUpdateCustomer(id, model, command));

        // POST api/customers/smsphoneconfirmation {"phone": "79000000000"}
        [HttpPost]
        public JsonResult SmsPhoneConfirmation(string phone, bool? addHash) => JsonApi(new SmsPhoneConfirmation(phone, addHash));
        
        // POST api/customers/smsphoneconfirmationcode {"phone": "79000000000", "code": "1234"}
        [HttpPost]
        public JsonResult SmsPhoneConfirmationCode(SmsPhoneConfirmationCodeModel model) => JsonApi(new SmsPhoneConfirmationCode(model));

        // GET api/customers/{id}/bonuses
        [HttpGet, BonusSystem]
        public JsonResult Bonuses(Guid id) => JsonApi(new GetCustomerBonuses(id, false));
    }
}