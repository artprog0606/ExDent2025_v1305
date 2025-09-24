﻿using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Models;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL2;
using AdvantShop.Helpers;
using AdvantShop.Models.Catalog;
using AdvantShop.Repository.Currencies;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Configuration.Settings;

namespace AdvantShop.Handlers.Catalog
{
    public sealed class ProductListHandler
    {
        #region Fields

        private readonly bool _inDepth;
        private readonly int _currentPageIndex;
        private readonly CategoryModel _filter;
        private readonly bool _isMobile;
        private readonly EProductOnMain _type;
        private readonly int _productListId;

        private SqlPaging _paging;
        private ProductListPagingModel _model;
        private readonly List<int> _currentWarehouseIds;

        #endregion

        public ProductListHandler(EProductOnMain type, bool inDepth, CategoryModel filter, int? productListId, bool isMobile)
        {
            _type = type;
            _inDepth = inDepth;
            _filter = filter;
            _isMobile = isMobile;
            _currentPageIndex = filter.Page ?? 1;
            _productListId = productListId ?? 0;
            
            _currentWarehouseIds = WarehouseContext.GetAvailableWarehouseIds();
        }

        public ProductListPagingModel Get()
        {
            _model = new ProductListPagingModel(_inDepth);
            
            BuildPaging();

            var totalCount = _paging.TotalRowsCount;
            var totalPages = _paging.PageCount(totalCount);

            _model.Pager = new Pager()
            {
                TotalItemsCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = _currentPageIndex
            };

            if ((totalPages < _currentPageIndex && _currentPageIndex > 1) || _currentPageIndex < 0)
            {
                return _model;
            }

            var products = 
                _type != EProductOnMain.NewArrivals 
                    ? _paging.PageItemsList<ProductModel>()
                    : _paging.PageItemsListByField<ProductModel>("Product.ProductId");

            _model.Products = new ProductViewModel(products, _isMobile) { 
               ShowBriefDescription = SettingsCatalog.CatalogVisibleBriefDescription
            };

            BuildExcludingFilters();
            GetViewMode();

            return _model;
        }

        private void BuildPaging()
        {
            _paging = new SqlPaging("ProductList", 1000)
                .Select(
                    "Product.ProductID",
                    "CountPhoto",
                    "Photo.PhotoId",
                    "Photo.PhotoName",
                    "Photo.PhotoNameSize1",
                    "Photo.PhotoNameSize2",
                    "Photo.Description".AsSqlField("PhotoDescription"),
                    "Product.ArtNo",
                    "Product.Name",
                    "Recomended".AsSqlField("Recomend"),
                    "Product.Bestseller",
                    "Product.New",
                    "Product.OnSale".AsSqlField("Sales"),
                    "Product.Discount",
                    "Product.DiscountAmount",
                    "Product.BriefDescription",
                    "Product.MinAmount",
                    "Product.MaxAmount",
                    "Product.Enabled",
                    "Product.AllowPreOrder",
                    "Product.Ratio",
                    "Product.ManualRatio",
                    "Product.UrlPath",
                    "Product.DateAdded",
                    "Product.DoNotApplyOtherDiscounts",
                    "Product.Multiplicity",
                    "Product.MainCategoryId",
                    "Offer.OfferID",
                    "Offer.Amount".AsSqlField("AmountOffer"),
                    "Offer.ArtNo".AsSqlField("OfferArtNo"),
                    "Offer.ColorID",
                    "Offer.SizeID",
                    "MaxAvailable".AsSqlField("Amount"),
                    "Comments",
                    "CurrencyValue",
                    "Gifts",
                    "Units.DisplayName".AsSqlField("UnitDisplayName"),
                    "Units.Name".AsSqlField("UnitName"),
                    "ProductExt.AllowAddToCartInCatalog"
                )
                .From("[Catalog].[Product]")
                .Left_Join("[Catalog].[ProductExt] ON [Product].[ProductID] = [ProductExt].[ProductID]");
                
            if (_currentWarehouseIds is null)
                _paging
                    .Left_Join("[Catalog].[Photo] ON [Photo].[PhotoId] = [ProductExt].[PhotoId]")
                    .Left_Join("[Catalog].[Offer] ON [ProductExt].[OfferID] = [Offer].[OfferID]");
            else
                _paging
                    .Select("Offer.ColorID".AsSqlField("PreSelectedColorId"))
                    .Outer_Apply(
                        "(Select top (1) Offer.OfferID, Amount, ArtNo, ColorID, SizeID, Price " +
                        "From [Catalog].[Offer] " +
                        "Inner Join [Catalog].[WarehouseStocks] ON [Offer].[OfferID] = [WarehouseStocks].[OfferId] " +
                        "Where Offer.ProductId = Product.ProductID " +
                        "Order by (Case When [WarehouseStocks].[WarehouseId] in ({0}) AND [WarehouseStocks].[Quantity] > 0 Then 1 Else 0 End) desc, Main desc" +
                        ") as Offer", _currentWarehouseIds.ToArray())
                    .Outer_Apply(
                        "(Select top (1) PhotoId, PhotoName, PhotoNameSize1, PhotoNameSize2, Description " +
                        "From Catalog.Photo " +
                        "Where Photo.ObjId = Product.ProductId and (Photo.ColorID = Offer.ColorID OR Photo.ColorID IS NULL) AND Type = 'Product' " +
                        "Order by [Photo].Main DESC, [Photo].[PhotoSortOrder], [PhotoId]" +
                        ") as Photo"
                    );
                
            _paging
                .Inner_Join("[Catalog].[Currency] ON [Currency].[CurrencyID] = [Product].[CurrencyID]")
                .Left_Join("[Catalog].[Units] ON [Product].[Unit] = [Units].[Id]");

            if (SettingsCatalog.ComplexFilter)
            {
                _paging.Select(
                    "Colors",
                    "NotSamePrices".AsSqlField("MultiPrices"),
                    "MinPrice".AsSqlField("BasePrice")
                );
            }
            else
            {
                _paging.Select(
                    "null".AsSqlField("Colors"),
                    "0".AsSqlField("MultiPrices"),
                    "Price".AsSqlField("BasePrice")
                );
            }

            BuildSorting();
            BuildFilter();

            _paging.ItemsPerPage = _currentPageIndex != 0 ? SettingsCatalog.ProductsPerPage : int.MaxValue;
            _paging.CurrentPageIndex = _currentPageIndex != 0 ? _currentPageIndex : 1;
        }

