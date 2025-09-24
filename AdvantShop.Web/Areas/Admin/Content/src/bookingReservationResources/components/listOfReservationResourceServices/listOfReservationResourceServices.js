import listOfReservationResourceServicesTemplate from './listOfReservationResourceServices.html';
import addUpdateBookingServiceTemplate from './../../../bookingServices/modals/addUpdateBookingService/addUpdateBookingService.html';
(function (ng) {
    'use strict';

    var ListOfReservationResourceServicesCtrl = function (
        $uibModal,
        $q,
        uiGridConstants,
        uiGridCustomConfig,
        $http,
        toaster,
        SweetAlert,
        $translate,
    ) {
        var ctrl = this;
        ctrl.gridServicesInited = false;
        var columnDefsServices = [
            {
                name: 'PhotoSrc',
                headerCellCalss: 'ui-grid-custom-header-cell-center',
                displayName: 'Фото',
                cellTemplate:
                    '<div class="ui-grid-cell-contents"><span class="ui-grid-custom-flex-center ui-grid-custom-link-for-img">' +
                    '<img class="ui-grid-custom-col-img" ng-src="{{COL_FIELD}}"></span></div>',
                width: 80,
                enableSorting: false,
                filter: {
                    placeholder: 'Фото',
                    type: uiGridConstants.filter.SELECT,
                    name: 'HasPhoto',
                    selectOptions: [
                        {
                            label: 'С фото',
                            value: true,
                        },
                        {
                            label: 'Без фото',
                            value: false,
                        },
                    ],
                },
            },
            {
                name: 'Name',
                displayName: 'Название',
                cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>',
                //filter: {
                //    placeholder: 'Название',
                //    type: uiGridConstants.filter.INPUT,
                //    name: 'Name'
                //}
            },
            {
                name: 'Price',
                displayName: 'Цена',
                cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{row.entity.PriceString}}</span></div>',
                width: 100,
            },
            {
                visible: false,
                name: 'Enabled',
                displayName: 'Активна',
                cellTemplate: '<ui-grid-custom-switch row="row" class="js-grid-not-clicked" field-name="Enabled"></ui-grid-custom-switch>',
                width: 90,
                filter: {
                    name: 'Enabled',
                    placeholder: 'Активность',
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: [
                        {
                            label: 'Активные',
                            value: true,
                        },
                        {
                            label: 'Неактивные',
                            value: false,
                        },
                    ],
                },
            },
            {
                name: '_serviceColumn',
                displayName: '',
                enableSorting: false,
                useInSwipeBlock: true,
                width: 80,
                cellTemplate:
                    '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                    '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadService(row.entity.Id)" aria-label="Редактировать"></button> ' +
                    '<button type="button" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteService(row.entity.Id)" ' +
                    'class="btn-icon ui-grid-custom-service-icon fa fa-times link-invert" aria-label="Удалить"></button> ' +
                    '</div></div>' +
                    '<a ng-if="grid.appScope.$ctrl.isMobile" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteService(row.entity.Id)" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</a>',
            },
        ];
        ctrl.gridServicesOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsServices,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadService(row.entity.Id);
                },
                selectionOptions: [
                    {
                        text: 'Удалить выделенные',
                        url: 'bookingResources/deleteServices',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm('Вы уверены, что хотите удалить?', {
                                title: 'Удаление',
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
        });
        ctrl.$onInit = function () {
            if (ctrl.readonly) {
                ctrl.gridServicesOptions.columnDefs.forEach(function (item) {
                    item.enableCellEdit = false;
                    if (item.name === '_serviceColumn') {
                        item.visible = false;
                    }
                });
                ctrl.gridServicesOptions.uiGridCustom.selectionOptions = null;
            }
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    grid: ctrl.gridServices,
                });
            }
            if (ctrl.onInitComponent != null) {
                ctrl.onInitComponent({
                    item: ctrl,
                });
            }
        };
        ctrl.loadService = function (id) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddUpdateBookingServiceCtrl',
                    controllerAs: 'ctrl',
                    size: 'lg',
                    backdrop: 'static',
                    templateUrl: addUpdateBookingServiceTemplate,
                    resolve: {
                        params: {
                            id: id,
                            canBeEditing: !ctrl.readonly,
                        },
                    },
                })
                .result.then(
                    function (result) {
                        ctrl.fetch();
                        return result;
                    },
                    function (result) {
                        ctrl.fetch();
                        return result;
                    },
                );
        };
        ctrl.gridServicesOnInit = function (grid) {
            ctrl.gridServices = grid;
            ctrl.gridServicesInited = true;
        };
        ctrl.fetch = function () {
            ctrl.gridServices.fetchData();
        };
        ctrl.addServicesModal = function (result) {
            if (result == null || result.servicesIds == null || result.servicesIds.length === 0) return;
            $http
                .post('bookingResources/addServices', {
                    affiliateId: ctrl.affiliateId,
                    reservationResourceId: ctrl.reservationResourceId,
                    serviceIds: result.servicesIds,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.BookingUsers.ChangesSaved'));
                        ctrl.fetch();
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                        if (!data.errors) {
                            toaster.pop('error', $translate.instant('Admin.Js.BookingUsers.FailedToSaveServices'));
                        }
                    }
                });
        };
        ctrl.deleteService = function (servicesId) {
            SweetAlert.confirm($translate.instant('Admin.Js.BookingUsers.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.BookingUsers.Deleting'),
            }).then(function (result) {
                if (result === true || result.value === true) {
                    $http
                        .post('bookingResources/deleteService', {
                            affiliateId: ctrl.affiliateId,
                            reservationResourceId: ctrl.reservationResourceId,
                            serviceId: servicesId,
                        })
                        .then(function (response) {
                            var data = response.data;
                            if (data.result === true) {
                                toaster.pop('success', '', $translate.instant('Admin.Js.BookingUsers.ChangesSaved'));
                                ctrl.fetch();
                            } else {
                                data.errors.forEach(function (error) {
                                    toaster.pop('error', error);
                                });
                                if (!data.errors) {
                                    toaster.pop('error', $translate.instant('Admin.Js.BookingUsers.FailedToDeletingServices'));
                                }
                            }
                        });
                }
            });
        };
    };
    ListOfReservationResourceServicesCtrl.$inject = [
        '$uibModal',
        '$q',
        'uiGridConstants',
        'uiGridCustomConfig',
        '$http',
        'toaster',
        'SweetAlert',
        '$translate',
    ];
    ng.module('listOfReservationResourceServices', [])
        .controller('ListOfReservationResourceServicesCtrl', ListOfReservationResourceServicesCtrl)
        .component('listOfReservationResourceServices', {
            templateUrl: listOfReservationResourceServicesTemplate,
            controller: 'ListOfReservationResourceServicesCtrl',
            transclude: true,
            bindings: {
                onInit: '&',
                affiliateId: '<',
                reservationResourceId: '<',
                readonly: '<?',
                onInitComponent: '&',
            },
        });
})(window.angular);
