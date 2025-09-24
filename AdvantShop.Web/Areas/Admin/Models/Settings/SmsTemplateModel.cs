namespace AdvantShop.Web.Admin.Models.Settings
{
    public class SmsTemplateModel
    {
        public int Id { get; set; }
        public int OrderStatusId { get; set; }
        public string OrderStatusName { get; set; }
        public string SmsText { get; set; }
        public bool Enabled { get; set; }
    }
}
