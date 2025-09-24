import modalReservationResourceShedulerTemplate from './../../bookingJournal/modal/reservationResourceSheduler/ModalReservationResourceSheduler.html';
(function (ng) {
    'use strict';

    /* @ngInject */
    var BookingShedulerDaysCtrl = function ($http, $location, $uibModal) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl._params = ctrl.shedulerParams;
            ctrl.optionsFromUrl();
            if (!ctrl._params.dateFrom) {
                let date = new Date();
                ctrl.dateFrom = date.getFullYear() + '-' + ('0' + (date.getMonth() + 1)).slice(-2) + '-' + ('0' + date.getDate()).slice(-2);
                //ctrl.date = date;
                ctrl._params.dateFrom = ctrl.dateFrom;
            } else {
                ctrl.dateFrom = ctrl._params.dateFrom;
            }
            if (!ctrl._params.dateTo) {
                let date = new Date();
                date = new Date(date.setMonth(date.getMonth() + 1));
                ctrl.dateTo = date.getFullYear() + '-' + ('0' + (date.getMonth() + 1)).slice(-2) + '-' + ('0' + date.getDate()).slice(-2);
                //ctrl.date = date;
                ctrl._params.dateTo = ctrl.dateTo;
            } else {
                ctrl.dateTo = ctrl._params.dateTo;
            }
            ctrl._onInit();
        };
        ctrl._onInit = function () {
            ctrl.fetchData().then(function () {
                if (ctrl.shedulerOnInit != null) {
                    ctrl.shedulerOnInit({
                        sheduler: ctrl,
                    });
                }
            });
        };

        //#region Filter

        ctrl.optionsFromUrl = function () {
            var shedulerParamsByUrl = ctrl.getParamsByUrl(ctrl.uid);
            if (shedulerParamsByUrl != null) {
                ng.extend(ctrl._params, shedulerParamsByUrl);
            }
        };
        ctrl.setParamsByUrl = function (params) {
            $location.search(ctrl.uid, JSON.stringify(params));
        };
        ctrl.getParamsByUrl = function (uid) {
            return JSON.parse($location.search()[uid] || null);
        };
        ctrl.getRequestParams = function () {
            var params = {};
            ng.extend(params, ctrl._params);
            return params;
        };
        ctrl.fetchData = function () {
            console.log('fetchData');
            var params = ctrl.getRequestParams();
            ctrl.shedulerProcessing = true;
            return $http
                .post(ctrl.fetchUrl, {
                    model: params,
                })
                .then(function (response) {
                    ctrl.shedulerObj = response.data;
                    ctrl._params = ctrl._params || {};
                    ctrl.shedulerProcessing = false;
                });
        };

        //#endregion

        ctrl.changeDate = function () {
            ctrl._params.dateFrom = ctrl.dateFrom;
            ctrl._params.dateTo = ctrl.dateTo;
            ctrl.fetchData().then(function () {
                ctrl.setParamsByUrl(ctrl._params);
            });
        };
        ctrl.showReservationResourceSheduler = function (affiliateId, reservationResourceId, date, slotHeightPx, name, bookingDuration) {
            if (!affiliateId || !reservationResourceId) {
                return;
            }
            var fnModalShedulerClose = function (result) {
                if (result && result.reservationResourcesChanged && result.reservationResourcesChanged.length) {
                    ctrl.fetchData();
                }
            };
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalReservationResourceShedulerCtrl',
                    controllerAs: 'ctrl',
                    size: 'xs-11',
                    //backdrop: 'static',
                    templateUrl: modalReservationResourceShedulerTemplate,
                    resolve: {
                        params: {
                            affiliateId: affiliateId,
                            reservationResourceId: reservationResourceId,
                            date: date,
                            slotHeightPx: slotHeightPx,
                            name: name,
                            bookingDuration: bookingDuration,
                            mode: 'edit',
                        },
                    },
                })
                .result.then(
                    function (result) {
                        fnModalShedulerClose(result);
                        return result;
                    },
                    function (result) {
                        fnModalShedulerClose(result);
                        return result;
                    },
                );
        };
    };
    ng.module('bookingShedulerDays').controller('BookingShedulerDaysCtrl', BookingShedulerDaysCtrl);
})(window.angular);
