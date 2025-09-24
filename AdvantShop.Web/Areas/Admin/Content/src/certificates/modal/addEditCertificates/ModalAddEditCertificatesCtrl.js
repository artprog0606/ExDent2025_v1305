(function (ng) {
    'use strict';

    var ModalAddEditCertificatesCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.CertificateId = params.id != null ? params.id : 0;
            ctrl.mode = ctrl.CertificateId != 0 && ctrl.CertificateId != undefined && ctrl.CertificateId != null ? 'edit' : 'add';

            if (ctrl.mode == 'edit') {
                ctrl.getCertificate(ctrl.CertificateId);
            } else {
                ctrl.CertificateId = 0;
                ctrl.getCertificate(null);
                ctrl.Enable = true;
            }
        };

        function selectedValue(value) {
            return value.Selected == true;
        }

        ctrl.getCertificate = function (ID) {
            $http
                .get('Certificates/getCertificatesItem', {
                    params: {
                        ID: ID,
                        rnd: Math.random(),
                    },
                })
                .then(function (response) {
                    var certif = response.data.model;
                    var payment = response.data.listsPayment;
                    if (certif != null) {
                        ctrl.CertificateCode = certif.CertificateCode;
                        ctrl.FromName = certif.FromName;
                        ctrl.ToName = certif.ToName;
                        ctrl.Sum = certif.Sum;
                        ctrl.ToEmail = certif.ToEmail;
                        ctrl.Used = certif.Used;
                        ctrl.Enable = certif.Enable;
                        ctrl.Paid = certif.OrderCertificatePaid;
                        ctrl.CertificateMessage = certif.CertificateMessage;
                        ctrl.OrderId = certif.OrderId;
                    }

                    ctrl.FromEmail = response.data.FromEmail;

                    if (payment != null) {
                        ctrl.payment = payment;
                        var mass = payment.filter(selectedValue);
                        ctrl.paymentSelected = mass.length > 0 ? mass[0] : null;
                    }
                });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.saveCertificate = function () {
            ctrl.btnSleep = true;

            var params = {
                CertificateId: ctrl.CertificateId,
                CertificateCode: ctrl.CertificateCode,
                FromName: ctrl.FromName,
                ToName: ctrl.ToName,
                Sum: ctrl.Sum,
                ToEmail: ctrl.ToEmail,
                Used: ctrl.Used,
                Enable: ctrl.Enable,
                Paid: ctrl.Paid,
                CertificateMessage: ctrl.CertificateMessage,
                paymentid: ctrl.paymentSelected != null ? ctrl.paymentSelected.Value : 0,
                FromEmail: ctrl.FromEmail,
                rnd: Math.random(),
            };

            var url = ctrl.mode == 'add' ? 'Certificates/AddCertificates' : 'Certificates/EditCertificates';

            $http.post(url, params).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Certificates.ChangesSaved'));
                    $uibModalInstance.close('saveCertificate');
                } else {
                    if (data.errors.length) {
                        toaster.pop('error', '', data.errors[0]);
                    } else {
                        toaster.pop(
                            'error',
                            $translate.instant('Admin.Js.Certificates.Error'),
                            $translate.instant('Admin.Js.Certificates.ErrorWhileCreating'),
                        );
                    }
                    ctrl.btnSleep = false;
                }
            });
        };
    };

    ModalAddEditCertificatesCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalAddEditCertificatesCtrl', ModalAddEditCertificatesCtrl);
})(window.angular);
