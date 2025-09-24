(function (ng) {
    'use strict';

    var ModalAddBrandCtrl = function ($uibModalInstance, uiGridCustomConfig, $translate) {
        var ctrl = this;
        var columnDefs = [
            {
                name: 'BrandName',
                displayName: $translate.instant('Admin.Js.AddBrand.ManufactrerName'),
            },
            {
                name: 'ProductsCount',
                displayName: $translate.instant('Admin.Js.AddBrand.QuantityOfProducts'),
                width: 120,
            },
        ];
        var selectedBrandObj = {};

        ctrl.$onInit = function () {
            ctrl.selectedBrandsList = ctrl.$resolve.params != null ? ctrl.$resolve.params.selectedBrandsList : [];

            if (ctrl.selectedBrandsList != null) {
                ctrl.convertSelectedBrandsListToObj(ctrl.selectedBrandsList);
            }

            ctrl.gridSelectionEnabled = ctrl.$resolve.params != null ? ctrl.$resolve.params.gridSelectionEnabled === true : false;

            if (!ctrl.gridSelectionEnabled) {
                columnDefs.push({
                    name: '_serviceColumn',
                    displayName: '',
                    width: 100,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                        '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.choose(row.entity.BrandId, row.entity.BrandName)">' +
                        $translate.instant('Admin.Js.AddCategory.Select') +
                        '</a> ' +
                        '</div></div>',
                });
            }

            ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: columnDefs,
            });

            if (ctrl.$resolve.multiSelect === false) {
                ng.extend(ctrl.gridOptions, {
                    multiSelect: false,
                    modifierKeysToMultiSelect: false,
                    enableRowSelection: true,
                    enableRowHeaderSelection: false,
                });
            }
        };

        ctrl.gridOnInit = function (grid) {
            ctrl.gridBrands = grid;
        };

        ctrl.choose = function (brandId, brandName) {
            $uibModalInstance.close(
                ctrl.gridSelectionEnabled ? [{ BrandId: brandId, BrandName: brandName }] : { brandId: brandId, brandName: brandName },
            );
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.gridSelectionOnInit = function (selectionCustom) {
            ctrl.selectionCustom = selectionCustom;
        };

        ctrl.saveBrands = function () {
            ctrl.selectedBrandsList = ctrl.selectionCustom.getRowsFromStorage();
            ctrl.convertSelectedBrandsListToObj(ctrl.selectedBrandsList);
            $uibModalInstance.close(ctrl.selectedBrandsList);
        };

        ctrl.convertSelectedBrandsListToObj = function (brandList) {
            brandList.forEach(function (it) {
                selectedBrandObj[it.BrandId] = it.BrandId;
            });
        };

        ctrl.getSelectedBrandsItems = function (rowEntity) {
            if (rowEntity != null) {
                return selectedBrandObj[rowEntity.BrandId] != null;
            }
            return false;
        };
    };

    ModalAddBrandCtrl.$inject = ['$uibModalInstance', 'uiGridCustomConfig', '$translate'];

    ng.module('uiModal').controller('ModalAddBrandCtrl', ModalAddBrandCtrl);
})(window.angular);
