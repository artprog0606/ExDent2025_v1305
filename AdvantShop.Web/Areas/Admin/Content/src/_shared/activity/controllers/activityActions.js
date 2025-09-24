import activityActionsTemplate from './../templates/activity-actions.html';
(function (ng) {
    'use strict';

    var ActivityActionsCtrl = function ($http) {
        var ctrl = this;
        ctrl.$onInit = function () {
            $http
                .get('activity/getActions', {
                    params: {
                        customerId: ctrl.customerId,
                    },
                })
                .then(function (response) {
                    ctrl.items = response.data.DataItems;
                });
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    activityActions: ctrl,
                });
            }
        };
    };
    ActivityActionsCtrl.$inject = ['$http'];
    ng.module('activity').component('activityActions', {
        templateUrl: activityActionsTemplate,
        controller: ActivityActionsCtrl,
        bindings: {
            customerId: '<?',
            onInit: '&',
        },
    });
})(window.angular);
