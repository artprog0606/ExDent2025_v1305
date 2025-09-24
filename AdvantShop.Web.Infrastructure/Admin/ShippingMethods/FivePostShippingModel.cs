using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Core.Services.Shipping.FivePost.Helpers;
using AdvantShop.Payment;
using AdvantShop.Shipping.FivePost;
using AdvantShop.Shipping.FivePost.Api;
using AdvantShop.Shipping.FivePost.PickPoints;
using AdvantShop.Shipping.Shiptor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("FivePost")]
    public class FivePostShippingModel : ShippingMethodAdminModel
    {
        public string ApiKey
        {
            get { return Params.ElementOrDefault(FivePostTemplate.ApiKey); }
            set
            {
                Params.TryAddValue(FivePostTemplate.ApiKey, value.DefaultOrEmpty());
                if (value.IsNotEmpty())
                {
                    var apiService = new FivePostApiService(value);
                    if (!FivePostPickPointService.ExistsPickPoints())
                        FivePost.SyncPickPoints(apiService);
                }
            }
        }

        public string BarcodeEnrichment
        {
            get { return Params.ElementOrDefault(FivePostTemplate.BarcodeEnrichment, ((int)Shipping.FivePost.EFivePostBarcodeEnrichment.None).ToString()); }
            set { Params.TryAddValue(FivePostTemplate.BarcodeEnrichment, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> BarcodeEnrichmentList
        {
            get
            {
                return Enum.GetValues(typeof(Shipping.FivePost.EFivePostBarcodeEnrichment))
                    .Cast<EFivePostBarcodeEnrichment>()
                    .Select(x => new SelectListItem
                    {
                        Text = x.Localize(),
                        Value = ((int)x).ToString()
                    }).ToList();
            }
        }

        public string UndeliverableOption
        {
            get { return Params.ElementOrDefault(FivePostTemplate.UndeliverableOption, ((int)Shipping.FivePost.Api.EFivePostUndeliverableOption.ReturnToWarehouse).ToString()); }
            set { Params.TryAddValue(FivePostTemplate.UndeliverableOption, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> UndeliverableOptionList
        {
            get
            {
                return Enum.GetValues(typeof(Shipping.FivePost.Api.EFivePostUndeliverableOption))
                    .Cast<EFivePostUndeliverableOption>()
                    .Select(x => new SelectListItem
                    {
                        Text = x.Localize(),
                        Value = ((int)x).ToString()
                    }).ToList();
            }
        }

        public bool WithInsure
        {
            get { return Params.ElementOrDefault(FivePostTemplate.WithInsure).TryParseBool(); }
            set { Params.TryAddValue(FivePostTemplate.WithInsure, value.ToString()); }
        }

        public string TypeViewPoints
        {
            get { return Params.ElementOrDefault(FivePostTemplate.TypeViewPoints, ((int)Shipping.FivePost.ETypeViewPoints.YandexMap).ToString()); }
            set { Params.TryAddValue(FivePostTemplate.TypeViewPoints, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> TypeViewPointsList
        {
            get
            {
                return Enum.GetValues(typeof(ETypeViewPoints))
                    .Cast<ETypeViewPoints>()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Localize(),
                        Value = ((int)x).ToString()
                    })
                    .ToList();
            }
        }

        public string YaMapsApiKey
        {
            get { return Params.ElementOrDefault(FivePostTemplate.YandexMapApiKey); }
            set { Params.TryAddValue(FivePostTemplate.YandexMapApiKey, value.DefaultOrEmpty()); }
        }

        public string WidgetKey
        {
            get { return Params.ElementOrDefault(FivePostTemplate.WidgetKey); }
            set { Params.TryAddValue(FivePostTemplate.WidgetKey, value.DefaultOrEmpty()); }
        }

        public bool? IsValidToken => new FivePostApiService(ApiKey).IsValidToken;

        public override bool StatusesSync
        {
            get { return Params.ElementOrDefault(FivePostTemplate.StatusesSync).TryParseBool(); }
            set { Params.TryAddValue(FivePostTemplate.StatusesSync, value.ToString()); }
        }

        public override string StatusesReference
        {
            get { return Params.ElementOrDefault(FivePostTemplate.StatusesReference); }
            set { Params.TryAddValue(FivePostTemplate.StatusesReference, value.DefaultOrEmpty()); }
        }

        private Dictionary<string, string> _statuses;
        public override Dictionary<string, string> Statuses =>
        _statuses ?? (_statuses = Enum.GetValues(typeof(EFivePostStatus))
            .Cast<EFivePostStatus>()
            .ToDictionary(x => ((int)x).ToString(), x => x.Localize()));

        private List<SelectListItem> _warehouseList;
        public List<SelectListItem> WarehouseList =>
            _warehouseList ?? (_warehouseList =
                CacheManager.Get($"FivePostWarehouses_{ApiKey}",
                    () => new FivePostApiService(ApiKey).GetWarehouses(null)
                    .OrderBy(x => x.Name)
                    .Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.InnerId
                    })
                    .ToList()));
        public string WarehouseListJson => JsonConvert.SerializeObject(WarehouseList);

        private string[] _activeTarifs;
        public string[] ActiveTarifs
        {
            get
            {
                return _activeTarifs ?? (_activeTarifs
                    = Params.ElementOrDefault(FivePostTemplate.ActiveTarifs).IsNotEmpty()
                    ? Params.ElementOrDefault(FivePostTemplate.ActiveTarifs).Split(";").ToArray()
                    : TarifList?.Select(x => x.Value).ToArray());
            }
            set { Params.TryAddValue(FivePostTemplate.ActiveTarifs, value != null ? string.Join(";", value) : string.Empty); }
        }

        private List<SelectListItem> _tarifList;
        public List<SelectListItem> TarifList
            => _tarifList ?? (_tarifList = new FivePostApiService(ApiKey)
                .GetRateList()?
                .Select(x => new SelectListItem
                {
                    Text = x.Type,
                    Value = x.TypeCode.ToString()
                })
            .ToList());

        public string WarehouseDeliveryTypeReference
        {
            get { return Params.ElementOrDefault(FivePostTemplate.WarehouseDeliveryTypeReferences); }
            set { Params.TryAddValue(FivePostTemplate.WarehouseDeliveryTypeReferences, value.DefaultOrEmpty()); }
        }

        private List<SelectListItem> _deliveryTypes;

        public string DeliveryTypesJson
            => JsonConvert.SerializeObject
            (_deliveryTypes ?? (_deliveryTypes = new FivePostApiService(ApiKey)
                .GetDeliveryTypeList()?
                .Select(x => new SelectListItem
                {
                    Text = x.Type.StrName(),
                    Value = x.Code.ToString()
                }).ToList()
            ));

        public string PaymentCodCardId
        {
            get { return Params.ElementOrDefault(FivePostTemplate.PaymentCodCardId); }
            set { Params.TryAddValue(FivePostTemplate.PaymentCodCardId, value.TryParseInt(true).ToString() ?? string.Empty); }
        }

        public List<SelectListItem> PaymentsCod
        {
            get
            {
                var paymentsCod = PaymentService.GetAllPaymentMethods(false)
                    .Where(x => x.PaymentKey.Equals("CashOnDelivery", System.StringComparison.OrdinalIgnoreCase))
                    .Select(x => new SelectListItem() { Text = x.Name, Value = x.PaymentMethodId.ToString() })
                    .ToList();

                paymentsCod.Insert(0, new SelectListItem() { Text = "-", Value = string.Empty });

                return paymentsCod;
            }
        }

        public string RateDeliverySLReference
        {
            get { return Params.ElementOrDefault(FivePostTemplate.RateDeliverySLReference); }
            set { Params.TryAddValue(FivePostTemplate.RateDeliverySLReference, value ?? string.Empty); }
        }
    }
}
