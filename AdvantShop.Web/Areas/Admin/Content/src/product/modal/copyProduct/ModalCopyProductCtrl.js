(function (ng) {
    'use strict';

    var ModalCopyProductCtrl = function ($uibModalInstance, $http, $window, toaster, $translate, $sce) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.params = ctrl.$resolve.product;
            ctrl.fromGrid = true;
            if (ctrl.params.productId != null) {
                ctrl.productId = ctrl.params.productId;
                ctrl.fromGrid = false;
            }

            if (ctrl.fromGrid && ctrl.$resolve.name.ids.length == 1) {
                ctrl.productName = ctrl.$resolve.name.ids.pop();
            } else if (ctrl.fromGrid === false) {
                ctrl.productName = ctrl.params.name;
            }

            var name = (ctrl.productName != null ? ctrl.productName : '#PRODUCT_NAME#') + $translate.instant('Admin.Js.Product.Copy');

            ctrl.name = ng.element('<textarea></textarea>').html(name)[0].value;
            ctrl.count = 1;
            ctrl.useTemplates = false;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.copyProduct = function () {
            if (ctrl.fromGrid) {
                $http
                    .post('product/copyProducts', ng.extend(ctrl.params || {}, { copyName: ctrl.name, count: ctrl.count }))
                    .then(function (response) {
                        if (response.data.result == true) {
                            $uibModalInstance.close();
                        } else {
                            toaster.pop('error', response.data.error);
                            ctrl.btnLoading = false;
                        }
                    });
            } else {
                $http.post('product/copyProduct', { productId: ctrl.productId, copyName: ctrl.name, count: ctrl.count }).then(function (response) {
                    if (response.data.result == true) {
                        if (response.data.productId != null)
                            $window.location.assign(window.baseUrl + 'adminv2/product/edit/' + response.data.productId);
                        if (response.data.categoryId != null)
                            $window.location.assign(window.baseUrl + 'adminv2/catalog?categoryId=' + response.data.categoryId);
                    } else {
                        toaster.pop('error', response.data.error);
                        ctrl.btnLoading = false;
                    }
                });
            }
        };

        ctrl.updateCount = function (value) {
            ctrl.count = value;
            if (ctrl.count > 1 && !ctrl.useTemplates) {
                ctrl.useTemplates = true;
                ctrl.name = ctrl.name + ' #N#';
            }
            if (ctrl.count <= 1 && ctrl.useTemplates) {
                ctrl.useTemplates = false;
                ctrl.name = ctrl.name.replace(' #N#', '');
            }
        };
    };

    ModalCopyProductCtrl.$inject = ['$uibModalInstance', '$http', '$window', 'toaster', '$translate', '$sce'];

    ng.module('uiModal').controller('ModalCopyProductCtrl', ModalCopyProductCtrl);
})(window.angular);
