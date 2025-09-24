using System;
using System.Collections.Generic;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Shipping
{
    public abstract class AbstractShippingOption : IShippingOption
    {
        public virtual string Id => MethodId + "_" + (Name + MethodId + DeliveryId).GetHashCode();

        public int DeliveryId { get; set; }
        public int MethodId { get; set; }
        public string Name { get; set; }
        public string Hint { get; set; }
        public string Desc { get; set; }
        public bool DisplayCustomFields { get; set; }
        public bool DisplayIndex { get; set; }

        private string _iconName;

        public string IconName
        {
            get
            {
                var newName = this.GetPropValue<string>(nameof(Name));

                _iconName = _iconName ?? ShippingIcons.GetShippingIcon(ShippingType, null, newName ?? Name);
                return _iconName;
            }
            set => _iconName = value;
        }

        public bool ShowInDetails { get; set; }
        public string ZeroPriceMessage { get; set; }
        public int? TaxId { get; set; }
        public ePaymentMethodType PaymentMethodType { get; set; } = ePaymentMethodType.full_prepayment;
        public ePaymentSubjectType PaymentSubjectType { get; set; } = ePaymentSubjectType.payment;
        public string ShippingType { get; set; }

        public string NameRate { get; set; }
        public bool HideAddressBlock { get; set; }

        private float _rate;

        public float Rate
        {
            get => _rate;
            set
            {
                _rate = value;
                _finalRate = null;
            }
        }

        //for Admin
        public float ManualRate { get; set; }

        public bool UseExtracharge { get; set; }
        public float ExtrachargeInNumbers { get; set; }
        public float ExtrachargeInPercents { get; set; }
        public bool ExtrachargeFromOrder { get; set; }

        public float PreCost { get; set; }// должно приходить в валюте доставки (ShippingCurrency)
        public Currency ShippingCurrency { get; set; }

        private Currency _currentCurrency;

        public Currency CurrentCurrency
        {
            get => _currentCurrency;
            set
            {
                _currentCurrency = value;
                _finalRate = null;
            }
        }

        public float GetExtracharge()
        {
            return GetExtracharge(ExtrachargeInNumbers, ExtrachargeInPercents,
                ExtrachargeFromOrder
                    ? PreCost
                    : Rate);
        }

        public float GetExtracharge(float extrachargeInNumbers, float extrachargeInPercents, float coast)
        {
            var extracharge = 0f;

            if (extrachargeInNumbers != 0)
                extracharge += CurrencyService.ConvertCurrency(extrachargeInNumbers, ShippingCurrency != null
                    ? ShippingCurrency.Rate
                    : 1f, 1f);// фиксированная наценка в базовой валюте

            if (extrachargeInPercents != 0)
                extracharge += extrachargeInPercents * coast / 100;

            return extracharge;
        }

        private float? _finalRate;

        public float FinalRate
        {
            get
            {
                if (!_finalRate.HasValue)
                {
                    _finalRate = (Rate > 0f && UseExtracharge
                            ? Rate + GetExtracharge()
                            : Rate)
                        .RoundPrice(ShippingCurrency?.Rate ?? 1f, CurrentCurrency);
                    if (_finalRate.Value < 0f) _finalRate = 0f;
                }

                return _finalRate.Value;
            }
        }

        public string FormatRate =>
            FinalRate == 0
                ? ZeroPriceMessage
                : CurrentCurrency == null
                    ? FinalRate.FormatPrice(false, false)
                    : FinalRate.FormatPrice(CurrentCurrency);

        public string DeliveryTime { get; set; }

        public int ExtraDeliveryTime { get; set; }

        public Type ModelType => this.GetType();

        public bool UseDeliveryInterval { get; set; }

        public bool ShowSoonest { get; set; }

        public EnTypeOfDelivery? TypeOfDelivery { get; set; }

        public string DeliveryIntervalsStr { get; set; }

        public string DateOfDeliveryStr { get; set; }
        
        public string MinDate { get; set; }

        public string MaxDate { get; set; }

        /// <summary>
        /// Текущее время с учетом часового пояса и смещение на минимальный срок доставки
        /// </summary>
        public DateTimeOffset StartDateTime { get; set; }

        public float? TimeZoneOffset { get; set; }

        public List<int> Warehouses { get; set; }

        public virtual string Template { get; set; }
        
        public virtual string TemplateName => string.Empty;
        
        public virtual string ErrorMessage { get; set; }
        
        public virtual OptionValidationResult Validate() => new OptionValidationResult { IsValid = ErrorMessage.IsNullOrEmpty(), ErrorMessage = ErrorMessage};

        public virtual string ForMailTemplate() => NameRate ?? Name;

        public virtual string GetDescriptionForPayment() => string.Empty;

        /// <summary>
        /// Применение к доставке оплаты
        /// </summary>
        /// <returns>Флаг указывающий, что применение повлияло на доставку (изменило ее)</returns>
        public virtual bool ApplyPay(BasePaymentOption payOption) => false;

        public virtual bool IsAvailablePaymentCashOnDelivery { get; set; }
        public virtual bool IsAvailablePaymentPickPoint { get; set; }
        
        public BaseShippingPoint SelectedPoint { get; set; }

        public virtual DateTime? DateOfDelivery { get; set; }
        /// <summary>
        /// Время доставки в формате "hh:mm-hh:mm|zzz"
        /// </summary>
        public virtual string TimeOfDelivery { get; set; }
        
        public bool? Asap => TimeOfDelivery == null;

        /// <summary>
        /// Проверка доступности оплаты к доставке
        /// <para>Для реализации дополнительной логики конкретной доставки</para>
        /// </summary>
        /// <returns>Доступна указанная оплата</returns>
        public virtual bool AvailablePayment(BasePaymentOption payOption) => true;
    }
}
