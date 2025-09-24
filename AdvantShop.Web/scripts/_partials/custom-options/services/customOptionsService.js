/* @ngInject */
/** @type{CustomOptionsServiceFnType}
 *  @this{CustomOptionsServiceType}
 *  */
function customOptionsService($http, urlHelper) {
    var service = this;

    service.getData = function (productId) {
        return $http
            .get(urlHelper.getAbsUrl('productExt/getcustomoptions', true), {
                params: { productId: productId },
            })
            .then(function (response) {
                return response.data;
            });
    };

    service.get = function (productId, items, canceler = null) {
        var selectedOptions = service.getSelectedOptions(items);

        return $http
            .post(
                urlHelper.getAbsUrl('productExt/customoptions', true),
                {
                    productId: productId,
                    selectedOptions: selectedOptions,
                },
                {
                    timeout: canceler,
                },
            )
            .then(function (response) {
                return response.data;
            })
            .catch((reason) => {
                if (reason?.xhrStatus !== 'abort') {
                    console.error(reason);
                }
            });
    };

    service.getSelectedOptions = function (items) {
        let selectedOptions = [];

        for (let i = 0; i < items.length; i++) {
            const customOption = items[i];

            switch (customOption.InputType) {
                // DropDownList, RadioButton
                case 0:
                case 1:
                    if (customOption.SelectedOptions != null) {
                        if (Array.isArray(customOption.SelectedOptions)) {
                            if (customOption.SelectedOptions.length > 0 && customOption.SelectedOptions[0] != null) {
                                // радио кнопка не выбранная имеет значение 0
                                customOption.SelectedOptions[0].DefaultQuantity = customOption.SelectedOptions[0].DefaultQuantity || 1;
                                selectedOptions.push(customOption.SelectedOptions[0]);
                            }
                        } else {
                            customOption.SelectedOptions.DefaultQuantity = customOption.SelectedOptions.DefaultQuantity || 1;
                            selectedOptions.push(customOption.SelectedOptions);
                        }
                    }
                    break;
                // TextBoxSingleLine, TextBoxMultiLine
                case 4:
                case 3:
                    if (customOption.SelectedOptions != null && customOption.SelectedOptions?.OptionText.length) {
                        selectedOptions.push(customOption.SelectedOptions);
                    }
                    break;
                //CheckBox
                case 2:
                    if (customOption.Selected || customOption.IsRequired) {
                        customOption.Options[0].DefaultQuantity = customOption.Options[0].DefaultQuantity || 1;
                        selectedOptions.push(customOption.Options[0]);
                    }
                    break;
                //ChoiceOfProduct
                case 5:
                    if (customOption.SelectedOptions != null && customOption.SelectedOptions.length > 0 && customOption.SelectedOptions[0] != null) {
                        selectedOptions.push(customOption.SelectedOptions[0]);
                    } else {
                        for (let j = 0; j < customOption.Options.length; j++) {
                            var option = customOption.Options[j];

                            if (option.MaxQuantity > 0 && option.DefaultQuantity > 0) {
                                selectedOptions.push(option);
                            }
                        }
                    }

                    break;
                // MultiCheckBox
                case 6:
                    for (let j = 0; j < customOption.Options.length; j++) {
                        var chekboxOption = customOption.Options[j];

                        if (!chekboxOption.Selected) {
                            continue;
                        }

                        if (chekboxOption.MaxQuantity === 1) {
                            chekboxOption.DefaultQuantity = 1;
                        }
                        selectedOptions.push(chekboxOption);
                    }
                    break;
                default:
                    throw Error('Not found InputType for custom options: ' + customOption.InputType);
            }
        }
        return selectedOptions;
    };

    service.isValidAddOption = function (nextValueOption, item, option) {
        if (item.MaxQuantity == null) return true;

        const { InputType, Options, MaxQuantity } = item;
        let isValid = true;

        if (InputType === 1) {
            isValid = MaxQuantity >= nextValueOption;
        } else if (InputType === 5) {
            const count = Options.reduce((prev, cur) => prev + (cur.OptionId === option.OptionId ? nextValueOption : cur.DefaultQuantity), 0);
            isValid = MaxQuantity >= count;
        } else if (InputType === 6) {
            const countAllSelected = Options.reduce(
                (prev, cur) => prev + (cur.Selected && cur.OptionId !== option.OptionId ? cur.DefaultQuantity || 1 : 0),
                0,
            );
            const count = countAllSelected + nextValueOption;
            isValid = MaxQuantity >= count;
        }

        return isValid;
    };

    service.isValidOptions = function (options) {
        const invalidOptions = new Set();
        let isValidOptions = true;

        options.forEach((it) => {
            let isValid = true;
            const { MinQuantity, MaxQuantity, InputType, SelectedOptions, Options, IsRequired } = it;

            const getTotalCount = (filterFn) => Options.reduce((prev, cur) => prev + (filterFn(cur) ? cur.DefaultQuantity || 1 : 0), 0);

            if (MinQuantity != null && MaxQuantity != null) {
                if (InputType === 0 || InputType === 1) {
                    if (SelectedOptions?.length > 0 && SelectedOptions[0] != null) {
                        isValid = MinQuantity <= SelectedOptions[0].DefaultQuantity && MaxQuantity >= SelectedOptions[0].DefaultQuantity;
                    }
                } else if (InputType === 5) {
                    const count = getTotalCount(() => true);
                    isValid = MinQuantity <= count && MaxQuantity >= count;
                } else if (InputType === 6) {
                    const count = getTotalCount((cur) => cur.Selected);
                    isValid = IsRequired ? count > 0 && MinQuantity <= count && MaxQuantity >= count : MinQuantity <= count && MaxQuantity >= count;
                }
            } else if (InputType === 6) {
                const count = getTotalCount((cur) => cur.Selected);
                if (IsRequired && count === 0) {
                    isValid = false;
                }
            }

            if (!isValid) {
                isValidOptions = false;
                invalidOptions.add(it);
            }
        });

        return { invalidOptions, isValidOptions };
    };

    service.isEqualCustomOptions = function (target, other) {
        /** @type{(t: IEvaluatedCustomOptions, o: IEvaluatedCustomOptions) => boolean }*/
        const compareOptionsId = (_target, _other) => {
            return _target.CustomOptionId === _other.CustomOptionId && _other.OptionId === _target.OptionId;
        };
        /** @type{(t: IEvaluatedCustomOptions, o: IEvaluatedCustomOptions) => boolean }*/
        const compareOptionValue = (_target, _other) => {
            if (_other.OptionAmount == null && _target.OptionAmount == null) {
                return _other.OptionTitle === _target.OptionTitle;
            }
            return _other.OptionAmount === _target.OptionAmount;
        };
        return target.every((t) => {
            return other.find((o) => compareOptionsId(t, o) && compareOptionValue(t, o));
        });
    };

    service.customOptionItemToEvaluatedCustomOptionsMapper = (options) => {
        return options.map((x) => {
            return {
                CustomOptionId: x.CustomOptionsId,
                OptionId: x.OptionId,
                OptionAmount: x.DefaultQuantity,
                OptionTitle: x.OptionText,
                OptionPriceBc: x.PriceString,
                CustomOptionTitle: x.Title,
            };
        });
    };
}

export default customOptionsService;
