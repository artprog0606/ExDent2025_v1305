using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;
using CsvHelper;
using CsvHelper.Configuration;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class GetExampleBrandsFileHandler
    {
        private readonly string _columnSeparator;
        private readonly string _encoding;
        private readonly string _outputFilePath;
        private Dictionary<EBrandFields, string> _brandsExample;

        public GetExampleBrandsFileHandler(ImportBrandsModel model, string outputFilePath)
        {
            _columnSeparator = model.ColumnSeparator.IsNullOrEmpty() &&
                               string.Equals(model.ColumnSeparator, SeparatorsEnum.Custom.ToString(),
                                   StringComparison.InvariantCultureIgnoreCase)
                ? model.CustomColumnSeparator
                : model.ColumnSeparator;
            _encoding = model.Encoding;
            _outputFilePath = outputFilePath;
        }

        private CsvWriter InitWriter()
        {
            var csvConfiguration = CsvConstants.DefaultCsvConfiguration;
            csvConfiguration.Delimiter = _columnSeparator;
            var streamWriter = new StreamWriter(_outputFilePath, false, Encoding.GetEncoding(_encoding));
            var writer = new CsvWriter(streamWriter, csvConfiguration);
            return writer;
        }

        public void Execute()
        {
            if (_columnSeparator.IsNullOrEmpty())
                throw new BlException(LocalizationService.GetResource("Admin.Import.Errors.NotColumnSeparator"));

            if (File.Exists(_outputFilePath))
                File.Delete(_outputFilePath);

            GetBrandExample();
            using (var csv = InitWriter())
            {
                foreach (EBrandFields item in Enum.GetValues(typeof(EBrandFields)))
                {
                    if (item == EBrandFields.None || !_brandsExample.ContainsKey(item))
                        continue;
                    csv.WriteField(item.StrName().ToLower());
                }

                csv.NextRecord();

                foreach (EBrandFields item in Enum.GetValues(typeof(EBrandFields)))
                {
                    if (item == EBrandFields.None || !_brandsExample.ContainsKey(item))
                        continue;
                    csv.WriteField(_brandsExample[item]);
                }

                csv.NextRecord();
            }
        }

        private void GetBrandExample()
        {
            _brandsExample = new Dictionary<EBrandFields, string>
            {
                {EBrandFields.Name, "Apple"},
                {
                    EBrandFields.Description,
                    @"<p>Apple Inc. &mdash; американская корпорация, производитель персональных и планшетных компьютеров, аудиоплееров, телефонов, программного обеспечения. Один из пионеров в области персональных компьютеров и современных многозадачных операционных систем с графическим интерфейсом.<br /><br />Штаб-квартира &mdash; в Купертино, штат Калифорния. Благодаря инновационным технологиям и эстетичному дизайну, корпорация Apple создала уникальную репутацию, сравнимую с культом, в индустрии потребительской электроники. В мае 2011 года торговая марка Apple была признана самым дорогим брендом в мире (c оценкой в $153,3 млрд) в рейтинге международного исследовательского агентства Millward Brown.</p>"
                },
                {
                    EBrandFields.BriefDescription,
                    @"<p>Apple Inc. &mdash; американская корпорация, производитель персональных и планшетных компьютеров, аудиоплееров, телефонов, программного обеспечения. Один из пионеров в области персональных компьютеров и современных многозадачных операционных систем с графическим интерфейсом. Штаб-квартира &mdash; в Купертино, штат Калифорния.</p>"
                },
                {EBrandFields.Enabled, "True"},
                {EBrandFields.UrlPath, "apple"},
                {EBrandFields.BrandSiteUrl, "apple.com"},
                {EBrandFields.CountryId, "196"},
                {EBrandFields.CountryName, "США"},
                {EBrandFields.CountryOfManufactureId, "196"},
                {EBrandFields.CountryOfManufactureName, "США"},
                {EBrandFields.Photo, "https://www.apple.com/ac/globalnav/5/ru_RU/images/globalnav/apple/image_large.svg"}
            };
        }
    }
}
