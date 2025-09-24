//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Threading.Tasks;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Diagnostics;
using AdvantShop.Statistic;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using CsvHelper.Configuration;
using System.Linq;

namespace AdvantShop.ExportImport
{
    public class CsvExportCategories
    {
        private const int MaxCellLength = 60000;
        private readonly string _path;
        private readonly Encoding _encoding;
        private readonly string _delimiter;
        private readonly List<CategoryFields> _fieldMapping;
        private readonly int _categoriesCount;
        private readonly IEnumerable<ExportFeedCsvCategory> _categories;
        private readonly IEnumerable<string> _relatedPropertyNames;
        private readonly IEnumerable<string> _similarPropertyNames;
        private bool _useCommonStatistic;

        private CsvExportCategories(IEnumerable<ExportFeedCsvCategory> categories, string filePath, string delimiter,
            Encoding encoding, List<CategoryFields> fieldMapping,  int categoriesCount, bool useCommonStatistic)
        {
            _path = filePath;
            _encoding = encoding;
            _delimiter = delimiter;
            _fieldMapping = fieldMapping;

            _categories = categories;
            _categoriesCount = categoriesCount;
            _useCommonStatistic = useCommonStatistic;
            _relatedPropertyNames =
                _fieldMapping.Contains(CategoryFields.RelatedProperties)
                    ? _categories.SelectMany(x => x.RelatedPropertyNames).Distinct()
                    : new List<string>();
            _similarPropertyNames =
                _fieldMapping.Contains(CategoryFields.SimilarProperties)
                    ? _categories.SelectMany(x => x.SimilarPropertyNames).Distinct()
                    : new List<string>();
        }

        public static CsvExportCategories Factory(IEnumerable<ExportFeedCsvCategory> categories, string filePath, string delimiter,
            Encoding encoding, List<CategoryFields> fieldMapping, int categoriesCount, bool useCommonStatistic)
        {
            return new CsvExportCategories(categories, filePath, delimiter, encoding, fieldMapping, categoriesCount,
                useCommonStatistic);
        }

        private CsvWriter InitWriter()
        {
            var csvConfiguration = CsvConstants.DefaultCsvConfiguration;
            csvConfiguration.Delimiter = _delimiter;
            var writer = new CsvWriter(new StreamWriter(_path, false, _encoding), csvConfiguration);
            return writer;
        }

        public void SaveCategoriesToCsv()
        {
            using (var writer = InitWriter())
            {
                WriteHeader(writer);

                if (_categories == null) return;

                foreach (ExportFeedCsvCategory category in _categories)
                {                    
                    if (_useCommonStatistic && (!CommonStatistic.IsRun || CommonStatistic.IsBreaking)) return;

                    if (_fieldMapping.Contains(CategoryFields.Description) && category.Description.Length > MaxCellLength)
                    {
                        DoForCommonStatistic(() =>
                        {
                            CommonStatistic.WriteLog(string.Format(LocalizationService.GetResource("Core.ExportImport.ExportCsv.TooLargeDescription"), category.Name, category.CategoryId));
                            CommonStatistic.TotalErrorRow++;
                        });
                        continue;
                    }

                    if (_fieldMapping.Contains(CategoryFields.BriefDescription) && category.BriefDescription.Length > MaxCellLength)
                    {
                        DoForCommonStatistic(() =>
                        {
                            CommonStatistic.WriteLog(string.Format(LocalizationService.GetResource("Core.ExportImport.ExportCsv.TooLargeBriefDescription"), category.Name, category.CategoryId));
                            CommonStatistic.TotalErrorRow++;
                        });
                        continue;
                    }

                    WriteItem(writer, category);
                    DoForCommonStatistic(() => CommonStatistic.RowPosition++);
                }
            }
        }

        private void WriteHeader(IWriter writer)
        {
            foreach (var item in _fieldMapping)
                switch (item)
                {
                    case CategoryFields.RelatedProperties:
                        foreach (var name in _relatedPropertyNames)
                            writer.WriteField($"{item.StrName()}: {name}");
                        break;
                    case CategoryFields.SimilarProperties:
                        foreach (var name in _similarPropertyNames)
                            writer.WriteField($"{item.StrName()}: {name}");
                        break;
                    default:
                        writer.WriteField(item.StrName());
                        break;
                }
            writer.NextRecord();
        }

