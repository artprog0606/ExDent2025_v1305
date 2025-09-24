namespace AdvantShop.Web.Admin.Models.Crm
{
    public class SaveInstagramSettingsModel
    {
        public int Id { get; set; }
        public bool CreateLeadFromDirectMessages { get; set; }
        public bool CreateLeadFromComments { get; set; }
    }
}
