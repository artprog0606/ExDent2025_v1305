//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.ExportImport
{
    [ExportFeedKey("GoogleMerchentCenter")]
    public class ExportFeedGoogleMerchantCenter : BaseExportFeed<ExportFeedGoogleMerchantCenterOptions>
    {
        private const string GoogleBaseNamespace = "http://base.google.com/ns/1.0";

        private readonly ExportProductsService<ExportFeedGoogleMerchantCenterProduct, ExportFeedGoogleMerchantCenterOptions> _exportService;
        private readonly ExportFeedGoogleMerchantCenterOptions _options;

        private Currency _currency;

        public ExportFeedGoogleMerchantCenter(int exportFeedId) : this(exportFeedId, false) { }

        public ExportFeedGoogleMerchantCenter(int exportFeedId, bool useCommonStatistic) : base(exportFeedId, useCommonStatistic)
        {
            _exportService = new ExportProductsService<ExportFeedGoogleMerchantCenterProduct, ExportFeedGoogleMerchantCenterOptions>(exportFeedId, Settings);
            _options = Settings?.AdvancedSettings;
        }

        protected override void Handle()
        {
            _currency = CurrencyService.GetCurrencyByIso3(_options.Currency);

            var productsCount = _exportService.GetProductsCount();
            CsSetTotalRow(productsCount);

            var products = _exportService.GetProducts(productsCount);

            using (var stream = new FileStream(Settings.FileFullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true }))
                {
                    // source http://www.google.com/support/merchants/bin/answer.py?answer=188494&expand=GB
                    writer.WriteStartDocument();

                    writer.WriteStartElement("rss");
                    writer.WriteAttributeString("version", "2.0");
                    writer.WriteAttributeString("xmlns", "g", null, GoogleBaseNamespace);
                    writer.WriteStartElement("channel");
                    writer.WriteElementString("title", _options.DatafeedTitle.Replace("#STORE_NAME#", SettingsMain.ShopName));
                    writer.WriteElementString("link", SettingsMain.SiteUrl);
                    writer.WriteElementString("description", _options.DatafeedDescription.Replace("#STORE_NAME#", SettingsMain.ShopName));

                    foreach (var productRow in products)
                    {
                        ProcessProductRow(productRow, writer);
                        CsNextRow();
                    };

                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
        }

        private void ProcessProductRow(ExportFeedGoogleMerchantCenterProduct row, XmlWriter writer)
        {
            writer.WriteStartElement("item");

            #region Основные сведения о товарах

            switch (_options.OfferIdType)
            {
                case "id":
                    writer.WriteElementString("g", "id", GoogleBaseNamespace, row.OfferId.ToString(CultureInfo.InvariantCulture));
                    break;

                case "artno":
                    writer.WriteElementString("g", "id", GoogleBaseNamespace, row.OfferArtNo.ToString(CultureInfo.InvariantCulture));
                    break;

                default:
                    writer.WriteElementString("g", "id", GoogleBaseNamespace, row.OfferId.ToString(CultureInfo.InvariantCulture));
                    break;
            }

            //title [title]
            writer.WriteStartElement("title");
            var title = row.Name +
                (_options.ColorSizeToName && !row.SizeName.IsNullOrEmpty() ? " " + row.SizeName : string.Empty) +
                (_options.ColorSizeToName && !row.ColorName.IsNullOrEmpty() ? " " + row.ColorName : string.Empty);
            //title should be not longer than 150 characters
            if (title.Length > 150)
                title = title.Substring(0, 150);
            writer.WriteCData(title);
            writer.WriteEndElement();

            //description

            //if (!string.Equals(_options.ProductDescriptionType, "none"))
            //{
            // Sckeef: Согласно инструкции, описание обязательно для товара 05/12/18
            var desc = _options.ProductDescriptionType == "full" && _options.ProductDescriptionType != "none"
                ? row.Description
                : row.BriefDescription;
            if (_options.RemoveHtml)
                desc = StringHelper.RemoveHTML(desc);

            if (desc.IsNotEmpty())
            {
                writer.WriteStartElement("g", "description", GoogleBaseNamespace);
                writer.WriteCData(desc.RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }
            //}
            //else
            //{
            //    writer.WriteStartElement("g", "description", GoogleBaseNamespace);
            //    writer.WriteEndElement();
            //}

            //google_product_category http://www.google.com/support/merchants/bin/answer.py?answer=160081
            var googleProductCategory = row.GoogleProductCategory.Default(_options.GoogleProductCategory);
            if (!googleProductCategory.IsNullOrEmpty())
            {
                writer.WriteStartElement("g", "google_product_category", GoogleBaseNamespace);
                writer.WriteCData(googleProductCategory);
                writer.WriteEndElement();
            }

            //product_type
            var localPath = string.Empty;
            var cats = CategoryService.GetParentCategories(row.ParentCategory)
                .Where(x => x.CategoryId != 0)
                .Reverse()
                .ToList();

            for (var i = 0; i < cats.Count; i++)
            {
                localPath += cats[i].Name;
                if (i == cats.Count - 1) 
                    continue;                
                localPath += " > ";
            }
            writer.WriteStartElement("g", "product_type", GoogleBaseNamespace);
            writer.WriteCData(localPath);
            writer.WriteEndElement();

            if (row.Adult)
                writer.WriteElementString("g", "adult", GoogleBaseNamespace, row.Adult.ToString());

            //link
            writer.WriteElementString("link", CreateLink(row));

            //image link
            if (!string.IsNullOrEmpty(row.Photos))
            {
                var temp = row.Photos.Split(',');
                for (var i = 0; i < temp.Length && i < 11; i++)
                    writer.WriteElementString("g", i == 0 ? "image_link" : "additional_image_link", GoogleBaseNamespace, GetImageProductPath(temp[i]));
            }

            //condition
            writer.WriteElementString("g", "condition", GoogleBaseNamespace, "new");

            #endregion Основные сведения о товарах

            #region наличие и цена

            //availability
            var availability = "in stock";
            if (row.Amount <= 0)
                availability = _options.AllowPreOrderProducts && row.AllowPreorder
                    ? "preorder"
                    : "out_of_stock";

            writer.WriteElementString("g", "availability", GoogleBaseNamespace, availability);

            if ((availability == "preorder" || availability == "backorder") 
                && row.GoogleAvailabilityDate != null)
            {
                writer.WriteElementString("g", "availability_date", GoogleBaseNamespace, 
                                            row.GoogleAvailabilityDate.Value.ToString("yyyy-MM-ddTHH:mm"));
            }

            float discount = 0;
            if (ProductDiscountModels != null)
            {
                var prodDiscount = ProductDiscountModels.Find(d => d.ProductId == row.ProductId);
                if (prodDiscount != null)
                {
                    discount = prodDiscount.Discount;
                }
            }

            var markupOldPrice = GetMarkup(row.Price, row.CurrencyValue);

            var price = PriceService.GetFinalPrice(row.Price + markupOldPrice, new Discount(), row.CurrencyValue, _currency);
            writer.WriteElementString("g", "price", GoogleBaseNamespace, price + " " + _options.Currency);

            var priceDiscount = discount > 0 && discount > row.Discount
                ? new Discount(discount, 0)
                : new Discount(row.Discount, row.DiscountAmount);
            
            if (priceDiscount.HasValue)
            {
                var newPrice = GetPriceWithDiscount(row.Price, priceDiscount, _currency, row.CurrencyValue);                
                writer.WriteElementString("g", "sale_price", GoogleBaseNamespace, newPrice + " " + _options.Currency);
            }

            #endregion наличие и цена

            #region Уникальные идентификаторы товаров

            var gtin = row.Gtin;
            if (!string.IsNullOrEmpty(gtin))
            {
                writer.WriteElementString("g", "gtin", GoogleBaseNamespace, gtin);
            }

            if (!string.IsNullOrEmpty(row.BrandName))
            {
                writer.WriteStartElement("g", "brand", GoogleBaseNamespace);
                writer.WriteCData(row.BrandName);
                writer.WriteFullEndElement(); // g:brand
            }

            if (!string.IsNullOrEmpty(row.Mpn))
            {
                writer.WriteElementString("g", "mpn", GoogleBaseNamespace, row.Mpn);
            }

            #endregion Уникальные идентификаторы товаров

            #region Варианты товара

            if (row.ColorName.IsNotEmpty() || row.SizeName.IsNotEmpty())
            {
                //item_group_id
                writer.WriteElementString("g", "item_group_id", GoogleBaseNamespace, row.ProductId.ToString());
                //color
                if (row.ColorName.IsNotEmpty())
                {
                    writer.WriteElementString("g", "color", GoogleBaseNamespace, row.ColorName);
                }
                //size
                if (row.SizeName.IsNotEmpty())
                {
                    writer.WriteElementString("g", "size", GoogleBaseNamespace, row.SizeName);
                }
            }

            #endregion Варианты товара

            writer.WriteElementString("g", "expiration_date", GoogleBaseNamespace, DateTime.Now.AddDays(28).ToString("yyyy-MM-dd"));
            writer.WriteEndElement();
        }

        private string CreateLink(ExportFeedGoogleMerchantCenterProduct row)
        {
            var queryParams = new List<string>();
            if (!row.Main)
            {
                if (row.ColorId != 0)
                    queryParams.Add($"color={row.ColorId}");
                if (row.SizeId != 0)
                    queryParams.Add($"size={row.SizeId}");
            }

            var urlTags = GetAdditionalUrlTags(row);
            if (!string.IsNullOrEmpty(urlTags))
                queryParams.Add(urlTags);

            return SettingsMain.SiteUrl + "/" + UrlService.GetLink(ParamType.Product, row.UrlPath, row.ProductId, string.Join("&", queryParams));
        }

        public override void SetDefaultSettings()
        {
            var fileExtension = GetAvailableFileExtentions()[0];
            
            ExportFeedSettingsProvider.SetSettings(_exportFeedId, 
                new ExportFeedSettings<ExportFeedGoogleMerchantCenterOptions>
                {
                    FileName = ExportFeedService.GetNewExportFileName(_exportFeedId, "export/google", fileExtension),
                    FileExtention = fileExtension,
                    Interval = 1,
                    IntervalType = Core.Scheduler.TimeIntervalType.Hours,
                    Active = false,
                    AdditionalUrlTags = string.Empty,
                    AdvancedSettings = new ExportFeedGoogleMerchantCenterOptions
                    {
                        ProductDescriptionType = "short",
                        DatafeedTitle = "#STORE_NAME#",
                        DatafeedDescription = "#STORE_NAME#",
                        Currency = CurrencyService.BaseCurrency.Iso3,
                        RemoveHtml = true,
                        OfferIdType = "id"
                    },
                    ExportAdult = true
                });
        }

        public override object GetAdvancedSettings()
        {
            return ExportFeedSettingsProvider.GetAdvancedSettings<ExportFeedGoogleMerchantCenterOptions>(_exportFeedId);
        }

        public override List<string> GetAvailableVariables()
        {
            return new List<string> { "#STORE_NAME#", "#STORE_URL#", "#PRODUCT_NAME#", "#PRODUCT_ID#", "#PRODUCT_ARTNO#" };
        }

        public override List<string> GetAvailableFileExtentions() => new List<string> { "xml" };
    }
}