import './editComment.html';
(function (ng) {
    'use strict';

    var ModalEditCommentCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            if (ctrl.$resolve != null && ctrl.$resolve.params != null) {
                ctrl.id = ctrl.$resolve.params.id;
                if (ctrl.$resolve.params.message != null) {
                    ctrl.text = ctrl.$resolve.params.message;
                }
            }
        };

        ctrl.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function (id, text) {
            $http.post('adminComments/update', { id: id, text: text }).then(function (response) {
                if (response.data.Result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Partials.ChangesSaved'));
                    $uibModalInstance.close(ctrl.text);
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.Partials.Error'));
                }
            });
        };
    };

    ModalEditCommentCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalEditCommentCtrl', ModalEditCommentCtrl);
})(window.angular);