        private void BuildSorting()
        {
            var sort = _filter.Sort ?? ESortOrder.NoSorting;
            _model.Filter.Sorting = sort;

            if (SettingsCatalog.MoveNotAvaliableToEnd)
            {
                _paging.OrderByDesc("(CASE WHEN PriceTemp=0 THEN 0 ELSE 1 END)".AsSqlField("TempSort"))
                       .OrderByDesc("AmountSort".AsSqlField("TempAmountSort"));
            }

            switch (sort)
            {
                case ESortOrder.AscByName:
                    _paging.OrderBy("Product.Name".AsSqlField("NameSort"));
                    break;
                case ESortOrder.DescByName:
                    _paging.OrderByDesc("Product.Name".AsSqlField("NameSort"));
                    break;

                case ESortOrder.AscByPrice:
                    _paging.OrderBy("PriceTemp");
                    break;

                case ESortOrder.DescByPrice:
                    _paging.OrderByDesc("PriceTemp");
                    break;

                case ESortOrder.AscByRatio:
                    _paging.OrderBy("(CASE WHEN ManualRatio is NULL THEN Ratio ELSE ManualRatio END)".AsSqlField("RatioSort"));
                    break;

                case ESortOrder.DescByRatio:
                    _paging.OrderByDesc("(CASE WHEN ManualRatio is NULL THEN Ratio ELSE ManualRatio END)".AsSqlField("RatioSort"));
                    break;

                case ESortOrder.DescByAddingDate:
                    _paging.OrderByDesc("Product.ProductID".AsSqlField("ProductIDSort"));
                    break;
                    
                case ESortOrder.DescByPopular:
                    _paging.OrderByDesc("SortPopular");
                    break;
                    
                case ESortOrder.DescByDiscount:
                    _paging.OrderByDesc("Discount".AsSqlField("DiscountSorting"))
                           .OrderByDesc("DiscountAmount".AsSqlField("DiscountAmountSorting"));
                    break;

                default:
                    switch (_type)
                    {
                        case EProductOnMain.Best:
                            _paging.OrderBy("SortBestseller".AsSqlField("Sorting"))
                                   .OrderByDesc("Product.ProductID".AsSqlField("ProductIDSorting"));
                            break;
                        case EProductOnMain.New:
                            _paging.OrderBy("SortNew".AsSqlField("Sorting"))
                                   .OrderByDesc("Product.ProductID".AsSqlField("ProductIDSorting"));
                            break;
                        case EProductOnMain.NewArrivals:
                            _paging.OrderByDesc("Product.ProductID".AsSqlField("ProductIDSorting"));
                            break;
                        case EProductOnMain.Sale:
                            _paging.OrderBy("SortDiscount".AsSqlField("Sorting"))
                                   .OrderByDesc("Product.ProductID".AsSqlField("ProductIDSorting"));
                            break;
                        case EProductOnMain.List:
                            _paging.OrderBy("[Product_ProductList].[SortOrder]".AsSqlField("Sorting"));
                            break;
                    }
                    break;
            }
        }

