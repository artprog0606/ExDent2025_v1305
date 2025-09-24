using System;
using AdvantShop.Core;
using AdvantShop.Core.Services.Crm.Instagram;
using AdvantShop.Core.Services.Crm.Ok;
using AdvantShop.Core.Services.Crm.Telegram;
using AdvantShop.Core.Services.Crm.Vk;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Shared
{
    public class ChangeSocialCustomer : AbstractCommandHandler
    {
        private readonly Guid _fromCustomerId;
        private readonly Guid _toCustomerId;
        private readonly string _type;
        private Customer _fromCustomer;
        private Customer _toCustomer;

        public ChangeSocialCustomer(Guid fromCustomerId, Guid toCustomerId, string type)
        {
            _fromCustomerId = fromCustomerId;
            _toCustomerId = toCustomerId;
            _type = type;
        }
        
        protected override void Validate()
        {
            _fromCustomer = CustomerService.GetCustomer(_fromCustomerId);
            if (_fromCustomer == null)
                throw new BlException("Пользователь не найден");
            
            _toCustomer = CustomerService.GetCustomer(_toCustomerId);
            if (_toCustomer == null)
                throw new BlException("Пользователь не найден");
            
            if (_fromCustomerId == _toCustomerId)
                throw new BlException("Выбран один и тот же пользователь");
        }
        
        protected override void Handle()
        {
            switch (_type)
            {
                case "telegram":
                    new TelegramService().ChangeCustomer(_fromCustomerId, _toCustomerId);
                    break;
                case "vk":
                    VkService.ChangeCustomer(_fromCustomerId, _toCustomerId);
                    break;
                case "instagram":
                    InstagramService.ChangeCustomer(_fromCustomerId, _toCustomerId);
                    break;
                case "ok":
                    OkService.ChangeCustomer(_fromCustomerId, _toCustomerId);
                    break;
            }
        }
    }
}