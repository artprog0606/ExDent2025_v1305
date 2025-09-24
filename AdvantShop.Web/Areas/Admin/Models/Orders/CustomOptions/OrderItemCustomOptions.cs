using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Web.Admin.Models.Orders.CustomOptions
{
    public class OrderItemCustomOption
    {
        public int ID { get; }
        public int CustomOptionsId { get; }
        public string Title { get; }
        public bool IsRequired { get; }
        public CustomOptionInputType InputType { get; }

        public float? MinQuantity { get; set; }
        public float? MaxQuantity { get; set; }
        public string Description { get; set; }

        public List<OrderItemCustomOptionItem> Options { get; }
        public List<OrderItemCustomOptionItem> SelectedOptions { get; }

        public OrderItemCustomOption(CustomOption customOption, Currency currency)
        {
            ID = customOption.ID;
            CustomOptionsId = customOption.CustomOptionsId;
            Title = customOption.Title;
            IsRequired = customOption.IsRequired;
            InputType = customOption.InputType;
            MinQuantity = customOption.MinQuantity;
            MaxQuantity = customOption.MaxQuantity;
            Options = customOption.Options.Select(x => new OrderItemCustomOptionItem(x, currency)).ToList();
            Description = customOption.Description;
            SelectedOptions = customOption.SelectedOptions?.Select(x => new OrderItemCustomOptionItem(x, currency)).ToList();
        }
    }

    public class OrderItemCustomOptionItem
    {
        public int ID { get; }
        public int OptionId { get; }
        public int CustomOptionsId { get; }
        public OptionPriceType PriceType { get; }
        public string Title { get; }
        public string OptionText { get; }

        public int? ProductId { get; set; }
        public int? OfferId { get; set; }
        public float? MinQuantity { get; set; }
        public float? MaxQuantity { get; set; }
        public float? DefaultQuantity { get; set; }
        public string Description { get; set; }

        public OrderItemCustomOptionItem(OptionItem option, Currency currency)
        {
            ID = option.ID;
            OptionId = option.OptionId;
            CustomOptionsId = option.CustomOptionsId;
            PriceType = option.PriceType;
            Title = option.Title;
            OptionText = option.GetOptionText(currency);
            ProductId = option.ProductId;
            OfferId = option.OfferId;
            MinQuantity = option.MinQuantity;
            MaxQuantity = option.MaxQuantity;
            DefaultQuantity = option.DefaultQuantity;
            Description = option.Description;
        }
    }
}