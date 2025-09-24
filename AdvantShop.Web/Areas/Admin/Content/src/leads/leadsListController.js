(function (ng) {
    'use strict';

    var LeadsListCtrl = function (leadsService, $element, $compile, $scope, $parse, $attrs) {
        var ctrl = this;

        ctrl.$onInit = function () {
            leadsService.addLeadsList(ctrl);
            ctrl.update();
        };

        ctrl.update = function () {
            return leadsService.fetchDataList($attrs.excludeLeadListId).then(function (data) {
                ctrl.data = data;
                var html = $compile(ctrl.data)($scope);
                ng.element($element[0]).after(html);
                $element.remove();
            });
        };
    };

    LeadsListCtrl.$inject = ['leadsService', '$element', '$compile', '$scope', '$parse', '$attrs'];

    ng.module('leads').controller('LeadsListCtrl', LeadsListCtrl);
})(window.angular);
