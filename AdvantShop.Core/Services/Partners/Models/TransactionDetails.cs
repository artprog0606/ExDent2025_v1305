using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Partners
{
    public class TransactionDetails
    {
        public TransactionDetails()
        {
            OrderItemsDetails = new List<TransactionOrderItemDetails>();
        }

        public List<TransactionOrderItemDetails> OrderItemsDetails { get; set; }
    }

    public class TransactionOrderItemDetails
    {
        /// <summary>
        /// Accrued money considering partner reward percent
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float? RewardPercent { get; set; }

        /// <summary>
        /// Accrued money considering category reward percent
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float? CategoryRewardPercent { get; set; }
        /// <summary>
        /// Categories names hierarchy (from root category to child)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CategoryPath { get; set; }

        public string ArtNo { get; set; }
        public string Name { get; set; }

        public float Amount { get; set; }
        public decimal Price { get; set; }
        public decimal Sum { get; set; }
        public decimal Reward { get; set; }
    }
}
