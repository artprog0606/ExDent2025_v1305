(function (ng) {
    'use strict';

    var ModalAddEditPhotoCategoryCtrl = function ($uibModalInstance, $http, $q, $timeout, toaster, Upload, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.id = params.Id != null ? params.Id : 0;
            ctrl.mode = ctrl.id != 0 ? 'edit' : 'add';

            if (ctrl.mode == 'edit') {
                ctrl.getPhotoCategory(ctrl.id);
            } else {
                ctrl.data = {
                    SortOrder: 0,
                    Enabled: true,
                };
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getPhotoCategory = function (id) {
            return $http.get('photoCategory/get', { params: { id: id } }).then(function (response) {
                var data = response.data;
                ctrl.data = data.obj;
                return data;
            });
        };

        ctrl.save = function () {
            var url = ctrl.mode == 'add' ? 'photoCategory/add' : 'photoCategory/update';

            $http.post(url, ctrl.data).then(function (response) {
                var data = response.data;

                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.ChangesSaved'));
                    $uibModalInstance.close();
                } else if (data.errors != null) {
                    data.errors.forEach(function (err) {
                        toaster.pop('error', '', err);
                    });
                }
            });
        };
    };

    ModalAddEditPhotoCategoryCtrl.$inject = ['$uibModalInstance', '$http', '$q', '$timeout', 'toaster', 'Upload', '$translate'];

    ng.module('uiModal').controller('ModalAddEditPhotoCategoryCtrl', ModalAddEditPhotoCategoryCtrl);
})(window.angular);
