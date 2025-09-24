import managersReportTemplate from './managersReport.html';
(function (ng) {
    'use strict';

    var ManagersReportCtrl = function ($http) {
        var ctrl = this;
        ctrl.$onInit = function () {
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    managers: ctrl,
                });
            }
        };
        ctrl.recalc = function (dateFrom, dateTo) {
            ctrl.fetch(dateFrom, dateTo);
        };
        ctrl.fetch = function (dateFrom, dateTo) {
            $http
                .get('analytics/getManagers', {
                    params: {
                        dateFrom: dateFrom,
                        dateTo: dateTo,
                    },
                })
                .then(function (result) {
                    ctrl.Managers = result.data;
                });
        };
    };
    ManagersReportCtrl.$inject = ['$http'];
    ng.module('analyticsReport')
        .controller('ManagersReportCtrl', ManagersReportCtrl)
        .component('managersReport', {
            templateUrl: managersReportTemplate,
            controller: ManagersReportCtrl,
            bindings: {
                onInit: '&',
            },
        });
})(window.angular);
