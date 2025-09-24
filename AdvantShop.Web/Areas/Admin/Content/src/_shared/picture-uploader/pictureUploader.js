(function (ng) {
    'use strict';

    var PictureUploaderCtrl = function ($http, Upload, toaster, SweetAlert, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.src = ctrl.startSrc;
            ctrl.pictureId = ctrl.startPictureId;

            if (ctrl.fileTypes == null) {
                ctrl.fileTypes = 'image';
            }
            ctrl.allowExts = ctrl.fileTypes?.replace(/\s?\./g, '')?.split(',');

            if (ctrl.onInit != null) {
                ctrl.onInit({ pictureUploader: ctrl });
            }
        };

        ctrl.upload = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (ctrl.uploadParams == null) {
                ctrl.uploadParams = {};
            }

            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                ctrl.send($file);
            } else if ($invalidFiles != null && $invalidFiles.length > 0) {
                toaster.pop(
                    'error',
                    $translate.instant('Admin.Js.PictureUploader.ErrorWhileLoading'),
                    $translate.instant('Admin.Js.PictureUploader.FileDoesNotMeetRequirements'),
                );
            }
        };

        ctrl.uploadByLink = function (result) {
            if (ctrl.uploadbylinkParams == null) {
                ctrl.uploadbylinkParams = {};
            }
            ctrl.uploadbylinkParams.fileLink = result;
            $http.post(ctrl.uploadbylinkUrl, ctrl.uploadbylinkParams).then(function (response) {
                var data = response.data;

                if (data.result === true) {
                    ctrl.src = data.obj.picture;
                    ctrl.pictureId = data.obj.pictureId;
                    toaster.pop('success', '', $translate.instant('Admin.Js.PictureUploader.ImageSaved'));
                } else {
                    toaster.error($translate.instant('Admin.Js.PictureUploader.ErrorWhileLoading'), (data.errors || [data.error])[0]);
                }

                if (ctrl.onUpdate != null) {
                    ctrl.onUpdate({ result: data.obj || {} });
                }
            });
        };

        ctrl.delete = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.PictureUploader.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.PictureUploader.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    let deleteParams = ctrl.deleteParams;

                    if (deleteParams == null) {
                        var objId = (ctrl.uploadParams && ctrl.uploadParams.objId) || (ctrl.uploadbylinkParams && ctrl.uploadbylinkParams.objId);
                        deleteParams = { pictureId: ctrl.pictureId, objId: objId };
                    }

                    return $http.post(ctrl.deleteUrl, deleteParams).then(function (response) {
                        var data = response.data;
                        if (data.result === true) {
                            ctrl.updatePhotoData(null, data.obj.picture);
                            toaster.pop('success', '', $translate.instant('Admin.Js.PictureUploader.ImageDeleted'));

                            if (ctrl.onDelete != null) {
                                ctrl.onDelete({ result: data.obj || {} });
                            }
                        } else {
                            toaster.error($translate.instant('Admin.Js.PictureUploader.ErrorWhileDeleting'), (data.errors || [data.error])[0]);
                        }
                    });
                }
            });
        };

        ctrl.send = function (file) {
            return Upload.upload({
                url: ctrl.uploadUrl,
                file: file,
                data: ng.extend(ctrl.uploadParams, {
                    rnd: Math.random(),
                }),
            }).then(function (response) {
                var data = response.data;

                if (data.result === true) {
                    ctrl.updatePhotoData(data.obj.pictureId, data.obj.picture);

                    toaster.pop('success', '', $translate.instant('Admin.Js.PictureUploader.ImageSaved'));
                } else {
                    toaster.error($translate.instant('Admin.Js.PictureUploader.ErrorWhileLoading'), (data.errors || [data.error])[0]);
                }

                if (ctrl.onUpdate != null) {
                    ctrl.onUpdate({ result: data.obj || {} });
                }
            });
        };

        ctrl.updatePhotoData = function (pictureId, src) {
            ctrl.pictureId = pictureId;
            ctrl.src = src;
        };
    };

    PictureUploaderCtrl.$inject = ['$http', 'Upload', 'toaster', 'SweetAlert', '$translate'];

    ng.module('pictureUploader', ['uiModal', 'toaster', 'ngFileUpload']).controller('PictureUploaderCtrl', PictureUploaderCtrl);
})(window.angular);
