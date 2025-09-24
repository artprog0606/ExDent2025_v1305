import addEditTaxTemplate from './modal/addEditTax/AddEditTax.html';
(function (ng) {
    'use strict';

    var SettingsCheckoutCtrl = function (
        $uibModal,
        $http,
        $q,
        uiGridConstants,
        uiGridCustomConfig,
        SweetAlert,
        toaster,
        $translate,
        $location,
        isMobileService,
    ) {
        var ctrl = this;

        // #region Taxes
        var columnDefsTaxes = [
            {
                name: 'Name',
                displayName: $translate.instant('Admin.Js.SettingsCheckout.Name'),
                cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>',
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsCheckout.Name'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Name',
                },
            },
            {
                name: 'IsDefault',
                displayName: $translate.instant('Admin.Js.SettingsCheckout.DefaultTax'),
                enableCellEdit: false,
                enableSorting: false,
                cellTemplate: '<ui-grid-custom-switch row="row" field-name="IsDefault" class="js-grid-not-clicked"></ui-grid-custom-switch>',
                width: 100,
            },
            {
                name: 'Enabled',
                displayName: $translate.instant('Admin.Js.SettingsCheckout.Actively'),
                enableCellEdit: false,
                cellTemplate:
                    '<ui-grid-custom-switch row="row" field-name="Enabled" class= "js-grid-not-clicked" on-click="grid.appScope.$ctrl.gridExtendCtrl.changeEnabled(row.entity)" readonly="row.entity.Enabled && !row.entity.CanBeDeleted"></ui-grid-custom-switch>',
                width: 100,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsCheckout.Activity'),
                    name: 'Enabled',
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: [
                        {
                            label: $translate.instant('Admin.Js.SettingsCheckout.AreActive'),
                            value: true,
                        },
                        {
                            label: $translate.instant('Admin.Js.SettingsCheckout.Inactive'),
                            value: false,
                        },
                    ],
                },
            },
            {
                name: 'TaxTypeFormatted',
                displayName: $translate.instant('Admin.Js.SettingsCheckout.Type'),
                enableCellEdit: false,
                width: 200,
            },
            {
                name: 'Rate',
                displayName: $translate.instant('Admin.Js.SettingsCheckout.Rate'),
                width: 100,
            },
            {
                name: '_serviceColumn',
                displayName: '',
                enableSorting: false,
                width: 80,
                useInSwipeBlock: true,
                cellTemplate:
                    '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                    '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadTax(row.entity.TaxId)" aria-label="Редактировать"></button> ' +
                    '<button type="button" class="btn-icon" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteTax(row.entity)" ng-class="(!row.entity.CanBeDeleted ? \'ui-grid-custom-service-icon fa fa-times link-disabled\' : \'ui-grid-custom-service-icon fa fa-times link-invert\')" aria-label="Удалить"></button> ' +
                    '</div></div>' +
                    '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="settingsCheckout/deletetax" params="{\'id\': row.entity.TaxId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
            },
        ];
        ctrl.gridTaxesOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsTaxes,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadTax(row.entity.TaxId);
                },
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.SettingsCheckout.DeleteSelected'),
                        url: 'settingsCheckout/deleteItems',
                        field: 'TaxId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.SettingsCheckout.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.SettingsCheckout.Deleting'),
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
        });
        ctrl.gridTaxesOnInit = function (grid) {
            ctrl.gridTaxes = grid;
        };
        ctrl.gridTaxesUpdate = function () {
            ctrl.gridTaxes.fetchData();
        };
        ctrl.loadTax = function (id) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddEditTaxCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: addEditTaxTemplate,
                    resolve: {
                        id: function () {
                            return id;
                        },
                    },
                })
                .result.then(
                    function (result) {
                        ctrl.gridTaxes.fetchData();
                        return result;
                    },
                    function (result) {
                        return result;
                    },
                );
        };
        ctrl.deleteTax = function (obj) {
            if (obj.CanBeDeleted) {
                SweetAlert.confirm($translate.instant('Admin.Js.SettingsCheckout.AreYouSureDelete'), {
                    title: $translate.instant('Admin.Js.SettingsCheckout.Deleting'),
                }).then(function (result) {
                    if (result === true || result.value === true) {
                        $http
                            .post('settingsCheckout/deletetax', {
                                id: obj.TaxId,
                            })
                            .then(function (response) {
                                ctrl.gridTaxes.fetchData();
                            });
                    }
                });
            } else {
                var errors = [];
                if (obj.IsDefault) errors.push($translate.instant('Admin.Js.SettingsCheckout.IsDefault'));
                if (obj.ProductsCount > 0) errors.push($translate.instant('Admin.Js.SettingsCheckout.HasProducts'));
                if (obj.UsedInCertificates) errors.push($translate.instant('Admin.Js.SettingsCheckout.UsedInCertificates'));
                if (obj.UsedInPaymentMethods) errors.push($translate.instant('Admin.Js.SettingsCheckout.UsedInPaymentMethods'));
                if (obj.UsedInShippingMethods) errors.push($translate.instant('Admin.Js.SettingsCheckout.UsedInShippingMethods'));
                SweetAlert.alert(errors.join('\n\r'), {
                    title: $translate.instant('Admin.Js.SettingsCheckout.DeletingIsImpossible'),
                    customClass: 'pre-line',
                });
            }
        };

        // #endregion

        ctrl.changeOrderId = function (orderId) {
            $http
                .post('settingsCheckout/changeOrderId', {
                    orderId: orderId,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCheckout.OrderNumberChanged'));
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', '', error);
                        });
                    }
                });
        };
        ctrl.changeEnabled = function (obj) {
            if (obj.Enabled && !obj.CanBeDeleted) {
                var errors = [];
                if (obj.IsDefault) errors.push($translate.instant('Admin.Js.SettingsCheckout.IsDefault'));
                if (obj.ProductsCount > 0) errors.push($translate.instant('Admin.Js.SettingsCheckout.HasProducts'));
                if (obj.UsedInCertificates) errors.push($translate.instant('Admin.Js.SettingsCheckout.UsedInCertificates'));
                if (obj.UsedInPaymentMethods) errors.push($translate.instant('Admin.Js.SettingsCheckout.UsedInPaymentMethods'));
                if (obj.UsedInShippingMethods) errors.push($translate.instant('Admin.Js.SettingsCheckout.UsedInShippingMethods'));
                toaster.pop('error', $translate.instant('Admin.Js.SettingsCheckout.ChangeEnabledIsImpossible'), errors.join('<br>'));
            }
        };
        ctrl.onSelectTab = function (indexTab) {
            ctrl.tabActiveIndex = indexTab;
        };
    };
    SettingsCheckoutCtrl.$inject = [
        '$uibModal',
        '$http',
        '$q',
        'uiGridConstants',
        'uiGridCustomConfig',
        'SweetAlert',
        'toaster',
        '$translate',
        '$location',
        'isMobileService',
    ];
    ng.module('settingsCheckout', [
        'thankYouPageProducts',
        'orderstatuses',
        'ordersources',
        'color.picker',
        'uiAceTextarea',
        'uiModal',
        'productsSelectvizr',
        'analytics',
        'analyticsFilter',
        'isMobile',
    ]).controller('SettingsCheckoutCtrl', SettingsCheckoutCtrl);
})(window.angular);
