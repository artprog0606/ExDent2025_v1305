(function (ng) {
    'use strict';

    /* @ngInject */
    var ModalAddCardCtrl = function ($uibModalInstance, $http, $window, toaster, $q, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            if (ctrl.$resolve != null) {
                var params = ctrl.$resolve.params;
                if (params != null) {
                    ctrl.customerId = params.customerId;
                    ctrl.selectedFirstName = params.firstName;
                    ctrl.selectedLastName = params.lastName;
                    ctrl.noredirect = params.noredirect;
                    ctrl.isMobile = params.isMobile;
                }
            }

            $http.get('grades/GetAllGrades').then(
                function (result) {
                    ctrl.Grades = result.data.obj;
                },
                function (err) {
                    toaster.pop('error', $translate.instant('Admin.Js.Cards.ErrorObtainingTheGrades'), err);
                },
            );

            $http.get('grades/defaultgrade').then(
                function (result) {
                    ctrl.GradeId = result.data.obj;
                },
                function (err) {
                    toaster.pop('error', $translate.instant('Admin.Js.Cards.ErrorGettingTheDefaultGrade'), err);
                },
            );

            $http.get('cards/generate').then(
                function (result) {
                    ctrl.CardNumber = result.data.obj;
                },
                function (err) {
                    toaster.pop('error', $translate.instant('Admin.Js.Cards.ErrorGettingTheDefaultGrade'), err);
                },
            );
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.selectCustomer = function (result) {
            ctrl.getCustomer(result).then(function (result) {
                return result || $q.reject('error');
            });
        };

        ctrl.getCustomer = function (result) {
            if (result == null || result.customerId == null) {
                return false;
            }

            return $http.get('customers/getCustomerWithContact', { params: { customerId: result.customerId } }).then(function (response) {
                var customer = response.data;

                if (customer == null) return false;

                ctrl.customerId = customer.Id;

                //if (isNullOrWhitespace(ctrl.firstName))
                ctrl.firstName = ctrl.selectedFirstName = customer.FirstName;

                //if (isNullOrWhitespace(ctrl.lastName))
                ctrl.lastName = ctrl.selectedLastName = customer.LastName;

                //if (isNullOrWhitespace(ctrl.patronymic))
                ctrl.patronymic = customer.Patronymic;

                //if (isNullOrWhitespace(ctrl.email))
                ctrl.email = customer.Email;

                //if (isNullOrWhitespace(ctrl.phone))
                ctrl.phone = customer.Phone;

                //if (isNullOrWhitespace(ctrl.standardPhone))
                ctrl.standardPhone = customer.StandardPhone;

                ctrl.bonusCardNumber = customer.BonusCardNumber;
                ctrl.customerGroup = customer.CustomerGroup;
                var contacts = customer.Contacts;

                if (contacts != null && contacts.length > 0) {
                    var contact = contacts[0];

                    if (ctrl.region?.length > 0) ctrl.region = contact.Region;

                    if (ctrl.city?.length > 0) ctrl.city = contact.City;

                    if (ctrl.zip?.length > 0) ctrl.zip = contact.Zip;

                    if (ctrl.address?.length > 0) ctrl.address = contact.Address;

                    ctrl.customField1 = contact.CustomField1;
                    ctrl.customField2 = contact.CustomField2;
                    ctrl.customField3 = contact.CustomField3;
                }
                return true;
            });
        };

        ctrl.addCard = function () {
            ctrl.btnLoading = true;
            $http
                .post('cards/add', {
                    CardId: ctrl.customerId,
                    CardNumber: ctrl.CardNumber,
                    GradeId: ctrl.GradeId,
                })
                .then(
                    function (result) {
                        var data = result.data.result;
                        if (data === true) {
                            if (ctrl.isMobile) {
                                toaster.pop('success', $translate.instant('Admin.Js.Cards.CardAdded'));
                                //$uibModalInstance.close();
                                $window.location.reload();
                            } else if (ctrl.noredirect) {
                                $uibModalInstance.close();
                            } else {
                                $window.location.assign('cards/edit/' + ctrl.customerId);
                            }
                            toaster.pop('success', $translate.instant('Admin.Js.Cards.CardAdded'));
                        } else {
                            var er = '';
                            if (result.data.errors != null) {
                                er = result.data.errors.join('</br>');
                            }
                            toaster.pop('error', $translate.instant('Admin.Js.Cards.ErrorAddingCard'), er);
                        }
                    },
                    function () {
                        toaster.pop('error', $translate.instant('Admin.Js.Cards.ErrorAddingCard'));
                    },
                )
                .finally(function () {
                    ctrl.btnLoading = false;
                });
        };
    };

    ng.module('uiModal').controller('ModalAddCardCtrl', ModalAddCardCtrl);
})(window.angular);
