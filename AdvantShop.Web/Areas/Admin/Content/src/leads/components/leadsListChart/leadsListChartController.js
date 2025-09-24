(function (ng) {
    'use strict';

    var LeadsListChartCtrl = function ($http, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.loadData();
        };

        ctrl.loadData = function () {
            $http
                .post('leads/LeadsAnalyticsChartPartial', {
                    leadsListId: ctrl.leadsListId,
                    dateFrom: ctrl.dateFrom,
                    dateTo: ctrl.dateTo,
                })
                .then(function (response) {
                    if (response.data.result) {
                        ctrl.data = response.data.obj;
                        ctrl.dateFrom = response.data.obj.DateFrom;
                        ctrl.dateTo = response.data.obj.DateTo;
                    }
                });
        };
    };

    LeadsListChartCtrl.$inject = ['$http', '$translate'];

    ng.module('leads').controller('LeadsListChartCtrl', LeadsListChartCtrl);
})(window.angular);
