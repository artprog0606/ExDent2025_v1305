(function (ng) {
    'use strict';

    var NewsCtrl = function (
        $location,
        $window,
        uiGridConstants,
        uiGridCustomConfig,
        $q,
        SweetAlert,
        $http,
        $uibModal,
        $translate,
        toaster,
        uiGridCustomService,
    ) {
        var NEWS_TAB_SEARCH_NAME = 'newsCategoryTab';
        var ctrl = this;

        ctrl.$onInit = function () {
            var search = $location.search();
            ctrl.activeTab = (search != null && search[NEWS_TAB_SEARCH_NAME]) || 'news';
        };

        var columnDefs = [
            {
                name: 'Title',
                displayName: $translate.instant('Admin.Js.News.Title'),
                cellTemplate: '<div class="ui-grid-cell-contents"><a href="news/edit/{{row.entity.NewsId}}">{{COL_FIELD}}</a></div>',
                filter: {
                    placeholder: $translate.instant('Admin.Js.News.Title'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Title',
                },
            },
            {
                name: 'AddingDateFormatted',
                displayName: $translate.instant('Admin.Js.News.AddingDate'),
                width: 150,
                filter: {
                    placeholder: $translate.instant('Admin.Js.News.AddingDate'),
                    type: 'datetime',
                    term: {
                        from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                        to: new Date(),
                    },
                    datetimeOptions: {
                        from: {
                            name: 'AddingDateFrom',
                        },
                        to: {
                            name: 'AddingDateTo',
                        },
                    },
                },
            },
            {
                name: 'NewsCategory',
                displayName: $translate.instant('Admin.Js.News.Category'),
                width: 200,
                filter: {
                    placeholder: $translate.instant('Admin.Js.News.Category'),
                    type: uiGridConstants.filter.SELECT,
                    name: 'NewsCategoryId',
                    fetch: 'news/getNewsCategories',
                },
            },
            {
                name: 'ShowOnMainPage',
                displayName: $translate.instant('Admin.Js.News.AtHomePage'),
                cellTemplate: '<ui-grid-custom-switch row="row" field-name="ShowOnMainPage" class="js-grid-not-clicked"></ui-grid-custom-switch>',
                enableCellEdit: false,
                width: 90,
                filter: {
                    placeholder: $translate.instant('Admin.Js.News.AtHomePage'),
                    type: uiGridConstants.filter.SELECT,
                    name: 'ShowOnMainPage',
                    selectOptions: [
                        {
                            label: $translate.instant('Admin.Js.News.Yes'),
                            value: true,
                        },
                        { label: $translate.instant('Admin.Js.News.No'), value: false },
                    ],
                },
            },
            {
                name: 'Enabled',
                displayName: $translate.instant('Admin.Js.News.SheActive'),
                enableCellEdit: false,
                cellTemplate: '<ui-grid-custom-switch row="row" field-name="Enabled" class="js-grid-not-clicked"></ui-grid-custom-switch>',
                width: 90,
                filter: {
                    placeholder: $translate.instant('Admin.Js.News.Activity'),
                    name: 'Enabled',
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: [
                        {
                            label: $translate.instant('Admin.Js.News.TheyActive'),
                            value: true,
                        },
                        { label: $translate.instant('Admin.Js.News.Inactive'), value: false },
                    ],
                },
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 80,
                useInSwipeBlock: true,
                cellTemplate:
                    '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                    '<a ng-href="news/edit/{{row.entity.NewsId}}" class="ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></a>' +
                    '<ui-grid-custom-delete url="news/deleteNewsItem" params="{\'NewsId\': row.entity.NewsId}"></ui-grid-custom-delete>' +
                    '</div></div>' +
                    '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="news/deleteNewsItem" params="{\'NewsId\': row.entity.NewsId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
            },
        ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.News.DeleteSelected'),
                        url: 'news/deleteNews',
                        field: 'NewsId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.News.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.News.Deleting'),
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                    {
                        text: $translate.instant('Admin.Js.News.MakeActive'),
                        url: 'news/setNewsEnabled',
                        field: 'NewsId',
                    },
                    {
                        text: $translate.instant('Admin.Js.News.MakeInactive'),
                        url: 'news/setNewsDisabled',
                        field: 'NewsId',
                    },
                    {
                        text: $translate.instant('Admin.Js.News.ShowAtMain'),
                        url: 'news/setNewsOnMainPage',
                        field: 'NewsId',
                    },
                    {
                        text: $translate.instant('Admin.Js.News.DoNotShowAtMain'),
                        url: 'news/setNewsNotOnMainPage',
                        field: 'NewsId',
                    },
                ],
            },
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.changeRssViewNews = function () {
            $http.post('news/changeRssViewNews', { enabled: ctrl.RssViewNews }).then(function (response) {
                toaster.pop('success', '', $translate.instant('Admin.Js.ChangesSaved'));
            });
        };

        ctrl.updateGrid = function () {
            ctrl.grid.fetchData();
        };
    };

    NewsCtrl.$inject = [
        '$location',
        '$window',
        'uiGridConstants',
        'uiGridCustomConfig',
        '$q',
        'SweetAlert',
        '$http',
        '$uibModal',
        '$translate',
        'toaster',
        'uiGridCustomService',
    ];

    ng.module('news', ['uiGridCustom', 'urlHelper']).controller('NewsCtrl', NewsCtrl);
})(window.angular);
