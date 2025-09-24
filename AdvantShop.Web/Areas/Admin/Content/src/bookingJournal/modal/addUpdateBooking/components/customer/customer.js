import customerTemplate from './customer.html';
import './find-customer.html';

(function (ng) {
    'use strict';

    var BookingCustomerCtrl = function ($http, $q, toaster, $timeout, SweetAlert, $translate) {
        var ctrl = this,
            timerProcessCompanyName;
        ctrl.$onInit = function () {
            ctrl.getCustomerTypes();
            if (ctrl.params.customerId) {
                ctrl.getCustomer(ctrl.params.customerId).then(function () {
                    if (ctrl.params.phone) {
                        ctrl.customer.phone = ctrl.params.phone;
                    }
                    ctrl.changedCustomer();
                });
            } else {
                if (ctrl.params.phone) {
                    ctrl.customer.phone = ctrl.params.phone;
                }
            }
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    bookingCustomer: ctrl,
                });
            }
        };
        ctrl.findCustomer = function (val) {
            if (ctrl.mode === 'add' || !ctrl.customer.customerId) {
                return $http
                    .get('customers/getCustomersAutocomplete', {
                        params: {
                            q: val,
                            rnd: Math.random(),
                        },
                    })
                    .then(function (response) {
                        return response.data;
                    });
            }
            return null;
        };
        ctrl.selectCustomer = function (result) {
            if (result != null) {
                ctrl.getCustomer(result.customerId || result.CustomerId).then(function (result) {
                    ctrl.changedCustomer();
                    return result || $q.reject('error');
                });
            }
        };
        ctrl.clearCustomer = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Colors.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Colors.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    ctrl.customer.lastName = null;
                    ctrl.customer.firstName = null;
                    ctrl.customer.patronymic = null;
                    ctrl.customer.organization = null;
                    ctrl.customer.phone = null;
                    ctrl.customer.standardPhone = null;
                    ctrl.customer.email = null;
                    ctrl.customer.birthday = null;
                    ctrl.customer.customerId = null;
                    ctrl.changedCustomer();
                }
            });
        };
        ctrl.changedCustomer = function () {
            ctrl.getCustomerFields();
            ctrl.getCustomerSocial();
            $timeout(function () {
                if (ctrl.bookingEvents) {
                    ctrl.bookingEvents.getLeadEvents();
                }
            });
        };
        ctrl.getCustomerSocial = function () {
            $http
                .get('booking/getCustomerSocial', {
                    params: {
                        customerId: ctrl.customer.customerId,
                    },
                })
                .then(function (response) {
                    ctrl.customer.social = response.data;
                });
        };
        ctrl.callGetgCustomerFields = function () {
            return ctrl.getCustomerFields();
        };
        ctrl.getCustomerFields = function () {
            return ctrl.getFunctionGetCustomerFields().then(function () {
                return ctrl.getCustomerFieldsFn();
            });
        };
        ctrl.onCustomerFieldsInit = function (reloadFn) {
            ctrl.getCustomerFieldsFn = reloadFn || function () {};
            if (ctrl.functionGetCustomerFieldsPromise) {
                ctrl.functionGetCustomerFieldsPromise.resolve();
            }
        };
        ctrl.getFunctionGetCustomerFields = function () {
            if (ctrl.getCustomerFieldsFn) {
                return $q.resolve();
            } else {
                ctrl.functionGetCustomerFieldsPromise = ctrl.functionGetCustomerFieldsPromise ? ctrl.functionGetCustomerFieldsPromise : $q.defer();
                return ctrl.functionGetCustomerFieldsPromise.promise;
            }
        };
        ctrl.getCustomer = function (customerId) {
            if (customerId == null) {
                return $q.defer().resolve(false);
            }
            return $http
                .get('customers/getCustomerWithContact', {
                    params: {
                        customerId: customerId,
                    },
                })
                .then(function (response) {
                    var customer = response.data;
                    if (customer == null) return false;
                    ctrl.customer.customerId = customer.Id;
                    ctrl.customer.firstName = customer.FirstName;
                    ctrl.customer.lastName = customer.LastName;
                    ctrl.customer.patronymic = customer.Patronymic;
                    ctrl.customer.organization = customer.Organization;
                    ctrl.customer.email = customer.Email;
                    ctrl.customer.phone = customer.Phone;
                    ctrl.customer.standardPhone = customer.StandardPhone;
                    ctrl.customer.birthday = customer.BirthDay;
                    ctrl.customer.customerType = ctrl.customerTypes.filter((x) => x.value == customer.CustomerType)[0];
                    return true;
                });
        };
        ctrl.addSocialUser = function (type, link) {
            if (ctrl.btnSocialAdding != null || !link) return;
            var url = '';
            switch (type) {
                case 'vk':
                    url = 'vk/addVkUser';
                    break;
                case 'facebook':
                    url = 'facebook/addFacebookUser';
                    break;
                case 'instagram':
                    url = 'instagram/addInstagramUser';
                    break;
            }
            ctrl.btnSocialAdding = type;
            $http
                .post(url, {
                    customerId: ctrl.customer.customerId,
                    link: link,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        toaster.pop('success', '', 'Изменения сохранены');
                        ctrl.getCustomerSocial();
                    } else {
                        if (data.errors != null) {
                            data.errors.forEach(function (error) {
                                toaster.pop('error', '', error);
                            });
                        } else {
                            toaster.pop('error', 'Ошибка при сохранении');
                        }
                    }
                })
                .finally(function () {
                    ctrl.btnSocialAdding = null;
                });
        };
        ctrl.filterEvents = function (filterBy) {
            ctrl.bookingEvents.filterType = filterBy;
        };
        ctrl.updateBookingEvents = function () {
            ctrl.bookingEvents.getLeadEvents();
        };
        ctrl.updateBookingEventsWithDelay = function () {
            setTimeout(ctrl.updateBookingEvents, 800);
        };
        ctrl.getCustomerTypes = function () {
            $http.post('booking/getBookingCustomerFormData').then(function (response) {
                var data = response.data;
                if (data) {
                    ctrl.showCustomerType = data.isRegistrationAsPhysicalEntity && data.isRegistrationAsLegalEntity;
                    ctrl.customerTypes = data.customerTypes;
                    if (ctrl.mode === 'add')
                        ctrl.customer.customerType = ctrl.showCustomerType
                            ? ctrl.customerTypes[0]
                            : data.isRegistrationAsPhysicalEntity
                              ? ctrl.customerTypes[0]
                              : ctrl.customerTypes[1];
                    else if (ctrl.mode === 'edit' && Number.isInteger(ctrl.customer.customerType))
                        ctrl.customer.customerType = ctrl.customerTypes[ctrl.customer.customerType];
                }
            });
        };
        ctrl.processCompanyName = function (item) {
            if (timerProcessCompanyName != null) {
                $timeout.cancel(timerProcessCompanyName);
            }
            return (timerProcessCompanyName = $timeout(
                function () {
                    if (item != null && item.CompanyData) {
                        ctrl.instance.customerfieldsJs.forEach(function (field) {
                            if (field.FieldAssignment == 1) field.Value = item.CompanyData.CompanyName;
                            else if (field.FieldAssignment == 2) field.Value = item.CompanyData.LegalAddress;
                            else if (field.FieldAssignment == 3) field.Value = item.CompanyData.INN;
                            else if (field.FieldAssignment == 4) field.Value = item.CompanyData.KPP;
                            else if (field.FieldAssignment == 5) field.Value = item.CompanyData.OGRN;
                            else if (field.FieldAssignment == 6) field.Value = item.CompanyData.OKPO;
                        });
                    } else if (item != null && item.BankData) {
                        ctrl.instance.customerfieldsJs.forEach(function (field) {
                            if (field.FieldAssignment == 7) field.Value = item.BankData.BIK;
                            else if (field.FieldAssignment == 8) field.Value = item.BankData.BankName;
                            else if (field.FieldAssignment == 9) field.Value = item.BankData.CorrespondentAccount;
                        });
                    }
                },
                item != null ? 0 : 700,
            ));
        };
    };
    BookingCustomerCtrl.$inject = ['$http', '$q', 'toaster', '$timeout', 'SweetAlert', '$translate'];
    ng.module('bookingCustomer', [])
        .controller('BookingCustomerCtrl', BookingCustomerCtrl)
        .component('bookingCustomer', {
            templateUrl: customerTemplate,
            controller: 'BookingCustomerCtrl',
            bindings: {
                onInit: '&',
                params: '<?',
                customer: '<',
                mode: '<',
                canBeEditing: '<?',
                bookingEvents: '<',
            },
        });
})(window.angular);
