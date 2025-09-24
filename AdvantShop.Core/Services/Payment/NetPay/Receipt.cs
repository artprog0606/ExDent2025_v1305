using System.Collections.Generic;


namespace AdvantShop.Core.Services.Payment.NetPay
{
    public class Cashbox
    {
        public string timestamp { get; set; } // not required
        public Service service { get; set; } // not required
        public Receipt receipt { get; set; } // not required

    }

    public class Service
    {
        public string inn { get; set; }
        public string payment_address { get; set; }
        public string callback_url { get; set; }
    }

    public class Receipt
    {
        public CustomerDetails attributes { get; set; } // not required
        public List<Item> items { get; set; }
        public List<Payments> payments { get; set; } // not required
        public string total { get; set; }
    }

    public class CustomerDetails
    {
        public string email { get; set; }
        public string phone { get; set; }         // not required
    }


    public class Item
    {
        public string name { get; set; }
        public string price { get; set; }
        public string quantity { get; set; }
        public string sum { get; set; }
        public string tax { get; set; } // not required

    }

    public class Payments
    {
        public string sum { get; set; }
        public int type { get; set; }
    }
}
