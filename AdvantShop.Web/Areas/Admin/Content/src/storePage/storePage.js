(function (ng) {
    'use strict';

    var StorePageCtrl = function ($http, $window, SweetAlert, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.tab = '0';
        };

        ctrl.changeTab = function (tab) {
            if (tab != null) {
                ctrl.tab = tab;
            }
            //$location.search(TAB_SEARCH_NAME, ctrl.tab);
        };
    };

    StorePageCtrl.$inject = ['$http', '$window', 'SweetAlert', '$translate'];

    ng.module('storePage', []).controller('StorePageCtrl', StorePageCtrl);
})(window.angular);
