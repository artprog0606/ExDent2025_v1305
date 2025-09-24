/* @ngInject */
function orderItemCustomOptionsService($http, urlHelper) {
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

    service.getOrderItemData = function (orderItemId) {
        return $http
            .get(urlHelper.getAbsUrl('orders/getOrderItemCustomOptions', false), {
                params: { orderItemId: orderItemId },
            })
            .then(function (response) {
                return response.data.obj;
            });
    };

    service.get = function (productId, items) {
        var selectedOptions = service.getSelectedOptions(items);

        return $http
            .post(urlHelper.getAbsUrl('productExt/customoptions', true), {
                productId: productId,
                selectedOptions: selectedOptions,
            })
            .then(function (response) {
                return response.data;
            });
    };

    service.getSelectedOptions = function (items) {
        let selectedOptions = [];

        for (let i = 0; i < items.length; i++) {
            const customOption = items[i];

            switch (customOption.InputType) {
                // DropDownList, RadioButton, TextBoxSingleLine, TextBoxMultiLine
                case 0:
                case 1:
                case 3:
                case 4:
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
                //CheckBox
                case 2:
                    if (customOption.Selected || customOption.IsRequired) {
                        customOption.Options[0].DefaultQuantity = customOption.Options[0].DefaultQuantity || 1;
                        selectedOptions.push(customOption.Options[0]);
                    }
                    break;
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

    service.isValidOptions = function (options) {
        const invalidOptions = new Set();
        let isValidOptions = true;

        options.forEach((it) => {
            let isValid = true;

            if (it.MinQuantity != null && it.MaxQuantity != null) {
                if (
                    (it.InputType === 0 || it.InputType === 1) &&
                    it.SelectedOptions != null &&
                    it.SelectedOptions.length > 0 &&
                    it.SelectedOptions[0] != null
                ) {
                    isValid = it.MinQuantity <= it.SelectedOptions[0].DefaultQuantity && it.MaxQuantity >= it.SelectedOptions[0].DefaultQuantity;
                }
                if (it.InputType === 5) {
                    const count = it.Options.reduce((prev, cur) => {
                        return prev + (cur.DefaultQuantity || 1);
                    }, 0);
                    isValid = it.MinQuantity <= count && it.MaxQuantity >= count;
                }
                if (it.InputType === 6) {
                    const count = it.Options.reduce((prev, cur) => {
                        if (cur.Selected) {
                            return prev + (cur.DefaultQuantity || 1);
                        }
                        return prev;
                    }, 0);
                    if (it.IsRequired && count === 0) {
                        isValid = false;
                    } else {
                        isValid = it.MinQuantity <= count && it.MaxQuantity >= count;
                    }
                }
            }

            if (isValid === false) {
                isValidOptions = false;
                invalidOptions.add(it);
            }

            return isValid;
        });

        return {
            invalidOptions,
            isValidOptions,
        };
    };
}
export default orderItemCustomOptionsService;
