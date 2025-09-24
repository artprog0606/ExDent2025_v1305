using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Web.Admin.Models.Catalog;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Core.Services.FullSearch;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Web.Admin.Handlers.Catalog
{
    public class GetCatalog
    {
        private readonly CatalogFilterModel _filter;
        
        /// <summary>
        /// Для модального окна выбора товара?
        /// Для мод. окна выводятся товары с рассчитанными ценами. Для обычного грида - цена товара.
        /// </summary>
        private readonly bool _isProductSelector;
        
        private readonly bool _onlyProductId;
        private SqlPaging _paging;

        public GetCatalog(CatalogFilterModel filterModel)
        {
            _filter = filterModel;
            _isProductSelector = _filter.IsProductSelector != null && _filter.IsProductSelector.Value;
        }
        
        public GetCatalog(CatalogFilterModel filterModel, bool onlyProductId) : this(filterModel)
        {
            _onlyProductId = onlyProductId;
        }

        public FilterResult<IAdminCatalogGridProduct> Execute()
        {
            var model = new FilterResult<IAdminCatalogGridProduct>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = $"{LocalizationService.GetResource("Admin.Catalog.ProductsFound")}: {model.TotalItemsCount}{(_paging.LimitReached ? "+" : "")}";

            if (model.TotalPageCount < _filter.Page && _filter.Page > 1)
            {
                return model;
            }

            model.DataItems =
                !_isProductSelector
                    ? _paging.PageItemsList<CatalogProductModel>().Select(x => (IAdminCatalogGridProduct)x).ToList()
                    : _paging.PageItemsList<CatalogProductModelBySelector>().Select(x => (IAdminCatalogGridProduct)x).ToList();

            return model;
        }

        public List<T> GetItemsIds<T>(string fieldName)
        {
            GetPaging();

            return _paging.ItemsIds<T>(fieldName);
        }

        private void GetPaging()
        {
            _paging = new SqlPaging("AdminCatalogPaging", SettingsAdmin.CatalogGridWithoutLimits ? default(int?) : 10000)
            {
                ItemsPerPage = _filter.ItemsPerPage,
                CurrentPageIndex = _filter.Page
            };
            
            var countWarehouse = WarehouseService.GetList().Count;

            if (_onlyProductId)
            {
                _paging.Select("Product.ProductID");
            }
            else
            {
                _paging
                    .Select(
                        "Product.ProductID",
                        "Product.Name",
                        "Product.ArtNo".AsSqlField("ProductArtNo"),
                        "PhotoName",
                        "Price",
                        "(Select sum (Amount) from catalog.Offer where Offer.ProductID=Product.productID)".AsSqlField("Amount"),
                        "(Select count (offerid) from catalog.Offer where Offer.ProductID=Product.productID)".AsSqlField("OffersCount"),
                        "(SELECT COUNT (DISTINCT ws.[WarehouseId]) FROM [Catalog].[Offer] LEFT JOIN [Catalog].[WarehouseStocks] AS ws ON ws.OfferId = Offer.OfferID WHERE Offer.ProductID=Product.productID)".AsSqlField("UsedWarehouses"),
                        countWarehouse.ToString().AsSqlField("CountWarehouses"),
                        "Enabled",
                        "[Currency].Code".AsSqlField("CurrencyCode"),
                        "[Currency].CurrencyIso3",
                        "[Currency].CurrencyValue",
                        "[Currency].IsCodeBefore",
                        (_filter.ShowMethod == ECatalogShowMethod.Normal
                            ? "ProductCategories.SortOrder"
                            : "-1 as SortOrder")
                    );
            }
            
            _paging
                .From("[Catalog].[Product]")
                .Left_Join("[Catalog].[Offer] ON [Product].[ProductID] = [Offer].[ProductID] and [Offer].[Main] = 1")
                .Left_Join("[Catalog].[Photo] ON [Product].[ProductID] = [Photo].[ObjId] and Type='Product' AND [Photo].[Main] = 1")
                .Left_Join("[Catalog].[Currency] ON [Product].[CurrencyID] = [Currency].[CurrencyID]");

            if (_isProductSelector)
            {
                _paging
                    .Select(
                        "Product.Discount",
                        "Product.DiscountAmount",
                        "Product.DoNotApplyOtherDiscounts",
                        "Product.MainCategoryId")
                    .Left_Join("[Catalog].[ProductExt] ON [ProductExt].[ProductId] = [Product].[ProductId]");
            }

            switch (_filter.ShowMethod)
            {
                case ECatalogShowMethod.AllProducts:
                    break;

                case ECatalogShowMethod.OnlyWithoutCategories:
                    _paging.Inner_Join("(Select ProductId from Catalog.Product where ProductId not in (Select ProductId from Catalog.ProductCategories)) as tmp on tmp.ProductId=[Product].[ProductID]");
                    break;

                case ECatalogShowMethod.Normal:
                    _paging.Inner_Join("[Catalog].[ProductCategories] on [ProductCategories].[ProductId] = [Product].[ProductID]");
                    break;
            }

            if (_filter.ShowMethod == ECatalogShowMethod.Normal && _filter.CategoryId != null)
            {
                _paging.Where("ProductCategories.CategoryID = {0}", _filter.CategoryId);
            }

            Filter();
            Sorting();
        }

        private void Filter()
        {
            switch (_filter.Type)
            {
                case EProductOnMain.Best:
                    _paging.Where("Bestseller = 1");
                    break;

                case EProductOnMain.New:
                    _paging.Where("New = 1");
                    break;

                case EProductOnMain.Sale:
                    _paging.Where("(Discount <> 0 or DiscountAmount <> 0)");
                    break;
            }

            if (!string.IsNullOrEmpty(_filter.Search))
            {
                var res = ProductSearcherAdmin.Search(_filter.Search);
                var productIds = res.SearchResultItems.Aggregate("", (current, item) => current + (item.Id + "/"));
                _paging.Inner_Join(
                    "(select item, sort from [Settings].[ParsingBySeperator]({0},'/')) as dtt on Product.ProductId=convert(int, dtt.item)",
                    productIds);

                if (_filter.SortingType == FilterSortingType.None)
                {
                    _paging.OrderBy("dtt.sort");
                }
            }

            if (!string.IsNullOrWhiteSpace(_filter.ArtNo))
                _paging.Where("(Product.ArtNo LIKE '%'+{0}+'%' OR [Settings].[ArtNoToString](Product.ProductID) LIKE  '%'+{0}+'%')", _filter.ArtNo);
            
            if (!string.IsNullOrWhiteSpace(_filter.Name))
                _paging.Where("Product.Name LIKE '%'+{0}+'%'", _filter.Name);
            
            if (_filter.ColorId != null)
            {
                var color = ColorService.GetColor(_filter.ColorId);
                if (color != null)
                    _paging.Where("EXISTS (SELECT 1 FROM [Catalog].[Offer] WHERE [Offer].ProductID = Product.ProductID AND [Offer].ColorID={0})", _filter.ColorId);
            }

            if (_filter.SizeId != null)
            {
                var size = SizeService.GetSize(_filter.SizeId);
                if (size != null)
                    _paging.Where("EXISTS (SELECT 1 FROM [Catalog].[Offer] WHERE [Offer].ProductID = Product.ProductID AND [Offer].SizeID={0})", _filter.SizeId);
            }

            if (_filter.PriceFrom.HasValue || _filter.PriceTo.HasValue)
            {
                if (!_isProductSelector && !_onlyProductId)
                {
                    if (_filter.PriceFrom.HasValue)
                        _paging.Where("ISNULL(Price, 0) >= {0}", _filter.PriceFrom.Value);
                

                    if (_filter.PriceTo.HasValue)
                        _paging.Where("ISNULL(Price, 0) <= {0}", _filter.PriceTo.Value);
                }
                else
                {
                    if (_onlyProductId)
                        _paging.Left_Join("[Catalog].[ProductExt] ON [ProductExt].[ProductId] = [Product].[ProductId]");
                    
                    var currency = CurrencyService.BaseCurrency;
                    
                    if (_filter.PriceFrom.HasValue)
                        _paging.Where("ProductExt.PriceTemp >= {0}", _filter.PriceFrom.Value * currency.Rate);

                    if (_filter.PriceTo.HasValue)
                        _paging.Where("ProductExt.PriceTemp <= {0}", _filter.PriceTo.Value * currency.Rate);
                }
            }

            if (_filter.AmountFrom != null)
                _paging.Where("(ISNULL((Select sum (Amount) from catalog.Offer where Offer.ProductID=Product.productID), 0) >= {0})",  _filter.AmountFrom.Value);
            
            if (_filter.AmountTo != null)
                _paging.Where("(ISNULL((Select sum (Amount) from catalog.Offer where Offer.ProductID=Product.productID), 0) <= {0})", _filter.AmountTo.Value);
            

            if (_filter.SortingFrom != null && _filter.SortingTo != null && _filter.ShowMethod == ECatalogShowMethod.Normal)
            {
                _paging.Where("(ProductCategories.SortOrder >= {0} and ProductCategories.SortOrder <= {1})", _filter.SortingFrom, _filter.SortingTo);
            }

            if (_filter.HasPhoto != null)
                _paging.Where($"PhotoName is {(_filter.HasPhoto.Value ? "not" : "")} null");
            
            if (_filter.BrandId != null)
                _paging.Where("BrandId = {0}", _filter.BrandId.Value);

            if (_filter.UnitId.HasValue)
                _paging.Where("Unit = {0}", _filter.UnitId.Value);

            if (_filter.Enabled != null)
                _paging.Where("Enabled = {0}", _filter.Enabled.Value ? "1" : "0");
            
            if (_filter.ExcludeIds != null && !_onlyProductId)
            {
                var excludeIds = _filter.ExcludeIds.Split(',').Select(x => x.TryParseInt()).Where(x => x != 0).ToList();
                if (excludeIds.Count > 0)
                {
                    _paging.Where("Product.ProductId not in (" + String.Join(",", excludeIds) + ")");
                }
            }

            if(_filter.PropertyId.IsNotEmpty())
            {
                var property = PropertyService.GetPropertyById(_filter.PropertyId.TryParseInt());
                if(property != null)
                {
                    _paging.Where("EXISTS (SELECT 1 " +
                        "FROM [Catalog].[ProductPropertyValue] " +
                        "JOIN [Catalog].[PropertyValue] ON [ProductPropertyValue].[PropertyValueID] = [PropertyValue].[PropertyValueID] " +
                        "WHERE [ProductPropertyValue].[ProductID] = Product.ProductId AND [PropertyValue].PropertyID = {0})", property.PropertyId);
                }
            }

            if (_filter.PropertyValueId.IsNotEmpty())
            {
                var propertyValue = PropertyService.GetPropertyValueById(_filter.PropertyValueId.TryParseInt());
                if (propertyValue != null)
                {
                    _paging.Where("EXISTS (SELECT 1 " +
                        "FROM [Catalog].[ProductPropertyValue] " +
                        "WHERE [ProductPropertyValue].[ProductID] = Product.ProductId AND [ProductPropertyValue].PropertyValueID = {0})", propertyValue.PropertyValueId);
                }
            }

            if (_filter.Tags != null && _filter.Tags.Count > 0)
            {
                var tags = string.Join(",", _filter.Tags.Select(x => x.ToString()).ToArray());
                
                _paging.Where(
                    "(SELECT COUNT(*) FROM [Catalog].[TagMap] WHERE [TagMap].[ObjId] = [Product].[ProductId] AND [TagMap].[Type] = {0} AND [TagMap].[TagId] in (" + tags + ")) >= " + _filter.Tags.Count.ToString(), 
                    ETagType.Product.ToString()
                );
            }

            if (_filter.DiscountFrom.HasValue)
                _paging.Where("Discount >= {0}", _filter.DiscountFrom.Value);
            
            if (_filter.DiscountTo.HasValue)
                _paging.Where("Discount <= {0}", _filter.DiscountTo.Value);
            
            if (_filter.DiscountAmountFrom.HasValue)
                _paging.Where("DiscountAmount >= {0}", _filter.DiscountAmountFrom.Value);
            

            if (_filter.DiscountAmountTo.HasValue)
                _paging.Where("DiscountAmount <= {0}", _filter.DiscountAmountTo.Value);
            
            if (_filter.CreatedByList != null && _filter.CreatedByList.Count > 0)
            {
                var condition = $"CreatedBy IN ('{string.Join("','", _filter.CreatedByList)}')";
                if (_filter.CreatedByList.Contains(string.Empty))
                    condition = $"({condition} OR CreatedBy IS NULL)";
                _paging.Where(condition);
            }

            if (!string.IsNullOrWhiteSpace(_filter.BarCode))
                _paging.Where(
                    "( [Product].[BarCode] LIKE '%'+{0}+'%' OR Exists (Select 1 From [Catalog].[Offer] o Where o.ProductId = Product.ProductId and o.BarCode LIKE '%'+{0}+'%') )", 
                    _filter.BarCode);
            
            if (_filter.WarehouseIds != null && _filter.WarehouseIds.Count > 0)
                _paging.Where(
                    "Exists(" +
                              "Select 1 " +
                              "from [Catalog].[Offer] " +
                              "Inner Join [Catalog].[WarehouseStocks] on [Offer].[OfferID] = [WarehouseStocks].[OfferId] " +
                              "Where [WarehouseStocks].[WarehouseId] in ({0}) " +
                              "     AND Offer.ProductId = Product.ProductID " +
                              "     AND [WarehouseStocks].[Quantity] > 0)",
                    _filter.WarehouseIds.ToArray());
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filter.Sorting) || _filter.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy(
                    _filter.ShowMethod != ECatalogShowMethod.Normal
                        ? "Product.ArtNo"
                        : "ProductCategories.SortOrder");
                return;
            }

            var sorting = _filter.Sorting.ToLower();
            var sortingForApply = sorting == "pricestring" ? "price" : sorting;

            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sortingForApply);
            if (field != null)
            {
                if (_filter.SortingType == FilterSortingType.Asc)
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