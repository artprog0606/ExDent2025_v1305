using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("Alfabank")]
    public class AlfabankPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string UserName
        {
            get { return Parameters.ElementOrDefault(AlfabankTemplate.UserName); }
            set { Parameters.TryAddValue(AlfabankTemplate.UserName, value.DefaultOrEmpty()); }
        }

        public string Password
        {
            get { return Parameters.ElementOrDefault(AlfabankTemplate.Password); }
            set { Parameters.TryAddValue(AlfabankTemplate.Password, value.DefaultOrEmpty()); }
        }

        public string MerchantLogin
        {
            get { return Parameters.ElementOrDefault(AlfabankTemplate.MerchantLogin); }
            set { Parameters.TryAddValue(AlfabankTemplate.MerchantLogin, value.DefaultOrEmpty()); }
        }

        public bool SendReceiptData
        {
            get { return Parameters.ElementOrDefault(AlfabankTemplate.SendReceiptData).TryParseBool(); }
            set { Parameters.TryAddValue(AlfabankTemplate.SendReceiptData, value.ToString()); }
        }

        public string Taxation
        {
            get { return Parameters.ElementOrDefault(AlfabankTemplate.Taxation); }
            set { Parameters.TryAddValue(AlfabankTemplate.Taxation, value.DefaultOrEmpty()); }
        }

        public string GatewayUrl
        {
            get
            {
                var gatewayUrl = Parameters.ElementOrDefault(AlfabankTemplate.GatewayUrl);
                if (gatewayUrl.IsNullOrEmpty())
                {
                    //поддержка старых настроек
                    if (Parameters.ElementOrDefault(AlfabankTemplate.UseTestMode).TryParseBool(true) ?? true)
                        gatewayUrl = Alfabank.GatewayUrlTestMode;
                    else if (Parameters.ElementOrDefault(AlfabankTemplate.GatewayCountry) == "kz")
                        gatewayUrl = Alfabank.GatewayUrlKz;
                    else
                        gatewayUrl = Alfabank.GatewayUrlRu;
                }

                return gatewayUrl;
            }
            set { Parameters.TryAddValue(AlfabankTemplate.GatewayUrl, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> Taxations
        {
            get
            {
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "Общая", Value = "0"},
                    new SelectListItem() {Text = "Упрощенная (доходы)", Value = "1"},
                    new SelectListItem() {Text = "Упрощенная (доходы минус расходы)", Value = "2"},
                    new SelectListItem() {Text = "Единый налог на вмененный доход", Value = "3"},
                    new SelectListItem() {Text = "Единый сельскохозяйственный налог", Value = "4"},
                    new SelectListItem() {Text = "Патентная система налогообложения", Value = "5"},
                };

                var type = types.Find(x => x.Value == Taxation);
                if (type != null)
                    type.Selected = true;

                return types;
            }
        }

        // public List<SelectListItem> GatewayCountries
        // {
        //     get
        //     {
        //         var types = new List<SelectListItem>()
        //         {
        //             new SelectListItem() {Text = "Россия", Value = ""},
        //             new SelectListItem() {Text = "Казахстан", Value = "kz"},
        //         };
        //
        //         var type = types.Find(x => x.Value == Taxation);
        //         if (type != null)
        //             type.Selected = true;
        //
        //         return types;
        //     }
        // }

        public List<SelectListItem> Gateways
        {
            get
            {
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = $"{Alfabank.GatewayUrlTestMode} (Тестовый режим для логинов без префиксов и с префиксом r-)", Value = Alfabank.GatewayUrlTestMode},
                    new SelectListItem() {Text = $"{Alfabank.GatewayUrlTest2Mode} (Тестовый режим для логинов с префиксом i-)", Value = Alfabank.GatewayUrlTest2Mode},
                    new SelectListItem() {Text = $"{Alfabank.GatewayUrlRu} (Продуктивный режим для логинов без префиксов)", Value = Alfabank.GatewayUrlRu},
                    new SelectListItem() {Text = $"{Alfabank.GatewayUrlRuNew} (Продуктивный режим для логинов с префиксом r-)", Value = Alfabank.GatewayUrlRuNew},
                    new SelectListItem() {Text = $"{Alfabank.GatewayUrlRu2} (Продуктивный режим для логинов с префиксом i-)", Value = Alfabank.GatewayUrlRu2},
                };

                var gatewayUrl = GatewayUrl;
                var type = types.Find(x => x.Value == gatewayUrl);
                if (type != null)
                    type.Selected = true;
                else if (gatewayUrl.IsNotEmpty())
                {
                    type = new SelectListItem() {Text = $"{gatewayUrl} (Текущий, устарел)", Value = gatewayUrl, Disabled = true, Selected = true};
                    types.Insert(0, type);
                }

                return types;
            }
        }
        
        public byte TypeFfd
        {
            get { return (byte)Parameters.ElementOrDefault(AlfabankTemplate.TypeFfd).TryParseInt((int)Alfabank.EnTypeFfd.Less1_2); }
            set { Parameters.TryAddValue(AlfabankTemplate.TypeFfd, value.ToString()); }
        }
 
        public List<SelectListItem> TypesFfd
        {
            get
            {
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "ФФД 1.2 и старше", Value = ((byte)Alfabank.EnTypeFfd.From1_2).ToString()},
                    new SelectListItem() {Text = "До ФФД 1.2", Value = ((byte)Alfabank.EnTypeFfd.Less1_2).ToString()},
                };

                var type = types.Find(x => x.Value == TypeFfd.ToString());
                if (type != null)
                    type.Selected = true;

                return types;
            }
        }

        public bool ExistsUnitsWithOutMeasure => AdvantShop.Core.Services.Catalog.UnitService.GetList().Any(unit => unit.MeasureType is null);
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(GatewayUrl))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
         
        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("https://www.advantshop.net/help/pages/alfa-bank", "Инструкция. Подключение платежного модуля Альфа-Банк"); }
        }
    }
}
