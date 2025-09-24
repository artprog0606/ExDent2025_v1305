using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Products;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Products
{
    public sealed class GetProductPriceApi : AbstractCommandHandler<GetProductPriceResponse>
    {
        private readonly int _productId;
        private readonly GetPriceModel _model;
        
        private Product _product;
        private Offer _offer;
        private IList<CustomOption> _customOptions;

        public GetProductPriceApi(int id, GetPriceModel model)
        {
            _productId = id;
            _model = model;
        }
        
        public GetProductPriceApi(Product product, Offer offer, IList<CustomOption> customOptions, List<SelectedCustomOptionApi> options, float amount)
        {
            _product = product;
            _productId = product.ProductId;
            _offer = offer;
            _customOptions = customOptions;
            
            _model = new GetPriceModel()
            {
                OfferId = offer.OfferId,
                Options = options,
                Amount = amount
            };
        }
        
        protected override void Load()
        {
            if (_model == null)
                throw new BlException("Не указан offerId");
            
            if (_model != null && _model.Amount <= 0)
                throw new BlException("Не указано кол-во");
            
            if (_offer == null)
            {
                if (_model.OfferId == 0)
                    throw new BlException("Не указан offerId");

                _offer = OfferService.GetOffer(_model.OfferId);
                
                if (_offer == null)
                    throw new BlException("Неправильный offerId");
                
                _product = _offer.Product;
                _customOptions = CustomOptionsService.GetCustomOptionsByProductIdCached(_productId);
            }
            
            // set default required options
            if (_model.Options == null && _customOptions != null)
            {
                _model.Options = new List<SelectedCustomOptionApi>();
                
                foreach (var customOption in _customOptions.Where(x => x.IsRequired && x.SelectedOptions != null))
                {
                    _model.Options.Add(new SelectedCustomOptionApi()
                    {
                        Id = customOption.CustomOptionsId,
                        OptionItems = 
                            customOption.SelectedOptions
                                .Select(x => new SelectedCustomOptionItemApi()
                                {
                                    OptionId = x.OptionId, 
                                    Amount = x.DefaultQuantity
                                }).ToList()
                    });
                }
            }
        }

        protected override void Validate()
        {
            if (_offer == null)
                throw new BlException("Товар не найден");
        }

        protected override GetProductPriceResponse Handle()
        {
            var customer = CustomerContext.CurrentCustomer;
            
            return GetPrice(_offer, _product, _model.Options, customer);
        }
        
        private GetProductPriceResponse GetPrice(Offer offer, Product product, List<SelectedCustomOptionApi> options, Customer customer)
        {
            var offerRoundedPrice = offer.RoundedPrice;
            
            _offer.SetPriceRule(_model.Amount, customer.CustomerGroupId);
            
            var customOptionsPrice = GetOptionsPrice(options, offer.RoundedPrice, product);

            var price = (offer.RoundedPrice + customOptionsPrice).RoundPrice(CurrencyService.CurrentCurrency.Rate);
            var offerPrice = (offerRoundedPrice + customOptionsPrice).RoundPrice(CurrencyService.CurrentCurrency.Rate);

            var finalDiscount =
                offer.PriceRule == null || offer.PriceRule.ApplyDiscounts
                    ? PriceService.GetFinalDiscount(price, product.Discount, product.Currency.Rate,
                                                    customer.CustomerGroup, product.ProductId,
                                                    doNotApplyOtherDiscounts: product.DoNotApplyOtherDiscounts,
                                                    productMainCategoryId: product.CategoryId)
                    : new Discount(0, offerPrice - price);

            var oldPrice =
                offer.PriceRule == null || offer.PriceRule.ApplyDiscounts
                    ? price
                    : offerPrice;
            
            var newPrice = 
                offer.PriceRule == null || offer.PriceRule.ApplyDiscounts
                    ? PriceService.GetFinalPrice(price, finalDiscount)
                    : price;
            
            var bonusPlus = GetBonusPlus(offer, customer, newPrice, finalDiscount);
            
            return new GetProductPriceResponse()
            {
                OldPrice = finalDiscount.HasValue ? oldPrice : default,
                Price = newPrice,
                Bonuses = bonusPlus,
                Discount = new ProductDiscountApi(finalDiscount)
            };
        }

        private float GetOptionsPrice(List<SelectedCustomOptionApi> options, float price, Product product)
        {
            if (_customOptions == null)
                return 0;
            
            if (options == null || options.Count == 0)
                return 0;

            float fixedPrice = 0;
            float percentPrice = 0;

            foreach (var option in _customOptions)
            {
                var selectedOption = options.Find(x => x.Id == option.CustomOptionsId);
                if (selectedOption == null)
                    continue;
                
                selectedOption.ConvertOptionsToOptionItems();

                foreach (var item in option.Options)
                {
                    var selectedOptionItems = selectedOption.OptionItems?.Where(x => x.OptionId == item.OptionId).ToList();
                    if (selectedOptionItems == null || selectedOptionItems.Count == 0)
                        continue;

                    foreach (var selectedOptionItem in selectedOptionItems)
                    {
                        var optionAmount = selectedOptionItem.Amount ?? item.DefaultQuantity ?? 1;
                        var optionPrice = item.BasePrice * optionAmount;

                        switch (item.PriceType)
                        {
                            case OptionPriceType.Fixed:
                                fixedPrice += optionPrice;
                                break;

                            case OptionPriceType.Percent:
                                percentPrice += price * optionPrice * 0.01F;
                                break;
                        }
                    }
                }
            }

            return (fixedPrice + percentPrice);
        }

        private string GetBonusPlus(Offer offer, Customer customer, float newPrice, Discount totalDiscount)
        {
            if (!BonusSystem.IsActive || !offer.Product.AccrueBonuses || offer.RoundedPrice == 0) 
                return null;
            
            var bonusCard = BonusSystemService.GetCard(customer.Id);
            if (bonusCard != null && bonusCard.Blocked)
                return null;
                
            if (bonusCard != null)
                return PriceFormatService.RenderBonusPrice((float)bonusCard.Grade.BonusPercent, newPrice, totalDiscount, true);
                
            if (BonusSystem.BonusFirstPercent != 0)
                return PriceFormatService.RenderBonusPrice((float)BonusSystem.BonusFirstPercent, newPrice, totalDiscount, true);

            return null;
        }
    }
}