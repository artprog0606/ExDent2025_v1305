using AdvantShop.Customers;

namespace AdvantShop.Models.Common
{
    public class ZoneDialogTrigger
    {
        public CustomerContact CurrentContact { get; set; }
        public string CurrentCity { get; set; }
        public bool EnablesAddressList { get; set; }
    }
}