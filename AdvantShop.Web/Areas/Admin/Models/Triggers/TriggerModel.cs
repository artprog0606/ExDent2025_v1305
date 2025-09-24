using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Messengers;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Letters;
using AdvantShop.MobileApp;
using AdvantShop.Saas;
using AdvantShop.Trial;

namespace AdvantShop.Web.Admin.Models.Triggers
{
    public class TriggerDataModel
    {
        private readonly List<ETriggerEventType> _eventTypes;

        public TriggerDataModel() : this(null)
        {
        }

        public TriggerDataModel(ETriggerObjectType[] objectTypes)
        {
            _eventTypes = new List<ETriggerEventType>();
            if (objectTypes != null)
            {
                foreach (var objectType in objectTypes)
                {
                    switch (objectType)
                    {
                        case ETriggerObjectType.Order:
                            _eventTypes.AddRange(new[] { ETriggerEventType.OrderCreated, ETriggerEventType.OrderStatusChanged, ETriggerEventType.OrderPaied });
                            break;
                        case ETriggerObjectType.Lead:
                            _eventTypes.AddRange(new[] { ETriggerEventType.LeadCreated, ETriggerEventType.LeadStatusChanged });
                            break;
                        case ETriggerObjectType.Customer:
                            _eventTypes.AddRange(new[] { ETriggerEventType.CustomerCreated, ETriggerEventType.SignificantCustomerDate, ETriggerEventType.SignificantDate, ETriggerEventType.TimeFromLastOrder });
                            break;
                    }
                }
            }
            if (!_eventTypes.Any())
                _eventTypes = new List<ETriggerEventType>(Enum.GetValues(typeof(ETriggerEventType)).Cast<ETriggerEventType>().Where(et => et != ETriggerEventType.None));

            EventTypes = new List<TriggerRuleShortDto>(_eventTypes.Select(item => new TriggerRuleShortDto()
            {
                Id = (int)item,
                EventType = item,
                Name = item.Localize()
            }));

            IsLicense = !TrialService.IsTrialEnabled && !SaasDataService.IsSaasEnabled;

            PreferredHours = new List<SelectItemModel<int>>();
            for (var i = 0; i < 24; i++)
                PreferredHours.Add(new SelectItemModel<int>(string.Format("{0:00}:00 {1}", i, !IsLicense ? "МСК" : ""), i));

            SinceOptions = new List<SelectItemModel<int>>();
            foreach (TriggerParamsDateSince item in Enum.GetValues(typeof(TriggerParamsDateSince)))
                SinceOptions.Add(new SelectItemModel<int>(item.Localize(), (int)item));


            var module = SmsNotifier.GetActiveSmsModule();
            IsSmsActive = module != null;
            IsSmsAndSocialMediaActive = module != null && module is ISmsAndSocialMediaService;
            
            IsWazzupActive = MessengerServices.GetActiveModules() != null;
            EmailSettingsError = MailService.ValidateMailSettingsBeforeSending();

            var notificationModule = NotificationService.GetActiveModule();
            if (notificationModule != null)
            {
                var (Result, Messages) = notificationModule.AllowSendNotification();
                AllowSendNotification = Result;
                PushNotificationErrorMessage = string.Join(". ", Messages);
            }
        }

        public List<TriggerRuleShortDto> EventTypes { get; set; }
        
        public List<SelectItemModel> EventObjectGroups { get; set; }
        public List<SelectItemModel> EventObjects { get; set; }
        public List<SelectItemModel> Categories { get; set; }
        public string EventObjectsFetchUrl { get; set; }
        public List<SelectItemModel> IntervalTypes { get; set; }
        public List<LetterFormatKey> AvailableVariables { get; set; }
        public TriggerMailFormat DefaultMailTemplate { get; set; }

        public List<SelectItemModel<int>> SinceOptions { get; private set; }
        public List<SelectItemModel<int>> PreferredHours { get; private set; }
        public string ProcessType { get; set; }
        public bool IsSmsActive { get; private set; }
        public bool IsSmsAndSocialMediaActive { get; private set; }
        public bool IsWazzupActive { get; private set; }
        public bool AllowSendNotification { get; private set; }
        public string PushNotificationErrorMessage { get; private set; }

        public bool IsLicense { get; private set; }

        public string EmailSettingsError { get; private set; }
        public string[] SendRequestParameters { get; set; }
        public string[] SendToShippingServiceVariables { get; set; }
        public bool IsTriggerForOrder { get; set; }
        public string ShippingMethodsUseUnloadOrder { get; set; }
    }

    public class TriggerModel
    {
        public int Id { get; set; }
        public int? EventObjId { get; set; }
        public int? EventObjValue { get; set; }
        public int? CategoryId { get; set; }

        public string Name { get; set; }
        public bool WorksOnlyOnce { get; set; }

        public ETriggerEventType EventType { get; set; }
        public string EventTypeDescription => EventType.DescriptionKey();
        public ETriggerObjectType ObjectType { get; set; }


        public virtual ITriggerObjectFilter Filter { get; set; }
        public string FilterSerialized { get; set; }

        public List<TriggerAction> Actions { get; set; }
        
        public ITriggerParams TriggerParams { get; set; }
        public string TriggerParamsSerialized { get; set; }
        public int? PreferredHour { get; set; }
        public string ProcessType { get; set; }
        public Coupon Coupon { get; set; }

        // used in add trigger
        public int? OrderSourceId { get; set; }
    }

}
