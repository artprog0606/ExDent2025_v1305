namespace AdvantShop.Core.Services.Smses
{
    public class SmsTemplateOnOrderChanging
    {
        public int Id { get; set; }
        public int OrderStatusId { get; set; }
        public string SmsText { get; set; }
        public bool Enabled { get; set; }
    }
}
