import addUpdateBookingTemplate from './../bookingJournal/modal/addUpdateBooking/addUpdateBooking.html';
(function (ng) {
    'use strict';

    var bookingService = function ($http, $uibModal, Upload, $ocLazyLoad, $q) {
        var service = this;
        service.getBooking = function (id) {
            return $http
                .get('booking/get', {
                    params: {
                        id: id,
                    },
                })
                .then(function (response) {
                    if (service.isSafariBrowser()) {
                        return $q.when(moment == null ? import('moment') : true).then(function () {
                            response.data.obj.BeginDate = moment(response.data.obj.BeginDate.replace('T', ' ')).toDate();
                            response.data.obj.EndDate = moment(response.data.obj.EndDate.replace('T', ' ')).toDate();
                            return response.data;
                        });
                    } else {
                        return response.data;
                    }
                });
        };
        service.changeStatus = function (id, status) {
            return $http
                .post('booking/changeStatus', {
                    id: id,
                    status: status,
                })
                .then(function (response) {
                    return response.data;
                });
        };
        service.createOrder = function (id) {
            return $http
                .post('booking/createOrder', {
                    id: id,
                })
                .then(function (response) {
                    return response.data;
                });
        };
        service.updateBookingAfterDrag = function (id, reservationResourceId, beginDate, endDate, userConfirmed) {
            return $http
                .post('booking/updateAfterDrag', {
                    id: id,
                    reservationResourceId: reservationResourceId,
                    beginDate: beginDate,
                    endDate: endDate,
                    userConfirmed: userConfirmed,
                })
                .then(function (response) {
                    return response.data;
                });
        };
        service.delete = function (id) {
            return $http
                .post('booking/delete', {
                    Id: id,
                })
                .then(function (result) {
                    return result.data;
                });
        };
        service.showBookingModal = function (id, affiliateId, beginDate, endDate, reservationResourceId) {
            return $uibModal.open({
                bindToController: true,
                controller: 'ModalAddUpdateBookingCtrl',
                controllerAs: 'ctrl',
                size: 'xs-11',
                backdrop: 'static',
                windowClass: 'modal--panel modal-booking-sheduler',
                openedClass: 'modal-open--panel',
                templateUrl: addUpdateBookingTemplate,
                resolve: {
                    params: {
                        id: id,
                        affiliateId: affiliateId,
                        beginDate: beginDate,
                        endDate: endDate,
                        reservationResourceId: reservationResourceId,
                    },
                },
            });
        };
        service.getAttachments = function (bookingId) {
            return $http
                .get('booking/getAttachments', {
                    params: {
                        bookingId: bookingId,
                    },
                })
                .then(function (response) {
                    return response.data;
                });
        };
        service.uploadAttachment = function (bookingId, $files) {
            return Upload.upload({
                url: 'booking/uploadAttachments',
                data: {
                    bookingId: bookingId,
                },
                file: $files,
            }).then(function (response) {
                return response.data;
            });
        };
        service.deleteAttachment = function (bookingId, id) {
            return $http
                .post('booking/deleteAttachment', {
                    bookingId: bookingId,
                    id: id,
                })
                .then(function (response) {
                    return response.data;
                });
        };
        service.selectableTimeEventStop = function (selected, model) {
            var instenceModel = ng.copy(model);
            var deactivateTime = selected.filter(function (time) {
                if (instenceModel.indexOf(time) === -1) {
                    instenceModel.push(time);
                    return false;
                }
                return true;
            });
            if (deactivateTime.length > 0) {
                model.length = 0;
                instenceModel.forEach(function (time) {
                    if (deactivateTime.indexOf(time) === -1) {
                        model.push(time);
                    }
                });
            } else {
                model = selected.forEach(function (time) {
                    model.push(time);
                });
            }
        };
        service.isSafariBrowser = function () {
            return (
                navigator.vendor &&
                navigator.vendor.indexOf('Apple') > -1 &&
                navigator.userAgent &&
                navigator.userAgent.indexOf('CriOS') == -1 &&
                navigator.userAgent.indexOf('FxiOS') == -1
            );
        };
        service.transformDate = function (date, asString, checkSafariBrowser) {
            //checkSafariBrowser в некоторых местах не требуется проверка SAFARI так как с проверкой работает некорректно
            if (checkSafariBrowser && service.isSafariBrowser()) {
                return moment(date.utc().toDate().toISOString().slice(0, -5).replace('T', ' ')).toDate(); //для старых версий сафари
            }
            var transformedDate = date.utc().toDate().toISOString().slice(0, -5);
            if (asString) {
                return transformedDate;
            }
            return new Date(transformedDate);
        };
    };
    bookingService.$inject = ['$http', '$uibModal', 'Upload', '$ocLazyLoad', '$q'];
    ng.module('booking').service('bookingService', bookingService);
})(window.angular);
