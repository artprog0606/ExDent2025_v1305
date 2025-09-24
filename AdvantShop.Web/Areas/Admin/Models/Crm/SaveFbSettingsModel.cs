namespace AdvantShop.Web.Admin.Models.Crm
{
    public class SaveFbSettingsModel
    {
        public int Id { get; set; }
        public bool CreateLeadFromMessages { get; set; }
        public bool CreateLeadFromComments { get; set; }
    }
}
