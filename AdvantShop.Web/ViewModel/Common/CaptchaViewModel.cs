
using AdvantShop.Configuration;

namespace AdvantShop.ViewModel.Common
{
    public class CaptchaViewModel
    {
        public string CaptchaId { get; set; }
        public string CaptchaBase64Text { get; set; }

        public string CaptchaEncodedBase64Text { get; set; }

        public string CaptchaSource { get; set; }

        public string CaptchaCode { get; set; }

        public string NgModel { get; set; }

	public string NgModelSource { get; set; }
        public CaptchaMode CaptchaMode { get; set; }

        public int CodeLength { get; set; }
    }
}