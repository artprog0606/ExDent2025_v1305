(function (ng) {
    'use strict';

    var ModalAddEditBookingTagCtrl = function ($uibModalInstance, $http, toaster, $translate) {
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
                ctrl.getTag(ctrl.id);
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getTag = function (id) {
            $http.get('settingsBooking/getTag', { params: { id: id } }).then(function (response) {
                var data = response.data;
                if (data != null) {
                    ctrl.name = data.Name;
                    ctrl.sortOrder = data.SortOrder;
                }
                ctrl.addEditBookingTagForm.$setPristine();
                ctrl.formInited = true;
            });
        };

        ctrl.save = function () {
            ctrl.btnSleep = true;

            var params = {
                id: ctrl.id,
                name: ctrl.name,
                sortOrder: ctrl.sortOrder,
            };

            var url = ctrl.mode == 'add' ? 'settingsBooking/addTag' : 'settingsBooking/updateTag';

            $http.post(url, params).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.pop('success', '', ctrl.mode == 'add' ? 'Тег добавлен' : 'Тег обновлен');
                    $uibModalInstance.close('saveBookingTag');
                } else {
                    toaster.pop('error', 'Ошибка', 'Не удалось сохранить данные');
                    ctrl.btnSleep = false;
                }
            });
        };
    };

    ModalAddEditBookingTagCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalAddEditBookingTagCtrl', ModalAddEditBookingTagCtrl);
})(window.angular);
