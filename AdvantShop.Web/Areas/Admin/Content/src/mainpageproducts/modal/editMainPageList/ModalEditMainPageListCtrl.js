(function (ng) {
    'use strict';

    var ModalEditMainPageListCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.type = params.type != null ? params.type : 0;
            ctrl.typeStr = params.data != null && params.data.typeStr != null ? params.data.typeStr : null;

            $http
                .get('mainpageproducts/getMainPageList', {
                    params: { type: ctrl.type != 0 ? ctrl.type : ctrl.typeStr },
                })
                .then(function (response) {
                    ctrl.data = response.data;
                });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function () {
            var params = ctrl.data;
            $http.post('mainpageproducts/updateMainPageList', params).then(function (response) {
                var data = response.data;

                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.MainPageProducts.ChangesSaved'));
                    $uibModalInstance.close();
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', '', error);
                    });
                }
            });
        };
    };

    ModalEditMainPageListCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalEditMainPageListCtrl', ModalEditMainPageListCtrl);
})(window.angular);
