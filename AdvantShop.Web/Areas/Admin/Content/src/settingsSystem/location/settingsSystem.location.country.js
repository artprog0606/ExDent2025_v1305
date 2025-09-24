import addEditCountryTemplate from './modal/addEditCountry/AddEditCountry.html';
(function (ng) {
    'use strict';

    var SettingsSystemLocationCountryCtrl = function ($q, uiGridConstants, uiGridCustomConfig, SweetAlert, $translate, uiGridCustomService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var columnDefsCountry = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.SettingsSystem.Country'),
                    enableCellEdit: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.onSelect({id : row.entity.CountryId, name: row.entity.Name, entity: row.entity})">{{COL_FIELD}}</a></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsSystem.Country'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },
                {
                    name: 'Iso2',
                    displayName: 'ISO2',
                    enableCellEdit: true,
                    uiGridCustomEdit: {
                        attributes: {
                            maxlength: 2,
                        },
                    },
                    width: 80,
                    filter: {
                        placeholder: 'ISO2',
                        type: uiGridConstants.filter.INPUT,
                        name: 'Iso2',
                    },
                },
                {
                    name: 'Iso3',
                    displayName: 'ISO3',
                    width: 80,
                    enableCellEdit: true,
                    uiGridCustomEdit: {
                        attributes: {
                            maxlength: 3,
                        },
                    },
                    filter: {
                        placeholder: 'ISO3',
                        type: uiGridConstants.filter.INPUT,
                        name: 'Iso3',
                    },
                },
                {
                    name: 'DisplayInPopup',
                    displayName: $translate.instant('Admin.Js.SettingsSytem.BasicCountry'),
                    enableCellEdit: true,
                    type: 'checkbox',
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><label class="ui-grid-custom-edit-field adv-checkbox-label" data-e2e="switchOnOffLabel"><input type="checkbox" class="adv-checkbox-input" ng-model="MODEL_COL_FIELD " data-e2e="switchOnOffSelect" /><span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span></label></div>',
                    width: 80,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsSytem.BasicCountry'),
                        name: 'DisplayInPopup',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            { label: $translate.instant('Admin.Js.SettingsSystem.Yes'), value: true },
                            { label: $translate.instant('Admin.Js.SettingsSystem.No'), value: false },
                        ],
                    },
                },
                {
                    name: 'DialCode',
                    displayName: $translate.instant('Admin.Js.SettingsSystem.PhoneCode'),
                    type: 'number',
                    enableCellEdit: true,
                    width: 150,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsSystem.PhoneCode'),
                        type: 'number',
                        name: 'DialCode',
                    },
                },
                {
                    name: 'SortOrder',
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
                    name: 'edit',
                    displayName: '',
                    width: 35,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<ui-modal-trigger data-controller="\'ModalAddEditCountryCtrl\'" controller-as="ctrl" ' +
                        'template-url="' +
                        addEditCountryTemplate +
                        '" ' +
                        'data-resolve="{\'entity\': row.entity}" ' +
                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                        uiGridCustomService.getTemplateCellEdit() +
                        '</ui-modal-trigger>',

                    //     cellTemplate:
                    //         '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents">' +
                    //             '<div class="">' +
                    //                 '<ui-modal-trigger data-controller="\'ModalAddEditCountryCtrl\'" controller-as="ctrl" ' +
                    //                         'template-url="../areas/admin/content/src/settingsSystem/location/modal/addEditCountry/AddEditCountry.html" ' +
                    //                         'data-resolve="{\'entity\': row.entity}" ' +
                    //                         'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                    //                             '<a ng-href="" class="ui-grid-custom-service-icon fas fa-pencil-alt">{{COL_FIELD}}</a>' +
                    //                 '</ui-modal-trigger>' +
                    //                 '<ui-grid-custom-delete url="countries/deletecountry" params="{\'Ids\': row.entity.CountryId}"></ui-grid-custom-delete>' +
                    //             '</div>' +
                    //         '</div>' +
                    //         '<div ng-if="grid.appScope.$ctrl.isMobile">' +
                    //             '<ui-modal-trigger data-controller="\'ModalAddEditCountryCtrl\'" controller-as="ctrl" ' +
                    //                                 'template-url="../areas/admin/content/src/settingsSystem/location/modal/addEditCountry/AddEditCountry.html" ' +
                    //                                 'data-resolve="{\'entity\': row.entity}" ' +
                    //                                 'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                    //                 '<a ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadTaskGroup(row.entity.Id)" class="btn btn-sm btn-success flex center-xs middle-xs" style="height:100%;">Редактировать</a>' +
                    //             '</ui-modal-trigger>' +
                    //         '</div>' +
                    //         '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="countries/deletecountry" params="{\'Ids\': row.entity.CountryId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>'
                    //
                },
                {
                    name: 'delete',
                    displayName: '',
                    width: 40,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate: uiGridCustomService.getTemplateCellDelete('countries/deletecountry', '{Ids: row.entity.CountryId}'),
                },
            ];

            ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: columnDefsCountry,
                uiGridCustom: {
                    rowUrl: '', //'countryregioncity/edit/{{row.entity.CountryId}}',
                    selectionOptions: [
                        {
                            text: $translate.instant('Admin.Js.SettingsSystem.DeleteSelected'),
                            url: 'countries/deletecountry',
                            field: 'CountryId',
                            before: function () {
                                return SweetAlert.confirm($translate.instant('Admin.Js.SettingsSystem.AreYouSureDelete'), {
                                    title: $translate.instant('Admin.Js.SettingsSystem.Deleting'),
                                }).then(function (result) {
                                    return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                                });
                            },
                        },
                        {
                            text: $translate.instant('Admin.Js.SettingsSystem.ShowWhenCountrySelected'),
                            url: 'countries/activatecountry',
                            field: 'CountryId',
                        },
                        {
                            text: $translate.instant('Admin.Js.SettingsSystem.NotDisplayWhenSelectingACountry'),
                            url: 'countries/disablecountry',
                            field: 'CountryId',
                        },
                    ],
                },
            });
        };
    };

    SettingsSystemLocationCountryCtrl.$inject = ['$q', 'uiGridConstants', 'uiGridCustomConfig', 'SweetAlert', '$translate', 'uiGridCustomService'];

    ng.module('settingsSystem').controller('SettingsSystemLocationCountryCtrl', SettingsSystemLocationCountryCtrl);
})(window.angular);
