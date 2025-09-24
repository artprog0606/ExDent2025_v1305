(function (ng) {
    'use strict';

    var ModalEditTaskObserverCtrl = function ($uibModalInstance, lastStatisticsService, toaster, $translate, tasksService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.parentCtrl = ctrl.$resolve.params.parent;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.changeObserver = function (model) {
            $uibModalInstance.close(model);
        };
    };

    ModalEditTaskObserverCtrl.$inject = ['$uibModalInstance', 'lastStatisticsService', 'toaster', '$translate', 'tasksService'];

    ng.module('uiModal').controller('ModalEditTaskObserverCtrl', ModalEditTaskObserverCtrl);
})(window.angular);
