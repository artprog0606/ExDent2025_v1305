(function (ng) {
    'use strict';

    var BrandsListCtrl = function (
        $q,
        $location,
        $window,
        uiGridConstants,
        uiGridCustomConfig,
        uiGridCustomParamsConfig,
        uiGridCustomService,
        brandsListService,
        SweetAlert,
        $translate,
        $scope,
    ) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'PhotoSrc',
                    headerCellClass: 'ui-grid-custom-header-cell-center',
                    displayName: $translate.instant('Admin.Js.BrandList.Img'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><a class="ui-grid-custom-flex-center ui-grid-custom-link-for-img" ng-href="brands/edit/{{row.entity.BrandId}}"><img class="ui-grid-custom-col-img" ng-src="{{row.entity.PhotoSrc}}"></a></div>',
                    width: 80,
                    enableSorting: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Brandlist.Image'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'HasPhoto',
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.BrandList.WithPhoto'),
                                value: true,
                            },
                            { label: $translate.instant('Admin.Js.BrandList.WithoutPhoto'), value: false },
                        ],
                    },
                },
                {
                    name: 'BrandName',
                    displayName: $translate.instant('Admin.Js.BrandList.Name'),
                    enableCellEdit: false,
                    cellTemplate: '<div class="ui-grid-cell-contents"><a ng-href="brands/edit/{{row.entity.BrandId}}">{{COL_FIELD}}</a></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.BrandList.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'BrandName',
                    },
                },
                {
                    name: 'CountryName',
                    displayName: $translate.instant('Admin.Js.BrandList.Country'),
                    enableCellEdit: false,
                    width: 250,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.BrandList.Country'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'CountryId',
                        fetch: 'countries/getcountries',
                    },
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.BrandList.Ord'),
                    type: 'number',
                    width: 80,
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.BrandList.Sorting'),
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
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.BrandList.Activ'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row"></ui-grid-custom-switch>',
                    width: 76,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.BrandList.Activity'),
                        name: 'Enabled',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.BrandList.Active'),
                                value: true,
                            },
                            { label: $translate.instant('Admin.Js.BrandList.Inactive'), value: false },
                        ],
                    },
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div class="js-grid-not-clicked"><a ng-href="brands/edit/{{row.entity.BrandId}}" class="ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></a><ui-grid-custom-delete url="brands/deletebrand" params="{\'BrandId\': row.entity.BrandId}"></ui-grid-custom-delete></div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="brands/deletebrand" params="{\'BrandId\': row.entity.BrandId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];

        var SETTINGSTAB_SEARCH_NAME = 'BrandsListTab';

        ctrl.$onInit = function () {
            var search = $location.search();
            ctrl.settingsTab = (search != null && search[SETTINGSTAB_SEARCH_NAME]) || 'list';
        };

        ctrl.changeSettingsTab = function (tab) {
            ctrl.settingsTab = tab;
            $location.search(SETTINGSTAB_SEARCH_NAME, tab);
        };

        ctrl.reinitSwipeLine = function () {
            $scope.$broadcast('reinitSwipeLineEvent');
        };

        ctrl.categories = [];
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowUrl: 'brands/edit/{{row.entity.BrandId}}',
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.BrandList.DeleteSelected'),
                        url: 'brands/deletebrands',
                        field: 'BrandId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.BrandList.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.BrandList.Deleting'),
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                    {
                        text: $translate.instant('Admin.Js.BrandList.MakeActive'),
                        url: 'brands/activatebrands',
                        field: 'BrandId',
                    },
                    {
                        text: $translate.instant('Admin.Js.BrandList.MakeIncactive'),
                        url: 'brands/disablebrands',
                        field: 'BrandId',
                    },
                ],
            },
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.export = function () {
            if (ctrl.grid != null) {
                ctrl.grid.export();
            }
        };

        ctrl.onFinishImport = function (data) {
            if (data.ProcessedPercent === 100) {
                ctrl.grid.fetchData();
            }
        };
    };

    BrandsListCtrl.$inject = [
        '$q',
        '$location',
        '$window',
        'uiGridConstants',
        'uiGridCustomConfig',
        'uiGridCustomParamsConfig',
        'uiGridCustomService',
        'brandsListService',
        'SweetAlert',
        '$translate',
        '$scope',
    ];

    ng.module('brandsList', ['uiGridCustom']).controller('BrandsListCtrl', BrandsListCtrl);
})(window.angular);
