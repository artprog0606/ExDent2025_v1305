using System.ComponentModel.DataAnnotations;

namespace AdvantShop.ViewModel.MyAccount
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string NewPasswordConfirm { get; set; }
    }
}