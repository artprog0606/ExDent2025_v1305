using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Repository.Currencies;
using AdvantShop.SEO;
using AdvantShop.Web.Admin.Models.Catalog.Products;
using AdvantShop.Web.Infrastructure.Extensions;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class GetProduct
    {
        private readonly int _productId;

        public GetProduct(int productId)
        {
            _productId = productId;
        }

        public AdminProductModel Execute()
        {
            var product = ProductService.GetProduct(_productId);
            if (product == null)
                return null;

            var model = new AdminProductModel()
            {
                ProductId = product.ProductId,
                Name = product.Name,
                ArtNo = product.ArtNo,
                Enabled = product.Enabled,
                Hidden = product.Hidden,
                UrlPath = product.UrlPath,

                BestSeller = product.BestSeller,
                Recomended = product.Recomended,
                New = product.New,
                Sales = product.OnSale,
                
                CurrencyId = product.CurrencyID,
                DiscountPercent = product.Discount.Percent,
                DiscountAmount = product.Discount.Amount,
                DiscountType = product.Discount.Type,
                DoNotApplyOtherDiscounts = product.DoNotApplyOtherDiscounts,
                AllowPreOrder = product.AllowPreOrder,

                SizeChartId = product.SizeChart?.Id,
                UnitId = product.UnitId,
                Unit = product.Unit?.Name,
                MinAmount = product.MinAmount,
                MaxAmount = product.MaxAmount,
                Multiplicity = product.Multiplicity,
                ShippingPrice = product.ShippingPrice,

                Photo = product.Photo,
                BriefDescription = product.BriefDescription,
                Description = product.Description,

                BrandId = product.BrandId,
                Brand = product.Brand,

                ReviewsCount = ReviewService.GetReviewsCount(product.ProductId, EntityType.Product),
                AccrueBonuses = product.AccrueBonuses,

                TaxId = product.TaxId,
                PaymentSubjectType = product.PaymentSubjectType,
                PaymentMethodType = product.PaymentMethodType,

                Ratio = product.Ratio,
                ManualRatio = product.ManualRatio,

                Comment = product.Comment,
                
                DownloadLink = product.DownloadLink,
                IsDigital = product.IsDigital
            };

            var currency = model.Currencies.Find(x => x.Value == model.CurrencyId.ToString());
            if (currency != null)
                currency.Selected = true;

            model.Currency = CurrencyService.GetAllCurrencies(true).Find(x => x.CurrencyId == product.CurrencyID) ??
                             CurrencyService.CurrentCurrency;

            var meta = MetaInfoService.GetMetaInfo(product.ProductId, MetaType.Product);
            if (meta == null)
            {
                model.DefaultMeta = true;
            }
            else
            {
                model.DefaultMeta = false;
                model.SeoTitle = meta.Title;
                model.SeoH1 = meta.H1;
                model.SeoKeywords = meta.MetaKeywords;
                model.SeoDescription = meta.MetaDescription;
            }

            //var options = CustomOptionsService.GetCustomOptionsByProductId(product.ProductId);
            //model.HasCustomOptions = options != null && options.Count > 0;

            if (product.CategoryId != 0 && product.CategoryId != CategoryService.DefaultNonCategoryId)
            {
                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

                model.BreadCrumbs =
                    CategoryService.GetParentCategories(product.CategoryId)
                        .Select(x => new BreadCrumbs(x.Name, urlHelper.AbsoluteActionUrl("Index", "Catalog", new { categoryId = x.CategoryId })))
                        .Reverse()
                        .ToList();
            }

            // modules
            foreach (var module in AttachedModules.GetModules<IAdminProductTabs>())
            {
                var classInstance = (IAdminProductTabs)Activator.CreateInstance(module);
                model.TabModules.AddRange(classInstance.GetAdminProductTabs(model.ProductId));
            }

            // landing
            //model.IsLandingEnabled = SettingsLandingPage.ActiveLandingPage;
            //model.LandingProductDescription = ProductLandingPageService.GetDescriptionByProductId(model.ProductId, true);

            //var path = HostingEnvironment.MapPath("~/Funnels/templates/");
            //if (Directory.Exists(path))
            //    model.LandingLinks.AddRange(Directory.GetDirectories(path).Select(p => new DirectoryInfo(p).Name));


            var channels = SalesChannelService.GetList();
            model.AllSalesChannelCount = channels != null ? channels.Count : 0;
            var productChannels = SalesChannelService.GetExcludedProductSalesChannelList(model.ProductId);
            model.ProductSalesChannelCount = productChannels != null ? model.AllSalesChannelCount - productChannels.Count : 0;
            
            model.ShowImageSearchEnabled = SettingsCatalog.ShowImageSearchEnabled;
            if (!model.ShowImageSearchEnabled)
                model.ShowGoogleImageSearch = AttachedModules.GetModules<IPhotoSearcher>().Any(x => x.Name == "GoogleImagesSearchModule");

            model.HasFunnels = new LpSiteService().GetList().Where(x => x.Template == LpFunnelType.ProductCrossSellDownSell.ToString()).Any();


            var productOffers = OfferService.GetProductOffers(product.ProductId);
            
            var offerMain = productOffers.FirstOrDefault(x => x.Main);
            if (offerMain != null)
            {
                model.Weight = offerMain.GetWeight();
                model.Length = offerMain.GetLength();
                model.Height = offerMain.GetHeight();
                model.Width = offerMain.GetWidth();
                model.BarCode = offerMain.BarCode;
            }

            if (productOffers.Select(x => x.BarCode).Distinct().Count() > 1)
                model.UseOfferBarCode = true;

            if (productOffers.Select(x => x.GetWeight()).Distinct().Count() > 1 ||
                productOffers.Select(x => x.GetDimensions()).Distinct().Count() > 1)
                model.UseOfferWeightAndDimensions = true;

            model.SortPopular =
                SQLDataAccess.ExecuteScalar<int>(
                    "Select SortPopular From Catalog.Product Where ProductId = @productId", CommandType.Text,
                    new SqlParameter("@productId", product.ProductId));

            model.PriceRules = JsonConvert.SerializeObject(PriceRuleService.GetList().Select(x => x.Name).ToList()) ?? "null";
            model.Comment = product.Comment;
            model.ShowMarketplaceProductButton = ModulesExecuter.ShowMarketplaceProductButton(product.ProductId, true);
            model.AvailableWarehouses = Saas.SaasDataService.IsEnabledFeature(saas => saas.HasWarehouses);

            return model;
        }
    }
}
