using System;
using System.Collections.Generic;

namespace AdvantShop.App.Landing.Models.Booking.Cart
{
    public class CartItemAddingModel
    {
        public CartItemAddingModel()
        {
            SelectedServices = new List<int>();
        }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public int AffiliateId { get; set; }
        public int ResourceId { get; set; }
        public List<int> SelectedServices { get; set; }
    }
}
