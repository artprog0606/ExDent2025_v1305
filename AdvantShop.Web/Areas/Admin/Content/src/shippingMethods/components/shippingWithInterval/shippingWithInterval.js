import shippingWithIntervalTemplate from './templates/shippingWithInterval.html';
(function (ng) {
    'use strict';

    var ShippingWithIntervalCtrl = function ($http, toaster) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.data = {
                deliveryIntervals: {},
                timeZoneId: ctrl.timeZoneId,
                timeZones: JSON.parse(ctrl.timeZones),
                countVisibleDeliveryDay: !ctrl.countVisibleDeliveryDay ? '' : Number(ctrl.countVisibleDeliveryDay),
                minDeliveryTime: !ctrl.minDeliveryTime ? '' : Number(ctrl.minDeliveryTime),
                showSoonest: ctrl.showSoonest,
                countHiddenDeliveryDay: !ctrl.countHiddenDeliveryDay ? '' : Number(ctrl.countHiddenDeliveryDay),
                orderProcessingDeadline: ctrl.orderProcessingDeadline,
            };
            if (ctrl.deliveryIntervalsStr) {
                ctrl.deliveryIntervalsStr.split('|').forEach(function (dayStr) {
                    var arr = dayStr.split('!');
                    var day = arr[0];
                    var intervals = arr[1] ? arr[1].split('&').filter((x) => !!x && x.length != 0) : [];
                    ctrl.data.deliveryIntervals[day] = intervals || [];
                });
            }
        };
        ctrl.changeIntervals = function (data) {
            ctrl.data = data;
            Object.keys(data).forEach(function (key) {
                ctrl[key] = data[key];
            });
            ctrl.deliveryIntervalsStr = ctrl.getDeliveryIntervalsStr(data.deliveryIntervals);
        };
        ctrl.getDeliveryIntervalsStr = function (deliveryIntervals) {
            var daysWithIntervalsStr = [];
            Object.keys(deliveryIntervals).forEach(function (key) {
                var intervalsStr = deliveryIntervals[key].join('&');
                if (intervalsStr) daysWithIntervalsStr.push(key + '!' + intervalsStr);
            });
            return daysWithIntervalsStr.join('|');
        };
    };
    ShippingWithIntervalCtrl.$inject = ['$http', 'toaster'];
    ng.module('shippingMethod')
        .controller('ShippingWithIntervalCtrl', ShippingWithIntervalCtrl)
        .component('shippingWithIntervalSettings', {
            templateUrl: shippingWithIntervalTemplate,
            controller: 'ShippingWithIntervalCtrl',
            bindings: {
                onInit: '&',
                deliveryIntervalsStr: '=',
                timeZoneId: '=',
                timeZones: '@',
                countVisibleDeliveryDay: '=',
                minDeliveryTime: '=',
                showSoonest: '=',
                countHiddenDeliveryDay: '=',
                orderProcessingDeadline: '=',
            },
        });
})(window.angular);
