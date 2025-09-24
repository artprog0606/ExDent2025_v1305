using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Shipping.Hermes;
using AdvantShop.Shipping.Hermes.Api;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("Hermes")]
    public class HermesShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {
        public string SecuredToken
        {
            get { return Params.ElementOrDefault(HermesTemplate.SecuredToken); }
            set { Params.TryAddValue(HermesTemplate.SecuredToken, value.DefaultOrEmpty()); }
        }

        public string PublicToken
        {
            get { return Params.ElementOrDefault(HermesTemplate.PublicToken); }
            set { Params.TryAddValue(HermesTemplate.PublicToken, value.DefaultOrEmpty()); }
        }

        private RestApiClient _service;
        private RestApiClient Service
        {
            get
            {
                if (_service == null && !string.IsNullOrEmpty(SecuredToken) && !string.IsNullOrEmpty(PublicToken))
                    _service = new RestApiClient(SecuredToken, PublicToken);

                return _service;
            }
        }

        public string BusinesUnitCode
        {
            get { return Params.ElementOrDefault(HermesTemplate.BusinessUnitCode); }
            set { Params.TryAddValue(HermesTemplate.BusinessUnitCode, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> BusinesUnitsCode
        {
            get
            {
                var list = new List<SelectListItem>() { new SelectListItem { Text = "-", Value = string.Empty} };

                if (Service != null)
                {
                    var getBusinessUnitsResponse = Service.GetBusinessUnits();
                    if (getBusinessUnitsResponse != null && getBusinessUnitsResponse.IsSuccess == true)
                        list.AddRange(
                            getBusinessUnitsResponse.BusinessUnits.Select(x => new SelectListItem()
                            {
                                Text = x.FullName,
                                Value = x.Code
                            }).OrderBy(x => x.Text));
                }

                return list;
            }
        }

        public bool AvailableDrop
        {
            get
            {
                if (Service != null)
                {
                    var getBusinessUnitsResponse = Service.GetBusinessUnits();
                    if (getBusinessUnitsResponse != null && getBusinessUnitsResponse.IsSuccess == true)
                    {
                        var businessUnit = getBusinessUnitsResponse.BusinessUnits.FirstOrDefault(x => x.Code == BusinesUnitCode);
                        return businessUnit != null && businessUnit.Services.Contains("DROP_OFF_TO_TARGET_PARCELSHOP");
                    }
                }

                return false;
            }
        }

        public bool AvailableDc
        {
            get
            {
                return Distributions != null && Distributions.Count > 1;
            }
        }

        public string ParcelShopLocation
        {
            get { return Params.ElementOrDefault(HermesTemplate.ParcelShopLocation); }
            set { Params.TryAddValue(HermesTemplate.ParcelShopLocation, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> ParcelsShop
        {
            get
            {
                var list = new List<SelectListItem>() { new SelectListItem { Text = "-", Value = string.Empty } };
                if (Service != null && AvailableDrop)
                {
                    var pointsCacheKey = string.Format("Hermes-{0}-ParcelShops", (SecuredToken + PublicToken + BusinesUnitCode).GetHashCode());
                    List<ParcelShop> points = null;
                    if (!CacheManager.TryGetValue(pointsCacheKey, out points))
                    {
                        var getParcelShops = Service.GetParcelShops(BusinesUnitCode);
                        if (getParcelShops != null && getParcelShops.IsSuccess == true)
                        {
                            points = getParcelShops.ParcelShops;
                            if (points != null)
                                CacheManager.Insert(pointsCacheKey, points, 60 * 24);
                        }
                    }

                    if (points != null)
                    {
                        list.AddRange(points.Select(x => new SelectListItem() { Text = string.Format("{0}, {1}, {2}", x.Region, x.City, x.Address), Value = x.ParcelShopCode }).OrderBy(x => x.Text));
                    }
                }
                return list;
            }
        }

        public string DistributionCenterLocation
        {
            get { return Params.ElementOrDefault(HermesTemplate.DistributionCenterLocation); }
            set { Params.TryAddValue(HermesTemplate.DistributionCenterLocation, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> Distributions
        {
            get
            {
                var list = new List<SelectListItem>() { new SelectListItem { Text = "-", Value = string.Empty } };

                if (Service != null)
                {
                    var getDistributionCenters = Service.GetDistributionCenters(BusinesUnitCode);
                    if (getDistributionCenters != null && getDistributionCenters.IsSuccess == true)
                        list.AddRange(
                            getDistributionCenters.DcModels.OrderByDescending(x => x.IsDefault).ThenBy(x => x.Address).Select(x => new SelectListItem()
                            {
                                Text = x.Address,
                                Value = x.DcCode
                            }));
                }

                return list;
            }
        }

        public bool LocationIsDistributionCenter
        {
            get { return Params.ElementOrDefault(HermesTemplate.LocationIsDistributionCenter).TryParseBool(); }
            set { Params.TryAddValue(HermesTemplate.LocationIsDistributionCenter, value.ToString()); }
        }

        public List<SelectListItem> LocationIsDistributionCenterValues
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem(){ Text = "Склад", Value = "true"},
                    new SelectListItem(){ Text = "ПВЗ (Drop Service)", Value = "false"},
                };
            }
        }

        public bool WithInsure
        {
            get { return Params.ElementOrDefault(HermesTemplate.WithInsure).TryParseBool(); }
            set { Params.TryAddValue(HermesTemplate.WithInsure, value.ToString()); }
        }

        public string TypeViewPoints
        {
            get { return Params.ElementOrDefault(HermesTemplate.TypeViewPoints, ((int)AdvantShop.Shipping.Hermes.TypeViewPoints.WidgetHermes).ToString()); }
            set { Params.TryAddValue(HermesTemplate.TypeViewPoints, value.DefaultOrEmpty()); }
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
            get { return Params.ElementOrDefault(HermesTemplate.YaMapsApiKey); }
            set { Params.TryAddValue(HermesTemplate.YaMapsApiKey, value.DefaultOrEmpty()); }
        }

        public override bool StatusesSync
        {
            get { return Params.ElementOrDefault(HermesTemplate.StatusesSync).TryParseBool(); }
            set { Params.TryAddValue(HermesTemplate.StatusesSync, value.ToString()); }
        }

        public override string StatusesReference
        {
            get { return Params.ElementOrDefault(HermesTemplate.StatusesReference); }
            set { Params.TryAddValue(HermesTemplate.StatusesReference, value.DefaultOrEmpty()); }
        }

        private Dictionary<string, string> _statuses;
        public override Dictionary<string, string> Statuses =>
        _statuses ?? (_statuses = Hermes.Statuses);

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(SecuredToken))
                yield return new ValidationResult("Введите токен клиента", new[] { "Token" });
            if (string.IsNullOrWhiteSpace(PublicToken))
                yield return new ValidationResult("Введите публичный токен", new[] { "Token" });
            //if (string.IsNullOrWhiteSpace(Login))
            //    yield return new ValidationResult("Введите логин", new[] { "Login" });
            //if (string.IsNullOrWhiteSpace(Password))
            //    yield return new ValidationResult("Введите пароль", new[] { "Password" });
        }
    }
}
