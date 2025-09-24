(function (ng) {
    'use strict';

    angular.module('module').directive('module', [
        '$q',
        '$sce',
        'moduleService',
        function ($q, $sce, moduleService) {
            return {
                restrict: 'A',
                scope: {
                    key: '@',
                    updateOnLoad: '<?',
                },
                controller: 'ModuleCtrl',
                controllerAs: 'module',
                bindToController: true,
                replace: true,
                transclude: true,
                template: '<div data-ng-bind-html="module.content"></div>',
                link: function (scope, element, attrs, ctrl, transclude) {
                    if (ctrl.updateOnLoad === true) {
                        moduleService.add(ctrl.key, ctrl);
                        moduleService.update(ctrl.key);
                    } else {
                        ctrl.content = $sce.trustAsHtml(angular.element('<div />').html(transclude()).html());
                        moduleService.add(ctrl.key, ctrl);
                    }
                },
            };
        },
    ]);
})(window.angular);
