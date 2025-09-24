import productsSelectvizrModalTemplate from '../../../_shared/modal/products-selectvizr/productsSelectvizrModal.html';
import addCategoryTemplate from '../../../_shared/modal/addCategory/AddCategory.html';
import addBrandTemplate from '../../../_shared/modal/addBrand/AddBrand.html';
import selectStaticPageTemplate from '../../../_shared/modal/selectStaticPage/selectStaticPage.html';
import selectNewsTemplate from '../../../_shared/modal/selectNews/selectNews.html';
import './addEditMenuItem.html';

(function (ng) {
    'use strict';

    var ModalAddEditMenuItemCtrl = function ($uibModalInstance, $http, $filter, $uibModal, Upload, SweetAlert, toaster, $window, $q, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.id = params != null && params.id != null ? params.id : 0;
            ctrl.menuType = params != null && params.menuType != null ? params.menuType : 0;
            ctrl.type = ctrl.id != 0 ? 'edit' : 'add';

            ctrl.showModes = [
                {
                    label: $translate.instant('Admin.Js.Menus.Everyone'),
                    value: 0,
                },
                {
                    label: $translate.instant('Admin.Js.Menus.Registered'),
                    value: 1,
                },
                {
                    label: $translate.instant('Admin.Js.Menus.NotRegistered'),
                    value: 2,
                },
            ];

            if (ctrl.type === 'edit') {
                ctrl.getMenuItem();
            } else {
                ctrl.menuItem = {};
                ctrl.menuItem.MenuItemUrlType = ctrl.menuType == 'Admin' ? '5' : '-1'; // url для меня админки, в других случаях - новая статическая страница
                ctrl.menuItem.ShowMode = ctrl.showModes[0];
                ctrl.menuItem.MenuItemParentId = 0;
                ctrl.menuItem.MenuItemParentName = $translate.instant('Admin.Js.Menus.RootElement');
                ctrl.menuItem.SortOrder = 0;
                ctrl.menuItem.Enabled = true;
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getMenuItem = function () {
            $http.get('menus/getMenuItem', { params: { menuItemId: ctrl.id } }).then(function (response) {
                var data = response.data;
                ctrl.menuItem = data;
                ctrl.menuItem.ShowMode = $filter('filter')(ctrl.showModes, { value: data.ShowMode }, true)[0];
            });
        };

        ctrl.changeUrl = function () {
            var controllerName = '',
                templateUrl = '';

            switch (ctrl.menuItem.MenuItemUrlType.toString()) {
                // product
                case '0':
                    controllerName = 'ModalProductsSelectvizrCtrl';
                    templateUrl = productsSelectvizrModalTemplate;
                    break;

                // category
                case '1':
                    controllerName = 'ModalAddCategoryCtrl';
                    templateUrl = addCategoryTemplate;
                    break;

                // brand
                case '4':
                    controllerName = 'ModalAddBrandCtrl';
                    templateUrl = addBrandTemplate;
                    break;

                // static page
                case '2':
                    controllerName = 'ModalSelectStaticPageCtrl';
                    templateUrl = selectStaticPageTemplate;
                    break;

                // news
                case '3':
                    controllerName = 'ModalSelectNewsCtrl';
                    templateUrl = selectNewsTemplate;
                    break;

                default:
                    return;
            }

            $uibModal
                .open({
                    bindToController: true,
                    controller: controllerName,
                    controllerAs: 'ctrl',
                    templateUrl: templateUrl,
                    size: 'lg',
                    resolve: {
                        multiSelect: false,
                    },
                })
                .result.then(function (result) {
                    if (result.ids != null) {
                        if (ctrl.menuItem.MenuItemUrlType === '0' || ctrl.menuItem.MenuItemUrlType === 0) {
                            result.productId = result.ids;
                        }
                    }

                    $http
                        .get('menus/getLinkUrl', {
                            params: ng.extend({ type: ctrl.menuItem.MenuItemUrlType }, result),
                        })
                        .then(function (response) {
                            var data = response.data;
                            if (data.result === true) {
                                ctrl.menuItem.MenuItemUrlPath = data.url;
                            }
                        });
                });
        };

        ctrl.changeParentItem = function (result) {
            ctrl.menuItem.MenuItemParentId = result.menuItemId;
            ctrl.menuItem.MenuItemParentName = result.menuItemName;
        };

        ctrl.save = function () {
            var newPageId = 0,
                defer = $q.defer(),
                promise = defer.promise;

            if (ctrl.menuItem.MenuItemUrlType === '-1') {
                $http
                    .post('staticPages/addStaticPage', {
                        name: ctrl.newPageName,
                        menuParentId: ctrl.menuItem.MenuItemParentId,
                    })
                    .then(function (response) {
                        var data = response.data;
                        if (data.result === true) {
                            newPageId = data.id;
                            ctrl.menuItem.MenuItemUrlPath = data.url;
                            ctrl.menuItem.MenuItemUrlType = '2';
                        }

                        defer.resolve();
                    });
            } else {
                defer.resolve();
            }

            var params = ctrl.menuItem;
            if (params.MenuType == null) {
                params.MenuType = ctrl.menuType;
            }

            params.ShowMode = ctrl.menuItem.ShowMode.value;

            promise.then(function () {
                $http.post(ctrl.type === 'add' ? 'menus/addMenuItem' : 'menus/updateMenuItem', params).then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        if (newPageId !== 0) {
                            $window.location.assign('staticpages/edit/' + newPageId);
                        } else {
                            if (ctrl.menuType == 'Admin') {
                                $window.location.reload();
                            }
                            $uibModalInstance.close();
                        }
                    }
                });
            });
        };

        ctrl.uploadIcon = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                ctrl.sendIcon($file);
            } else if ($invalidFiles.length > 0) {
                toaster.pop('error', $translate.instant('Admin.Js.Menus.ErrorLoading'), $translate.instant('Admin.Js.Menus.FileDoesNotMeet'));
            }
        };

        ctrl.deleteIcon = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Menus.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Menus.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    return $http
                        .post('menus/deleteIcon', {
                            itemId: ctrl.menuItem.MenuItemId,
                            menuItemIcon: ctrl.menuItem.MenuItemIcon,
                        })
                        .then(function (response) {
                            var data = response.data;
                            if (data.result === true) {
                                ctrl.menuItem.MenuItemIconPath = null;
                                ctrl.menuItem.MenuItemIcon = null;
                                toaster.pop('success', '', $translate.instant('Admin.Js.Menus.ImageDeleted'));
                            } else {
                                toaster.pop('error', $translate.instant('Admin.Js.Menus.ErrorWhileDeleting'), data.error);
                            }
                        });
                }
            });
        };

        ctrl.sendIcon = function (file) {
            var width = 48;
            var height = ctrl.menuType === 'Admin' ? 24 : 30;
            return Upload.upload({
                url: 'menus/uploadIcon',
                data: {
                    file: file,
                    itemId: ctrl.menuItem.MenuItemId,
                    iconWidth: width,
                    iconHeight: height,
                    rnd: Math.random(),
                },
            }).then(function (response) {
                var data = response.data;

                if (data.Result === true) {
                    ctrl.menuItem.MenuItemIconPath = data.Picture;
                    ctrl.menuItem.MenuItemIcon = data.PictureName;
                    toaster.pop('success', '', $translate.instant('Admin.Js.Menus.ImageSaved'));
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.Menus.ErrorLoading'), data.error);
                }
            });
        };
    };

    ModalAddEditMenuItemCtrl.$inject = [
        '$uibModalInstance',
        '$http',
        '$filter',
        '$uibModal',
        'Upload',
        'SweetAlert',
        'toaster',
        '$window',
        '$q',
        '$translate',
    ];

    ng.module('uiModal').controller('ModalAddEditMenuItemCtrl', ModalAddEditMenuItemCtrl);
})(window.angular);
