using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Handlers.ProductDetails
{
    public class GetProductCustomOptionsHandler
    {
        private readonly int _productId;
        private readonly bool _preSelect;
        private readonly List<OptionItem> _selectedOptions;
        private List<CustomOption> _productOptions;
        private List<EvaluatedCustomOptions> _evCustomOptions;

        public GetProductCustomOptionsHandler(int productId, List<OptionItem> selectedOptions, bool preSelect = true)
        {
            _productId = productId;
            _selectedOptions = selectedOptions;
            _preSelect = preSelect;
            Load();
        }

        public string GetXml() => CustomOptionsService.SerializeToXml(_evCustomOptions);

        public string GetJsonHash() => CustomOptionsService.GetJsonHash(_evCustomOptions);

        public bool HasOptions => _productOptions.Any();

        private void Load()
        {
            _productOptions = CustomOptionsService.GetCustomOptionsByProductId(_productId);
            var selectedOptions = new List<OptionItem>();

            if (_preSelect)
            {
                // выбираем доп опции по умолчанию
                selectedOptions = _productOptions.SelectMany(x => x.SelectedOptions ?? Enumerable.Empty<OptionItem>()).ToList();
            }
            else
            {
                foreach (var customOption in _productOptions)
                {
                    var option = _selectedOptions?.FirstOrDefault(optionItem =>
                        optionItem.CustomOptionsId == customOption.CustomOptionsId);

                    // если не нашли опцию и она необязательна
                    if (option == null && customOption.IsRequired is false)
                        continue;

                    if (customOption.InputType == CustomOptionInputType.TextBoxMultiLine
                        || customOption.InputType == CustomOptionInputType.TextBoxSingleLine)
                    {
                        // переносим текст из клиентского поля
                        customOption.Options[0].Title = option?.OptionText ?? customOption.Options[0].Title;
                        if (option != null || customOption.IsRequired)
                        {
                            AddSelectedOption(customOption.Options[0]);
                        }
                    }

                    if (option == null)
                        continue;

                    // для комбо и мультивыбора может быть несколько опций
                    if (customOption.InputType == CustomOptionInputType.ChoiceOfProduct
                        || customOption.InputType == CustomOptionInputType.MultiCheckBox)
                    {
                        var optionList = _selectedOptions
                            ?.Where(optionItem => optionItem.CustomOptionsId == customOption.CustomOptionsId)
                            .ToList();
                        optionList?.ForEach(AddSelectedOption);
                    }

                    if (customOption.InputType == CustomOptionInputType.DropDownList ||
                        customOption.InputType == CustomOptionInputType.RadioButton
                        || customOption.InputType == CustomOptionInputType.CheckBox)
                    {
                        AddSelectedOption(option);
                    }

                    continue;

                    void AddSelectedOption(OptionItem selectedOption)
                    {
                        var opt = customOption.Options.WithId(selectedOption.OptionId);
                        opt.DefaultQuantity = selectedOption.DefaultQuantity;
                        selectedOptions.Add(opt);
                    }
                }
            }

            _evCustomOptions = CustomOptionsService.GetEvaluatedCustomOptions(_productOptions, selectedOptions);
        }
    }
}