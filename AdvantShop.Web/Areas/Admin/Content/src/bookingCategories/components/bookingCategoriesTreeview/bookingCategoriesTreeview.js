(function (ng) {
    'use strict';

    var BookingCategoriesTreeviewCtrl = function ($window, bookingCategoriesService, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.url =
                'bookingCategory/getCategoriesTree?affiliateId=' +
                (ctrl.affiliateId || '') +
                '&categoryIdSelected=' +
                (ctrl.categoryIdSelected || '0');
        };

        ctrl.onInitTree = function (jstree) {
            ctrl.jstree = jstree;

            if (ctrl.onInit != null) {
                ctrl.onInit({ jstree: jstree });
            }
        };

        ctrl.treeCallbacks = {
            select_node: function (event, data) {
                if (data.event) {
                    $window.location.assign('bookingcategory/view/' + data.node.id);
                }
            },
            check_callback: function (operation, node, node_parent, node_position, more) {
                if (operation === 'move_node') {
                    return node_parent.parent == null ? true : false;
                }
                return true;
            },
            move_node: function (event, data) {
                var tree = ctrl.jstree,
                    nodeId,
                    prev,
                    next,
                    prevId = null,
                    nextId = null;

                nodeId = data.node.id;
                next = tree.get_next_dom(data.node, true);
                prev = tree.get_prev_dom(data.node, true);

                if (next != null) {
                    nextId = tree.get_node(next).id;
                }

                if (prev != null) {
                    prevId = tree.get_node(prev).id;
                }

                bookingCategoriesService.changeCategorySorting(nodeId, prevId, nextId).then(function (data) {
                    if (data.result === true) {
                        toaster.pop('success', '', $translate('Admin.Js.BookingCategories.ChangesSaved'));
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                        tree.refresh();
                    }
                });
            },
        };
    };

    BookingCategoriesTreeviewCtrl.$inject = ['$window', 'bookingCategoriesService', 'toaster', '$translate'];

    ng.module('bookingCategoriesTreeview', []).controller('BookingCategoriesTreeviewCtrl', BookingCategoriesTreeviewCtrl);
})(window.angular);
