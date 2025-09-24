//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Catalog
{

    public enum CustomOptionInputType
    {
        [Localize("Core.CustomOption.CustomOptionInputType.DropDownList")]
        DropDownList = 0,

        [Localize("Core.CustomOption.CustomOptionInputType.RadioButton")]
        RadioButton = 1,

        [Localize("Core.CustomOption.CustomOptionInputType.CheckBox")]
        CheckBox = 2,

        [Localize("Core.CustomOption.CustomOptionInputType.TextBoxSingleLine")]
        TextBoxSingleLine = 3,

        [Localize("Core.CustomOption.CustomOptionInputType.TextBoxMultiLine")]
        TextBoxMultiLine = 4,

        [Localize("Core.CustomOption.CustomOptionInputType.ChoiceOfProduct")]
        ChoiceOfProduct = 5,

        [Localize("Core.CustomOption.CustomOptionInputType.MultiCheckBox")]
        MultiCheckBox = 6,
    }

    public enum CustomOptionField
    {
        Title = 1,
        SortOrder = 2
    }

    [Serializable]
    public class CustomOption : IDentable, IValidatableObject
    {
        public int ID => CustomOptionsId;
        public int CustomOptionsId { get; set; }
        public string Title { get; set; }
        public bool IsRequired { get; set; }
        public CustomOptionInputType InputType { get; set; }
        public int SortOrder { get; set; }
        public int ProductId { get; set; }

        public float? MinQuantity { get; set; }
        public float? MaxQuantity { get; set; }
        public string Description { get; set; }

        private List<OptionItem> _options;

        public List<OptionItem> Options 
        { 
            get => _options ?? (_options = CustomOptionsService.GetCustomOptionItems(CustomOptionsId));
            set => _options = value;
        }
        
        public List<OptionItem> SelectedOptions { get; set; }

        public CustomOption()
        {
        }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Title))
                yield return new ValidationResult("Заполните название");
            
            if (InputType == CustomOptionInputType.DropDownList || InputType == CustomOptionInputType.RadioButton || InputType == CustomOptionInputType.ChoiceOfProduct
                || InputType == CustomOptionInputType.MultiCheckBox)
            {
                if (Options == null || Options.Count == 0)
                    yield return new ValidationResult("В таблице значений должен быть хотя бы 1 элемент");

                if (MinQuantity.HasValue != MaxQuantity.HasValue)
                    yield return new ValidationResult("Заполните минимальное и максимальное количество");

                if (Options != null)
                {
                    if (Options.Any(x => string.IsNullOrEmpty(x.Title)))
                        yield return new ValidationResult("Заполните названия в таблице значений");

                    if (InputType == CustomOptionInputType.ChoiceOfProduct && Options.Any(x => x.ProductId.HasValue is false))
                        yield return new ValidationResult("Выберите товар");

                    if (MinQuantity.HasValue && MaxQuantity.HasValue)
                    {
                        if (MinQuantity > MaxQuantity)
                            yield return new ValidationResult("Максимальное количество должно быть больше или равно минимальному");
                        if (Options.Any(x => x.MinQuantity.HasValue is false))
                            yield return new ValidationResult("Заполните минимальное количество");
                        if (Options.Any(x => x.MaxQuantity.HasValue is false))
                            yield return new ValidationResult("Заполните максимальное количество");
                        if (Options.Any(x => x.DefaultQuantity.HasValue is false))
                            yield return new ValidationResult("Заполните количество по умолчанию");

                        if (Options.Any(x => x.MinQuantity < 0 || x.MaxQuantity < 0 || x.DefaultQuantity < 0))
                            yield return new ValidationResult("Количество должно быть больше 0");

                        if (Options.Any(x => x.MinQuantity > x.MaxQuantity))
                            yield return new ValidationResult("Максимальное количество для записи должно быть больше или равно минимальному");
                        if (Options.Any(x => x.DefaultQuantity > x.MaxQuantity))
                            yield return new ValidationResult("Значение по умолчанию не должно быть больше максимального");
                        if (Options.Any(x => x.DefaultQuantity < x.MinQuantity))
                            yield return new ValidationResult("Значение по умолчанию не должно быть меньше минимального");

                        if (InputType == CustomOptionInputType.MultiCheckBox)
                        {
                            var minQuantitySum = Options.Sum(x => x.MinQuantity ?? 0);
                            if (minQuantitySum > MaxQuantity)
                                yield return new ValidationResult("Максимальное количество опции не должно быть меньше суммы минимального количества записей");
                            if (minQuantitySum > MinQuantity)
                                yield return new ValidationResult("Минимальное количество опции не должно быть меньше суммы минимального количества записей");
                        }
                    }
                    // если заполнено только количество в гриде
                    else if ((MinQuantity.HasValue && MaxQuantity.HasValue) is false &&
                        Options.Any(x => x.MaxQuantity.HasValue || x.MinQuantity.HasValue || x.DefaultQuantity.HasValue))
                        yield return new ValidationResult("Заполните минимальное и максимальное значение");
                }
            }
        }
    }
}