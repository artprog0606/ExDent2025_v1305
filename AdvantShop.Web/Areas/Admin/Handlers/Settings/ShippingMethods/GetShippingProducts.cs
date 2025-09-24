using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.FullSearch;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Catalog;
using AdvantShop.Web.Admin.Models.Settings.ShippingMethods;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Settings.ShippingMethods
{
    public class GetShippingProducts
    {
        private readonly ShippingProductsFilterModel _filterModel;
        private SqlPaging _paging;
    
        public GetShippingProducts(ShippingProductsFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<ShippingLinkProductModel> Execute()
        {
            var model = new FilterResult<ShippingLinkProductModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = string.Format("Найдено товаров: {0}", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<ShippingLinkProductModel>();

            return model;
        }

        public List<T> GetItemsIds<T>(string fieldName)
        {
            GetPaging();

            return _paging.ItemsIds<T>(fieldName);
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "Product.ProductID",
                "Product.Name",
                // "[Settings].[ArtNoToString](Product.ProductID)".AsSqlField("ArtNo"),
                "Product.ArtNo".AsSqlField("ProductArtNo"),
                "PhotoName",
                //"(Select Count(ProductID) From Catalog.ProductCategories Where ProductID=Product.ProductID)".AsSqlField("ProductCategoriesCount"),
                "Price",
                "(Select sum (Amount) from catalog.Offer where Offer.ProductID=Product.productID)".AsSqlField("Amount"),
                "(Select count (offerid) from catalog.Offer where Offer.ProductID=Product.productID)".AsSqlField("OffersCount"),
                "Product.Enabled",
                "[Currency].Code".AsSqlField("CurrencyCode"),
                "[Currency].CurrencyIso3",
                "[Currency].CurrencyValue",
                "[Currency].IsCodeBefore",
                (_filterModel.ShowMethod == ECatalogShowMethod.Normal ? "ProductCategories.SortOrder" : "-1 as SortOrder"),
                // "Offer.ColorID",
                // "Offer.SizeID",

                "Brand.BrandId",
                "Brand.BrandName".AsSqlField("BrandName"),
                (_filterModel.ShippingId.ToString()).AsSqlField("ShippingId"),
                "(Select ISNULL([LinkShippingProduct].MethodId, 0))".AsSqlField("LinkShipping")
                );

            _paging.From("[Catalog].[Product]");

            _paging.Left_Join("[Catalog].[Offer] ON [Product].[ProductID] = [Offer].[ProductID] and [Offer].[Main] = 1");
            _paging.Left_Join("[Catalog].[Photo] ON [Product].[ProductID] = [Photo].[ObjId]  and Type='Product' AND [Photo].[Main] = 1");
            _paging.Left_Join("[Catalog].[Currency] ON [Product].[CurrencyID] = [Currency].[CurrencyID]");
            _paging.Left_Join("[Catalog].[Brand] ON [Product].[BrandId] = [Brand].[BrandId]");
            if (_filterModel.TypeRef == EnTypeRef.Exclude)
            {
                _paging.Left_Join(
                    "[Order].[ShippingProductExcluded] as [LinkShippingProduct] ON [Product].[ProductId] = [LinkShippingProduct].[ProductId] and [LinkShippingProduct].[MethodId]=" +
                    _filterModel.ShippingId);
            }


            switch (_filterModel.ShowMethod)
            {
                case ECatalogShowMethod.AllProducts:
                    //_paging.Left_Join("[Catalog].[ProductCategories] ON [Catalog].[ProductCategories].[ProductID] = [Product].[ProductID]");                    
                    break;

                case ECatalogShowMethod.OnlyWithoutCategories:
                    _paging.Inner_Join("(Select ProductId from Catalog.Product where ProductId not in (Select ProductId from Catalog.ProductCategories)) as tmp on tmp.ProductId=[Product].[ProductID]");
                    break;

                case ECatalogShowMethod.Normal:
                    _paging.Inner_Join("[Catalog].[ProductCategories] on [ProductCategories].[ProductId] = [Product].[ProductID]");
                    break;
            }

            if (_filterModel.ShowMethod == ECatalogShowMethod.Normal)
            {
                _paging.Where("ProductCategories.CategoryID = {0}", _filterModel.CategoryId);
            }

            Filter();
            Sorting();
        }

        private void Filter()
        {
            if (!string.IsNullOrEmpty(_filterModel.Search))
            {
                var res = ProductSearcherAdmin.Search(_filterModel.Search);
                var productIds = res.SearchResultItems.Aggregate("", (current, item) => current + (item.Id + "/"));
                _paging.Inner_Join(
                    "(select item, sort from [Settings].[ParsingBySeperator]({0},'/')) as dtt on Product.ProductId=convert(int, dtt.item)",
                    productIds);

                if (_filterModel.SortingType == FilterSortingType.None)
                {
                    _paging.OrderBy("dtt.sort");
                }
            }

            if (!string.IsNullOrWhiteSpace(_filterModel.ArtNo))
            {
                _paging.Where("(Product.ArtNo LIKE '%'+{0}+'%' OR [Settings].[ArtNoToString](Product.ProductID) LIKE  '%'+{0}+'%')", _filterModel.ArtNo);
            }

            if (!string.IsNullOrWhiteSpace(_filterModel.Name))
            {
                _paging.Where("Product.Name LIKE '%'+{0}+'%'", _filterModel.Name);
            }


            if (_filterModel.ColorId != null)
            {
                var color = ColorService.GetColor(_filterModel.ColorId);
                if (color != null)
                    _paging.Where("EXISTS (SELECT COUNT([Offer].OfferID) FROM [Catalog].[Offer] WHERE [Offer].ProductID = Product.ProductID AND [Offer].ColorID={0})", _filterModel.ColorId);
            }

            if (_filterModel.SizeId != null)
            {
                var size = SizeService.GetSize(_filterModel.SizeId);
                if (size != null)
                    _paging.Where("EXISTS (SELECT COUNT([Offer].OfferID) FROM [Catalog].[Offer] WHERE [Offer].ProductID = Product.ProductID AND [Offer].SizeID={0})", _filterModel.SizeId);
            }

            if (_filterModel.PriceFrom.HasValue)
            {
                _paging.Where("Price >= {0}", _filterModel.PriceFrom.Value);
            }

            if (_filterModel.PriceTo.HasValue)
            {
                _paging.Where("Price <= {0}", _filterModel.PriceTo.Value);
            }

            if (_filterModel.AmountFrom != null)
            {
                _paging.Where("((Select sum (Amount) from catalog.Offer where Offer.ProductID=Product.productID) >= {0})", _filterModel.AmountFrom.Value);
            }

            if (_filterModel.AmountTo != null)
            {
                _paging.Where("((Select sum (Amount) from catalog.Offer where Offer.ProductID=Product.productID) <= {0})", _filterModel.AmountTo.Value);
            }


            if (_filterModel.SortingFrom != null && _filterModel.SortingTo != null && _filterModel.ShowMethod == ECatalogShowMethod.Normal)
            {
                _paging.Where("(ProductCategories.SortOrder >= {0} and ProductCategories.SortOrder <= {1})", _filterModel.SortingFrom, _filterModel.SortingTo);
            }

            if (_filterModel.HasPhoto != null)
            {
                _paging.Where(string.Format("PhotoName is {0} null", _filterModel.HasPhoto.Value ? "not" : ""));
            }

            if (_filterModel.BrandId != null)
            {
                _paging.Where("Brand.BrandId = {0}", _filterModel.BrandId.Value);
            }

            if (_filterModel.Enabled != null)
            {
                _paging.Where("Product.Enabled = {0}", _filterModel.Enabled.Value ? "1" : "0");
            }

            if (_filterModel.LinkShipping != null)
            {
                _paging.Where(string.Format("[LinkShippingProduct].MethodId is {0} null", _filterModel.LinkShipping.Value ? "not" : ""));
            }


            if (_filterModel.ExcludeIds != null)
            {
                var exludeIds = _filterModel.ExcludeIds.Split(',').Select(x => x.TryParseInt()).Where(x => x != 0).ToList();
                if (exludeIds.Count > 0)
                {
                    _paging.Where("Product.ProductId not in (" + String.Join(",", exludeIds) + ")");
                }
            }
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy(
                    _filterModel.ShowMethod != ECatalogShowMethod.Normal
                        ? "ProductArtNo"
                        : "ProductCategories.SortOrder");
                return;
            }

            var sorting = _filterModel.Sorting.ToLower();
            var sortingForApply = sorting == "pricestring" ? "price" : sorting;

            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sortingForApply);
            if (field != null)
            {
                if (_filterModel.SortingType == FilterSortingType.Asc)
                {
                    _paging.OrderBy(sortingForApply);
                }
                else
                {
                    _paging.OrderByDesc(sortingForApply);
                }
            }
        }  
    }
}