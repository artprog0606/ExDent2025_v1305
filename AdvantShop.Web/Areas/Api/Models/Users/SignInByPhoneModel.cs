namespace AdvantShop.Areas.Api.Models.Users
{
    public class SignInByPhoneModel
    {
        public string Phone { get; set; }
        public bool SignUp { get; set; }
        public bool AddHash { get; set; }
    }
    
    public class SignInByPhoneConfirmCodeModel
    {
        public string Phone { get; set; }
        public string Code { get; set; }
        public bool SignUp { get; set; }
    }
}