namespace AdvantShop.Models.SocialWidgets
{
    public class SocialWidgetModel
    {
        public bool IsShowVk { get; set; }
        public bool IsShowFb { get; set; }
        public bool IsShowJivosite { get; set; }
        public bool IsShowCallback { get; set; }
        public bool IsShowCallbackIpTelephony { get; set; }
        public long VkGroupId { get; set; }
        public string FacebookMsgId { get; set; }
        public bool IsShowWhatsApp { get; set; }
        public string LinkWhatsApp { get; set; }
        public bool IsShowViber { get; set; }
        public bool IsShowTelegram { get; set; }
        public bool IsShowOdnoklassniki { get; set; }
        public bool IsShowCustomWidget1 { get; set; }
        public bool IsShowCustomWidget2 { get; set; }
        public bool IsShowCustomWidget3 { get; set; }

        public int ActiveCount { get; set; }
        
        public bool ShowPoppover { get; set; }
        public string PoppoverText { get; set; }
    }
}