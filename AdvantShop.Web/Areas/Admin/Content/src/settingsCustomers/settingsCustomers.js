import addEditCustomerFieldValueTemplate from './modal/addEditCustomerFieldValue/AddEditCustomerFieldValue.html';
import addEditCustomerFieldTemplate from './modal/addEditCustomerField/AddEditCustomerField.html';
(function (ng) {
    'use strict';

    var SettingsCustomersCtrl = function ($uibModal, $q, $location, uiGridConstants, uiGridCustomConfig, SweetAlert, $translate, isMobileService) {
        var ctrl = this;
        ctrl.init = function (isVisibleCustomerType) {
            var columnDefsCustomerFields = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.SettingsCustomers.Name'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsCustomers.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },
                {
                    name: 'FieldTypeFormatted',
                    displayName: $translate.instant('Admin.Js.SettingsCustomers.Type'),
                    width: 150,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsCustomers.Type'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'FieldType',
                        fetch: 'customerFields/getCustomerFieldTypes',
                    },
                },
                {
                    name: 'HasValues',
                    displayName: $translate.instant('Admin.Js.SettingsCustomers.FieldSettings'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><span class="link" ng-if="row.entity.HasValues" ng-click="grid.appScope.$ctrl.gridExtendCtrl.editValues(row.entity)">' +
                        $translate.instant('Admin.Js.SettingsCustomers.ListOfValues') +
                        '</span></div>',
                    width: 140,
                    enableSorting: false,
                },
            ];
            if (isVisibleCustomerType) {
                columnDefsCustomerFields.push({
                    name: 'CustomerTypeFormatted',
                    displayName: $translate.instant('Admin.Js.SettingsCustomers.CustomerType'),
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsCustomers.CustomerType'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'CustomerType',
                        fetch: 'customerFields/getCustomerFieldCustomerTypes',
                    },
                });
            }
            columnDefsCustomerFields = columnDefsCustomerFields.concat([
                {
                    name: 'Required',
                    displayName: $translate.instant('Admin.Js.SettingsCustomers.Obligatory'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row" field-name="Required" class="js-grid-not-clicked"></ui-grid-custom-switch>',
                    width: 100,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsCustomers.Obligatory'),
                        name: 'Required',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.SettingsCustomers.Yes'),
                                value: true,
                            },
                            {
                                label: $translate.instant('Admin.Js.SettingsCustomers.No'),
                                value: false,
                            },
                        ],
                    },
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.SettingsCustomers.SortingOrder'),
                    type: 'number',
                    width: 100,
                    enableCellEdit: true,
                },
                {
                    name: 'ShowInRegistration',
                    displayName: $translate.instant('Admin.Js.SettingsCustomers.RequestFromBuyerInRegistration'),
                    enableCellEdit: false,
                    cellTemplate:
                        '<ui-grid-custom-switch row="row" field-name="ShowInRegistration" class="js-grid-not-clicked"></ui-grid-custom-switch>',
                    width: 130,
                    visible: 1440,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsCustomers.RequestFromBuyerInRegistration'),
                        name: 'ShowInRegistration',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.SettingsCustomers.Yes'),
                                value: true,
                            },
                            {
                                label: $translate.instant('Admin.Js.SettingsCustomers.No'),
                                value: false,
                            },
                        ],
                    },
                },
                {
                    name: 'ShowInCheckout',
                    displayName: $translate.instant('Admin.Js.SettingsCustomers.RequestFromBuyerInCheckout'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row" field-name="ShowInCheckout" class="js-grid-not-clicked"></ui-grid-custom-switch>',
                    width: 140,
                    visible: 1440,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsCustomers.RequestFromBuyerInCheckout'),
                        name: 'ShowInCheckout',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.SettingsCustomers.Yes'),
                                value: true,
                            },
                            {
                                label: $translate.instant('Admin.Js.SettingsCustomers.No'),
                                value: false,
                            },
                        ],
                    },
                },
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.SettingsCustomers.Actively'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row" field-name="Enabled" class="js-grid-not-clicked"></ui-grid-custom-switch>',
                    width: 75,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsCustomers.Activity'),
                        name: 'Enabled',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.SettingsCustomers.Active'),
                                value: true,
                            },
                            {
                                label: $translate.instant('Admin.Js.SettingsCustomers.Inactive'),
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
                        '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadCustomerField(row.entity.Id)" aria-label="Редактировать"></button> ' +
                        '<ui-grid-custom-delete url="customerFields/delete" params="{\'Id\': row.entity.Id}"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="customerFields/delete" params="{\'Id\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ]);
            ctrl.gridCustomerFieldsOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: columnDefsCustomerFields,
                uiGridCustom: {
                    rowClick: function ($event, row) {
                        ctrl.loadCustomerField(row.entity.Id);
                    },
                    selectionOptions: [
                        {
                            text: $translate.instant('Admin.Js.SettingsCustomers.DeleteSelected'),
                            url: 'customerFields/deleteItems',
                            field: 'Id',
                            before: function () {
                                return SweetAlert.confirm($translate.instant('Admin.Js.SettingsCustomers.AreYouSureDelete'), {
                                    title: $translate.instant('Admin.Js.SettingsCustomers.Deleting'),
                                }).then(function (result) {
                                    return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                                });
                            },
                        },
                    ],
                },
            });
        };
        ctrl.editValues = function (field) {
            ctrl.field = field;
            // при перезагрузке страницы из редактирования значений в урле остается gridCustomerFieldValues
            $location.search('gridCustomerFieldValues', null);
        };
        ctrl.back = function () {
            ctrl.field = null;
            ctrl.gridCustomerFieldValues.clearParams();
        };

        // #region CustomerFields

        ctrl.gridCustomerFieldsOnInit = function (grid) {
            ctrl.gridCustomerFields = grid;
        };
        ctrl.loadCustomerField = function (id) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddEditCustomerFieldCtrl',
                    controllerAs: 'ctrl',
                    size: 'middle',
                    templateUrl: addEditCustomerFieldTemplate,
                    resolve: {
                        id: function () {
                            return id;
                        },
                    },
                })
                .result.then(
                    function (result) {
                        ctrl.gridCustomerFields.fetchData();
                        return result;
                    },
                    function (result) {
                        return result;
                    },
                );
        };
        // #endregion

        // #region CustomerFieldValues
        var columnDefsCustomerFieldValues = [
            {
                name: 'Value',
                displayName: $translate.instant('Admin.Js.SettingsCustomers.Value'),
                cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>',
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsCustomers.Value'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Value',
                },
            },
            {
                name: 'SortOrder',
                displayName: $translate.instant('Admin.Js.SettingsCustomers.SortingOrder'),
                type: 'number',
                width: 100,
                enableCellEdit: true,
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 80,
                enableSorting: false,
                cellTemplate:
                    '<div class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                    '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadCustomerFieldValue(row.entity.Id)" aria-label="Редактировать"></button> ' +
                    '<ui-grid-custom-delete url="customerFieldValues/delete" params="{\'Id\': row.entity.Id}"></ui-grid-custom-delete>' +
                    '</div></div>',
            },
        ];
        ctrl.gridCustomerFieldValuesOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsCustomerFieldValues,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadCustomerFieldValue(row.entity.Id);
                },
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.SettingsCustomers.DeleteSelected'),
                        url: 'customerFieldValues/deleteItems',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.SettingsCustomers.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.SettingsCustomers.Deleting'),
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
        });
        ctrl.gridCustomerFieldValuesOnInit = function (grid) {
            ctrl.gridCustomerFieldValues = grid;
        };
        ctrl.loadCustomerFieldValue = function (id) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddEditCustomerFieldValueCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: addEditCustomerFieldValueTemplate,
                    resolve: {
                        id: function () {
                            return id;
                        },
                    },
                })
                .result.then(
                    function (result) {
                        ctrl.gridCustomerFieldValues.fetchData();
                        return result;
                    },
                    function (result) {
                        return result;
                    },
                );
        };
        // #endregion

        ctrl.onSelectTab = function (indexTab) {
            ctrl.tabActiveIndex = indexTab;
        };
        ctrl.getExportCustomersCtrl = function (exportCustomers) {
            ctrl.exportCustomersCtrl = exportCustomers;
        };
        ctrl.clickUploadBtn = function () {
            let btn = document.querySelector('.subscription__import-btn--callback-click');
            if (btn) {
                btn.click();
            }
        };

        ctrl.showSaveButton = function () {
            return ctrl.tabActiveIndex === 'typesofcustomers';
        };
    };
    SettingsCustomersCtrl.$inject = [
        '$uibModal',
        '$q',
        '$location',
        'uiGridConstants',
        'uiGridCustomConfig',
        'SweetAlert',
        '$translate',
        'isMobileService',
    ];
    ng.module('settingsCustomers', [
        'as.sortable',
        'vkAuth',
        'customers',
        'customergroups',
        'customerSegments',
        'subscription',
        'ngFileUpload',
        'fileUploader',
        'import',
        'customerTags',
        'isMobile',
    ]).controller('SettingsCustomersCtrl', SettingsCustomersCtrl);
})(window.angular);
