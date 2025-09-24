using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Shipping.LPost;
using AdvantShop.Shipping.LPost.Api;
using AdvantShop.Shipping.LPost.PickPoints;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("LPost")]
    public class LPostShippingModel : ShippingMethodAdminModel
    {
        public string SecretKey
        {
            get { return Params.ElementOrDefault(LPostTemplate.SecretKey); }
            set 
            {
                Params.TryAddValue(LPostTemplate.SecretKey, value.DefaultOrEmpty());
                if (value.IsNotEmpty() && !LPostPickPointService.ExistsPickPoints())
                {
                    var apiService = new LPostApiService(value);
                    if (apiService.GetToken().IsNotEmpty())
                        LPost.SyncPickPoints(apiService);
                }
            }
        }
        public bool WithInsure
        {
            get { return Params.ElementOrDefault(LPostTemplate.WithInsure).TryParseBool(); }
            set { Params.TryAddValue(LPostTemplate.WithInsure, value.ToString()); }
        }

        private string[] _deliveryTypes;
        public string[] DeliveryTypes
        {
            get { return _deliveryTypes ?? (_deliveryTypes = (Params.ElementOrDefault(LPostTemplate.DeliveryTypes) ?? string.Empty).Split(",")); }
            set { Params.TryAddValue(LPostTemplate.DeliveryTypes, value != null ? string.Join(",", value) : string.Empty); }
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
            get { return Params.ElementOrDefault(LPostTemplate.TypeViewPoints, ((int)Shipping.LPost.TypeViewPoints.WidgetPVZ).ToString()); }
            set { Params.TryAddValue(LPostTemplate.TypeViewPoints, value.DefaultOrEmpty()); }
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

        public string Token
        {
            get { return new LPostApiService(SecretKey).GetToken(); }
        }

        public string DefaultReceivePoint
        {
            get { return Params.ElementOrDefault(LPostTemplate.ReceivePoint, "3"); }
            set { Params.TryAddValue(LPostTemplate.ReceivePoint, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> ListReceivePoints
        {
            get
            {
                return new LPostApiService(SecretKey).GetReceivePoints()
                    .OrderBy(x => x.City)
                    .ThenBy(x => x.Address)
                    .Select(x => new SelectListItem
                    {
                        Text = $"{x.City}, {x.Address}",
                        Value = x.WarehouseId.ToString()
                    }).ToList();
            }
        }

        public string YaMapsApiKey
        {
            get { return Params.ElementOrDefault(LPostTemplate.YandexMapApiKey); }
            set { Params.TryAddValue(LPostTemplate.YandexMapApiKey, value.DefaultOrEmpty()); }
        }

        //public TypeViewPoints TypeViewPoints
        //{
        //    get 
        //    { 
        //        return (TypeViewPoints)Enum.Parse(
        //        typeof(TypeViewPoints),
        //        Params.ElementOrDefault(
        //            LPostTemplate.TypeViewPoints, 
        //            TypeViewPoints.WidgetPVZ.ToString())
        //        ); 
        //    }
        //    set { Params.TryAddValue(LPostTemplate.TypeViewPoints, value); }
        //}
    }
}
