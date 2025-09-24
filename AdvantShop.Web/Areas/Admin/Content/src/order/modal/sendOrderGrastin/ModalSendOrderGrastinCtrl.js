(function (ng) {
    'use strict';

    var ModalSendOrderGrastinCtrl = function ($uibModalInstance, $window, toaster, $q, $http, $translate, $filter) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.obj;
            ctrl.orderId = params.orderId;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.send = function () {
            var params = {
                orderId: ctrl.orderId,
                takeWarehouse: ctrl.takeWarehouse,
                buyer: ctrl.buyer,
                phone: ctrl.phone,
                email: ctrl.email,
                comment: ctrl.comment,
            };
            var url = '';

            if (ctrl.typeForm === 'grastin') {
                url = 'grastin/sendorderforgrastin';

                params.phone2 = ctrl.phone2;
                params.service = ctrl.service;
                params.deliveryDate = $filter('ngFlatpickr')(ctrl.deliveryDate, 'Y-m-d', 'd.m.Y');
                params.deiveryTimeFrom = ctrl.deiveryTimeFrom;
                params.deiveryTimeTo = ctrl.deiveryTimeTo;
                params.index = ctrl.index;
                params.addressCourier = ctrl.addressCourier;
                params.addressPoint = ctrl.addressPoint;
                params.officeId = ctrl.officeId;
                params.typeRecipient = ctrl.typeRecipient;
                params.passport = ctrl.passport;
                params.organization = ctrl.organization;
                params.inn = ctrl.inn;
                params.kpp = ctrl.kpp;
                params.seats = ctrl.seats;
                params.assessedCost = ctrl.assessedCost;
                params.cashOnDelivery = ctrl.cashOnDelivery;
            } else if (ctrl.typeForm === 'russianpost') {
                url = 'grastin/sendorderforrussianpost';

                params.service = ctrl.service;
                params.deliveryDate = $filter('ngFlatpickr')(ctrl.deliveryDate, 'Y-m-d', 'd.m.Y');
                params.index = ctrl.index;
                params.region = ctrl.region;
                params.district = ctrl.district;
                params.city = ctrl.city;
                params.address = ctrl.address;
                params.cashOnDelivery = ctrl.cashOnDelivery;
                params.assessedCost = ctrl.assessedCost;
            } else if (ctrl.typeForm === 'boxberry') {
                url = 'grastin/sendorderforboxberry';

                params.phone2 = ctrl.phone2;
                params.service = ctrl.service;
                params.pointId = ctrl.pointId;
                params.postcodeId = ctrl.postcodeId;
                params.address = ctrl.address;
                params.seats = ctrl.seats;
                //params.weight = ctrl.weight;
                params.cashOnDelivery = ctrl.cashOnDelivery;
                params.assessedCost = ctrl.assessedCost;
            } else if (ctrl.typeForm === 'cdek') {
                url = 'grastin/sendorderforcdek';

                params.phone2 = ctrl.phone2;
                params.service = ctrl.service;
                params.pointId = ctrl.pointId;
                params.cityId = ctrl.cityId;
                params.address = ctrl.address;
                params.seats = ctrl.seats;
                //params.weight = ctrl.weight;
                params.cashOnDelivery = ctrl.cashOnDelivery;
                params.assessedCost = ctrl.assessedCost;
            } else if (ctrl.typeForm === 'hermes') {
                url = 'grastin/sendorderforhermes';

                params.pointId = ctrl.pointId;
                params.seats = ctrl.seats;
                params.assessedCost = ctrl.assessedCost;
                //params.weight = ctrl.weight;
                params.cashOnDelivery = ctrl.cashOnDelivery;
            } else if (ctrl.typeForm === 'partner') {
                url = 'grastin/sendorderforpartner';

                params.pointId = ctrl.pointId;
                params.seats = ctrl.seats;
                params.assessedCost = ctrl.assessedCost;
                //params.weight = ctrl.weight;
                params.cashOnDelivery = ctrl.cashOnDelivery;
            }

            if (url) {
                $http.post(url, params).then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.OrderSuccesfullySent'));
                        $uibModalInstance.close();
                    } else {
                        ctrl.btnLoading = false;
                        data.errors.forEach(function (error) {
                            toaster.pop('error', '', error);
                        });
                    }
                });
            }
        };
    };

    ModalSendOrderGrastinCtrl.$inject = ['$uibModalInstance', '$window', 'toaster', '$q', '$http', '$translate', '$filter'];

    ng.module('uiModal').controller('ModalSendOrderGrastinCtrl', ModalSendOrderGrastinCtrl);
})(window.angular);
