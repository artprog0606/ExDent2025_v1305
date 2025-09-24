using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Webhook.Models.Api
{
    public class OrderItemModel
    {
        public static OrderItemModel FromOrderItem(OrderItem orderItem)
        {
            if (orderItem == null)
                return null;

            return new OrderItemModel
            {
                ArtNo = orderItem.ArtNo,
                Name = orderItem.Name,
                Color = orderItem.Color,
                Size = orderItem.Size,
                Price = orderItem.Price,
                Amount = orderItem.Amount,
                PhotoSrc = orderItem.Photo?.ImageSrcSmall(),
                ProductId = orderItem.ProductID,
                OrderItemId = orderItem.OrderItemID,
                Unit = orderItem.Unit
            };
        }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? OfferId { get; set; }

        public string ArtNo { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public float Price { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PreparedPrice { get; set; }
        
        public float Amount { get; set; }
        public string PhotoSrc { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Unit { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<SelectedOrderCustomOptionItem> SelectedCustomOptions { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? CanAddToCard { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float? Multiplicity { get; set; }
        
        [JsonIgnore]
        public int? ProductId { get; set; }
        
        [JsonIgnore]
        public int OrderItemId { get; set; }
    }

    public class SelectedOrderCustomOptionItem
    {
        public int Id { get; }
        public int OptionId { get; }
        public string OptionText { get; }
        public string Title { get; }
        public string Value { get; }

        public SelectedOrderCustomOptionItem(EvaluatedCustomOptions item, List<CustomOption> productCustomOptions, Currency currency)
        {
            Id = item.CustomOptionId;
            OptionId = item.OptionId;
            Title = item.CustomOptionTitle;
            Value = item.OptionTitle +
                    (item.OptionTitle.IsNotEmpty() && item.OptionAmount > 1 ? " " : "") +
                    (item.OptionAmount > 1 ? "x " + item.OptionAmount : "");

            var customOption = productCustomOptions.Find(x => x.CustomOptionsId == Id);
            if (customOption != null && 
                (customOption.InputType == CustomOptionInputType.TextBoxMultiLine ||
                 customOption.InputType == CustomOptionInputType.TextBoxSingleLine))
            {
                OptionText = item.OptionTitle;
            }
        }
    }
}
