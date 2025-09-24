using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public class StartImportCategoriesHandler : BaseStartImportHandler
    {
        public new readonly CsvCategoriesFieldsMapping FieldMapping;
        private readonly string _propertySeparator;
        private readonly string _nameSameProductProperty;
        private readonly string _nameNotSameProductProperty;

        public StartImportCategoriesHandler(ImportCategoriesModel model, string inputFilePath) : base(model, inputFilePath)
        {
            FieldMapping = new CsvCategoriesFieldsMapping();
            for (int i = 0; i < model.SelectedFields.Count; i++)
                FieldMapping.AddField(model.SelectedFields[i], i);
            _propertySeparator = model.PropertySeparator;
            _nameSameProductProperty = model.NameSameProductProperty;
            _nameNotSameProductProperty = model.NameNotSameProductProperty;
        }

        protected override void ValidateData()
        {
            if (!FieldMapping.ContainsKey(CategoryFields.CategoryId.StrName()) && 
                !FieldMapping.ContainsKey(CategoryFields.ExternalId.StrName()) && 
                !FieldMapping.ContainsKey(CategoryFields.Name.StrName()))
            {
                throw new BlException(LocalizationService.GetResource("Admin.ImportCategories.Errors.FieldsRequired"));
            }
        }

        protected override void Handle()
        {
            var importCategories = new CsvImportCategories(InputFilePath, HaveHeader, ColumnSeparator, Encoding, FieldMapping, _propertySeparator, _nameSameProductProperty, _nameNotSameProductProperty);
            
            importCategories.ProcessThroughACommonStatistic(
                "import?importTab=importCategories", LocalizationService.GetResource("Admin.ImportCategories.ProcessName"));

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Categories_ImportCategories);
        }
    }
}
