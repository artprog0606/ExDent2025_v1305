(function (ng) {
    'use strict';

    var ModalAddEditCouponCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.CouponId = params.CouponId != null ? params.CouponId : 0;
            ctrl.TriggerActionId = params.triggerActionId != null ? params.triggerActionId : null;
            ctrl.TriggerId = params.triggerId != null ? params.triggerId : null;
            ctrl.CouponMode = params.couponMode != null ? params.couponMode : 0;

            ctrl.mode = ctrl.CouponId != 0 ? 'edit' : 'add';

            ctrl.ShowUseStartDate = !ctrl.isTemplate();

            ctrl.getCouponData().then(function () {
                if (ctrl.mode == 'edit') {
                    ctrl.getCoupon(ctrl.CouponId);
                } else {
                    ctrl.getCouponCode();
                    ctrl.Value = 0;
                    ctrl.MinimalOrderPrice = 0;
                    ctrl.Enabled = true;
                    ctrl.IsMinimalOrderPriceFromAllCart = false;
                    ctrl.UsePosibleUses = true;
                    ctrl.UseExpirationDate = !ctrl.isTemplate() || ctrl.CouponMode == 4 ? true : false;
                    ctrl.Type = ctrl.Types[0];
                    ctrl.CurrencyIso3 = ctrl.Currencies[0];
                    ctrl.AddingDate = new Date();
                    ctrl.CategoryIds = [];
                    ctrl.ProductsIds = [];
                    ctrl.CustomerGroupIds = [];
                    ctrl.Days = 14;
                    ctrl.UseStartDate = true; //!ctrl.isTemplate() ? true : false;
                    ctrl.ForFirstOrder = false;
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getCoupon = function (id) {
            $http.get('coupons/getCoupon', { params: { couponId: id } }).then(function (response) {
                var data = response.data;
                if (data != null) {
                    ctrl.CouponId = data.CouponId;
                    ctrl.Code = data.Code;
                    ctrl.Value = data.Value;
                    ctrl.Type = ctrl.Types.filter(function (x) {
                        return x.value == data.Type;
                    })[0];
                    ctrl.PossibleUses = data.PossibleUses;
                    ctrl.UsePosibleUses = ctrl.PossibleUses == 0;
                    ctrl.ForFirstOrder = data.ForFirstOrder;

                    ctrl.AddingDate = data.AddingDate;
                    ctrl.AddingDateFormatted = data.AddingDateFormatted;

                    ctrl.CurrencyIso3 = ctrl.Currencies.filter(function (x) {
                        return x.value == data.CurrencyIso3;
                    })[0];

                    ctrl.Enabled = data.Enabled;
                    ctrl.MinimalOrderPrice = data.MinimalOrderPrice;
                    ctrl.IsMinimalOrderPriceFromAllCart = data.IsMinimalOrderPriceFromAllCart;
                    ctrl.ActualUses = data.ActualUses;
                    ctrl.CategoryIds = data.CategoryIds;
                    ctrl.ProductsIds = data.ProductsIds;
                    ctrl.TriggerActionId = data.TriggerActionId;
                    ctrl.TriggerId = data.TriggerId;
                    ctrl.TriggerName = data.TriggerName;

                    ctrl.CouponMode = data.Mode;
                    ctrl.Days = data.Days || 14;

                    ctrl.ExpirationDate = data.ExpirationDate;
                    ctrl.UseExpirationDate = data.ExpirationDate == null && data.Days == null;

                    ctrl.PartnerId = data.PartnerId;
                    ctrl.PartnerName = data.PartnerName;

                    ctrl.StartDate = data.StartDate;
                    ctrl.UseStartDate = data.StartDate == null;

                    ctrl.CustomerGroupIds = data.CustomerGroupIds;

                    ctrl.GiftOfferId = data.GiftOfferId;
                    ctrl.GiftOfferError = data.GiftOfferError;
                    ctrl.OnlyInMobileApp = data.OnlyInMobileApp;
                    ctrl.ForFirstOrderInMobileApp = data.ForFirstOrderInMobileApp;
                    ctrl.OnlyOnCustomerBirthday = data.OnlyOnCustomerBirthday;
                    ctrl.DaysBeforeBirthday = data.DaysBeforeBirthday;
                    ctrl.DaysAfterBirthday = data.DaysAfterBirthday;
                    ctrl.Comment = data.Comment;
                    ctrl.IsAppliedToPriceWithDiscount = data.IsAppliedToPriceWithDiscount;
                }
            });
        };

        ctrl.getCouponCode = function (id) {
            return $http.get('coupons/getCouponCode', { params: { couponId: id } }).then(function (response) {
                if (ctrl.CouponMode == 4) {
                    ctrl.Code = 'Шаблон партнерского купона';
                } else {
                    ctrl.Code = (ctrl.isTemplate() ? 'Шаблон купона ' : '') + response.data.code;
                }
            });
        };

        ctrl.getCouponData = function () {
            return $http.get('coupons/getCouponData').then(function (response) {
                var data = response.data;

                ctrl.Types = data.types;
                ctrl.Currencies = data.currencies;
                ctrl.AddingDateFormatted = data.dateNow;
                ctrl.CustomerGroupList = data.customerGroupList;
            });
        };

        ctrl.selectCategories = function (result) {
            ctrl.CategoryIds = result.categoryIds;
        };

        ctrl.selectProducts = function () {
            $http.get('coupons/getCouponProducts', { params: { couponId: ctrl.CouponId, rnd: Math.random() } }).then(function (response) {
                ctrl.ProductsIds = response.data.ids;
            });
        };

        ctrl.resetCategories = function () {
            if (ctrl.mode === 'add') {
                ctrl.CategoryIds = [];
            } else {
                $http.post('coupons/resetCouponCategories', { couponId: ctrl.CouponId }).then(function (response) {
                    ctrl.CategoryIds = [];
                });
            }
        };

        ctrl.resetProducts = function () {
            if (ctrl.mode === 'add') {
                ctrl.ProductsIds = [];
            } else {
                $http.post('coupons/resetCouponProducts', { couponId: ctrl.CouponId }).then(function (response) {
                    ctrl.ProductsIds = [];
                });
            }
        };

        ctrl.save = function () {
            var params = {
                CouponId: ctrl.CouponId,
                Code: ctrl.Code,
                Value: ctrl.Value,
                Type: ctrl.Type.value,
                PossibleUses: !ctrl.UsePosibleUses ? ctrl.PossibleUses : 0,
                ExpirationDate: !ctrl.UseExpirationDate && !ctrl.isTemplate() ? ctrl.ExpirationDate : null,
                AddingDate: ctrl.AddingDate,
                CurrencyIso3: ctrl.CurrencyIso3.value,
                Enabled: ctrl.Enabled,
                MinimalOrderPrice: ctrl.MinimalOrderPrice,
                IsMinimalOrderPriceFromAllCart: ctrl.IsMinimalOrderPriceFromAllCart,
                CategoryIds: ctrl.CategoryIds,
                ProductsIds: ctrl.ProductsIds,
                TriggerActionId: ctrl.TriggerActionId,
                TriggerId: ctrl.TriggerId,
                Mode: ctrl.CouponMode,
                Days: !ctrl.UseExpirationDate && ctrl.isTemplate() ? ctrl.Days : null,
                StartDate: !ctrl.UseStartDate && !ctrl.isTemplate() ? ctrl.StartDate : null,
                ForFirstOrder: ctrl.ForFirstOrder,
                CustomerGroupIds: ctrl.CustomerGroupIds,
                GiftOfferId: ctrl.GiftOfferId,
                OnlyInMobileApp: ctrl.OnlyInMobileApp,
                ForFirstOrderInMobileApp: ctrl.ForFirstOrderInMobileApp,
                OnlyOnCustomerBirthday: ctrl.OnlyOnCustomerBirthday,
                DaysBeforeBirthday: ctrl.DaysBeforeBirthday,
                DaysAfterBirthday: ctrl.DaysAfterBirthday,
                Comment: ctrl.Comment,
                IsAppliedToPriceWithDiscount: ctrl.IsAppliedToPriceWithDiscount,
            };

            var url = ctrl.mode == 'add' ? 'coupons/addCoupon' : 'coupons/updateCoupon';

            $http.post(url, params).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Coupons.ChangesSuccessfullySaved'));
                    $uibModalInstance.close(data.obj);
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.Coupons.Error'), data.errors);
                }
            });
        };

        ctrl.isTemplate = function () {
            return ctrl.CouponMode == 1 || ctrl.CouponMode == 3 || ctrl.CouponMode == 4;
        };

        ctrl.selectOffer = function (result) {
            ctrl.GiftOfferError = null;

            if (result && result.ids) {
                ctrl.GiftOfferId = result.ids[0];
            } else toaster.pop('error', 'Не удалось выбрать товар');
        };

        ctrl.checkSelectedItem = function (rowEntity) {
            return ctrl.GiftOfferId === rowEntity.OfferId;
        };
    };

    ModalAddEditCouponCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalAddEditCouponCtrl', ModalAddEditCouponCtrl);
})(window.angular);
