using System.Collections.Specialized;
using System.Text;

namespace AdvantShop.Payment
{
    public enum FormMethod
    {
        POST,
        GET
    }

    public class PaymentForm
    {
        public string Url { get; set; }
        
        public FormMethod Method { get; set; } 
            = FormMethod.POST;
        
        public string FormName { get; set; } 
            = "Pay";
        
        public NameValueCollection InputValues { get; set; } 
            = new NameValueCollection();
        
        public Encoding Encoding { get; set; } 
            = Encoding.UTF8;
    }
}