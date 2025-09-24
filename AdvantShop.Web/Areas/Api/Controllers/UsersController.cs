using System;
using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Customers;
using AdvantShop.Areas.Api.Handlers.Users;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Areas.Api.Models.Users;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApiKeyByUser]
    public class UsersController : BaseApiController
    {
        // POST api/users/signin
        [HttpPost]
        public JsonResult SignIn(SignInModel model) => JsonApi(new SignInApi(model));

        // POST api/users/signInByPhone
        [HttpPost]
        public JsonResult SignInByPhone(SignInByPhoneModel model) => JsonApi(new SignInByPhoneApi(model));

        // POST api/users/signInByPhoneConfirmCode
        [HttpPost]
        public JsonResult SignInByPhoneConfirmCode(SignInByPhoneConfirmCodeModel model) =>
            JsonApi(new SignInByPhoneConfirmCodeApi(model));


        // GET api/users/me
        [HttpGet, AuthUserApi]
        public JsonResult Me() => JsonApi(new GetUserMe());

        // POST api/users/me
        [HttpPost, AuthUserApi]
        public JsonResult ChangeMe(AddUpdateCustomerModel model) => JsonApi(new ChangeMe(model));
        
        // GET api/users/me/bonuses
        [HttpGet, AuthUserApi, BonusSystem]
        public JsonResult MeBonuses() => JsonApi(new GetCustomerBonuses(CustomerContext.CustomerId, false));

        // POST api/users/me/smsphoneconfirmation
        [HttpPost, AuthUserApi]
        public JsonResult MeSmsPhoneConfirmation(bool? addHash) =>
            JsonApi(new SmsPhoneConfirmation(CustomerContext.CurrentCustomer, addHash));

        // POST api/users/me/smsphoneconfirmationcode
        [HttpPost, AuthUserApi]
        public JsonResult MeSmsPhoneConfirmationCode(string code) =>
            JsonApi(new SmsPhoneConfirmationCode(CustomerContext.CurrentCustomer, code));

        // GET api/users/me/customer-fields
        [HttpGet, AuthUserApi]
        public JsonResult MeCustomerFields(CustomerType? type) =>
            JsonApi(new GetCustomerFields(CustomerContext.CustomerId, type));

        // POST api/users/me/remove-account
        [HttpPost, AuthUserApi]
        public JsonResult MeRemoveAccount() => JsonApi(new MeRemoveAccount(CustomerContext.CustomerId));

        // POST api/users/me/fcmtoken
        [HttpPost, AuthUserApi]
        public JsonResult MeUpdateFcmToken(string token) =>
            JsonApi(new MeUpdateFcmToken(CustomerContext.CustomerId, token));
        
        // GET api/users/me/statistics
        [HttpGet, AuthUserApi]
        public JsonResult MeStatistics() => JsonApi(new GetUserMeStatistics());
        
        
        // GET api/users/me/contacts
        [HttpGet, AuthUserApi]
        public JsonResult MeGetContacts() => JsonApi(new MeGetContacts());
        
        // POST api/users/me/contacts/add
        [HttpPost, AuthUserApi]
        public JsonResult MeAddContact(CustomerContactModel model) => JsonApi(new MeAddUpdateContact(model, true));
        
        // POST api/users/me/contacts/{id}/update
        [HttpPost, AuthUserApi]
        public JsonResult MeUpdateContact(Guid id, CustomerContactModel model) => JsonApi(new MeAddUpdateContact(id, model, false));
        
        // POST api/users/me/contacts/{id}/delete
        [HttpPost, AuthUserApi]
        public JsonResult MeDeleteContact(Guid id) => JsonApi(new MeDeleteContact(id));
    }
}