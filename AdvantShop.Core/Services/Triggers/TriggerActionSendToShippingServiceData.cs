namespace AdvantShop.Core.Services.Triggers
{
    public class TriggerActionSendToShippingServiceData
    {
        public bool SendEmailOnError { get; set; }
        public string EmailForError { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
    }
}
