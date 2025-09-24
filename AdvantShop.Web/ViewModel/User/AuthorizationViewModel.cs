namespace AdvantShop.ViewModel.User
{
    public class AuthorizationViewModel
    {
        public string RedirectTo { get; set; }

        public string ForgotPasswordUrl { get; set; }

        public string RegistrationUrl { get; set; }

        public bool IsLanding { get; set; }
        
        public bool SignUp { get; set; }
    }
}