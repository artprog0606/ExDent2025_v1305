using System.Collections.Generic;
using AdvantShop.App.Landing.Models.Catalogs;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Models;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL2;
using System.Linq;
using System.Threading.Tasks;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.App.Landing.Handlers.Products
{
    public class CatalogProductPaging
    {
        #region Ctor

        private readonly ProductsByCategoryModel _categoryModel;
        private readonly bool _indepth;
        private readonly int _currentPageIndex;
        private readonly Category _category;

        private SqlPaging _paging;
        private CatalogProductPagingModel _model;

        public CatalogProductPaging(ProductsByCategoryModel categoryModel, bool indepth)
        {
            _categoryModel = categoryModel;
            _indepth = indepth;

            _currentPageIndex = _categoryModel.Page > 0 ? _categoryModel.Page : 1;
            _category = CategoryService.GetCategory(_categoryModel.CategoryId);
        }

        #endregion

        public CatalogProductPagingModel Execute()
        {
            _model = new CatalogProductPagingModel()
            {
                Filter = new LpCategoryFiltering(_categoryModel.CategoryId, _indepth)
            };

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

            var products = _paging.PageItemsList<ProductModel>();

            _model.ProductsModel = new ProductViewModel(products);

            return _model;
        }

        public CatalogProductPagingModel GetForFilter()
        {
            _model = new CatalogProductPagingModel()
            {
                Filter = new LpCategoryFiltering(_categoryModel.CategoryId, _indepth)
            };

            BuildPaging();
            BuildExludingFilters();

            return _model;
        }

        private void BuildPaging()
        {
            _paging = new SqlPaging("LpCategoryPaging");
            _paging.Select(
                "distinct Product.ProductID",
                "CountPhoto",
                "Photo.PhotoId",
                "PhotoName",
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
                "Offer.ArtNo".AsSqlField("OfferArtNo"),
                "Offer.ColorID",
                "MaxAvailable".AsSqlField("Amount"),
                "Comments",
                "CurrencyValue",
                "Gifts"
                );

            _paging.From("[Catalog].[Product]");
            _paging.Left_Join("[Catalog].[ProductExt] ON [Product].[ProductID] = [ProductExt].[ProductID]");
            _paging.Left_Join("[Catalog].[Photo] ON [Photo].[PhotoId] = [ProductExt].[PhotoId]");
            _paging.Left_Join("[Catalog].[Offer] ON [ProductExt].[OfferID] = [Offer].[OfferID]");
            _paging.Inner_Join("[Catalog].[Currency] ON [Currency].[CurrencyID] = [Product].[CurrencyID]");

            if (!_indepth)
                _paging.Inner_Join("[Catalog].[ProductCategories] ON [ProductCategories].[CategoryID] = {0} and  ProductCategories.ProductId = [Product].[ProductID]", _categoryModel.CategoryId);
            else
                _paging.Inner_Join("[Catalog].[ProductCategories] ON ProductCategories.ProductId = [Product].[ProductID]");

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
            
            BuildFilter();
            BuildSorting();

            _paging.ItemsPerPage = _categoryModel.CountPerPage != 0 ? _categoryModel.CountPerPage : SettingsCatalog.ProductsPerPage;
            _paging.CurrentPageIndex = _currentPageIndex;
        }

        private void BuildSorting()
        {
            var sort = _categoryModel.Sort != null
                ? (ESortOrder) _categoryModel.Sort
                : (_category != null ? _category.Sorting : ESortOrder.NoSorting);

            if (SettingsCatalog.MoveNotAvaliableToEnd)
            {
                _paging.OrderByDesc("(CASE WHEN Price=0 THEN 0 ELSE 1 END)".AsSqlField("TempSort"));
                _paging.OrderByDesc("AmountSort".AsSqlField("TempAmountSort"));
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
                    _paging.OrderBy("DateAdded".AsSqlField("DateAddedSort"));
                    break;

                case ESortOrder.DescByAddingDate:
                    _paging.OrderByDesc("DateAdded".AsSqlField("DateAddedSort"));
                    break;

                case ESortOrder.DescByPopular:
                    _paging.OrderByDesc("SortPopular");
                    break;

                case ESortOrder.DescByDiscount:
                    _paging.OrderByDesc("Discount".AsSqlField("DiscountSorting"));
                    _paging.OrderByDesc("DiscountAmount".AsSqlField("DiscountAmountSorting"));
                    break;
            }

            if (!_indepth)
                _paging.OrderBy("[ProductCategories].[SortOrder]".AsSqlField("ProductCategorySortOrder"));
        }

        private void BuildFilter()
        {
            _paging.Where("Product.Enabled={0}", true);
            _paging.Where("AND CategoryEnabled={0}", true);
            _paging.Where("AND (Offer.Main={0} OR Offer.Main IS NULL)", true);
            
            if (_indepth)
            {
                _paging.Where(
                    "AND Exists( select 1 from [Catalog].[ProductCategories] INNER JOIN [Settings].[GetChildCategoryByParent]({0}) AS hCat ON hCat.id = [ProductCategories].[CategoryID] and  ProductCategories.ProductId = [Product].[ProductID])", 
                    _category.CategoryId);
            }

            if (_categoryModel.Brand != null && _categoryModel.Brand.Count > 0)
            {
                var brandIds = _categoryModel.Brand.Where(x => x != 0).ToList();
                if (brandIds.Count > 0)
                {
                    _paging.Where("AND Product.BrandID IN ({0})", brandIds.ToArray());
                    _model.Filter.BrandIds = brandIds;
                }
            }

            if (_categoryModel.Size != null && _categoryModel.Size.Count > 0)
            {
                var sizeIds = _categoryModel.Size.Where(x => x != 0).ToList();
                if (sizeIds.Count > 0)
                {
                    _paging.Where(
                        SettingsCatalog.ShowOnlyAvalible
                            ? "and Exists(Select 1 from [Catalog].[Offer] where Offer.[SizeID] IN ({0}) and Offer.ProductId = [Product].[ProductID] AND (Offer.amount > 0 OR [Product].[AllowPreOrder] = 1))"
                            : "and Exists(Select 1 from [Catalog].[Offer] where Offer.[SizeID] IN ({0}) and Offer.ProductId = [Product].[ProductID])",
                        sizeIds.ToArray());

                    _model.Filter.SizeIds = sizeIds;
                }
            }

            if (_categoryModel.Color != null && _categoryModel.Color.Count > 0)
            {
                var colorIds = _categoryModel.Color.Where(x => x != 0).ToList();
                if (colorIds.Count > 0)
                {
                    _paging.Where(
                        SettingsCatalog.ShowOnlyAvalible
                            ? "and Exists(Select 1 from [Catalog].[Offer] where Offer.[ColorID] IN ({0}) and Offer.ProductId = [Product].[ProductID] AND (Offer.amount > 0 OR OR [Product].[AllowPreOrder] = 1))"
                            : "and Exists(Select 1 from [Catalog].[Offer] where Offer.[ColorID] IN ({0}) and Offer.ProductId = [Product].[ProductID])",
                        colorIds.ToArray());

                    _model.Filter.ColorIds = colorIds;
                }

                if (SettingsCatalog.ComplexFilter)
                {
                    _paging.Select(
                        string.Format(
                            "(select Top 1 PhotoName from catalog.Photo inner join Catalog.Offer on Photo.objid=Offer.Productid and Type='product'" +
                            " Where Offer.ProductId=Product.ProductId and Photo.ColorID in({0}) Order by Photo.Main Desc, Photo.PhotoSortOrder)",
                            colorIds.AggregateString(",")).AsSqlField("AdditionalPhoto"));
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

            var currency = CurrencyService.CurrentCurrency;
            if (SettingsCatalog.DefaultCurrencyIso3 != currency.Iso3)
            {
                _paging.Where("", currency.Iso3);
            }

            if (_categoryModel.PriceFrom.HasValue || _categoryModel.PriceTo.HasValue)
            {
                var pricefrom = _categoryModel.PriceFrom ?? 0;
                var priceto = _categoryModel.PriceTo ?? int.MaxValue;

                _paging.Where("and (ProductExt.PriceTemp >= {0} and ProductExt.PriceTemp <= {1})", pricefrom * currency.Rate, priceto * currency.Rate);

                _model.Filter.PriceFrom = pricefrom;
                _model.Filter.PriceTo = priceto;
            }

            if (_categoryModel.Prop != null && _categoryModel.Prop.Count > 0)
            {
                foreach (var propIds in _categoryModel.Prop)
                {
                    _paging.Where(
                        "AND Exists(Select 1 from [Catalog].[ProductPropertyValue] where [Product].[ProductID] = [ProductID] and PropertyValueID IN ({0}))",
                        propIds.ToArray());

                    _model.Filter.PropertyIds.AddRange(propIds);
                }
            }

            if (_categoryModel.PropRange != null && _categoryModel.PropRange.Count > 0)
            {
                foreach (var rangeItem in _categoryModel.PropRange)
                {
                    _paging.Where(
                        "AND Exists( select 1 from [Catalog].[ProductPropertyValue] " +
                        "Inner Join [Catalog].[PropertyValue] on [PropertyValue].[PropertyValueID] = [ProductPropertyValue].[PropertyValueID] " +
                        "Where [Product].[ProductID] = [ProductID] and PropertyId = {0} and RangeValue >= {1} and RangeValue <= {2})",
                        rangeItem.Id, rangeItem.Min, rangeItem.Max);

                    _model.Filter.RangePropertyIds.Add(rangeItem.Id, new KeyValuePair<float, float>(rangeItem.Min, rangeItem.Max));
                }
            }
            
            if (SettingsCatalog.ShowOnlyAvalible)
            {
                _paging.Where("AND (MaxAvailable > 0 OR [Product].[AllowPreOrder] = 1)");

                _model.Filter.Available = true;
            }
        }

        private void BuildExludingFilters()
        {
            if (!SettingsCatalog.ExcludingFilters)
                return;

            var tasks = new List<Task>();
            var task = Task.Run(() =>
            {
                _model.Filter.AvailablePropertyIds =
                    _paging.GetCustomData("PropertyValueID", " AND PropertyValueID is not null",
                        reader => SQLDataHelper.GetInt(reader, "PropertyValueID"), true,
                        "Left JOIN [Catalog].[ProductPropertyValue] ON [Product].[ProductID] = [ProductPropertyValue].[ProductID]");
            });
            tasks.Add(task);

            if (SettingsCatalog.ShowProducerFilter)
            {
                task = Task.Run(() =>
                {
                    _model.Filter.AvailableBrandIds =
                        _paging.GetCustomData("Product.BrandID", " AND Product.BrandID is not null",
                            reader => SQLDataHelper.GetInt(reader, "BrandID"), true);
                });
                tasks.Add(task);
            }

            if (SettingsCatalog.ShowSizeFilter)
            {
                task = Task.Run(() =>
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
                task = Task.Run(() =>
                {
                    _model.Filter.AvailableColorIds =
                        _paging.GetCustomData("colorOffer.ColorID", " AND colorOffer.ColorID is not null",
                            reader => SQLDataHelper.GetInt(reader, "ColorID"), true,
                            "Left JOIN [Catalog].[Offer] as colorOffer ON [Product].[ProductID] = [colorOffer].[ProductID]");
                });
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray(), 1000);
        }
    }
}
