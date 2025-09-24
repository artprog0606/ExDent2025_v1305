using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Shipping.PointDelivery;
using Newtonsoft.Json;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("PointDelivery")]
    public class PointDeliveryShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {
        public string TypePoints
        {
            get { return Params.ElementOrDefault(PointDeliveryTemplate.TypePoints, ((int)EnTypePoints.List).ToString()); }
            set { Params.TryAddValue(PointDeliveryTemplate.TypePoints, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> ListTypePoints
        {
            get
            {
                return Enum.GetValues(typeof(EnTypePoints))
                    .Cast<EnTypePoints>()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Localize(),
                        Value = ((int)x).ToString()
                    })
                    .ToList();
            }
        }

        public List<DeliveryPointShipping> Points
        {
            get
            {
                var oldPoints = Params.ElementOrDefault(PointDeliveryTemplate.Points);
                if (!string.IsNullOrEmpty(oldPoints))
                    return oldPoints.Split(';').Select((x, index) => new DeliveryPointShipping
                    {
                        Id = index.ToString(),
                        Address = x
                    }).ToList();

                var newPoints = Params.ElementOrDefault(PointDeliveryTemplate.NewPoints)
                                     ?.Replace("\"PointX\":", "\"Latitude\":")
                                      .Replace("\"PointY\":", "\"Longitude\":");
                return !string.IsNullOrEmpty(newPoints)
                    ? JsonConvert.DeserializeObject<List<DeliveryPointShipping>>(newPoints)
                    : new List<DeliveryPointShipping>();
            }
            set
            {
                Params.TryAddValue(PointDeliveryTemplate.Points, "");

                Params.TryAddValue(PointDeliveryTemplate.NewPoints,
                    JsonConvert.SerializeObject(value ?? new List<DeliveryPointShipping>()));
            }
        }

        public string PointsJson
        {
            get
            {
                return JsonConvert.SerializeObject(Points);
            }
            set
            {
                Points = JsonConvert.DeserializeObject<List<DeliveryPointShipping>>(value);
            }
        }

        public string ShippingPrice
        {
            get { return Params.ElementOrDefault(PointDeliveryTemplate.ShippingPrice, "0"); }
            set { Params.TryAddValue(PointDeliveryTemplate.ShippingPrice, value.TryParseFloat().ToString()); }
        }

        public string DeliveryTime
        {
            get { return Params.ElementOrDefault(PointDeliveryTemplate.DeliveryTime); }
            set { Params.TryAddValue(PointDeliveryTemplate.DeliveryTime, value.DefaultOrEmpty()); }
        }

        public string YaMapsApiKey
        {
            get { return Params.ElementOrDefault(PointDeliveryTemplate.YaMapsApiKey); }
            set { Params.TryAddValue(PointDeliveryTemplate.YaMapsApiKey, value.DefaultOrEmpty()); }
        }

        public string PointListTitle
        {
            get { return Params.ElementOrDefault(PointDeliveryTemplate.PointListTitle, "Пункт самовывоза:"); }
            set { Params.TryAddValue(PointDeliveryTemplate.PointListTitle, value.DefaultOrEmpty()); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(ShippingPrice))
                yield return new ValidationResult("Укажите стоимость доставки");
            
            if (!ShippingPrice.IsDecimal())
                yield return new ValidationResult("Стоимость доставки дожна быть числом");

            if (PointListTitle.IsNullOrEmpty())
                yield return new ValidationResult("Не указано название пунктов самовывоза");
        }
    }
}
