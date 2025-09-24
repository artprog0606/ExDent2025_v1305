using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Shipping.OzonRocket;
using AdvantShop.Shipping.OzonRocket.Api;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("OzonRocket")]
    public class OzonRocketShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {
        public string ClientId
        {
            get => Params.ElementOrDefault(OzonRocketTemplate.ClientId);
            set => Params.TryAddValue(OzonRocketTemplate.ClientId, value.DefaultOrEmpty());
        }
       
        public string ClientSecret
        {
            get => Params.ElementOrDefault(OzonRocketTemplate.ClientSecret);
            set => Params.TryAddValue(OzonRocketTemplate.ClientSecret, value.DefaultOrEmpty());
        }
       
        public string FromPlaceId
        {
            get => Params.ElementOrDefault(OzonRocketTemplate.FromPlaceId);
            set => Params.TryAddValue(OzonRocketTemplate.FromPlaceId, value.DefaultOrEmpty());
        }
 
        public List<SelectListItem> FromPlaces
        {
            get
            {
                var list = new List<SelectListItem>() { new SelectListItem { Text = "-", Value = string.Empty} };

                if (ClientId.IsNotEmpty() && ClientSecret.IsNotEmpty())
                {
                    var ozonRocketClient = OzonRocketClient.Create(ClientId, ClientSecret);
                    var fromPlaces = ozonRocketClient.Delivery.GetDropOffPlaces();
                    if (fromPlaces != null)
                        list.AddRange(
                            fromPlaces.Select(x => new SelectListItem()
                            {
                                Text = $"{x.Address} ({x.Name})",
                                Value = x.Id.ToString()
                            }).OrderBy(x => x.Text));
                }

                return list;
            }
        }
      
        public string[] DeliveryTypes
        {
            get { return (Params.ElementOrDefault(OzonRocketTemplate.DeliveryTypes) ?? string.Empty).Split(","); }
            set { Params.TryAddValue(OzonRocketTemplate.DeliveryTypes, value != null ? string.Join(",", value) : string.Empty); }
        }

        public List<SelectListItem> ListDeliveryTypes
        {
            get
            {
                var listDeliveryTypes = new List<SelectListItem>();

                foreach (var delivertyType in DeliveryType.AsList())
                {
                    listDeliveryTypes.Add(new SelectListItem()
                    {
                        Text = delivertyType.Localize(),
                        Value = delivertyType.Value
                    });
                }

                return listDeliveryTypes;
            }
        }
        
        public string TypeViewPoints
        {
            get { return Params.ElementOrDefault(OzonRocketTemplate.TypeViewPoints, ((int)Shipping.OzonRocket.TypeViewPoints.YaWidget).ToString()); }
            set { Params.TryAddValue(OzonRocketTemplate.TypeViewPoints, value.DefaultOrEmpty()); }
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

        public bool ShowAddressComment
        {
            get { return Params.ElementOrDefault(OzonRocketTemplate.ShowAddressComment).TryParseBool(); }
            set { Params.TryAddValue(OzonRocketTemplate.ShowAddressComment, value.ToString()); }
        }

        public string YaMapsApiKey
        {
            get { return Params.ElementOrDefault(OzonRocketTemplate.YaMapsApiKey); }
            set { Params.TryAddValue(OzonRocketTemplate.YaMapsApiKey, value.DefaultOrEmpty()); }
        }

        public string OzonRocketWidgetToken
        {
            get { return Params.ElementOrDefault(OzonRocketTemplate.OzonRocketWidgetToken); }
            set { Params.TryAddValue(OzonRocketTemplate.OzonRocketWidgetToken, value.DefaultOrEmpty()); }
        }

        public bool OzonRocketWidgetShowDeliveryPrice
        {
            get { return Params.ElementOrDefault(OzonRocketTemplate.OzonRocketWidgetShowDeliveryPrice, "True").TryParseBool(); }
            set { Params.TryAddValue(OzonRocketTemplate.OzonRocketWidgetShowDeliveryPrice, value.ToString()); }
        }

        public bool OzonRocketWidgetShowDeliveryTime
        {
            get { return Params.ElementOrDefault(OzonRocketTemplate.OzonRocketWidgetShowDeliveryTime, "True").TryParseBool(); }
            set { Params.TryAddValue(OzonRocketTemplate.OzonRocketWidgetShowDeliveryTime, value.ToString()); }
        }
 
        public override bool StatusesSync
        {
            get { return Params.ElementOrDefault(OzonRocketTemplate.StatusesSync).TryParseBool(); }
            set { Params.TryAddValue(OzonRocketTemplate.StatusesSync, value.ToString()); }
        }

        public override string StatusesReference
        {
            get { return Params.ElementOrDefault(OzonRocketTemplate.StatusesReference); }
            set { Params.TryAddValue(OzonRocketTemplate.StatusesReference, value.DefaultOrEmpty()); }
        }

        private Dictionary<string, string> _statuses;
        public override Dictionary<string, string> Statuses => _statuses ?? (_statuses = OzonRocket.Statuses);

        public bool AllowPartialDelivery
        {
            get { return Params.ElementOrDefault(OzonRocketTemplate.AllowPartialDelivery).TryParseBool(); }
            set { Params.TryAddValue(OzonRocketTemplate.AllowPartialDelivery, value.ToString()); }
        }
 
        public bool AllowUncovering
        {
            get { return Params.ElementOrDefault(OzonRocketTemplate.AllowUncovering).TryParseBool(); }
            set { Params.TryAddValue(OzonRocketTemplate.AllowUncovering, value.ToString()); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(ClientId))
                yield return new ValidationResult("Введите ClientId", new[] { "ClientId" });
            if (string.IsNullOrWhiteSpace(ClientSecret))
                yield return new ValidationResult("Введите ClientSecret", new[] { "ClientSecret" });
        }
    }
}