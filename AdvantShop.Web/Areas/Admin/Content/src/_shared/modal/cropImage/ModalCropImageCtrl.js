import './cropImage.html';

(function (ng) {
    'use strict';

    var ModalCropImageCtrl = function ($uibModalInstance, Upload, toaster, $http, $window, urlHelper, Cropper, $scope, $timeout, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.params = ctrl.$resolve != null ? ctrl.$resolve.params : null;
            $scope.cropper = {};
            ctrl.cropperProxy = 'cropper.first';

            ctrl.cropOptions = {
                viewMode: 1,
                maximize: true,
                preview: '.preview-container',
                aspectRatio: 1 / 1,
                crop: function (dataNew) {
                    data = dataNew;
                },
            };
        };

        ctrl.showUploadByUrl = false;

        ctrl.close = function () {
            ctrl.showUploadByUrl = false;
            $uibModalInstance.dismiss('cancel');
        };

        /* cropper */
        var file, fileName, data;

        ctrl.cropImage = function () {
            if (!file || !data) return;
            Cropper.crop(file, data.detail)
                .then(Cropper.encode)
                .then(function (dataUrl) {
                    var meta = dataUrl.split(';')[0];
                    var type = meta.split(':')[1];
                    $uibModalInstance.close({
                        fileName: file.name != null ? file.name : fileName,
                        base64String: dataUrl,
                        file: new File([Cropper.decode(dataUrl)], file.name != null ? file.name : fileName, { type: type }),
                    });
                });
        };

        ctrl.showEvent = 'show';
        ctrl.hideEvent = 'hide';

        function showCropper() {
            $scope.$broadcast(ctrl.showEvent);
        }
        /* end cropper */

        // upload image by file upload
        ctrl.uploadImage = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                Cropper.encode((file = $file)).then(function (dataUrl) {
                    ctrl.imageSrc = dataUrl;
                    ctrl.replaceCropperImage(ctrl.imageSrc);
                    $timeout(showCropper);
                });
            }
        };

        ctrl.addImageByUrl = function () {
            ctrl.showUploadByUrl = true;
        };

        // upload image by url
        ctrl.uploadImageByUrl = function () {
            if (ctrl.url == null || ctrl.url == '') return;

            $http.post('common/uploadimagebyurl', { url: ctrl.url }).then(function (response) {
                var data = response.data;

                if (data.result === true) {
                    fileName = data.file;

                    $http.get(data.file, { responseType: 'blob' }).then(function (responseUploaded) {
                        var blob = responseUploaded.data;
                        Cropper.encode((file = blob)).then(function (dataUrl) {
                            ctrl.imageSrc = dataUrl;
                            ctrl.replaceCropperImage(ctrl.imageSrc);
                            $timeout(showCropper);
                        });
                    });
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.CropImage.ErrorWhileLoading'), data.error);
                }
            });
        };

        ctrl.replaceCropperImage = function (newUrl) {
            if ($scope.cropper && $scope.cropper.first) {
                const jqCropper = $scope.cropper.first();
                if (jqCropper) {
                    const cropper = jqCropper[0].cropper;
                    if (cropper) {
                        cropper.replace(newUrl);
                    }
                }
            }
        };
    };

    ModalCropImageCtrl.$inject = [
        '$uibModalInstance',
        'Upload',
        'toaster',
        '$http',
        '$window',
        'urlHelper',
        'Cropper',
        '$scope',
        '$timeout',
        '$translate',
    ];

    ng.module('uiModal').controller('ModalCropImageCtrl', ModalCropImageCtrl);
})(window.angular);
