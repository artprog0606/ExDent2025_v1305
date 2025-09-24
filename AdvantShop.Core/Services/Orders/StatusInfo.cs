//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Orders
{
    public class StatusInfo
    {
        public string Status { get; set; }
        public string Comment { get; set; }

        public string PreviousStatus { get; set; }
        public bool Hidden { get; set; }
    }
}