import addEditCouponTemplate from './modal/addEditCoupon/addEditCoupon.html';
(function (ng) {
    'use strict';

    var CouponsCtrl = function (uiGridConstants, uiGridCustomConfig, $q, SweetAlert, $http, toaster, $translate) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Code',
                    displayName: $translate.instant('Admin.Js.Coupons.CouponCode'),
                    enableCellEdit: true,
                    cellClass: 'word-break',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Coupons.CouponCode'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Code',
                    },
                },
                {
                    name: 'TypeFormatted',
                    displayName: $translate.instant('Admin.Js.Coupons.TypeOfCoupon'),
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Coupons.TypeOfCoupon'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'Type',
                        fetch: 'coupons/getTypes',
                    },
                },
                {
                    name: 'Value',
                    displayName: $translate.instant('Admin.Js.Coupons.Value'),
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Coupons.Value'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Value',
                    },
                    width: 100,
                },
                {
                    name: 'StartDateFormatted',
                    displayName: $translate.instant('Admin.Js.Coupons.StartDate'),
                    width: 160,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Coupons.StartDate'),
                        type: 'datetime',
                        term: {
                            from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                            to: new Date(),
                        },
                        datetimeOptions: {
                            from: {
                                name: 'StartDateFrom',
                            },
                            to: {
                                name: 'StartDateTo',
                            },
                        },
                    },
                },
                {
                    name: 'ExpirationDateFormatted',
                    displayName: $translate.instant('Admin.Js.Coupons.DateOfEnding'),
                    width: 150,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Coupons.DateOfEnding'),
                        type: 'datetime',
                        term: {
                            from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                            to: new Date(),
                        },
                        datetimeOptions: {
                            from: {
                                name: 'ExpirationDateFrom',
                            },
                            to: {
                                name: 'ExpirationDateTo',
                            },
                        },
                    },
                },
                {
                    name: 'ActualUses',
                    displayName: $translate.instant('Admin.Js.Coupons.Used'),
                    cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.ActualUses}} / {{row.entity.PossibleUses || "-"}}</div>',
                },
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.Coupons.Activity'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row"></ui-grid-custom-switch>',
                    width: 76,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Coupons.Activity'),
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            { label: $translate.instant('Admin.Js.Coupons.TheyActive'), value: true },
                            { label: $translate.instant('Admin.Js.Coupons.Inactive'), value: false },
                        ],
                    },
                },
                {
                    name: 'MinimalOrderPrice',
                    displayName: $translate.instant('Admin.Js.Coupons.MinOrderSum'),
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Coupons.MinOrderSum'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'MinimalOrderPrice',
                    },
                },
                {
                    name: 'AddingDateFormatted',
                    displayName: $translate.instant('Admin.Js.Coupons.AddingDate'),
                    width: 150,
                    visible: 1601,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Coupons.AddingDate'),
                        type: 'datetime',
                        term: {
                            from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                            to: new Date(),
                        },
                        datetimeOptions: {
                            from: {
                                name: 'AddingDateFrom',
                            },
                            to: {
                                name: 'AddingDateTo',
                            },
                        },
                    },
                },
                {
                    name: '_noopColumnForFirstOrder',
                    visible: false,
                    filter: {
                        name: 'ForFirstOrder',
                        placeholder: $translate.instant('Admin.Js.Coupons.ForFirstOrder'),
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            { label: $translate.instant('Admin.Js.Yes'), value: true },
                            { label: $translate.instant('Admin.Js.No'), value: false },
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
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<ui-modal-trigger data-controller="\'ModalAddEditCouponCtrl\'" controller-as="ctrl" size="middle" ' +
                        'template-url="' +
                        addEditCouponTemplate +
                        '" ' +
                        'data-resolve="{\'CouponId\': row.entity.CouponId}" ' +
                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                        '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></button> ' +
                        '</ui-modal-trigger>' +
                        '<ui-grid-custom-delete url="coupons/deleteCoupon" params="{\'CouponId\': row.entity.CouponId}"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="coupons/deleteCoupon" params="{\'CouponId\': row.entity.CouponId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Coupons.DeleteSelected'),
                        url: 'coupons/deleteCoupons',
                        field: 'CouponId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Coupons.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.Coupons.Deleting'),
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

        ctrl.changeDiscountsState = function () {
            $http.post('discountsPriceRange/enableDiscounts', { state: ctrl.enableDiscounts }).then(function (response) {
                toaster.pop('success', '', $translate.instant('Admin.Js.Coupons.ChangesSuccessfullySaved'));
            });
        };
    };

    CouponsCtrl.$inject = ['uiGridConstants', 'uiGridCustomConfig', '$q', 'SweetAlert', '$http', 'toaster', '$translate'];

    ng.module('coupons', ['uiGridCustom', 'urlHelper', 'productsSelectvizr']).controller('CouponsCtrl', CouponsCtrl);
})(window.angular);
