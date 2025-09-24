import abcxyzAnalysisTemplate from './abcxyzAnalysis.html';
(function (ng) {
    'use strict';

    var AbcxyzAnalysisCtrl = function ($http) {
        var ctrl = this;
        ctrl.$onInit = function () {
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    abcxyz: ctrl,
                });
            }
        };
        ctrl.recalc = function (dateFrom, dateTo) {
            ctrl.from = dateFrom;
            ctrl.to = dateTo;
            ctrl.fetch();
            ctrl.getData();
        };
        ctrl.fetch = function () {
            $http
                .get('analytics/getAbcxyzAnalysis', {
                    params: {
                        dateFrom: ctrl.from,
                        dateTo: ctrl.to,
                    },
                })
                .then(function (result) {
                    ctrl.Data = result.data;
                });
        };
        ctrl.getData = function () {
            $http.get('analytics/getCatalogData').then(function (result) {
                ctrl.ProductsData = result.data;
            });
        };
        ctrl.showProducts = function (group) {
            var url = 'analytics/analyticsFilter?from=' + ctrl.from + '&to=' + ctrl.to + '&group=' + group + '&type=abcxyz';
            var win = window.open(url, '_blank');
            win.focus();
        };
    };
    AbcxyzAnalysisCtrl.$inject = ['$http'];
    ng.module('analyticsReport')
        .controller('AbcxyzAnalysisCtrl', AbcxyzAnalysisCtrl)
        .component('abcxyzAnalysis', {
            templateUrl: abcxyzAnalysisTemplate,
            controller: AbcxyzAnalysisCtrl,
            bindings: {
                onInit: '&',
            },
        });
})(window.angular);
