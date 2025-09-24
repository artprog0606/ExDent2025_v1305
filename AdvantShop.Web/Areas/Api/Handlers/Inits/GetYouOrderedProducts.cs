using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Catalogs;
using AdvantShop.Areas.Api.Models.Inits;
using AdvantShop.Areas.Api.Services;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Inits
{
    public class GetYouOrderedProducts : AbstractCommandHandler<GetYouOrderedProductsResponse>
    {
        private const int Count = 7;
        private readonly CatalogApiService _catalogApiService;

        public GetYouOrderedProducts()
        {
            _catalogApiService = new CatalogApiService();
        }

        protected override GetYouOrderedProductsResponse Handle()
        {
            var products = GetProducts();

            var items = new List<CatalogProductItem>();
            
            foreach (var product in products)
            {
                var productItem = new ProductItem(product, 0, null);
                
                items.Add(new CatalogProductItem(productItem, false, ShoppingCartService.CurrentWishlist));
            }
            
            _catalogApiService.SetMarkers(items);

            return new GetYouOrderedProductsResponse(items);
        }

        private List<ProductModel> GetProducts()
        {
            try
            {
                var query =
                    ";with cte (ProductId) as (" +
                    "Select top(@Count) oi.ProductId " +

                    "From [Order].[OrderItems] oi " +
                    "Inner Join [Order].[OrderCustomer] oc on oi.OrderId = oc.OrderId " +
                    "Inner join [Catalog].[Product] p on p.ProductId = oi.ProductId " +
                    "Inner join [Catalog].[ProductExt] on [ProductExt].ProductId = p.ProductId " +

                    "Where oc.CustomerId = @CustomerId and oi.ProductId is not null " +
                    "and p.Enabled = 1 and p.Hidden = 0 and CategoryEnabled = 1  " +
                    (SettingsCatalog.ShowOnlyAvalible ? " AND (MaxAvailable>0 OR p.[AllowPreOrder] = 1) " : "") +
                    "Group by oi.ProductId " +
                    "Order by Sum(oi.Amount) desc)" +

                    "Select [Product].[ProductID], Product.ArtNo, Product.Name, Recomended as Recomend, Bestseller, New, OnSale as Sales, Discount, DiscountAmount, " +
                    "Product.Enabled, Product.UrlPath, AllowPreOrder, Ratio, ManualRatio, Offer.OfferID, MaxAvailable AS Amount, MinAmount, MaxAmount, Offer.Amount AS AmountOffer, " +
                    "CountPhoto, Photo.PhotoId, PhotoName, PhotoNameSize1, PhotoNameSize2, Photo.Description as PhotoDescription, Offer.ColorID, Product.DateAdded, " +
                    "null as AdditionalPhoto, Product.DoNotApplyOtherDiscounts, Product.MainCategoryId, Product.Multiplicity, Product.BriefDescription, " +
                    (SettingsCatalog.ComplexFilter
                        ? "Colors, NotSamePrices as MultiPrices, MinPrice as BasePrice, "
                        : "null as Colors, 0 as MultiPrices, Price as BasePrice, ") +
                    "CurrencyValue, Comments, Gifts " +
                    "From cte inner join [Catalog].[Product] on cte.ProductId = [Product].ProductId " +
                    "Left Join [Catalog].[ProductExt]  On [Product].[ProductID] = [ProductExt].[ProductID]  " +
                    "Inner Join Catalog.Currency On Currency.CurrencyID = Product.CurrencyID " +
                    "Left Join [Catalog].[Photo] On [Photo].[PhotoId] = [ProductExt].[PhotoId] And Type=@Type " +
                    "Left Join [Catalog].[Offer] On [ProductExt].[OfferID] = [Offer].[OfferID] ";

                return CacheManager.Get(CacheNames.MainPageProductsCacheName("YouOrdered", Count, query),
                    () =>
                        SQLDataAccess
                            .Query<ProductModel>(query,
                                new
                                {
                                    Count, CustomerId = CustomerContext.CustomerId, Type = PhotoType.Product.ToString()
                                })
                            .ToList());
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return new List<ProductModel>();
        }
    }
}