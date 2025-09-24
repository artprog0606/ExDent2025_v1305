(function (ng) {
    'use strict';

    var ModalAddEditApiWebhookCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.mode = params ? 'edit' : 'add';
            ctrl.apiWebhook = params;

            ctrl.loadTypes();
        };

        ctrl.save = function () {
            ctrl.apiWebhook.EventTypeName = ctrl.eventTypes.find(function (item) {
                return item.value == ctrl.apiWebhook.EventType;
            }).label;

            $uibModalInstance.close({
                apiWebhook: ctrl.apiWebhook,
            });
        };

        ctrl.loadTypes = function () {
            return $http.get('settingsApi/getEventTypes').then(function (response) {
                if (response.data.result === true) {
                    ctrl.eventTypes = response.data.obj;
                } else {
                    response.data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });

                    if (!response.data.errors) {
                        toaster.pop('error', 'Не удалось загрузить события webhooks');
                    }
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalAddEditApiWebhookCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalAddEditApiWebhookCtrl', ModalAddEditApiWebhookCtrl);
})(window.angular);
