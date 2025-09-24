using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Shipping.Pec;
using AdvantShop.Shipping.Pec.Api;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("Pec")]
    public class PecShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {
        public string Login
        {
            get { return Params.ElementOrDefault(PecTemplate.Login); }
            set { Params.TryAddValue(PecTemplate.Login, value.DefaultOrEmpty()); }
        }

        public string ApiKey
        {
            get { return Params.ElementOrDefault(PecTemplate.ApiKey); }
            set { Params.TryAddValue(PecTemplate.ApiKey, value.DefaultOrEmpty()); }
        }

        public string LocationFrom
        {
            get { return Params.ElementOrDefault(PecTemplate.LocationFrom); }
            set { Params.TryAddValue(PecTemplate.LocationFrom, value.DefaultOrEmpty()); }
            // set { 
            //     Params.TryAddValue(PecTemplate.LocationFrom, value.DefaultOrEmpty());
            //     if (value.IsNotEmpty())
            //     {
            //         long? cityId = null;
            //         var pecApi = new PecApiService(Login, ApiKey);
            //         var findResult = pecApi.FindBranchesByTitle(value);
            //
            //         if (findResult != null && findResult.Success && findResult.Result.Success && findResult.Result.Items != null)
            //         {
            //             var cityItem = findResult.Result.Items.FirstOrDefault();
            //             if (cityItem != null)
            //                 cityId = cityItem.CityId.HasValue ? cityItem.CityId : cityItem.BranchId;
            //         }
            //
            //         if (cityId.HasValue)
            //         {
            //             Params.TryAddValue(PecTemplate.SenderCityId, cityId.Value.ToString());
            //         }
            //         else
            //         {
            //             Params.TryAddValue(PecTemplate.LocationFrom, "");
            //             Params.TryAddValue(PecTemplate.SenderCityId, "");
            //         }
            //     }
            //     else
            //         Params.TryAddValue(PecTemplate.SenderCityId, "");
            // }
        }

        public string LocationId
        {
            get { return Params.ElementOrDefault(PecTemplate.SenderCityId); }
            set { Params.TryAddValue(PecTemplate.SenderCityId, value.DefaultOrEmpty()); }
        }

        public string[] DeliveryTypes
        {
            get { return (Params.ElementOrDefault(PecTemplate.DeliveryTypes) ?? string.Empty).Split(","); }
            set { Params.TryAddValue(PecTemplate.DeliveryTypes, value != null ? string.Join(",", value) : string.Empty); }
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

        public string ShowTransportsDelivery
        {
            get { return Params.ElementOrDefault(PecTemplate.ShowTransportDelivery); }
            set { Params.TryAddValue(PecTemplate.ShowTransportDelivery, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> ListShowTransportDelivery
        {
            get
            {
                return EnTransportingType.AsList()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Localize(),
                        Value = x.Value.ToString()
                    })
                    .ToList();
            }
        }

        public bool WithInsure
        {
            get { return Params.ElementOrDefault(PecTemplate.WithInsure).TryParseBool(); }
            set { Params.TryAddValue(PecTemplate.WithInsure, value.ToString()); }
        }

        public string TypeViewPoints
        {
            get { return Params.ElementOrDefault(PecTemplate.TypeViewPoints, ((int)Shipping.Pec.TypeViewPoints.YaWidget).ToString()); }
            set { Params.TryAddValue(PecTemplate.TypeViewPoints, value.DefaultOrEmpty()); }
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

        public string YaMapsApiKey
        {
            get { return Params.ElementOrDefault(PecTemplate.YaMapsApiKey); }
            set { Params.TryAddValue(PecTemplate.YaMapsApiKey, value.DefaultOrEmpty()); }
        }

        public string SenderInn
        {
            get { return Params.ElementOrDefault(PecTemplate.SenderInn); }
            set { Params.TryAddValue(PecTemplate.SenderInn, value.DefaultOrEmpty()); }
        }

        public string SenderTitle
        {
            get { return Params.ElementOrDefault(PecTemplate.SenderTitle); }
            set { Params.TryAddValue(PecTemplate.SenderTitle, value.DefaultOrEmpty()); }
        }

        public string SenderFs
        {
            get { return Params.ElementOrDefault(PecTemplate.SenderFs); }
            set { Params.TryAddValue(PecTemplate.SenderFs, value.DefaultOrEmpty()); }
        }

        public string SenderPhone
        {
            get { return Params.ElementOrDefault(PecTemplate.SenderPhone); }
            set { Params.TryAddValue(PecTemplate.SenderPhone, value.DefaultOrEmpty()); }
        }

        public string SenderPhoneAdditional
        {
            get { return Params.ElementOrDefault(PecTemplate.SenderPhoneAdditional); }
            set { Params.TryAddValue(PecTemplate.SenderPhoneAdditional, value.DefaultOrEmpty()); }
        }

        public override bool StatusesSync
        {
            get { return Params.ElementOrDefault(PecTemplate.StatusesSync).TryParseBool(); }
            set { Params.TryAddValue(PecTemplate.StatusesSync, value.ToString()); }
        }

        public override string StatusesReference
        {
            get { return Params.ElementOrDefault(PecTemplate.StatusesReference); }
            set { Params.TryAddValue(PecTemplate.StatusesReference, value.DefaultOrEmpty()); }
        }

        private Dictionary<string, string> _statuses;
        public override Dictionary<string, string> Statuses
        {
            get
            {
                if (_statuses != null)
                    return _statuses;

                var resultGetStatuses = new PecApiService(Login, ApiKey).GetStatuses();
                if (resultGetStatuses != null && resultGetStatuses.Success && resultGetStatuses.Result != null)
                {
                    var dictionary = new Dictionary<string, string>();
                    foreach (var item in resultGetStatuses.Result)
                        if (!dictionary.ContainsKey(item.Id.ToString()))
                            dictionary.Add(item.Id.ToString(), item.Name);
                    return dictionary;
                }

                return _statuses = new Dictionary<string, string>();
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Login))
                yield return new ValidationResult("Введите логин", new[] { "Login" });
            if (string.IsNullOrWhiteSpace(ApiKey))
                yield return new ValidationResult("Введите ключ API", new[] { "ApiKey" });
            if (string.IsNullOrWhiteSpace(LocationId))
                yield return new ValidationResult("Укажите город отправления", new[] { "LocationId" });
        }
    }
}
