using System.Collections.Generic;

namespace AdvantShop.Areas.Api.Models.Products
{
    public class GetPriceModel
    {
        public int OfferId { get; set; }
        
        public List<SelectedCustomOptionApi> Options { get; set; }
        
        public float Amount { get; set; }
    }
}