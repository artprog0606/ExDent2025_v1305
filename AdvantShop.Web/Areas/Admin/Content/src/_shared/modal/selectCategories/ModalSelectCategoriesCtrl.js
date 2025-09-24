import './selectCategories.html';

(function (ng) {
    'use strict';

    var ModalSelectCategoriesCtrl = function ($uibModalInstance, $http) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.categoriesSelected = [];
            ctrl.btnChangeDisabled = true;
            ctrl.treeCheckbox = ctrl.$resolve != null && ctrl.$resolve.treeCheckbox != null ? ctrl.$resolve.treeCheckbox : { three_state: false };
            ctrl.treeAjax = ctrl.$resolve != null && ctrl.$resolve.treeAjax != null ? ctrl.$resolve.treeAjax.url : 'catalog/categoriestree';
            ctrl.treePlugins = ctrl.$resolve != null && ctrl.$resolve.treePlugins != null ? ctrl.$resolve.treePlugins.items : 'checkbox,search';
            ctrl.notUnboxingCategory = ctrl.$resolve != null && ctrl.$resolve.notUnboxingCategory != null ? ctrl.$resolve.notUnboxingCategory : false;
            ctrl.treeShowRoot = ctrl.$resolve != null && ctrl.$resolve.treeShowRoot != null ? ctrl.$resolve.treeShowRoot : false;

            var params = ctrl.$resolve.params;

            if (params != null) {
                ctrl.treeAjaxData = {
                    selectedIds: params.selectedIds != null ? params.selectedIds.join(',') : undefined,
                    excludeIds: params.excludeIds != null ? params.excludeIds.join(',') : undefined,
                };
            }
        };

        ctrl.choose = function () {
            if (ctrl.categoriesSelected.length > 0) {
                if (ctrl.notUnboxingCategory !== true) {
                    $http.post('catalog/getSelectedCategoriesTree', { categoriesSelected: ctrl.categoriesSelected }).then(function (response) {
                        $uibModalInstance.close({ categoryIds: response.data });
                    });
                } else {
                    $uibModalInstance.close({ categoryIds: ctrl.categoriesSelected });
                }
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.treeCallbacks = {
            select_node: function (event) {
                var tree = ng.element(event.target).jstree(true);
                var selectedNodes = tree.get_selected(true);
                ctrl.categoriesSelected = selectedNodes.map(function (item) {
                    return { categoryId: item.id, name: item.original.name, opened: item.state.opened };
                });

                ctrl.btnChangeDisabled = false;
            },

            deselect_node: function (event) {
                var tree = ng.element(event.target).jstree(true);
                var selectedNodes = tree.get_selected(true);

                ctrl.categoriesSelected = selectedNodes.map(function (item) {
                    return { categoryId: item.id, name: item.original.name, opened: item.state.opened };
                });

                ctrl.btnChangeDisabled = false;
            },
        };
    };

    ModalSelectCategoriesCtrl.$inject = ['$uibModalInstance', '$http'];

    ng.module('uiModal').controller('ModalSelectCategoriesCtrl', ModalSelectCategoriesCtrl);
})(window.angular);
