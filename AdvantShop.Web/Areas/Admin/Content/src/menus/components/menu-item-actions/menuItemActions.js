import menuItemEditLinkTemplate from './templates/menuItemEditLink.html';
import menuItemActionsTemplate from './templates/menuItemActions.html';
(function (ng) {
    'use strict';

    var MenuItemActionsCtrl = function ($http, toaster, SweetAlert, $translate, $element, $scope) {
        var ctrl = this;
        ctrl.$onInit = function () {
            $element.on('$destroy', () => {
                $scope.$destroy();
            });
        };
        ctrl.editMenuItem = function () {
            toaster.pop('success', '', $translate.instant('Admin.Js.Menus.ChangesSuccessfullySaved'));
            ctrl.menuTreeviewCtrl?.treeRefresh();
        };
        ctrl.deleteMenuItem = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Menus.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Menus.Deleting'),
            }).then(function (result) {
                if (result === true || result.value === true) {
                    $http
                        .post('menus/deleteMenuItem', {
                            menuItemId: ctrl.id,
                            menuType: ctrl.type,
                        })
                        .then(function (response) {
                            toaster.pop('success', '', $translate.instant('Admin.Js.Menus.ChangesSuccessfullySaved'));
                            ctrl.menuTreeviewCtrl?.treeRefresh();
                        });
                }
            });
        };
    };
    MenuItemActionsCtrl.$inject = ['$http', 'toaster', 'SweetAlert', '$translate', '$element', '$scope'];
    ng.module('menus')
        .controller('MenuItemActionsCtrl', MenuItemActionsCtrl)
        .component('menuItemActions', {
            require: {
                menuTreeviewCtrl: '?^menuTreeview',
            },
            templateUrl: menuItemActionsTemplate,
            controller: 'MenuItemActionsCtrl',
            bindings: {
                id: '@',
                type: '@',
            },
        })
        .component('menuItemEditLink', {
            require: {
                menuTreeviewCtrl: '?^menuTreeview',
            },
            templateUrl: menuItemEditLinkTemplate,
            controller: 'MenuItemActionsCtrl',
            bindings: {
                name: '@',
                id: '@',
                type: '@',
            },
        });
})(window.angular);
