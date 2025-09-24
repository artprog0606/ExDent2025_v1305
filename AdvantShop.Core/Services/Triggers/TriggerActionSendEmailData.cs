namespace AdvantShop.Core.Services.Triggers
{
    public class TriggerActionSendEmailData
    {
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public TriggerActionSendEmailAdditionalParams Params { get; set; }
    }

    public class TriggerActionSendEmailAdditionalParams
    {
        public bool RecipientIsCustomer { get; set; } = true;
        public bool RecipientIsAnother { get; set; }
        public string EmailToReceive { get; set; }
    }
}
