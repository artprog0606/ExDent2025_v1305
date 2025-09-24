using System;
using System.Collections.Generic;

namespace AdvantShop.MobileApp
{
    public class Notification
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public Guid CustomerId { get; set; } 
        public Dictionary<string, string> RequestParams { get; set; }
    }
}
