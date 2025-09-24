(function (ng) {
    'use strict';

    //
    // Используется в редактировании заказа и лида
    //
    var ModalShippingsCtrl = function ($uibModalInstance, $window, toaster, $q, $http, $translate) {
        var ctrl = this;

        ctrl.shippingLoading = true;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.order;
            ctrl.id = params.orderId;
            ctrl.isLead = params.isLead || false;
            ctrl.urlPath = !ctrl.isLead ? 'orders' : 'leads';

            ctrl.contact = {
                Country: params.country,
                Region: params.region,
                District: params.district,
                City: params.city,
                Zip: params.zip, //City - с большой потому что используется в scripts\_partials\shipping\extend\yandexdelivery\yandexdelivery.js
                Street: params.street,
                House: params.house,
                Structure: params.structure,
                Apartment: params.apartment,
                Entrance: params.entrance,
                Floor: params.floor,
            };

            ctrl.getShippings().then(function (data) {
                return ctrl.changeShipping(ctrl.selectShipping);
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getShippings = function () {
            return $http
                .get(ctrl.urlPath + '/getShippings', { params: ng.extend(ctrl.contact, { id: ctrl.id }) })
                .then(function (response) {
                    var data = response.data;
                    if (data != null) {
                        ctrl.shippings = data.Shippings;
                        ctrl.selectShipping = data.SelectShipping;
                        ctrl.customShipping = data.SelectShipping != null && data.SelectShipping.IsCustom ? data.SelectShipping : data.CustomShipping;
                    }
                })
                .finally(function () {
                    ctrl.shippingLoading = false;
                });
        };

        ctrl.changeShipping = function (shipping) {
            if (shipping == null) {
                return;
            }

            ctrl.shippingLoading = true;

            $http
                .post(ctrl.urlPath + '/calculateShipping', ng.extend(ctrl.contact, { id: ctrl.id, shipping: shipping }))
                .then(function (response) {
                    if (response.data != null) {
                        if (shipping != null && shipping.IsCustom === true) {
                            ctrl.selectShipping = ng.extend(shipping, response.data.selectShipping);
                        } else {
                            ctrl.selectShipping = ctrl.getSelectedItem(ctrl.shippings, response.data.selectShipping);
                        }
                    }
                })
                .finally(function () {
                    ctrl.shippingLoading = false;
                });
        };

        ctrl.focusShipping = function (shipping, customShipping) {
            if (
                ctrl.selectShipping != null &&
                shipping != null &&
                ctrl.selectShipping.Id === shipping.Id &&
                ctrl.contact.shipping != null &&
                ctrl.contact.shipping.ManualRate === shipping.ManualRate
            ) {
                return;
            }

            if (shipping != null && shipping.IsCustom === true) {
                ctrl.selectShipping = shipping;
            } else {
                ctrl.selectShipping = ctrl.getSelectedItem(ctrl.shippings, shipping);
            }
            ng.extend(ctrl.contact, { id: ctrl.id, shipping: shipping });
        };

        ctrl.getSelectedItem = function (array, selectedItem) {
            var item;
            if (selectedItem != null) {
                for (var i = array.length - 1; i >= 0; i--) {
                    if (array[i].Id === selectedItem.Id) {
                        //selectedItem имеет заполненные поля какие опции выбраны, поэтому объединяем
                        array[i] = ng.extend(array[i], selectedItem);
                        item = array[i];
                        break;
                    }
                }
            }

            return item;
        };

        ctrl.save = function () {
            if (ctrl.contact.shipping == null) {
                toaster.pop('error', '', $translate.instant('Admin.Js.Order.SelectShippingMethod'));
                ctrl.btnLoading = false;
                return;
            }

            var params = { id: ctrl.id };

            $http.post(ctrl.urlPath + '/saveShipping', ng.extend(ctrl.contact, params)).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.DataSavedSuccessfully'));
                    $uibModalInstance.close({ shipping: ctrl.selectShipping });
                } else {
                    ctrl.btnLoading = false;
                    data.errors.forEach(function (error) {
                        toaster.pop('error', '', error);
                    });
                }
            });
        };
    };

    ModalShippingsCtrl.$inject = ['$uibModalInstance', '$window', 'toaster', '$q', '$http', '$translate'];

    ng.module('uiModal').controller('ModalShippingsCtrl', ModalShippingsCtrl);
})(window.angular);
