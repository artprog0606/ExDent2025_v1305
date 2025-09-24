import addEditNewsCategoryTemplate from './modal/addEditNewsCategory/addEditNewsCategpry.html';
(function (ng) {
    'use strict';

    var NewsCategoryCtrl = function (
        $location,
        $window,
        uiGridConstants,
        uiGridCustomConfig,
        $q,
        SweetAlert,
        $http,
        $uibModal,
        $translate,
        uiGridCustomService,
    ) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.NewsCategory.Name'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents news-category-link"><ui-modal-trigger data-controller="\'ModalAddEditNewsCategoryCtrl\'" controller-as="ctrl" ' +
                        'template-url="' +
                        addEditNewsCategoryTemplate +
                        '" ' +
                        'data-resolve="{\'id\': row.entity.NewsCategoryId}" ' +
                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                        '<a ng-href="">{{COL_FIELD}}</a>' +
                        '</ui-modal-trigger></div>',
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.NewsCategory.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },
                {
                    name: 'UrlPath',
                    displayName: $translate.instant('Admin.Js.NewsCategory.URLsynonym'),
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.NewsCategory.URLsynonym'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'UrlPath',
                    },
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.NewsCategory.Sorting'),
                    enableCellEdit: true,
                },
                {
                    name: '_serviceColumnEdit',
                    displayName: '',
                    width: 38,
                    enableSorting: false,
                    // useInSwipeBlock: true,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                        '<ui-modal-trigger data-controller="\'ModalAddEditNewsCategoryCtrl\'" controller-as="ctrl" ' +
                        'template-url="' +
                        addEditNewsCategoryTemplate +
                        '" ' +
                        'data-resolve="{\'id\': row.entity.NewsCategoryId}" ' +
                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                        uiGridCustomService.getTemplateCellLink('') +
                        // '<a ng-href="" class="ui-grid-custom-service-icon fas fa-pencil-alt">{{COL_FIELD}}</a>' +
                        '</ui-modal-trigger>' +
                        '</div></div>',
                },
                {
                    name: '_serviceColumnDelete',
                    displayName: '',
                    width: 35,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate: uiGridCustomService.getTemplateCellDelete(
                        'NewsCategory/DeleteNewsCategory',
                        '{Ids: row.entity.NewsCategoryId}',
                        'confirm-text="' + $translate.instant('Admin.Js.NewsCategory.WhenDeleteNewsCategory') + '"',
                    ),
                    // cellTemplate:
                    //     '<div class="ui-grid-cell-contents"><div>' +
                    //         '<ui-modal-trigger data-controller="\'ModalAddEditNewsCategoryCtrl\'" controller-as="ctrl" ' +
                    //                 'template-url="../areas/admin/content/src/newsCategory/modal/addEditNewsCategory/addEditNewsCategpry.html" ' +
                    //                 'data-resolve="{\'id\': row.entity.NewsCategoryId}" ' +
                    //                 'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                    //                 '<a ng-href="" class="ui-grid-custom-service-icon fas fa-pencil-alt">{{COL_FIELD}}</a>' +
                    // '</ui-modal-trigger>' +
                    // '<ui-grid-custom-delete url="NewsCategory/DeleteNewsCategory" params="{\'Ids\': row.entity.NewsCategoryId}" confirm-text="' + $translate.instant('Admin.Js.NewsCategory.WhenDeleteNewsCategory') + '"></ui-grid-custom-delete>' +
                    //     '</div></div>'
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.NewsCategory.DeleteSelected'),
                        url: 'NewsCategory/DeleteNewsCategory',
                        field: 'NewsCategoryId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.NewsCategory.WhenDeleteNewsCategory'), {
                                title: $translate.instant('Admin.Js.NewsCategory.Deleting'),
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
    };

    NewsCategoryCtrl.$inject = [
        '$location',
        '$window',
        'uiGridConstants',
        'uiGridCustomConfig',
        '$q',
        'SweetAlert',
        '$http',
        '$uibModal',
        '$translate',
        'uiGridCustomService',
    ];

    ng.module('newsCategory', ['uiGridCustom', 'urlHelper']).controller('NewsCategoryCtrl', NewsCategoryCtrl);
})(window.angular);
