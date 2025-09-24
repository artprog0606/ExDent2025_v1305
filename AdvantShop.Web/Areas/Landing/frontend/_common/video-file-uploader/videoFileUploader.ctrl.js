(function (ng) {
    'use strict';

    var VideoFileUploaderCtrl = function ($http, Upload, toaster, SweetAlert, $timeout) {
        var ctrl = this;

        ctrl.$onInit = function () {
            if (ctrl.urlListVideo != null) {
                ctrl.getVideoList(ctrl.urlListVideo).then(function (result) {
                    ctrl.videoList = result;
                });
            }
            ctrl.disabledLoadBtn = true;
        };

        ctrl.load = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            ctrl.fileName = $file.name;
            ctrl.file = $file;
            ctrl.disabledLoadBtn = false;
        };

        ctrl.submit = function (file) {
            if (file != null) {
                Upload.upload({
                    url: ctrl.uploadUrl,
                    data: { file: file, rnd: Math.random() },
                }).then(
                    function (resp) {
                        ctrl.disabledLoadBtn = true;
                        ctrl.fileName = null;
                        ctrl.file = null;
                        ctrl.settings.urlVideo = resp.data.url;

                        return ctrl.getVideoList(ctrl.urlListVideo).then(function (result) {
                            return (ctrl.videoList = result);
                        });
                    },
                    function (resp) {
                        toaster.pop('error', '', 'Не удалось загрузить видеофайл');
                    },
                    function (evt) {
                        ctrl.progressPercentage = parseInt((100.0 * evt.loaded) / evt.total);
                        if (ctrl.progressPercentage === 100) {
                            ctrl.progressPercentage = -1;
                        }
                    },
                );
            }
        };

        ctrl.onDeleteVideo = function (url, name) {
            SweetAlert.alert('Вы уверены что хотите удалить видеофайл?', { title: '', showCancelButton: true }).then(function (result) {
                if (result === true || result.value === true) {
                    ctrl.deleteVideo(url, name).then(function (response) {
                        if (response.result === true) {
                            toaster.pop('success', '', 'Видеофайл успешно удален');

                            if ('userfiles/lp-video/' + name == ctrl.settings.urlVideo) {
                                ctrl.settings.urlVideo = '';
                            }

                            ctrl.getVideoList(ctrl.urlListVideo).then(function (result) {
                                ctrl.videoList = result;
                            });
                        }
                    });
                }
            });
        };

        ctrl.getVideoList = function (url) {
            return $http
                .get(url + '?rnd=' + Math.random())
                .then(function (response) {
                    return response.data;
                })
                .catch(function (error) {
                    console.error(error);
                });
        };

        ctrl.deleteVideo = function (url, name) {
            return $http
                .post(url, { name: name, rnd: Math.random() })
                .then(function (response) {
                    return response.data;
                })
                .catch(function (error) {
                    console.error(error);
                });
        };
    };

    ng.module('videoFileUploader').controller('VideoFileUploaderCtrl', VideoFileUploaderCtrl);

    VideoFileUploaderCtrl.$inject = ['$http', 'Upload', 'toaster', 'SweetAlert', '$timeout'];
})(window.angular);
