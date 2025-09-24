import addEditTemplateTemplate from './modal/addEditTemplate/addEditTemplate.html';
(function (ng) {
    'use strict';

    var SettingsTemplatesDocxCtrl = function ($uibModal, $http, $q, uiGridConstants, uiGridCustomConfig, SweetAlert, toaster, $translate) {
        var ctrl = this;
        ctrl.gridTemplatesInited = false;
        var columnDefsTemplates = [
            {
                name: 'Name',
                displayName: 'Название',
                cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>',
                enableCellEdit: true,
            },
            {
                name: 'TaxTypeFormatted',
                displayName: 'Тип',
                enableCellEdit: false,
                width: 200,
                filter: {
                    placeholder: 'Тип',
                    type: uiGridConstants.filter.SELECT,
                    name: 'Type',
                    fetch: 'settingsTemplatesDocx/getTemplateTypes',
                },
            },
            {
                name: 'SortOrder',
                displayName: 'Сортировка',
                enableCellEdit: true,
            },
            {
                name: 'FileSizeFormatted',
                displayName: 'Размер файла',
                enableCellEdit: false,
                width: 200,
            },
            {
                name: 'DateModifiedFormatted',
                displayName: 'Дата изменения',
                width: 150,
            },
            {
                name: 'Файл',
                cellTemplate: '<div class="ui-grid-cell-contents">' + '<a href="" ng-href="{{row.entity.PathAdmin}}">Скачать</a>' + '</div>',
                enableCellEdit: false,
                enableSorting: false,
                width: 100,
            },
            {
                name: '_serviceColumn',
                displayName: '',
                enableSorting: false,
                useInSwipeBlock: true,
                width: 80,
                cellTemplate:
                    '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                    '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadTemplate(row.entity.Id)"></button> ' +
                    '<button type="button" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteTemplate(row.entity.Id)" class="btn-icon ui-grid-custom-service-icon fa fa-times link-invert" aria-label="Удалить"></button> ' +
                    '</div></div>' +
                    '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="settingsTemplatesDocx/deleteTemplate" params="{\'id\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
            },
        ];
        ctrl.gridTemplatesOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsTemplates,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadTemplate(row.entity.Id);
                },
                selectionOptions: [
                    {
                        text: 'Удалить выделенные',
                        url: 'settingsTemplatesDocx/deleteTemplates',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm('Вы уверены, что хотите удалить?', {
                                title: 'Удаление',
                            }).then(function (result) {
                                if (result === true || result.value === true) {
                                    return $q.resolve('sweetAlertConfirm');
                                } else {
                                    $q.reject('sweetAlertCancel');
                                }
                            });
                        },
                    },
                ],
            },
        });
        ctrl.gridTemplatesOnInit = function (grid) {
            ctrl.gridTemplates = grid;
            ctrl.gridTemplatesInited = true;
        };
        ctrl.gridTemplatesUpdate = function () {
            ctrl.gridTemplates.fetchData();
        };
        ctrl.loadTemplate = function (id) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddEditTemplateCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: addEditTemplateTemplate,
                    resolve: {
                        params: {
                            id: id,
                        },
                    },
                })
                .result.then(
                    function (result) {
                        ctrl.gridTemplates.fetchData();
                        return result;
                    },
                    function (result) {
                        return result;
                    },
                );
        };
        ctrl.deleteTemplate = function (id) {
            SweetAlert.confirm('Вы уверены, что хотите удалить?', {
                title: 'Удаление',
            }).then(function (result) {
                if (result === true || result.value === true) {
                    $http
                        .post('settingsTemplatesDocx/deleteTemplate', {
                            id: id,
                        })
                        .then(function (response) {
                            ctrl.gridTemplates.fetchData();
                        });
                }
            });
        };
    };
    SettingsTemplatesDocxCtrl.$inject = ['$uibModal', '$http', '$q', 'uiGridConstants', 'uiGridCustomConfig', 'SweetAlert', 'toaster', '$translate'];
    ng.module('settingsTemplatesDocx', ['uiGridCustom', 'fileUploader', 'ngFileUpload']).controller(
        'SettingsTemplatesDocxCtrl',
        SettingsTemplatesDocxCtrl,
    );
})(window.angular);
