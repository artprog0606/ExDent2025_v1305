using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Deliveries;
using AdvantShop.Areas.Api.Models.Products;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Deliveries
{
    public sealed class CalculateDeliveries : AbstractCommandHandler<CalculateDeliveriesResponse>
    {
        private readonly CalculateDeliveriesModel _model;
        private ShoppingCart _cart = new ShoppingCart();

        public CalculateDeliveries(CalculateDeliveriesModel model)
        {
            _model = model;
        }

        protected override void Validate()
        {
            if (_model.Address == null)
                throw new BlException("Адрес не найден");

            if (string.IsNullOrWhiteSpace(_model.Address.Country) && string.IsNullOrWhiteSpace(_model.Address.Region) &&
                string.IsNullOrWhiteSpace(_model.Address.District) && string.IsNullOrWhiteSpace(_model.Address.City) &&
                string.IsNullOrWhiteSpace(_model.Address.ZipCode))
            {
                throw new BlException("Укажите адрес");
            }

            if (_model.Products == null || _model.Products.Count == 0)
                throw new BlException("Товары не найдены");

            foreach (var product in _model.Products)
            {
                var offer = OfferService.GetOffer(product.OfferId);
                if (offer == null)
                    throw new BlException($"Товар {product.OfferId} не найден");

                _cart.Add(
                    new ShoppingCartItem()
                    {
                        OfferId = offer.OfferId,
                        Amount = product.Amount,
                        AttributesXml = GetCustomOptionsXml(product.Options, offer),
                        ShoppingCartType = ShoppingCartType.ShoppingCart
                    }
                );
            }
        }

        protected override CalculateDeliveriesResponse Handle()
        {
            var shippingManager = new ShippingManager(GetShippingCalculationParameters(_model.Address));

            var deliveries = shippingManager.GetOptions();

            if (_model.InProductDetails)
                deliveries = deliveries.Take(SettingsDesign.ShippingsMethodsInDetailsCount).ToList();
            
            return new CalculateDeliveriesResponse(deliveries.Select(x => new DeliveryItem(x)));
        }
        
        private ShippingCalculationParameters GetShippingCalculationParameters(CalculateDeliveriesAddress address)
        {
            var config =
                ShippingCalculationConfigurator
                    .Configure()
                    .WithCountry(address.Country)
                    .WithRegion(address.Region)
                    .WithDistrict(address.District)
                    .WithCity(address.City)
                    .WithZip(address.ZipCode)
                    .WithCurrency(CurrencyService.CurrentCurrency)
                    .ByShoppingCart(_cart);

            if (_model.InProductDetails)
                config.ShowOnlyInDetails();
            
            return config.Build();
        }

        private string GetCustomOptionsXml(List<SelectedCustomOptionApi> selectedCustomOptions, Offer offer)
        {
            if (selectedCustomOptions == null)
                return null;
            
            var customOptions = CustomOptionsService.GetCustomOptionsByProductIdCached(offer.ProductId);
            if (customOptions == null || selectedCustomOptions.Count == 0)
                return null;
            
            var evOptions = new List<EvaluatedCustomOptions>();
            
            foreach (var customOption in customOptions)
            {
                var selectedOption = selectedCustomOptions.Find(x => x.Id == customOption.CustomOptionsId);
                if (selectedOption == null)
                    continue;
                
                selectedOption.ConvertOptionsToOptionItems();

                foreach (var item in customOption.Options)
                {
                    var selectedOptionItems = selectedOption.OptionItems?.Where(x => x.OptionId == item.OptionId).ToList();
                    if (selectedOptionItems == null || selectedOptionItems.Count == 0)
                        continue;

                    foreach (var selectedOptionItem in selectedOptionItems)
                    {
                        evOptions.Add(new EvaluatedCustomOptions()
                        {
                            CustomOptionId = item.CustomOptionsId,
                            OptionId = item.OptionId,
                            CustomOptionTitle = item.Title,
                            OptionTitle = item.Title,
                            OptionPriceBc = item.BasePrice,
                            OptionPriceType = item.PriceType,
                            OptionAmount = selectedOptionItem.Amount ?? item.DefaultQuantity
                        });
                    }
                }
            }

            return CustomOptionsService.SerializeToXml(evOptions);
        }
    }
}