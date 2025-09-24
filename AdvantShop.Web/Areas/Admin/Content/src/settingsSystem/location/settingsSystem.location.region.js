import addEditRegionsTemplate from './modal/addEditRegion/AddEditRegions.html';
import addEditAdditionalSettingsTemplate from './modal/addEditRegion/modal/AddEditAdditionalSettingsRegion.html';
(function (ng) {
    'use strict';

    var SettingsSystemLocationRegionCtrl = function (
        $q,
        uiGridConstants,
        uiGridCustomConfig,
        SweetAlert,
        $translate,
        uiGridCustomService,
        $uibModal,
    ) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var columnDefsRegion = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.SettingsSystem.Region'),
                    enableCellEdit: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.onSelect({id : row.entity.RegionId, name: row.entity.Name, entity: row.entity})">{{COL_FIELD}}</a></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsSystem.Region'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },
                {
                    name: 'RegionCode',
                    displayName: $translate.instant('Admin.Js.SettingsSystem.RegionCode'),
                    type: uiGridConstants.filter.INPUT,
                    enableCellEdit: true,
                    width: 150,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsSystem.RegionCode'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'RegionCode',
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
                        '<div class="ui-grid-cell-contents"><div>' +
                        '<button type="button" ng-click="grid.appScope.$ctrl.gridExtendCtrl.openModal(row.entity, \'ModalAddEditRegionsCtrl\', \'' +
                        addEditRegionsTemplate +
                        '\')" class="btn-icon ui-grid-custom-service-icon fas fa-pencil-alt">{{COL_FIELD}}</button>' +
                        '</div></div>',
                    // '<ui-modal-trigger data-controller="\'ModalAddEditRegionsCtrl\'" controller-as="ctrl" ' +
                    // 'template-url="' + addEditRegionsTemplate + '" ' +
                    // 'data-resolve="{\'entity\': row.entity}" ' +
                    // 'data-on-close="grid.appScope.$ctrl.gridExtendCtrl.fetchData()"> ' +
                    // uiGridCustomService.getTemplateCellEdit() +
                    // '</ui-modal-trigger>'
                },
                {
                    name: 'delete',
                    displayName: '',
                    width: 40,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate: uiGridCustomService.getTemplateCellDelete('Regions/DeleteRegion', '{Ids: row.entity.RegionId}'),
                },

                // {
                //         name: '_serviceColumn',
                //         displayName: '',
                //         width: 75,
                //         enableSorting: false,
                //         useInSwipeBlock: true,
                //         cellTemplate: '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div class="">' +
                //                             '<ui-modal-trigger data-controller="\'ModalAddEditRegionsCtrl\'" controller-as="ctrl" ' +
                //                             'template-url="../areas/admin/content/src/settingsSystem/location/modal/addEditRegion/AddEditRegions.html" ' +
                //                             'data-resolve="{\'entity\': row.entity}" ' +
                //                             'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                //                             '<a ng-href="" class="ui-grid-custom-service-icon fas fa-pencil-alt">{{COL_FIELD}}</a>' +
                //                        '</ui-modal-trigger>' +
                //             '<ui-grid-custom-delete url="Regions/DeleteRegion" params="{\'Ids\': row.entity.RegionId}"></ui-grid-custom-delete></div></div>' +
                //             '<div ng-if="grid.appScope.$ctrl.isMobile">' +
                //             '<ui-modal-trigger data-controller="\'ModalAddEditRegionsCtrl\'" controller-as="ctrl" ' +
                //             'template-url="../areas/admin/content/src/settingsSystem/location/modal/addEditRegion/AddEditRegions.html" ' +
                //             'data-resolve="{\'entity\': row.entity}" ' +
                //             'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                //             '<a ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadTaskGroup(row.entity.Id)" class="btn btn-sm btn-success flex center-xs middle-xs" style="height:100%;">Редактировать</a>' +
                //             '</ui-modal-trigger></div>' +
                //             '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="Regions/DeleteRegion" params="{\'Ids\': row.entity.RegionId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>'
                //     }
            ];

            ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: columnDefsRegion,
                uiGridCustom: {
                    rowUrl: '', //'countryregioncity/edit/{{row.entity.CountryId}}',
                    selectionOptions: [
                        {
                            text: $translate.instant('Admin.Js.SettingsSystem.DeleteSelected'),
                            url: 'Regions/DeleteRegion',
                            field: 'RegionId',
                            before: function () {
                                return SweetAlert.confirm($translate.instant('Admin.Js.SettingsSystem.AreYouSureDelete'), {
                                    title: $translate.instant('Admin.Js.SettingsSystem.Deleting'),
                                }).then(function (result) {
                                    return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                                });
                            },
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
                ctrl.openModal(result.entity, 'ModalAddEditAdditionalSettingsRegionCtrl', addEditAdditionalSettingsTemplate);
            } else if (result && result.returnToMainSettings) {
                ctrl.openModal(result.entity, 'ModalAddEditRegionsCtrl', addEditRegionsTemplate);
            } else {
                ctrl.grid.fetchData();
            }
        };
    };

    SettingsSystemLocationRegionCtrl.$inject = [
        '$q',
        'uiGridConstants',
        'uiGridCustomConfig',
        'SweetAlert',
        '$translate',
        'uiGridCustomService',
        '$uibModal',
    ];

    ng.module('settingsSystem').controller('SettingsSystemLocationRegionCtrl', SettingsSystemLocationRegionCtrl);
})(window.angular);
