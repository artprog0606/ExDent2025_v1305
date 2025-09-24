using System.Collections.Generic;

namespace AdvantShop.ExportImport
{
    public interface IExportFeed
    {
        string Export();
        object GetAdvancedSettings();
        void SetDefaultSettings();
        List<string> GetAvailableFileExtentions();
    }
}