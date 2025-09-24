using System;
using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Saas;

namespace AdvantShop.ViewModel.Compare
{
    public class CompareProductsViewModel
    {
        public CompareProductsViewModel(List<CompareProductModel> products, List<Property> list)
        {
            HidePrice = SettingsCatalog.HidePrice;
            TextInsteadOfPrice = SettingsCatalog.TextInsteadOfPrice;

            DisplayBuyButton = SettingsCatalog.DisplayBuyButton;
            DisplayPreOrderButton = SettingsCatalog.DisplayPreOrderButton;

            BuyButtonText = SettingsCatalog.BuyButtonText;
            PreOrderButtonText = SettingsCatalog.PreOrderButtonText;

            Properties = list ?? new List<Property>();
            Products = new List<CompareProductItem>();

            if (products != null && products.Count > 0)
            {
                var productDiscounts = new List<ProductDiscount>();
                
                var customerGroup = CustomerContext.CurrentCustomer.CustomerGroup;
                var priceRulesActive = !SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.PriceTypes;

                var discountModules = AttachedModules.GetModules<IDiscount>();
                foreach (var discountModule in discountModules)
                {
                    if (discountModule != null)
                    {
                        var classInstance = (IDiscount)Activator.CreateInstance(discountModule);
                        productDiscounts.AddRange(classInstance.GetProductDiscountsList());
                    }
                }

                foreach (var product in products)
                {
                    var discountByDatetime = DiscountByTimeService.GetCurrentDiscount(product.Product.ProductId);
                    
                    Products.Add(new CompareProductItem(product.Brand, product.Product, customerGroup, discountByDatetime, productDiscounts, priceRulesActive));
                }
            }
        }
        
        public bool DisplayBuyButton { get; set; }
        public bool DisplayPreOrderButton { get; set; }

        public string BuyButtonText { get; set; }
        public string PreOrderButtonText { get; set; }

        public List<Property> Properties { get; set; }
        public List<CompareProductItem> Products { get; set; }

        public bool HidePrice { get; set; }
        public string TextInsteadOfPrice { get; set; }
    }

    public class CompareProductModel
    {
        public ProductModel Product { get; set; }
        public CompareBrandModel Brand { get; set; }
    }
    
    public class CompareProductItem : ProductItem
    {
        private List<PropertyValue> _productPropertyValues;
        public List<PropertyValue> ProductPropertyValues =>
            _productPropertyValues ??
            (_productPropertyValues = PropertyService.GetPropertyValuesByProductId(ProductId));

        public CompareBrandModel Brand { get; private set; }

        public CompareProductItem(CompareBrandModel brand, ProductModel product, CustomerGroup customerGroup,
                                    float discountByDatetime, List<ProductDiscount> productDiscounts,
                                    bool priceRulesActive)
                    : base(product, customerGroup, discountByDatetime, productDiscounts, priceRulesActive, null)
        {
            Brand = brand;
        }
    }

    public class CompareBrandModel
    {
        public string Name { get; set; }
        public string UrlPath { get; set; }
    }
}