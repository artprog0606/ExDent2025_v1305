(function (ng) {
    'use strict';

    /* @ngInject */
    var ModalUserInfoPopupInviteCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.userList = [
                {
                    FirstName: null,
                    LastName: null,
                    Email: null,
                },
            ];
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.addFieldsUser = function () {
            ctrl.userList.push({
                FirstName: null,
                LastName: null,
                Email: null,
            });
        };

        ctrl.removeUser = function (user) {
            var index = ctrl.userList.indexOf(user);

            if (index !== -1) {
                ctrl.userList.splice(index, 1);
            } else {
                throw Error('Not found user in invite list');
            }
        };

        ctrl.invite = function () {
            ctrl.btnLoading = true;
            $http.post('users/invite', { users: ctrl.userList }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.UserInfoPopup.InvitesSent'));
                    $uibModalInstance.close();
                } else {
                    toaster.pop('error', '', data.errors);
                }
                ctrl.btnLoading = false;
            });
        };
    };

    ng.module('uiModal').controller('ModalUserInfoPopupInviteCtrl', ModalUserInfoPopupInviteCtrl);
})(window.angular);
