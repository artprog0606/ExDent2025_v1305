import grastinTemplate from './grastin.tpl.html';
(function (ng) {
    'use strict';

    angular
        .module('grastin', [])
        .controller('GrastinCtrl', [
            '$http',
            '$scope',
            'modalService',
            'zoneService',
            '$element',
            'checkoutService',
            'shippingService',
            function ($http, $scope, modalService, zoneService, $element, checkoutService, shippingService) {
                var ctrl = this;

                ctrl.$onInit = function () {
                    $element.on('$destroy', function () {
                        modalService.destroy(ctrl.modalId);
                    });
                };

                ctrl.init = function () {
                    ctrl.modalId = 'modalGrastinWidget' + ctrl.grastinShipping.MethodId;
                    if (!window.isGrastinLoaded) {
                        window.isGrastinLoaded = true;

                        jQuery.ajax({
                            dataType: 'script',
                            cache: true,
                            url: 'https://grastin.ru/widgets/delivery_widget/js/gWidget.js',
                        });
                    }

                    ctrl.initModal();
                };

                ctrl.initModal = function () {
                    shippingService.fireTemplateReady($scope);

                    var divgWidgetContainer = $('<div class="gWidget" style="height:500px;"></div>');
                    Object.keys(ctrl.grastinWidgetConfigData).map(function (objectKey) {
                        divgWidgetContainer.attr(objectKey, ctrl.grastinWidgetConfigData[objectKey]);
                    });

                    modalService.renderModal(
                        ctrl.modalId,
                        null,
                        divgWidgetContainer.prop('outerHTML'),
                        '<button type="button" data-modal-close="" class="btn btn-xsmall btn-submit">Выбрать</button>',
                        {
                            modalClass: 'shipping-dialog',
                            callbackInit: 'grastin.GrastinWidgetInitModal()',
                            callbackOpen: 'grastin.GrastinWidgetOpenModal()',
                            callbackClose: 'grastin.GrastinWidgetCloseModal()',
                        },
                        {
                            grastin: {
                                GrastinWidgetInitModal: function () {
                                    if (window.gwClient) {
                                        gwClient.createWidgets();
                                        ctrl.createdWidgets = true;
                                    }
                                },
                                GrastinWidgetOpenModal: function () {
                                    if (ctrl.beforeGrastinWidgetConfigData == null) {
                                        ctrl.beforeGrastinWidgetConfigData = angular.copy(ctrl.grastinWidgetConfigData);
                                    }
                                    if (!angular.equals(ctrl.beforeGrastinWidgetConfigData, ctrl.grastinWidgetConfigData)) {
                                        var divgWidgetContainer = $('<div class="gWidget" style="height:500px;"></div>');
                                        Object.keys(ctrl.grastinWidgetConfigData).map(function (objectKey) {
                                            divgWidgetContainer.attr(objectKey, ctrl.grastinWidgetConfigData[objectKey]);
                                        });

                                        $('#' + ctrl.modalId)
                                            .find('.gWidget')
                                            .replaceWith(divgWidgetContainer.prop('outerHTML'));
                                        gwClient.createWidgets();
                                        ctrl.createdWidgets = true;

                                        ctrl.beforeGrastinWidgetConfigData = angular.copy(ctrl.grastinWidgetConfigData);
                                    } else if (!ctrl.createdWidgets) {
                                        gwClient.createWidgets();
                                        ctrl.createdWidgets = true;
                                    }

                                    window.grastinPvzWidgetCallback = ctrl.setGrastinPvz;
                                },
                                GrastinWidgetCloseModal: function () {
                                    window.grastinPvzWidgetCallback = null;
                                },
                            },
                        },
                    );
                };

                ctrl.setGrastinPvz = function (delivery) {
                    if (typeof delivery === 'string') {
                        delivery = new Function('return ' + delivery)();
                    }

                    var closeModal = true;
                    //                                                Если почта, то не обязательно данные по точке самовыова (их нет)
                    if (
                        delivery.cost <= 0 ||
                        !delivery.partnerId ||
                        (delivery.partnerId !== 'post' && delivery.deliveryType === 'pvz' && (!delivery.currentId || !delivery.pvzData))
                    )
                        return;

                    var deliveryClone = JSON.parse(JSON.stringify(delivery));
                    deliveryClone.partnerId = undefined;

                    if (delivery.deliveryType === 'courier') {
                        deliveryClone.deliveryType = 1;
                    } else if (delivery.deliveryType === 'pvz') {
                        deliveryClone.deliveryType = 2;
                    }

                    var company = '';
                    if (delivery.partnerId === 'grastin') {
                        company = 'Grastin';
                        deliveryClone.partner = 1;
                    } else if (delivery.partnerId === 'dpd') {
                        company = 'DPD';
                        deliveryClone.partner = 5;
                    } else if (delivery.partnerId === 'hermes') {
                        company = 'Hermes';
                        deliveryClone.partner = 2;
                    } else if (delivery.partnerId === 'boxberry') {
                        company = 'BoxBerry';
                        deliveryClone.partner = 4;
                    } else if (delivery.partnerId === 'post') {
                        company = 'Почта РФ';
                        deliveryClone.partner = 3;
                    } else if (delivery.partnerId === 'partner') {
                        company = 'Grastin партнер';
                        deliveryClone.partner = 6;
                    } else if (delivery.partnerId === 'cdekcourier' || delivery.partnerId === 'cdekpikup' || delivery.partnerId === 'cdekpostamat') {
                        company = 'СДЭК';
                        deliveryClone.partner = 7;
                    } else if (delivery.partnerId) {
                        company = delivery.partnerId;
                    }

                    if (delivery.partnerId === 'post') {
                        // Особый случай с почтой
                        ctrl.grastinShipping.PickpointId = delivery.partnerId;
                        ctrl.grastinShipping.PickpointAddress = delivery.cityTo;
                        ctrl.grastinShipping.PickpointAdditionalData = JSON.stringify(deliveryClone);
                        ctrl.grastinShipping.PickpointAdditionalDataObj = deliveryClone;
                        ctrl.grastinShipping.NameRate = 'Самовывоз ' + company;
                        ctrl.grastinShipping.Rate = delivery.cost;
                        ctrl.grastinShipping.DeliveryTime = ''; //(delivery.time && delivery.time > 0 ? delivery.time + " д." : "");
                    } else if (delivery.deliveryType === 'pvz') {
                        ctrl.grastinShipping.PickpointId = delivery.currentId;
                        ctrl.grastinShipping.PickpointAddress = delivery.pvzData.name;
                        ctrl.grastinShipping.PickpointAdditionalData = JSON.stringify(deliveryClone);
                        ctrl.grastinShipping.PickpointAdditionalDataObj = deliveryClone;
                        ctrl.grastinShipping.NameRate = 'Самовывоз ' + company;
                        ctrl.grastinShipping.Rate = delivery.cost;
                        ctrl.grastinShipping.DeliveryTime = ''; //(delivery.time && delivery.time > 0 ? delivery.time + " д." : "");
                    } else {
                        ctrl.grastinShipping.PickpointId = delivery.partnerId;
                        ctrl.grastinShipping.PickpointAddress = delivery.cityTo;
                        ctrl.grastinShipping.PickpointAdditionalData = JSON.stringify(deliveryClone);
                        ctrl.grastinShipping.PickpointAdditionalDataObj = deliveryClone;
                        ctrl.grastinShipping.NameRate = 'Курьерская доставка ' + company;
                        ctrl.grastinShipping.Rate = delivery.cost;
                        ctrl.grastinShipping.DeliveryTime = ''; //(delivery.time && delivery.time > 0 ? delivery.time + " д." : "");
                    }

                    if (
                        !ctrl.grastinIsAdmin &&
                        delivery.cityTo.toLowerCase() !== (ctrl.grastinWidgetConfigData['data-to-city'] || '').toLowerCase()
                    ) {
                        if (ctrl.grastinContact.ContactId) {
                            //зарегенный пользователь
                            $http.post('checkout/GetCheckoutUser').then(function (response) {
                                if (response.data.obj !== null && response.data.obj.Data !== null) {
                                    var checkoutUserData = response.data.obj;
                                    if (checkoutUserData.Data.Contact !== null) {
                                        //обновляем данные адреса клиента
                                        checkoutUserData.Data.Contact.City = delivery.cityTo;

                                        $http
                                            .post('checkout/CheckoutContactPost', {
                                                address: checkoutUserData.Data.Contact,
                                            })
                                            .then(function (response) {
                                                ctrl.grastinCallback({
                                                    event: 'grastinWidget',
                                                    field: ctrl.grastinShipping.PickpointId || 0,
                                                    shipping: ctrl.grastinShipping,
                                                });
                                            });
                                    }
                                }
                            });
                        } else {
                            var beforeShipping = angular.copy(ctrl.grastinShipping);
                            var callBackFunction = function () {
                                ctrl.grastinShipping.PickpointId = beforeShipping.PickpointId;
                                ctrl.grastinShipping.PickpointAddress = beforeShipping.PickpointAddress;
                                ctrl.grastinShipping.PickpointAdditionalData = beforeShipping.PickpointAdditionalData;
                                ctrl.grastinShipping.PickpointAdditionalDataObj = beforeShipping.PickpointAdditionalDataObj;
                                ctrl.grastinShipping.NameRate = beforeShipping.NameRate;
                                ctrl.grastinShipping.Rate = beforeShipping.Rate;
                                ctrl.grastinShipping.DeliveryTime = beforeShipping.DeliveryTime;

                                ctrl.grastinCallback({
                                    event: 'grastinWidget',
                                    field: ctrl.grastinShipping.PickpointId || 0,
                                    shipping: ctrl.grastinShipping,
                                });
                                checkoutService.removeCallback('address', callBackFunction);
                            };

                            // после setCurrentZone сработает обновление списка доставок в checkout,
                            // по завершению чего будет вызван Callback 'address'
                            checkoutService.addCallback('address', callBackFunction);
                            zoneService.getCurrentZone().then(function (data) {
                                zoneService.setCurrentZone(delivery.cityTo, null, data.CountryId, null, data.CountryName, null);
                            });
                        }
                    } else {
                        ctrl.grastinCallback({
                            event: 'grastinWidget',
                            field: ctrl.grastinShipping.PickpointId || 0,
                            shipping: ctrl.grastinShipping,
                        });
                    }
                    if (closeModal) {
                        //modalService.close(ctrl.modalId);
                    }

                    //$scope.$digest();
                };
            },
        ])
        .directive('grastin', [
            'urlHelper',
            function (urlHelper) {
                return {
                    scope: {
                        grastinShipping: '=',
                        grastinCallback: '&',
                        grastinWidgetConfigData: '=',
                        grastinIsSelected: '=',
                        grastinContact: '=',
                        grastinIsAdmin: '<?',
                    },
                    controller: 'GrastinCtrl',
                    controllerAs: 'grastin',
                    bindToController: true,
                    templateUrl: grastinTemplate,
                    link: function (scope, element, attrs, ctrl) {
                        ctrl.init();
                    },
                };
            },
        ]);
})(window.angular);
