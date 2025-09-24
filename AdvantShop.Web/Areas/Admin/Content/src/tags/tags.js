import addTagTemplate from '../_shared/modal/addTag/addTag.html';
(function (ng) {
    'use strict';

    var TagsCtrl = function (
        $location,
        $window,
        uiGridConstants,
        uiGridCustomConfig,
        uiGridCustomParamsConfig,
        uiGridCustomService,
        toaster,
        $http,
        $q,
        SweetAlert,
        $translate,
    ) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Tags.Name'),
                    cellTemplate:
                        '<ui-modal-trigger class="ui-grid-cell-contents" data-controller="\'ModalAddTagCtrl\'" controller-as="ctrl" size="lg" data-template-url="' +
                        addTagTemplate +
                        '" data-resolve="{data: {tagId: row.entity.Id}}" data-on-close="tags.grid.fetchData()"><a href="javascript:void(0);">{{COL_FIELD}}</a></ui-modal-trigger>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tags.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },
                {
                    name: 'UrlPath',
                    displayName: $translate.instant('Admin.Js.Tags.SynonymForUrl'),
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tags.SynonymForUrl'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Url',
                    },
                },
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.Tags.Activ'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row" field-name="Enabled"></ui-grid-custom-switch>',
                    width: 76,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tags.Activity'),
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            { label: $translate.instant('Admin.Js.Tags.Active'), value: true },
                            { label: $translate.instant('Admin.Js.Tags.Inactive'), value: false },
                        ],
                    },
                },
                {
                    name: 'VisibilityForUsers',
                    displayName: $translate.instant('Admin.Js.Tags.Visibility'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row" field-name="VisibilityForUsers"></ui-grid-custom-switch>',
                    width: 100,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tags.Visibility'),
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            { label: $translate.instant('Admin.Js.Tags.Yes'), value: true },
                            { label: $translate.instant('Admin.Js.Tags.No'), value: false },
                        ],
                    },
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.Tags.Sorting'),
                    enableCellEdit: true,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<ui-modal-trigger data-controller="\'ModalAddTagCtrl\'" controller-as="ctrl" size="lg" data-template-url="' +
                        addTagTemplate +
                        '" data-resolve="{data: {tagId: row.entity.Id}}" data-on-close="tags.grid.fetchData()"><button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></button></ui-modal-trigger>' +
                        '<ui-grid-custom-delete url="tags/deleteTag" params="{\'id\': row.entity.Id}"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="tags/deleteTag" params="{\'id\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                // rowUrl: 'tags/edit/{{row.entity.Id}}',
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Tags.DeleteSelected'),
                        url: 'tags/deleteTags',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Tags.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.Tags.Deleting'),
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
        });

        ctrl.gridOnInit = function (grid, parentCtrl) {
            ctrl.grid = grid;
            if (parentCtrl) {
                parentCtrl.tagsGrid = grid;
            }
        };

        ctrl.setActive = function (active, id) {
            if (id <= 0) return;

            $http.post('tags/setTagActive', { id: id, active: active }).then(function (response) {
                if (response.data === true) {
                    ctrl.Enabled = active;
                    toaster.pop('success', $translate.instant('Admin.Js.Tags.ChangesSaved'));
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.Tags.ErrorChangingActivity'), '');
                }
            });
        };

        ctrl.deleteTag = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.Tags.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Tags.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('tags/deleteTag', { id: id }).then(function (response) {
                        //$window.location.assign('tags');
                        $window.location.assign('settingscatalog?catalogTab=tags');
                    });
                }
            });
        };
    };

    TagsCtrl.$inject = [
        '$location',
        '$window',
        'uiGridConstants',
        'uiGridCustomConfig',
        'uiGridCustomParamsConfig',
        'uiGridCustomService',
        'toaster',
        '$http',
        '$q',
        'SweetAlert',
        '$translate',
    ];

    ng.module('tags', ['uiGridCustom', 'urlHelper']).controller('TagsCtrl', TagsCtrl);
})(window.angular);
