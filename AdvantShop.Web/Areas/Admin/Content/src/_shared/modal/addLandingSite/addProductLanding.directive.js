import addProductLandingTemplate from './addProductLanding.html';
(function (ng) {
    'use strict';

    var AddProductLandingCtrl = function () {
        var ctrl = this;
        ctrl.removeItem = function (item) {
            var index = ctrl.products != null && ctrl.products.length > 0 ? ctrl.products.indexOf(item) : null;
            if (index != null && index !== -1) {
                ctrl.products.splice(index, 1);
                if (ctrl.onClose != null) {
                    ctrl.onClose({
                        result: {
                            ids: ctrl.products.map(function (item) {
                                return item.ProductId;
                            }),
                        },
                    });
                }
            }
        };
    };
    AddProductLandingCtrl.$inject = [];
    ng.module('addProductLanding', [])
        .controller('AddProductLandingCtrl', AddProductLandingCtrl)
        .component('addProductLanding', {
            templateUrl: addProductLandingTemplate,
            controller: 'AddProductLandingCtrl',
            bindings: {
                onClose: '&',
                products: '<',
                settingsSelectvizr: '<?',
            },
        });
})(window.angular);
