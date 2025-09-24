import ddeliveryTemplate from './ddelivery.tpl.html';
(function (ng) {
    'use strict';

    ng.module('ddelivery', [])
        .controller('DdeliveryCtrl', [
            '$http',
            '$scope',
            '$window',
            'modalService',
            'shippingService',
            function ($http, $scope, $window, modalService, shippingService) {
                var ctrl = this;

                ctrl.init = function () {
                    if (!window.isDdeliveryLoaded) {
                        //<script src="https://widgets.saferoute.ru/cart/api.js"></script>

                        //jQuery.getScript("//sdk.ddelivery.ru/assets/ddelivery.js", function () {
                        jQuery.getScript('https://widgets.saferoute.ru/cart/api.js?new', function () {
                            //var divWidgetContainer = $('<div id="widgetDdelivery" style="height:600px;width:600px;"></div>');
                            var divWidgetContainer = $('<div style="width:1000px"><div id="saferoute-cart-widget"></div></div>');
                            modalService.renderModal(
                                'modalDdeliveryWidget',
                                null,
                                divWidgetContainer.prop('outerHTML'),
                                null,
                                {
                                    modalClass: 'ddelivery-widget-dialog',
                                    callbackOpen: 'ddelivery.DDeliveryWidgetInitModal()',
                                },
                                {
                                    ddelivery: {
                                        DDeliveryWidgetInitModal: function () {
                                            ctrl.ddeliveryInitWidget();
                                        },
                                    },
                                },
                            );
                            window.isDdeliveryLoaded = true;
                        });
                    } else {
                        ctrl.ddeliveryInitWidget();
                    }
                };

                ctrl.ddeliveryChange = function (data) {
                    if (typeof delivery === 'string') {
                        delivery = new Function('return ' + delivery)();
                    }

                    if (ctrl.prevDelivery !== data.delivery && data.delivery != null) {
                        ctrl.prevDelivery = data.delivery;
                        modalService.close('modalDdeliveryWidget');
                    } else {
                        return;
                    }

                    if (data.delivery.type == 1 && data.delivery.point != null) {
                        var additionalData = {
                            Code: data.delivery.point.id,
                            DeliveryTypeId: data.delivery.type,
                            Address: data.delivery.point.address,
                            Rate: data.delivery.point.price_delivery || data.delivery.point.priceDelivery,
                            DeliveryDate: data.delivery.point.delivery_date || data.delivery.point.deliveryDate,
                            DeliveryCompanyId: data.delivery.point.delivery_company_id || data.delivery.point.deliveryCompanyId,
                            CityId: data.delivery.point.city_id || data.delivery.point.cityId,
                            City: data.delivery.point.city_name || data.delivery.point.cityName,
                        };

                        ctrl.ddeliveryDeliveryShipping.PickpointId = data.delivery.point.id;
                        ctrl.ddeliveryDeliveryShipping.PickpointAddress = data.delivery.point.address;
                        ctrl.ddeliveryDeliveryShipping.PickpointAdditionalData = JSON.stringify(additionalData);
                        ctrl.ddeliveryDeliveryShipping.Rate = data.delivery.point.price_delivery || data.delivery.point.priceDelivery;
                        ctrl.ddeliveryDeliveryCost = data.delivery.point.price_delivery || data.delivery.point.priceDelivery;
                    }

                    if (data.delivery.type == 2 || data.delivery.type == 3) {
                        additionalData = {
                            DeliveryDate: data.delivery.delivery_date || data.delivery.deliveryDate,
                            DeliveryDays: data.delivery.delivery_days || data.delivery.deliveryDays,
                            DeliveryTypeId: data.delivery.type,
                            DeliveryCompanyId: data.delivery.delivery_company_id || data.delivery.deliveryCompanyId,
                            PickupCompanyId: data.delivery.pickup_company_id || data.delivery.pickupCompanyId,
                            Rate: data.delivery.total_price || data.delivery.totalPrice,
                            CityId: data.city.id,
                            City: data.city.name,
                            ContactFullName: data.contacts.fullName,
                            ContactPhone: data.contacts.phone,
                            ContactIndex: data.contacts.address.index,
                            ContactStreet: data.contacts.address.street,
                            ContactHouse: data.contacts.address.house,
                            ContactFlat: data.contacts.address.flat,
                        };

                        ctrl.ddeliveryDeliveryShipping.PickpointId = -1;
                        ctrl.ddeliveryDeliveryShipping.PickpointAddress =
                            data.city.name +
                            ',' +
                            (data.contacts.address.index !== null ? data.contacts.address.index + ',' : '') +
                            data.contacts.address.street +
                            ',' +
                            data.contacts.address.house +
                            ',' +
                            data.contacts.address.flat;

                        ctrl.ddeliveryDeliveryShipping.PickpointAdditionalData = JSON.stringify(additionalData);
                        ctrl.ddeliveryDeliveryShipping.Rate = data.delivery.total_price || data.delivery.totalPrice;
                        ctrl.ddeliveryDeliveryCost = data.delivery.total_price || data.delivery.totalPrice;

                        $http.post('checkout/GetCheckoutUser').then(function (response) {
                            if (response.data.obj !== null && response.data.obj.Customer !== null) {
                                var checkoutUserData = response.data.obj;
                                //обновляем данные пользователя
                                checkoutUserData.Customer.Phone = data.contacts.phone;
                                var fullname = data.contacts.fullName.split(' ');
                                if (fullname.length > 0) {
                                    checkoutUserData.Customer.LastName = fullname[0];
                                }
                                if (fullname.length > 1) {
                                    checkoutUserData.Customer.FirstName = fullname[1];
                                }
                                if (fullname.length > 2) {
                                    checkoutUserData.Customer.Patronymic = fullname[2];
                                }

                                $http.post('checkout/CheckoutUserPost', { customer: checkoutUserData.Customer }).then(function (response) {
                                    if (checkoutUserData.Data.Contact !== null && response.data) {
                                        //обновляем данные адреса клиента
                                        checkoutUserData.Data.Contact.Zip = data.contacts.address.index;
                                        checkoutUserData.Data.Contact.City = data.city.name.replace('г. ', '');
                                        $http
                                            .post('checkout/CheckoutContactPost', {
                                                address: checkoutUserData.Data.Contact,
                                            })
                                            .then(function (response) {});
                                    }
                                });
                            }
                        });
                    }

                    ctrl.ddeliveryDeliveryCallback({
                        event: 'ddeliveryDelivery',
                        field: ctrl.ddeliveryDeliveryShipping.PickpointId || 0,
                        shipping: ctrl.ddeliveryDeliveryShipping,
                    });

                    $scope.$digest();
                };

                ctrl.ddeliveryInitWidget = function () {
                    var url = window.location.href.split('?')[0].split('#')[0].replace('/checkout/lp', '/checkout');

                    if (url.indexOf('/adminv2/') != -1) {
                        url = url.split('/adminv2/')[0] + '/checkout';
                    }

                    var widget = new SafeRouteCartWidget('saferoute-cart-widget', {
                        apiScript:
                            url +
                            '/DdeliveryRequest/?id=' +
                            ctrl.ddeliveryDeliveryShipping.MethodId +
                            '&nppOption=' +
                            ctrl.ddeliveryWidgetConfigData.nppOption,
                        products: ctrl.ddeliveryWidgetConfigData.products,
                        regionName: ctrl.ddeliveryWidgetConfigData.regionName,
                        //stopSubmit: ctrl.ddeliveryWidgetConfigData.stopSubmit,
                        userFullName: ctrl.ddeliveryWidgetConfigData.userFullName,
                        userPhone: ctrl.ddeliveryWidgetConfigData.userPhone,
                        itemCount: ctrl.ddeliveryWidgetConfigData.itemCount,
                        width: ctrl.ddeliveryWidgetConfigData.width,
                        height: ctrl.ddeliveryWidgetConfigData.height,
                        length: ctrl.ddeliveryWidgetConfigData.length,
                        weight: ctrl.ddeliveryWidgetConfigData.weight,
                        nppOption: ctrl.ddeliveryWidgetConfigData.nppOption,
                        priceDeclared: ctrl.ddeliveryWidgetConfigData.priceDeclared,
                    });

                    // Обработчики событий
                    widget.on('change', function (response) {
                        ctrl.ddeliveryChange(response);
                    });

                    shippingService.fireTemplateReady($scope);
                };
            },
        ])

        .directive('ddelivery', [
            'urlHelper',
            function (urlHelper) {
                return {
                    require: ['ddelivery', '^shippingList'],
                    scope: {
                        ddeliveryDeliveryShipping: '=',
                        ddeliveryCallback: '&',
                        ddeliveryDeliveryCallback: '&',
                        ddeliveryOpenWidget: '&',
                        ddeliveryWidgetConfigData: '=',
                        ddeliveryIsSelected: '=',
                        ddeliveryContact: '=',

                        ddeliveryDeliveryAmount: '=',
                        ddeliveryDeliveryWeight: '=',
                        ddeliveryDeliveryCost: '=',
                        ddeliveryDeliveryDimensions: '=',
                    },
                    controller: 'DdeliveryCtrl',
                    controllerAs: 'ddelivery',
                    bindToController: true,
                    templateUrl: ddeliveryTemplate,
                    link: function (scope, element, attrs, ctrls) {
                        var ddelivery = ctrls[0];
                        var shippingList = ctrls[1];

                        ddelivery.init();

                        shippingList.addCallbackOnLoad(ddelivery.init);
                    },
                };
            },
        ]);
})(window.angular);
