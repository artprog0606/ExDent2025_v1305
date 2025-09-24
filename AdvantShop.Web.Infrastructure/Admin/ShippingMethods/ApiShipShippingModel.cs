using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Repository;
using AdvantShop.Shipping.ApiShip;
using AdvantShop.Shipping.ApiShip.Api;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("ApiShip")]
    public class ApiShipShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {
        public string ApiKey
        {
            get => Params.ElementOrDefault(ApiShipTemplate.ApiKey);
            set => Params.TryAddValue(ApiShipTemplate.ApiKey, value.DefaultOrEmpty());
        }

        public string CityFrom
        {
            get => Params.ElementOrDefault(ApiShipTemplate.CityFrom);
            set => Params.TryAddValue(ApiShipTemplate.CityFrom, value.DefaultOrEmpty());
        }

        public bool ShowPointsAsList
        {
            get => (Params.ElementOrDefault(ApiShipTemplate.ShowPointsAsList) ?? "True").TryParseBool();
            set => Params.TryAddValue(ApiShipTemplate.ShowPointsAsList, value.ToString());
        }

        public bool ShowAddressComment
        {
            get => Params.ElementOrDefault(ApiShipTemplate.ShowAddressComment).TryParseBool();
            set => Params.TryAddValue(ApiShipTemplate.ShowAddressComment, value.ToString());
        }

        public string YaMapsApiKey
        {
            get => Params.ElementOrDefault(ApiShipTemplate.YaMapsApiKey);
            set => Params.TryAddValue(ApiShipTemplate.YaMapsApiKey, value.DefaultOrEmpty());
        }

        public string SenderName
        {
            get => Params.ElementOrDefault(ApiShipTemplate.SenderName);
            set => Params.TryAddValue(ApiShipTemplate.SenderName, value.DefaultOrEmpty().Reduce(255));
        }

        public string SenderPhone
        {
            get => Params.ElementOrDefault(ApiShipTemplate.SenderPhone);
            set => Params.TryAddValue(ApiShipTemplate.SenderPhone, value.DefaultOrEmpty());
        }
        public string SenderRegion
        {
            get => Params.ElementOrDefault(ApiShipTemplate.SenderRegion);
            set => Params.TryAddValue(ApiShipTemplate.SenderRegion, value.DefaultOrEmpty().Reduce(255));
        }
        public string SenderAddress
        {
            get => Params.ElementOrDefault(ApiShipTemplate.SenderAddress);
            set => Params.TryAddValue(ApiShipTemplate.SenderAddress, value.DefaultOrEmpty().Reduce(255));
        }

        public string ReturnAddress
        {
            get => Params.ElementOrDefault(ApiShipTemplate.ReturnAddress);
            set { Params.TryAddValue(ApiShipTemplate.ReturnAddress, value.DefaultOrEmpty().Reduce(255)); ClearCache(); }
        }

        public List<SelectListItem> ListCountries
        {
            get
            {
                var list = CountryService.GetAllCountries()
                    .Select(x=> new SelectListItem() 
                    { 
                        Text = x.Name,
                        Value = x.CountryId.ToString()
                    }).ToList();

                if (TypeOfDelivery is null)
                    list.Insert(0, new SelectListItem() { Text = LocalizationService.GetResource("AdvantShop.Shipping.AdminModel.Point.NotSet"), Value = "", Selected = true });
                return list;
            }
        }

        public int SendedCountry
        {
            get {
                string val = Params.ElementOrDefault(ApiShipTemplate.SendedCountry);
                int value = 0;
                if (!string.IsNullOrEmpty(val) && int.TryParse(val, out value))
                {
                    return value;
                }            
                return 0;
            }
            set { Params.TryAddValue(ApiShipTemplate.SendedCountry, value.ToString()); ClearCache(); }
        }

        public int ReturnCountry
        {
            get
            {
                string val = Params.ElementOrDefault(ApiShipTemplate.ReturnCountry);
                int value = 0;
                if (!string.IsNullOrEmpty(val) && int.TryParse(val, out value))
                {
                    return value;
                }
                return 0;
            }
            set { Params.TryAddValue(ApiShipTemplate.ReturnCountry, value.ToString()); ClearCache(); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
        
        private void ClearCache()
        {
            CacheManager.RemoveByPattern("ApiShip");
        }
    }
}
