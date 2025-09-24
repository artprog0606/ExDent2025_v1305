using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Cart;
using AdvantShop.Catalog;

namespace AdvantShop.Areas.Api.Services
{
    public sealed class CustomOptionsHelper
    {
        public static string GetAttributesXml(Offer offer, List<SelectedCartCustomOptionApi> selectedOptions)
        {
            var customOptions = CustomOptionsService.GetCustomOptionsByProductIdCached(offer.ProductId);
            if (customOptions == null || customOptions.Count == 0)
                return "";

            var requiredCustomOptions = customOptions.Where(x => x.IsRequired && x.SelectedOptions != null).ToList();

            // если у товара есть обязательные доп опции, но их не прислали, то ставим 
            if (requiredCustomOptions.Count > 0 && (selectedOptions == null || selectedOptions.Count == 0))
            {
                selectedOptions = new List<SelectedCartCustomOptionApi>();
                
                foreach (var customOption in requiredCustomOptions)
                    foreach (var item in customOption.SelectedOptions)
                    {
                        selectedOptions.Add(new SelectedCartCustomOptionApi()
                        {
                            Id = customOption.CustomOptionsId,
                            OptionId = item.OptionId,
                            OptionAmount = item.DefaultQuantity,
                            OptionText = item.OptionText
                        });
                    }
            }

            if (selectedOptions == null || selectedOptions.Count == 0)
                return "";

            var evOptions = new List<EvaluatedCustomOptions>();
            
            foreach (var customOption in customOptions)
            {
                var selected = selectedOptions.Where(x => x.Id == customOption.CustomOptionsId).ToList();
                if (selected.Count == 0)
                    continue;

                foreach (var selectedOption in selected)
                {
                    var selectedOptionItem = customOption.Options.Find(x => x.OptionId == selectedOption.OptionId);
                    if (selectedOptionItem == null)
                        continue;

                    evOptions.Add(new EvaluatedCustomOptions()
                    {
                        CustomOptionId = customOption.CustomOptionsId,
                        OptionId = selectedOptionItem.OptionId,
                        OptionTitle = selectedOption.OptionText ?? selectedOptionItem.Title,
                        CustomOptionTitle = customOption.Title,
                        OptionPriceBc = selectedOptionItem.BasePrice,
                        OptionPriceType = selectedOptionItem.PriceType,
                        OptionAmount = selectedOption.OptionAmount ?? selectedOptionItem.DefaultQuantity
                    });
                }
            }

            return CustomOptionsService.SerializeToXml(evOptions);
        }
        
        public static string GetCustomOptionsHash(Offer offer, List<SelectedCartCustomOptionApi> selectedOptions)
        {
            var customOptions = CustomOptionsService.GetCustomOptionsByProductIdCached(offer.ProductId);
            if (customOptions == null || customOptions.Count == 0)
                return null;

            var requiredCustomOptions = customOptions.Where(x => x.IsRequired && x.SelectedOptions != null).ToList();

            // если у товара есть обязательные доп опции, но их не прислали, то ставим 
            if (requiredCustomOptions.Count > 0 && (selectedOptions == null || selectedOptions.Count == 0))
            {
                selectedOptions = new List<SelectedCartCustomOptionApi>();
                
                foreach (var customOption in requiredCustomOptions)
                    foreach (var item in customOption.SelectedOptions)
                    {
                        selectedOptions.Add(new SelectedCartCustomOptionApi()
                        {
                            Id = customOption.CustomOptionsId,
                            OptionId = item.OptionId,
                            OptionAmount = item.DefaultQuantity,
                            OptionText = item.OptionText
                        });
                    }
            }

            if (selectedOptions == null || selectedOptions.Count == 0)
                return null;

            var evOptions = new List<EvaluatedCustomOptions>();
            
            foreach (var customOption in customOptions)
            {
                var selected = selectedOptions.Where(x => x.Id == customOption.CustomOptionsId).ToList();
                if (selected.Count == 0)
                    continue;

                foreach (var selectedOption in selected)
                {
                    var selectedOptionItem = customOption.Options.Find(x => x.OptionId == selectedOption.OptionId);
                    if (selectedOptionItem == null)
                        continue;

                    evOptions.Add(new EvaluatedCustomOptions()
                    {
                        CustomOptionId = customOption.CustomOptionsId,
                        OptionId = selectedOptionItem.OptionId,
                        OptionTitle = selectedOption.OptionText ?? selectedOptionItem.Title,
                        CustomOptionTitle = customOption.Title,
                        OptionPriceBc = selectedOptionItem.BasePrice,
                        OptionPriceType = selectedOptionItem.PriceType,
                        OptionAmount = selectedOption.OptionAmount ?? selectedOptionItem.DefaultQuantity
                    });
                }
            }

            return CustomOptionsService.GetJsonHash(evOptions);
        }
    }
}