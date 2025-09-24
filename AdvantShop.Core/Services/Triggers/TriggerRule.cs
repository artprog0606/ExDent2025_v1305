using System;
using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Letters;
using AdvantShop.Mails;

namespace AdvantShop.Core.Services.Triggers
{
    public enum ETriggerEventType
    {
        None = 0,

        [Localize("Core.Services.Triggers.ETriggerEventType.OrderCreated")]
        [DescriptionKey("Core.Services.Triggers.ETriggerEventType.Description.OrderCreated")]
        OrderCreated = 1,

        [Localize("Core.Services.Triggers.ETriggerEventType.OrderStatusChanged")]
        [DescriptionKey("Core.Services.Triggers.ETriggerEventType.Description.OrderStatusChanged")]
        OrderStatusChanged = 2,

        [Localize("Core.Services.Triggers.ETriggerEventType.LeadCreated")]
        [DescriptionKey("Core.Services.Triggers.ETriggerEventType.Description.LeadCreated")]
        LeadCreated = 3,

        [Localize("Core.Services.Triggers.ETriggerEventType.LeadStatusChanged")]
        [DescriptionKey("Core.Services.Triggers.ETriggerEventType.Description.LeadStatusChanged")]
        LeadStatusChanged = 4,

        [Localize("Core.Services.Triggers.ETriggerEventType.CustomerCreated")]
        [DescriptionKey("Core.Services.Triggers.ETriggerEventType.Description.CustomerCreated")]
        CustomerCreated = 5,
        
        [Localize("Core.Services.Triggers.ETriggerEventType.OrderPaied")]
        [DescriptionKey("Core.Services.Triggers.ETriggerEventType.Description.OrderPaied")]
        OrderPaied = 6,

        [Localize("Core.Services.Triggers.ETriggerEventType.TimeFromLastOrder")]
        [DescriptionKey("Core.Services.Triggers.ETriggerEventType.Description.TimeFromLastOrder")]
        TimeFromLastOrder = 7,

        [Localize("Core.Services.Triggers.ETriggerEventType.SignificantDate")]
        [DescriptionKey("Core.Services.Triggers.ETriggerEventType.Description.SignificantDate")]
        SignificantDate = 8,

        [Localize("Core.Services.Triggers.ETriggerEventType.SignificantCustomerDate")]
        [DescriptionKey("Core.Services.Triggers.ETriggerEventType.Description.SignificantCustomerDate")]
        SignificantCustomerDate = 9,

        [Localize("Core.Services.Triggers.ETriggerEventType.InstallMobileApp")]
        [DescriptionKey("Core.Services.Triggers.ETriggerEventType.Description.InstallMobileApp")]
        InstallMobileApp = 10,

    }

    public enum ETriggerObjectType
    {
        None = 0,
        Order = 1,
        Lead = 2,
        Customer = 3,
    }

    public enum ETriggerProcessType
    {
        None = 0,
        Datetime = 1,
    }

    public class TriggerRule
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public int? EventObjId { get; set; }
        public int? EventObjValue { get; set; }

        public ITriggerParams TriggerParams { get; set; }

        private string _name;
        public virtual string Name
        {
            get => _name ?? (_name = EventType.Localize());
            set => _name = value;
        }

        public virtual ETriggerEventType EventType => ETriggerEventType.None;
        public virtual ETriggerObjectType ObjectType => ETriggerObjectType.None;
        public virtual ITriggerObjectFilter Filter { get; set; }

        public virtual ETriggerProcessType ProcessType => ETriggerProcessType.None;
        public virtual IEnumerable<ITriggerObject> GeTriggerObjects() { return null; }

        /// <summary>
        /// Срабатывает только 1 раз
        /// </summary>
        public bool WorksOnlyOnce { get; set; }

        public bool Enabled { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }


        private List<TriggerAction> _actions;
        public List<TriggerAction> Actions
        {
            get
            {
                if (_actions != null)
                    return _actions;

                _actions = Id != 0 ? TriggerActionService.GetTriggerActions(Id) : new List<TriggerAction>();
                return _actions;
            }
            set => _actions = value;
        }


