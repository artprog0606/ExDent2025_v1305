namespace AdvantShop.ExportImport
{
    public interface IExportFeedCsvFilterOptions : IExportFeedOptions
    {
        bool CsvExportNoInCategory { get; }
        bool ExportFromMainCategories { get; }
    }
}
