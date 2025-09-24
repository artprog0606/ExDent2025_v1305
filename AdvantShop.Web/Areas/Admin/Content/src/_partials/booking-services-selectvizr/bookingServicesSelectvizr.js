(function (ng) {
    'use strict';

    var BookingServicesSelectvizrCtrl = function () {
        var ctrl = this;

        ctrl.ids = [];

        ctrl.$onInit = function () {
            ctrl.selectvizrGridParams = ctrl.selectvizrGridParams || {};
            ctrl.selectvizrGridParams.categoryFilterId = ctrl.selectvizrGridParams.categoryFilterId || null;
        };

        ctrl.treeCallbacks = {
            select_node: function (event, data) {
                if (data.node.children === true || data.node.children.length > 0) {
                    data.instance.open_node(data.node);
                }

                ctrl.selectvizrGridParams.categoryFilterId = data.node.id;

                ctrl.selectvizrGridParams.Page = 1;

                ctrl.grid.setParams(ctrl.selectvizrGridParams);

                ctrl.grid.fetchData();

                if (ctrl.selectvizrOnChange != null) {
                    ctrl.selectvizrOnChange({
                        categoryId: ctrl.selectvizrGridParams.categoryFilterId,
                        ids: ctrl.selectionCustom.getSelectedParams('Id').ids,
                    });
                }
            },
        };

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.gridSelectionOnInit = function (selectionCustom) {
            ctrl.selectionCustom = selectionCustom;

            ctrl.selectvizrOnChange(
                ng.extend(
                    {
                        categoryId: ctrl.selectvizrGridParams.categoryFilterId,
                    },
                    ctrl.selectionCustom.getSelectedParams('Id'),
                ),
            );
        };

        ctrl.gridSelectionOnChange = function () {
            if (ctrl.selectvizrOnChange != null && ctrl.selectionCustom != null) {
                ctrl.selectvizrOnChange(
                    ng.extend(
                        {
                            categoryId: ctrl.selectvizrGridParams.categoryFilterId,
                        },
                        ctrl.selectionCustom.getSelectedParams('Id'),
                    ),
                );
            }
        };

        ctrl.gridItemsSelectedFilterFn = function (rowEntity) {
            var result = false;

            if (!rowEntity.PhotoSrc) {
                rowEntity.PhotoSrc = '../images/nophoto_small.png';
            }

            if (ctrl.selectvizrGridItemsSelected != null && ctrl.selectvizrGridItemsSelected.length > 0) {
                for (var i = 0, len = ctrl.selectvizrGridItemsSelected.length; i < len; i++) {
                    if (rowEntity.Id === ctrl.selectvizrGridItemsSelected[i]) {
                        ctrl.selectvizrGridItemsSelected.splice(i, 1);
                        result = true;
                        break;
                    }
                }
            }

            return result;
        };
    };

    ng.module('bookingServicesSelectvizr', ['uiGridCustom', 'ui.grid']).controller('BookingServicesSelectvizrCtrl', BookingServicesSelectvizrCtrl);
})(window.angular);
