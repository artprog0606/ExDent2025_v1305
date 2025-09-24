(function (ng) {
    'use strict';

    var ModalAddEditYandexPromoFlashCtrl = function ($uibModalInstance, $http, toaster, $translate, $window) {
        var ctrl = this;
        ctrl.error = '';

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.value;
            ctrl.ExportFeedId = params.ExportFeedId;
            ctrl.PromoID = params.PromoID != null ? params.PromoID : null;
            ctrl.mode = ctrl.PromoID != null ? 'edit' : 'add';
            ctrl.promo = {
                Type: 'Flash',
                ProductIDs: [],
            };
            if (ctrl.mode == 'edit') {
                $http.post('exportfeeds/GetYandexPromo', { promoID: ctrl.PromoID, exportFeedId: ctrl.ExportFeedId }).then(function (responce) {
                    ctrl.promo = responce.data;
                });
            }
        };

        ctrl.save = function () {
            $http
                .post('exportfeeds/verifyyandexpromo', {
                    exportFeedId: ctrl.ExportFeedId,
                    model: ctrl.promo,
                    editing: ctrl.mode == 'edit',
                })
                .then(function (responce) {
                    if (responce.data != null) {
                        if (responce.data.result == true) {
                            ctrl.promo = responce.data.promo;
                            $uibModalInstance.close(ctrl.promo);
                        } else {
                            ctrl.error = responce.data.errors;
                        }
                    }
                });
        };

        ctrl.addProductsModal = function (products) {
            ctrl.promo.ProductIDs = products.ids;
            ctrl.error = '';
        };

        ctrl.removeProducts = function () {
            ctrl.promo.ProductIDs = [];
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalAddEditYandexPromoFlashCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate', '$window'];

    ng.module('uiModal').controller('ModalAddEditYandexPromoFlashCtrl', ModalAddEditYandexPromoFlashCtrl);
})(window.angular);
