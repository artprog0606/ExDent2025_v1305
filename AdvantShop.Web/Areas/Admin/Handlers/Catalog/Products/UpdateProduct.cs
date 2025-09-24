using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.SEO;
using AdvantShop.Web.Admin.Models.Catalog.Products;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class UpdateProduct
    {
        private readonly AdminProductModel _model;

        public UpdateProduct(AdminProductModel model)
        {
            _model = model;
        }

        public bool Execute()
        {
            var product = ProductService.GetProduct(_model.ProductId);
            if (product == null)
                return false;

            product.Name = _model.Name;
            product.ArtNo = _model.ArtNo;
            product.Enabled = _model.Enabled;
            product.UrlPath = _model.UrlPath;

            product.BestSeller = _model.BestSeller;
            product.Recomended = _model.Recomended;
            product.New = _model.New;
            product.OnSale = _model.Sales;

            product.CurrencyID = _model.CurrencyId;
            product.Discount = new Discount(_model.DiscountPercent, _model.DiscountAmount);
            product.DoNotApplyOtherDiscounts = _model.DoNotApplyOtherDiscounts;
            product.AllowPreOrder = _model.AllowPreOrder;

            product.UnitId = _model.UnitId;
            product.MinAmount = _model.MinAmount;
            product.MaxAmount = _model.MaxAmount;
            product.Multiplicity = _model.Multiplicity > 0 ? _model.Multiplicity : 1;
            product.ShippingPrice = _model.ShippingPrice;
            product.TaxId = _model.TaxId;

            product.PaymentMethodType = _model.PaymentMethodType;
            product.PaymentSubjectType = _model.PaymentSubjectType;

            if (BonusSystem.IsActive)
            {
                product.AccrueBonuses = _model.AccrueBonuses;
            }

            product.BriefDescription = _model.BriefDescription == null || _model.BriefDescription == "<br />" ||
                                       _model.BriefDescription == "&nbsp;" || _model.BriefDescription == "\r\n"
                                            ? string.Empty
                                            : _model.BriefDescription;
            product.Description = _model.Description == null || _model.Description == "<br />" ||
                                  _model.Description == "&nbsp;" || _model.Description == "\r\n"
                                            ? string.Empty
                                            : _model.Description;
            
            product.Meta =
                new MetaInfo(0, _model.ProductId, MetaType.Product, _model.SeoTitle.DefaultOrEmpty(),
                    _model.SeoKeywords.DefaultOrEmpty(), _model.SeoDescription.DefaultOrEmpty(),
                    _model.SeoH1.DefaultOrEmpty());

            product.ModifiedBy = CustomerContext.CustomerId.ToString();

            if (_model.Tags != null && _model.Tags.Count > 0)
            {
                var prevTags = TagService.Gets(product.ProductId, ETagType.Product).Select(x => x.Name).ToList();
                if (_model.Tags.Any(x => !prevTags.Contains(x)))
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Products_AddTagToProduct);

                product.Tags = _model.Tags.Select(x => new Tag
                {
                    Name = x,
                    UrlPath = StringHelper.TransformUrl(StringHelper.Translit(x)),
                    Enabled = true,
                    VisibilityForUsers = true
                }).ToList();
            }
            else
            {
                product.Tags = new List<Tag>();
            }

            product.HasMultiOffer = product.Offers != null &&
                                    product.Offers.Count(x => x.ColorID != null || x.SizeID != null) > 0;
            product.ActiveView360 = product.ProductPhotos360 != null && product.ProductPhotos360.Count > 0;

            if (!SettingsCatalog.EnableOfferWeightAndDimensions && product.Offers != null)
            {
                foreach (var offer in product.Offers)
                {
                    offer.Weight = _model.Weight;
                    offer.Length = _model.Length;
                    offer.Height = _model.Height;
                    offer.Width = _model.Width;
                }
            }

            if (!SettingsCatalog.EnableOfferBarCode && product.Offers != null)
            {
                foreach (var offer in product.Offers)
                    offer.BarCode = _model.BarCode;
            }

            product.Comment = _model.Comment;
            product.DownloadLink = _model.DownloadLink;
            product.IsDigital = _model.IsDigital;

            if (_model.SizeChartId.HasValue)
            {
                if (_model.SizeChartId.Value != product.SizeChart?.Id)
                {
                    product.SizeChart = SizeChartService.Get(_model.SizeChartId.Value);
                }
            }
            else
            {
                product.SizeChart = null;
            }

            try
            {
                var productBefore = ProductService.GetProduct(_model.ProductId);
                var offersBefore = productBefore.Offers;
                
                ProductService.UpdateProduct(product, true, true);
                ProductService.SetProductHierarchicallyEnabled(product.ProductId);

                if (CategoryService.IsNeedRecalculateProductsCountInCategories(productBefore, product, offersBefore, product.Offers))
                {
                    CategoryService.RecalculateProductsCountInCategories(product.ProductId);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message, ex);
                return false;
            }

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Products_EditProduct);

            return true;
        }
    }
}
