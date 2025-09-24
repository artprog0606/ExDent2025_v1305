(function (ng) {
    'use strict';

    var CategorySelectorCtrl = function (blocksConstructorService, $http, $timeout) {
        var ctrl = this,
            categoriesSelected = [];

        ctrl.setSelectedIds = function (categoryArrayObjs) {
            ctrl.selectedIds = categoryArrayObjs
                .map(function (item) {
                    return item.CategoryId;
                })
                .join(',');
            ctrl.showJSTree = true;
        };

        ctrl.selectCategory = function (event, data) {
            var tree = ng.element(event.target).jstree(true);
            categoriesSelected = ctrl.getCategoriesSelected(tree.get_selected(true));
        };

        ctrl.getCategoriesSelected = function (selectedNodes) {
            return selectedNodes.map(function (item) {
                return {
                    CategoryId: item.id,
                    Parents: item.parents,
                    Parent: item.parent,
                    Name: item.original.name,
                    Enabled: item.state.enabled,
                    Opened: item.state.opened,
                };
            });
        };

        ctrl.deselectCategory = function (event, data) {
            var tree = ng.element(event.target).jstree(true);
            categoriesSelected = ctrl.getCategoriesSelected(tree.get_selected(true));
        };

        ctrl.apply = function (modal) {
            return ctrl.getCategoriesSelectedWithChildrens(categoriesSelected).then(function (data) {
                return ng.copy(data.obj);
            });
        };

        ctrl.getCategoriesSelectedWithChildrens = function (categoriesSelected) {
            return $http.post('landinginplace/GetCategories', { categoriesSelected: categoriesSelected }).then(function (response) {
                return response.data;
            });
        };
    };

    ng.module('blocksConstructor').controller('CategorySelectorCtrl', CategorySelectorCtrl);

    CategorySelectorCtrl.$inject = ['blocksConstructorService', '$http', '$timeout'];
})(window.angular);
