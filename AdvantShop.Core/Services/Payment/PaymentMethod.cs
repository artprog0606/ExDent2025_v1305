//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using System;
using System.Collections.Generic;
using System.Web;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Helpers;

namespace AdvantShop.Payment
{
    public interface IPayment
    {
        ProcessType ProcessType { get; }
        NotificationType NotificationType { get; }
        UrlStatus ShowUrls { get; }
        string SuccessUrl { get; }
        string CancelUrl { get; }
        string FailUrl { get; }
        string NotificationUrl { get; }
        int PaymentMethodId { get; }
        string ProcessJavascript(Order order);
        string ProcessJavascriptButton(Order order);
        string ProcessServerRequest(Order order);
        string ProcessResponse(HttpContext context);

        BasePaymentOption GetOption(BaseShippingOption shippingOption, float preCoast, CustomerType? customerType);
    }


    [Serializable]
    public abstract class PaymentMethod : IPayment
    {
        //public common settings
        public int PaymentMethodId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public int SortOrder { get; set; }
        public string ModuleStringId { get; set; }

        public float ExtrachargeInNumbers { get; set; }
        public float ExtrachargeInPercents { get; set; }

        public int? TaxId { get; set; }

        public float GetExtracharge(Order order)
        {
            var extracharge = ExtrachargeInNumbers;

            if (ExtrachargeInPercents != 0)
            {
                extracharge += ExtrachargeInPercents *
                    (order.OrderItems.Sum(x => x.Price * x.Amount) - order.TotalDiscount - order.BonusCost +
                     order.ShippingCost + (order.Taxes.Where(x => !x.ShowInPrice).Sum(x => x.Sum) ?? 0f)) / 100;
            }

            return extracharge;
        }

        public float GetExtracharge(float preCoast)
        {
            return ExtrachargeInNumbers.RoundPrice() +
                   (ExtrachargeInPercents != 0
                       ? (ExtrachargeInPercents * preCoast / 100).RoundPrice(CurrencyService.CurrentCurrency.Rate)
                       : 0);
        }

        private int _currencyId;

        public int CurrencyId
        {
            get
            {
                if (_currencyId != 0)
                    return _currencyId;

                var currency =
                    CurrencyService.GetAllCurrencies(true)
                        .FirstOrDefault(x => x.Iso3 == SettingsCatalog.DefaultCurrencyIso3);

                return _currencyId = currency?.CurrencyId ?? 0;
            }
            set => _currencyId = value;
        }

        public int CustomerType { get; set; }

        private Currency _paymentCurrency;

        public Currency PaymentCurrency
        {
            get
            {
                if (_paymentCurrency != null)
                    return _paymentCurrency;

                return
                    _paymentCurrency =
                        CurrencyService.GetAllCurrencies(true).FirstOrDefault(x => x.CurrencyId == CurrencyId) ??
                        CurrencyService.GetAllCurrencies(true).FirstOrDefault(x => x.Iso3 == SettingsCatalog.DefaultCurrencyIso3);
            }
        }

        public virtual bool CurrencyAllAvailable => true;

        public virtual string[] CurrencyIso3Available => null;

        private Photo _picture;

        public Photo IconFileName
        {
            get => _picture ?? (_picture = PhotoService.GetPhotoByObjId(PaymentMethodId, PhotoType.Payment));
            set => _picture = value;
        }

        private Dictionary<string, string> _parameters = new Dictionary<string, string>();

        public virtual Dictionary<string, string> Parameters
        {
            get => _parameters;
            set => _parameters = value;
        }

        public string PaymentKey => AttributeHelper.GetAttributeValue<PaymentKeyAttribute, string>(this);

        //public processing methods
        //public virtual IEnumerable<string> ShippingKeys
        //{
        //    get { return null; }
        //}

        public virtual ProcessType ProcessType => ProcessType.None;

        public virtual NotificationType NotificationType => NotificationType.None;

        public virtual UrlStatus ShowUrls => UrlStatus.None;

        private string _baseSiteUrl;

        private string BaseSiteUrl()
        {
            if (_baseSiteUrl != null)
                return _baseSiteUrl;

            var url = StringHelper.ToPuny(SettingsMain.SiteUrl.ToLower());

            return _baseSiteUrl = (url.StartsWith("http")
                                      ? url
                                      : "http://" + url)
                                  + (!url.EndsWith("/")
                                      ? "/"
                                      : string.Empty);
        }

        private Uri BaseSiteUri() => new Uri(BaseSiteUrl());

        public virtual string SuccessUrl => new Uri(BaseSiteUri(), $"paymentreturnurl/{PaymentMethodId}").ToString();

        public virtual string CancelUrl => new Uri(BaseSiteUri(), "cancel").ToString();

        public virtual string FailUrl => new Uri(BaseSiteUri(), "fail").ToString();

        public virtual string NotificationUrl => new Uri(BaseSiteUri(), $"paymentnotification/{PaymentMethodId}").ToString();

        public virtual string ButtonText =>
            this is ICreditPaymentMethod creditPaymentMethod && creditPaymentMethod.ActiveCreditPayment
                ? LocalizationService.GetResource("Core.Payment.PaymentMethod.ButtonTextCredit")
                : LocalizationService.GetResource("Core.Payment.PaymentMethod.DefaultButtonText");

        public virtual PaymentForm GetPaymentForm(Order order)
        {
            return null;
        }

        public virtual string ProcessJavascript(Order order)
        {
            return string.Empty;
        }

        public virtual string ProcessJavascriptButton(Order order)
        {
            return string.Empty;
        }

        public virtual string ProcessServerRequest(Order order)
        {
            throw new NotImplementedException();
        }

        public virtual string ProcessResponse(HttpContext context)
        {
            throw new NotImplementedException();
        }

        public virtual BasePaymentOption GetOption(BaseShippingOption shippingOption, float preCoast, CustomerType? customerType)
        {
            var option = new BasePaymentOption(this, preCoast);
            return option;
        }

        public static string GetOrderDescription(string orderNumber)
        {
            return string.Format(LocalizationService.GetResource("Core.Payment.OrderDescription"), orderNumber);
        }

        public virtual PaymentDetails PaymentDetails()
        {
            return null;
        }

        public static PaymentMethod Create(string paymentKey)
        {
            var type = ReflectionExt.GetTypeByAttributeValue<PaymentKeyAttribute>(typeof(IPayment), atr => atr.Value, paymentKey);
            return type != null
                ? (PaymentMethod)Activator.CreateInstance(type)
                : null;
        }
    }
}
