import salesChannelsGraphTemplate from './salesChannelsGraph.html';
(function (ng) {
    'use strict';

    var SalesChannelsGraphCtrl = function ($http) {
        var ctrl = this;
        var countDaysInRange;
        ctrl.$onInit = function () {
            ctrl.chartOptions = {
                scales: {
                    y: {
                        min: 0,
                    },
                },
                elements: {
                    line: {
                        tension: 0.4,
                        fill: true,
                    },
                },
            };
            countDaysInRange = {
                week: 7,
                month: 31,
                // 'year': 365
                year: ctrl.getDaysOfAYear(new Date().getFullYear()),
            };
        };
        ctrl.$postLink = function () {
            ctrl.range = 'week';
            ctrl.dateRange = ctrl.getRange(ctrl.range);
            ctrl.group = 'day';
            ctrl.selectedSalesChannelIds = ['All'];
            ctrl.fetchData().then(function (data) {
                ctrl.group = data.GroupDate;
            });
        };
        ctrl.getRange = function (rangeName) {
            const dateNow = new Date();
            const dateToResult = new Date(new Date().setHours(23, 59, 59, 0));
            const diff = dateNow.getDate() - countDaysInRange[rangeName];
            const _dateFrom = new Date(dateNow.setDate(diff));
            const dateFromResult = new Date(_dateFrom.setHours(0, 0, 0, 0));
            return [dateFromResult, dateToResult];
        };
        ctrl.selectSalesChannel = function (id) {
            let index = ctrl.selectedSalesChannelIds.indexOf(id);
            if (index === -1) {
                ctrl.selectedSalesChannelIds.push(id);
            } else {
                ctrl.selectedSalesChannelIds.splice(index, 1);
            }
            ctrl.fetchData();
        };
        ctrl.isChecked = function (id) {
            return ctrl.selectedSalesChannelIds.indexOf(id) > -1;
        };
        ctrl.changeDate = function (selectedDates, dateStr, instance) {
            if (selectedDates.length === 2) {
                ctrl.dateRange = selectedDates;
                ctrl.fetchData();
            }
        };
        ctrl.setRange = function (rangeName) {
            if (countDaysInRange[rangeName] != null) {
                ctrl.range = rangeName;
                ctrl.dateRange = ctrl.getRange(rangeName);
                ctrl.fetchData();
            }
        };
        ctrl.changeGroup = function () {
            ctrl.fetchData();
        };
        ctrl.changePaid = function () {
            ctrl.fetchData();
        };
        ctrl.fetchData = function () {
            let params = {
                selectedSalesChannelIds: ctrl.selectedSalesChannelIds,
                groupDate: ctrl.group,
                dateFrom: ctrl.dateRange[0],
                dateTo: ctrl.dateRange[1],
                onlyPaid: !ctrl.onlyPaid ? null : ctrl.onlyPaid,
            };
            return $http
                .get('home/getSalesChannelsGraphDashboardGraph', {
                    params: params,
                })
                .then(function (result) {
                    let data = result.data;
                    ctrl.Graph = data.Graph;
                    ctrl.Grid = data.Grid;
                    return data;
                });
        };
        ctrl.getDaysOfAYear = function (year) {
            return ctrl.isLeapYear(year) ? 366 : 365;
        };
        ctrl.isLeapYear = function (year) {
            return year % 400 === 0 || (year % 100 !== 0 && year % 4 === 0);
        };
    };
    SalesChannelsGraphCtrl.$inject = ['$http'];
    ng.module('home').controller('SalesChannelsGraphCtrl', SalesChannelsGraphCtrl).component('salesChannelsGraph', {
        templateUrl: salesChannelsGraphTemplate,
        controller: SalesChannelsGraphCtrl,
    });
})(window.angular);
