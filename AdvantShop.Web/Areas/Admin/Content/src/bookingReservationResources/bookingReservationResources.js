import addUpdateReservationResourceTemplate from './modal/addUpdateReservationResource/addUpdateReservationResource.html';
(function (ng) {
    'use strict';

    var BookingReservationResourcesCtrl = function ($http, $q, uiGridConstants, uiGridCustomConfig, SweetAlert, toaster, $uibModal, $translate) {
        var ctrl = this;
        ctrl.gridReservationResourcesInited = false;
        var columnDefsResources = [
            {
                name: 'PhotoSrc',
                headerCellClass: 'ui-grid-custom-header-cell-center',
                displayName: $translate.instant('Admin.Js.BookingUsers.Photo'),
                cellTemplate:
                    '<div class="ui-grid-cell-contents"><span class="ui-grid-custom-flex-center ui-grid-custom-link-for-img">' +
                    '<img class="ui-grid-custom-col-img" ng-src="{{COL_FIELD}}"></span></div>',
                width: 80,
                enableSorting: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.BookingUsers.Photo'),
                    type: uiGridConstants.filter.SELECT,
                    name: 'HasPhoto',
                    selectOptions: [
                        {
                            label: $translate.instant('Admin.Js.BookingUsers.WithPhoto'),
                            value: true,
                        },
                        {
                            label: $translate.instant('Admin.Js.BookingUsers.WithoutPhoto'),
                            value: false,
                        },
                    ],
                },
            },
            {
                name: 'Name',
                displayName: $translate.instant('Admin.Js.BookingUsers.FullName'),
                cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>',
            },
            {
                name: 'SortOrder',
                displayName: 'Порядок',
                type: 'number',
                width: 150,
                enableCellEdit: true,
                filter: {
                    placeholder: 'Порядок',
                    type: 'range',
                    rangeOptions: {
                        from: {
                            name: 'SortingFrom',
                        },
                        to: {
                            name: 'SortingTo',
                        },
                    },
                },
            },
            {
                name: 'BindAffiliate',
                displayName: 'Привязка',
                cellTemplate: '<ui-grid-custom-switch row="row" class="js-grid-not-clicked" field-name="BindAffiliate"></ui-grid-custom-switch>',
                // меняется в init
                width: 100,
                filter: {
                    name: 'HasAffiliate',
                    placeholder: 'Привязка к филиалу',
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: [
                        {
                            label: 'Привязанные',
                            value: true,
                        },
                        {
                            label: 'Непривязанные',
                            value: false,
                        },
                    ],
                },
            },
            {
                name: 'Enabled',
                displayName: $translate.instant('Admin.Js.BookingUsers.Active'),
                cellTemplate: '<ui-grid-custom-switch row="row" class="js-grid-not-clicked" field-name="Enabled"></ui-grid-custom-switch>',
                // меняется в init
                width: 100,
                filter: {
                    name: 'Enabled',
                    placeholder: $translate.instant('Admin.Js.BookingUsers.Activity'),
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: [
                        {
                            label: $translate.instant('Admin.Js.BookingUsers.AreActive'),
                            value: true,
                        },
                        {
                            label: $translate.instant('Admin.Js.BookingUsers.NotActive'),
                            value: false,
                        },
                    ],
                },
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 80,
                enableSorting: false,
                useInSwipeBlock: true,
                cellTemplate:
                    '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                    '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadReservationResource(row.entity.Id, row.entity.BindAffiliate ? row.entity.AffiliateId : null)" aria-label="Редактировать"></button> ' +
                    '<button type="button" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteReservationResource(row.entity.CanBeDeleted, row.entity.Id)" ' +
                    'class="btn-icon ui-grid-custom-service-icon fa fa-times link-invert" aria-label="Удалить"></button> ' +
                    //'ng-class="(!row.entity.CanBeDeleted ? \'ui-grid-custom-service-icon fa fa-times link-disabled\' : \'ui-grid-custom-service-icon fa fa-times link-invert\')"></a> ' +
                    '</div></div>' +
                    '<a ng-if="grid.appScope.$ctrl.isMobile" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteReservationResource(row.entity.CanBeDeleted, row.entity.Id)" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</a>',
            },
        ];
        ctrl.init = function (readOnly) {
            ctrl.readOnly = readOnly;
            var gridConfig = {
                columnDefs: columnDefsResources,
                uiGridCustom: {
                    rowClick: function ($event, row) {
                        ctrl.loadReservationResource(row.entity.Id, row.entity.BindAffiliate ? row.entity.AffiliateId : null);
                    },
                    selectionOptions: [],
                },
            };
            if (ctrl.readOnly) {
                gridConfig.columnDefs.forEach(function (item) {
                    item.enableCellEdit = false;
                    if (item.name === '_serviceColumn') {
                        item.visible = false;
                    }
                    if (item.name === 'BindAffiliate') {
                        item.cellTemplate =
                            '<ui-grid-custom-switch row="row" class="js-grid-not-clicked" field-name="BindAffiliate" readonly="true"></ui-grid-custom-switch>';
                    }
                    if (item.name === 'Enabled') {
                        item.cellTemplate =
                            '<ui-grid-custom-switch row="row" class="js-grid-not-clicked" field-name="Enabled" readonly="true"></ui-grid-custom-switch>';
                    }
                });
            } else {
                gridConfig.uiGridCustom.selectionOptions.push({
                    text: $translate.instant('Admin.Js.BookingUsers.DeleteSelected'),
                    url: 'bookingResources/deleteReservationResources',
                    field: 'Id',
                    before: function () {
                        return SweetAlert.confirm($translate.instant('Admin.Js.BookingUsers.AreYouSureDelete'), {
                            title: $translate.instant('Admin.Js.BookingUsers.Deleting'),
                        }).then(function (result) {
                            return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                        });
                    },
                });
            }
            ctrl.gridResourcesOptions = ng.extend({}, uiGridCustomConfig, gridConfig);
            ctrl.isInit = true;
        };
        ctrl.gridResourcesOnInit = function (grid) {
            ctrl.gridReservationResources = grid;
            ctrl.gridReservationResourcesInited = true;
        };
        ctrl.loadReservationResource = function (reservationResourceId, affiliateId) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddUpdateReservationResourceCtrl',
                    controllerAs: 'ctrl',
                    size: 'lg',
                    backdrop: 'static',
                    templateUrl: addUpdateReservationResourceTemplate,
                    resolve: {
                        params: {
                            id: reservationResourceId,
                            affiliateId: affiliateId,
                        },
                    },
                })
                .result.then(
                    function (result) {
                        ctrl.onResourceAddUpdate();
                        return result;
                    },
                    function (result) {
                        ctrl.onResourceAddUpdate();
                        return result;
                    },
                );
        };
        ctrl.deleteReservationResource = function (canBeDeleted, reservationResourceId) {
            if (canBeDeleted) {
                SweetAlert.confirm($translate.instant('Admin.Js.BookingUsers.AreYouSureDelete'), {
                    title: $translate.instant('Admin.Js.BookingUsers.Deleting'),
                }).then(function (result) {
                    if (result === true || result.value === true) {
                        $http
                            .post('bookingResources/delete', {
                                reservationResourceId: reservationResourceId,
                            })
                            .then(function (result) {
                                var data = result.data;
                                if (data.result === false) {
                                    data.errors.forEach(function (error) {
                                        toaster.pop('error', error);
                                    });
                                }
                                ctrl.gridReservationResources.fetchData();
                            });
                    }
                });
            }
        };
        ctrl.onResourceAddUpdate = function () {
            ctrl.gridReservationResources.fetchData();
        };
    };
    BookingReservationResourcesCtrl.$inject = [
        '$http',
        '$q',
        'uiGridConstants',
        'uiGridCustomConfig',
        'SweetAlert',
        'toaster',
        '$uibModal',
        '$translate',
    ];
    ng.module('bookingReservationResources', [
        'uiGridCustom',
        'listOfReservationResourceServices',
        'bookingServicesSelectvizr',
        'listReservationResourceAdditionalTime',
    ]).controller('BookingReservationResourcesCtrl', BookingReservationResourcesCtrl);
})(window.angular);
