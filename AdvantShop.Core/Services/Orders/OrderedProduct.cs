using System;
using AdvantShop.Configuration;

namespace AdvantShop.Core.Services.Orders
{
    public class OrderedProduct
    {
        public string Name { get; set; }
        public string ArtNo { get; set; }
        public int? ProductID { get; set; }
        public float Price { get; set; }
        public DateTime LastOrderDateTime { get; set; }
        public string LastOrderNumber { get; set; }
        public string Url { get; set; }
        public string PhotoName { get; set; }
        
        
        public string LastOrderDate => LastOrderDateTime.ToString(SettingsMain.ShortDateFormat);
        public string LastOrderTime => LastOrderDateTime.ToString("HH:mm");
    }
}