import addEditColorTemplate from './modal/addEditColor/AddEditColor.html';
(function (ng) {
    'use strict';

    var ColorsCtrl = function (
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
                    name: 'ColorIcon',
                    displayName: $translate.instant('Admin.Js.Colors.Icon'),
                    width: 75,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                        '<span ng-if="row.entity.ColorIcon != \'\'"><img ng-src="{{row.entity.ColorIcon}}" /></span> ' +
                        '<span ng-if="row.entity.ColorIcon == \'\'"><i class="fa fa-square color-square" ng-style={"color":row.entity.ColorCode}></i></span> ' +
                        '</div>',
                },
                {
                    name: 'ColorName',
                    displayName: $translate.instant('Admin.Js.Colors.Name'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                        '<ui-modal-trigger data-controller="\'ModalAddEditColorCtrl\'" controller-as="ctrl" ' +
                        'template-url="' +
                        addEditColorTemplate +
                        '" ' +
                        'data-resolve="{\'colorId\': row.entity.ColorId}" ' +
                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                        '<a href="">{{COL_FIELD}}</a> ' +
                        '</ui-modal-trigger>' +
                        '</div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Colors.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'ColorName',
                    },
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.Colors.Order'),
                    width: 100,
                    enableCellEdit: true,
                },
                {
                    name: 'ProductsCount',
                    displayName: $translate.instant('Admin.Js.Colors.UsedForProducts'),
                    width: 90,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"> <a ng-href="catalog?showMethod=AllProducts&grid=%7B%22ColorId%22:{{row.entity.ColorId}}%7D">{{ COL_FIELD }}</a></div>',
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<ui-modal-trigger data-controller="\'ModalAddEditColorCtrl\'" controller-as="ctrl" ' +
                        'template-url="' +
                        addEditColorTemplate +
                        '" ' +
                        'data-resolve="{\'colorId\': row.entity.ColorId}" ' +
                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                        '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></button> ' +
                        '</ui-modal-trigger>' +
                        ' <button type="button" class="btn-icon" ng-click="grid.appScope.$ctrl.gridExtendCtrl.delete(row.entity.CanBeDeleted, row.entity.ColorId)" ng-class="(!row.entity.CanBeDeleted ? \'ui-grid-custom-service-icon fa fa-times link-disabled\' : \'ui-grid-custom-service-icon fa fa-times link-invert\')" aria-label="Удалить"></button> ' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="colors/deleteColor" params="{\'colorId\': row.entity.ColorId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Colors.DeleteSelected'),
                        url: 'colors/deleteColors',
                        field: 'ColorId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Colors.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.Colors.Deleting'),
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

        ctrl.delete = function (canBeDeleted, colorId) {
            if (canBeDeleted) {
                SweetAlert.confirm($translate.instant('Admin.Js.Colors.AreYouSureDelete'), {
                    title: $translate.instant('Admin.Js.Colors.Deleting'),
                }).then(function (result) {
                    if (result === true || result.value) {
                        $http.post('colors/deleteColor', { colorId: colorId }).then(function (response) {
                            ctrl.grid.fetchData();
                        });
                    }
                });
            } else {
                SweetAlert.alert($translate.instant('Admin.Js.Colors.ColorsForProductsCantDeleted'), {
                    title: $translate.instant('Admin.Js.Colors.DeletingImpossible'),
                });
            }
        };

        ctrl.startUpdateUnSetColors = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Colors.Updating'), {
                title: $translate.instant('Admin.Js.Colors.AreYouSureUpdate'),
            }).then(function (result) {
                if (result === true || result.value) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Colors.StartUpdateProcess'));
                    $http.post('colors/updateUnSetColors').then(function (response) {
                        ctrl.grid.fetchData();
                        toaster.pop('success', '', $translate.instant('Admin.Js.Colors.FinishUpdateProcess'));
                    });
                }
            });
        };

        ctrl.startExportColors = function () {
            $window.location.assign('colors/Export');
        };
    };

    ColorsCtrl.$inject = [
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

    ng.module('colors', ['uiGridCustom', 'urlHelper']).controller('ColorsCtrl', ColorsCtrl);
})(window.angular);
