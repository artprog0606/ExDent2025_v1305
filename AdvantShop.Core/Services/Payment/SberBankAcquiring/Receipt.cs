using System.Collections.Generic;


namespace AdvantShop.Core.Services.Payment.SberBankAcquiring
{
    public class Receipt
    {

        public CartItems cartItems { get; set; }
        public string receiptType { get { return "sell"; } }

        public string ffdVersion { get; set; }

        public List<Payment> payments { get; set; }

        public Company company { get; set; }

    }

    public class CustomerDetails
    {
        public string email { get; set; }
        //public string phone { get; set; }         // not required
        //public string contact { get; set; }       // not required
        //public object deliveryInfo { get; set; }  // not required
    }


    public class CartItems
    {
        public List<Item> items { get; set; }

    }

    public class Item
    {
        public string positionId { get; set; }
        public string name { get; set; }
        public BaseQuantity quantity { get; set; }

        public int itemAmount { get; set; }
        public string itemCode { get; set; }
        public int itemPrice { get; set; }

        public Tax tax { get; set; } 


        public string paymentMethod { get; set; }

        public string paymentObject { get; set; }

    }

    public class Payment
    {
        public int type { get { return 1; } }
        public long sum { get; set; }
    }

    public class Company
    { 
        public string email { get; set; }
        public string sno { get; set; }
        public string inn { get; set; }
        public string paymentAddress { get; set; }
    }


    public class BaseQuantity
    {
        public float value { get; set; }
    }

    public class Quantity : BaseQuantity
    {
        public string measure { get; set; }
    }

    public class Tax
    {
        public int taxType { get; set; }
        //public float taxSum { get; set; }
    }

}
