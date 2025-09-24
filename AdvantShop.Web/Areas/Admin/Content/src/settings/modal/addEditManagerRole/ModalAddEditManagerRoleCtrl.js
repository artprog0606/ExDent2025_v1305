(function (ng) {
    'use strict';

    var ModalAddEditManagerRoleCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;
        ctrl.formInited = false;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.id = params.id != null ? params.id : 0;
            ctrl.mode = ctrl.id != 0 ? 'edit' : 'add';

            if (ctrl.mode == 'add') {
                ctrl.sortOrder = 0;
                ctrl.formInited = true;
            } else {
                ctrl.getManagerRole(ctrl.id);
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getManagerRole = function (id) {
            $http.get('managerRoles/getManagerRole', { params: { id: id, rnd: Math.random() } }).then(function (response) {
                var data = response.data;
                if (data != null) {
                    ctrl.name = data.Name;
                    ctrl.sortOrder = data.SortOrder;
                }
                ctrl.addEditManagerRoleForm.$setPristine();
                ctrl.formInited = true;
            });
        };

        ctrl.save = function () {
            ctrl.btnSleep = true;

            var params = {
                id: ctrl.id,
                name: ctrl.name,
                sortOrder: ctrl.sortOrder,
                rnd: Math.random(),
            };

            var url = ctrl.mode == 'add' ? 'managerRoles/addManagerRole' : 'managerRoles/updateManagerRole';

            $http.post(url, params).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.pop(
                        'success',
                        '',
                        ctrl.mode == 'add'
                            ? $translate.instant('Admin.Js.Settings.AddEdit.RoleAdded')
                            : $translate.instant('Admin.Js.Settings.AddEdit.ChangesSaved'),
                    );
                    $uibModalInstance.close('saveManagerRole');
                } else {
                    toaster.pop(
                        'error',
                        $translate.instant('Admin.Js.Settings.AddEdit.Error'),
                        $translate.instant('Admin.Js.Settings.AddEdit.ErrorWhile') +
                            (ctrl.mode == 'add'
                                ? $translate.instant('Admin.Js.Settings.AddEdit.Creating')
                                : $translate.instant('Admin.Js.Settings.AddEdit.Editing')),
                    );
                    ctrl.btnSleep = false;
                }
            });
        };
    };

    ModalAddEditManagerRoleCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalAddEditManagerRoleCtrl', ModalAddEditManagerRoleCtrl);
})(window.angular);
