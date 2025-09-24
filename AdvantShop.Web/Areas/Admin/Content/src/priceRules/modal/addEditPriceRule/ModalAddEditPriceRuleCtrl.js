(function (ng) {
    'use strict';

    var ModalAddEditPriceRuleCtrl = function ($uibModalInstance, $http, $q, $timeout, toaster, Upload, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.id = params.Id != null ? params.Id : 0;
            ctrl.mode = ctrl.id != 0 ? 'edit' : 'add';
            ctrl.selectedCustomerGroups = [];
            ctrl.selectedWarehouses = [];

            ctrl.getInfo().then(function (data) {
                if (ctrl.mode == 'add') {
                    ctrl.priceRule = {
                        Amount: 0,
                        SortOrder: data.sortOrder,
                        Enabled: true,
                    };
                    if (ctrl.customerGroups != null && ctrl.customerGroups.length > 0) {
                        ctrl.selectedCustomerGroups.push(ctrl.customerGroups[0]);
                    }
                    if (ctrl.priceRule.PaymentMethodId == null && ctrl.paymentMethods.length > 0) {
                        ctrl.priceRule.PaymentMethodId = ctrl.paymentMethods[0].value;
                    }
                    if (ctrl.priceRule.ShippingMethodId == null && ctrl.shippingMethods.length > 0) {
                        ctrl.priceRule.ShippingMethodId = ctrl.shippingMethods[0].value;
                    }
                } else {
                    ctrl.getPriceRule(ctrl.id);
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getPriceRule = function (id) {
            return $http.get('priceRules/get', { params: { id: id } }).then(function (response) {
                let data = (ctrl.priceRule = response.data);

                if (ctrl.priceRule.CustomerGroupIds != null && ctrl.priceRule.CustomerGroupIds.length > 0) {
                    ctrl.priceRule.CustomerGroupIds.forEach(function (id) {
                        let item = ctrl.customerGroups.find((x) => x.Id === id);
                        if (item != null) {
                            ctrl.selectedCustomerGroups.push(item);
                        }
                    });
                }

                if (ctrl.priceRule.WarehouseIds != null && ctrl.priceRule.WarehouseIds.length > 0) {
                    ctrl.priceRule.WarehouseIds.forEach(function (id) {
                        let item = ctrl.warehouses.find((x) => x.Id === id);
                        if (item != null) {
                            ctrl.selectedWarehouses.push(item);
                        }
                    });
                }

                return data;
            });
        };

        ctrl.getInfo = function () {
            return $http.get('priceRules/getInfo').then(function (response) {
                ctrl.customerGroups = response.data.customerGroups;
                ctrl.paymentMethods = response.data.paymentMethods;
                ctrl.shippingMethods = response.data.shippingMethods;
                ctrl.warehouses = response.data.warehouses;
                ctrl.warehousesActive = response.data.warehousesActive;
                return response.data;
            });
        };

        ctrl.save = function () {
            let url = ctrl.mode === 'add' ? 'priceRules/add' : 'priceRules/update';

            ctrl.priceRule.CustomerGroupIds = ctrl.selectedCustomerGroups.map((x) => x.Id);
            ctrl.priceRule.WarehouseIds = ctrl.selectedWarehouses.map((x) => x.Id);

            $http.post(url, ctrl.priceRule).then(function (response) {
                let data = response.data;

                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.ChangesSaved'));
                    $uibModalInstance.close();
                } else if (data.errors != null) {
                    data.errors.forEach(function (err) {
                        toaster.pop('error', '', err);
                    });
                }
            });
        };
    };

    ModalAddEditPriceRuleCtrl.$inject = ['$uibModalInstance', '$http', '$q', '$timeout', 'toaster', 'Upload', '$translate'];

    ng.module('uiModal').controller('ModalAddEditPriceRuleCtrl', ModalAddEditPriceRuleCtrl);
})(window.angular);
