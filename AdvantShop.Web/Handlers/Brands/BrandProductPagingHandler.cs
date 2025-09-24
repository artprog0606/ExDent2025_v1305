using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Models;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.SQL2;
using AdvantShop.Models.Brand;
using AdvantShop.Models.Catalog;
using AdvantShop.ViewModel.Catalog;


namespace AdvantShop.Handlers.Brands
{
    public class BrandProductPagingHandler
    {
        private readonly BrandModel _brandModel;
        private readonly int _currentPageIndex;
        private readonly bool _isMobile;
        private readonly List<int> _warehouseIds;
        
        private SqlPaging _paging;
        private ProductListViewModel _model;

        public BrandProductPagingHandler(BrandModel brandModel, bool isMobile)
        {
            _brandModel = brandModel;
            _currentPageIndex = brandModel.Page ?? 1;
            _isMobile = isMobile;
            _warehouseIds = WarehouseContext.GetAvailableWarehouseIds();
        }

        public ProductListViewModel GetForBrandItem()
        {
            _model = new ProductListViewModel() {Filter = new CategoryFiltering()};
            
            BuildPaging();
            BuildSorting();
            BuildFilter();

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

            var products = _paging.PageItemsList<ProductModel>();

            _model.Products = new ProductViewModel(products, _isMobile);

            GetViewMode();

            return _model;
        }

        private void BuildPaging()
        {
            _paging = new SqlPaging()
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
                    "Product.MainCategoryId",
                    "Offer.OfferID",
                    "Offer.Amount".AsSqlField("AmountOffer"),
                    "Offer.ArtNo".AsSqlField("OfferArtNo"),
                    "Offer.ColorID",
                    "Offer.SizeID",
                    "MaxAvailable".AsSqlField("Amount"),
                    "Comments",
                    "CurrencyValue",
                    "ProductExt.Gifts",
                    "Units.DisplayName".AsSqlField("UnitDisplayName"),
                    "Units.Name".AsSqlField("UnitName")
                )
                .From("[Catalog].[Product]")
                .Left_Join("[Catalog].[ProductExt] ON [Product].[ProductID] = [ProductExt].[ProductID]")
                .Inner_Join("[Catalog].[Brand] ON [Product].[BrandID] = [Brand].[BrandID] AND [Brand].BrandId = {0}", _brandModel.BrandId);
                
            if (_warehouseIds is null)
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
                        ") as Offer", _warehouseIds.ToArray())
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

            _paging.ItemsPerPage = _currentPageIndex != 0 ? SettingsCatalog.ProductsPerPage : int.MaxValue;
            _paging.CurrentPageIndex = _currentPageIndex != 0 ? _currentPageIndex : 1;
        }

        private void BuildFilter()
        {
            _paging.Where("Product.Enabled={0}", true)
                   .Where("Product.Hidden={0}".IgnoreInCustomData(), false)
                   .Where("AND CategoryEnabled={0}", true);
            
            if (_warehouseIds is null)
                _paging.Where("AND (Offer.Main={0} OR Offer.Main IS NULL)", true);

            if (SettingsCatalog.ShowOnlyAvalible)
            {
                _paging.Where("AND (MaxAvailable > 0 OR [Product].[AllowPreOrder] = 1)");
            }
        }

        private void BuildSorting()
        {
            var sort = _brandModel.Sort != null ? (ESortOrder)_brandModel.Sort : ESortOrder.NoSorting;
            _model.Filter.Sorting = sort;

            if (SettingsCatalog.MoveNotAvaliableToEnd)
            {
                _paging.OrderByDesc("(CASE WHEN Price=0 THEN 0 ELSE 1 END)".AsSqlField("TempSort"))
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

                case ESortOrder.AscByAddingDate:
                    _paging.OrderBy("Product.DateAdded".AsSqlField("DateAddedSort"));
                    break;

                case ESortOrder.DescByAddingDate:
                    _paging.OrderByDesc("Product.DateAdded".AsSqlField("DateAddedSort"));
                    break;

                case ESortOrder.DescByPopular:
                    _paging.OrderByDesc("SortPopular");
                    break;

                case ESortOrder.DescByDiscount:
                    //_paging.OrderByDesc("SortDiscount");
                    _paging.OrderByDesc("Discount".AsSqlField("DiscountSorting"))
                           .OrderByDesc("DiscountAmount".AsSqlField("DiscountAmountSorting"));
                    break;

                default:
                    break;
            }
        }

        private void GetViewMode()
        {
            var mode = SettingsCatalog.GetViewMode(SettingsCatalog.EnabledCatalogViewChange, "viewmode", SettingsCatalog.DefaultCatalogView, false);

            _model.Filter.ViewMode = mode.ToString().ToLower();
            _model.Filter.AllowChangeViewMode = SettingsCatalog.EnabledCatalogViewChange;
        }
    }
}