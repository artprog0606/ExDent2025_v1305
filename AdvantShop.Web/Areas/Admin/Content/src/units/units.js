import addEditUnitTemplate from './modal/addEditUnit/addEditUnit.html';

(function (ng) {
    'use strict';

    var UnitsCtrl = function (uiGridConstants, uiGridCustomConfig, SweetAlert, $http, $q, $translate) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Units.Name'),
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Units.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.Units.SortOrder'),
                    width: 100,
                    enableCellEdit: true,
                },
                {
                    name: 'ProductsCount',
                    displayName: $translate.instant('Admin.Js.Units.UsedForProducts'),
                    width: 90,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"> <a ng-href="catalog?showMethod=AllProducts&grid=%7B%22UnitId%22:{{row.entity.Id}}%7D">{{ COL_FIELD }}</a></div>',
                },
                {
                    name: '_noopColumnWithOutMeasureType',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Units.WithOutMeasureType'),
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            { label: $translate.instant('Admin.Js.Units.Yes'), value: true },
                            { label: $translate.instant('Admin.Js.Units.No'), value: false },
                        ],
                        name: 'WithOutMeasureType',
                    },
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<ui-modal-trigger data-controller="\'ModalAddEditUnitCtrl\'" controller-as="ctrl" ' +
                        'template-url="' +
                        addEditUnitTemplate +
                        '" ' +
                        'data-resolve="{\'unitId\': row.entity.Id}" ' +
                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                        '<button type="button" aria-label="Редактировать" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt"></button> ' +
                        '</ui-modal-trigger>' +
                        ' <button type="button" class="btn-icon" ng-click="grid.appScope.$ctrl.gridExtendCtrl.delete(row.entity.CanBeDeleted, row.entity.Id)" ng-class="(!row.entity.CanBeDeleted ? \'ui-grid-custom-service-icon fa fa-times link-disabled\' : \'ui-grid-custom-service-icon fa fa-times link-invert\')" aria-label="Удалить"></button> ' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="units/deleteUnit" params="{\'unitId\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Units.DeleteSelected'),
                        url: 'units/deleteUnits',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Units.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.Units.Deleting'),
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

        ctrl.delete = function (canBeDeleted, unitId) {
            if (canBeDeleted) {
                SweetAlert.confirm($translate.instant('Admin.Js.Units.AreYouSureDelete'), {
                    title: $translate.instant('Admin.Js.Units.Deleting'),
                }).then(function (result) {
                    if (result === true || result.value) {
                        $http.post('units/deleteUnit', { unitId: unitId }).then(function (response) {
                            ctrl.grid.fetchData();
                        });
                    }
                });
            } else {
                SweetAlert.alert($translate.instant('Admin.Js.Units.UnitsCanNotBeDeleted'), {
                    title: $translate.instant('Admin.Js.Units.DeletingIsImpossible'),
                });
            }
        };
    };

    UnitsCtrl.$inject = ['uiGridConstants', 'uiGridCustomConfig', 'SweetAlert', '$http', '$q', '$translate'];

    ng.module('units', ['uiGridCustom', 'urlHelper']).controller('UnitsCtrl', UnitsCtrl);
})(window.angular);
