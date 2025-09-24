import addEditSettingsSearchTemplate from './modal/addEditSettingsSearch/AddEditSettingsSearch.html';
(function (ng) {
    'use strict';

    var SettingsSearchCtrl = function (
        $location,
        $window,
        uiGridConstants,
        uiGridCustomConfig,
        uiGridCustomParamsConfig,
        uiGridCustomService,
        toaster,
        SweetAlert,
        $http,
        $q,
        $translate,
    ) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Title',
                    displayName: $translate.instant('Admin.Js.SettingsSearch.Title'),
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SettingsSearch.Title'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Title',
                    },
                },
                {
                    name: 'Link',
                    displayName: $translate.instant('Admin.Js.SettingSearch.Link'),
                    width: 400,
                    enableCellEdit: false,
                    cellTemplate: '<div class="ui-grid-cell-contents">' + '<a href="{{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a> ' + '</div>',
                },
                {
                    name: 'KeyWords',
                    displayName: $translate.instant('Admin.Js.SettingsSearch.KeyWords'),
                    width: 400,
                    enableCellEdit: true,
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.SettingsSearch.Sorting'),
                    width: 100,
                    enableCellEdit: true,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 45,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                        '<ui-modal-trigger data-controller="\'ModalAddEditSettingsSearchCtrl\'" controller-as="ctrl" ' +
                        'template-url="' +
                        addEditSettingsSearchTemplate +
                        '" ' +
                        "data-resolve=\"{value:{'Id': row.entity.Id, 'Title': row.entity.Title, 'Link': row.entity.Link, 'KeyWords': row.entity.KeyWords, 'SortOrder': row.entity.SortOrder }}\"" +
                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                        '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></button> ' +
                        '</ui-modal-trigger>' +
                        // ' <a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.delete(row.entity.Id)" ng-class="(\'ui-grid-custom-service-icon fa fa-times link-invert\')"></a> ' +
                        '</div></div>',
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            selectionEnabled: false,
            rowHeight: 40,
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.delete = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.SettingsSystem.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.SettingsSystem.Deleting'),
            }).then(function (result) {
                if (result === true || result.value === true) {
                    $http.post('SettingsSearch/deleteSettingsSearch', { id: id }).then(function (response) {
                        ctrl.grid.fetchData();
                    });
                }
            });
        };
    };

    SettingsSearchCtrl.$inject = [
        '$location',
        '$window',
        'uiGridConstants',
        'uiGridCustomConfig',
        'uiGridCustomParamsConfig',
        'uiGridCustomService',
        'toaster',
        'SweetAlert',
        '$http',
        '$q',
        '$translate',
    ];

    ng.module('settingsSearch', ['uiGridCustom', 'urlHelper']).controller('SettingsSearchCtrl', SettingsSearchCtrl);
})(window.angular);
