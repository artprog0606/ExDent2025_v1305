import salesFunnelsTemplate from './salesFunnels.html';
import addEditSalesFunnelTemplate from './modals/addEditSalesFunnel/addEditSalesFunnel.html';
(function (ng) {
    'use strict';

    var salesFunnelsCtrl = function ($q, $uibModal, uiGridConstants, uiGridCustomConfig, $http, toaster, SweetAlert, $translate) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.SalesFunnels.Name'),
                    enableCellEdit: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                        '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadSalesFunnel(row.entity.Id, $event)">{{COL_FIELD}}</a>' +
                        '</div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SalesFunnels.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.SalesFunnels.SortOrder'),
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SalesFunnels.SortOrder'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'SortOrder',
                    },
                    width: 200,
                },
                {
                    name: 'Enable',
                    displayName: $translate.instant('Admin.Js.SalesFunnels.Activity'),
                    enableCellEdit: false,
                    cellTemplate:
                        '<ui-grid-custom-switch field-name="Enable" row="row" readonly="row.entity.IsDefaultFunnel"></ui-grid-custom-switch>',
                    width: 76,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SalesFunnels.Activity'),
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.SalesFunnels.TheyActive'),
                                value: true,
                            },
                            {
                                label: $translate.instant('Admin.Js.SalesFunnels.Inactive'),
                                value: false,
                            },
                        ],
                    },
                    //cellEditableCondition: function ($scope) {
                    //    return !$scope.row.entity.IsDefaultFunnel || !$scope.row.entity.Enable
                    //}
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 80,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadSalesFunnel(row.entity.Id, $event)" aria-label="Редактировать"></button>' +
                        '<ui-grid-custom-delete url="salesFunnels/deleteSalesFunnel" params="{\'id\': row.entity.Id}"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="salesFunnels/deleteSalesFunnel" params="{\'id\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.SalesFunnels.DeleteSelected'),
                        url: 'salesFunnels/deleteSalesFunnels',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.Deleting'),
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
        ctrl.$onInit = function () {
            ctrl.getFormData().then(ctrl.fetch);
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    salesFunnels: ctrl,
                });
            }
        };
        ctrl.fetch = function () {
            if (ctrl.grid != null) {
                ctrl.grid.fetchData();
            }
        };
        ctrl.getFormData = function () {
            return $http.get('salesFunnels/getFormData').then(function (response) {
                ctrl.canAddSalesFunnel = response.data.canAddSalesFunnel;
            });
        };
        ctrl.onAddEdit = function () {
            if (ctrl.grid != null) {
                ctrl.grid.fetchData();
            }
            if (ctrl.onChange != null) {
                ctrl.onChange();
            }
        };
        ctrl.loadSalesFunnel = function (id, $event) {
            if ($event) {
                $event.preventDefault();
            }
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddEditSalesFunnelCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: addEditSalesFunnelTemplate,
                    resolve: {
                        Id: function () {
                            return id;
                        },
                    },
                    size: 'middle',
                })
                .result.then(function (result) {
                    ctrl.fetch();
                    return result;
                })
                .catch(function (reject) {
                    console.log(reject);
                });
        };
    };
    salesFunnelsCtrl.$inject = ['$q', '$uibModal', 'uiGridConstants', 'uiGridCustomConfig', '$http', 'toaster', 'SweetAlert', '$translate'];
    ng.module('salesFunnels', ['as.sortable'])
        .controller('salesFunnelsCtrl', salesFunnelsCtrl)
        .component('salesFunnels', {
            templateUrl: salesFunnelsTemplate,
            controller: 'salesFunnelsCtrl',
            controllerAs: 'ctrl',
            transclude: true,
            bindings: {
                onInit: '&',
                onChange: '&',
            },
        });
})(window.angular);
