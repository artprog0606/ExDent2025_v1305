using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper.Configuration;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class GetExampleOrdersFileHandler
    {
        private readonly string _columnSeparator;
        private readonly string _encoding;
        private readonly string _outputFilePath;

        private readonly string _customOptionOptionsSeparator;

        private Dictionary<EOrderFields, string> _ordersExample;

        public GetExampleOrdersFileHandler(ImportOrdersModel model, string outputFilePath)
        {
            _columnSeparator = model.ColumnSeparator.IsNotEmpty() && model.ColumnSeparator.ToLower() == SeparatorsEnum.Custom.ToString().ToLower()
                ? model.CustomColumnSeparator
                : model.ColumnSeparator;
            _encoding = model.Encoding;
            _outputFilePath = outputFilePath;

            _customOptionOptionsSeparator = model.CustomOptionOptionsSeparator;
        }

        private CsvWriter InitWriter()
        {
            var csvConfiguration = CsvConstants.DefaultCsvConfiguration;
            csvConfiguration.Delimiter = _columnSeparator;
            var streamWriter = new StreamWriter(_outputFilePath, false, System.Text.Encoding.GetEncoding(_encoding));
            var writer = new CsvWriter(streamWriter, csvConfiguration);
            return writer;
        }

        public object Execute()
        {
            if (_columnSeparator.IsNullOrEmpty())
                throw new BlException(LocalizationService.GetResource("Admin.Import.Errors.NotColumnSeparator"));

            if (File.Exists(_outputFilePath))
                File.Delete(_outputFilePath);

            GetOrderExample();
            using (var csvWriter = InitWriter())
            {
                foreach(EOrderFields item in Enum.GetValues(typeof(EOrderFields)))
                {
                    if (item == EOrderFields.None || !_ordersExample.ContainsKey(item))
                        continue;
                    if (item == EOrderFields.OrderItemCustomOptions)
                        csvWriter.WriteField(item.StrName().ToLower() + ":Комплект");
                    else
                        csvWriter.WriteField(item.StrName().ToLower());
                }
                csvWriter.NextRecord();

                foreach (EOrderFields item in Enum.GetValues(typeof(EOrderFields)))
                {
                    if (item == EOrderFields.None || !_ordersExample.ContainsKey(item))
                        continue;
                    csvWriter.WriteField(_ordersExample[item]);
                }
                csvWriter.NextRecord();
            }
            return new { Result = true, };
        }

        private void GetOrderExample()
        {
            _ordersExample = new Dictionary<EOrderFields, string>
            {
                { EOrderFields.Number, "1" },
                { EOrderFields.OrderStatus, "Новый" },
                { EOrderFields.OrderSource, "Корзина интернет магазина" },
                { EOrderFields.OrderDateTime, DateTime.Now.ToString() },
                { EOrderFields.FullName, "Иванов Иван Иванович" },
                { EOrderFields.Email, "example@example.com" },
                { EOrderFields.Phone, "+79999999999" },
                { EOrderFields.CustomerGroup, "Обычный покупатель" },
                { EOrderFields.RecipientFullName, "Иванов Иван Иванович" },
                { EOrderFields.RecipientPhone, "+79999999999" },
                { EOrderFields.Weight, "2" },
                { EOrderFields.Dimensions, "10x10x10" },
                { EOrderFields.Payed, "Да" },
                { EOrderFields.DiscountValue, "0" },
                { EOrderFields.ShippingCost, "0" },
                { EOrderFields.PaymentCost, "0" },
                { EOrderFields.Coupon, "0" },
                { EOrderFields.BonusCost, "0" },
                { EOrderFields.Currency, CurrencyService.CurrentCurrency.Symbol },
                { EOrderFields.ShippingName, "Самовывоз" },
                { EOrderFields.PaymentName, "Картой" },
                { EOrderFields.Country, "Россия" },
                { EOrderFields.Region, "Московская область" },
                { EOrderFields.City, "Москва" },
                { EOrderFields.District, "Внуково" },
                { EOrderFields.Street, "Ленина" },
                { EOrderFields.House, "1" },
                { EOrderFields.Structure, string.Empty },
                { EOrderFields.Apartment, string.Empty },
                { EOrderFields.Entrance, string.Empty },
                { EOrderFields.Floor, string.Empty },
                { EOrderFields.DeliveryDate, DateTime.Now.ToString("d") },
                { EOrderFields.DeliveryTime, DateTime.Now.ToString("t") },
                { EOrderFields.CustomerComment, string.Empty },
                { EOrderFields.AdminComment, string.Empty },
                { EOrderFields.StatusComment, string.Empty },
                { EOrderFields.Manager, "Петров Петр" },
                { EOrderFields.CouponCode, string.Empty },
                { EOrderFields.OrderItemArtNo, "1647" },
                { EOrderFields.OrderItemName, "Футболка" },
                { EOrderFields.OrderItemCustomOptions, $"Вешалка{_customOptionOptionsSeparator}Браслет" },
                { EOrderFields.OrderItemSize, "XS" },
                { EOrderFields.OrderItemColor, "Белый" },
                { EOrderFields.OrderItemPrice, "1000" },
                { EOrderFields.OrderItemAmount, "1" },
                { EOrderFields.GoogleClientId, string.Empty },
                { EOrderFields.YandexClientId, string.Empty },
                { EOrderFields.Referral, string.Empty },
                { EOrderFields.LoginPage, string.Empty },
                { EOrderFields.UtmSource, string.Empty },
                { EOrderFields.UtmMedium, string.Empty },
                { EOrderFields.UtmCampaign, string.Empty },
                { EOrderFields.UtmContent, string.Empty },
                { EOrderFields.UtmTerm, string.Empty },
                { EOrderFields.Sum, "1000" },
            };
        }
    }
}
