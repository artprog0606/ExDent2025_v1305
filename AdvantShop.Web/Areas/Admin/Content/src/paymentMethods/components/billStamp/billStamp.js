import billStampTemplate from './templates/billStamp.html';
(function (ng) {
    'use strict';

    var BillStampCtrl = function ($http, toaster, Upload, SweetAlert, $translate) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.StampImageName = ctrl.stampImg;
            ctrl.StampImageSrc = ctrl.stampImgSrc;
        };
        ctrl.uploadBillStampImage = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                ctrl.sendIcon($file);
            } else if ($invalidFiles.length > 0) {
                toaster.pop(
                    'error',
                    $translate.instant('Admin.Js.PaymentMethods.ErrorWhileUploading'),
                    $translate.instant('Admin.Js.PaymentMethods.FileNotMeet'),
                );
            }
        };
        ctrl.deleteBillStampImage = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.PaymentMethods.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.PaymentMethods.Deleting'),
            }).then(function (result) {
                if (result === true || result.value === true) {
                    return $http
                        .post('paymentMethods/deleteBillStamp', {
                            methodId: ctrl.methodId,
                        })
                        .then(function (response) {
                            var data = response.data;
                            if (data.result === true) {
                                ctrl.StampImageName = null;
                                ctrl.StampImageSrc = null;
                                toaster.pop('success', '', $translate.instant('Admin.Js.PaymentMethods.ImageDeleted'));
                            } else {
                                toaster.pop('error', $translate.instant('Admin.Js.PaymentMethods.ErrorWhileDeleting'), data.error);
                            }
                        });
                }
            });
        };
        ctrl.sendIcon = function (file) {
            return Upload.upload({
                url: 'paymentMethods/uploadBillStamp',
                data: {
                    file: file,
                    methodId: ctrl.methodId,
                    rnd: Math.random(),
                },
            }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    ctrl.StampImageName = data.imgName;
                    ctrl.StampImageSrc = data.src;
                    toaster.pop('success', '', $translate.instant('Admin.Js.PaymentMethods.ImageSaved'));
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.PaymentMethods.ErrorWhileUploading'), data.error);
                }
            });
        };
    };
    BillStampCtrl.$inject = ['$http', 'toaster', 'Upload', 'SweetAlert', '$translate'];
    ng.module('paymentMethod')
        .controller('BillStampCtrl', BillStampCtrl)
        .component('billStamp', {
            templateUrl: billStampTemplate,
            controller: 'BillStampCtrl',
            bindings: {
                onInit: '&',
                methodId: '@',
                stampImg: '@',
                stampImgSrc: '@',
            },
        });
})(window.angular);
