using System.Linq;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.Models.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Leads
{
    public class GetLeadShippings
    {
        private readonly int _leadId;
        private readonly string _country;
        private readonly string _region;
        private readonly string _district;
        private readonly string _city;
        private readonly string _zip;
        private string _street;
        private string _house;
        private string _structure;
        private string _apartment;
        private string _entrance;
        private string _floor;
        private readonly BaseShippingOption _shipping;
        private readonly bool _getAll;

        public GetLeadShippings(int leadId, string country, string city, string district, string region, string zip,
                                string street, string house, string structure, string apartment, string entrance, string floor,
                                BaseShippingOption shipping = null, bool getAll = true)
        {
            _leadId = leadId;
            _country = country;
            _region = region;
            _district = district;
            _city = city;
            _zip = zip;
            _street = street;
            _house = house;
            _structure = structure;
            _apartment = apartment;
            _entrance = entrance;
            _floor = floor;
            _shipping = shipping;
            _getAll = getAll;
        }

        public OrderShippingsModel Execute()
        {
            var lead = LeadService.GetLead(_leadId);
            if (lead == null)
                return null;

            var model = new OrderShippingsModel();

            if (lead.LeadItems == null || lead.LeadItems.Count == 0)
                return model;

            var manager = new ShippingManager(
                config => config
                         .WithCountry(_country ?? "")
                         .WithRegion(_region ?? "")
                         .WithDistrict(_district ?? "")
                         .WithCity(_city ?? "")
                         .WithStreet(_street ?? "")
                         .WithHouse(_house ?? "")
                         .WithStructure(_structure ?? "")
                         .WithApartment(_apartment ?? "")
                         .WithEntrance(_entrance ?? "")
                         .WithFloor(_floor ?? "")
                         .WithZip(_zip ?? "")
                         .WithCurrency(lead.LeadCurrency ?? CurrencyService.CurrentCurrency)
                         .WithShippingOption(_shipping)
                         .WithPreOrderItems(
                              lead.LeadItems
                                  .Select(x => new PreOrderItem(x))
                                  .ToList())
                         .WithItemsTotalPriceWithDiscounts(lead.Sum - lead.ShippingCost)
                         .WithItemsTotalPriceWithDiscountsWithoutBonuses(lead.Sum - lead.ShippingCost)
                         .FromAdminArea()
                         .Build());
#if !DEBUG
            manager.TimeLimitMilliseconds = 20_000; // 20 seconds
#endif
            if (!_getAll)
                manager.PreferShippingOptionFromParameters();
            model.Shippings = manager.GetOptions();

            if (model.Shippings != null)
                model.Shippings.ForEach(item => item.ManualRate = item.FinalRate);

            model.CustomShipping = new BaseShippingOption()
            {
                Name = "",
                Rate = 0,
                IsCustom = true
            };
            if (lead.ShippingMethodId == 0)
            {
                model.CustomShipping.Name = lead.ShippingName;
                model.CustomShipping.Rate = lead.ShippingCost;
                model.SelectShipping = model.CustomShipping;
            } else
            {
                model.SelectShipping = model.Shippings.FirstOrDefault(item => item.MethodId == lead.ShippingMethodId);
                if (model.SelectShipping != null)
                {
                    var orderPickPoint = !string.IsNullOrEmpty(lead.ShippingPickPoint)
                        ? JsonConvert.DeserializeObject<OrderPickPoint>(lead.ShippingPickPoint)
                        : null;
                    if (orderPickPoint != null)
                        model.SelectShipping.UpdateFromOrderPickPoint(orderPickPoint);
                }
            }
            
            return model;
        }
    }
}
