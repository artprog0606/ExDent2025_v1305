(function (ng) {
    'use strict';

    /* @ngInject */
    var RecalcCtrl = function ($http) {
        var ctrl = this;

        ctrl.recalc = function () {
            $http.post('catalog/recalculateproductscount').then(function (response) {
                location.reload();
            });
        };
    };

    ng.module('recalc', []).controller('RecalcCtrl', RecalcCtrl).component('recalcTrigger', {
        template: '<a href="" data-ng-click="$ctrl.recalc()" ng-transclude></a>',
        controller: RecalcCtrl,
        transclude: true,
    });
})(window.angular);