        public virtual List<LetterFormatKey> AvailableVariables => null;

        public virtual string[] SendRequestParameters
        {
            get
            {
                switch (ObjectType)
                {
                    case ETriggerObjectType.Lead:
                        return new[]
                        {
                            "#LeadId#", "#Description#", "#ClientId#", "#ClientLastName#", "#ClientFirstName#",
                            "#ClientPatronymic#", "#ClientFullName#", "#ClientPhone#", "#ClientEmail#",
                            "#ClientAddress#", "#AdditionalFields#", "#Products#", "#Sum#", "#Status#",
                            "#ShippingMethod#"
                        };

                    case ETriggerObjectType.Order:
                        return new[]
                        {
                            "#OrderId#", "#Number#", "#IsPaid#", "#Sum#", "#Status#", "#OrderSource#", "#Currency#",
                            "#ShippingMethod#", "#PaymentMethod#", "#ClientId#", "#ClientFirstName#",
                            "#ClientLastName#", "#ClientPatronymic#", "#ClientOrganization#", "#ClientPhone#", 
                            "#ClientEmail#", "#ClientCountry#", "#ClientRegion#", "#ClientCity#", "#ClientDistrict#", 
                            "#ClientZip#", "#ClientStreet#", "#ClientHouse#", "#ClientApartment#", "#ClientStructure#",
                            "#ClientEntrance#", "#ClientFloor#", "#ClientAddress#", "#ClientCustomField1#", 
                            "#ClientCustomField2#", "#ClientCustomField3#", "#AdditionalFields#", 
                            "#ShippingCost#", "#PaymentCost#", "#BonusCost#", "#OrderDiscountPercent#", 
                            "#OrderDiscountValue#", "#BonusCardNumber#", "#DeliveryDate#", "#DeliveryTime#", 
                            "#TotalWeight#", "#TotalLength#", "#TotalWidth#", "#TotalHeight#", "#Products#", 
                            "#TrackNumber#", "#Comments#", "#StatusComment#", "#AdminComment#",
                        };

                    case ETriggerObjectType.Customer:
                        return new[]
                        {
                            "#ClientId#", "#ClientFirstName#", "#ClientLastName#", "#ClientPatronymic#", "#ClientPhone#",
                            "#ClientEmail#", "#ClientOrganization#", "#AdditionalFields#"
                        };
                }
                return null;
            }
        }

        public virtual string[] SendToShippingServiceVariables
        {
            get
            {
                switch (ObjectType)
                {
                    case ETriggerObjectType.Order:
                        return new[]
                        {
                            "#OrderId#", "#Number#", "#IsPaid#", "#Sum#", "#Status#", "#ShippingMethod#",
                            "#PaymentMethod#", "#ErrorMessage#",
                        };
                }
                return null;
            }
        }

        protected MailTemplate _mail;

        public virtual string ReplaceVariables(string value, ITriggerObject triggerObject, Coupon couponTemplate, string triggerCouponCode)
        {
            return value;
        }

        public virtual Dictionary<string, string> GetFormattedParameters(string value, ITriggerObject triggerObject,
                                                                            Coupon coupon, string triggerCouponCode)
        {
            return null;
        }

        public virtual TriggerMailFormat GetDefaultMailTemplate()
        {
            return null;
        }

        /// <summary>
        /// Предпочтительное время срабатывания триггера 
        /// </summary>
        public int? PreferredHour { get; set; }


        private Coupon _coupon;
        public Coupon Coupon
        {
            get
            {
                if (_coupon != null || Id == 0)
                    return _coupon;

                return _coupon = CouponService.GetCouponByTrigger(Id);
            }
        }
    }



    public class TriggerRuleShortDto
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Name { get; set; }
        public string Description => EventType.DescriptionKey();
        public ETriggerEventType EventType { get; set; }
        public string EventTypeStr => EventType.ToString();
        public bool Enabled { get; set; }
    }
}
