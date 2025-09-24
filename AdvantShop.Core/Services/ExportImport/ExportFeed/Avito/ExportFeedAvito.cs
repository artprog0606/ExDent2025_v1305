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
using System.Web;
using System.Xml;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Diagnostics;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.ExportImport
{
    [ExportFeedKey("Avito")]
    public class ExportFeedAvito : BaseExportFeed<ExportFeedAvitoOptions>
    {
        private readonly ExportCsvProductsService<ExportFeedAvitoProduct, ExportFeedAvitoOptions> _exportService;
        private readonly ExportFeedAvitoOptions _options;
        private readonly List<EAvitoCommonTags> _avitoCommonTags;
        private readonly NumberFormatInfo _nfi;

        public ExportFeedAvito(int exportFeedId) : this(exportFeedId, false) { }

        public ExportFeedAvito(int exportFeedId, bool useCommonStatistic) : base(exportFeedId, useCommonStatistic)
        {
            _exportService = new ExportCsvProductsService<ExportFeedAvitoProduct, ExportFeedAvitoOptions>(exportFeedId, Settings);
            _options = Settings?.AdvancedSettings;
            _avitoCommonTags = Enum.GetValues(typeof(EAvitoCommonTags)).Cast<EAvitoCommonTags>().ToList();
            
            _nfi = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            _nfi.NumberDecimalSeparator = ".";
        }

        protected override void Handle()
        {
            var productsCount = _exportService.GetProductsCount();
            CsSetTotalRow(productsCount);

            var products =
                _exportService.GetProducts(productsCount,
                    reader => ExportFeedAvitoService.GetAvitoProductsFromReader(reader, Settings));

            using (var outputFile = new FileStream(Settings.FileFullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                var settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };
                using (var writer = XmlWriter.Create(outputFile, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Ads");
                    writer.WriteAttributeString("formatVersion", "3");
                    writer.WriteAttributeString("target", "Avito.ru");

                    var currency = CurrencyService.GetCurrencyByIso3(_options.Currency);

                    foreach (var product in products)
                    {
                        if (_options.ExportMode == EAvitoExportMode.Product)
                        {
                            ProcessProductRow(product, writer, currency);
                        }
                        else
                        {
                            foreach (var offer in product.Offers)
                            {
                                var productFromOffer = ExportFeedAvitoProduct.Create(product, offer, _options);
                                ProcessProductRow(productFromOffer, writer, currency);
                            }
                        }
                        CsNextRow();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();

                    writer.Flush();
                    writer.Close();
                }
            }
        }

        private void ProcessProductRow(ExportFeedAvitoProduct product, XmlWriter writer, Currency currency)
        {
            writer.WriteStartElement("Ad");

            // common tags
            if (!product.AvitoProperties.Any(x => x.PropertyName == "Id"))
            {
                writer.WriteStartElement("Id");
                writer.WriteRaw(product.ArtNo.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            var date = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id, "Russian Standard Time");

            var productDateBegin = product.AvitoProperties.FirstOrDefault(x => x.PropertyName == EAvitoCommonTags.DateBegin.StrName());
            writer.WriteStartElement("DateBegin");
            writer.WriteRaw(productDateBegin != null ? productDateBegin.PropertyValue : date.AddDays(_options.PublicationDateOffset).ToString("yyyy-MM-dd"));
            writer.WriteEndElement();

            var productDateEnd = product.AvitoProperties.FirstOrDefault(x => x.PropertyName == EAvitoCommonTags.DateEnd.StrName());
            writer.WriteStartElement("DateEnd");
            writer.WriteRaw(productDateEnd != null
                ? productDateEnd.PropertyValue
                : date.AddDays(_options.PublicationDateOffset + _options.DurationOfPublicationInDays)
                    .ToString("yyyy-MM-dd"));
            writer.WriteEndElement();

            var productListingFee = product.AvitoProperties.FirstOrDefault(x => x.PropertyName == EAvitoCommonTags.ListingFee.StrName());
            writer.WriteStartElement("ListingFee");
            writer.WriteRaw(productListingFee != null ? productListingFee.PropertyValue : _options.PaidPublicationOption.StrName());
            writer.WriteEndElement();

            var productAdStatus = product.AvitoProperties.FirstOrDefault(x => x.PropertyName == EAvitoCommonTags.AdStatus.StrName());
            writer.WriteStartElement("AdStatus");
            writer.WriteRaw(productAdStatus != null ? productAdStatus.PropertyValue : _options.PaidServices.StrName());
            writer.WriteEndElement();

            var avitoId = product.AvitoProperties.FirstOrDefault(x => x.PropertyName == EAvitoCommonTags.AvitoId.StrName());
            if (avitoId != null && !string.IsNullOrEmpty(avitoId.PropertyValue))
            {
                writer.WriteStartElement("AvitoId");
                writer.WriteRaw(avitoId.PropertyValue);
                writer.WriteEndElement();
            }

            // contact information
            var productAllowEmail = product.AvitoProperties.FirstOrDefault(x => x.PropertyName == EAvitoCommonTags.AllowEmail.StrName());
            writer.WriteStartElement("AllowEmail");
            writer.WriteRaw(productAllowEmail != null
                ? productAllowEmail.PropertyValue
                : (_options.EmailMessages ? "Да" : "Нет"));
            writer.WriteEndElement();

            var productManagerName = product.AvitoProperties.FirstOrDefault(x => x.PropertyName == EAvitoCommonTags.ManagerName.StrName());
            writer.WriteStartElement("ManagerName");
            writer.WriteRaw(productManagerName != null ? productManagerName.PropertyValue : _options.ManagerName);
            writer.WriteEndElement();

            var productContactPhone = product.AvitoProperties.FirstOrDefault(x => x.PropertyName == EAvitoCommonTags.ContactPhone.StrName());
            writer.WriteStartElement("ContactPhone");
            writer.WriteRaw(productContactPhone != null ? productContactPhone.PropertyValue : _options.Phone);
            writer.WriteEndElement();

            //location
            var productAddress = product.AvitoProperties.FirstOrDefault(x => x.PropertyName == EAvitoCommonTags.Address.StrName());
            writer.WriteStartElement("Address");
            writer.WriteRaw(((productAddress != null ? productAddress.PropertyValue : _options.Address) ?? "").Reduce(256));
            writer.WriteEndElement();

            //Product description Будет отличаться в зависимости от выбранной категории Авито

            var commonProperties =
                product.AvitoProperties.Where(item => !_avitoCommonTags.Any(tag => tag.StrName() == item.PropertyName));
            
            foreach (var productProperty in commonProperties)
            {
                try
                {
                    writer.WriteStartElement(XmlConvert.EncodeName(productProperty.PropertyName.XmlEncode().RemoveInvalidXmlChars()));
                    writer.WriteRaw(productProperty.PropertyValue.XmlEncode().RemoveInvalidXmlChars());
                    writer.WriteEndElement();
                }
                catch (Exception ex)
                {
                    CsRowError(ex.Message);
                    Debug.Log.Error(ex);
                }
            }

            if (!product.AvitoProperties.Any(x => x.PropertyName == "Category") &&
                !string.IsNullOrEmpty(_options.DefaultAvitoCategory))
            {
                writer.WriteStartElement("Category");
                writer.WriteRaw(_options.DefaultAvitoCategory);
                writer.WriteEndElement();
            }

            if (!product.AvitoProperties.Any(x => x.PropertyName == "Title"))
            {
                writer.WriteStartElement("Title");
                writer.WriteRaw(product.Name.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            if (!product.AvitoProperties.Any(x => x.PropertyName == "Description"))
            {
                writer.WriteStartElement("Description");
                var description = string.Empty;

                if (!_options.NotExportColorSize)
                {
                    if (!string.IsNullOrEmpty(product.Colors))
                        description += SettingsCatalog.ColorsHeader + ": " +
                                       product.Colors.Replace(";", ", ").Trim(' ', ',') +
                                       "<br/>";

                    if (!string.IsNullOrEmpty(product.Sizes))
                        description += SettingsCatalog.SizesHeader + ": " +
                                       product.Sizes.Replace(";", ", ").Trim(' ', ',') +
                                       "<br/>";
                }

                description += _options.ProductDescriptionType == "full" ? product.Description : product.BriefDescription;

                if (_options.UnloadProperties && !string.IsNullOrEmpty(product.Properties))
                {
                    foreach (var item in product.Properties.Split(","))
                        description += "<br/>" + item;
                }

                if (_options.IsActiveAboveAdditionalDescription && !string.IsNullOrEmpty(_options.AboveAdditionalDescription))
                    description = $"{_options.AboveAdditionalDescription}<br/>{description}";

                if (_options.IsActiveBelowAdditionalDescription && !string.IsNullOrEmpty(_options.BelowAdditionalDescription))
                    description = $"{description}<br/>{_options.BelowAdditionalDescription}";

                writer.WriteCData(description.RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            if (product.AvitoProperties.Find(x => x.PropertyName == "Price") == null)
            {
                decimal price = 0m;
                float discount = 0;
                
                if (ProductDiscountModels != null)
                {
                    var prodDiscount = ProductDiscountModels.Find(d => d.ProductId == product.ProductId);
                    if (prodDiscount != null)
                        discount = prodDiscount.Discount;
                }
                
                var priceDiscount = discount > 0 && discount > product.Discount
                    ? new Discount(discount, 0)
                    : new Discount(product.Discount, product.DiscountAmount);
                
                if (product.PriceByRule != null)
                {
                    var markup = GetMarkup(product.PriceByRule.Value, product.Currency.Rate);
                
                    price = (decimal) PriceService.RoundPrice(product.PriceByRule.Value + markup, currency, product.Currency.Rate);
                }
                else
                {
                    price = GetPriceWithDiscount(product.Price, priceDiscount, currency, product.Currency.Rate);
                }

                //var newPrice = GetPriceWithDiscount(product.Price, priceDiscount, currency, product.Currency.Rate);

                // выгрузка поля с ценой не обязательна - не выводим цену для товаров, чья итоговая цена 0
                if (price > 0)
                {
                    writer.WriteStartElement("Price");
                    writer.WriteRaw(price.ToString(_nfi)); //Convert.ToInt32(newPrice).ToString());
                    writer.WriteEndElement();
                }
            }

            if (!string.IsNullOrEmpty(product.BarCode))
            {
                writer.WriteStartElement("Barcode");
                writer.WriteRaw(product.BarCode);
                writer.WriteEndElement();
            }

            if (product.PhotosIds != null && product.PhotosIds.Count > 0)
            {
                writer.WriteStartElement("Images");
                foreach (var photoId in product.PhotosIds.Take(10))
                {
                    writer.WriteStartElement("Image");
                    writer.WriteAttributeString("url", SettingsMain.SiteUrl.TrimEnd('/') + "/avito/avitophoto?photoid=" + photoId);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public override void SetDefaultSettings()
        {
            var fileExtension = GetAvailableFileExtentions()[0];
            
            ExportFeedSettingsProvider.SetSettings(
                _exportFeedId, 
                new ExportFeedSettings<ExportFeedAvitoOptions>
                {
                    FileName = ExportFeedService.GetNewExportFileName(_exportFeedId, "export/avito", fileExtension),
                    FileExtention = fileExtension,
                    AdditionalUrlTags = string.Empty,
                    Interval = 1,
                    IntervalType = Core.Scheduler.TimeIntervalType.Days,
                    JobStartTime = new DateTime(2017, 1, 1, 1, 0, 0),
                    Active = false,
                    AdvancedSettings = new ExportFeedAvitoOptions
                    {
                        Currency = ExportFeedAvito.AvailableCurrencies[0],
                        PublicationDateOffset = 0,
                        DurationOfPublicationInDays = 0,
                        PaidPublicationOption = EPaidPublicationOption.Package,
                        PaidServices = EPaidServices.Free,

                        EmailMessages = false,
                        ManagerName = string.Empty,
                        Address = string.Empty,
                        Phone = HttpUtility.HtmlEncode(SettingsMain.Phone)
                    },
                    ExportAdult = true
                });
        }

        public override object GetAdvancedSettings()
        {
            return ExportFeedSettingsProvider.GetAdvancedSettings<ExportFeedAvitoOptions>(_exportFeedId);
        }

        public static List<string> AvailableCurrencies => new List<string> { "RUB", "RUR" };

        public override List<string> GetAvailableVariables()
        {
            return new List<string> { "#STORE_NAME#", "#STORE_URL#", "#PRODUCT_NAME#", "#PRODUCT_ID#", "#PRODUCT_ARTNO#" };
        }

        public override List<string> GetAvailableFileExtentions() => new List<string> { "xml"/*, "yml"*/ };
    }
}