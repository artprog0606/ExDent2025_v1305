import mobileModalEditDiscount from './addEditMobileCustomerGroupCategoryDiscount.html';

(function (ng) {
    'use strict';

    var ModalAddEditCustomerGroupCategoryDiscountCtrl = function (
        uiGridCustomConfig,
        uiGridConstants,
        $translate,
        $uibModalInstance,
        $uibModal,
        $http,
        toaster,
    ) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.ModalAddEditCustomerGroupCategoryDiscount.Category'),
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.ModalAddEditCustomerGroupCategoryDiscount.Category'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Category',
                    },
                },
                {
                    name: 'Discount',
                    displayName: $translate.instant('Admin.Js.ModalAddEditCustomerGroupCategoryDiscount.Discount'),
                    enableCellEdit: true,
                    uiGridCustomEdit: {
                        replaceNullable: false,
                    },
                    filter: {
                        placeholder: $translate.instant('Admin.Js.ModalAddEditCustomerGroupCategoryDiscount.Discount'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Discount',
                    },
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
        });

        ctrl.$onInit = function () {
            let params = ctrl.$resolve;
            ctrl.CustomerGroupId = params.CustomerGroupId;
        };

        ctrl.gridOnInit = function (grid) {
            grid.setParams({
                CustomerGroupId: ctrl.CustomerGroupId,
            });
            ctrl.grid = grid;
            ctrl.grid.fetchData(true);
        };

        ctrl.selectCategory = function (event, data) {
            ctrl.selectedCategory = data.node.id;
            ctrl.grid.setParams({
                CategoryId: data.node.id,
            });
            ctrl.grid.fetchData(true);
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.openMobileModal = function (entity) {
            let objParams = {
                CategoryId: entity.CategoryId,
                SizeId: entity.SizeId,
                SizeName: entity.SizeName,
                SizeNameForCategory: entity.SizeNameForCategory,
            };

            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddEditCustomerGroupCategoryDiscountCtrl',
                    controllerAs: 'ctrl',
                    backdrop: 'static',
                    templateUrl: mobileModalEditDiscount,
                    resolve: {
                        params: entity,
                    },
                })
                .result.then(function (result) {
                    ctrl.grid.fetchData(true);
                });
        };

        ctrl.saveDiscount = function (params) {
            $http.post('customerGroups/inplaceCustomerGroupCategoryDiscount', params).then(function (response) {
                let data = response.data;
                if (data.result) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.ChangesSaved'));
                    $uibModalInstance.close();
                    ctrl.grid.fetchData(true);
                } else {
                    data.errors.forEach(function (err) {
                        toaster.pop('error', '', err);
                    });
                }
            });
        };
    };

    ModalAddEditCustomerGroupCategoryDiscountCtrl.$inject = [
        'uiGridCustomConfig',
        'uiGridConstants',
        '$translate',
        '$uibModalInstance',
        '$uibModal',
        '$http',
        'toaster',
    ];

    ng.module('uiModal').controller('ModalAddEditCustomerGroupCategoryDiscountCtrl', ModalAddEditCustomerGroupCategoryDiscountCtrl);
})(window.angular);
