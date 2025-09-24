namespace AdvantShop.Web.Admin.Models.Modules
{
    public class PreviewModuleModel
    {
        public string ModuleName { get; set; }
        public string ModuleStringId { get; set; }
        public int ModuleId { get; set; }
        public string ModuleVersion { get; set; }
        public string PreviewRightText { get; set; }
        public string PreviewLeftText { get; set; }
        public bool ShowInstalledAndPreview { get; set; }
        public string PreviewButtonText { get; set; }
        public string PriceString { get; set; }
    }
}