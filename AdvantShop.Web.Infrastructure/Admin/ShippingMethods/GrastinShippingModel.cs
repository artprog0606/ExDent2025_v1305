using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Core.Services.Shipping.Grastin.Api;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Shipping.Grastin;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("Grastin")]
    public class GrastinShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {
        public string WidgetFromCity
        {
            get { return Params.ElementOrDefault(GrastinTemplate.WidgetFromCity); }
            set { Params.TryAddValue(GrastinTemplate.WidgetFromCity, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> ListCities
        {
            get
            {
                var cities = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "Москва", Value = "Москва"},
                    new SelectListItem() {Text = "Санкт-Петербург", Value = "Санкт-Петербург"},
                };

                var city = cities.Find(x => x.Value == WidgetFromCity);
                if (city != null)
                    city.Selected = true;

                return cities;
            }
        }

        public bool WidgetFromCityHide
        {
            get { return Params.ElementOrDefault(GrastinTemplate.WidgetFromCityHide).TryParseBool(); }
            set { Params.TryAddValue(GrastinTemplate.WidgetFromCityHide, value.ToString()); }
        }

        public bool WidgetFromCityNoChange
        {
            get { return Params.ElementOrDefault(GrastinTemplate.WidgetFromCityNoChange).TryParseBool(); }
            set { Params.TryAddValue(GrastinTemplate.WidgetFromCityNoChange, value.ToString()); }
        }

        public string[] WidgetHidePartnersShort
        {
            get { return (Params.ElementOrDefault(GrastinTemplate.WidgetHidePartnersShort) ?? string.Empty).Split(","); }
            set { Params.TryAddValue(GrastinTemplate.WidgetHidePartnersShort, value != null ? string.Join(",", value) : string.Empty); }
        }

        public List<SelectListItem> ListPartnersShort
        {
            get
            {
                var partners = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "Grastin самовывоз", Value = "grastinpikup"},
                    new SelectListItem() {Text = "Grastin курьер", Value = "grastincourier"},
                    new SelectListItem() {Text = "Boxberry самовывоз", Value = "boxberrypikup"},
                    new SelectListItem() {Text = "Boxberry курьер", Value = "boxberrycourier"},
                    new SelectListItem() {Text = "Партнерские ПВЗ", Value = "partnerpikup"},
                };

                partners.Where(x => WidgetHidePartnersShort.Contains(x.Value)).ForEach(x => x.Selected = true);

                return partners;
            }
        }

        public string[] WidgetHidePartnersFull
        {
            get { return (Params.ElementOrDefault(GrastinTemplate.WidgetHidePartnersFull) ?? string.Empty).Split(","); }
            set { Params.TryAddValue(GrastinTemplate.WidgetHidePartnersFull, value != null ? string.Join(",", value) : string.Empty); }
        }

        public List<SelectListItem> ListPartnersFull
        {
            get
            {
                var partners = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "Grastin самовывоз", Value = "grastinpikup"},
                    new SelectListItem() {Text = "Grastin курьер", Value = "grastincourier"},
                    new SelectListItem() {Text = "Boxberry самовывоз", Value = "boxberrypikup"},
                    new SelectListItem() {Text = "Boxberry курьер", Value = "boxberrycourier"},
                    new SelectListItem() {Text = "Hermes самовывоз", Value = "hermespikup"},
                    new SelectListItem() {Text = "DPD самовывоз", Value = "dpdpikup"},
                    new SelectListItem() {Text = "DPD самовывоз", Value = "dpdpikup"},
                    new SelectListItem() {Text = "Партнерские ПВЗ", Value = "partnerpikup"},
                    new SelectListItem() {Text = "Почта России", Value = "post"},
                    new SelectListItem() {Text = "Почта РФ посылка online", Value = "postpackageonline"},
                    new SelectListItem() {Text = "Почта РФ курьер online", Value = "postcourieronline"},
                };

                partners.Where(x => WidgetHidePartnersFull.Contains(x.Value)).ForEach(x => x.Selected = true);

                return partners;
            }
        }

        public bool WidgetHideCost
        {
            get { return Params.ElementOrDefault(GrastinTemplate.WidgetHideCost).TryParseBool(); }
            set { Params.TryAddValue(GrastinTemplate.WidgetHideCost, value.ToString()); }
        }

        public bool WidgetHideDuration
        {
            get { return Params.ElementOrDefault(GrastinTemplate.WidgetHideDuration).TryParseBool(); }
            set { Params.TryAddValue(GrastinTemplate.WidgetHideDuration, value.ToString()); }
        }

        public string WidgetExtrachargeTypen
        {
            get { return Params.ElementOrDefault(GrastinTemplate.WidgetExtrachargeTypen); }
            set { Params.TryAddValue(GrastinTemplate.WidgetExtrachargeTypen, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> ExtrachargeTypes
        {
            get
            {
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "%", Value = "%"},
                    new SelectListItem() {Text = "Руб", Value = "Руб"},
                };

                var type = types.Find(x => x.Value == WidgetExtrachargeTypen);
                if (type != null)
                    type.Selected = true;

                return types;
            }
        }

        public float WidgetExtracharge
        {
            get { return Params.ElementOrDefault(GrastinTemplate.WidgetExtracharge).TryParseFloat(); }
            set { Params.TryAddValue(GrastinTemplate.WidgetExtracharge, value.ToString()); }
        }

        public int WidgetAddDuration
        {
            get { return Params.ElementOrDefault(GrastinTemplate.WidgetAddDuration).TryParseInt(); }
            set { Params.TryAddValue(GrastinTemplate.WidgetAddDuration, value.ToString()); }
        }

        public string WidgetHidePartnersJson
        {
            get { return Params.ElementOrDefault(GrastinTemplate.WidgetHidePartnersJson); }
            set { Params.TryAddValue(GrastinTemplate.WidgetHidePartnersJson, value.DefaultOrEmpty()); }
        }

        public string YaMapsApiKey
        {
            get { return Params.ElementOrDefault(GrastinTemplate.YaMapsApiKey); }
            set { Params.TryAddValue(GrastinTemplate.YaMapsApiKey, value.DefaultOrEmpty()); }
        }

        public bool ShowDrivingDescriptionPoint
        {
            get { return Params.ElementOrDefault(GrastinTemplate.ShowDrivingDescriptionPoint).TryParseBool(); }
            set { Params.TryAddValue(GrastinTemplate.ShowDrivingDescriptionPoint, value.ToString()); }
        }

        public string ApiKey
        {
            get { return Params.ElementOrDefault(GrastinTemplate.ApiKey); }
            set { Params.TryAddValue(GrastinTemplate.ApiKey, value.DefaultOrEmpty()); }
        }

        public string OrderPrefix
        {
            get { return Params.ElementOrDefault(GrastinTemplate.OrderPrefix); }
            set { Params.TryAddValue(GrastinTemplate.OrderPrefix, value.DefaultOrEmpty()); }
        }

        public string TypePaymentDelivery
        {
            get { return Params.ElementOrDefault(GrastinTemplate.TypePaymentDelivery); }
            set { Params.TryAddValue(GrastinTemplate.TypePaymentDelivery, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> TypesPaymentDelivery
        {
            get
            {
                return (new[] { EnCourierService.DeliveryWithPayment, EnCourierService.DeliveryWithCashServices, EnCourierService.DeliverWithCreditCard, EnCourierService.GreatDeliveryWithPayment, EnCourierService.GreatDeliveryWithCashServices })
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Localize(),
                        Value = ((int)x).ToString()
                    })
                    .ToList();
            }
        }

        public string TypePaymentPickup
        {
            get { return Params.ElementOrDefault(GrastinTemplate.TypePaymentPickup); }
            set { Params.TryAddValue(GrastinTemplate.TypePaymentPickup, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> TypesPaymentPickup
        {
            get
            {
                return (new[] { EnCourierService.PickupWithPayment, EnCourierService.PickupWithCashServices, EnCourierService.PickupWithCreditCard })
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Localize(),
                        Value = ((int)x).ToString()
                    })
                    .ToList();
            }
        }

        public string TypeCalc
        {
            get { return Params.ElementOrDefault(GrastinTemplate.TypeCalc, ((int)EnTypeCalc.Api).ToString()); }
            set { Params.TryAddValue(GrastinTemplate.TypeCalc, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> ListTypesCalc
        {
            get
            {
                return Enum.GetValues(typeof (EnTypeCalc))
                    .Cast<EnTypeCalc>()
                    .Where(x => x == EnTypeCalc.Api || x == EnTypeCalc.ApiAndYaWidget)
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Localize(),
                        Value = ((int) x).ToString()
                    })
                    .ToList();
            }
        }

        public bool Insure
        {
            get { return Params.ElementOrDefault(GrastinTemplate.Insure).TryParseBool(); }
            set { Params.TryAddValue(GrastinTemplate.Insure, value.ToString()); }
        }

        public bool ExcludeCostOrderprocessing
        {
            get { return Params.ElementOrDefault(GrastinTemplate.ExcludeCostOrderprocessing).TryParseBool(); }
            set { Params.TryAddValue(GrastinTemplate.ExcludeCostOrderprocessing, value.ToString()); }
        }

        public string MoscowRegionId
        {
            get { return Params.ElementOrDefault(GrastinTemplate.MoscowRegionId); }
            set { Params.TryAddValue(GrastinTemplate.MoscowRegionId, value.DefaultOrEmpty()); }
        }

        public string SaintPetersburgRegionId
        {
            get { return Params.ElementOrDefault(GrastinTemplate.SaintPetersburgRegionId); }
            set { Params.TryAddValue(GrastinTemplate.SaintPetersburgRegionId, value.DefaultOrEmpty()); }
        }

        public string NizhnyNovgorodRegionId
        {
            get { return Params.ElementOrDefault(GrastinTemplate.NizhnyNovgorodRegionId); }
            set { Params.TryAddValue(GrastinTemplate.NizhnyNovgorodRegionId, value.DefaultOrEmpty()); }
        }

        public string OrelRegionId
        {
            get { return Params.ElementOrDefault(GrastinTemplate.OrelRegionId); }
            set { Params.TryAddValue(GrastinTemplate.OrelRegionId, value.DefaultOrEmpty()); }
        }

        public string KrasnodarRegionId
        {
            get { return Params.ElementOrDefault(GrastinTemplate.KrasnodarRegionId); }
            set { Params.TryAddValue(GrastinTemplate.KrasnodarRegionId, value.DefaultOrEmpty()); }
        }

        public string BoxberryRegionId
        {
            get { return Params.ElementOrDefault(GrastinTemplate.BoxberryRegionId); }
            set { Params.TryAddValue(GrastinTemplate.BoxberryRegionId, value.DefaultOrEmpty()); }
        }

        public string PartnerRegionId
        {
            get { return Params.ElementOrDefault(GrastinTemplate.PartnerRegionId); }
            set { Params.TryAddValue(GrastinTemplate.PartnerRegionId, value.DefaultOrEmpty()); }
        }

        public string RussianPostRegionId
        {
            get { return Params.ElementOrDefault(GrastinTemplate.RussianPostRegionId); }
            set { Params.TryAddValue(GrastinTemplate.RussianPostRegionId, value.DefaultOrEmpty()); }
        }

        public string CdekRegionId
        {
            get { return Params.ElementOrDefault(GrastinTemplate.CdekRegionId); }
            set { Params.TryAddValue(GrastinTemplate.CdekRegionId, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> ListRegionIds
        {
            get
            {
                var listRegionIds = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "нет", Value = ""}
                };

                var apiKey = Params.ElementOrDefault(GrastinTemplate.ApiKey);
                if (!string.IsNullOrEmpty(apiKey))
                {
                    var grastinApiService = new GrastinApiService(apiKey);
                    var contracts = grastinApiService.GetContracts();
                    if (contracts != null)
                    {
                        listRegionIds.AddRange(contracts.Select(x => new SelectListItem() {Text = x.Name, Value = x.IdRegion}));
                    }
                }

                return listRegionIds;
            }
        }

        public string[] ActiveDeliveryTypes
        {
            get { return (Params.ElementOrDefault(GrastinTemplate.ActiveDeliveryTypes) ?? string.Empty).Split(","); }
            set { Params.TryAddValue(GrastinTemplate.ActiveDeliveryTypes, value != null ? string.Join(",", value) : string.Empty); }
        }

        public List<SelectListItem> ListDeliveryTypes
        {
            get
            {
                var partners = Enum.GetValues(typeof(EnTypeDelivery))
                    .Cast<EnTypeDelivery>()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Localize(),
                        Value = ((int)x).ToString()
                    })
                    .ToList();

                partners.Where(x => ActiveDeliveryTypes.Contains(x.Value)).ForEach(x => x.Selected = true);

                return partners;
            }
        }

        private string _extracharge;
        public string Extracharge
        {
            get { return Params.ElementOrDefault(GrastinTemplate.Extracharge, "0"); }
            set
            {
                _extracharge = value;
                Params.TryAddValue(GrastinTemplate.Extracharge, value.TryParseFloat().ToString());
            }
        }

        public bool EnabledCod
        {
            get { return Params.ElementOrDefault(GrastinTemplate.EnabledCOD).TryParseBool(); }
            set { Params.TryAddValue(GrastinTemplate.EnabledCOD, value.ToString()); }
        }

        public bool EnabledPickPoint
        {
            get { return Params.ElementOrDefault(GrastinTemplate.EnabledPickPoint).TryParseBool(); }
            set { Params.TryAddValue(GrastinTemplate.EnabledPickPoint, value.ToString()); }
        }
        
        public override bool StatusesSync
        {
            get { return Params.ElementOrDefault(GrastinTemplate.StatusesSync).TryParseBool(); }
            set { Params.TryAddValue(GrastinTemplate.StatusesSync, value.ToString()); }
        }

        public override string StatusesReference
        {
            get { return Params.ElementOrDefault(GrastinTemplate.StatusesReference); }
            set { Params.TryAddValue(GrastinTemplate.StatusesReference, value.DefaultOrEmpty()); }
        }

        private Dictionary<string, string> _statuses;
        public override Dictionary<string, string> Statuses =>
        _statuses ?? (_statuses = Grastin.Statuses);

        public int ShipIdCod
        {
            get { return Params.ElementOrDefault(GrastinTemplate.ShipIdCOD, "0").TryParseInt(); }
            set
            {
                var paymentMethodId = value;

                if (EnabledCod)
                {
                    var payment = PaymentService.GetPaymentMethod(paymentMethodId);
                    if (payment == null)
                    {
                        var methodId = ShippingMethodId;

                        if (methodId == 0 && HttpContext.Current != null && HttpContext.Current.Request["shippingmethodid"] != null)
                            methodId = HttpContext.Current.Request["shippingmethodid"].TryParseInt();

                        var payMethod = new CashOnDelivery
                        {
                            Name = "Оплата курьеру (Грастин)",
                            Enabled = true,
                            ShippingMethodId = methodId
                        };
                        var paymentId = PaymentService.AddPaymentMethod(payMethod);

                        Params.TryAddValue(GrastinTemplate.ShipIdCOD, paymentId.ToString());
                    }
                }
                else
                {
                    if (paymentMethodId != 0)
                        PaymentService.DeletePaymentMethod(paymentMethodId);

                    Params.TryAddValue(GrastinTemplate.ShipIdCOD, "");
                }
            }
        }

        public int ShipIdPickPoint
        {
            get { return Params.ElementOrDefault(GrastinTemplate.ShipIdPickPoint).TryParseInt(); }
            set
            {
                var paymentMethodId = value;

                if (EnabledPickPoint)
                {
                    var payment = PaymentService.GetPaymentMethod(paymentMethodId);
                    if (payment == null)
                    {
                        var methodId = ShippingMethodId;

                        if (methodId == 0 && HttpContext.Current != null && HttpContext.Current.Request["shippingmethodid"] != null)
                            methodId = HttpContext.Current.Request["shippingmethodid"].TryParseInt();

                        var payMethod = new PickPoint
                        {
                            Name = "Оплата при получении заказа в пункте выдачи (Грастин)",
                            Enabled = true,
                            ShippingMethodId = methodId
                        };
                        var paymentId = PaymentService.AddPaymentMethod(payMethod);

                        Params.TryAddValue(GrastinTemplate.ShipIdPickPoint, paymentId.ToString());
                    }
                }
                else
                {
                    if (paymentMethodId != 0)
                        PaymentService.DeletePaymentMethod(paymentMethodId);

                    Params.TryAddValue(GrastinTemplate.ShipIdPickPoint, "");
                }
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                yield return new ValidationResult("Укажите API ключ", new[] { "ApiKey" });
            }
            if (string.IsNullOrWhiteSpace(OrderPrefix))
            {
                yield return new ValidationResult("Укажите префикс к заказу", new[] { "OrderPrefix" });
            }

            if (((EnTypeCalc) TypeCalc.TryParseInt()) == EnTypeCalc.Api)
            {
                var activeDeliveryTypes = ActiveDeliveryTypes;
                if (activeDeliveryTypes == null || activeDeliveryTypes.Length == 0)
                {
                    yield return new ValidationResult("Укажите методы доставки", new[] { "ActiveDeliveryTypes" });
                }

                //if (string.IsNullOrWhiteSpace(_extracharge) || !_extracharge.IsDecimal())
                //{
                //    yield return new ValidationResult("Укажите наценку в базовой валюте", new[] { "Extracharge" });
                //}
            }
        }
    }
}
