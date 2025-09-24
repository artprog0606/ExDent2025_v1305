namespace AdvantShop.Web.Admin.Models.Crm
{
    public class SaveVkSettingsModel
    {
        public int Id { get; set; }
        public bool CreateLeadFromMessages { get; set; }
        public bool CreateLeadFromComments { get; set; }
        public bool SyncOrdersFromVk { get; set; }
    }
}
