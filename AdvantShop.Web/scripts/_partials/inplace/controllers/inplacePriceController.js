/* @ngInject */
function InplacePriceCtrl($scope, $timeout, inplaceService) {
    var ctrl = this;

    ctrl.callbacks = [];

    ctrl.$onInit = function () {
        ctrl.needReinit = {};

        ctrl.inplaceParams = ctrl.inplaceParams();
    };

    ctrl.active = function () {
        ctrl.startContent = ctrl.editor.getData();
        ctrl.isShow = true;
    };

    ctrl.save = function () {
        if (ctrl.product == null || ctrl.product.offerSelected == null) {
            return;
        }

        var content = ctrl.convertToFloat(ctrl.editor.getData()),
            params;

        if (content == null) {
            return;
        }

        params = angular.extend(ctrl.inplaceParams, {
            content: content,
            id: ctrl.product.offerSelected.OfferId,
            field: ctrl.type,
        });

        inplaceService.save(ctrl.inplaceUrl, params).finally(function () {
            ctrl.isShow = false;
            ctrl.product.refreshPrice().then(function () {
                ctrl.setNeedReinit();
                $timeout(function () {
                    ctrl.callCallbacks(ctrl.callbacks); //когда изменилась верстка надо обновить позиция
                }, 300);
            });
        });
    };

    ctrl.setNeedReinit = function () {
        for (var key in ctrl.needReinit) {
            if (Object.prototype.hasOwnProperty.call(ctrl.needReinit, key)) {
                ctrl.needReinit[key] = true;
            }
        }
    };

    ctrl.cancel = function () {
        ctrl.isShow = false;
        ctrl.editor.container.$.innerHTML = ctrl.startContent;
    };

    ctrl.convertToFloat = function (priceString) {
        var price = priceString
            .replace(/,/g, '.')
            .replace(/ /g, '')
            .replace(/&nbsp;/g, '');

        if (priceString.length === 0) {
            price = 0;
        } else if (/^[0-9]+(\.[0-9][0-9])?$/.test(price) === false) {
            price = null;
        } else {
            price = parseFloat(price);
        }

        return price;
    };

    ctrl.addCallback = function (callback) {
        ctrl.callbacks.push(callback);
    };

    ctrl.callCallbacks = function (callbacksArray) {
        callbacksArray.forEach(function (callback) {
            callback();
        });
    };

    ctrl.destroy = function () {
        $scope.$destroy();
        //$element.remove();
    };
}

export default InplacePriceCtrl;
