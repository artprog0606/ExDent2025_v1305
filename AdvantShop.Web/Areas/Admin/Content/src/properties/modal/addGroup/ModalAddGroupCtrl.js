import './AddGroup.html';
(function (ng) {
    'use strict';

    var ModalAddGroupCtrl = function ($uibModalInstance, $http, $window, urlHelper, toaster, $translate, isMobileService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.groupId = params.groupId != null ? params.groupId : 0;
            ctrl.mode = ctrl.groupId != 0 ? 'edit' : 'add';

            if (ctrl.mode == 'add') {
                ctrl.sortOrder = 0;
            } else {
                $http.get('properties/getGroup', { params: { groupId: ctrl.groupId } }).then(function (response) {
                    var data = response.data;
                    ctrl.name = data.Name;
                    ctrl.nameDisplayed = data.NameDisplayed;
                    ctrl.sortOrder = data.SortOrder;
                });
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.addGroup = function () {
            if (ctrl.name == null || ctrl.name === '') return;

            if (ctrl.mode == 'add') {
                $http
                    .post('properties/addGroup', {
                        name: ctrl.name,
                        nameDisplayed: ctrl.nameDisplayed,
                        sortOrder: ctrl.sortOrder,
                    })
                    .then(function (response) {
                        $uibModalInstance.close({ groupId: response.groupId, name: ctrl.name });
                    })
                    .then(function (res) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Properties.ChangesSaved'));
                        if (isMobileService.getValue()) {
                            document.querySelector('.popover-backdrop').click();
                        }
                    })
                    .catch(function (error) {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Properties.Error'));
                    });
            } else {
                $http
                    .post('properties/updateGroup', {
                        propertyGroupId: ctrl.groupId,
                        name: ctrl.name,
                        nameDisplayed: ctrl.nameDisplayed,
                        sortOrder: ctrl.sortOrder,
                    })
                    .then(function (response) {
                        $uibModalInstance.close('');
                    })
                    .then(function (res) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Properties.ChangesSaved'));
                    })
                    .catch(function (error) {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Properties.Error'));
                    });
            }
        };
    };

    ModalAddGroupCtrl.$inject = ['$uibModalInstance', '$http', '$window', 'urlHelper', 'toaster', '$translate', 'isMobileService'];

    ng.module('uiModal').controller('ModalAddGroupCtrl', ModalAddGroupCtrl);
})(window.angular);
