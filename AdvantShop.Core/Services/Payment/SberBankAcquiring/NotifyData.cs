using System.Collections.Generic;


namespace AdvantShop.Core.Services.Payment.SberBankAcquiring
{
    public class NotifyData
    {

        public string mdOrder { get; set; }
        public string orderNumber { get; set; }

        public string operation { get; set; }

        int status { get; set; }

    }

}
