import addEditCustomerGroupCategoryDiscountTemplate from './modal/addEditCustomerGroupCategoryDiscount/addEditCustomerGroupCategoryDiscount.html';

(function (ng) {
    'use strict';

    var CustomerGroupsCtrl = function ($window, uiGridConstants, uiGridCustomConfig, $q, SweetAlert, $http, $translate) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'GroupName',
                    displayName: $translate.instant('Admin.Js.Customergroups.NameOfGroup'),
                    enableCellEdit: true,
                },
                {
                    name: 'GroupDiscount',
                    displayName: $translate.instant('Admin.Js.CustomerGroups.BaseDiscount'),
                    enableCellEdit: true,
                },
                {
                    name: 'MinimumOrderPrice',
                    displayName: $translate.instant('Admin.Js.Customergroups.MinimumOrderAmount'),
                    enableCellEdit: true,
                },
                {
                    name: '_categoryDiscountColumn',
                    displayName: '',
                    width: 150,
                    enableSorting: false,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<ui-modal-trigger data-controller="\'ModalAddEditCustomerGroupCategoryDiscountCtrl\'" controller-as="ctrl" size="middle" ' +
                        'template-url="' +
                        addEditCustomerGroupCategoryDiscountTemplate +
                        '" data-resolve="{\'CustomerGroupId\': row.entity.CustomerGroupId}" data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                        '<a href="" class="">' +
                        $translate.instant('Admin.Js.CustomerGroups.SetDiscountForCategory') +
                        '</a> ' +
                        '</ui-modal-trigger>',
                },
                {
                    name: 'CustomersCount',
                    displayName: $translate.instant('Admin.Js.Customergroups.UsedForCustomers'),
                    width: 110,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"> <a target="_blank" href="customers?gridCustomers={%22GroupId%22:%22{{row.entity.CustomerGroupId}}%22}">{{ COL_FIELD }}</a></div>',
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 55,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        ' <a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.delete(row.entity.CanBeDeleted, row.entity.CustomerGroupId)" ng-class="(!row.entity.CanBeDeleted ? \'ui-grid-custom-service-icon fa fa-times link-disabled\' : \'ui-grid-custom-service-icon fa fa-times link-invert\')"></a> ' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="customergroups/deletecustomergroup" params="{\'CustomerGroupId\': row.entity.CustomerGroupId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Customergroups.DeleteSelected'),
                        url: 'customergroups/deletecustomergroups',
                        field: 'CustomerGroupId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Customergroups.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.Customergroups.Deleting'),
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

        ctrl.delete = function (canBeDeleted, groupId) {
            if (canBeDeleted) {
                SweetAlert.confirm($translate.instant('Admin.Js.Customergroups.AreYouSureDelete'), {
                    title: $translate.instant('Admin.Js.Customergroups.Deleting'),
                }).then(function (result) {
                    if (result === true || result.value) {
                        $http.post('customergroups/deletecustomergroup', { CustomerGroupId: groupId }).then(function (response) {
                            ctrl.grid.fetchData();
                        });
                    }
                });
            } else {
                SweetAlert.alert($translate.instant('Admin.Js.Customergroups.CanNotDeleteDefaultGroup'), {
                    title: $translate.instant('Admin.Js.Customergroups.DeletingImpossible'),
                });
            }
        };
    };

    CustomerGroupsCtrl.$inject = ['$window', 'uiGridConstants', 'uiGridCustomConfig', '$q', 'SweetAlert', '$http', '$translate'];

    ng.module('customergroups', ['uiGridCustom', 'urlHelper']).controller('CustomerGroupsCtrl', CustomerGroupsCtrl);
})(window.angular);
