import productPhotos360Template from './productPhotos360.html';
(function (ng) {
    'use strict';

    var ProductPhotos360Ctrl = function ($http, $q, toaster, SweetAlert, Upload, $translate) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.getPhotos(ctrl.productId).then(function (photos) {
                ctrl.photos = photos;
            });
            ctrl.getActivity(ctrl.productId).then(function (isActive) {
                ctrl.showPhotos360 = isActive;
            });
        };
        ctrl.deletePhoto = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Product.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Product.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http
                        .post('product/deletePhoto360', {
                            productId: ctrl.productId,
                        })
                        .then(function (response) {
                            ctrl.getPhotos(ctrl.productId).then(function (photos) {
                                ctrl.photos = photos;
                            });
                        });
                }
            });
        };
        ctrl.upload = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                var photosExist = ctrl.photos != null;
                if (photosExist) {
                    SweetAlert.confirm($translate.instant('Admin.Js.Product.AreYouSureUpload360Images'), {
                        title: $translate.instant('Admin.Js.Product.Attention'),
                    }).then(function (result) {
                        if (result === true || result.value) uploadPhotos($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event);
                    });
                } else {
                    uploadPhotos($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event);
                }
            } else if ($invalidFiles.length > 0) {
                toaster.pop(
                    'error',
                    $translate.instant('Admin.Js.Product.ErrorWhileUploading'),
                    $translate.instant('Admin.Js.Product.FileDoesNotMeetRequirements'),
                );
            }
        };
        ctrl.setActivity = function (productId, isActive) {
            $http
                .post('product/setActivityPhotos360', {
                    productId: productId,
                    isActive: isActive,
                })
                .then(
                    function (response) {
                        if (response.data.result === true) {
                            toaster.pop('success', $translate.instant('Admin.Js.Product.ChangesSuccessfullyApplied'));
                        }
                    },
                    function () {
                        toaster.pop('error', $translate.instant('Admin.Js.Product.ErrorWhileSavingSettings'));
                    },
                );
        };
        ctrl.getActivity = function (productId) {
            return $http
                .get('product/getActivityPhotos360', {
                    params: {
                        productId: productId,
                    },
                })
                .then(function (response) {
                    return response.data;
                });
        };
        function uploadPhotos($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            Upload.upload({
                url: 'product/uploadphotos360',
                data: {
                    files: $files,
                    productId: ctrl.productId,
                },
            }).then(
                function (response) {
                    if (response.data.result === true) {
                        ctrl.getPhotos(ctrl.productId).then(function (photos) {
                            ctrl.photos = photos;
                            toaster.pop('success', $translate.instant('Admin.Js.Product.UploadingPhotoWasSuccessful'));
                        });
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.Product.ErrorWhileUploadingPhotos'), response.data.error);
                    }
                },
                function (error) {
                    toaster.pop('error', $translate.instant('Admin.Js.Product.ErrorWhileUploadingPhotos'));
                },
            );
        }
        ctrl.getPhotos = function (productId) {
            return $http
                .get('product/getPhotos360', {
                    params: {
                        productId: productId,
                    },
                })
                .then(function (response) {
                    return response.data;
                });
        };

        ctrl.addParameterWithRandomValue = function (src) {
            return src + '?' + new Date().getMinutes();
        };
    };
    ProductPhotos360Ctrl.$inject = ['$http', '$q', 'toaster', 'SweetAlert', 'Upload', '$translate'];
    ng.module('productPhotos360', ['ngFileUpload'])
        .controller('ProductPhotos360Ctrl', ProductPhotos360Ctrl)
        .component('productPhotos360', {
            templateUrl: productPhotos360Template,
            controller: 'ProductPhotos360Ctrl',
            bindings: {
                productId: '@',
            },
        });
})(window.angular);
