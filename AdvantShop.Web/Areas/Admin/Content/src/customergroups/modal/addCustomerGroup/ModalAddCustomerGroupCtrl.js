(function (ng) {
    'use strict';

    var ModalAddCustomerGroupCtrl = function ($uibModalInstance, $http) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var resolve = ctrl.$resolve;
            ctrl.mode = resolve.params != undefined ? 'edit' : 'add';

            if (ctrl.mode == 'edit') {
                ctrl.name = resolve.params.GroupName;
                ctrl.discount = resolve.params.GroupDiscount;
                ctrl.minimumOrderPrice = resolve.params.MinimumOrderPrice;
                ctrl.groupId = resolve.params.CustomerGroupId;
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.addCustomerGroup = function () {
            if (ctrl.mode == 'add') {
                let params = {
                    groupName: ctrl.name,
                    groupDiscount: ctrl.discount,
                    minimumOrderPrice: ctrl.minimumOrderPrice,
                };

                $http.post('customergroups/addCustomerGroup', params).then(function (response) {
                    $uibModalInstance.close('addCustomerGroup');
                });
            } else {
                let params = {
                    CustomerGroupId: ctrl.groupId,
                    GroupName: ctrl.name,
                    GroupDiscount: ctrl.discount,
                    MinimumOrderPrice: ctrl.minimumOrderPrice,
                };

                $http
                    .post('customergroups/inplace', params)
                    .then(function (response) {
                        ctrl.$resolve.params.GroupName = ctrl.name;
                        ctrl.$resolve.params.GroupDiscount = ctrl.discount;
                        ctrl.$resolve.params.MinimumOrderPrice = ctrl.minimumOrderPrice;
                        $uibModalInstance.close('addCustomerGroup');
                    })
                    .catch(function (err) {
                        console.log(err);
                    });
            }
        };
    };

    ModalAddCustomerGroupCtrl.$inject = ['$uibModalInstance', '$http'];

    ng.module('uiModal').controller('ModalAddCustomerGroupCtrl', ModalAddCustomerGroupCtrl);
})(window.angular);
