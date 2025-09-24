(function (ng) {
    'use strict';

    ng.module('search', []);

    var SearchBlockCtrl = function ($q, $http, $scope, domService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.isFocus = false;
        };

        ctrl.$postLink = function () {};

        ctrl.shouldSelect = function () {
            return false;
        };

        ctrl.find = function (val) {
            return $http.get('search/autocomplete', { params: { q: val, rnd: Math.random() } }).then(function (response) {
                return response.data;
            });
        };

        ctrl.toggle = function () {
            ctrl.isFocus = !ctrl.isFocus;
        };

        ctrl.windowClick = function (event) {
            if (domService.closest(event.target, '.search-header') == null) {
                ctrl.isFocus = false;
                if ($scope.$$phase == null) {
                    $scope.$digest();
                }
            }
        };
    };

    ng.module('search').controller('SearchBlockCtrl', SearchBlockCtrl);

    SearchBlockCtrl.$inject = ['$q', '$http', '$scope', 'domService'];
})(window.angular);
