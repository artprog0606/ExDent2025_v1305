import addEditCitysTemplate from './modal/addEditCitys/AddEditCitys.html';
import addEditAdditionalSettingsTemplate from './modal/addEditCitys/modal/AddEditAdditionalSettingsCity.html';
(function (ng) {
    'use strict';

    var SettingsSystemLocationCityCtrl = function ($q, uiGridConstants, uiGridCustomConfig, SweetAlert, $translate, $uibModal) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var columnDefsCity = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.SettingsSystem.City'),
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsSystem.City'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },
                {
                    name: 'DisplayInPopup',
                    displayName: $translate.instant('Admin.Js.SettingsSystem.BasicCity'),
                    enableCellEdit: true,
                    type: 'checkbox',
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><label class="ui-grid-custom-edit-field adv-checkbox-label" data-e2e="switchOnOffLabel"><input type="checkbox" class="adv-checkbox-input" ng-model="MODEL_COL_FIELD " data-e2e="switchOnOffSelect" /><span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span></label></div>',
                    width: 80,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsSystem.BasicCity'),
                        name: 'DisplayInPopup',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            { label: $translate.instant('Admin.Js.SettingsSystem.Yes'), value: true },
                            { label: $translate.instant('Admin.Js.SettingsSystem.No'), value: false },
                        ],
                    },
                },
                {
                    name: 'PhoneNumber',
                    displayName: $translate.instant('Admin.Js.SettingsSystem.PhoneNumber'),
                    type: uiGridConstants.filter.INPUT,
                    enableCellEdit: true,
                    width: 150,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsSystem.PhoneNumber'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'PhoneNumber',
                    },
                },
                {
                    name: 'MobilePhoneNumber',
                    displayName: $translate.instant('Admin.Js.SettingsSystem.PhoneNumberInMobileVersion'),
                    type: uiGridConstants.filter.INPUT,
                    enableCellEdit: true,
                    width: 150,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsSystem.PhoneNumberInMobileVersion'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'MobilePhoneNumber',
                    },
                },
                {
                    name: 'Zip',
                    displayName: $translate.instant('Admin.Js.SettingsSystem.Zip'),
                    type: uiGridConstants.filter.INPUT,
                    enableCellEdit: true,
                    width: 100,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsSystem.Zip'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Zip',
                    },
                },
                {
                    name: 'CitySort',
                    displayName: $translate.instant('Admin.Js.SettingsSystem.Order'),
                    enableCellEdit: true,
                    type: 'number',
                    width: 100,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsSystem.Order'),
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
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<button type="button" ng-click="grid.appScope.$ctrl.gridExtendCtrl.openModal(row.entity, \'ModalAddEditCitysCtrl\', \'' +
                        addEditCitysTemplate +
                        '\')" class="btn-icon ui-grid-custom-service-icon fas fa-pencil-alt">{{COL_FIELD}}</button>' +
                        '<ui-grid-custom-delete url="Cities/DeleteCity" params="{\'Ids\': row.entity.CityId}"></ui-grid-custom-delete></div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="Cities/DeleteCity" params="{\'Ids\': row.entity.CityId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];

            ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: columnDefsCity,
                uiGridCustom: {
                    rowUrl: '', //'countryregioncity/edit/{{row.entity.CountryId}}',
                    selectionOptions: [
                        {
                            text: $translate.instant('Admin.Js.SettingsSystem.DeleteSelected'),
                            url: 'Cities/DeleteCity',
                            field: 'CityId',
                            before: function () {
                                return SweetAlert.confirm($translate.instant('Admin.Js.SettingsSystem.AreYouSureDelete'), {
                                    title: $translate.instant('Admin.Js.SettingsSystem.Deleting'),
                                }).then(function (result) {
                                    return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                                });
                            },
                        },
                        {
                            text: $translate.instant('Admin.Js.SettingsSystem.ShowWhenSelectingACity'),
                            url: 'Cities/ActivateCity',
                            field: 'CityId',
                        },
                        {
                            text: $translate.instant('Admin.Js.SettingsSystem.NotShowWhenSelectingACity'),
                            url: 'Cities/DisableCity',
                            field: 'CityId',
                        },
                    ],
                },
            });
        };

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.openModal = function (entity, controller, template) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: controller,
                    controllerAs: 'ctrl',
                    templateUrl: template,
                    resolve: {
                        entity: function () {
                            return entity;
                        },
                    },
                })
                .result.then(
                    function (result) {
                        ctrl.modalClose(result);
                        return result;
                    },
                    function (result) {
                        ctrl.modalClose(result);
                        return result;
                    },
                );
        };

        ctrl.modalClose = function (result) {
            if (result && result.openAdditionalSettings) {
                ctrl.openModal(result.entity, 'ModalAddEditAdditionalSettingsCityCtrl', addEditAdditionalSettingsTemplate);
            } else if (result && result.returnToMainSettings) {
                ctrl.openModal(result.entity, 'ModalAddEditCitysCtrl', addEditCitysTemplate);
            } else {
                ctrl.grid.fetchData();
            }
        };
    };

    SettingsSystemLocationCityCtrl.$inject = ['$q', 'uiGridConstants', 'uiGridCustomConfig', 'SweetAlert', '$translate', '$uibModal'];

    ng.module('settingsSystem').controller('SettingsSystemLocationCityCtrl', SettingsSystemLocationCityCtrl);
})(window.angular);
