using System.Collections.Generic;
using AdvantShop.Areas.Api.Models.Cart;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Preorder
{
    public sealed class PreorderModel
    {
        public PreorderOffer Offer { get; set; }
        
        public PreorderCustomer Customer { get; set; }
        
        public bool IsAgree { get; set; }
    }

    public sealed class PreorderOffer
    {
        public int OfferId { get; set; }
        
        public List<SelectedCartCustomOptionApi> CustomOptions { get; set; }

        public float Amount { get; set; }
    }
    
    public sealed class PreorderCustomer
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Comment { get; set; }
    }

    public sealed class PreorderResponse : ApiResponse
    {
        public string SuccessText { get; set; }
    }
}