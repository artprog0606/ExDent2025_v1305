(function (ng) {
    'use strict';

    var ModalChangeOrderCustomerCtrl = function ($uibModalInstance, $http, $timeout, $translate, toaster, $window, $q, $scope) {
        var ctrl = this,
            timerProcessAddress,
            timerProcessCompanyName;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.orderId = params.orderId;
        };

        ctrl.initCustomerFields = function (customerFields) {
            return (ctrl.customerFields = !customerFields ? [] : customerFields);
        };

        ctrl.getCustomerFields = function () {
            return ctrl.getFunctionGetCustomerFields().then(function () {
                return ctrl.getCustomerFieldsFn();
            });
        };

        ctrl.getFunctionGetCustomerFields = function () {
            if (ctrl.getCustomerFieldsFn) {
                return $q.resolve();
            } else {
                ctrl.functionGetCustomerFieldsPromise = ctrl.functionGetCustomerFieldsPromise ? ctrl.functionGetCustomerFieldsPromise : $q.defer();
                return ctrl.functionGetCustomerFieldsPromise.promise;
            }
        };

        ctrl.onCustomerFieldsInit = function (reloadFn) {
            ctrl.getCustomerFieldsFn = reloadFn || function () {};
            if (ctrl.functionGetCustomerFieldsPromise) {
                ctrl.functionGetCustomerFieldsPromise.resolve();
            }
        };

        ctrl.initModal = function (orderId, isEditMode, isDraft, customerId, standardPhone) {
            ctrl.orderId = orderId;
            ctrl.isEditMode = isEditMode;
            ctrl.isDraft = isDraft;
            ctrl.customerId = customerId;
            ctrl.standardPhone = standardPhone;
        };

        ctrl.changeStatusClient = function (currentStatus) {
            ctrl.clientStatus = ctrl.clientStatus === currentStatus ? 'none' : currentStatus;
            ctrl.updateStatus();
        };

        ctrl.updateStatus = function () {
            $http.post('customers/updateClientStatus', { id: ctrl.customerId, clientStatus: ctrl.clientStatus }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Customer.ChangesSaved'));
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.Customer.ErrorWhileSaving'));
                }
            });
        };

        ctrl.selectCustomer = function (result) {
            ctrl.getCustomer(result);
        };

        ctrl.getCustomer = function (result) {
            if (result == null || result.customerId == null) {
                return false;
            }

            return $http.get('customers/getCustomerWithContact', { params: { customerId: result.customerId } }).then(function (response) {
                var customer = response.data;

                if (customer == null) return false;

                ctrl.customerId = customer.Id;
                ctrl.firstName = ctrl.selectedFirstName = customer.FirstName;
                ctrl.lastName = ctrl.selectedLastName = customer.LastName;
                ctrl.patronymic = customer.Patronymic;
                ctrl.email = customer.Email;
                ctrl.phone = customer.Phone;
                ctrl.standardPhone = customer.StandardPhone;
                ctrl.organization = customer.Organization;
                ctrl.customerType = customer.CustomerType;

                ctrl.bonusCardNumber = customer.BonusCardNumber;
                ctrl.customerGroup = customer.CustomerGroup;
                ctrl.customerFields = customer.customerFields;
                var contacts = customer.Contacts;

                if (contacts != null && contacts.length > 0) {
                    var contact = contacts[0];

                    ctrl.country = contact.Country;
                    ctrl.region = contact.Region;
                    ctrl.district = contact.District;
                    ctrl.city = contact.City;
                    ctrl.zip = contact.Zip;
                    ctrl.street = contact.Street;
                    ctrl.entrance = contact.Entrance;
                    ctrl.floor = contact.Floor;
                    ctrl.house = contact.House;
                    ctrl.structure = contact.Structure;
                    ctrl.apartment = contact.Apartment;

                    ctrl.customField1 = contact.CustomField1;
                    ctrl.customField2 = contact.CustomField2;
                    ctrl.customField3 = contact.CustomField3;
                }
                ctrl.getCustomerFields();
                return true;
            });
        };

        ctrl.resetOrderCustomer = function () {
            ctrl.customerId = null;
            ctrl.firstName = ctrl.selectedFirstName = null;
            ctrl.lastName = ctrl.selectedLastName = null;
            ctrl.patronymic = null;
            ctrl.email = null;
            ctrl.phone = null;
            ctrl.standardPhone = null;
            ctrl.country = null;
            ctrl.region = null;
            ctrl.district = null;
            ctrl.city = null;
            ctrl.zip = null;
            ctrl.street = null;
            ctrl.entrance = null;
            ctrl.floor = null;
            ctrl.house = null;
            ctrl.structure = null;
            ctrl.apartment = null;
            ctrl.organization = null;
        };

        ctrl.findCustomers = function (val) {
            if (ctrl.isDraft && val != null && val.length > 1) {
                return $http.get('customers/getCustomersAutocomplete?q=' + val).then(function (response) {
                    return response.data;
                });
            }
        };

        ctrl.selectCustomerByAutocomplete = function ($item, $model, $label, $event) {
            var customerId = $item.value;
            return ctrl.getCustomer({ customerId: customerId });
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
                    firstName: ctrl.firstName,
                    lastName: ctrl.lastName,
                    patronymic: ctrl.patronymic,
                    email: ctrl.email,
                    phone: ctrl.phone,
                    standardPhone: ctrl.standardPhone,
                    country: ctrl.country,
                    region: ctrl.region,
                    district: ctrl.district,
                    city: ctrl.city,
                    zip: ctrl.zip,
                    address: ctrl.address,
                    customField1: ctrl.customField1,
                    customField2: ctrl.customField2,
                    customField3: ctrl.customField3,
                    street: ctrl.street,
                    house: ctrl.house,
                    apartment: ctrl.apartment,
                    structure: ctrl.structure,
                    entrance: ctrl.entrance,
                    floor: ctrl.floor,
                    organization: ctrl.organization,
                    customerType: ctrl.customerType,
                },
                customerFields: ctrl.customerFields,
            };

            return $http.post('orders/saveCustomer', params).then(function (response) {
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

        ctrl.processCompanyName = function (item) {
            if (timerProcessCompanyName != null) {
                $timeout.cancel(timerProcessCompanyName);
            }

            return (timerProcessCompanyName = $timeout(
                function () {
                    if (item != null && item.CompanyData) {
                        ctrl.customerfieldsJs.forEach(function (field) {
                            if (field.FieldAssignment == 1) field.Value = item.CompanyData.CompanyName;
                            else if (field.FieldAssignment == 2) field.Value = item.CompanyData.LegalAddress;
                            else if (field.FieldAssignment == 3) field.Value = item.CompanyData.INN;
                            else if (field.FieldAssignment == 4) field.Value = item.CompanyData.KPP;
                            else if (field.FieldAssignment == 5) field.Value = item.CompanyData.OGRN;
                            else if (field.FieldAssignment == 6) field.Value = item.CompanyData.OKPO;
                        });
                    } else if (item != null && item.BankData) {
                        ctrl.customerfieldsJs.forEach(function (field) {
                            if (field.FieldAssignment == 7) field.Value = item.BankData.BIK;
                            else if (field.FieldAssignment == 8) field.Value = item.BankData.BankName;
                            else if (field.FieldAssignment == 9) field.Value = item.BankData.CorrespondentAccount;
                        });
                    }
                },
                item != null ? 0 : 700,
            ));
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

    ModalChangeOrderCustomerCtrl.$inject = ['$uibModalInstance', '$http', '$timeout', '$translate', 'toaster', '$window', '$q', '$scope'];

    ng.module('uiModal').controller('ModalChangeOrderCustomerCtrl', ModalChangeOrderCustomerCtrl);
})(window.angular);
