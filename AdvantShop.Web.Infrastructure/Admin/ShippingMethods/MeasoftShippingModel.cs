using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Payment;
using AdvantShop.Shipping.Measoft;
using AdvantShop.Shipping.Measoft.Api;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("Measoft")]
    public class MeasoftShippingModel : ShippingMethodAdminModel
    {
        public string Login
        {
            get { return Params.ElementOrDefault(MeasoftTemplate.Login); }
            set { Params.TryAddValue(MeasoftTemplate.Login, value.DefaultOrEmpty()); }
        }
        public string Password
        {
            get { return Params.ElementOrDefault(MeasoftTemplate.Password); }
            set { Params.TryAddValue(MeasoftTemplate.Password, value.DefaultOrEmpty()); }
        }
        public string Extra
        {
            get { return Params.ElementOrDefault(MeasoftTemplate.Extra); }
            set { Params.TryAddValue(MeasoftTemplate.Extra, value.DefaultOrEmpty()); }
        }

        public bool WithInsure
        {
            get { return Params.ElementOrDefault(MeasoftTemplate.WithInsure).TryParseBool(); }
            set { Params.TryAddValue(MeasoftTemplate.WithInsure, value.ToString()); }
        }
        public override bool StatusesSync
        {
            get { return Params.ElementOrDefault(MeasoftTemplate.StatusesSync).TryParseBool(); }
            set { Params.TryAddValue(MeasoftTemplate.StatusesSync, value.ToString()); }
        }

        public override string StatusesReference
        {
            get { return Params.ElementOrDefault(MeasoftTemplate.StatusesReference); }
            set { Params.TryAddValue(MeasoftTemplate.StatusesReference, value.DefaultOrEmpty()); }
        }

        private Dictionary<string, string> _statuses;
        public override Dictionary<string,string> Statuses =>
        _statuses ?? (_statuses = Enum.GetValues(typeof(EMeasoftStatus))
            .Cast<EMeasoftStatus>()
            .ToDictionary(x => ((int)x).ToString(), x => x.Localize()));

        public bool CalculateCourier
        {
            get { return Params.ElementOrDefault(MeasoftTemplate.CalculateCourier).TryParseBool(); }
            set { Params.TryAddValue(MeasoftTemplate.CalculateCourier, value.ToString()); }
        }

        public string YaMapsApiKey
        {
            get { return Params.ElementOrDefault(MeasoftTemplate.YaMapsApiKey); }
            set { Params.TryAddValue(MeasoftTemplate.YaMapsApiKey, value.DefaultOrEmpty()); }
        }

        private string[] _deliveryTypes;
        public string[] DeliveryTypes
        {
            get { return _deliveryTypes ?? (_deliveryTypes = (Params.ElementOrDefault(MeasoftTemplate.DeliveryTypes) ?? string.Empty).Split(",")); }
            set { Params.TryAddValue(MeasoftTemplate.DeliveryTypes, value != null ? string.Join(",", value) : string.Empty); }
        }

        public List<SelectListItem> ListDeliveryTypes
        {
            get
            {
                var listDeliveryTypes = new List<SelectListItem>();

                foreach (var delivertyType in Enum.GetValues(typeof(TypeDelivery)).Cast<TypeDelivery>())
                {
                    listDeliveryTypes.Add(new SelectListItem()
                    {
                        Text = delivertyType.Localize(),
                        Value = ((int)delivertyType).ToString()
                    });
                }

                return listDeliveryTypes;
            }
        }

        public string TypeViewPoints
        {
            get { return Params.ElementOrDefault(MeasoftTemplate.TypeViewPoints, ((int)Shipping.Measoft.Api.TypeViewPoints.List).ToString()); }
            set { Params.TryAddValue(MeasoftTemplate.TypeViewPoints, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> ListTypesViewPoints
        {
            get
            {
                return Enum.GetValues(typeof(TypeViewPoints))
                    .Cast<TypeViewPoints>()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Localize(),
                        Value = ((int)x).ToString()
                    })
                    .ToList();
            }
        }

        private string[] _activeDeliveryServices;
        public string[] ActiveDeliveryServices
        {
            get { return _activeDeliveryServices ?? (_activeDeliveryServices = (Params.ElementOrDefault(MeasoftTemplate.ActiveDeliveryServices) ?? string.Empty).Split(";")); }
            set { Params.TryAddValue(MeasoftTemplate.ActiveDeliveryServices, value != null ? string.Join(";", value) : string.Empty); }
        }

        public List<SelectListItem> ListDeliveryServices
        {
            get
            {
                return MeasoftApiService.GetDeliveryServices(Extra)
                    .OrderBy(x => x.Code)
                    .Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Code.ToString()
                    }).ToList();
            }
        }

        public string PaymentCodCardId
        {
            get { return Params.ElementOrDefault(MeasoftTemplate.PaymentCodCardId); }
            set { Params.TryAddValue(MeasoftTemplate.PaymentCodCardId, value.TryParseInt(true).ToString() ?? string.Empty); }
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

        private string[] _activeStoreList;
        public string[] ActiveStoreList
        {
            get { return _activeStoreList ?? (_activeStoreList = (Params.ElementOrDefault(MeasoftTemplate.ActiveStoreList) ?? string.Empty).Split(";")); }
            set 
            { 
                _activeStoreList = value; 
                Params.TryAddValue(MeasoftTemplate.ActiveStoreList, value != null ? string.Join(";", value) : string.Empty); 
            }
        }

        public List<SelectListItem> ListStoreList
        {
            get
            {
                var storeList = new MeasoftApiService(new MeasoftXmlConverter(new MeasoftAuthOption { Extra = Extra })).GetStoreList(Extra);
                if (storeList != null)
                    return storeList
                        .OrderBy(x => x.Code)
                        .Select(x => new SelectListItem
                        {
                            Text = x.Name,
                            Value = x.Code.ToString()
                        }).ToList();
                return new List<SelectListItem>();
            }
        }
    }
}
