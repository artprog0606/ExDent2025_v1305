(function (ng) {
    'use strict';

    var TriggerAnalyticsCtrl = function ($http, toaster) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var _dateNow = new Date();
            var _dateFrom = new Date();
            _dateFrom = new Date(_dateFrom.setDate(_dateNow.getDate() - 14));
            ctrl.dateFrom = new Date(_dateFrom.setHours(0, 0, 0, 0));
            ctrl.dateTo = new Date(_dateNow.setHours(23, 59, 59, 999));
        };
    };

    TriggerAnalyticsCtrl.$inject = ['$http', 'toaster'];

    ng.module('triggerAnalytics', ['emailingAnalytics', 'emailingLog']).controller('TriggerAnalyticsCtrl', TriggerAnalyticsCtrl);
})(window.angular);
