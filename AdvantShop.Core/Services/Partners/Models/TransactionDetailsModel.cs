using System;
using System.Collections.Generic;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Partners
{
    public class TransactionDetailsModel : TransactionDetails
    {
        public TransactionDetailsModel()
        {
            OrderItemsDetails = new List<TransactionOrderItemDetailsModel>();
        }

        public TransactionDetailsModel(string json, TransactionCurrency currency, int transactionId)
        {
            var details = new TransactionDetailsModel();
            if (json.IsNotEmpty())
            {
                try
                {
                    details = JsonConvert.DeserializeObject<TransactionDetailsModel>(json) ?? new TransactionDetailsModel();
                    details.OrderItemsDetails.ForEach((oiDetails) => oiDetails.FormatPrices(currency));
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(string.Format("Error at deserialize transaction {0} details", transactionId), ex);
                }
            }
            OrderItemsDetails = details.OrderItemsDetails;
        }

        public new List<TransactionOrderItemDetailsModel> OrderItemsDetails { get; set; }
    }

    public class TransactionOrderItemDetailsModel : TransactionOrderItemDetails
    {
        public string PriceFormatted { get; set; }
        public string SumFormatted { get; set; }
        public string RewardFormatted { get; set; }

        public void FormatPrices(TransactionCurrency currency)
        {
            PriceFormatted = PriceFormatService.FormatPrice((float)Price, currency);
            SumFormatted = PriceFormatService.FormatPrice((float)Sum, currency);
            RewardFormatted = PriceFormatService.FormatPrice((float)Reward, currency);
        }
    }
}
