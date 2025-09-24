import './searchImages.html';

(function (ng) {
    'use strict';

    var ModalSearchImagesCtrl = function ($uibModalInstance, $http, toaster, $q, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;

            ctrl.enumSelectMode = {
                single: 'single',
                multiple: 'multiple',
            };

            ctrl.uploadbylinkUrl = params.uploadbylinkUrl;
            ctrl.uploadbylinkParams = params.uploadbylinkParams;
            ctrl.selectMode = params.selectMode != null ? params.selectMode : 'single';

            ctrl.page = 0;
            ctrl.value = [];
            ctrl.enabled = null;

            ctrl.checkEnabled().then(ctrl.fetch);
        };

        ctrl.checkEnabled = function () {
            return $http.get('search/searchImagesEnabled').then(function (response) {
                ctrl.enabled = response.data.enabled;
            });
        };

        ctrl.setEnabled = function () {
            $http.post('search/setSearchImagesEnabled').then(function () {
                ctrl.checkEnabled().then(ctrl.fetch);
            });
        };

        ctrl.hideNotice = function () {
            $http.post('search/hideSearchImagesEnabled').then(function () {
                ctrl.close();
                location.reload(true);
            });
        };

        ctrl.fetch = function () {
            if (!ctrl.enabled) return;

            $http.get('search/searchImagesById', { params: ng.extend(ctrl.uploadbylinkParams, { page: ctrl.page }) }).then(function (response) {
                var data = response.data;
                if (data.errors != null && data.errors.length > 0) {
                    ctrl.error = data.errors[0];
                } else {
                    ctrl.error = null;
                    ctrl.value.length = 0;
                    ctrl.images = ng.copy(data.items);
                }
            });
        };

        ctrl.add = function () {
            if (ctrl.value != null && ctrl.value.length > 0) {
                ctrl.btnLoading = true;

                $http
                    .post(
                        ctrl.uploadbylinkUrl,
                        ng.extend(
                            ctrl.uploadbylinkParams,
                            ctrl.selectMode === ctrl.enumSelectMode.single ? { fileLink: ctrl.value[0] } : { fileLinks: ctrl.value },
                        ),
                    )
                    .then(function (response) {
                        var data = response.data;

                        if (data.result === true) {
                            toaster.pop('success', $translate.instant('Admin.Js.SearchImages.ImageSaved'));
                            $uibModalInstance.close({ result: data.obj });
                        } else {
                            data.errors.forEach(function (err) {
                                toaster.pop('error', $translate.instant('Admin.Js.SearchImages.ErrorWhileLoading'), err);
                            });
                            if (ctrl.selectMode === ctrl.enumSelectMode.multiple) {
                                $uibModalInstance.close({ result: data.obj });
                            }
                        }
                    })
                    .finally(function () {
                        ctrl.btnLoading = false;
                    });
            }
        };

        ctrl.findMore = function () {
            ctrl.page = ctrl.page + 1;
            ctrl.fetch();
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.change = function (value) {
            if (ctrl.selectMode === ctrl.enumSelectMode.single) {
                ctrl.value.splice(0, ctrl.value.length);
                ctrl.value.push(value);
            }
        };
    };

    ModalSearchImagesCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$q', '$translate'];

    ng.module('uiModal').controller('ModalSearchImagesCtrl', ModalSearchImagesCtrl);
})(window.angular);
