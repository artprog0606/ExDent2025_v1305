namespace AdvantShop.App.Landing.Models.Landing
{
    public class SubmitFormReturnModel
    {
        public string Message { get; set; }
        public string RedirectUrl { get; set; }

        public string PostAction { get; set; }
        public int RedirectDelay { get; set; }
    }
}
