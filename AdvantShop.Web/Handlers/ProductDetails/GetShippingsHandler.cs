using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.ViewModel.ProductDetails;
using System.Web;

namespace AdvantShop.Handlers.ProductDetails
{
    public class GetShippingsHandler
    {
        private readonly int _offerId;
        private readonly float _amount;
        private readonly string _customOptions;
        private readonly string _zip;

        public GetShippingsHandler(int offerId, float amount, string customOptions, string zip)
        {
            _offerId = offerId;
            _amount = amount;
            _customOptions = HttpUtility.UrlDecode(customOptions);
            _zip = zip;
        }

        public ShippingsViewModel Get()
        {
            if (OfferService.GetOffer(_offerId) == null ||
                SettingsDesign.ShowShippingsMethodsInDetails == SettingsDesign.eShowShippingsInDetails.Never)
            {
                return null;
            }

            var tempShopCart = new ShoppingCart
            {
                new ShoppingCartItem()
                {
                    Amount = _amount,
                    ShoppingCartType = ShoppingCartType.ShoppingCart,
                    OfferId = _offerId,
                    AttributesXml = _customOptions
                }
            };

            var currentZone = IpZoneContext.CurrentZone;
            if (!string.IsNullOrEmpty(_zip) && currentZone.Zip != _zip)
            {
                currentZone.Zip = _zip;

                if (string.IsNullOrEmpty(currentZone.City) && !string.IsNullOrEmpty(_zip))
                {
                    var city = CityService.GetCityByZip(_zip);
                    if (city != null)
                    {
                        currentZone.City = city.Name;
                        currentZone.District = city.District;

                        var region = RegionService.GetRegion(city.RegionId);
                        if (region != null)
                            currentZone.Region = region.Name;
                    }
                }
                IpZoneContext.SetCustomerCookie(currentZone);
            }

            var shippingManager = new ShippingManager(
                config => config
                         .WithCountry(currentZone.CountryName)
                         .WithRegion(currentZone.Region)
                         .WithDistrict(currentZone.District)
                         .WithCity(currentZone.City)
                         .WithZip(currentZone.Zip)
                         .WithCurrency(CurrencyService.CurrentCurrency)
                         .ByShoppingCart(tempShopCart)
                         .ShowOnlyInDetails()
                         .Build());
            var shippingRates = shippingManager.GetOptions();

            var showZip = ShippingMethodService.GetAllShippingMethods(true).Where(x => x.ShowInDetails && x.DisplayIndex).Any();
            if (shippingRates.Count == 0)
            {
                if (_zip.IsNotEmpty() || !showZip)
                    return null;

                return new ShippingsViewModel() {AdvancedObj = new {ShowZip = true}};
            }

            var model = new ShippingsViewModel()
            {
                Shippings =
                    shippingRates/*.OrderBy(item => item.FinalRate)*/.Take(SettingsDesign.ShippingsMethodsInDetailsCount)
                        .Select(x => new ShippingItemModel()
                        {
                            Name = x.NameRate ?? x.Name,
                            DeliveryTime = x.DeliveryTime,
                            Rate = x.FinalRate == 0 ? x.ZeroPriceMessage : x.FinalRate.FormatPrice()
                        }).ToList(),
                AdvancedObj = new
                {
                    ShowZip = showZip
                }
            };

            return model;
        }
    }
}