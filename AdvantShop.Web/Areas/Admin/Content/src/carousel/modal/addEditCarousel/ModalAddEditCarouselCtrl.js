(function (ng) {
    'use strict';

    var ModalAddEditCarouselCtrl = function ($uibModalInstance, $http, toaster, Upload, $translate) {
        const ctrl = this;

        ctrl.$onInit = function () {
            ctrl.carouselId = 0;
            const params = ctrl.$resolve;
            const carousel = params.carousel;
            ctrl.fileTypes = params?.resolve?.fileTypes;
            ctrl.mode = carousel != null ? 'edit' : 'add';
            if (carousel) {
                ctrl.carouselId = carousel.CarouselId != null ? carousel.CarouselId : 0;
                ctrl.CaruselUrl = carousel.CarouselUrl;
                ctrl.DisplayInOneColumn = carousel.DisplayInOneColumn;
                ctrl.DisplayInTwoColumns = carousel.DisplayInTwoColumns;
                ctrl.DisplayInMobile = carousel.DisplayInMobile;
                ctrl.Blank = carousel.Blank;
                ctrl.SortOrder = carousel.SortOrder;
                ctrl.Enabled = carousel.Enabled;
                ctrl.ImageSrc = carousel.ImageSrc;
                ctrl.Description = carousel.Description;
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.updateImage = function (result) {
            ctrl.ImageSrc = result.pictureName;
        };

        ctrl.saveCarousel = function () {
            ctrl.btnSleep = true;

            if (ctrl.ImageSrc == null || ctrl.ImageSrc === '') {
                toaster.pop(
                    'error',
                    $translate.instant('Admin.Js.Carousel.ImageNotUploaded'),
                    $translate.instant('Admin.Js.Carousel.PleaseUploadAnImage'),
                );
                ctrl.btnSleep = false;
                return;
            }

            var params = {
                CarouselID: ctrl.carouselId,
                CarouselUrl: ctrl.CaruselUrl,
                DisplayInOneColumn: ctrl.DisplayInOneColumn,
                DisplayInTwoColumns: ctrl.DisplayInTwoColumns,
                DisplayInMobile: ctrl.DisplayInMobile,
                Blank: ctrl.Blank,
                SortOrder: ctrl.SortOrder,
                Enabled: ctrl.Enabled,
                ImageSrc: ctrl.ImageSrc,
                Description: ctrl.Description,
                rnd: Math.random(),
            };

            const url = ctrl.mode === 'edit' ? 'Carousel/InplaceCarousel' : 'Carousel/AddCarousel';

            $http.post(url, params).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Carousel.ChangesSaved'));
                    $uibModalInstance.close(params);
                } else {
                    toaster.pop(
                        'error',
                        $translate.instant('Admin.Js.Carousel.Error'),
                        $translate.instant('Admin.Js.Carousel.ErrorWhileAddingImage'),
                    );
                    ctrl.btnSleep = false;
                }
            });
        };
    };

    ModalAddEditCarouselCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', 'Upload', '$translate'];

    ng.module('uiModal').controller('ModalAddEditCarouselCtrl', ModalAddEditCarouselCtrl);
})(window.angular);
