using System.IO;
using System.Linq;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ExportCategories
{
    public class StartingExportCategoriesHandler
    {
        private readonly string _fileName = "export_categories";
        private readonly string _fileExtention = ".csv";

        public string Execute(bool useCommonStatistic)
        {
            //delete old
            foreach (var item in Directory.GetFiles(FoldersHelper.GetPathAbsolut(FolderType.PriceTemp)).Where(f => f.Contains(_fileName)))
            {
                FileHelpers.DeleteFile(item);
            }

            CsvExportCategories.Factory(
                ExportFeedCsvCategoryService.GetCsvCategories(ExportFeedCsvCategorySettings.FieldMapping, ExportFeedCsvCategorySettings.PropertySeparator, ExportFeedCsvCategorySettings.NameSameProductProperty, ExportFeedCsvCategorySettings.NameNotSameProductProperty),
                string.Format("{0}{1}.csv", SettingsGeneral.AbsolutePath,
                    FoldersHelper.GetPathRelative(FolderType.PriceTemp, _fileName, false)),
                ExportFeedCsvCategorySettings.CsvSeparator != "custom"
                    ? ExportFeedCsvCategorySettings.CsvSeparator
                    : ExportFeedCsvCategorySettings.CsvSeparatorCustom,
                Encoding.GetEncoding(ExportFeedCsvCategorySettings.CsvEnconing),
                ExportFeedCsvCategorySettings.FieldMapping,
                ExportFeedCsvCategoryService.GetCsvCategoriesCount(),
                useCommonStatistic
            ).Process();

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Categories_ExportCategories);

            return UrlService.GetAbsoluteLink(FoldersHelper.PhotoFoldersPath[FolderType.PriceTemp] + _fileName + _fileExtention);
        }
    }
}
