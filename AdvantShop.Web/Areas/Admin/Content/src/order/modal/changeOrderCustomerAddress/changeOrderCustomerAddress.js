(function (ng) {
    'use strict';

    /* @ngInject */
    var ModalChangeOrderCustomerAddressCtrl = function ($uibModalInstance, $http, $timeout, $translate, toaster, $window, $q, $scope) {
        var ctrl = this,
            timerProcessAddress,
            timerProcessCompanyName;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.orderId = params.orderId;
        };

        ctrl.initModal = function (orderId, isEditMode, isDraft, customerId, standardPhone) {
            ctrl.orderId = orderId;
            ctrl.isEditMode = isEditMode;
            ctrl.isDraft = isDraft;
            ctrl.customerId = customerId;
            ctrl.standardPhone = standardPhone;
        };

        ctrl.getMapAddress = function () {
            var address = ctrl.country != null ? ctrl.country : '';
            address += (address.length > 0 ? ', ' : '') + (ctrl.region != null ? ctrl.region : '');
            address += (address.length > 0 ? ', ' : '') + (ctrl.district != null ? ctrl.district : '');
            address += (address.length > 0 ? ', ' : '') + (ctrl.city != null ? ctrl.city : '');
            if (ctrl.address != null && ctrl.address !== '') {
                address += (address.length > 0 ? ', ' : '') + (ctrl.address != null ? ctrl.address : '');
            } else {
                address += (address.length > 0 ? ', ' : '') + (ctrl.street != null ? ctrl.street : '');
                address += (address.length > 0 ? ', ' : '') + (ctrl.house != null ? ctrl.house : '');
                address += (address.length > 0 ? ', ' : '') + (ctrl.structure != null ? ctrl.structure : '');
            }

            return encodeURIComponent(address);
        };

        ctrl.save = function () {
            var params = {
                orderId: ctrl.orderId,
                orderCustomer: {
                    customerId: ctrl.customerId,
                    country: ctrl.country,
                    region: ctrl.region,
                    district: ctrl.district,
                    city: ctrl.city,
                    zip: ctrl.zip,
                    address: ctrl.address,
                    street: ctrl.street,
                    house: ctrl.house,
                    apartment: ctrl.apartment,
                    structure: ctrl.structure,
                    entrance: ctrl.entrance,
                    floor: ctrl.floor,
                },
            };

            return $http.post('orders/SaveCustomerAddress', params).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                    $uibModalInstance.close();
                } else {
                    ctrl.btnLoading = false;
                    data.errors.forEach(function (error) {
                        toaster.pop('error', '', error);
                    });
                }

                return data;
            });
        };

        ctrl.processCity = function (zone) {
            if (timerProcessAddress != null) {
                $timeout.cancel(timerProcessAddress);
            }

            return (timerProcessAddress = $timeout(
                function () {
                    if (zone != null) {
                        ctrl.country = zone.Country;
                        ctrl.region = zone.Region;
                        ctrl.district = zone.District;
                        ctrl.zip = zone.Zip;
                    }
                    if (zone == null || !zone.Zip) {
                        ctrl.processCustomerContact(zone == null).then(function (data) {
                            if (data.result === true) {
                                ctrl.country = data.obj.Country;
                                ctrl.region = data.obj.Region;
                                ctrl.district = data.obj.District;
                                ctrl.zip = data.obj.Zip;
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
                        ctrl.zip = data.Zip;
                    } else {
                        ctrl.processCustomerContact().then(function (data) {
                            if (data.result === true) {
                                ctrl.zip = data.obj.Zip;
                            }
                        });
                    }
                },
                data != null ? 0 : 700,
            ));
        };

        ctrl.processCustomerContact = function (byCity) {
            var contact = {
                country: ctrl.country,
                region: ctrl.region,
                district: ctrl.district,
                city: ctrl.city,
                zip: ctrl.zip,
                street: ctrl.street,
                house: ctrl.house,
                byCity: byCity,
            };
            return $http.post('customers/processCustomerContact', contact).then(function (response) {
                return response.data;
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.changeAddress = function (address) {
            ctrl.country = address.Country;
            ctrl.region = address.Region;
            ctrl.district = address.District;
            ctrl.city = address.City;
            ctrl.zip = address.Zip;
            ctrl.street = address.Street;
            ctrl.entrance = address.Entrance;
            ctrl.floor = address.Floor;
            ctrl.house = address.House;
            ctrl.structure = address.Structure;
            ctrl.apartment = address.Apartment;
        };
    };

    ng.module('uiModal').controller('ModalChangeOrderCustomerAddressCtrl', ModalChangeOrderCustomerAddressCtrl);
})(window.angular);
