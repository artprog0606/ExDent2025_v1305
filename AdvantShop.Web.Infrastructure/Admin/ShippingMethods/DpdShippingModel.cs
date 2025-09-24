using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Shipping.Dpd;
using AdvantShop.Shipping.Dpd.Api;
using AdvantShop.Shipping.Dpd.GeographyServices;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("Dpd")]
    public class DpdShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {

        public string ClientNumber
        {
            get { return Params.ElementOrDefault(DpdTemplate.ClientNumber); }
            set { Params.TryAddValue(DpdTemplate.ClientNumber, value.DefaultOrEmpty()); }
        }

        public string ClientKey
        {
            get { return Params.ElementOrDefault(DpdTemplate.ClientKey); }
            set { Params.TryAddValue(DpdTemplate.ClientKey, value.DefaultOrEmpty()); }
        }

        public bool TestServers
        {
            get { return Params.ElementOrDefault(DpdTemplate.TestServers).TryParseBool(); }
            set { Params.TryAddValue(DpdTemplate.TestServers, value.ToString()); }
        }

        public string PickupCountryIso2
        {
            get 
            {
                var value = Params.ElementOrDefault(DpdTemplate.PickupCountryIso2);
                if (value.IsNullOrEmpty())
                {
                    var sellerCountry = AdvantShop.Repository.CountryService.GetCountry(Configuration.SettingsMain.SellerCountryId);
                    if (sellerCountry != null)
                        value = sellerCountry.Iso2;
                }
                return value;
            }
            set {
                var oldValue = PickupCountryIso2;
                if (oldValue.IsNotEmpty() && !string.Equals(oldValue, value, StringComparison.OrdinalIgnoreCase))
                {
                    //Params.TryAddValue(DpdTemplate.PickupCityName, string.Empty);
                    Params.TryAddValue(DpdTemplate.PickupCityId, string.Empty);
                    //Params.TryAddValue(DpdTemplate.PickupCityCode, string.Empty);
                    Params.TryAddValue(DpdTemplate.PickupPointCode, string.Empty);
                }
                Params.TryAddValue(DpdTemplate.PickupCountryIso2, value.DefaultOrEmpty());
            }
        }

        public List<SelectListItem> Countries
        {
            get
            {
                var listPointIndex = new List<SelectListItem>();

                listPointIndex.AddRange(Repository.CountryService.GetAllCountries()
                    .Where(x => x.Iso2.IsNotEmpty())
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Iso2.ToUpper()
                    }));

                return listPointIndex;
            }
        }

        public bool SelfPickup
        {
            get { return Params.ElementOrDefault(DpdTemplate.SelfPickup).TryParseBool(); }
            set { Params.TryAddValue(DpdTemplate.SelfPickup, value.ToString()); }
        }

        public string[] EAEUCountriesIso2
        {
            get { return Dpd.EAEUCountriesIso2; }
        }

        public string PickupRegionName
        {
            get { return Params.ElementOrDefault(DpdTemplate.PickupRegionName); }
            set { Params.TryAddValue(DpdTemplate.PickupRegionName, value.DefaultOrEmpty()); }
        }

        public string PickupCityName
        {
            get { return Params.ElementOrDefault(DpdTemplate.PickupCityName); }
            set { Params.TryAddValue(DpdTemplate.PickupCityName, value.DefaultOrEmpty()); }
        }

        public string PickupPointCode
        {
            get { return Params.ElementOrDefault(DpdTemplate.PickupPointCode); }
            set {
                Params.TryAddValue(DpdTemplate.PickupPointCode, value.DefaultOrEmpty());

                if (value != "-1")
                {
                    var terminal = TerminalsService.Get(value);
                    if (terminal != null)
                    {
                        Params.TryAddValue(DpdTemplate.PickupRegionName, terminal.RegionName);
                        Params.TryAddValue(DpdTemplate.PickupCityName, terminal.CityName);
                        Params.TryAddValue(DpdTemplate.PickupCityId, terminal.CityId.ToString());
                        return;
                    }

                    var parcelShop = ParcelShopsService.Get(value);
                    if (parcelShop != null)
                    {
                        Params.TryAddValue(DpdTemplate.PickupRegionName, parcelShop.RegionName);
                        Params.TryAddValue(DpdTemplate.PickupCityName, parcelShop.CityName);
                        Params.TryAddValue(DpdTemplate.PickupCityId, parcelShop.CityId.ToString());
                    }
                }
                else
                {
                    Params.TryAddValue(DpdTemplate.PickupCityId, string.Empty);
                }
            }
        }

        public List<SelectListItem> PickupPoints
        {
            get
            {
                var listPointIndex = new List<SelectListItem>();

                if (ClientNumber.IsNotEmpty() && ClientKey.IsNotEmpty() && PickupCountryIso2.IsNotEmpty())
                {
                    if (!TerminalsService.ExistsTerminals() && !CashCityService.ExistsCities() && !ParcelShopsService.ExistsParcelShops())
                        Dpd.SyncGeographyServices(new DpdApiService(TestServers, ClientNumber.TryParseLong(), ClientKey));

                    var dictionaryCountries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach(var country in Repository.CountryService.GetAllCountries())
                        if (!dictionaryCountries.ContainsKey(country.Iso2))
                            dictionaryCountries.Add(country.Iso2, country.Name);

                    var selfPickup = SelfPickup;

                    var terminals = TerminalsService.Find(PickupCountryIso2, region: null, city: null, cityId: null);

                    listPointIndex.AddRange((terminals ?? new List<Terminal>())
                        .Where(t => !selfPickup || t.IsSelfPickup)
                        .Select(t => new SelectListItem()
                        {
                            Text = new[] { 
                                (dictionaryCountries.ContainsKey(t.CountryCode) ? dictionaryCountries[t.CountryCode] : t.CountryCode),
                                t.RegionName,
                                t.CityName,
                                t.Address
                            }.Where(x => x.IsNotEmpty())
                            .AggregateString(", "),
                            Value = t.Code
                        }));

                    var parcelShops = ParcelShopsService.Find(PickupCountryIso2, region: null, city: null, cityId: null);

                    listPointIndex.AddRange((parcelShops ?? new List<ParcelShop>())
                        .Where(t => !selfPickup || t.IsSelfPickup)
                        .Select(t => new SelectListItem()
                        {
                            Text = new[] {
                                (dictionaryCountries.ContainsKey(t.CountryCode) ? dictionaryCountries[t.CountryCode] : t.CountryCode),
                                t.RegionName,
                                t.CityName,
                                t.Address
                            }.Where(x => x.IsNotEmpty())
                            .AggregateString(", "),
                            Value = t.Code
                        }));
                }

                var result = listPointIndex.OrderBy(x => x.Text).ToList();
                result.Add(new SelectListItem() { Text = "Нет пункта в моем городе", Value = "-1" });
                return result;
            }
        }

        public string[] DeliveryTypes
        {
            get { return (Params.ElementOrDefault(DpdTemplate.DeliveryTypes) ?? string.Empty).Split(","); }
            set { Params.TryAddValue(DpdTemplate.DeliveryTypes, value != null ? string.Join(",", value) : string.Empty); }
        }

        public List<SelectListItem> ListDeliveryTypes
        {
            get
            {
                return Enum.GetValues(typeof(EnDeliveryType))
                    .Cast<EnDeliveryType>()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Localize(),
                        Value = ((int)x).ToString()
                    })
                    .ToList();
            }
        }

        public string[] ServiceCodes
        {
            get { return (Params.ElementOrDefault(DpdTemplate.ServiceCodes) ?? string.Empty).Split(","); }
            set { Params.TryAddValue(DpdTemplate.ServiceCodes, value != null ? string.Join(",", value) : string.Empty); }
        }

        public List<SelectListItem> ServiceCodesList
        {
            get
            {
                return new List<SelectListItem> {
                    new SelectListItem() { Text = "DPD 18:00", Value = "BZP" },
                    new SelectListItem() { Text = "DPD ECONOMY", Value = "ECN" },
                    new SelectListItem() { Text = "DPD ECONOMY CU", Value = "ECU" },
                    new SelectListItem() { Text = "DPD CLASSIC", Value = "CUR" },
                    new SelectListItem() { Text = "DPD EXPRESS", Value = "NDY" },
                    new SelectListItem() { Text = "DPD Online Express", Value = "CSM" },
                    new SelectListItem() { Text = "DPD OPTIMUM", Value = "PCL" },
                    new SelectListItem() { Text = "DPD SHOP", Value = "PUP" },
                    new SelectListItem() { Text = "DPD MAX domestic", Value = "MAX" },
                    new SelectListItem() { Text = "DPD Standart", Value = "MXO" },
                    new SelectListItem() { Text = "DPD CLASSIC international IMPORT", Value = "DPI" },
                    new SelectListItem() { Text = "DPD CLASSIC international EXPORT", Value = "DPE" },
                };
            }
        }

        public bool WithInsure
        {
            get { return Params.ElementOrDefault(DpdTemplate.WithInsure).TryParseBool(); }
            set { Params.TryAddValue(DpdTemplate.WithInsure, value.ToString()); }
        }

        public string TypeViewPoints
        {
            get { return Params.ElementOrDefault(DpdTemplate.TypeViewPoints, ((int)AdvantShop.Shipping.Dpd.TypeViewPoints.YaWidget).ToString()); }
            set { Params.TryAddValue(DpdTemplate.TypeViewPoints, value.DefaultOrEmpty()); }
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
            get { return Params.ElementOrDefault(DpdTemplate.YaMapsApiKey); }
            set { Params.TryAddValue(DpdTemplate.YaMapsApiKey, value.DefaultOrEmpty()); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(ClientNumber))
                yield return new ValidationResult("Введите клиентский номер в системе DPD", new[] { "ClientNumber" });
            if (string.IsNullOrWhiteSpace(ClientKey))
                yield return new ValidationResult("Введите уникальный ключ для авторизации", new[] { "ClientKey" });
        }
    }
}
