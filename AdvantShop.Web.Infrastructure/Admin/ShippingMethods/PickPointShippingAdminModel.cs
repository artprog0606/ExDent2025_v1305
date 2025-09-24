using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Core.Services.Shipping.PickPoint.Api;
using AdvantShop.Core.Services.Shipping.PickPoint.Postamats;
using AdvantShop.Diagnostics;
using AdvantShop.Shipping.PickPoint;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("PickPoint")]
    public class PickPointShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {
        public string Login
        {
            get { return Params.ElementOrDefault(PickPointTemplate.Login); }
            set { Params.TryAddValue(PickPointTemplate.Login, value.DefaultOrEmpty()); }
        }

        public string Password
        {
            get { return Params.ElementOrDefault(PickPointTemplate.Password); }
            set { Params.TryAddValue(PickPointTemplate.Password, value.DefaultOrEmpty()); }
        }

        public string Ikn
        {
            get { return Params.ElementOrDefault(PickPointTemplate.Ikn); }
            set { Params.TryAddValue(PickPointTemplate.Ikn, value.DefaultOrEmpty()); }
        }

        public string FromRegion
        {
            get { return Params.ElementOrDefault(PickPointTemplate.FromRegion); }
            set { Params.TryAddValue(PickPointTemplate.FromRegion, value.DefaultOrEmpty()); }
        }

        public string FromCity
        {
            get { return Params.ElementOrDefault(PickPointTemplate.FromCity); }
            set { Params.TryAddValue(PickPointTemplate.FromCity, value.DefaultOrEmpty()); }
        }

        public string FromCityId
        {
            get { return Params.ElementOrDefault(PickPointTemplate.FromCityId); }
            set {
                try
                {
                    var cityId = value.TryParseInt(true);

                    if (cityId.HasValue)
                    {
                        var apiClient = new PickPointApiService(Login, Password);
                        var authResponse = apiClient.Authentication(Ikn);
                        if (authResponse != null && authResponse.Error.IsNullOrEmpty() && 
                            authResponse.Cities != null && authResponse.Cities.Count > 0)
                        {
                            var city = authResponse.Cities.FirstOrDefault(x => x.Id == cityId);

                            if (city != null)
                            {
                                Params.TryAddValue(PickPointTemplate.FromCityId, cityId.Value.ToString());
                                Params.TryAddValue(PickPointTemplate.FromCity, (city.City.IsNullOrEmpty() ? city.Region : city.City).DefaultOrEmpty());
                                Params.TryAddValue(PickPointTemplate.FromRegion, city.Region.DefaultOrEmpty());
                            }
                        }
                        else
                        {
                            var cities = apiClient.GetCities();
                            var city = cities?.FirstOrDefault(x => x.Id == cityId);

                            if (city != null)
                            {
                                Params.TryAddValue(PickPointTemplate.FromCityId, cityId.Value.ToString());
                                Params.TryAddValue(PickPointTemplate.FromCity, (city.Name.IsNullOrEmpty() ? city.RegionName : city.Name).DefaultOrEmpty());
                                Params.TryAddValue(PickPointTemplate.FromRegion, city.RegionName.DefaultOrEmpty().RemoveTypeFromRegion());
                            }
                        }

                        // заодно загружаем постоматы, если они ранее не грузились
                        if (!PostamatService.ExistsPostamats(Ikn))
                            PickPoint.SyncPostamats(apiClient, Ikn);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Warn(ex);
                }
            }
        }

        public List<SelectListItem> Cities
        {
            get
            {
                if (Login.IsNotEmpty() && Password.IsNotEmpty() && Ikn.IsNotEmpty())
                {
                    var apiClient = new PickPointApiService(Login, Password);
                    var authResponse = apiClient.Authentication(Ikn);
                    if (authResponse != null && authResponse.Error.IsNullOrEmpty() && 
                        authResponse.Cities != null && authResponse.Cities.Count > 0)
                    {
                        var cities = authResponse.Cities
                            .OrderBy(x => x.Region)
                            .ThenBy(x => x.City)
                                .Select(x => new SelectListItem()
                                {
                                    Text = string.Format("{0}{2}{1}", x.Region, x.City, x.City.IsNotEmpty() ? ", " : string.Empty),
                                    Value = x.Id.ToString()
                                })
                                .ToList();

                        if (FromCityId.IsNullOrEmpty() || cities.All(x => x.Value != FromCityId))
                            cities.Add(new SelectListItem() { Text = "Не указан город", Value = "", Selected = true });

                        return cities;
                    }
                    else
                    {
                        var cities = apiClient.GetCities();
                        if (cities != null)
                        {
                            var citiesItems = cities
                                .OrderBy(x => x.RegionName)
                                .ThenBy(x => x.Name)
                                .Select(x => new SelectListItem()
                                {
                                    Text = string.Format("{0}{2}{1}", x.RegionName, x.Name, x.Name.IsNotEmpty() ? ", " : string.Empty),
                                    Value = x.Id.ToString()
                                })
                                .ToList();

                            if (FromCityId.IsNullOrEmpty() || citiesItems.All(x => x.Value != FromCityId))
                                citiesItems.Add(new SelectListItem() { Text = "Не указан город", Value = "", Selected = true });

                            return citiesItems;
                        }
                    }
                }

                return new List<SelectListItem>();
            }
        }

        public string GettingType
        {
            get { return Params.ElementOrDefault(PickPointTemplate.GettingType); }
            set { Params.TryAddValue(PickPointTemplate.GettingType, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> GettingTypes
        {
            get
            {
                return
                    Enum.GetValues(typeof(GettingType))
                        .Cast<GettingType>()
                        .Select(x => new SelectListItem()
                        {
                            Text = x.Localize(),
                            Value = ((int) x).ToString()
                        })
                        .ToList();
            }
        }
        
        public string FromPostamatId
        {
            get { return Params.ElementOrDefault(PickPointTemplate.FromPostamatId); }
            set { Params.TryAddValue(PickPointTemplate.FromPostamatId, value.DefaultOrEmpty()); }
        }
 
        public List<SelectListItem> PostamatsSelfDelivery
        {
            get
            {
                if (Ikn.IsNotEmpty())
                {
                    var postamats = PostamatService.GetSelfDeliveryList(FromRegion, FromCity, Ikn);
                    if (postamats != null)
                    {
                        var postamatsItems = postamats
                            .OrderBy(x => x.Region)
                            .ThenBy(x => x.City)
                            .ThenBy(x => x.Address)
                            .Select(x => new SelectListItem()
                            {
                                Text = $"{x.Region}, {x.City}, {x.Address}",
                                Value = x.Id.ToString()
                            })
                            .ToList();

                        // if (FromPostamatId.IsNullOrEmpty() || postamatsItems.All(x => x.Value != FromPostamatId))
                            postamatsItems.Add(new SelectListItem() { Text = "-", Value = "" });

                        return postamatsItems;
                    }
                }

                return new List<SelectListItem>();
            }
        }
       
        public string DeliveryMode
        {
            get { return Params.ElementOrDefault(PickPointTemplate.DeliveryMode); }
            set { Params.TryAddValue(PickPointTemplate.DeliveryMode, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> DeliveryModes
        {
            get
            {
                return
                    Enum.GetValues(typeof(DeliveryMode))
                        .Cast<DeliveryMode>()
                        .Select(x => new SelectListItem()
                        {
                            Text = x.Localize(),
                            Value = ((int) x).ToString()
                        })
                        .ToList();
            }
        }

        public string TypeViewPoints
        {
            get { return Params.ElementOrDefault(PickPointTemplate.TypeViewPoints, ((int)Shipping.PickPoint.TypeViewPoints.YaWidget).ToString()); }
            set { Params.TryAddValue(PickPointTemplate.TypeViewPoints, value.DefaultOrEmpty()); }
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
            get { return Params.ElementOrDefault(PickPointTemplate.ShowAddressComment).TryParseBool(); }
            set { Params.TryAddValue(PickPointTemplate.ShowAddressComment, value.ToString()); }
        }

        public string YaMapsApiKey
        {
            get { return Params.ElementOrDefault(PickPointTemplate.YaMapsApiKey); }
            set { Params.TryAddValue(PickPointTemplate.YaMapsApiKey, value.DefaultOrEmpty()); }
        }

        public override bool StatusesSync
        {
            get { return Params.ElementOrDefault(PickPointTemplate.StatusesSync).TryParseBool(); }
            set { Params.TryAddValue(PickPointTemplate.StatusesSync, value.ToString()); }
        }

        public override string StatusesReference
        {
            get { return Params.ElementOrDefault(PickPointTemplate.StatusesReference); }
            set { Params.TryAddValue(PickPointTemplate.StatusesReference, value.DefaultOrEmpty()); }
        }

        private Dictionary<string, string> _statuses;
        public override Dictionary<string, string> Statuses
        {
            get
            {
                if (_statuses != null)
                    return _statuses;

                var statuses = new PickPointApiService(string.Empty, string.Empty).GetStatuses();
                _statuses = new Dictionary<string, string>();

                if (statuses != null)
                {
                    foreach (var status in statuses)
                    {
                        _statuses.Add(status.Code.ToString(), status.Name);
                        if (status.VisualStates != null)
                            foreach (var visualState in status.VisualStates)
                                _statuses.Add(visualState.Code.ToString(), status.Name + " - " + visualState.Name);
                    }
                }

                return _statuses;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Login))
            {
                yield return new ValidationResult("Укажите логин", new[] { "Login" });
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult("Укажите пароль", new[] { "Password" });
            }
            if (string.IsNullOrWhiteSpace(Ikn))
            {
                yield return new ValidationResult("Укажите номер договора", new[] { "Ikn" });
            }
            var authResponse = new PickPointApiService(Login, Password).Authentication(Ikn);
            if (authResponse == null || authResponse.Error.IsNotEmpty())
            {
                yield return new ValidationResult(
                    authResponse == null || authResponse.Error.IsNullOrEmpty() 
                        ? "Неверная комбинация логина/пароля/номера договора" 
                        : "Ошибка авторизации: " + authResponse.Error);
            }
            // if (string.IsNullOrWhiteSpace(FromCityId))
            // {
            //     yield return new ValidationResult("Укажите город отправления", new[] { "FromCity" });
            // }
            //if (string.IsNullOrWhiteSpace(FromCity))
            //{
            //    yield return new ValidationResult("Укажите город отправления", new[] { "FromCity" });
            //}
            //if (string.IsNullOrWhiteSpace(FromRegion))
            //{
            //    yield return new ValidationResult("Укажите регион города отправления", new[] { "FromRegion" });
            //}
            if (string.IsNullOrWhiteSpace(GettingType))
            {
                yield return new ValidationResult("Укажите тип сдачи отправления", new[] { "GettingType" });
            }
            if (string.IsNullOrWhiteSpace(DeliveryMode))
            {
                yield return new ValidationResult("Укажите режим доставки", new[] { "DeliveryMode" });
            }

            if (TypeViewPoints == ((int)Shipping.PickPoint.TypeViewPoints.YaWidget).ToString() && string.IsNullOrWhiteSpace(YaMapsApiKey))
            {
                yield return new ValidationResult("Укажите API-ключ яндекс.карт", new[] { "YaMapsApiKey" });
            }
        }

    }
}