        private void BuildFilter()
        {
            _paging.Where("Product.Enabled={0}", true)
                   .Where("AND CategoryEnabled={0}", true);

            bool hasFilter = false;

            if (_filter.CategoryId > 0)
            {
                _paging.Where(
                  "AND Exists( select 1 from [Catalog].[ProductCategories] INNER JOIN [Settings].[GetChildCategoryByParent]({0}) AS hCat ON hCat.id = [ProductCategories].[CategoryID] and  ProductCategories.ProductId = [Product].[ProductID])",
                  _filter.CategoryId);
            }

            if (_type == EProductOnMain.Best)
            {
                _paging.Where("AND Bestseller=1");
            }
            else if (_type == EProductOnMain.New)
            {
                _paging.Where("AND New=1");
            }
            else if (_type == EProductOnMain.Sale)
            {
                _paging.Where("AND (Discount>0 or DiscountAmount>0)");
            }
            else if (_type == EProductOnMain.List && _productListId != 0)
            {
                _paging.Left_Join("[Catalog].[Product_ProductList] ON [Product_ProductList].[ProductId] = [Product].[ProductId]");
                _paging.Where("AND [Product_ProductList].[ListId] = {0}", _productListId);
            }

            var currency = CurrencyService.CurrentCurrency;
            if (SettingsCatalog.DefaultCurrencyIso3 != currency.Iso3)
            {
                _paging.Where("", currency.Iso3);
            }

            if (SettingsCatalog.ShowOnlyAvalible || _filter.Available)
            {
                _paging.Where("AND (MaxAvailable > 0 OR [Product].[AllowPreOrder] = 1)");
            }
            _model.Filter.Available = _filter.Available;
            
            if (!string.IsNullOrEmpty(_filter.Brand))
            {
                var brandIds = _filter.Brand.Split(',').Select(x => x.TryParseInt()).Where(x => x != 0).ToList();
                if (brandIds.Count > 0)
                {
                    _model.Filter.BrandIds = brandIds;
                    _paging.Where("AND Product.BrandID IN ({0})", brandIds.ToArray());
                    hasFilter = true;
                }
            }

            if (_filter.PriceFrom.HasValue)
            {
                _model.Filter.PriceFrom = _filter.PriceFrom;

                _paging.Where("and ProductExt.PriceTemp >= {0} ", _filter.PriceFrom * currency.Rate);
                hasFilter = true;
            }
            
            if (_filter.PriceTo.HasValue)
            {
                _model.Filter.PriceTo = _filter.PriceTo;

                _paging.Where("and ProductExt.PriceTemp <= {0}", _filter.PriceTo * currency.Rate);
                hasFilter = true;
            }

            if (!string.IsNullOrEmpty(_filter.Size))
            {
                var sizeIds = _filter.Size.Split(',').Select(item => item.TryParseInt()).Where(id => id != 0).ToList();
                if (sizeIds.Count > 0)
                {
                    _model.Filter.SizeIds = sizeIds;
                    _paging.Where(
                        SettingsCatalog.ShowOnlyAvalible || _filter.Available
                            ? "and Exists(Select 1 from [Catalog].[Offer] where Offer.[SizeID] IN ({0}) and Offer.ProductId = [Product].[ProductID] AND (Offer.amount > 0 OR [Product].[AllowPreOrder] = 1))"
                            : "and Exists(Select 1 from [Catalog].[Offer] where Offer.[SizeID] IN ({0}) and Offer.ProductId = [Product].[ProductID])",
                        sizeIds.ToArray());
                    hasFilter = true;
                }
            }

            if (!string.IsNullOrEmpty(_filter.Color))
            {
                var colorIds = _filter.Color.Split(',').Select(item => item.TryParseInt()).Where(id => id != 0).ToList();
                if (colorIds.Count > 0)
                {
                    _model.Filter.ColorIds = colorIds;
                    _paging.Where(
                        SettingsCatalog.ShowOnlyAvalible || _filter.Available
                            ? "and Exists(Select 1 from [Catalog].[Offer] where Offer.[ColorID] IN ({0}) and Offer.ProductId = [Product].[ProductID] AND (Offer.amount > 0 OR [Product].[AllowPreOrder] = 1))"
                            : "and Exists(Select 1 from [Catalog].[Offer] where Offer.[ColorID] IN ({0}) and Offer.ProductId = [Product].[ProductID])",
                        colorIds.ToArray());
                    hasFilter = true;
                }

                if (SettingsCatalog.ComplexFilter)
                {
                    _paging.Select(
                        string.Format(
                            "(select Top 1 PhotoName from catalog.Photo inner join Catalog.Offer on Photo.objid=Offer.Productid and Type='product'" +
                            " Where Offer.ProductId=Product.ProductId and Photo.ColorID in({0}) Order by Photo.Main Desc, Photo.PhotoSortOrder)",
                            _model.Filter.ColorIds.AggregateString(",")).AsSqlField("AdditionalPhoto"));
                }
                else
                {
                    _paging.Select("null".AsSqlField("AdditionalPhoto"));
                }
            }
            else
            {
                _paging.Select("null".AsSqlField("AdditionalPhoto"));
            }
            
            
            var filterByCurrentWarehouseIds = WarehouseContext.GetAvailableWarehouseIds();
            if (filterByCurrentWarehouseIds != null && filterByCurrentWarehouseIds.Count > 0)
            {
                _paging.Where(
                    "and (Product.AllowPreOrder = 1 " +
                                    "OR Exists(" +
                                        "Select 1 from [Catalog].[Offer] " + 
                                        "Inner Join [Catalog].[WarehouseStocks] on [Offer].[OfferID] = [WarehouseStocks].[OfferId] " + 
                                        "Where [WarehouseStocks].[WarehouseId] in ({0}) " + 
                                        "     AND Offer.ProductId = Product.ProductID " + 
                                        "     AND [WarehouseStocks].[Quantity] > 0))", 
                                        filterByCurrentWarehouseIds.ToArray());
            }

            if (!string.IsNullOrEmpty(_filter.Warehouse))
            {
                var warehouseIds =
                    _filter.Warehouse
                        .Split(',')
                        .Select(x => x.TryParseInt())
                        .Where(x => x != 0)
                        .ToList();
                
                if (warehouseIds.Count > 0)
                {
                    _model.Filter.WarehouseIds = warehouseIds;
                    _paging.Where(
                        "and Exists(Select 1 from [Catalog].[Offer] "
                        + "Inner Join [Catalog].[WarehouseStocks] ON [Offer].[OfferID] = [WarehouseStocks].[OfferId] "
                        + "where [WarehouseStocks].[WarehouseId] IN ({0}) "
                        + "     and Offer.ProductId = [Product].[ProductID] "
                        + "     AND [WarehouseStocks].[Quantity] > 0)", 
                        warehouseIds.ToArray());
                    hasFilter = true;
                }
            }

            if (!string.IsNullOrEmpty(_filter.Prop))
            {
                var selectedPropertyIDs = new List<int>();
                var filterCollection = _filter.Prop.Split('-');
                foreach (var val in filterCollection)
                {
                    var tempListIds = new List<int>();
                    foreach (int id in val.Split(',').Select(item => item.TryParseInt()).Where(id => id != 0))
                    {
                        tempListIds.Add(id);
                        selectedPropertyIDs.Add(id);
                    }
                    if (tempListIds.Count > 0)
                        _paging.Where("AND Exists(Select 1 from [Catalog].[ProductPropertyValue] where [Product].[ProductID] = [ProductID] and PropertyValueID IN ({0}))", tempListIds.ToArray());
                }
                _model.Filter.PropertyIds = selectedPropertyIDs;
                hasFilter = true;
            }

            var rangeIds = new Dictionary<int, KeyValuePair<float, float>>();
            var rangeQueries =
                HttpContext.Current.Request.QueryString.AllKeys.Where(
                    p => p != null && p.StartsWith("prop_") && (p.EndsWith("_min") || p.EndsWith("_max"))).ToList();

            foreach (var rangeQuery in rangeQueries)
            {
                if (rangeQuery.EndsWith("_max"))
                    continue;

                var propertyId = rangeQuery.Split('_')[1].TryParseInt();
                if (propertyId == 0)
                    continue;

                var min = HttpContext.Current.Request.QueryString[rangeQuery].TryParseFloat();
                var max = HttpContext.Current.Request.QueryString[rangeQuery.Replace("min", "max")].TryParseFloat();

                rangeIds.Add(propertyId, new KeyValuePair<float, float>(min, max));
            }

            rangeIds = ModulesExecuter.GetRangeIds(rangeIds);
            if (rangeIds.Count > 0)
            {
                foreach (var id in rangeIds.Keys)
                {
                    _paging.Where(
                        "AND Exists( select 1 from [Catalog].[ProductPropertyValue] " +
                        "Inner Join [Catalog].[PropertyValue] on [PropertyValue].[PropertyValueID] = [ProductPropertyValue].[PropertyValueID] " +
                        "Where [Product].[ProductID] = [ProductID] and PropertyId = {0} and RangeValue >= {1} and RangeValue <= {2})",
                        id, rangeIds[id].Key, rangeIds[id].Value);
                }
                hasFilter = true;
            }
            _model.Filter.RangePropertyIds = rangeIds;

            if (!string.IsNullOrWhiteSpace(_filter.TagUrl) && _filter.TagId > 0)
            {
                _paging.Where("AND Exists(Select 1 from [Catalog].[TagMap] where TagMap.[ObjId] = Product.[ProductID] and TagMap.[Type] ='Product' and TagMap.TagId={0})", _filter.TagId);
                hasFilter = true;
            }

            if (!hasFilter && !_filter.Available)
                _paging.Where("Product.Hidden={0}".IgnoreInCustomData(), false);
        }

