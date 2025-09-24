import './orderCustomerChangeAddress.html';
(function (ng) {
    'use strict';

    var ModalChangeOrderAddressCtrl = /* @ngInject */ function ($uibModalInstance, $window, toaster, $q, $http, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.customerId = params.customerId;
            ctrl.contactId = params.contactId;
            /*  ctrl.getItems();*/
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.change = function (address) {
            ctrl.addressSelected = address;
        };

        //ctrl.getItems = function () {
        //    $http.get('Customers/getCustomerContact', { params: { orderItemId: ctrl.orderItemId }}).then(function (response) {
        //        var data = response.data;
        //        if (data.result === true) {
        //            ctrl.codes = data.obj.Codes;
        //            ctrl.name = data.obj.Name;
        //        } else {
        //            data.errors.forEach(function (error) {
        //                toaster.pop('error', '', error);
        //            });
        //        }
        //    });
        //}

        ctrl.save = function () {
            $uibModalInstance.close(ctrl.addressSelected);
        };
    };

    ng.module('uiModal').controller('ModalChangeOrderAddressCtrl', ModalChangeOrderAddressCtrl);
})(window.angular);
