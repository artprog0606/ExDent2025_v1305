(function (ng) {
    'use strict';

    var ModalAddEditYandexPromoCodeCtrl = function ($uibModalInstance, $http, toaster, $translate, $window) {
        var ctrl = this;
        ctrl.warning = '';

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.value;
            ctrl.ExportFeedId = params.ExportFeedId;
            ctrl.PromoID = params.PromoID != null ? params.PromoID : null;
            ctrl.mode = ctrl.PromoID != null ? 'edit' : 'add';
            ctrl.promo = {};
            ctrl.fetchCoupons();
        };

        ctrl.fetchCoupons = function () {
            ctrl.warning = '';
            $http.post('coupons/GetCoupons').then(function (response) {
                if (response.data != null) {
                    if (response.data.DataItems.length == 0) {
                        toaster.pop('error', 'Ошибка', $translate.instant('Admin.Js.AddEditYandexPromo.ErrorGettingCouponsCount'));
                    }
                    if (
                        response.data.DataItems.filter(function (elem) {
                            return elem.Code.length > 20;
                        }).length > 0
                    ) {
                        ctrl.warning = $translate.instant('Admin.Js.AddEditYandexPromo.WarningSomeCouponsFiltered');
                    }
                    ctrl.coupons = response.data.DataItems.filter(function (elem) {
                        return elem.Code.length <= 20;
                    });
                    if (
                        response.data.DataItems.length != 0 &&
                        ctrl.coupons.find(function (element) {
                            return element.Enabled;
                        }) == null
                    ) {
                        toaster.pop('error', 'Внимание', $translate.instant('Admin.Js.AddEditYandexPromo.ErrorGettingCouponsAllDisabled'));
                    }
                    ctrl.promo.Coupon = ctrl.coupons.find(function (element) {
                        return element.Enabled;
                    });

                    if (ctrl.mode == 'edit') {
                        $http
                            .post('exportfeeds/GetYandexPromo', {
                                promoID: ctrl.PromoID,
                                exportFeedId: ctrl.ExportFeedId,
                            })
                            .then(function (responce) {
                                ctrl.promo = responce.data;
                                var coupon = response.data.DataItems.find(function (element) {
                                    return element.CouponId == responce.data.CouponId;
                                });
                                if (coupon === undefined) {
                                    ctrl.promo.Coupon = {};
                                    ctrl.warning = $translate.instant('Admin.Js.AddEditYandexPromo.WarningCouponWasDeleted');
                                } else {
                                    ctrl.promo.Coupon = coupon;
                                    coupon = ctrl.coupons.find(function (element) {
                                        return element.CouponId == responce.data.CouponId;
                                    });
                                    if (coupon === undefined) {
                                        ctrl.promo.Coupon.Enabled = false;
                                        ctrl.coupons.push(ctrl.promo.Coupon);
                                        ctrl.warning = $translate.instant('Admin.Js.AddEditYandexPromo.WarningCouponTooLong');
                                    }
                                }
                            });
                    }
                } else {
                    toaster.pop('error', 'Ошибка', $translate.instant('Admin.Js.AddEditYandexPromo.ErrorGettingCoupons'));
                }
            });
        };

        ctrl.save = function () {
            var PromoCode = {
                Type: 'PromoCode',
                Name: ctrl.promo.Name,
                Description: ctrl.promo.Description,
                PromoUrl: ctrl.promo.PromoUrl,
                CouponId: ctrl.promo.Coupon.CouponId,
                PromoID: ctrl.PromoID,
            };
            $http
                .post('exportfeeds/verifyyandexpromo', {
                    exportFeedId: ctrl.ExportFeedId,
                    model: PromoCode,
                    editing: ctrl.mode == 'edit',
                })
                .then(function (responce) {
                    if (responce.data != null) {
                        if (responce.data.result == true) {
                            PromoCode = responce.data.promo;
                            $uibModalInstance.close(PromoCode);
                        } else {
                            toaster.pop('error', 'Ошибка', responce.data.errors);
                            //ctrl.error = responce.data.errors;
                        }
                    }
                });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalAddEditYandexPromoCodeCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate', '$window'];

    ng.module('uiModal').controller('ModalAddEditYandexPromoCodeCtrl', ModalAddEditYandexPromoCodeCtrl);
})(window.angular);
