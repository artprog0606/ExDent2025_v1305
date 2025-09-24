import hermesTemplate from './hermes.tpl.html';
(function (ng) {
    'use strict';

    ng.module('hermes', [])
        .controller('HermesCtrl', [
            '$http',
            '$scope',
            'modalService',
            '$element',
            'zoneService',
            'checkoutService',
            'shippingService',
            function ($http, $scope, modalService, $element, zoneService, checkoutService, shippingService) {
                var ctrl = this;

                ctrl.$onInit = function () {
                    $element.on('$destroy', function () {
                        modalService.destroy(ctrl.modalId);
                    });
                };

                ctrl.init = function () {
                    ctrl.modalId = 'modalHermesWidget' + ctrl.hermesShipping.MethodId;
                    ctrl.widgetId = 'hermes_widget_pvz' + ctrl.hermesShipping.MethodId;
                    if (!window.isHermesLoaded) {
                        window.isHermesLoaded = true;

                        jQuery.ajax({
                            dataType: 'script',
                            cache: true,
                            url: 'https://pschooser.hermesrussia.ru/lib',
                        });
                    }

                    ctrl.initModal();
                };

                ctrl.initModal = function () {
                    shippingService.fireTemplateReady($scope);

                    var divgWidgetContainer = $('<div id="' + ctrl.widgetId + '" style="height:500px;"></div>');

                    modalService.renderModal(
                        ctrl.modalId,
                        null,
                        divgWidgetContainer.prop('outerHTML'),
                        null,
                        {
                            modalClass: 'shipping-dialog',
                            callbackInit: 'hermes.HermesWidgetInitModal()',
                            callbackOpen: 'hermes.HermesWidgetOpenModal()',
                            callbackClose: 'hermes.HermesWidgetCloseModal()',
                        },
                        {
                            hermes: {
                                HermesWidgetInitModal: function () {},
                                HermesWidgetOpenModal: function () {
                                    if (ctrl.beforeWidgetConfigParams == null) {
                                        ctrl.beforeWidgetConfigParams = ng.copy(ctrl.hermesShipping.WidgetConfigParams);
                                    }

                                    if (!ctrl.createWidget || !ng.equals(ctrl.beforeWidgetConfigParams, ctrl.hermesShipping.WidgetConfigParams)) {
                                        if (ctrl.createWidget) {
                                            var divgWidgetContainer = $('<div id="' + ctrl.widgetId + '" style="height:500px;"></div>');
                                            $('#' + ctrl.modalId)
                                                .find('#' + ctrl.widgetId)
                                                .replaceWith(divgWidgetContainer.prop('outerHTML'));
                                        }

                                        var widgetOptions = {
                                            chooserUi: {
                                                runInFrame: true,
                                                width: 'auto', // Ширина диалогового окна
                                                height: 'auto', // Высота диалогового окна
                                                //dialogTop: 40, // Начальная позиция диалогового окна chooser'a
                                                //dialogLeft: 60, // Начальная позиция диалогового окна chooser'a
                                                containerId: ctrl.widgetId, // Если задан, то рисует себя в указанный контейнер, иначе в виде диалогового окна
                                            },
                                            filters: {
                                                orderId: ctrl.widgetId,
                                                businessUnitId: ctrl.hermesShipping.WidgetConfigParams.businessUnitCode,
                                                address: {
                                                    regionId: '0', // Код региона (Внутренний в Hermes).
                                                    cityName: ctrl.hermesShipping.WidgetConfigParams.city, // недокументированно
                                                    //asString: ctrl.hermesShipping.WidgetConfigParams.address // Адрес на котором требуется сфокусировать карту. (у них еще не реализовано)
                                                },
                                                parcel: {
                                                    weightKg: ctrl.hermesShipping.WidgetConfigParams.weight, // Вес посылки в килограмах. Покажет только те пункты выдачи, которые могут принимать посылки заданного веса.
                                                    overallSize: ctrl.hermesShipping.WidgetConfigParams.dimensionSum, // Сумма габаритов (длина + ширина + высота) в см. Покажет только те пункты выдачи, которые могут принимать посылки таких габаритов.
                                                    costInRub: ctrl.hermesShipping.WidgetConfigParams.orderCost, // Цена посылки в рублях. Покажет только те пункты выдачи, которые могут принимать посылки такой стоимости.
                                                },
                                                services: ['HAND_OUT_IN_PARCEL_SHOP'], //Значение установленных по умолчанию фильтров услуг, предоставляемых в пунктах выдачи.Когда, пользователь нажмет кнопку «Очистить» фильтры установятся в данные значение.
                                            },
                                            success: ctrl.setHermesPvz, // callback-function, которую вызовут, когда пользователь выберет пункт выдачи (информация о выбранном пункте будет переданна в этот callback- метод)
                                            cancel: null, // callback-function, которую вызовут, когда пользователь отменит выбор пункта выдачи
                                            error: null, // callback-function, которую вызовут, если во время работы произойдет критическая ошибка.
                                        };

                                        psChooser.create(widgetOptions);
                                        ctrl.beforeWidgetConfigParams = ng.copy(ctrl.hermesShipping.WidgetConfigParams);

                                        ctrl.createWidget = true;
                                    }
                                },
                                HermesWidgetCloseModal: function () {},
                            },
                        },
                    );
                };

                //var detail = {
                //    orderId: options.filters.orderId,
                //    parcelShop: {
                //        address: {
                //            cityAbbr: '', // Аббревиатура города
                //            cityName: '', // Наименование города
                //            region: '', // Наименование региона
                //            shortAddress: '', // Адрес в кратком варианте
                //            zipCode: '' // Индекс
                //        },
                //        code: '', // Код пункта выдачи, например 90005
                //        name: '', // Название
                //        latitude: '', // Широта
                //        longitude: '', // Долгота
                //        services: [''], // Предоставляемые услуги
                //        restrictions: {
                //            weight: , // Допустимый максимальный вес посылок в кг
                //            maxParcelSize: , // Допустимые габариты посылок в см
                //            cost: // Допустимая стоимость посылок в руб
                //        },
                //        workingTime: , // Режим работы пункта выдачи
                //        workingTimeString: // Режим работы в формате одной строки.
                //    }
                //}

                ctrl.setHermesPvz = function (detail) {
                    ctrl.hermesShipping.PickpointId = detail.parcelShop.code;
                    ctrl.hermesShipping.PickpointAddress = detail.parcelShop.address.shortAddress;

                    if (
                        !ctrl.hermesIsAdmin &&
                        detail.parcelShop.address.cityName.toLowerCase() !== ctrl.hermesShipping.WidgetConfigParams.city.toLowerCase()
                    ) {
                        if (ctrl.hermesContact.ContactId) {
                            //зарегенный пользователь
                            $http.post('checkout/GetCheckoutUser').then(function (response) {
                                if (response.data.obj !== null && response.data.obj.Data !== null) {
                                    var checkoutUserData = response.data.obj;
                                    if (checkoutUserData.Data.Contact !== null) {
                                        //обновляем данные адреса клиента
                                        checkoutUserData.Data.Contact.City = detail.parcelShop.address.cityName;

                                        $http
                                            .post('checkout/CheckoutContactPost', {
                                                address: checkoutUserData.Data.Contact,
                                            })
                                            .then(function (response) {
                                                ctrl.hermesCallback({
                                                    event: 'hermesWidget',
                                                    field: ctrl.hermesShipping.PickpointId || '0',
                                                    shipping: ctrl.hermesShipping,
                                                });
                                            });
                                    }
                                }
                            });
                        } else {
                            var beforeShipping = ng.copy(ctrl.hermesShipping);
                            var callBackFunction = function () {
                                ctrl.hermesShipping.PickpointId = beforeShipping.PickpointId;
                                ctrl.hermesShipping.PickpointAddress = beforeShipping.PickpointAddress;

                                ctrl.hermesCallback({
                                    event: 'hermesWidget',
                                    field: ctrl.hermesShipping.PickpointId || '0',
                                    shipping: ctrl.hermesShipping,
                                });
                                checkoutService.removeCallback('address', callBackFunction);
                            };

                            // после setCurrentZone сработает обновление списка доставок в checkout,
                            // по завершению чего будет вызван Callback 'address'
                            checkoutService.addCallback('address', callBackFunction);
                            zoneService.getCurrentZone().then(function (data) {
                                zoneService.setCurrentZone(
                                    detail.parcelShop.address.cityName,
                                    null,
                                    data.CountryId,
                                    detail.parcelShop.address.region,
                                    data.CountryName,
                                    null,
                                );
                            });
                        }
                    } else {
                        ctrl.hermesCallback({
                            event: 'hermesWidget',
                            field: ctrl.hermesShipping.PickpointId || '0',
                            shipping: ctrl.hermesShipping,
                        });
                    }

                    modalService.close(ctrl.modalId);

                    //$scope.$digest();
                };
            },
        ])
        .directive('hermes', [
            'urlHelper',
            function (urlHelper) {
                return {
                    scope: {
                        hermesShipping: '=',
                        hermesCallback: '&',
                        hermesIsSelected: '=',
                        hermesContact: '=',
                        hermesIsAdmin: '<?',
                    },
                    controller: 'HermesCtrl',
                    controllerAs: 'hermes',
                    bindToController: true,
                    templateUrl: hermesTemplate,
                    link: function (scope, element, attrs, ctrl) {
                        ctrl.init();
                    },
                };
            },
        ]);
})(window.angular);
