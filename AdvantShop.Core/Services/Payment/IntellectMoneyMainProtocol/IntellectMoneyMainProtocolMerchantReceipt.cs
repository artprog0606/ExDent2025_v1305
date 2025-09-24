using System.Collections.Generic;
using AdvantShop.Taxes;
using Newtonsoft.Json;

namespace AdvantShop.Payment
{
    // docs: https://wiki.intellectmoney.ru/pages/viewpage.action?pageId=6324234#id-%D0%9F%D1%80%D0%BE%D1%82%D0%BE%D0%BA%D0%BE%D0%BB%D0%BF%D1%80%D0%B8%D0%B5%D0%BC%D0%B0%D0%BF%D0%BB%D0%B0%D1%82%D0%B5%D0%B6%D0%B5%D0%B9Intellectmoney-merchantReceipt4.5%D0%9F%D1%80%D0%B0%D0%B2%D0%B8%D0%BB%D0%B0%D1%84%D0%BE%D1%80%D0%BC%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D1%8F%D1%87%D0%B5%D0%BA%D0%B0%D0%B4%D0%BB%D1%8F%D0%BE%D0%BD%D0%BB%D0%B0%D0%B9%D0%BD%D0%BA%D0%B0%D1%81%D1%81%D1%8B(merchantReceipt)
    
    public class IntellectMoneyMainProtocolMerchantReceipt
    {
        [JsonProperty("inn")]
        public string Inn { get; set; }
        
        [JsonProperty("skipAmountCheck")]
        public int SkipAmountCheck { get; set; }
        
        [JsonProperty("group")]
        public string Group { get; set; }
        
        [JsonProperty("content")]
        public IntellectMoneyMainProtocolMerchantReceiptContent Content { get; set; }
    }

    public class IntellectMoneyMainProtocolMerchantReceiptContent
    {
        [JsonProperty("type")]
        public int Type { get; set; }
        
        [JsonProperty("customerContact")]
        public string CustomerContact { get; set; }
        
        [JsonProperty("positions")]
        public List<IntellectMoneyMainProtocolMerchantReceiptContentPosition> Positions { get; set; }
    }
    
    public class IntellectMoneyMainProtocolMerchantReceiptContentPosition
    {
        [JsonProperty("quantity")]
        public string Quantity { get; set; }
        
        [JsonProperty("price")]
        public string Price { get; set; }
        
        [JsonProperty("tax")]
        public int Tax { get; private set; }
        
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("paymentSubjectType")]
        public int PaymentSubjectType { get; set; }
        [JsonProperty("paymentMethodType")]
        public int PaymentMethodType { get; set; }

        public IntellectMoneyMainProtocolMerchantReceiptContentPosition(TaxType? taxType)
        {
            switch (taxType)
            {
                case null:
                case TaxType.None:
                case TaxType.VatWithout:
                    Tax = 6;
                    break;
                case TaxType.Vat20:
                case TaxType.Vat18:
                    Tax = 1;
                    break;
                case TaxType.Vat10:
                    Tax = 2;
                    break;
                case TaxType.Vat5:
                    Tax = 7;
                    break;
                case TaxType.Vat7:
                    Tax = 8;
                    break;
                case TaxType.Vat0:
                    Tax = 5;
                    break;
            }
        }
    }
}