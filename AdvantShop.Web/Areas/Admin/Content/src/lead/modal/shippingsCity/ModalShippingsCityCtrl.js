import './shippingsCity.html';

(function (ng) {
    'use strict';

    var ModalShippingsCityCtrl = function ($uibModalInstance, toaster, $http, $timeout) {
        var ctrl = this;
        var timerProcessAddress;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.obj;
            ctrl.leadId = params.leadId;

            ctrl.getShippingCity();
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getShippingCity = function () {
            $http.get('leads/getShippingCity', { params: { leadId: ctrl.leadId } }).then(function (response) {
                ctrl.data = response.data;
            });
        };

        ctrl.save = function () {
            ctrl.data.leadId = ctrl.leadId;
            $http.post('leads/saveShippingCity', ctrl.data).then(function (response) {
                var data = response.data;
                $uibModalInstance.close(data != null ? data.obj : null);
            });
        };

        ctrl.processCity = function (zone) {
            if (timerProcessAddress != null) {
                $timeout.cancel(timerProcessAddress);
            }

            return (timerProcessAddress = $timeout(
                function () {
                    if (zone != null) {
                        ctrl.data.Country = zone.Country;
                        ctrl.data.Region = zone.Region;
                        ctrl.data.District = zone.District;
                        ctrl.data.Zip = zone.Zip;
                    }
                    if (zone == null || !zone.Zip) {
                        ctrl.processCustomerContact(zone == null).then(function (data) {
                            if (data.result === true) {
                                ctrl.data.Country = data.obj.Country;
                                ctrl.data.Region = data.obj.Region;
                                ctrl.data.District = data.obj.District;
                                ctrl.data.Zip = data.obj.Zip;
                            }
                        });
                    }
                },
                zone != null ? 0 : 700,
            ));
        };

        ctrl.processAddress = function (data) {
            if (timerProcessAddress != null) {
                $timeout.cancel(timerProcessAddress);
            }

            return (timerProcessAddress = $timeout(
                function () {
                    if (data != null && data.Zip) {
                        ctrl.data.Zip = data.Zip;
                    } else {
                        ctrl.processCustomerContact().then(function (data) {
                            if (data.result === true) {
                                ctrl.data.Zip = data.obj.Zip;
                            }
                        });
                    }
                },
                data != null ? 0 : 700,
            ));
        };

        ctrl.processCustomerContact = function (byCity) {
            var contact = {
                country: ctrl.data.Country,
                region: ctrl.data.Region,
                district: ctrl.data.District,
                city: ctrl.data.City,
                zip: ctrl.data.Zip,
                street: ctrl.data.Street,
                house: ctrl.data.House,
                byCity: byCity,
            };
            return $http.post('customers/processCustomerContact', contact).then(function (response) {
                return response.data;
            });
        };
    };

    ModalShippingsCityCtrl.$inject = ['$uibModalInstance', 'toaster', '$http', '$timeout'];

    ng.module('uiModal').controller('ModalShippingsCityCtrl', ModalShippingsCityCtrl);
})(window.angular);
