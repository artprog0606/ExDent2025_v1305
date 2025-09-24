using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;

using AdvantShop.Shipping.Boxberry;
using AdvantShop.Core.Services.Shipping.Boxberry;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("Boxberry")]
    public class BoxberryShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {
        private const string ApiUrlDe = "http://api.boxberry.de/json.php"; // for clients registered before 2019-06-01
        private const string ApiUrlRu = "http://api.boxberry.ru/json.php"; // for clients registered after 2019-06-01

        public string ApiUrl
        {
            get { return Params.ElementOrDefault(BoxberryTemplate.ApiUrl) ?? (Token.IsNotEmpty() ? ApiUrlDe : ApiUrlRu); }
            set { Params.TryAddValue(BoxberryTemplate.ApiUrl, value.DefaultOrEmpty()); }
        }
        public string Token
        {
            get { return Params.ElementOrDefault(BoxberryTemplate.Token); }
            set
            {
                if (value.IsNotEmpty() && ApiUrl.IsNotEmpty() && IntegrationToken.IsNullOrEmpty())
                {
                    try
                    {
                        var result = new BoxberryApiService(
                            ApiUrl,
                            value,
                            string.Empty).GetKeyIntegration();

                        if (result != null && result.Key.IsNotEmpty())
                            IntegrationToken = result.Key;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Error(ex);
                    }
                }
                Params.TryAddValue(BoxberryTemplate.Token, value.DefaultOrEmpty());
            }
        }
        
        public string IntegrationToken
        {
            get { return Params.ElementOrDefault(BoxberryTemplate.IntegrationToken); }
            set { Params.TryAddValue(BoxberryTemplate.IntegrationToken, value.DefaultOrEmpty()); }
        }

        public string ReceptionPointCode
        {
            get { return Params.ElementOrDefault(BoxberryTemplate.ReceptionPointCode) ?? null; }
            set { Params.TryAddValue(BoxberryTemplate.ReceptionPointCode, value.DefaultOrEmpty()); }
        }

        public int ReceptionPointCity
        {
            get { return Params.ElementOrDefault(BoxberryTemplate.ReceptionPointCity).TryParseInt(); }
            set { Params.TryAddValue(BoxberryTemplate.ReceptionPointCity, value.ToString()); }
        }

        public bool CalculateCourier
        {
            get { return Params.ElementOrDefault(BoxberryTemplate.CalculateCourier).TryParseBool(); }
            set { Params.TryAddValue(BoxberryTemplate.CalculateCourier, value.ToString()); }
        }

        private string[] _deliveryTypes;
        public string[] DeliveryTypes
        {
            get
            {
                if (_deliveryTypes == null)
                {
                    if (Params.ContainsKey(BoxberryTemplate.DeliveryTypes))
                        _deliveryTypes = (Params.ElementOrDefault(BoxberryTemplate.DeliveryTypes) ?? string.Empty).Split(",");
                    else
                    {
                        // поддержка настроек ранних версий
                        var tempDeliveries = Enum.GetValues(typeof(TypeDelivery)).Cast<TypeDelivery>().Select(x => ((int)x).ToString()).ToList();
                        if (CalculateCourier == false)
                            tempDeliveries.Remove(((int)TypeDelivery.Courier).ToString());

                        _deliveryTypes = tempDeliveries.ToArray();
                    }
                }

                return _deliveryTypes;
            }
            set { Params.TryAddValue(BoxberryTemplate.DeliveryTypes, value != null ? string.Join(",", value) : string.Empty); }
        }

        public List<SelectListItem> ListDeliveryTypes
        {
            get
            {
                var listDeliveryTypes = new List<SelectListItem>();

                foreach(var delivertyType in Enum.GetValues(typeof(TypeDelivery)).Cast<TypeDelivery>())
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

        public bool WithInsure
        {
            get { return Params.ElementOrDefault(BoxberryTemplate.WithInsure).TryParseBool(); }
            set { Params.TryAddValue(BoxberryTemplate.WithInsure, value.ToString()); }
        }

        public string TypeOption
        {
            get { return Params.ElementOrDefault(BoxberryTemplate.TypeOption, ((int)AdvantShop.Shipping.Boxberry.TypeOption.WidgetBoxberry).ToString()); }
            set { Params.TryAddValue(BoxberryTemplate.TypeOption, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> ListTypesOption
        {
            get
            {
                return Enum.GetValues(typeof(TypeOption))
                    .Cast<TypeOption>()
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
            get { return Params.ElementOrDefault(BoxberryTemplate.YaMapsApiKey); }
            set { Params.TryAddValue(BoxberryTemplate.YaMapsApiKey, value.DefaultOrEmpty()); }
        }

        public override bool StatusesSync
        {
            get { return Params.ElementOrDefault(BoxberryTemplate.StatusesSync).TryParseBool(); }
            set { Params.TryAddValue(BoxberryTemplate.StatusesSync, value.ToString()); }
        }

        public override string StatusesReference
        {
            get { return Params.ElementOrDefault(BoxberryTemplate.StatusesReference); }
            set { Params.TryAddValue(BoxberryTemplate.StatusesReference, value.DefaultOrEmpty()); }
        }

        private Dictionary<string, string> _statuses;
        public override Dictionary<string, string> Statuses => _statuses ?? (_statuses = Boxberry.Statuses);

        public List<BoxberryParcelPoint> ListReceptionPoints
        {
            get
            {
                var points = new List<BoxberryParcelPoint>();
                List<BoxberryParcelPoint> pointsFromBoxberry = null;

                try
                {
                    pointsFromBoxberry = new BoxberryApiService(
                        this.ApiUrl,
                        this.Token,
                        this.ReceptionPointCode).GetListPointsForParcels();
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }

                if (pointsFromBoxberry == null || pointsFromBoxberry.Count == 0)
                {
                    points.Add(new BoxberryParcelPoint
                    {
                        Code = null,
                        City = null,
                        Name = "Сохраните токен для загрузки списка пунктов забора"
                    });
                }
                else
                {
                    points.Add(new BoxberryParcelPoint
                    {
                        Code = null,
                        City = null,
                        Name = "Не выбрано"
                    });

                    points.AddRange(pointsFromBoxberry);
                }

                return points;
            }
        }

        public List<BoxberryCity> ListReceptionCities
        {
            get
            {
                return new BoxberryApiService(
                    this.ApiUrl,
                    this.Token,
                    this.ReceptionPointCode).GetListCities();
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}
