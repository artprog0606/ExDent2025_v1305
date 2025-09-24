(function (ng) {
    'use strict';

    ng.module('bookingCart').directive('bookingCartCount', [
        '$sce',
        function ($sce) {
            return {
                restrict: 'A',
                scope: true,
                controller: 'BookingCartCountCtrl',
                controllerAs: 'bookingCartCount',
                bindToController: true,
                link: function (scope, element, attrs, ctrl, transclude) {
                    ctrl.type = attrs.type;
                    var startValue = element.html();
                    ctrl.startValue = $sce.trustAsHtml(startValue);
                },
            };
        },
    ]);
})(window.angular);
