import addEditTemplate from './modal/addEdit/addEdit.html';
(function (ng) {
    'use strict';

    var ShippingReplaceGeoCtrl = function ($http, $q, uiGridConstants, uiGridCustomConfig, SweetAlert, $uibModal, $translate) {
        var ctrl = this;
        ctrl.$onInit = function () {};
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'ShippingType',
                    displayName: 'Тип доставки',
                    enableCellEdit: false,
                    filter: {
                        placeholder: 'Тип доставки',
                        type: uiGridConstants.filter.SELECT,
                        name: 'ShippingType',
                        fetch: 'shippingMethods/getTypesList',
                    },
                },
                {
                    name: 'InCountryName',
                    displayName: 'Страна',
                    enableCellEdit: false,
                    filter: {
                        placeholder: 'Страна (In)',
                        type: uiGridConstants.filter.INPUT,
                        name: 'InCountryName',
                    },
                },
                {
                    name: 'InCountryISO2',
                    displayName: 'ISO2',
                    enableCellEdit: false,
                    filter: {
                        placeholder: 'ISO2 (In)',
                        type: uiGridConstants.filter.INPUT,
                        name: 'InCountryISO2',
                    },
                },
                {
                    name: 'InRegionName',
                    displayName: 'Регион',
                    enableCellEdit: false,
                    filter: {
                        placeholder: 'Регион (In)',
                        type: uiGridConstants.filter.INPUT,
                        name: 'InRegionName',
                    },
                },
                {
                    name: 'InDistrict',
                    displayName: 'Район',
                    enableCellEdit: false,
                    filter: {
                        placeholder: 'Район (In)',
                        type: uiGridConstants.filter.INPUT,
                        name: 'InDistrict',
                    },
                },
                {
                    name: 'InCityName',
                    displayName: 'Город',
                    enableCellEdit: false,
                    filter: {
                        placeholder: 'Город (In)',
                        type: uiGridConstants.filter.INPUT,
                        name: 'InCityName',
                    },
                },
                {
                    name: 'InZip',
                    displayName: 'Индекс',
                    enableCellEdit: false,
                    filter: {
                        placeholder: 'Индекс (In)',
                        type: uiGridConstants.filter.INPUT,
                        name: 'InZip',
                    },
                },
                {
                    name: '_splitColumn',
                    displayName: '=>',
                    cellTemplate: '<div class="ui-grid-cell-contents">=&gt;</div>',
                    enableCellEdit: false,
                    enableSorting: false,
                    width: 30,
                },
                {
                    name: 'OutCountryName',
                    displayName: 'Страна',
                    enableCellEdit: false,
                    filter: {
                        placeholder: 'Страна (Out)',
                        type: uiGridConstants.filter.INPUT,
                        name: 'OutCountryName',
                    },
                },
                {
                    name: 'OutRegionName',
                    displayName: 'Регион',
                    enableCellEdit: false,
                    filter: {
                        placeholder: 'Регион (Out)',
                        type: uiGridConstants.filter.INPUT,
                        name: 'OutRegionName',
                    },
                },
                {
                    name: 'OutDistrict',
                    displayName: 'Район',
                    enableCellEdit: false,
                    filter: {
                        placeholder: 'Район (Out)',
                        type: uiGridConstants.filter.INPUT,
                        name: 'OutDistrict',
                    },
                },
                {
                    name: 'OutDistrictClear',
                    displayName: 'Очищать район',
                    cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.OutDistrictClear ? "Да" : "Нет"}}</div>',
                    enableCellEdit: false,
                    filter: {
                        placeholder: 'Очищать район (Out)',
                        name: 'OutDistrictClear',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: 'Да',
                                value: true,
                            },
                            {
                                label: 'Нет',
                                value: false,
                            },
                        ],
                    },
                },
                {
                    name: 'OutCityName',
                    displayName: 'Город',
                    enableCellEdit: false,
                    filter: {
                        placeholder: 'Город (Out)',
                        type: uiGridConstants.filter.INPUT,
                        name: 'OutCityName',
                    },
                },
                {
                    name: 'OutZip',
                    displayName: 'Индекс',
                    enableCellEdit: false,
                    filter: {
                        placeholder: 'Индекс (Out)',
                        type: uiGridConstants.filter.INPUT,
                        name: 'OutZip',
                    },
                },
                {
                    name: 'Enabled',
                    displayName: 'Активно',
                    cellTemplate:
                        '<ui-grid-custom-switch row="row" class="js-grid-not-clicked" field-name="Enabled" readonly="true"></ui-grid-custom-switch>',
                    width: 100,
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
                    name: 'Sort',
                    displayName: 'Порядок',
                    type: 'number',
                    width: 150,
                    enableCellEdit: false,
                },
                {
                    name: 'Comment',
                    displayName: 'Комментарий',
                    enableCellEdit: false,
                    enableSorting: false,
                    //filter: {
                    //    placeholder: 'Комментарий',
                    //    type: uiGridConstants.filter.INPUT,
                    //    name: 'Comment'
                    //}
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    enableSorting: false,
                    width: 80,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                        '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadReplace(row.entity.Id)" ng-if="row.entity.Id >= 5000" aria-label="Редактировать"></button> ' +
                        '<button type="button" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteReplace(row.entity.Id)" class="btn-icon ui-grid-custom-service-icon fa fa-times link-invert" ng-if="row.entity.Id >= 5000" aria-label="Удалить"></button> ' +
                        '</div></div>',
                },
            ],
            uiGridCustom: {
                rowClick: function ($event, row) {
                    if (row.entity.Id >= 5000) {
                        ctrl.loadReplace(row.entity.Id);
                    }
                },
                selectionOptions: [
                    {
                        text: 'Удалить выделенные',
                        url: 'shippingReplaceGeo/deleteShippingReplacesGeo',
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
        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
            ctrl.gridInited = true;
        };
        ctrl.loadReplace = function (id) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddEditShippingReplaceGeoCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: addEditTemplate,
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        params: {
                            id: id,
                        },
                    },
                })
                .result.then(
                    function (result) {
                        ctrl.onShippingReplaceGeoAddUpdate();
                        return result;
                    },
                    function (result) {
                        return result;
                    },
                );
        };
        ctrl.onShippingReplaceGeoAddUpdate = function () {
            ctrl.grid.fetchData();
        };
        ctrl.deleteReplace = function (id) {
            SweetAlert.confirm('Вы уверены, что хотите удалить?', {
                title: 'Удаление',
            }).then(function (result) {
                if (result === true || result.value === true) {
                    $http
                        .post('shippingReplaceGeo/delete', {
                            id: id,
                        })
                        .then(function (response) {
                            ctrl.grid.fetchData();
                        });
                }
            });
        };
    };
    ShippingReplaceGeoCtrl.$inject = ['$http', '$q', 'uiGridConstants', 'uiGridCustomConfig', 'SweetAlert', '$uibModal', '$translate'];
    ng.module('shippingReplaceGeo', ['uiGridCustom']).controller('ShippingReplaceGeoCtrl', ShippingReplaceGeoCtrl);
})(window.angular);
