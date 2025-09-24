using System;
using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Bonuses;
using AdvantShop.Areas.Api.Handlers.Customers;
using AdvantShop.Areas.Api.Models.Bonuses;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, BonusSystem]
    public class BonusCardsController : BaseApiController
    {
        // Получение бонусной карты по номеру
        // GET api/bonus-cards/{id}
        [HttpGet, AuthApiKey]
        public JsonResult Card(long id) => JsonApi(new GetCustomerBonuses(id));

        // Создание бонусной карты для покупателя
        // POST api/bonus-cards/add
        [HttpPost, AuthApiKey]
        public JsonResult Create(Guid customerId) => JsonApi(new CreateBonusCard(customerId));

        
        // Список транзакций по карте с пагинацией
        // GET api/bonus-cards/{id}/transactions
        [HttpGet, AuthApiKey]
        public JsonResult Transactions(long id, TransactionsFilterModel filter) => JsonApi(new GetTransactions(id, filter));

        
        // Начисление основных бонусов
        // POST api/bonus-cards/{id}/main-bonuses/accept
        [HttpPost, AuthApiKey, Obsolete]
        public JsonResult AcceptMainBonuses(long id, MainBonusModel model) => JsonApi(new AcceptMainBonuses(id, model, true));

        // Списание основных бонусов
        // POST api/bonus-cards/{id}/main-bonuses/substract
        [HttpPost, AuthApiKey, Obsolete]
        public JsonResult SubstractMainBonuses(long id, MainBonusModel model) => JsonApi(new AcceptMainBonuses(id, model, false));


        // Получение дополнительных бонусов
        // GET api/bonus-cards/{id}/additional-bonuses
        [HttpGet, AuthApiKey, Obsolete]
        public JsonResult GetAdditionalBonuses(long id) => JsonApi(new GetAdditionalBonuses(id));

        // Начисление дополнительных бонусов
        // POST api/bonus-cards/{id}/additional-bonuses/accept
        [HttpPost, AuthApiKey, Obsolete]
        public JsonResult AcceptAdditionalBonuses(long id, AddAdditionalBonusModel model) => JsonApi(new AcceptAdditionalBonuses(id, model));

        // Списание дополнительных бонусов
        // POST api/bonus-cards/{id}/additional-bonuses/substract
        [HttpPost, AuthApiKey, Obsolete]
        public JsonResult SubstractAdditionalBonuses(long id, SubctractAdditionalBonusModel model) => JsonApi(new SubtractAddAditionalBonuses(id, model));


        // Получение дополнительных бонусов
        // GET api/bonus-cards/{id}/bonuses
        [HttpGet, AuthApiKey]
        public JsonResult GetBonuses(long id) => JsonApi(new GetBonuses(id));

        // Начисление дополнительных бонусов
        // POST api/bonus-cards/{id}/bonuses/accept
        [HttpPost, AuthApiKey]
        public JsonResult AcceptBonuses(long id, AcceptBonusModel model) => JsonApi(new AcceptBonuses(id, model));

        // Списание дополнительных бонусов
        // POST api/bonus-cards/{id}/bonuses/substract
        [HttpPost, AuthApiKey]
        public JsonResult SubstractBonuses(long id, SubstractBonusModel model) => JsonApi(new SubstractBonuses(id, model));

        // Списание дополнительных бонусов
        // POST api/bonus-cards/{id}/bonuses/substract-free-amount
        [HttpPost, AuthApiKey]
        public JsonResult SubstractFreeAmountBonuses(long id, SubstractFreeAmountBonusModel model) => JsonApi(new SubstractFreeAmountBonuses(id, model));


        // Настройки бонусной системы
        // GET api/bonus-cards/settings
        [HttpGet, AuthApiKey]
        public JsonResult GetSettings() => JsonApi(new GetBonusSystemSettings());

        // Сохранение настроек бонусной системы
        // POST api/bonus-cards/settings
        [HttpPost, AuthApiKey]
        public JsonResult SaveSettings(BonusSystemSettings model) => JsonApi(new SaveBonusSystemSettings(model));
        
        
        // Создание бонусной карты для авторизованного покупателя
        // POST api/bonus-cards/me/add
        [HttpPost, AuthApiKeyByUser, AuthUserApi]
        public JsonResult MeCreate() => JsonApi(new CreateBonusCard(CustomerContext.CustomerId));
        
        // POST api/bonus-cards/me/transactions
        [HttpGet, AuthApiKeyByUser, AuthUserApi]
        public JsonResult GetTransactionsMe() => JsonApi(new GetTransactionsMe());
    }
}