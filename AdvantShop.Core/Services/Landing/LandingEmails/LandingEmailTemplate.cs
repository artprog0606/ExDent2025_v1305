namespace AdvantShop.Core.Services.Landing.LandingEmails
{
    public class LandingEmailTemplate
    {
        public int Id { get; set; }
        public int BlockId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        /// <summary>
        /// Через сколько отсылать письмо (в минутах)
        /// </summary>
        public int SendingTime { get; set; }
    }
}
