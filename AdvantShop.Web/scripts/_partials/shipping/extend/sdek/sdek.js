import sdekTemplate from './sdek.tpl.html';
(function (ng) {
    'use strict';

    angular
        .module('sdek', [])
        .controller('SdekCtrl', [
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
                        if (window.JCSdekWidgetPvz && window.JCSdekWidgetPvz[ctrl.widgetId]) {
                            //window.JCSdekWidgetPvz[ctrl.widgetId].destroy();
                            window.JCSdekWidgetPvz[ctrl.widgetId] = null;
                        }
                    });
                };

                ctrl.init = function () {
                    ctrl.modalId = 'modalSdekWidget' + ctrl.sdekShipping.MethodId;
                    ctrl.widgetId = 'sdek_widget_pvz' + ctrl.sdekShipping.MethodId;
                    if (!window.isSdekLoaded) {
                        window.isSdekLoaded = true;

                        jQuery.ajax({
                            dataType: 'script',
                            cache: true,
                            url: 'https://widget.cdek.ru/widget/widjet.js',
                        });
                    }

                    ctrl.initModal();
                };

                ctrl.initModal = function () {
                    shippingService.fireTemplateReady($scope);

                    var divgWidgetContainer = $('<div id="' + ctrl.widgetId + '" style="height: 500px;"></div>');

                    modalService.renderModal(
                        ctrl.modalId,
                        null,
                        divgWidgetContainer.prop('outerHTML'),
                        null,
                        {
                            modalClass: 'shipping-dialog',
                            callbackInit: 'sdek.SdekWidgetInitModal()',
                            callbackOpen: 'sdek.SdekWidgetOpenModal()',
                            callbackClose: 'sdek.SdekWidgetCloseModal()',
                        },
                        {
                            sdek: {
                                SdekWidgetInitModal: function () {},
                                SdekWidgetOpenModal: function () {
                                    if (ctrl.beforeWidgetConfigParams == null) {
                                        ctrl.beforeWidgetConfigParams = angular.copy(ctrl.sdekShipping.WidgetConfigParams);
                                    }

                                    if (window.JCSdekWidgetPvz && window.JCSdekWidgetPvz[ctrl.widgetId]) {
                                        if (!angular.equals(ctrl.beforeWidgetConfigParams, ctrl.sdekShipping.WidgetConfigParams)) {
                                            window.JCSdekWidgetPvz[ctrl.widgetId].city.set(ctrl.sdekShipping.WidgetConfigParams.defaultCity);
                                            window.JCSdekWidgetPvz[ctrl.widgetId].cargo.reset();
                                            window.JCSdekWidgetPvz[ctrl.widgetId].cargo.add(ctrl.sdekShipping.WidgetConfigParams.goods);
                                            ctrl.beforeWidgetConfigParams = angular.copy(ctrl.sdekShipping.WidgetConfigParams);
                                        } else if (
                                            window.JCSdekWidgetPvz[ctrl.widgetId].city.check(ctrl.sdekShipping.WidgetConfigParams.defaultCity) !==
                                            window.JCSdekWidgetPvz[ctrl.widgetId].city.get()
                                        ) {
                                            window.JCSdekWidgetPvz[ctrl.widgetId].city.set(ctrl.sdekShipping.WidgetConfigParams.defaultCity);
                                        }
                                    } else {
                                        if (!window.JCSdekWidgetPvz) {
                                            window.JCSdekWidgetPvz = {};
                                        }

                                        window.JCSdekWidgetPvz[ctrl.widgetId] = new ISDEKWidjet(
                                            angular.extend({}, ctrl.sdekShipping.WidgetConfigParams, {
                                                path: 'https://widget.cdek.ru/widget/scripts/',
                                                link: ctrl.widgetId,
                                                hidedelt: true,
                                            }),
                                        );
                                        window.JCSdekWidgetPvz[ctrl.widgetId].binders.add(ctrl.setSdekPvz, 'onChoose');
                                    }
                                },
                                SdekWidgetCloseModal: function () {},
                            },
                        },
                    );
                };

                ctrl.setSdekPvz = function (sdekPvz) {
                    if (sdekPvz) {
                        ctrl.sdekShipping.PickpointId = sdekPvz.id;
                        ctrl.sdekShipping.PickpointAddress = sdekPvz.cityName + ' ' + sdekPvz.PVZ.Address;
                        ctrl.sdekShipping.PickpointAdditionalData = JSON.stringify(sdekPvz, function (key, value) {
                            if (typeof value === 'object' && key !== 'PVZ' && key !== 'WeightLim') {
                                return undefined;
                            }
                            return value;
                        });
                        ctrl.sdekShipping.Description =
                            sdekPvz.PVZ.Note || sdekPvz.PVZ.WorkTime ? (sdekPvz.PVZ.Note || '') + ' ' + (sdekPvz.PVZ.WorkTime || '') : null;

                        if (!ctrl.sdekIsAdmin && sdekPvz.cityName.toLowerCase() !== ctrl.sdekShipping.WidgetConfigParams.defaultCity.toLowerCase()) {
                            if (ctrl.sdekContact.ContactId) {
                                //зарегенный пользователь
                                $http.post('checkout/GetCheckoutUser').then(function (response) {
                                    if (response.data.obj !== null && response.data.obj.Data !== null) {
                                        var checkoutUserData = response.data.obj;
                                        if (checkoutUserData.Data.Contact !== null) {
                                            //обновляем данные адреса клиента
                                            checkoutUserData.Data.Contact.City = sdekPvz.cityName;

                                            $http
                                                .post('checkout/CheckoutContactPost', {
                                                    address: checkoutUserData.Data.Contact,
                                                })
                                                .then(function (response) {
                                                    ctrl.sdekCallback({
                                                        event: 'sdekWidget',
                                                        field: ctrl.sdekShipping.PickpointId || '0',
                                                        shipping: ctrl.sdekShipping,
                                                    });
                                                });
                                        }
                                    }
                                });
                            } else {
                                var beforeShipping = angular.copy(ctrl.sdekShipping);
                                var callBackFunction = function () {
                                    ctrl.sdekShipping.PickpointId = beforeShipping.PickpointId;
                                    ctrl.sdekShipping.PickpointAddress = beforeShipping.PickpointAddress;
                                    ctrl.sdekShipping.PickpointAdditionalData = beforeShipping.PickpointAdditionalData;
                                    ctrl.sdekShipping.Description = beforeShipping.Description;

                                    ctrl.sdekCallback({
                                        event: 'sdekWidget',
                                        field: ctrl.sdekShipping.PickpointId || '0',
                                        shipping: ctrl.sdekShipping,
                                    });
                                    checkoutService.removeCallback('address', callBackFunction);
                                };

                                // после setCurrentZone сработает обновление списка доставок в checkout,
                                // по завершению чего будет вызван Callback 'address'
                                checkoutService.addCallback('address', callBackFunction);
                                zoneService.getCurrentZone().then(function (data) {
                                    zoneService.setCurrentZone(sdekPvz.cityName, null, data.CountryId, null, data.CountryName, null);
                                });
                            }
                        } else {
                            ctrl.sdekCallback({
                                event: 'sdekWidget',
                                field: ctrl.sdekShipping.PickpointId || '0',
                                shipping: ctrl.sdekShipping,
                            });
                        }

                        modalService.close(ctrl.modalId);

                        //$scope.$digest();
                    }
                };
            },
        ])
        .directive('sdek', [
            'urlHelper',
            function (urlHelper) {
                return {
                    scope: {
                        sdekShipping: '=',
                        sdekCallback: '&',
                        sdekIsSelected: '=',
                        sdekContact: '=',
                        sdekIsAdmin: '<?',
                    },
                    controller: 'SdekCtrl',
                    controllerAs: 'sdek',
                    bindToController: true,
                    templateUrl: sdekTemplate,
                    link: function (scope, element, attrs, ctrl) {
                        ctrl.init();
                    },
                };
            },
        ]);
})(window.angular);
