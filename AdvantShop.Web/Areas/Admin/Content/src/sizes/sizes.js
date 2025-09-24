import addEditSizeTemplate from './modal/addEditSize/AddEditSize.html';
(function (ng) {
    'use strict';

    var SizesCtrl = function (
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
                    name: 'SizeName',
                    displayName: $translate.instant('Admin.Js.Sizes.Name'),
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Sizes.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'SizeName',
                    },
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.Sizes.SortOrder'),
                    width: 100,
                    enableCellEdit: true,
                },
                {
                    name: 'ProductsCount',
                    displayName: $translate.instant('Admin.Js.Sizes.UsedForProducts'),
                    width: 90,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"> <a ng-href="catalog?showMethod=AllProducts&grid=%7B%22SizeId%22:{{row.entity.SizeId}}%7D">{{ COL_FIELD }}</a></div>',
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<ui-modal-trigger data-controller="\'ModalAddEditSizeCtrl\'" controller-as="ctrl" ' +
                        'template-url="' +
                        addEditSizeTemplate +
                        '" ' +
                        'data-resolve="{\'sizeId\': row.entity.SizeId}" ' +
                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                        '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></button> ' +
                        '</ui-modal-trigger>' +
                        ' <button type="button" class="btn-icon" ng-click="grid.appScope.$ctrl.gridExtendCtrl.delete(row.entity.CanBeDeleted, row.entity.SizeId)" ng-class="(!row.entity.CanBeDeleted ? \'ui-grid-custom-service-icon fa fa-times link-disabled\' : \'ui-grid-custom-service-icon fa fa-times link-invert\')" aria-label="Удалить"></a> ' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="sizes/deleteSize" params="{\'sizeId\': row.entity.SizeId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Sizes.DeleteSelected'),
                        url: 'sizes/deleteSizes',
                        field: 'SizeId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Sizes.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.Sizes.Deleting'),
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

        ctrl.delete = function (canBeDeleted, sizeId) {
            if (canBeDeleted) {
                SweetAlert.confirm($translate.instant('Admin.Js.Sizes.AreYouSureDelete'), {
                    title: $translate.instant('Admin.Js.Sizes.Deleting'),
                }).then(function (result) {
                    if (result === true || result.value) {
                        $http.post('sizes/deleteSize', { sizeId: sizeId }).then(function (response) {
                            ctrl.grid.fetchData();
                        });
                    }
                });
            } else {
                SweetAlert.alert($translate.instant('Admin.Js.Sizes.SizesCanNotBeDeleted'), {
                    title: $translate.instant('Admin.Js.Sizes.DeletingIsImpossible'),
                });
            }
        };
    };

    SizesCtrl.$inject = [
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

    ng.module('sizes', ['uiGridCustom', 'urlHelper']).controller('SizesCtrl', SizesCtrl);
})(window.angular);
