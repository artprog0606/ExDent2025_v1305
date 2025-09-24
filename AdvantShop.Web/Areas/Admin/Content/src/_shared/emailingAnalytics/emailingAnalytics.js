import emailingAnalyticsTemplate from './templates/emailingAnalytics.html';
(function (ng) {
    'use strict';

    var refetchFields = ['emailDateFrom', 'emailDateTo'];
    var EmailingAnalyticsCtrl = function ($http, $httpParamSerializer) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.fetch();
        };
        ctrl.$onChanges = function (changes) {
            var needRefetch = refetchFields.some(function (item) {
                return changes[item] != null && changes[item].isFirstChange() === false && changes[item].previousValue !== changes[item].currentValue;
            });
            if (needRefetch === true) {
                ctrl.fetch();
            }
        };
        ctrl.fetch = function () {
            ctrl.dataLoaded = false;
            $http
                .get(ctrl.emailDataUrl, {
                    params: Object.assign(
                        {},
                        {
                            id: ctrl.emailingId,
                            dateFrom: ctrl.emailDateFrom,
                            dateTo: ctrl.emailDateTo,
                        },
                        ctrl.requestParams || {},
                    ),
                })
                .then(function (response) {
                    ctrl.chartData = response.data.obj.ChartData;
                    ctrl.statusesData = response.data.obj.StatusesData;
                    if (ctrl.onChangeDate != null) {
                        ctrl.onChangeDate({
                            dateFrom: ctrl.emailDateFrom,
                            dateTo: ctrl.emailDateTo,
                        });
                    }
                })
                .finally(function () {
                    ctrl.dataLoaded = true;
                });
        };
        ctrl.getStatusUrlParams = function (statusName) {
            var dateParams = '';
            if (ctrl.emailDateFrom != '' && ctrl.emailDateFrom != undefined) {
                if (ctrl.emailDateFrom instanceof Date) ctrl.emailDateFrom = ctrl.emailDateFrom.toISOString().split('T')[0];
                if (ctrl.hideFlatpickr) ctrl.emailDateFrom = ctrl.emailDateFrom.split('T')[0].replace('"', '');
                dateParams += '"DateFrom":"' + ctrl.emailDateFrom.split('.').reverse().join('-') + '"';
            }
            if (ctrl.emailDateTo != '' && ctrl.emailDateTo != undefined) {
                if (ctrl.emailDateTo instanceof Date) ctrl.emailDateTo = ctrl.emailDateTo.toISOString().split('T')[0];
                if (ctrl.hideFlatpickr) ctrl.emailDateTo = ctrl.emailDateTo.split('T')[0].replace('"', '');
                dateParams += (dateParams == '' ? '"DateFrom":"",' : ',') + '"DateTo":"' + ctrl.emailDateTo.split('.').reverse().join('-') + '"';
            }
            var statusParams = '';
            if (statusName) {
                statusParams = '"Statuses":"' + statusName + '"';
            }
            if (dateParams != '' || statusParams != '') {
                //return '#?' + ctrl.gridName + '={' + dateParams + (dateParams == '' || statusParams == '' ? '' : ',') + statusParams + '}';
                return '{' + dateParams + (dateParams == '' || statusParams == '' ? '' : ',') + statusParams + '}';
            }
            return '{}';
        };
        ctrl.getStatusUrlParamsForMVC = function (statusName) {
            var data = JSON.parse(ctrl.getStatusUrlParams(statusName));
            return $httpParamSerializer(data);
        };
    };
    EmailingAnalyticsCtrl.$inject = ['$http', '$httpParamSerializer'];
    ng.module('emailingAnalytics', [])
        .controller('EmailingAnalyticsCtrl', EmailingAnalyticsCtrl)
        .component('emailingAnalytics', {
            templateUrl: emailingAnalyticsTemplate,
            controller: 'EmailingAnalyticsCtrl',
            bindings: {
                emailingId: '@',
                emailSubject: '@',
                sendTime: '@',
                emailLogUrl: '<?',
                emailComeBackUrl: '<?',
                emailDataUrl: '@',
                emailDateFrom: '<?',
                emailDateTo: '<?',
                hideFlatpickr: '<',
                emailComeBackClick: '&',
                emailLogClick: '&',
                onChangeDate: '&',
                hideComeBackLink: '<',
                requestParams: '<?',
            },
        });
})(window.angular);
