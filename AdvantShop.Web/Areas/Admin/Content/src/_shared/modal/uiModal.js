(function (ng) {
    'use strict';

    var UiModalTriggerCtrl = /* @ngInject */ function ($rootScope, $scope, $uibModal) {
        var ctrl = this;

        ctrl.open = function () {
            var options = {
                bindToController: true,
                controller: ctrl.controller,
                controllerAs: ctrl.controllerAs || 'ctrl',
                template: ctrl.template,
                templateUrl: ctrl.templateUrl,
                resolve: ctrl.resolveParse != null ? ctrl.resolveParse($scope.$parent) : null,
                size: ctrl.size,
                windowClass: ctrl.windowClass,
                backdrop: ctrl.backdrop != null ? (ctrl.backdrop === 'static' ? ctrl.backdrop : ctrl.backdrop === 'true') : true,
                keyboard: ctrl.keyboard != null ? ctrl.keyboard : true,
                animation: ctrl.animation != null ? ctrl.animation : true,
                openedClass: ctrl.openedClass,
                scope: ctrl.scope,
                component: ctrl.component,
            };

            if (ctrl.onBeforeOpen != null) {
                ctrl.onBeforeOpen();
            }

            $uibModal.open(options).result.then(
                function (result) {
                    if (ctrl.onClose != null) {
                        ctrl.onClose({ result: result });
                    }
                    return result;
                },
                function (result) {
                    if (ctrl.onDismiss != null) {
                        ctrl.onDismiss({ result: result });
                    }
                    return result;
                },
            );
        };
    };

    ng.module('uiModal', ['uiGridCustom', 'isMobile', 'ui.bootstrap.modal']).controller('UiModalTriggerCtrl', UiModalTriggerCtrl);
})(window.angular);
