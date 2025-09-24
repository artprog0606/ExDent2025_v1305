using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Handlers.ProductDetails;
using AdvantShop.Payment;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.App.Landing.Extensions;
using AdvantShop.CMS;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.Models.ProductDetails;
using AdvantShop.Repository.Currencies;
using AdvantShop.ViewModel.ProductDetails;
using AdvantShop.Web.Infrastructure.Filters;
using System.Collections.Generic;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class ProductExtController : BaseClientProductController
    {
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetShippings(int offerId, float amount, string customOptions, string zip)
        {
            var model = new GetShippingsHandler(offerId, amount, customOptions, zip).Get();
            return Json(model);
        }

        public JsonResult GetOffers(int productId, int? colorId, int? sizeId)
        {
            var product = ProductService.GetProduct(productId);
            if (product == null)
                return Json(null);

            var offers = product.Offers;
            if (offers == null || offers.Count == 0)
                return Json(null);

            var warehouseIds = WarehouseContext.GetAvailableWarehouseIds();
            var warehouseId = warehouseIds?.FirstOrDefault();
            if (warehouseIds != null)
                offers.SetAmountByStocksAndWarehouses(warehouseIds);
            
            var offerSelected = OfferService.GetMainOffer(offers, product.AllowPreOrder, colorId, sizeId, warehouseId);
            
            var result = new GetOffersModel(product, offers, offerSelected.OfferId, CustomerContext.CurrentCustomer);
            
            return Json(result);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult GetOfferPrice(int offerId, string attributesXml, int? lpBlockId, float amount)
        {
            var offer = OfferService.GetOffer(offerId);
            if (offer == null)
                return Json(new {PriceString = "", PriceNumber = 0F, Bonuses = ""});

            var warehouseIds = WarehouseContext.GetAvailableWarehouseIds();
            if (warehouseIds != null)
                offer.SetAmountByStocksAndWarehouses(warehouseIds);

            Discount customDiscount = null;

            if (lpBlockId != null)
            {
                var block = new LpBlockService().Get(lpBlockId.Value);
                var button = block?.TryGetSetting<LpButton>("button");

                if (button != null && button.Discount != null)
                    customDiscount = button.Discount;
            }
            
            var customer = CustomerContext.CurrentCustomer;
            
            var (oldPrice, finalPrice, finalDiscount, preparedPrice) =
                offer.GetOfferPricesWithPriceRule(amount, attributesXml, customer, customDiscount);

            var bonusPrice = string.Empty;

            if (BonusSystem.IsActive && offer.RoundedPrice > 0 && offer.Product.AccrueBonuses)
            {
                var bonusCard = BonusSystemService.GetCard(customer.Id);
                if (bonusCard != null && bonusCard.Blocked)
                {
                    bonusPrice = null;
                }
                else if (bonusCard != null)
                {
                    bonusPrice = PriceFormatService.RenderBonusPrice((float)bonusCard.Grade.BonusPercent, finalPrice, finalDiscount);
                }
                else if (BonusSystem.BonusFirstPercent != 0)
                {
                    bonusPrice = PriceFormatService.RenderBonusPrice((float)BonusSystem.BonusFirstPercent, finalPrice, finalDiscount);
                }
            }

            var amountByMultiplicity = offer.GetAmountByMultiplicity(offer.Product.Multiplicity);
            var minimumOrderPrice = CustomerGroupService.GetMinimumOrderPrice();

            var allowBuyOutOfStockProducts = offer.Product.AllowBuyOutOfStockProducts();
            
            var isAvailableForPurchase = 
                offer.IsAvailableForPurchase(amountByMultiplicity, amount, finalPrice, finalDiscount, allowBuyOutOfStockProducts);
            
            var isAvailableForPurchaseOnBuyOneClick =
                offer.IsAvailableForPurchaseOnBuyOneClick(isAvailableForPurchase, finalPrice, minimumOrderPrice);

            var amountPrice = finalPrice * amount;
            
            return Json(new
            {
                PriceString = preparedPrice,
                PriceNumber = finalPrice,
                PriceOldNumber = oldPrice,
                Bonuses = bonusPrice,
                
                AllowBuyOutOfStockProducts = allowBuyOutOfStockProducts,
                IsAvailableForPurchase = isAvailableForPurchase,
                IsAvailableForPurchaseOnBuyOneClick = isAvailableForPurchaseOnBuyOneClick,
                AmountPrice = PriceService.SimpleRoundPrice(amountPrice),
                AmountPriceString = PriceService.SimpleRoundPrice(amountPrice, CurrencyService.CurrentCurrency).FormatPrice()
            });
        }

        public JsonResult GetFirstPaymentPrice(float price, float discount, float discountAmount)
        {
            var finalPrice = PriceService.GetFinalPrice(price, new Discount(discount, discountAmount));
            foreach (var creditPayment in PaymentService.GetCreditPaymentMethods())
            {
                if (!creditPayment.ShowCreditButtonInProductCard)
                    continue;
                    
                var paymentMethod = creditPayment as PaymentMethod;
                var finalPriceInPaymentCurrency = 
                    finalPrice.ConvertCurrency(CurrencyService.CurrentCurrency,
                                        paymentMethod.PaymentCurrency ?? CurrencyService.CurrentCurrency);
                    
                if (finalPriceInPaymentCurrency < creditPayment.MinimumPrice || 
                    finalPriceInPaymentCurrency > creditPayment.MaximumPrice)
                    continue;

                if (creditPayment.TypePresentationOfCreditInformation
                    == EnTypePresentationOfCreditInformation.FirstPayment)
                {
                    var firstPayment = creditPayment.GetFirstPayment(finalPriceInPaymentCurrency);
                    if (firstPayment is null) 
                        continue;
                    
                    var firstPaymentInCurrentCurrency =
                        firstPayment.Value
                                    .ConvertCurrency(paymentMethod.PaymentCurrency ?? CurrencyService.CurrentCurrency,
                                         CurrencyService.CurrentCurrency)
                                    .RoundPrice();
                    var result = firstPaymentInCurrentCurrency > 0
                        ? firstPaymentInCurrentCurrency.FormatPrice(true, false) + "*"
                        : T("Product.WithoutFirstPayment");

                    return Json(result);
                }
                
                if (creditPayment.TypePresentationOfCreditInformation
                    == EnTypePresentationOfCreditInformation.AmountAndNumberOfPayments)
                {
                    var amountAndNumberOfPayments = creditPayment.GetAmountAndNumberOfPayments(finalPriceInPaymentCurrency);
                    if (amountAndNumberOfPayments.IsDefault())
                        continue;
                        
                    var amountPaymentInCurrentCurrency =
                        amountAndNumberOfPayments.AmountPyament
                                                 .ConvertCurrency(
                                                      paymentMethod.PaymentCurrency ?? CurrencyService.CurrentCurrency,
                                                      CurrencyService.CurrentCurrency)
                                                 .RoundPrice();

                    var result = 
                        string.Format("{0} x {1}*", 
                            amountPaymentInCurrentCurrency.FormatPrice(true, false), 
                            amountAndNumberOfPayments.NumberOfPayments);

                    return Json(result);
                }
            }

            return null;
        }

        public JsonResult GetVideos(int productId)
        {
            return Json(ProductVideoService.GetProductVideos(productId));
        }

        public JsonResult GetPhotos(int productId)
        {
            var photos = PhotoService.GetPhotos<ProductPhoto>(productId, PhotoType.Product).Select(photo => new
            {
                PathXSmall = photo.ImageSrcXSmall(),
                PathSmall = photo.ImageSrcSmall(),
                PathMiddle = photo.ImageSrcMiddle(),
                PathBig = photo.ImageSrcBig(),
                photo.ColorID,
                photo.PhotoId,
                photo.Description,
                photo.Main,
                SettingsPictureSize.XSmallProductImageHeight,
                SettingsPictureSize.XSmallProductImageWidth,
                SettingsPictureSize.SmallProductImageHeight,
                SettingsPictureSize.SmallProductImageWidth,
                SettingsPictureSize.MiddleProductImageWidth,
                SettingsPictureSize.MiddleProductImageHeight,
                SettingsPictureSize.BigProductImageWidth,
                SettingsPictureSize.BigProductImageHeight,
                photo.Alt,
                photo.Title
            }).ToList();

            return Json(photos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomOptions(int productId)
        {
            var customOptions = 
                CustomOptionsService.GetCustomOptionsByProductId(productId)
                    .Select(x => new CustomOptionModel(x))
                    .ToList();
            
            return Json(customOptions);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CustomOptions(int productId, List<OptionItem> selectedOptions)
        {
            var handler = new GetProductCustomOptionsHandler(productId, selectedOptions, preSelect: false);
            return Json(new
            {
                xml = HttpUtility.UrlEncode(handler.GetXml()),
                jsonHash = HttpUtility.UrlEncode(handler.GetJsonHash())
            });
        }

        public JsonResult AddRating(int objid, int rating)
        {
            float newRating = 0;

            if (objid != 0 && rating != 0)
                newRating = RatingService.Vote(objid, rating);

            return Json(newRating);
        }

        [HttpGet]
        public JsonResult GetPropertiesNames(string q)
        {
            return Json(PropertyService.GetPropertiesByName(q).ToList());
        }

        [HttpGet]
        public JsonResult GetPropertiesValues(string q, int productId, int propertyId = 0)
        {
            return Json(PropertyService.GetPropertyValuesByNameAndProductId(q, productId, propertyId).ToList());
        }

        [HttpGet]
        public JsonResult GetReviewsCount(int productId)
        {
            var reviewsCount = ReviewService.GetReviewsCount(productId, EntityType.Product, SettingsCatalog.ModerateReviews, true);
            return Json(new { reviewsCount });
        }
        
        [HttpGet]
        public JsonResult GetPriceAmountList(int productId, int offerId)
        {
            return ProcessJsonResult(new GetPriceAmountList(productId, offerId));
        }

        public JsonResult GetOfferStocks(int offerId)
        {
            return ProcessJsonResult(new GetOfferStocks(offerId){ShowOnlyAvalible = SettingsCatalog.ShowOnlyAvailableWarehousesInProduct});
        }
    }
}