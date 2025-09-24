using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.InplaceEditor;
using AdvantShop.Customers;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Handlers.Inplace
{
    public class InplaceOfferHandler
    {
        public object Execute(int id, string content, OfferInplaceField field)
        {
            var offer = OfferService.GetOffer(id);
            if (offer == null)
                return null;

            var productCurrency = offer.Product.Currency.Rate;
            Offer _offerMutation;

            switch (field)
            {
                case OfferInplaceField.Price:
                    var price = content.Replace("&nbsp;", "").Replace(" ", "").TryParseFloat();

                    offer.BasePrice = price / productCurrency * CurrencyService.CurrentCurrency.Rate;
                    OfferService.UpdateOffer(offer, trackChanges: true);
                    break;

                case OfferInplaceField.DiscountPercent:
                    
                    var percent = content.Replace("&nbsp;", "").Replace(" ", "").TryParseFloat();

                    offer.Product.Discount = new Discount(percent, 0);
   
                    ProductService.UpdateProductDiscount(offer.ProductId, offer.Product.Discount.Percent);

                    break;

                case OfferInplaceField.DiscountAbs:

                    var percentAbs = content.Replace("&nbsp;", "").Replace(" ", "").TryParseFloat() / productCurrency * CurrencyService.CurrentCurrency.Rate;

                    offer.Product.Discount = new Discount(percentAbs / offer.BasePrice * 100, 0);

                    ProductService.UpdateProductDiscount(offer.ProductId, offer.Product.Discount.Percent);

                    break;
                case OfferInplaceField.Amount:
                    var amount = content.TryParseFloat();
                    var offerStocks = WarehouseStocksService.GetOfferStocks(offer.OfferId);
                    if (offerStocks.Count == 1)
                    {
                        var offerStock = offerStocks.Single();
                        offerStock.Quantity = amount > 1000000 ? 1000000 : amount;
                        WarehouseStocksService.AddUpdateStocks(offerStock, trackChanges: true);
                    }
                    else if (offerStocks.Count == 0)
                    {
                        var warehouses = WarehouseService.GetList();
                        if (warehouses.Count == 1)
                            WarehouseStocksService.AddUpdateStocks(
                                new WarehouseStock
                                {
                                    OfferId = offer.OfferId,
                                    Quantity = amount > 1000000 ? 1000000 : amount,
                                    WarehouseId = warehouses.Single().Id
                                }, 
                                trackChanges: true);
                    }
                    break;

                case OfferInplaceField.ArtNo:
                    offer.ArtNo = content;
                    OfferService.UpdateOffer(offer);

                    if (offer.Product.Offers.Count == 1)
                    {
                        var p = offer.Product;
                        p.ArtNo = content;
                        p.ModifiedBy = CustomerContext.CustomerId.ToString();
                        ProductService.UpdateProduct(p, true, true);
                    }
                    break;

                case OfferInplaceField.Weight:
                    
                    var weight = content.TryParseFloat();
                    if (offer.Weight < 1)
                        weight /= 1000;
                    
                    if (!SettingsCatalog.EnableOfferWeightAndDimensions)
                    {
                        foreach (var productOffer in offer.Product.Offers)
                        {
                            productOffer.Weight = weight;
                            OfferService.UpdateOffer(productOffer, trackChanges: true);
                        }
                    }
                    else
                    {
                        offer.Weight = weight;
                        OfferService.UpdateOffer(offer, trackChanges: true);
                    }
                    break;

                case OfferInplaceField.Length:
                    _offerMutation = SettingsCatalog.EnableOfferWeightAndDimensions ? offer : (offer.Main ? offer : OfferService.GetMainOfferForExport(offer.ProductId));

                    _offerMutation.Length = content.TryParseFloat();
                    OfferService.UpdateOffer(_offerMutation, trackChanges: true);

                    break;

                case OfferInplaceField.Width:
                    _offerMutation = SettingsCatalog.EnableOfferWeightAndDimensions ? offer : (offer.Main ? offer : OfferService.GetMainOfferForExport(offer.ProductId));

                    _offerMutation.Width = content.TryParseFloat();
                    OfferService.UpdateOffer(_offerMutation, trackChanges: true);

                    break;

                case OfferInplaceField.Height:
                    _offerMutation = SettingsCatalog.EnableOfferWeightAndDimensions ? offer : (offer.Main ? offer : OfferService.GetMainOfferForExport(offer.ProductId));

                    _offerMutation.Height = content.TryParseFloat();
                    OfferService.UpdateOffer(_offerMutation, trackChanges: true);

                    break;

                default:
                    return false;
            }

            ProductService.PreCalcProductParams(offer.ProductId);
            return true;
        }
    }
}