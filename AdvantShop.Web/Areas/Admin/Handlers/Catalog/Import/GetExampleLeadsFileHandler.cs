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
    public class GetExampleLeadsFileHandler
    {
        private readonly string _columnSeparator;
        private readonly string _encoding;
        private readonly string _outputFilePath;

        private readonly string _propertySeparator;
        private readonly string _propertyValueSeparator;

        private Dictionary<ELeadFields, string> _leadsExample;

        public GetExampleLeadsFileHandler(ImportLeadsModel model, string outputFilePath)
        {
            _columnSeparator = model.ColumnSeparator.IsNotEmpty() && model.ColumnSeparator.ToLower() == SeparatorsEnum.Custom.ToString().ToLower()
                ? model.CustomColumnSeparator
                : model.ColumnSeparator;
            _encoding = model.Encoding;
            _outputFilePath = outputFilePath;

            _propertySeparator = model.PropertySeparator;
            _propertyValueSeparator = model.PropertyValueSeparator;
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

            GetLeadExample();
            using (var csvWriter = InitWriter())
            {
                foreach(ELeadFields item in Enum.GetValues(typeof(ELeadFields)))
                {
                    if (item == ELeadFields.None || !_leadsExample.ContainsKey(item))
                        continue;
                    csvWriter.WriteField(item.StrName().ToLower());
                }
                csvWriter.NextRecord();

                foreach (ELeadFields item in Enum.GetValues(typeof(ELeadFields)))
                {
                    if (item == ELeadFields.None || !_leadsExample.ContainsKey(item))
                        continue;
                    csvWriter.WriteField(_leadsExample[item]);
                }
                csvWriter.NextRecord();
            }
            return new { Result = true, };
        }

        private void GetLeadExample()
        {
            var multiOffer = string.Format("1647{0}790{0}1{1}1650{0}800{0}3", _propertyValueSeparator, _propertySeparator);

            _leadsExample = new Dictionary<ELeadFields, string>
            {
                { ELeadFields.Id, "" },
                { ELeadFields.SalesFunnel, "Лиды" },
                { ELeadFields.DealStatus, "Новый" },
                { ELeadFields.ManagerName, "Администратор Магазина" },
                { ELeadFields.Title, "Новый лид" },
                { ELeadFields.Description, "Новый лид из импорта" },
                { ELeadFields.CustomerId, Guid.Empty.ToString() },
                { ELeadFields.FirstName, "Иван" },
                { ELeadFields.LastName, "Иванов" },
                { ELeadFields.Patronymic, "Иванович" },
                { ELeadFields.Organization, "ООО Ромашка" },
                { ELeadFields.Email, "example@example.com" },
                { ELeadFields.Phone, "+79999999999" },
                { ELeadFields.Country, "Россия" },
                { ELeadFields.Region, "Московская область" },
                { ELeadFields.District, "" },
                { ELeadFields.City, "Москва" },
                { ELeadFields.BirthDay, "1980-01-31" },
                { ELeadFields.MultiOffer, multiOffer },
                { ELeadFields.Currency, CurrencyService.CurrentCurrency.Symbol },
                { ELeadFields.Discount, "0" },
                { ELeadFields.DiscountValue, "0" },
                { ELeadFields.ShippingName, "" },
                { ELeadFields.ShippingCost, "0" },
                { ELeadFields.TotalSum, "1590" },
                { ELeadFields.Comment, "Комментарий к лиду" },
                { ELeadFields.CreatedDate, DateTime.Now.ToString() }
            };
        }
    }
}
