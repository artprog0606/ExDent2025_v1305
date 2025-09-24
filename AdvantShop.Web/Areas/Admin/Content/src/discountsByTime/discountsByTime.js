import addEditDiscountsByTimeTemplate from './modal/discountByDatetime/discountByDatetime.html';
(function (ng) {
    'use strict';

    var DiscountsByTimeCtrl = function (uiGridConstants, uiGridCustomConfig, $q, SweetAlert, $http, toaster, $translate) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Discount',
                    displayName: $translate.instant('Admin.Js.DiscountByTime.Discount'),
                    enableCellEdit: true,
                    type: 'number',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.DiscountByTime.Discount'),
                        type: 'range',
                        rangeOptions: {
                            from: {
                                name: 'DiscountFrom',
                            },
                            to: {
                                name: 'DiscountTo',
                            },
                        },
                        name: 'Discount',
                    },
                },
                {
                    name: 'Time',
                    displayName: $translate.instant('Admin.Js.DiscountByTime.Time'),
                    enableCellEdit: false,
                },
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.DiscountByTime.Enabled'),
                    cellTemplate: '<ui-grid-custom-switch row="row"></ui-grid-custom-switch>',
                    width: 100,
                    filter: {
                        name: 'Enabled',
                        placeholder: $translate.instant('Admin.Js.DiscountByTime.Enabled'),
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            { label: $translate.instant('Admin.Js.DiscountByTime.Enable'), value: true },
                            { label: $translate.instant('Admin.Js.DiscountByTime.Disable'), value: false },
                        ],
                    },
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.DiscountByTime.SortOrder'),
                    enableCellEdit: true,
                    width: 100,
                    type: 'number',
                },
                {
                    name: '_noopColumnDiscountTime',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.DiscountByTime.DiscountTime'),
                        type: 'time',
                        term: {
                            from: new Date('01.01.2024 09:00'),
                            to: new Date('01.01.2024 18:00'),
                        },
                        timeOptions: {
                            from: { name: 'TimeFrom' },
                            to: { name: 'TimeTo' },
                        },
                    },
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 80,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<ui-modal-trigger data-controller="\'ModalDiscountByDatetimeCtrl\'" controller-as="ctrl" size="middle" ' +
                        'template-url="' +
                        addEditDiscountsByTimeTemplate +
                        '" ' +
                        'data-resolve="{\'id\': row.entity.Id}" ' +
                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                        '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></button> ' +
                        '</ui-modal-trigger>' +
                        '<ui-grid-custom-delete url="discountsByTime/delete" params="{\'id\': row.entity.Id}"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="discountsByTime/delete" params="{\'id\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.PriceRange.DeleteSelected'),
                        url: 'discountsByTime/deleteItems',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.PriceRange.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.PriceRange.Deleting'),
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };
    };

    DiscountsByTimeCtrl.$inject = ['uiGridConstants', 'uiGridCustomConfig', '$q', 'SweetAlert', '$http', 'toaster', '$translate'];

    ng.module('discountsByTime', ['uiGridCustom', 'urlHelper']).controller('DiscountsByTimeCtrl', DiscountsByTimeCtrl);
})(window.angular);