        private void BuildExcludingFilters()
        {
            if (!SettingsCatalog.ExcludingFilters)
                return;
            
            var tasks = new List<Task>();

            if (SettingsCatalog.ShowProducerFilter)
            {
               var task = Task.Run(() =>
                {
                    _model.Filter.AvailableBrandIds =
                        _paging.GetCustomData("Product.BrandID", " AND Product.BrandID is not null",
                            reader => SQLDataHelper.GetInt(reader, "BrandID"), true);
                });
                tasks.Add(task);
            }

            if (SettingsCatalog.ShowSizeFilter)
            {
                var task = Task.Run(() =>
                {
                    _model.Filter.AvailableSizeIds =
                        _paging.GetCustomData("sizeOffer.SizeID", " AND sizeOffer.SizeID is not null",
                            reader => SQLDataHelper.GetInt(reader, "SizeID"), true,
                            "Left JOIN [Catalog].[Offer] as sizeOffer ON [Product].[ProductID] = [sizeOffer].[ProductID]");
                });
                tasks.Add(task);
            }

            if (SettingsCatalog.ShowColorFilter)
            {
                var task = Task.Run(() =>
                {
                    _model.Filter.AvailableColorIds =
                        _paging.GetCustomData("colorOffer.ColorID", " AND colorOffer.ColorID is not null",
                            reader => SQLDataHelper.GetInt(reader, "ColorID"), true,
                            "Left JOIN [Catalog].[Offer] as colorOffer ON [Product].[ProductID] = [colorOffer].[ProductID]");
                });
                tasks.Add(task);
            }

            if (SettingsCatalog.ShowWarehouseFilter)
            {
                var task = Task.Run(() =>
                {
                    _model.Filter.AvailableWarehouseIds =
                        _paging.GetCustomData(
                            "[WarehouseStocks].[WarehouseId]", 
                            " AND [WarehouseStocks].[Quantity] > 0",
                            reader => SQLDataHelper.GetInt(reader, "WarehouseId"), 
                            true,
                            "Left JOIN [Catalog].[Offer] as warehouseOffer ON [Product].[ProductID] = [warehouseOffer].[ProductID] "
                            + "LEFT JOIN [Catalog].[WarehouseStocks] ON warehouseOffer.[OfferID] = [WarehouseStocks].[OfferId]");
                });
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray(), 1000);
        }

