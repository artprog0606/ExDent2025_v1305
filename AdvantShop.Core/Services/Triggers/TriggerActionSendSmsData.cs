namespace AdvantShop.Core.Services.Triggers
{
    public class TriggerActionSendSmsData
    {
        public string SmsText { get; set; }
        public int? SmsTemplateId { get; set; }
        public TriggerActionSendSmsDataAdditionalParams Params { get; set; }
    }

    public class TriggerActionSendSmsDataAdditionalParams
    {
        public bool RecipientIsCustomer { get; set; } = true;
        public bool RecipientIsAnother { get; set; }
        public string PhoneToReceive { get; set; }
    }
}
