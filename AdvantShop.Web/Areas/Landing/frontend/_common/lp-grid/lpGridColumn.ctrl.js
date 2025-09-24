(function (ng) {
    'use strict';

    var blank = ng.element(document.createElement('div'));

    var LpGridColumnCtrl = function ($transclude, lpGridTypes) {
        var ctrl = this;

        ctrl.$onInit = function () {
            if (!ctrl.visible) {
                if (ctrl.type === lpGridTypes.template) {
                    $transclude(function (clone, scope) {
                        ctrl.transcludeHtml = blank.append(clone).html();
                        ctrl.transcludeScope = scope;
                        blank.html('');
                    });
                }

                ctrl.lpGrid.addColumn(ctrl);
            }
        };

        ctrl.$onDestroy = function () {
            if (ctrl.transcludeScope != null) {
                ctrl.transcludeScope.$destroy();
            }

            var index = ctrl.lpGrid.columns.indexOf(ctrl);

            if (index !== -1) {
                ctrl.lpGrid.columns.splice(index, 1);
            }
        };
    };

    ng.module('lpGrid').controller('LpGridColumnCtrl', LpGridColumnCtrl);

    LpGridColumnCtrl.$inject = ['$transclude', 'lpGridTypes'];
})(window.angular);
