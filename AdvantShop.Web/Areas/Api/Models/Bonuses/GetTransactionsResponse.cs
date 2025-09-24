using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Bonuses.Model.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AdvantShop.Areas.Api.Models.Bonuses
{
    public class GetTransactionItem
    {
        [JsonProperty("balance")]
        public decimal Balance { get; set; }

        [JsonProperty("createOn")]
        public DateTime CreateOn { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("basis")]
        public string Basis { get; set; }

        [JsonIgnore]
        public EOperationType OperationType { get; set; }

        [JsonProperty("operationType")]
        public string OperationTypeStr => OperationType.ToString();

        [JsonProperty("purchaseId")]
        public int? PurchaseId { get; set; }
    }

    public class GetTransactionsResponse : ApiPaginationResponse, IApiResponse
    {
        [JsonProperty("transactions")]
        public List<GetTransactionItem> Transactions { get; set; }
    }
}