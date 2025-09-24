(function (ng) {
    'use strict';

    var LpGridGroupCtrl = function ($element, $compile) {
        var ctrl = this;
        ctrl.$onInit = function () {
            if (ctrl.row != null && ctrl.template != null) {
                var el = ng.element(ctrl.template.transcludeHtml);
                $element.append(el);
                $compile(el)(
                    ng.extend(ctrl.template.transcludeScope.$new(), {
                        lpGridRow: ctrl.row,
                    }),
                );
            }
        };
    };

    ng.module('lpGrid').controller('LpGridGroupCtrl', LpGridGroupCtrl);

    LpGridGroupCtrl.$inject = ['$element', '$compile'];
})(window.angular);
