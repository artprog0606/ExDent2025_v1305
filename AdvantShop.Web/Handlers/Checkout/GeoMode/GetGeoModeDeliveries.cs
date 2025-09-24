using System.Linq;
using System.Web;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.GeoModes;
using AdvantShop.Handlers.Home;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Shipping;
using AdvantShop.ViewModel.Checkout;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Handlers.Checkout.GeoMode
{
    public sealed class GetGeoModeDeliveries : AbstractCommandHandler<GeoModeDeliveriesResponse>
    {
        private readonly CalculationVariants? _typeCalculationVariants;

        public GetGeoModeDeliveries(CalculationVariants? typeCalculationVariants)
        {
            _typeCalculationVariants = typeCalculationVariants;
        }
        
        protected override GeoModeDeliveriesResponse Handle()
        {
            var customer = CustomerContext.CurrentCustomer;
            var current = MyCheckout.Factory(CustomerContext.CustomerId);

            if (_typeCalculationVariants == CalculationVariants.PickPoint)
            {
                var previousSelectedPointId = CommonHelper.GetCookieString(GeoModeConfig.PreviousShippingIdCookieName);
                if (previousSelectedPointId.IsNotEmpty())
                {
                    current.Data.PreSelectedShippingId = previousSelectedPointId;
                    
                    var pointId = CommonHelper.GetCookieString(GeoModeConfig.PreviousShippingPointIdCookieName);
                    if (pointId.IsNotEmpty())
                        current.Data.PreSelectedShippingPointId = HttpUtility.UrlDecode(pointId);  
                    
                    CommonHelper.DeleteCookie(GeoModeConfig.PreviousShippingIdCookieName);
                    CommonHelper.DeleteCookie(GeoModeConfig.PreviousShippingPointIdCookieName);
                }
            }

            if (customer.RegistredUser && customer.Contacts.Count > 0)
            {
                var contact = customer.Contacts.FirstOrDefault(x => x.IsMain) ?? 
                              customer.Contacts.FirstOrDefault();

                if (contact != null)
                {
                    current.Data.Contact = CheckoutAddress.Create(contact);
                    current.Update();
                }
            }
            else
            {
                var currentZone = IpZoneContext.CurrentZone;

                if (currentZone != null)
                {
                    current.Data.Contact = new CheckoutAddress()
                    {
                        Country = currentZone.CountryName,
                        Region = currentZone.Region,
                        City = currentZone.City,
                        District = currentZone.District,
                        Zip = currentZone.Zip
                    };
                    current.Update();
                }
            }

            var result = new GetCheckoutShippings(_typeCalculationVariants, true).Execute();

            return new GeoModeDeliveriesResponse()
            {
                Options = result.option,
                SelectedOption = result.selectShipping
            };
        }
    }
}