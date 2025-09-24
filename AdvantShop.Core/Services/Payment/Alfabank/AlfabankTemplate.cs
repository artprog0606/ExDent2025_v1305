//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace AdvantShop.Payment
{
    public class AlfabankTemplate
    {
        public const string UserName = "Alfabank_UserName";
        public const string Password = "Alfabank_Password";
        public const string MerchantLogin = "Alfabank_MerchantLogin";
        public const string SendReceiptData = "Alfabank_SendReceiptData";
        public const string Taxation = "Alfabank_Taxation";
        public const string GatewayUrl = "Alfabank_GatewayUrl";
        public const string TypeFfd = "Alfabank_TypeFfd";
        [Obsolete]
        public const string GatewayCountry = "Alfabank_GatewayCountry";
        [Obsolete]
        public const string UseTestMode = "Alfabank_UseTestMode";
    }
}
