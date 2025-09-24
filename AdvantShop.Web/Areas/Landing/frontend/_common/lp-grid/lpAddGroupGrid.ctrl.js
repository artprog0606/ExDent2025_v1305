(function (ng) {
    'use strict';

    var LpAddGroupGridCtrl = function ($transclude, lpGridTypes, $element, $scope) {
        var ctrl = this,
            el = $element[0];

        ctrl.$onInit = function () {
            ctrl.transcludeHtml = el.innerHTML;
            ctrl.type = 'group';
            ctrl.transcludeScope = $scope;
            ctrl.lpGrid.addGroup(ctrl);
        };

        ctrl.$onDestroy = function () {
            ctrl.transcludeScope.$destroy();
        };
    };

    ng.module('lpGrid').controller('LpAddGroupGridCtrl', LpAddGroupGridCtrl);

    LpAddGroupGridCtrl.$inject = ['$transclude', 'lpGridTypes', '$element', '$scope'];
})(window.angular);
