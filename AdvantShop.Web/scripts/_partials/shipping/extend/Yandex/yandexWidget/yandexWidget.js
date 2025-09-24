import yandexWidgetTemplate from './yandexWidget.tpl.html';

(function (ng) {
    'use strict';
    //https://conscious-woodwind-547.notion.site/8da2db9cbb214498a7c8e5037e0c84ae
    ng.module('yandexDeliveryWidget', [])
        .controller('YandexDeliveryWidgetCtrl', [
            '$http',
            '$scope',
            'shippingService',
            'modalService',
            'checkoutService',
            'zoneService',
            function ($http, $scope, shippingService, modalService, checkoutService, zoneService) {
                var ctrl = this;

                ctrl.init = function () {
                    ctrl.modalName = 'modalYandexWidget' + ctrl.yandexShipping.MethodId;
                    ctrl.widgetDivId = 'yaWidget' + ctrl.yandexShipping.MethodId;
                    if (!window.isWidgetLoaded) {
                        jQuery.ajax({
                            dataType: 'script',
                            cache: false,
                            url: 'https://widget-pvz.dostavka.yandex.net/widget.js',
                        });
                        window.isWidgetLoaded = true;
                    }

                    ctrl.initWidget();
                };

                ctrl.initWidget = function () {
                    shippingService.fireTemplateReady($scope);

                    var divWidgetContainer = $('<div style="height:500px;width:1000px"><div id="' + ctrl.widgetDivId + '"></div></div>');
                    modalService.renderModal(
                        ctrl.modalName,
                        null,
                        divWidgetContainer.prop('outerHTML'),
                        null,
                        {
                            callbackOpen: 'yandexWidget.YandexWidgetOpenModal()',
                        },
                        {
                            yandexWidget: {
                                YandexWidgetOpenModal: function () {
                                    window.YaDelivery ? ctrl.startWidget() : document.addEventListener('YaNddWidgetLoad', ctrl.startWidget);
                                },
                            },
                        },
                    );

                    document.addEventListener('YaNddWidgetPointSelected', function (data) {
                        ctrl.processPoint(data.detail);
                    });

                    shippingService.fireTemplateReady($scope);
                };

                ctrl.startWidget = function () {
                    var deliveryTypes = [];
                    if (ctrl.yandexWidgetConfigData.deliveryPVZ) {
                        deliveryTypes.push('pickup_point');
                    }
                    if (ctrl.yandexWidgetConfigData.deliveryPostamat) {
                        deliveryTypes.push('terminal');
                    }

                    var containerId = ctrl.widgetDivId; // Идентификатор HTML-элемента (контейнера),
                    var params = {
                        city: ctrl.yandexWidgetConfigData.city,
                        size: {
                            height: '500px',
                            width: '100%',
                        },
                        show_select_button: true, // Отображение кнопки выбора ПВЗ (false - скрыть кнопку, true - показать кнопку)
                        filter: {
                            type: deliveryTypes,
                            // Способ оплаты
                            payment_methods: [
                                'already_paid', // Доступен для доставки предоплаченных заказов
                                'card_on_receipt', // Доступна оплата картой при получении
                            ],
                        },
                    };

                    //window.document.getElementById(ctrl.widgetDivId).innerHTML = '';

                    window.YaDelivery.createWidget({
                        containerId: containerId,
                        params: params,
                    });
                };

                ctrl.processPoint = function (data) {
                    if (typeof data === 'string') {
                        data = new Function('return ' + delivery)();
                    }

                    // var selectedPoint = [];

                    ctrl.yandexShipping.PickpointId = data.id;
                    ctrl.yandexShipping.PickpointAddress = data.address.full_address;

                    // selectedPoint.Address = data.address.full_address;
                    // selectedPoint.Description = data.address.comment;
                    // selectedPoint.Code = data.id;
                    //
                    // ctrl.yandexShipping.SelectedPoint = selectedPoint;
                    var pickPointCity = data.address.locality;
                    var pickPointRegion = data.address.region;

                    if (!ctrl.yandexIsAdmin && pickPointCity.toLowerCase() !== ctrl.yandexWidgetConfigData.city.toLowerCase()) {
                        if (ctrl.yandexContact.ContactId) {
                            //зарегенный пользователь
                            $http.post('checkout/GetCheckoutUser').then(function (response) {
                                if (response.data.obj !== null && response.data.obj.Data !== null) {
                                    var checkoutUserData = response.data.obj;
                                    if (checkoutUserData.Data.Contact !== null) {
                                        //обновляем данные адреса клиента
                                        checkoutUserData.Data.Contact.City = pickPointCity;
                                        checkoutUserData.Data.Contact.Region = pickPointRegion;

                                        $http
                                            .post('checkout/CheckoutContactPost', {
                                                address: checkoutUserData.Data.Contact,
                                            })
                                            .then(function (response) {
                                                ctrl.yandexCallback({
                                                    event: 'yandexWidget',
                                                    field: ctrl.yandexShipping.PickpointId || 0,
                                                    shipping: ctrl.yandexShipping,
                                                });
                                            });
                                    }
                                }
                            });
                        } else {
                            var beforeShipping = ng.copy(ctrl.yandexShipping);
                            var callBackFunction = function () {
                                ctrl.yandexShipping.PickpointId = beforeShipping.PickpointId;
                                ctrl.yandexShipping.PickpointAddress = beforeShipping.PickpointAddress;
                                // ctrl.yandexShipping.SelectedPoint = beforeShipping.SelectedPoint;

                                ctrl.yandexCallback({
                                    event: 'yandexWidget',
                                    field: ctrl.yandexShipping.PickpointId || 0,
                                    shipping: ctrl.yandexShipping,
                                });
                                checkoutService.removeCallback('address', callBackFunction);
                            };

                            // после setCurrentZone сработает обновление списка доставок в checkout,
                            // по завершению чего будет вызван Callback 'address'
                            checkoutService.addCallback('address', callBackFunction);
                            zoneService.getCurrentZone().then(function (zoneData) {
                                zoneService.setCurrentZone(pickPointCity, null, zoneData.CountryId, pickPointRegion, zoneData.CountryName, null);
                            });
                        }
                    } else {
                        ctrl.yandexCallback({
                            event: 'yandexWidget',
                            field: ctrl.yandexShipping.PickpointId || 0,
                            shipping: ctrl.yandexShipping,
                        });
                    }

                    document.removeEventListener('YaNddWidgetPointSelected', function (data) {
                        ctrl.processPoint(data.detail);
                    });

                    modalService.close(ctrl.modalName);
                    //$scope.$digest();
                };
            },
        ])
        .directive('yandex', [
            'urlHelper',
            function (urlHelper) {
                return {
                    scope: {
                        yandexShipping: '=',
                        yandexCallback: '&',
                        yandexContact: '=',
                        yandexIsAdmin: '<?',
                        yandexIsSelected: '=',
                        yandexWidgetConfigData: '=',
                    },
                    controller: 'YandexDeliveryWidgetCtrl',
                    controllerAs: 'yandexDeliveryWidget',
                    bindToController: true,
                    templateUrl: yandexWidgetTemplate,
                    link: function (scope, element, attrs, ctrl) {
                        ctrl.init();
                    },
                };
            },
        ]);
})(window.angular);
