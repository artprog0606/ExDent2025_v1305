/* @ngInject */
function ShippingItemErrorCtrl($element) {
    const ctrl = this;

    function addInvalidClass() {
        ctrl.shippingList.customClassesByItemId[ctrl.shippingTemplate.shipping.Id] = {
            'ng-invalid': true,
        };
    }

    function removeInvalidClass() {
        if (ctrl.shippingList.customClassesByItemId[ctrl.shippingTemplate.shipping.Id] != null) {
            ctrl.shippingList.customClassesByItemId[ctrl.shippingTemplate.shipping.Id]['ng-invalid'] = false;
        }
    }

    ctrl.$onInit = function () {
        if (ctrl.shippingTemplate != null) {
            $element.on('$destroy', () => removeInvalidClass());
        }
    };

    ctrl.isErrorShow = function () {
        const result = (!ctrl.shippingTemplate || ctrl.shippingTemplate.isSelected) && ctrl.errorText != null && ctrl.errorText.length > 0;

        if (ctrl.shippingTemplate != null) {
            if (result) {
                addInvalidClass();
            } else {
                removeInvalidClass();
            }
        }

        return result;
    };
}

export default ShippingItemErrorCtrl;
