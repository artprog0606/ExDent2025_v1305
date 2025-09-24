namespace AdvantShop.ExportImport
{
    public interface IExportFeedSettings
    {
        string FileName { get; set; }
        string FileFullName { get; }
        string FileFullPath { get; }
    }
}