using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;

namespace AdvantShop.Models.ProductDetails
{
    public class CustomOptionModel
    {
        public int ID { get; }
        public int CustomOptionsId { get; }
        public string Title { get; }
        public bool IsRequired { get; }
        public CustomOptionInputType InputType { get; }
        public int SortOrder { get; }

        public float? MinQuantity { get; set; }
        public float? MaxQuantity { get; set; }

        public List<CustomOptionItemModel> Options { get; }
        public List<CustomOptionItemModel> SelectedOptions { get; }
        
        public string Description { get; }

        public CustomOptionModel(CustomOption customOption)
        {
            ID = customOption.ID;
            CustomOptionsId = customOption.CustomOptionsId;
            Title = customOption.Title;
            IsRequired = customOption.IsRequired;
            InputType = customOption.InputType;
            SortOrder = customOption.SortOrder;
            CustomOptionsId = customOption.CustomOptionsId;
            Options = customOption.Options.Select(x => new CustomOptionItemModel(x)).ToList();
            SelectedOptions = customOption.SelectedOptions?.Select(x => new CustomOptionItemModel(x)).ToList();
            MinQuantity = customOption.MinQuantity;
            MaxQuantity = customOption.MaxQuantity;
            Description = customOption.Description;
        }
    }

    public class CustomOptionItemModel
    {
        public int ID { get; }
        public int OptionId { get; }
        public int CustomOptionsId { get; }
        public string Title { get; }
        public OptionPriceType PriceType { get; }
        public string OptionText { get; }

        public int? ProductId { get; }
        public int? OfferId { get; }
        public float? MinQuantity { get; set; }
        public float? MaxQuantity { get; set; }
        public float? DefaultQuantity { get; set; }
        
        public string PictureUrl { get; set; }
        
        public string Description { get; set; }
        public string PriceString { get; set; }
        public CustomOptionItemModel(OptionItem optionItem)
        {
            ID = optionItem.ID;
            OptionId = optionItem.OptionId;
            CustomOptionsId = optionItem.CustomOptionsId;
            Title = optionItem.Title;
            PriceType = optionItem.PriceType;
            OptionText = optionItem.GetOptionText(null);
            ProductId = optionItem.ProductId;
            OfferId = optionItem.OfferId;
            MinQuantity = optionItem.MinQuantity;
            MaxQuantity = optionItem.MaxQuantity;
            DefaultQuantity = optionItem.DefaultQuantity;
            PictureUrl = FeaturesService.IsEnabled(EFeature.CustomOptionPicture) ? optionItem.PictureData.PictureUrl : null;
            Description = optionItem.Description;
            PriceString = optionItem.GetOptionPrice();
        }
    }
}