(function (ng) {
    'use strict';

    var MenuTreeviewCtrl = function ($window, $http, toaster, $translate, isMobileService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.url = 'menus/menustree?showActions=true&menutype=' + ctrl.type + '&selectedId=' + (ctrl.selectedId || '0');
            ctrl.treeCore = {
                check_callback: function (operation, node, node_parent, node_position, more) {
                    var result = true;
                    var resultMobile = true;
                    if (operation === 'move_node') {
                        result = node_parent.parents.length <= ctrl.level - 1;
                    }

                    if (isMobileService.getValue() && event.touches != null && event.touches.length > 0) {
                        let elem = document.elementFromPoint(event.touches[0].clientX, event.touches[0].clientY);
                        resultMobile = elem.closest('.navigation-item__icon') != null;
                    }

                    return result && resultMobile;
                },
            };
        };

        ctrl.dndOptions = {
            is_draggable: function (nodes, $event) {
                const event = $event;
                if (isMobileService.getValue() && event.touches != null && event.touches.length > 0) {
                    const elem = document.elementFromPoint(event.touches[0].clientX, event.touches[0].clientY);
                    return elem.closest('.navigation-item__icon') != null;
                }

                return true;
            },
        };

        ctrl.treeOnInit = function (jstree) {
            ctrl.jstree = jstree;
            ctrl.menuTreeviewOnInit({ jstree: jstree });
        };

        ctrl.treeRefresh = function () {
            ctrl.jstree.refresh();
        };

        ctrl.treeCallbacks = {
            move_node: function (event, data) {
                var tree, nodeId, parentId, prev, next, prevId, nextId;

                nodeId = data.node.id;
                parentId = data.parent !== '#' ? data.parent : '0';
                tree = ng.element(event.target).jstree(true);
                next = tree.get_next_dom(data.node, true);
                prev = tree.get_prev_dom(data.node, true);

                if (prev != null) {
                    prevId = tree.get_node(prev).id;
                }

                if (next != null) {
                    nextId = tree.get_node(next).id;
                }

                $http
                    .post('menus/changeMenuSortOrder', {
                        itemId: nodeId,
                        prevItemId: prevId,
                        nextItemId: nextId,
                        parentItemId: parentId,
                    })
                    .then(function (response) {
                        if (response.data) {
                            toaster.pop('success', '', $translate.instant('Admin.Js.Menus.ChangesSaved'));
                        }
                    });
            },
        };
    };

    MenuTreeviewCtrl.$inject = ['$window', '$http', 'toaster', '$translate', 'isMobileService'];

    ng.module('menus').controller('MenuTreeviewCtrl', MenuTreeviewCtrl);
})(window.angular);
