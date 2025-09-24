(function (ng) {
    'use strict';

    var ModalChangeLeadManagerCtrl = function ($http, $uibModalInstance, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var resolve = ctrl.$resolve;
            ctrl.params = resolve.params;
            ctrl.getManagers();
        };

        ctrl.getManagers = function () {
            $http.get('managers/getManagersSelectOptions?roleActions=Crm').then(function (response) {
                ctrl.managers = [{ label: '-', value: '' }].concat(response.data);
                ctrl.newManagerId = ctrl.managers[0].value;
            });
        };

        ctrl.save = function () {
            ctrl.btnSaveDisabled = true;

            $http.post('leads/changeManager', ng.extend(ctrl.params || {}, { newManagerId: ctrl.newManagerId })).then(function (response) {
                if (response.data.result === true) {
                    toaster.pop('success', '', 'Изменения успешно сохранены');
                }
                $uibModalInstance.close();
                ctrl.btnSaveDisabled = false;
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalChangeLeadManagerCtrl.$inject = ['$http', '$uibModalInstance', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalChangeLeadManagerCtrl', ModalChangeLeadManagerCtrl);
})(window.angular);
