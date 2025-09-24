import activityCallsTemplate from './../templates/activity-calls.html';
(function (ng) {
    'use strict';

    var ActivityCallsCtrl = function ($http) {
        var ctrl = this;
        if (ctrl.standardPhone == null || ctrl.standardPhone == '') {
            return;
        }
        ctrl.$onInit = function () {
            $http
                .get('activity/getCalls', {
                    params: {
                        customerId: ctrl.customerId,
                        standardPhone: ctrl.standardPhone,
                    },
                })
                .then(function (response) {
                    ctrl.items = response.data.DataItems;
                });
        };
    };
    ActivityCallsCtrl.$inject = ['$http'];
    ng.module('activity').component('activityCalls', {
        templateUrl: activityCallsTemplate,
        controller: ActivityCallsCtrl,
        bindings: {
            customerId: '<?',
            standardPhone: '<?',
        },
    });
})(window.angular);
