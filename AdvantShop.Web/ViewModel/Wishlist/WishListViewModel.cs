using AdvantShop.Configuration;

namespace AdvantShop.ViewModel.Wishlist
{
    public class WishListBlockViewModel
    {
        public WishListBlockViewModel()
        {
            OnlyIcon = SettingsDesign.IsMobileTemplate;
        }
        
        public int OfferId { get; set; }

        public string NgNameCallbackInit { get; set; }

        public bool Checked { get; set; }

        public string NgOfferId { get; set; }

        public string Mode { get; set; }
        public string Hash { get; set; }
        public bool OnlyIcon { get; set; }
    }
}