(function (ng) {
    'use strict';

    var ModalAddPropertyGroupCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;
        ctrl.emptyGroup = { label: 'Не выбрано' };

        ctrl.$onInit = function () {
            $http.get('category/getAllPropertyGroups').then(function (response) {
                ctrl.groups = response.data;
                if (response.data != null && response.data.length > 0) {
                    ctrl.group = response.data[0];
                } else {
                    ctrl.groups = [ctrl.emptyGroup];
                    ctrl.group = ctrl.groups[0];
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.addPropertyGroup = function () {
            $http
                .post('category/addgrouptocategory', {
                    categoryId: ctrl.$resolve.categoryId,
                    groupId: ctrl.group.value,
                })
                .then(function (response) {
                    if (response.data == true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Certificates.ChangesSaved'));
                    } else {
                        toaster.pop(
                            'error',
                            $translate.instant('Admin.Js.Certificates.Error'),
                            $translate.instant('Admin.Js.Category.ChangesNotSaved'),
                        );
                    }
                    $uibModalInstance.close();
                });
        };
    };

    ModalAddPropertyGroupCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalAddPropertyGroupCtrl', ModalAddPropertyGroupCtrl);
})(window.angular);
