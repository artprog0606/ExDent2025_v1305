using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.ViewModel.ProductDetails
{
    public class CustomOptionViewModel
    {
        public int ID { get; }
        public int CustomOptionsId { get; }
        public string Title { get; }
        public bool IsRequired { get; }
        public CustomOptionInputType InputType { get; }
        
        public List<CustomOptionItemViewModel> Options { get; }
        
        public CustomOptionViewModel(CustomOption customOption, Currency currency)
        {
            ID = customOption.ID;
            CustomOptionsId = customOption.CustomOptionsId;
            Title = customOption.Title;
            IsRequired = customOption.IsRequired;
            InputType = customOption.InputType;
            Options = customOption.Options.Select(x => new CustomOptionItemViewModel(x, currency)).ToList();
        }
    }

    public class CustomOptionItemViewModel
    {
        public int ID { get; }
        public int OptionId { get; }
        public int CustomOptionsId { get; }
        public OptionPriceType PriceType { get; }
        public string Title { get; }
        public string OptionText { get; }
        
        public CustomOptionItemViewModel(OptionItem option, Currency currency)
        {
            ID = option.ID;
            OptionId = option.OptionId;
            CustomOptionsId = option.CustomOptionsId;
            PriceType = option.PriceType;
            Title = option.Title;
            OptionText = option.GetOptionText(currency);
        }
    }
}