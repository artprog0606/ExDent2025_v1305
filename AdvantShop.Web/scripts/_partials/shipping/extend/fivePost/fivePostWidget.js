import fivePostWidgetTemplate from './fivePostWidget.tpl.html';

(function (ng) {
    'use strict';
    ng.module('fivePostWidget', [])
        .controller('fivePostWidgetCtrl', [
            '$http',
            '$scope',
            'shippingService',
            'modalService',
            'checkoutService',
            'zoneService',
            function ($http, $scope, shippingService, modalService, checkoutService, zoneService) {
                var ctrl = this;

                ctrl.init = function () {
                    ctrl.modalName = 'modalFivePostWidget' + ctrl.fivePostShipping.MethodId;
                    ctrl.widgetDivId = 'map' + ctrl.fivePostShipping.MethodId;
                    if (!window.isFivePostWidgetLoaded) {
                        jQuery.ajax({
                            dataType: 'script',
                            cache: false,
                            url: 'https://fivepost.ru/static/5post-widget-v1.0.js',
                        });
                        window.isFivePostWidgetLoaded = true;
                    }

                    ctrl.initWidget();
                };

                ctrl.initWidget = function () {
                    shippingService.fireTemplateReady($scope);

                    var divWidgetContainer = $('<div><div style="height:500px;width:1000px" id="' + ctrl.widgetDivId + '"></div></div>');
                    modalService.renderModal(
                        ctrl.modalName,
                        null,
                        divWidgetContainer.prop('outerHTML'),
                        null,
                        {
                            callbackOpen: 'fivePostWidget.FivePostWidgetOpenModal()',
                            callbackClose: 'fivePostWidget.FivePostWidgetCloseModal()',
                        },
                        {
                            fivePostWidget: {
                                FivePostWidgetOpenModal: function () {
                                    ctrl.startWidget();
                                },
                                FivePostWidgetCloseModal: function () {
                                    ctrl.fivepostMap.destroy();
                                },
                            },
                        },
                    );

                    shippingService.fireTemplateReady($scope);
                };

                ctrl.startWidget = function () {
                    ctrl.fivepostMap = new fivepost.PickupPointsMap({
                        apikey: ctrl.fivePostWidgetConfigData.widgetKey,
                        target: '#' + ctrl.widgetDivId,
                        mapCenter: ctrl.fivePostWidgetConfigData.mapCenter,
                        onSelectPoint: (point) => {
                            ctrl.processPoint(point);
                        },
                    });
                };

                ctrl.processPoint = function (data) {
                    if (typeof data === 'string') {
                        data = new Function('return ' + delivery)();
                    }

                    var selectedPoint = [];

                    ctrl.fivePostShipping.PickpointId = data.id;
                    ctrl.fivePostShipping.PickpointAddress = data.fullAddress;

                    var pickPointCity = data.city;
                    var pickPointRegion = data.region;

                    if (!ctrl.fivePostIsAdmin && pickPointCity.toLowerCase() !== ctrl.fivePostWidgetConfigData.city.toLowerCase()) {
                        if (ctrl.fivePostContact.ContactId) {
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
                                                ctrl.fivePostCallback({
                                                    event: 'fivePostWidget',
                                                    field: ctrl.fivePostShipping.PickpointId || 0,
                                                    shipping: ctrl.fivePostShipping,
                                                });
                                            });
                                    }
                                }
                            });
                        } else {
                            var beforeShipping = ng.copy(ctrl.fivePostShipping);
                            var callBackFunction = function () {
                                ctrl.fivePostShipping.PickpointId = beforeShipping.PickpointId;
                                ctrl.fivePostShipping.PickpointAddress = beforeShipping.PickpointAddress;

                                ctrl.fivePostCallback({
                                    event: 'fivePostWidget',
                                    field: ctrl.fivePostShipping.PickpointId || 0,
                                    shipping: ctrl.fivePostShipping,
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
                        ctrl.fivePostCallback({
                            event: 'fivePostWidget',
                            field: ctrl.fivePostShipping.PickpointId || 0,
                            shipping: ctrl.fivePostShipping,
                        });
                    }

                    document.removeEventListener('FivePostWidgetPointSelected', function (data) {
                        ctrl.processPoint(data.detail);
                    });

                    modalService.close(ctrl.modalName);
                };
            },
        ])
        .directive('fivePostWidget', [
            'urlHelper',
            function (urlHelper) {
                return {
                    scope: {
                        fivePostShipping: '=',
                        fivePostCallback: '&',
                        fivePostContact: '=',
                        fivePostIsAdmin: '<?',
                        fivePostWidgetConfigData: '=',
                    },
                    controller: 'fivePostWidgetCtrl',
                    controllerAs: 'fivePostWidget',
                    bindToController: true,
                    templateUrl: fivePostWidgetTemplate,
                    link: function (scope, element, attrs, ctrl) {
                        ctrl.init();
                    },
                };
            },
        ]);
})(window.angular);
