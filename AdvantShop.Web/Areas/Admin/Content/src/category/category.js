(function (ng) {
    'use strict';

    var CategoryCtrl = function ($http, uiGridCustomConfig, toaster, SweetAlert, $translate, isMobileService, $q) {
        var ctrl = this;

        var isMobile = isMobileService.getValue();

        ctrl.$onInit = function () {
            ctrl.showGridPropertyGroups = true;
        };

        ctrl.gridPropertyGroupsOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Category.PropertyGroups'),
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 40,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate: isMobile
                        ? '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="category/deleteGroupFromCategory" params="{\'groupId\': row.entity.PropertyGroupId, \'categoryId\': row.entity.CategoryId }" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>'
                        : '<div class="ui-grid-cell-contents"><div><ui-grid-custom-delete url="category/deleteGroupFromCategory" params="{\'groupId\': row.entity.PropertyGroupId, \'categoryId\': row.entity.CategoryId }"></ui-grid-custom-delete></div></div>',
                },
            ],
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.gridPropertyGroups = grid;
            ctrl.showGridPropertyGroups = grid.gridOptions.data.length > 0;
        };

        ctrl.gridOnFetch = function (grid) {
            ctrl.showGridPropertyGroups = grid.gridOptions.data.length > 0;
        };

        ctrl.changeCategory = function (result) {
            ctrl.ParentCategoryId = result.categoryId;
            ctrl.ParentCategoryName = result.categoryName;
        };

        ctrl.PictureId = 0;
        ctrl.IconId = 0;
        ctrl.MiniPictureId = 0;

        ctrl.updateMiniImage = function (result) {
            ctrl.MiniPictureId = result.pictureId;
        };

        ctrl.updateIconImage = function (result) {
            ctrl.IconId = result.pictureId;
        };

        ctrl.updateImage = function (result) {
            ctrl.PictureId = result.pictureId;
        };

        // load tags
        ctrl.loadTags = function (categoryId, form) {
            $http
                .get('category/getTags', { params: { categoryId: categoryId } })
                .then(function (response) {
                    ctrl.tags = response.data.tags;
                    ctrl.selectedTags = response.data.selectedTags;
                })
                .then(function () {
                    form.$setPristine();
                });
        };

        ctrl.tagTransform = function (newTag) {
            return { value: newTag };
        };

        ctrl.deleteCategory = function (categoryId) {
            SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Deleting'),
            }).then(function (result) {
                if (result.value === true) {
                    $http.post('category/delete', { id: categoryId }).then(function (response) {
                        if (response.data.result === true) {
                            if (response.data.needRedirect) {
                                window.location = 'catalog?categoryid=' + response.data.id;
                            }
                        } else {
                            toaster.pop('error', '', $translate.instant('Admin.Js.Category.ErrorWhileDeleting'), '');
                        }
                    });
                }
            });
        };

        ctrl.loadBrands = function (categoryId, form) {
            $http
                .get('category/getSelectedBrands', { params: { categoryId: categoryId } })
                .then(function (response) {
                    ctrl.selectedBrands = response.data;
                })
                .then(function () {
                    form.$setPristine();
                });
        };

        ctrl.recalculateBrands = function (categoryId) {
            $http.get('category/getCategoryBrands', { params: { categoryId: categoryId } }).then(function (response) {
                ctrl.selectedBrands = response.data;
            });
        };

        ctrl.firstCallBrands = function () {
            if (ctrl.brandsPage) return $q.resolve();
            ctrl.brandsPage = {};
            return ctrl.getBrands();
        };

        ctrl.getBrandsByName = function (q) {
            ctrl.brandsPage = null;
            ctrl.brands = null;
            return ctrl.getBrands(q);
        };

        ctrl.getBrands = function (q) {
            ctrl.brandsPage = ctrl.brandsPage || {};
            var currentPage = ctrl.brandsPage.page || 0;
            var newPage;

            if (ctrl.brandsPage.receivedAllBrands || ctrl.loadingBrands === true) {
                return $q.resolve();
            }
            console.log(ctrl.brandsPage);
            newPage = currentPage + 1;
            ctrl.loadingBrands = true;
            var params = {
                page: newPage,
                count: 100,
                q: q,
            };
            return $http
                .get('category/getBrandList', { params })
                .then(function (response) {
                    var data = response.data;
                    if (data != null) {
                        var brandsCount = ctrl.brands == null ? 0 : ctrl.brands.length;
                        ctrl.brands = ctrl.brands == null ? data : ctrl.brands.concat(data);

                        var totalPageCount = ctrl.brands.length - brandsCount;
                        if (totalPageCount <= 0) ctrl.brandsPage.receivedAllBrands = true;
                        ctrl.brandsPage.page = newPage;
                    }

                    return data;
                })
                .finally(function () {
                    ctrl.loadingBrands = false;
                });
        };

        // region automap categories

        ctrl.addAutomapCategories = function (categoryId, categoryIds) {
            if (categoryIds == null || categoryIds.length == 0) {
                return;
            }

            $http.post('category/addAutomapCategories', { categoryId: categoryId, categoryIds: categoryIds }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.success('', $translate.instant('Admin.Js.Category.AutomapCategoriesAdded'));
                    ctrl.getAutomapCategories(categoryId);
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };

        ctrl.getAutomapCategories = function (categoryId) {
            return $http.get('category/getAutomapCategories', { params: { categoryId: categoryId } }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    ctrl.automapCategories = data.obj.automapCategories || [];
                    ctrl.automapAction = data.obj.automapAction;
                    ctrl.mainAutomapCategory = data.obj.mainAutomapCategory;
                    ctrl.automapCategoriesIds = ctrl.automapCategories.map(function (ac) {
                        return ac.CategoryId;
                    });
                }
            });
        };

        ctrl.setMainAutomapCategory = function (categoryId, automapCategoryId, setValue, quiet) {
            if (typeof setValue != 'undefined') {
                ctrl.mainAutomapCategory = setValue;
            }
            $http
                .post('category/setMainAutomapCategory', {
                    categoryId: categoryId,
                    automapCategoryId: automapCategoryId,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        !quiet && toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                    } else {
                        toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                });
        };

        ctrl.setCategoryAutomapAction = function (categoryId, automapAction, prevAutomapAction, setValue, quiet) {
            if (typeof setValue != 'undefined') {
                ctrl.automapAction = setValue;
            }
            $http
                .post('category/setCategoryAutomapAction', {
                    categoryId: categoryId,
                    automapAction: prevAutomapAction != null ? automapAction | prevAutomapAction : automapAction,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        !quiet && toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                    } else {
                        toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                });
        };

        ctrl.deleteAutomapCategory = function (categoryId, automapCategoryId) {
            SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Deleting'),
            }).then(function (result) {
                if (result === true || result.value === true) {
                    $http
                        .post('category/deleteAutomapCategory', {
                            categoryId: categoryId,
                            automapCategoryId: automapCategoryId,
                        })
                        .then(function (response) {
                            var data = response.data;
                            if (data.result === true) {
                                toaster.success('', $translate.instant('Admin.Js.Category.AutomapCategoryDeleted'));
                                ctrl.getAutomapCategories(categoryId).then(function () {
                                    if (ctrl.automapCategories.length == 0) {
                                        ctrl.setCategoryAutomapAction(categoryId, 8, null, 8, true);
                                        ctrl.mainAutomapCategory = null;
                                    }
                                });
                            } else {
                                toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                            }
                        });
                }
            });
        };

        ctrl.deleteAutomapCategories = function (categoryId) {
            SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Deleting'),
            }).then(function (result) {
                if (result === true || result.value === true) {
                    $http.post('category/deleteAutomapCategories', { categoryId: categoryId }).then(function (response) {
                        var data = response.data;
                        if (data.result === true) {
                            toaster.success('', $translate.instant('Admin.Js.Category.AutomapCategoriesDeleted'));
                            ctrl.getAutomapCategories(categoryId).then(function () {
                                if (ctrl.automapCategories.length == 0) {
                                    ctrl.setCategoryAutomapAction(categoryId, 8, null, 8, true);
                                    ctrl.mainAutomapCategory = null;
                                }
                            });
                        } else {
                            toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                        }
                    });
                }
            });
        };

        // endregion
    };

    CategoryCtrl.$inject = ['$http', 'uiGridCustomConfig', 'toaster', 'SweetAlert', '$translate', 'isMobileService', '$q'];

    ng.module('category', ['angular-inview', 'uiGridCustom', 'urlGenerator', 'uiModal']).controller('CategoryCtrl', CategoryCtrl);
})(window.angular);
