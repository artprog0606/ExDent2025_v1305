(function (ng) {
    'use strict';

    angular.module('windowExt').directive('windowResize', [
        'windowService',
        function (windowService) {
            return {
                restrict: 'A',
                scope: {
                    windowResize: '&',
                },
                controller: 'WindowCtrl',
                controllerAs: 'windowCtrl',
                bindToController: true,
                link: function (scope, element, attrs, ctrl) {
                    windowService.addCallback('resize', ctrl.windowResize);
                },
            };
        },
    ]);

    angular.module('windowExt').directive('windowScroll', [
        'windowService',
        function (windowService) {
            return {
                restrict: 'A',
                scope: {
                    windowScroll: '&',
                },
                controller: 'WindowCtrl',
                controllerAs: 'windowCtrl',
                bindToController: true,
                link: function (scope, element, attrs, ctrl) {
                    windowService.addCallback('scroll', ctrl.windowScroll);
                },
            };
        },
    ]);

    angular.module('windowExt').directive('windowClick', [
        'windowService',
        function (windowService) {
            return {
                restrict: 'A',
                scope: {
                    windowClick: '&',
                },
                controller: 'WindowCtrl',
                controllerAs: 'windowCtrl',
                bindToController: true,
                link: function (scope, element, attrs, ctrl) {
                    windowService.addCallback('click', ctrl.windowClick);
                    windowService.addCallback('touchstart', ctrl.windowClick);
                },
            };
        },
    ]);

    angular.module('windowExt').directive('windowPrint', [
        'windowService',
        function (windowService) {
            return {
                restrict: 'A',
                controller: 'WindowCtrl',
                controllerAs: 'windowCtrl',
                bindToController: true,
                link: function (scope, element, attrs, ctrl) {
                    var func = function () {
                        windowService.open(attrs.windowPrint, attrs.windowPrintName, attrs.windowPrintParams);
                    };

                    windowService.addCallBack('click', func);
                    windowService.addCallBack('touchend', func);
                },
            };
        },
    ]);
})(angular);
