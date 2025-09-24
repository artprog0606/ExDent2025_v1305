using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.FullSearch;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using AdvantShop.Saas;
using AdvantShop.Trial;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class AddProduct : AbstractCommandHandler<int>
    {
        private string _name;
        private int _categoryId;

        public AddProduct(string name, int categoryId)
        {
            _name = name ?? string.Empty;
            _categoryId = categoryId;
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(_name))
            {
                throw new BlException("Укажите название товара");
            }

            _name = _name.Trim();

            if (SaasDataService.IsSaasEnabled && ProductService.GetProductsCount("[Enabled] = 1") >= SaasDataService.CurrentSaasData.ProductsCount)
            {
                throw new BlException(T("Admin.Product.AddProductHandler.Error.ExceedingSaasValue"));
            }
        }

        protected override int Handle()
        {
            var productId = 0;
            
            var url = StringHelper.TransformUrl(StringHelper.Translit(_name));

            var product = new Product()
            {
                Name = _name,
                ArtNo = null,
                UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Product, string.IsNullOrEmpty(url) ? _name : url),
                Enabled = true,
                CurrencyID = CurrencyService.CurrentCurrency.CurrencyId,
                Multiplicity = 1,
                Offers = new List<Offer>
                    {
                        new Offer()
                        {
                            ArtNo = null,
                            // Amount = 1,
                            BasePrice = 0,
                            Main = true,
                        }
                    },
                Meta = null,
                ModifiedBy = CustomerContext.CustomerId.ToString()
            };

            productId = ProductService.AddProduct(product, true);

            if (productId != 0)
            {
                var warehouses = WarehouseService.GetList();
                // поддержание старого поведения, когда не было складов
                if (warehouses.Count == 1)
                {
                    foreach (var productOffer in OfferService.GetProductOffers(productId))
                    {
                        WarehouseStocksService.AddUpdateStocks(new WarehouseStock
                        {
                            OfferId = productOffer.OfferId,
                            WarehouseId = warehouses[0].Id,
                            Quantity = 1,
                        });
                    }
                }
            }

            if (productId != 0 && _categoryId != 0 && _categoryId != CategoryService.DefaultNonCategoryId)
            {
                ProductService.AddProductLink(productId, _categoryId, 0, true);
                ProductService.SetProductHierarchicallyEnabled(product.ProductId);
                CategoryService.CalculateHasProductsForWarehousesInProductCategories(productId);

                var p = ProductService.GetProduct(productId);
                if (p != null)
                    ProductWriter.AddUpdate(p);
            }

            TrialService.TrackEvent(TrialEvents.AddProduct, string.Empty);

            return productId;
        }
    }
}
