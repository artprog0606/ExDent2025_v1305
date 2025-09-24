using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Repository;
using AdvantShop.Repository;
using AdvantShop.ViewModel.Shipping;
using System.Linq;

namespace AdvantShop.Handlers.Shipings
{
    public class GetShippingZonesPageHandler
    {
        public ShippingZonesPageViewModel Execute()
        {
            var model = new ShippingZonesPageViewModel();
            var cityId = IpZoneContext.CurrentZone.CityId;
            if (cityId == 0)
            {
                var city = CityService.GetCityByName(IpZoneContext.CurrentZone.City);
                cityId = city?.CityId ?? 0;
            }
            if (cityId == 0)
                return model;
            var additionalOptions = AdditionalOptionsService.Get(cityId, EnAdditionalOptionObjectType.City);

            var iframe =
                additionalOptions
                   .FirstOrDefault(option =>
                        CityAdditionalOptionNames.ShippingZonesIframe.Equals(option.Name))
                  ?.Value;
            var description = 
                additionalOptions
                   .FirstOrDefault(option =>
                        CityAdditionalOptionNames.Description.Equals(option.Name))
                  ?.Value;
            if (iframe.IsNullOrEmpty() && description.IsNullOrEmpty())
                return model;
            model.ShippingZonesIframe = iframe;
            model.Description = description;
            return model;
        }
    }
}
