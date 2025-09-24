(function (ng) {
    'use strict';

    ng.module('uiModal')
        //.component('uiModalTrigger', {
        //    template: '<span ng-click="$ctrl.open()" ng-transclude></span>',
        //    controller: 'UiModalTriggerCtrl',
        //    transclude: true,
        //    bindings: {
        //        controller: '<',
        //        resolve: '<?',
        //        controllerAs: '@',
        //        template: '@',
        //        templateUrl: '@',
        //        size: '@',
        //        backdrop: '@',
        //        windowClass: '@',
        //        onClose: '&',
        //        onDismiss: '&',
        //        onBeforeOpen: '&',
        //        keyboard: '<?'
        //    }
        //})
        .directive('uiModalTrigger', [
            '$parse',
            function ($parse) {
                return {
                    controller: 'UiModalTriggerCtrl',
                    bindToController: true,
                    scope: {
                        controller: '<',
                        resolve: '@',
                        controllerAs: '@',
                        template: '@',
                        templateUrl: '@',
                        size: '@',
                        backdrop: '@',
                        windowClass: '@',
                        onClose: '&',
                        onDismiss: '&',
                        onBeforeOpen: '&',
                        keyboard: '<?',
                        animation: '<?',
                        openedClass: '@',
                        scope: '<?',
                        component: '@',
                        isDisabled: '<?',
                    },
                    link: function (scope, element, attrs, ctrl) {
                        if (attrs.resolve != null && attrs.resolve.length > 0) {
                            ctrl.resolveParse = $parse(attrs.resolve);
                        }
                        element.on('click', function () {
                            ctrl.isDisabled = ctrl.isDisabled == null ? false : ctrl.isDisabled;

                            if (ctrl.isDisabled === false) {
                                ctrl.open();
                                scope.$digest();
                            }
                        });
                    },
                };
            },
        ])
        .component('uiModalCross', {
            template: '<div class="close" ng-click="$ctrl.close()"></div>',
            bindings: {
                closeFn: '&',
            },
            controller: [
                '$uibModalStack',
                '$attrs',
                function ($uibModalStack, $attrs) {
                    this.close = function () {
                        if ($attrs.closeFn != null) {
                            this.closeFn();
                        } else {
                            $uibModalStack.getTop().key.dismiss('crossClick');
                        }
                    };
                },
            ],
        })
        .component('uiModalDismiss', {
            transclude: true,
            template: '<span ng-click="$ctrl.close()" ng-transclude></span>',
            controller: [
                '$uibModalStack',
                function ($uibModalStack) {
                    this.close = function () {
                        $uibModalStack.getTop().key.dismiss('crossClick');
                    };
                },
            ],
        });
})(window.angular);
