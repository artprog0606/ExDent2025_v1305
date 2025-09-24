(function (ng) {
    'use strict';

    var ModalDiscountByDatetimeCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var id = ctrl.$resolve.id;
            ctrl.discountByTimeId = id;
            ctrl.getFormData();
            if (id) ctrl.getData(id);
            else ctrl.settings = { SortOrder: 0 };
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getFormData = function () {
            $http.get('discountsByTime/getFormData').then(function (response) {
                var data = response.data;
                if (data.result) {
                    ctrl.formData = data.obj;
                }
            });
        };

        ctrl.getData = function (id) {
            $http.get('discountsByTime/get', { params: { id: id } }).then(function (response) {
                var data = response.data;
                if (data.result) {
                    ctrl.settings = data.obj;
                    ctrl.settings.SelectedDays.sort(function (a, b) {
                        return b == '0' ? -1 : 0;
                    });
                }
            });
        };

        ctrl.save = function () {
            $http.post('discountsByTime/addEdit', ctrl.settings).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.PriceRange.ChangesSuccessfullySaved'));
                    $uibModalInstance.close();
                } else if (data.errors) {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });
                } else {
                    toaster.pop('error', 'Не сохранить настройки');
                }
            });
        };

        ctrl.selectCategories = function (result) {
            ctrl.settings.DiscountCategories = result.categoryIds;
        };

        ctrl.selectActiveByTimeCategories = function (result) {
            ctrl.settings.ActiveByTimeCategories = result.categoryIds;
        };

        ctrl.resetDiscountCategories = function () {
            ctrl.settings.DiscountCategories = [];
        };

        ctrl.resetActiveByTimeCategories = function () {
            ctrl.settings.ActiveByTimeCategories = [];
        };
    };

    ModalDiscountByDatetimeCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalDiscountByDatetimeCtrl', ModalDiscountByDatetimeCtrl);
})(window.angular);
