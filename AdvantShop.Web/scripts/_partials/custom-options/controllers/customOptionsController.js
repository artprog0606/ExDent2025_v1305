/* @ngInject */
function CustomOptionsCtrl(customOptionsService, toaster, $timeout, $q) {
    var ctrl = this,
        canceler = null,
        timeoutId;

    ctrl.$onInit = function () {
        customOptionsService.getData(ctrl.productId).then(function (customOptions) {
            ctrl.items = customOptions;
            $timeout(() => {
                customOptionsService.get(ctrl.productId, ctrl.items).then(function (data) {
                    ctrl.xml = data.xml;
                    ctrl.jsonHash = data.jsonHash;

                    if (ctrl.initFn != null) {
                        ctrl.initFn({ customOptions: ctrl });
                    }
                    ctrl.changeFn();
                });
            }, 0);
        });
    };

    ctrl.eventDebounce = function (value, item, option, customOptionComboView = false) {
        if (timeoutId != null) {
            clearTimeout(timeoutId);
        }

        timeoutId = setTimeout(ctrl.change.bind(ctrl, value, item, option, customOptionComboView), 0);
    };

    ctrl.change = function (value, item, option, customOptionComboView = false) {
        if (value == null) {
            if (item.DefaultQuantity != null) {
                item.DefaultQuantity = item.MinQuantity || 0;
            } else {
                option.DefaultQuantity = option.MinQuantity || 0;
            }
        }

        if (option) {
            if (item.InputType === 6) {
                if (option.MaxQuantity > 1 && option.DefaultQuantity === 0) {
                    option.Selected = false;
                }
            }

            if (item.InputType === 2) {
                if (item.MaxQuantity > 1) {
                    item.Selected = !(option.DefaultQuantity === 0 && item.IsRequired === false);
                }
            }
            if (!ctrl.isValidQuantity(option.DefaultQuantity || 1, item, option)) {
                option.Selected = false;
                return;
            }
        }

        if (canceler) {
            canceler.resolve();
        }

        canceler = $q.defer();

        customOptionsService.get(ctrl.productId, ctrl.items, canceler.promise).then(function (data) {
            if (!data) return;
            ctrl.xml = data.xml;
            ctrl.jsonHash = data.jsonHash;
            ctrl.changeFn({ item: item });
        });
    };

    ctrl.findSelectedOptionByOptionId = function (selectedOption, options) {
        return options.find((it) => selectedOption.OptionId === it.OptionId);
    };

    ctrl.isValidQuantity = function (nextValueOption, item, option) {
        const isValid = customOptionsService.isValidAddOption(nextValueOption, item, option);
        if (isValid === false) {
            ctrl.notifyWarning(`Вы можете выбрать из этой группы не больше ${item.MaxQuantity} товаров`);
        }
        return isValid;
    };

    ctrl.notifyWarning = function (text) {
        toaster.pop('warning', '', text);
    };

    ctrl.initSelect = function (item) {
        if (item.SelectedOptions != null) {
            if (item.InputType === 6 && item.SelectedOptions?.every((it) => it.DefaultQuantity === 0)) {
                item.SelectedOptions.length = 0;
            } else {
                const selectedOptions = [];
                item.Options.forEach(function (option) {
                    const selectedOption = item.SelectedOptions.find((selectedOption) => selectedOption.OptionId === option.OptionId);
                    if (!selectedOption) return;
                    option.Selected = true;
                    option.DefaultQuantity = selectedOption.DefaultQuantity;
                    selectedOptions.push(option);
                });
                if (selectedOptions.length > 0) {
                    item.SelectedOptions = selectedOptions;
                }
            }
        } else {
            item.SelectedOptions = item.IsRequired ? [item.Options[0]] : null;
        }
    };

    // если нет label для checkbox`a
    // из за label вызывается change 2 раза (от spinbox и label)
    ctrl.selectOption = function (item, option) {
        if (item.InputType === 2 && (item.MaxQuantity === 1 || item.MaxQuantity == null)) {
            item.Selected = !item.Selected;
            ctrl.change(item.Selected, item, option);
        }
    };
}

export default CustomOptionsCtrl;
