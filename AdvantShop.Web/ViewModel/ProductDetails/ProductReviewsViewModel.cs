using System.Collections.Generic;
using AdvantShop.CMS;

namespace AdvantShop.ViewModel.ProductDetails
{
    public class ProductReviewsViewModel : BaseProductViewModel
    {
        public int EntityId { get; set; }

        public int EntityType { get; set; }

        public bool ModerateReviews { get; set; }

        public bool ReviewsVoiteOnlyRegisteredUsers { get; set; }

        public bool IsAdmin { get; set; }

        public bool RegistredUser { get; set; }

        public List<Review> Reviews { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool ReviewsReadonly { get; set; }

        public string HeaderText { get; set; }

        public bool DisplayImage { get; set; }
        public Configuration.SettingsDesign.eWhoAllowReviews WhoAllowReviews { get; set; }
        public bool ShowVerificationCheckmarkAtAdminInReviews { get; set; }
        public int? Rating { get; set; }
        public bool AllowAddReviews { get; set; }
        public int CountInSection { get; set; }
        public int CountInLine { get; set; }
    }
}