        public ProductListPagingModel GetForFilter()
        {
            _model = new ProductListPagingModel(_inDepth);

            BuildPaging();
            BuildExcludingFilters();

            return _model;
        }

        public ProductListPagingModel GetForFilterProductCount()
        {
            _model = new ProductListPagingModel(_inDepth);

            BuildPaging();

            var totalCount = _paging.TotalRowsCount;

            _model.Pager = new Pager
            {
                TotalItemsCount = totalCount,
                CurrentPage = _currentPageIndex
            };

            return _model;
        }

        private void GetViewMode()
        {
            if (!_isMobile)
            {
                var mode = SettingsCatalog.GetViewMode(SettingsCatalog.EnabledCatalogViewChange, "viewmode", SettingsCatalog.DefaultCatalogView, false);

                _model.Filter.ViewMode = mode.ToString().ToLower();
                _model.Filter.AllowChangeViewMode = SettingsCatalog.EnabledCatalogViewChange;
            }
            else
            {
                var mode = SettingsCatalog.GetViewMode(SettingsMobile.EnableCatalogViewChange, "mobile_viewmode", SettingsMobile.DefaultCatalogView, true);

                _model.Filter.ViewMode = mode.ToString().ToLower();
                _model.Filter.AllowChangeViewMode = SettingsMobile.EnableCatalogViewChange;
            }
        }
    }
}