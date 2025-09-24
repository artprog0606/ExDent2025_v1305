using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Web.Admin.Models.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Catalog
{
    public class GetCatalogOffers
    {
        private readonly CatalogFilterModel _filter;
        private readonly FilterResult<IAdminCatalogGridProduct> _productsResult;
        private SqlPaging _paging;

        public GetCatalogOffers(CatalogFilterModel filterModel)
        {
            _filter = filterModel;
            
            // Используем FilterResult от товаров за основу, т.к.
            // он правильно выводит пагинацию в гриде,
            // далее по ProductId получаем модификации,
            // если надо фильтруем, чтобы сузить выборку 
            _productsResult = new GetCatalog(_filter, true).Execute();
        }

        public FilterResult<CatalogOfferModel> Execute()
        {
            var model = new FilterResult<CatalogOfferModel>();

            GetPaging();

            model.TotalItemsCount = _productsResult.TotalItemsCount;
            model.TotalPageCount = _productsResult.TotalPageCount;
            model.DataItems = new List<CatalogOfferModel>();

            if (model.TotalPageCount < _filter.Page && _filter.Page > 1)
                return model;
            
            model.DataItems = _paging.PageItemsList<CatalogOfferModel>();

            // для группировки в гриде нужна главная модификация, после фильтров может ее не быть, поэтому ставим
            foreach (var item in model.DataItems)
            {
                if (!item.Main
                    && model.DataItems.Count(x => x.ProductId == item.ProductId && x.Main) == 0)
                {
                    item.Main = true;
                }
            }
            
            return model;
        }

        public List<T> GetItemsIds<T>(string fieldName)
        {
            GetPaging();

            return _paging.ItemsIds<T>(fieldName);
        }

        private void GetPaging()
        {
            var productIds = 
                _productsResult.DataItems.Count > 0
                    ? _productsResult.DataItems.Select(x => x.ProductId).ToList()
                    : new List<int>() { -1 };
            
            _paging = new SqlPaging()
            {
                ItemsPerPage = 100000,
                CurrentPageIndex = 1 
            };

            _paging
                .Select(
                    "Product.ProductID",
                    "Product.Name",
                    "Product.Enabled",
                    "Product.Discount",
                    "Product.DiscountAmount",
                    "Product.DoNotApplyOtherDiscounts",
                    "Product.MainCategoryId",
                    "Offer.OfferId",
                    "Offer.ArtNo",
                    "Offer.Price",
                    "Offer.ColorID",
                    "Offer.SizeID",
                    "Offer.Amount",
                    "Color.ColorName",
                    "Size.SizeName",
                    "[Currency].Code".AsSqlField("CurrencyCode"),
                    "[Currency].CurrencyIso3",
                    "[Currency].CurrencyValue",
                    "[Currency].IsCodeBefore",
                    "PhotoName",
                    _filter.ShowMethod == ECatalogShowMethod.Normal ? "ProductCategories.SortOrder" : "-1 as SortOrder"
                )
                .From("[Catalog].[Product]")
                .Inner_Join(
                    "(select item, sort from [Settings].[ParsingBySeperator]({0},'/') ) as productIds on Product.ProductId = convert(int, productIds.item)",
                    string.Join("/", productIds)
                )
                .Inner_Join("[Catalog].[Offer] ON [Product].[ProductID] = [Offer].[ProductID]")
                .Left_Join("[Catalog].[Color] ON [Color].[ColorId] = [Offer].[ColorId]")
                .Left_Join("[Catalog].[Size] ON [Size].[SizeId] = [Offer].[SizeId]")
                .Left_Join("[Catalog].[Currency] ON [Product].[CurrencyID] = [Currency].[CurrencyID]")
                .Left_Join("[Catalog].[Photo] ON [Product].[ProductID] = [Photo].[ObjId] and Type='Product' AND [Photo].[Main] = 1");

            if (_filter.ShowMethod == ECatalogShowMethod.Normal)
            {
                _paging.Inner_Join("[Catalog].[ProductCategories] on [ProductCategories].[ProductId] = [Product].[ProductID]");
                _paging.Where("ProductCategories.CategoryID = {0}", _filter.CategoryId);
                
                _paging.Select("Category_Size.SizeNameForCategory");
                _paging.Left_Join("[Catalog].[Category_Size] ON Size.SizeId = Category_Size.SizeId and Category_Size.CategoryId = {0}", 
                                    _filter.CategoryId ?? 0);
            }
            
            Filter();
            Sorting();
        }

        private void Filter()
        {
            if (!string.IsNullOrWhiteSpace(_filter.ArtNo))
                _paging.Where("(Product.ArtNo LIKE '%'+{0}+'%' OR Offer.ArtNo LIKE '%'+{0}+'%')", _filter.ArtNo);

            if (_filter.ColorId != null)
                _paging.Where("Offer.ColorID={0}", _filter.ColorId);

            if (_filter.SizeId != null)
                    _paging.Where("Offer.SizeID={0}", _filter.SizeId);

            if (_filter.PriceFrom.HasValue || _filter.PriceTo.HasValue)
            {
                if (_filter.PriceFrom.HasValue)
                    _paging.Where("(CASE WHEN Product.Discount > 0 THEN (Price - Price*Product.Discount/100)*CurrencyValue ELSE (Price - Product.DiscountAmount)*CurrencyValue END) >= {0}", _filter.PriceFrom);
            
                if (_filter.PriceTo.HasValue)
                    _paging.Where("(CASE WHEN Product.Discount > 0 THEN (Price - Price*Product.Discount/100)*CurrencyValue ELSE (Price - Product.DiscountAmount)*CurrencyValue END) <= {0}", _filter.PriceTo);
            }
            
            if (_filter.AmountFrom != null)
                _paging.Where("Amount >= {0}", _filter.AmountFrom);
            
            if (_filter.AmountTo != null)
                _paging.Where("Amount <= {0}", _filter.AmountTo);
            
            if (!string.IsNullOrWhiteSpace(_filter.BarCode))
                _paging.Where(
                    "(Product.BarCode LIKE '%'+{0}+'%' OR Offer.BarCode LIKE '%'+{0}+'%')", 
                    _filter.BarCode);

            if (_filter.WarehouseIds != null && _filter.WarehouseIds.Count > 0)
                _paging.Where(
                    "Exists(" +
                    "Select 1 " +
                    "From [Catalog].[WarehouseStocks] " +
                    "Where [WarehouseStocks].[OfferId] = [Offer].[OfferID] " +
                    "     AND [WarehouseStocks].[WarehouseId] in ({0}) " +
                    "     AND [WarehouseStocks].[Quantity] > 0)",
                    _filter.WarehouseIds.ToArray());

            // TODO: check it
            if (_filter.ExcludeIds != null)
            {
                var excludeIds = _filter.ExcludeIds.Split(',').Select(x => x.TryParseInt()).Where(x => x != 0).ToList();
                
                if (excludeIds.Count > 0)
                    _paging.Where("Offer.OfferId not in (" + String.Join(",", excludeIds) + ")");
            }
        }

        private void Sorting()
        {
            _paging
                .OrderBy("productIds.sort")
                .OrderBy("Offer.ProductId")
                .OrderByDesc("Offer.Main");
        }
    }
}