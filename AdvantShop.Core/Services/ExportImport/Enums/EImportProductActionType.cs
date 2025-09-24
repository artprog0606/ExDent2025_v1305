using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.ExportImport
{
    public enum EImportProductActionType
    {
        [Localize("Core.ExportImport.ImportCsv.DoNothing")]
        Nothing = 0,
        [Localize("Core.ExportImport.ImportCsv.DeleteProducts")]
        Delete = 1,
        [Localize("Admin.Import.ImportProducts.DisableProducts")]
        Disable = 2,
        [Localize("Core.ExportImport.ImportCsv.ResetToZero")]
        ResetToZero = 3
    }
}