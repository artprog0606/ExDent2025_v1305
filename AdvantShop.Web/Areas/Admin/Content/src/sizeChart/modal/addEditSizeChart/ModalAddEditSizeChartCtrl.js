(function (ng) {
    'use strict';

    var ModalAddEditSizeChartCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;
        ctrl.textBySourceType = { 0: '', 1: '' };

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.id = params.Id != null ? params.Id : 0;
            ctrl.mode = ctrl.id != 0 ? 'edit' : 'add';

            if (ctrl.mode == 'edit') {
                ctrl.getSizeChart(ctrl.id);
            } else {
                ctrl.data = {
                    SortOrder: 0,
                    Enabled: true,
                    SourceType: 0,
                    LinkText: $translate.instant('Admin.Js.SizeChart.DefaultLinkText'),
                    ProductIds: [],
                    CategoryIds: [],
                    PropertyValues: [],
                };
                ctrl.selectedBrandsList = [];
            }
        };

        ctrl.init = function (form) {
            ctrl.sizeChartForm = form;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getSizeChart = function (id) {
            return $http.get('sizeChart/get', { params: { id: id } }).then(function (response) {
                var data = response.data;
                ctrl.data = data.obj;
                ctrl.selectedBrandsList = ctrl.data.BrandIds.map((x) => ({ BrandId: x }));
                ctrl.textBySourceType[ctrl.data.SourceType] = ctrl.data.Text;
                ctrl.sizeChartForm.$setPristine();
                return data.obj;
            });
        };

        ctrl.save = function () {
            ctrl.saveLoading = true;
            var url = ctrl.mode == 'add' ? 'sizeChart/add' : 'sizeChart/update';
            ctrl.data.Text = ctrl.textBySourceType[ctrl.data.SourceType];
            ctrl.data.BrandIds = ctrl.selectedBrandsList.map((x) => x.BrandId);
            //ctrl.data.PropertyValueIds = ctrl.selectedBrandsList.map(x => x.BrandId);
            $http.post(url, ctrl.data).then(function (response) {
                var data = response.data;

                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.ChangesSaved'));
                    $uibModalInstance.close();
                } else if (data.errors != null) {
                    data.errors.forEach(function (err) {
                        toaster.pop('error', '', err);
                    });
                }
                ctrl.saveLoading = false;
            });
        };

        ctrl.selectProducts = function (result) {
            ctrl.data.ProductIds = result.ids;
            ctrl.sizeChartForm.modified = true;
        };

        ctrl.resetProducts = function () {
            ctrl.data.ProductIds = [];
            ctrl.sizeChartForm.modified = true;
        };

        ctrl.selectCategories = function (result) {
            ctrl.data.CategoryIds = result.categoryIds;
            ctrl.sizeChartForm.modified = true;
        };

        ctrl.resetCategories = function () {
            ctrl.data.CategoryIds = [];
            ctrl.sizeChartForm.modified = true;
        };

        ctrl.checkSelectedItem = function (rowEntity) {
            return ctrl.data.ProductIds.indexOf(rowEntity.ProductId) != -1;
        };

        ctrl.insertExample = function () {
            ctrl.textBySourceType[0] = $translate.instant('Admin.Js.SizeChart.Example');
        };

        ctrl.changeBrand = function (result) {
            ctrl.selectedBrandsList = result;
            ctrl.sizeChartForm.modified = true;
        };

        ctrl.resetBrands = function () {
            ctrl.selectedBrandsList = [];
            ctrl.sizeChartForm.modified = true;
        };

        ctrl.addProperty = function (result) {
            ctrl.data.PropertyValues = result;
            ctrl.sizeChartForm.modified = true;
        };

        ctrl.resetProperties = function () {
            if (ctrl.data.PropertyValues && ctrl.data.PropertyValues.length > 0) {
                ctrl.data.PropertyValues = null;
                ctrl.sizeChartForm.modified = true;
            }
        };

        ctrl.resetPropertyValues = function () {
            ctrl.data.PropertyValues = [];
            ctrl.sizeChartForm.modified = true;
        };
    };

    ModalAddEditSizeChartCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalAddEditSizeChartCtrl', ModalAddEditSizeChartCtrl);
})(window.angular);
