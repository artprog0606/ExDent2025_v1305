using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("YandexKassa")]
    public class YandexKassaPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string ProtocolApi { get { return YandexKassa.ProtocolApi; } }
        public string Protocol
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.Protocol); }
            set { Parameters.TryAddValue(YandexKassaTemplate.Protocol, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> Protocols
        {
            get
            {
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "Платежный модуль", Value = ""},
                    new SelectListItem() {Text = "Виджет", Value = YandexKassa.ProtocolWidget},
                    new SelectListItem() {Text = "API", Value = YandexKassa.ProtocolApi},
                };

                var type = types.Find(x => x.Value == Protocol);
                if (type != null)
                    type.Selected = true;

                return types;
            }
        }

        public string ShopId
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.ShopID); }
            set { Parameters.TryAddValue(YandexKassaTemplate.ShopID, value.DefaultOrEmpty()); }
        }

        public string ScId
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.ScID); }
            set { Parameters.TryAddValue(YandexKassaTemplate.ScID, value.DefaultOrEmpty()); }
        }

        public string YaPaymentType
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.YaPaymentType); }
            set { Parameters.TryAddValue(YandexKassaTemplate.YaPaymentType, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> YaPaymentTypes
        {
            get
            {
                // https://yookassa.ru/docs/payment-solution/payments/supplementary/reference/payment-types
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "Умный платеж (все доступные методы)", Value = ""},
                    new SelectListItem() {Text = "Со счета в ЮMoney", Value = "PC"},
                    new SelectListItem() {Text = "С банковской карты", Value = "AC"},
                    new SelectListItem() {Text = "Со счета мобильного телефона", Value = "MC"},
                    new SelectListItem() {Text = "По коду через терминал", Value = "GP"},
                    new SelectListItem() {Text = "Оплата через Сбербанк: оплата по SMS или Сбербанк Онлайн", Value = "SB"},
                    new SelectListItem() {Text = "Оплата через мобильный терминал (mPOS)", Value = "MP"},
                    new SelectListItem() {Text = "Оплата через Альфа-Клик", Value = "AB"},
                    new SelectListItem() {Text = "Оплата через MasterPass", Value = "МА"},
                    // new SelectListItem() {Text = "Оплата через Промсвязьбанк", Value = "PB"},
                    new SelectListItem() {Text = "Оплата через QIWI Wallet", Value = "QW"},
                    //new SelectListItem() {Text = "Оплата через КупиВкредит (Тинькофф Банк)", Value = "KV"},
                    //new SelectListItem() {Text = "Оплата через Доверительный платеж на Куппи.ру", Value = "QP"},
                    new SelectListItem() {Text = "Сбербанк Бизнес Онлайн (B2B-платежи)", Value = "2S"},
                    new SelectListItem() {Text = "ЕРИП (Беларусь)", Value = "EP"},
                    new SelectListItem() {Text = "Заплатить по частям (кредит)", Value = "CR"},
                };

                var type = types.Find(x => x.Value == YaPaymentType);
                if (type != null)
                    type.Selected = true;

                return types;
            }
        }

        public List<SelectListItem> YaPaymentTypesNew
        {
            get
            {
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "Умный платеж (все доступные методы)", Value = ""},
                    new SelectListItem() {Text = "Банковские карты", Value = "bank_card"},
                    new SelectListItem() {Text = "ЮMoney", Value = "yoo_money"},
                    new SelectListItem() {Text = "Сбербанк Онлайн (SberPay)", Value = "sberbank"},
                    new SelectListItem() {Text = "QIWI Wallet", Value = "qiwi"},
                    // new SelectListItem() {Text = "Webmoney", Value = "webmoney"},
                    new SelectListItem() {Text = "Наличные через терминалы", Value = "cash"},
                    //new SelectListItem() {Text = "Баланс мобильного", Value = "mobile_balance"},
                    new SelectListItem() {Text = "Альфа-Клик", Value = "alfabank"},
                    new SelectListItem() {Text = "Тинькофф", Value = "tinkoff_bank"},
                    new SelectListItem() {Text = "СберБанк Бизнес Онлайн", Value = "b2b_sberbank"},
                    new SelectListItem() {Text = "Система быстрых платежей (СБП)", Value = "sbp"},
                    new SelectListItem() {Text = "Заплатить по частям (кредит)", Value = "installments"},
                };

                var type = types.Find(x => x.Value == YaPaymentType);
                if (type != null)
                    type.Selected = true;

                return types;
            }
        }

        public string Password
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.Password); }
            set { Parameters.TryAddValue(YandexKassaTemplate.Password, value.DefaultOrEmpty()); }
        }

        public string SecretKey
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.Password); }
            set { Parameters.TryAddValue(YandexKassaTemplate.Password, value.DefaultOrEmpty()); }
        }

        //public bool DemoMode
        //{
        //    get { return Parameters.ElementOrDefault(YandexKassaTemplate.DemoMode).TryParseBool(); }
        //    set { Parameters.TryAddValue(YandexKassaTemplate.DemoMode, value.ToString()); }
        //}

        public bool SendReceiptData
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.SendReceiptData).TryParseBool(); }
            set { Parameters.TryAddValue(YandexKassaTemplate.SendReceiptData, value.ToString()); }
        }

        public float MinimumPrice
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.MinimumPrice).TryParseFloat(); }
            set { Parameters.TryAddValue(YandexKassaTemplate.MinimumPrice, value.ToInvariantString()); }
        }

        public string MaximumPrice
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.MaximumPrice); }
            set { Parameters.TryAddValue(YandexKassaTemplate.MaximumPrice, value.TryParseFloat(true)?.ToInvariantString() ?? string.Empty); }
        }

        public float FirstPayment
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.FirstPayment).TryParseFloat(); }
            set { Parameters.TryAddValue(YandexKassaTemplate.FirstPayment, value.ToInvariantString()); }
        }

        public byte? TaxSystemCode
        {
            get { return (byte?)Parameters.ElementOrDefault(YandexKassaTemplate.TaxSystemCode).TryParseInt(true); }
            set { Parameters.TryAddValue(YandexKassaTemplate.TaxSystemCode, value.ToString()); }
        }

        public List<SelectListItem> TaxSystemCodes
        {
            get
            {
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "Не передавать", Value = string.Empty},
                    new SelectListItem() {Text = "Общая система налогообложения", Value = "1"},
                    new SelectListItem() {Text = "Упрощенная (УСН, доходы)", Value = "2"},
                    new SelectListItem() {Text = "Упрощенная (УСН, доходы минус расходы)", Value = "3"},
                    new SelectListItem() {Text = "Единый налог на вмененный доход (ЕНВД)", Value = "4"},
                    new SelectListItem() {Text = "Единый сельскохозяйственный налог (ЕСН)", Value = "5"},
                    new SelectListItem() {Text = "Патентная система налогообложения", Value = "6"},
                };

                var type = types.Find(x => x.Value == (TaxSystemCode?.ToString() ?? string.Empty));
                if (type != null)
                    type.Selected = true;

                return types;
            }
        }
        
        public byte TypeFfd
        {
            get { return (byte)Parameters.ElementOrDefault(YandexKassaTemplate.TypeFfd).TryParseInt((int)YandexKassa.EnTypeFfd.Less1_2); }
            set { Parameters.TryAddValue(YandexKassaTemplate.TypeFfd, value.ToString()); }
        }
 
        public List<SelectListItem> TypesFfd
        {
            get
            {
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "ФФД 1.2 и старше", Value = ((byte)YandexKassa.EnTypeFfd.From1_2).ToString()},
                    new SelectListItem() {Text = "До ФФД 1.2", Value = ((byte)YandexKassa.EnTypeFfd.Less1_2).ToString()},
                };

                var type = types.Find(x => x.Value == TypeFfd.ToString());
                if (type != null)
                    type.Selected = true;

                return types;
            }
        }

        public bool ExistsUnitsWithOutMeasure => UnitService.GetList().Any(unit => unit.MeasureType is null);

        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("http://www.advantshop.net/help/pages/connect-yandex-kassa", "Инструкция. Подключение платежного модуля \"Касса от ЮMoney\""); }
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Protocol == YandexKassa.ProtocolApi || Protocol == YandexKassa.ProtocolWidget)
            {
                if (string.IsNullOrWhiteSpace(ShopId) ||
                    string.IsNullOrWhiteSpace(SecretKey))
                {
                    yield return new ValidationResult("Заполните обязательные поля");
                }
            }
            else if (string.IsNullOrWhiteSpace(ShopId) ||
                string.IsNullOrWhiteSpace(ScId) ||
                string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
