using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Saas;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Catalog
{
    public partial class ProductViewModel
    {
        public ProductViewModel()
        {
            HidePrice = SettingsCatalog.HidePrice;
            TextInsteadOfPrice = SettingsCatalog.TextInsteadOfPrice;

            DisplayBuyButton = SettingsCatalog.DisplayBuyButton && !SettingsCatalog.HidePrice;
            DisplayPreOrderButton = SettingsCatalog.DisplayPreOrderButton && !SettingsCatalog.HidePrice;
            AllowBuyOutOfStockProducts = SettingsCheckout.OutOfStockAction == eOutOfStockAction.Cart;
            AllowPreOrderOutOfStockProducts = SettingsCheckout.OutOfStockAction == eOutOfStockAction.Preorder;

            DisplayRating = SettingsCatalog.EnableProductRating;
            DisplayComparison = SettingsCatalog.EnableCompareProducts;
            DisplayPhotoPreviews = SettingsCatalog.EnablePhotoPreviews;
            DisplayPhotoCount = SettingsCatalog.ShowCountPhoto;
            DisplayQuickView = SettingsCatalog.ShowQuickView;
            DisplayProductArtNo = SettingsCatalog.ShowProductArtNo;
            DisplayReviewCount = SettingsCatalog.AllowReviews;
            ShowNotAvailableLabel = SettingsCatalog.ShowNotAvaliableLable;
            
            BuyButtonText = SettingsCatalog.BuyButtonText;
            PreOrderButtonText = SettingsCatalog.PreOrderButtonText;

            ColorImageHeight = SettingsPictureSize.ColorIconHeightCatalog;
            ColorImageWidth = SettingsPictureSize.ColorIconWidthCatalog;
            
            PhotoWidth = SettingsPictureSize.SmallProductImageWidth;
            PhotoHeight = SettingsPictureSize.SmallProductImageHeight;

            PhotoPreviewWidth = SettingsPictureSize.XSmallProductImageWidth;
            PhotoPreviewHeight = SettingsPictureSize.XSmallProductImageHeight;

            PhotoXSmallWidth = SettingsPictureSize.XSmallProductImageWidth;
            PhotoSmallWidth = SettingsPictureSize.SmallProductImageWidth;
            PhotoMiddleWidth = SettingsPictureSize.MiddleProductImageWidth;
            PhotoMiddleHeight = SettingsPictureSize.MiddleProductImageHeight;
            PhotoBigWidth = SettingsPictureSize.BigProductImageWidth;
            
            CountProductsInLine = SettingsDesign.CountCatalogProductInLine;
            ColorsViewMode = SettingsCatalog.ColorsViewMode;
            ShowAmountsTableInCatalog = SettingsPriceRules.ShowAmountsTableInCatalog;
            CountLinesProductName = SettingsCatalog.CountLinesProductName;
            Products = new List<ProductItem>();
        }

        public ProductViewModel(List<ProductItem> products) : this()
        {
            Products = products ?? Products;
        }

        public ProductViewModel(List<ProductModel> products) : this()
        {
            if (products != null && products.Count > 0)
            {
                var productDiscounts = new List<ProductDiscount>();
                
                var customerGroup = CustomerContext.CurrentCustomer.CustomerGroup;

                var discountModules = AttachedModules.GetModuleInstances<IDiscount>();
                if (discountModules != null && discountModules.Count != 0)
                {
                    foreach (var discountModule in discountModules)
                    {
                        var discounts = discountModule.GetProductDiscountsList();
                        if (discounts != null && discounts.Count > 0)
                            productDiscounts.AddRange(discounts);
                    }
                }

                var priceRulesActive = !SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.PriceTypes;

                var customLabels = new List<ProductLabel>();

                var labelModules = AttachedModules.GetModuleInstances<ILabel>();
                if (labelModules != null && labelModules.Count != 0)
                {
                    foreach (var labelModule in labelModules)
                    {
                        var label = labelModule.GetLabel();
                        if (label != null)
                            customLabels.Add(label);

                        var labels = labelModule.GetLabels();
                        if (labels != null && labels.Count > 0)
                            customLabels.AddRange(labels);
                    }
                }

                foreach (var product in products)
                {
                    var discountByDatetime = DiscountByTimeService.GetCurrentDiscount(product.ProductId);

                    var productLabels = customLabels
                        .Where(x => x.ProductIds.Contains(product.ProductId))
                        .Select(x => x.LabelCode)
                        .ToList();   
                    
                    Products.Add(new ProductItem(product, customerGroup, discountByDatetime, productDiscounts, priceRulesActive, productLabels));
                }
            }
        }
        
        public ProductViewModel(List<ProductModel> products, bool isMobile) : this(products)
        {
            IsMobile = isMobile;

            if (isMobile)
            {
                BlockProductPhotoHeight = SettingsMobile.BlockProductPhotoHeight;
                BlockProductPhotoMiddleHeight = SettingsMobile.BlockProductPhotoMiddleHeight;
                
                var controllerName = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"] as string;
                var isCatalog = controllerName != null && controllerName == "Catalog";

                ProductViewMode =
                    isCatalog
                        ? SettingsCatalog.GetViewMode(SettingsMobile.EnableCatalogViewChange, "mobile_viewmode",
                            SettingsMobile.DefaultCatalogView, true)
                        : ProductViewMode.Tile;

                ProductImageType = ProductViewMode == ProductViewMode.Single
                    ? ProductImageType.Big
                    : SettingsMobile.ProductImageType;

                CountLinesProductName = SettingsMobile.CountLinesProductName;
            }
        }

        public bool IsMobile { get; private set; }

        public int Id { get; set; }
        public string Title { get; set; }
        public bool DisplayBuyButton { get; set; }
        public bool DisplayPreOrderButton { get; set; }
        public bool AllowBuyOutOfStockProducts { get; set; }
        public bool AllowPreOrderOutOfStockProducts { get; set; }

        public bool DisplayPhotoPreviews { get; set; }
        public bool DisplayComparison { get; set; }
        public bool DisplayRating { get; set; }
        public bool DisplayQuickView { get; set; }
        public bool DisplayProductArtNo { get; set; }
        public bool DisplayPhotoCount { get; set; }
        public bool DisplayReviewCount { get; set; }

        public bool HidePrice { get; set; }
        public string TextInsteadOfPrice { get; set; }

        public int CountProductsInLine { get; set; }

        public string BuyButtonText { get; set; }
        public string PreOrderButtonText { get; set; }

        public int ColorImageHeight { get; set; }
        public int ColorImageWidth { get; set; }
        public string SelectedColors { get; set; }

        private List<int> _selectedColorsList;

        public List<int> SelectedColorsList
        {
            get
            {
                if (_selectedColorsList == null && SelectedColors != null && SelectedColors.Length > 2)
                {
                    _selectedColorsList = JsonConvert.DeserializeObject<List<int>>(SelectedColors);
                }

                return _selectedColorsList;
            }
        }

        public int? SelectedSizeId { get; set; }

        public int PhotoWidth { get; set; }
        public int PhotoHeight { get; set; }

        public int PhotoPreviewWidth { get; set; }
        public int PhotoPreviewHeight { get; set; }
        
        public int PhotoXSmallWidth { get; set; }
        public int PhotoSmallWidth { get; set; }
        public int PhotoMiddleWidth { get; set; }
        public int PhotoMiddleHeight { get; set; }
        public int PhotoBigWidth { get; set; }
        
        public List<ProductItem> Products { get; private set; }

        public eLazyLoadType LazyLoadType { get; set; }

        public ColorsViewMode ColorsViewMode { get; set; }

        public int BlockProductPhotoHeight { get; set; }
        
        public int BlockProductPhotoMiddleHeight { get; set; }
        
        public bool ShowNotAvailableLabel { get; set; }
        public ProductImageType ProductImageType { get; set; }
        public bool HideMarkers { get; set; }



        public string WrapCssClass { get; set; }
        public bool ShowAmountsTableInCatalog { get; private set; }
        
        public ProductViewMode ProductViewMode { get; private set; }
        
        public bool ShowBriefDescription { get; set; }
        public int  CountLinesProductName { get; set; }
        
        public int SortOrder { get; set; }
        public EProductOnMain Type { get; set; }
        public string LinkText { get; set; }
        public string UrlPath { get; set; }
    }

    public enum eLazyLoadType
    {
        Default = 0,
        Carousel = 1
    }
}