(function (ng) {
    'use strict';

    var ModalAddUpdateBookingServiceCtrl = function ($uibModalInstance, $http, toaster, $translate, Upload, $filter) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.id = params.id != null ? params.id : 0;
            ctrl.categoryId = params.categoryId;
            ctrl.affiliateId = params.affiliateId;
            ctrl.mode = ctrl.id != 0 ? 'edit' : 'add';
            ctrl.canBeEditing = typeof params.canBeEditing != 'undefined' ? params.canBeEditing : true;

            ctrl.getData().then(function () {
                if (ctrl.mode === 'add') {
                    ctrl.currencyId = ctrl.currencies[0].value;
                    ctrl.enabled = true;
                    ctrl.sortOrder = ctrl.maxSortOrder;
                    ctrl.price = 0;
                    ctrl.canBeEditing = true;
                } else {
                    ctrl.getService(ctrl.id);
                }
            });
        };

        ctrl.getData = function () {
            return $http.get('bookingService/getServiceData', { params: { categoryId: ctrl.categoryId } }).then(function (response) {
                var data = response.data;
                ctrl.currencies = data.currencies;
                ctrl.maxSortOrder = data.maxSortOrder;
            });
        };

        ctrl.getService = function (serviceId) {
            $http.get('bookingService/getService', { params: { serviceId: serviceId } }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    ctrl.categoryId = data.obj.CategoryId;
                    ctrl.currencyId = data.obj.CurrencyId.toString();
                    ctrl.artNo = data.obj.ArtNo;
                    ctrl.name = data.obj.Name;
                    ctrl.price = data.obj.Price;
                    ctrl.description = data.obj.Description;
                    ctrl.image = data.obj.Image;
                    ctrl.enabled = data.obj.Enabled;
                    ctrl.photoSrc = data.obj.PhotoSrc;
                    ctrl.sortOrder = data.obj.SortOrder;
                    ctrl.duration = $filter('ngFlatpickr')(data.obj.Duration, 'H:i', 'Y-m-d H:i');
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });

                    if (!data.errors) {
                        toaster.pop(
                            'error',
                            $translate.instant('Admin.Js.BookingServices.Error'),
                            $translate.instant('Admin.Js.BookingServices.ErrorUploadingServiceData'),
                        );
                    }
                    ctrl.close();
                }
            });
        };

        ctrl.updateImage = function (params) {
            if (params != null) {
                ctrl.image = params.fileName;
                ctrl.photoSrc = ctrl.photoEncoded = params.base64String;
                ctrl.photoFile = params.file;
            }
        };

        ctrl.deleteImage = function () {
            if (ctrl.photoEncoded) {
                ctrl.image = null;
                ctrl.photoSrc = ctrl.photoEncoded = null;
                ctrl.photoFile = null;
            } else {
                $http.post('bookingService/deleteServiceImage', { id: ctrl.id }).then(function (response) {
                    var data = response.data;

                    if (data.result === true) {
                        ctrl.image = null;
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });

                        if (!data.errors) {
                            toaster.pop(
                                'error',
                                $translate.instant('Admin.Js.BookingServices.Error'),
                                $translate.instant('Admin.Js.BookingServices.ErrorWhileDeletingImage'),
                            );
                        }
                    }
                });
            }
        };

        ctrl.saveService = function () {
            var params = {
                id: ctrl.id,
                artNo: ctrl.artNo,
                categoryId: ctrl.categoryId,
                affiliateId: ctrl.affiliateId,
                name: ctrl.name,
                image: ctrl.image,
                currencyId: ctrl.currencyId,
                price: ctrl.price,
                description: ctrl.description,
                enabled: ctrl.enabled,
                sortOrder: ctrl.sortOrder ?? '',
                duration: ctrl.duration ?? '',
                PhotoFile: ctrl.photoFile,
            };
            var url = ctrl.mode === 'add' ? 'bookingService/add' : 'bookingService/update';
            Upload.upload({
                url: url,
                data: params,
            }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.BookingServices.ChangesSaved'));
                    $uibModalInstance.close('saveBookingService');
                } else {
                    ctrl.btnLoading = false;
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });

                    if (!data.errors) {
                        toaster.pop(
                            'error',
                            $translate.instant('Admin.Js.BookingServices.Error'),
                            $translate.instant('Admin.Js.BookingServices.ErrorCreating'),
                        );
                    }
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalAddUpdateBookingServiceCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate', 'Upload', '$filter'];

    ng.module('uiModal').controller('ModalAddUpdateBookingServiceCtrl', ModalAddUpdateBookingServiceCtrl);
})(window.angular);
