/* @ngInject */
function OrderItemCustomOptionsCtrl(orderItemCustomOptionsService, $timeout) {
    var ctrl = this,
        timeoutId;

    ctrl.$onInit = function () {
        orderItemCustomOptionsService.getOrderItemData(ctrl.orderItemId).then(function (customOptions) {
            ctrl.items = customOptions;
            $timeout(() => {
                orderItemCustomOptionsService.get(ctrl.productId, ctrl.items).then(function (data) {
                    ctrl.xml = data.xml;
                    ctrl.jsonHash = data.jsonHash;

                    if (ctrl.initFn != null) {
                        ctrl.initFn({ customOptions: ctrl });
                    }
                });
            }, 0);
        });
    };

    ctrl.eventDebounce = function (item, option) {
        if (timeoutId != null) {
            clearTimeout(timeoutId);
        }

        timeoutId = setTimeout(ctrl.change.bind(ctrl, item, option), 300);
    };

    ctrl.change = function (item, option) {
        if (option) {
            if (item.InputType === 6) {
                if (option.MaxQuantity > 1 && option.DefaultQuantity === 0) {
                    option.Selected = false;
                }
            }

            if (item.InputType === 2) {
                if (item.MaxQuantity > 1) {
                    item.Selected = !(option.DefaultQuantity === 0 && item.IsRequired === false);
                } else if (item.IsRequired === true) {
                    item.Selected = true;
                }
            }
        }

        orderItemCustomOptionsService.get(ctrl.productId, ctrl.items).then(function (data) {
            ctrl.xml = data.xml;
            ctrl.jsonHash = data.jsonHash;
            ctrl.changeFn({ item: item });
        });
    };

    ctrl.initSelect = function (item) {
        if (item.SelectedOptions != null) {
            var selectedOptions = [];
            item.Options.forEach(function (option) {
                var selectedOption = item.SelectedOptions.find((selectedOption) => selectedOption.OptionId == option.OptionId);
                if (!selectedOption) return;
                option.Selected = true;
                option.DefaultQuantity = selectedOption.DefaultQuantity;
                selectedOptions.push(option);
            });
            if (selectedOptions != null && selectedOptions.length > 0) {
                item.SelectedOptions = selectedOptions;
            }
        } else {
            item.SelectedOptions = item.IsRequired ? [item.Options[0]] : null;
        }
    };
}

export default OrderItemCustomOptionsCtrl;