        private void WriteItem(IWriter writer, ExportFeedCsvCategory model)
        {
            foreach (var item in _fieldMapping)
            {
                if (item == CategoryFields.CategoryId)
                    writer.WriteField(model.CategoryId);

                if (item == CategoryFields.ExternalId)
                    writer.WriteField(model.ExternalId);

                else if (item == CategoryFields.Name)
                    writer.WriteField(model.Name);

                else if (item == CategoryFields.Slug)
                    writer.WriteField(model.Slug);

                else if (item == CategoryFields.ParentCategory)
                    writer.WriteField(model.ParentCategory);

                else if (item == CategoryFields.SortOrder)
                    writer.WriteField(model.SortOrder);

                else if (item == CategoryFields.Enabled)
                    writer.WriteField(model.Enabled);

                else if (item == CategoryFields.Hidden)
                    writer.WriteField(model.Hidden);

                else if (item == CategoryFields.BriefDescription)
                    writer.WriteField(model.BriefDescription);

                else if (item == CategoryFields.Description)
                    writer.WriteField(model.Description);

                else if (item == CategoryFields.DisplayStyle)
                    writer.WriteField(model.DisplayStyle);

                else if (item == CategoryFields.Sorting)
                    writer.WriteField(model.Sorting);

                else if (item == CategoryFields.DisplayBrandsInMenu)
                    writer.WriteField(model.DisplayBrandsInMenu);

                else if (item == CategoryFields.DisplaySubCategoriesInMenu)
                    writer.WriteField(model.DisplaySubCategoriesInMenu);

                else if (item == CategoryFields.Tags)
                    writer.WriteField(model.Tags);

                else if (item == CategoryFields.Picture)
                    writer.WriteField(model.Picture);

                else if (item == CategoryFields.MiniPicture)
                    writer.WriteField(model.MiniPicture);

                else if (item == CategoryFields.Icon)
                    writer.WriteField(model.Icon);

                else if (item == CategoryFields.Title)
                    writer.WriteField(model.Title);

                else if (item == CategoryFields.H1)
                    writer.WriteField(model.H1);

                else if (item == CategoryFields.MetaKeywords)
                    writer.WriteField(model.MetaKeywords);

                else if (item == CategoryFields.MetaDescription)
                    writer.WriteField(model.MetaDescription);

                else if (item == CategoryFields.PropertyGroups)
                    writer.WriteField(model.PropertyGroups);

                else if (item == CategoryFields.ShowOnMainPage)
                    writer.WriteField(model.ShowOnMainPage);

                else if (item == CategoryFields.RelatedCategories)
                    writer.WriteField(model.RelatedCategories);

                else if (item == CategoryFields.SimilarCategories)
                    writer.WriteField(model.SimilarCategories);

                else if (item == CategoryFields.RelatedProperties)
                    foreach (var name in _relatedPropertyNames)
                        if (model.RelatedProperties.TryGetValue(name, out string value))
                            writer.WriteField(value);
                        else
                            writer.WriteField(null);

                else if (item == CategoryFields.SimilarProperties)
                    foreach (var name in _similarPropertyNames)
                        if (model.SimilarProperties.TryGetValue(name, out string value))
                            writer.WriteField(value);
                        else
                            writer.WriteField(null);

                else if (item == CategoryFields.SizeChart)
                    writer.WriteField(model.SizeChart);
            }

            writer.NextRecord();
        }

        public void Process()
        {
            try
            {
                DoForCommonStatistic(() => CommonStatistic.TotalRow = _categoriesCount);
                SaveCategoriesToCsv();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                DoForCommonStatistic(() => CommonStatistic.WriteLog(ex.Message));
            }
            finally
            {
            }
        }

        public Task<bool> ProcessThroughACommonStatistic(string currentProcess, string currentProcessName)
        {
            var ctx = HttpContext.Current;
            return CommonStatistic.StartNew(() =>
                {
                    if (ctx != null)
                        HttpContext.Current = ctx;

                    _useCommonStatistic = true;
                    Process();
                },
                currentProcess,
                currentProcessName);
        }

        protected void DoForCommonStatistic(Action commonStatisticAction)
        {
            if (_useCommonStatistic)
                commonStatisticAction();
        }
    }
}