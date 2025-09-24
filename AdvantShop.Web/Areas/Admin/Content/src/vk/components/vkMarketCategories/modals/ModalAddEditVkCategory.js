(function (ng) {


    const ModalAddEditVkCategoryCtrl = function ($uibModalInstance, $http, toaster) {
        const ctrl = this;

        ctrl.$onInit = function () {
            const params = ctrl.$resolve;
            ctrl.id = params.id !== null ? params.id : 0;
            ctrl.type = ctrl.id !== 0 ? 'edit' : 'add';
            ctrl.category = {};

            if (ctrl.id !== 0) {
                ctrl.getCategory().then(() => {
                    ctrl.filterMarketCategories(null, ctrl.category.VkCategoryId).then((marketCategories) => {
                        const categories = marketCategories.filter((x) => ctrl.category.VkCategoryId === x.Id);
                        if (categories !== null && categories.length > 0) {
                            ctrl.selectVkCategory(categories[0]);
                        }
                    });
                });
            } else {
                ctrl.category.SortOrder = 0;
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.filterMarketCategories = function (query, categoryId) {
            return $http.get('vkMarket/filterMarketCategories', { params: { q: query, categoryId } }).then((response) => {
                if (response.data.result === true) {
                    ctrl.marketCategories = response.data.obj;
                    return ctrl.marketCategories;
                } else if (response.data.errors !== null) {
                    response.data.errors.forEach((error) => {
                        toaster.pop('error', 'Ошибка при получении категорий ВКонтакте', error);
                    });
                }
            });
        };

        ctrl.selectVkCategory = function (category) {
            ctrl.vkCategoryId = category.Id;
            ctrl.vkCategory = category;
        };

        ctrl.getCategory = function () {
            return $http.get('vkMarket/getCategory', { params: { id: ctrl.id } }).then((response) => {
                const {data} = response;
                if (data !== null) {
                    ctrl.category = data;
                }
                return data;
            });
        };

        ctrl.selectCategories = function (result) {
            ctrl.category.CategoryIds = result.categoryIds;

            if (result.categoryIds !== null && result.categoryIds.length > 0) {
                $http.post('catalog/getCategoryNames', { categoryIds: result.categoryIds }).then((response) => {
                    ctrl.category.Categories = response.data
                        .map((item) => item.name)
                        .join(',<br> ');
                });
            }
        };

        ctrl.find = function (viewValue) {
            if (viewValue === null) return;

            const params = {
                q: viewValue,
            };
            $http.get('vkMarket/filterMarketCategories', { params }).then((response) => {
                if (response.data.result === true) {
                    ctrl.items = response.data.obj;
                } else if (response.data.errors !== null) {
                    response.data.errors.forEach((error) => {
                        toaster.pop('error', 'Ошибка при получении категорий ВКонтакте', error);
                    });
                }
            });
        };

        ctrl.save = function () {
            if (ctrl.category.CategoryIds === null || ctrl.category.CategoryIds.length === 0) {
                toaster.pop('error', '', 'Выберите категории магазина');
                return;
            }

            if (ctrl.vkCategoryId === null) {
                toaster.pop('error', '', 'Выберите категорию ВКонтакте');
                return;
            }

            ctrl.category.VkCategoryId = ctrl.vkCategoryId;

            const url = ctrl.type === 'add' ? 'vkMarket/addCategory' : 'vkMarket/updateCategory';

            $http.post(url, { category: ctrl.category }).then((response) => {
                const {data} = response;
                if (data.result === true) {
                    $uibModalInstance.close(data);
                } else if (data.errors !== null) {
                        data.errors.forEach((error) => {
                            toaster.pop('error', '', error);
                        });
                    } else {
                        toaster.pop('error', '', 'Ошибка при сохранении');
                    }
            });
        };
    };

    ModalAddEditVkCategoryCtrl.$inject = ['$uibModalInstance', '$http', 'toaster'];

    ng.module('uiModal').controller('ModalAddEditVkCategoryCtrl', ModalAddEditVkCategoryCtrl);
})(window.angular);
