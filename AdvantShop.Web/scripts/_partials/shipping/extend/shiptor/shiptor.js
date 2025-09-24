import shiptorTemplate from './shiptor.tpl.html';
(function (ng) {
    'use strict';

    angular
        .module('shiptor', ['yandexMaps'])
        .controller('ShiptorCtrl', [
            '$http',
            '$scope',
            'modalService',
            '$element',
            'yandexMapsService',
            'shippingService',
            function ($http, $scope, modalService, $element, yandexMapsService, shippingService) {
                var ctrl = this;

                ctrl.$onInit = function () {
                    $element.on('$destroy', function () {
                        modalService.destroy(ctrl.modalId);
                        if (window.JCShiptorWidgetPvz && window.JCShiptorWidgetPvz[ctrl.widgetId]) {
                            window.JCShiptorWidgetPvz[ctrl.widgetId].destroy();
                            window.JCShiptorWidgetPvz[ctrl.widgetId] = null;
                        }
                    });
                };

                ctrl.init = function () {
                    ctrl.modalId = 'modalShiptorWidget' + ctrl.shiptorShipping.MethodId;
                    ctrl.widgetId = 'shiptor_widget_pvz' + ctrl.shiptorShipping.MethodId;
                    if (!window.isShiptorLoaded) {
                        window.isShiptorLoaded = true;

                        jQuery.ajax({
                            dataType: 'script',
                            cache: true,
                            url: 'https://widget.shiptor.ru/embed/widget-pvz.js',
                        });
                    }

                    ctrl.initModal();
                };

                ctrl.initModal = function () {
                    shippingService.fireTemplateReady($scope);

                    var divgWidgetContainer = $('<div id="' + ctrl.widgetId + '" class="_shiptor_widget" style="height:500px;"></div>');
                    Object.keys(ctrl.shiptorShipping.WidgetConfigData).map(function (objectKey) {
                        divgWidgetContainer.attr(objectKey, ctrl.shiptorShipping.WidgetConfigData[objectKey]);
                    });

                    if (yandexMapsService.isLoadedYandexMap()) {
                        // в доке как признак не грузить яндекс.карты
                        // только все равно грузит (мож поправят)
                        // в момент инициализации эти атрибуты пропадают
                        divgWidgetContainer.attr('data-link-maps', '0');
                        divgWidgetContainer.attr('data-link-ymaps', '0'); // из кода widget-pvz.js
                    }

                    modalService.renderModal(
                        ctrl.modalId,
                        null,
                        divgWidgetContainer.prop('outerHTML'),
                        null,
                        {
                            modalClass: 'shipping-dialog',
                            callbackInit: 'shiptor.ShiptorWidgetInitModal()',
                            callbackOpen: 'shiptor.ShiptorWidgetOpenModal()',
                            callbackClose: 'shiptor.ShiptorWidgetCloseModal()',
                        },
                        {
                            shiptor: {
                                ShiptorWidgetInitModal: function () {},
                                ShiptorWidgetOpenModal: function () {
                                    if (ctrl.beforeWidgetConfigParams == null) {
                                        ctrl.beforeWidgetConfigParams = angular.copy(ctrl.shiptorShipping.WidgetConfigParams);
                                    }

                                    if (window.JCShiptorWidgetPvz && window.JCShiptorWidgetPvz[ctrl.widgetId]) {
                                        if (!angular.equals(ctrl.beforeWidgetConfigParams, ctrl.shiptorShipping.WidgetConfigParams)) {
                                            window.JCShiptorWidgetPvz[ctrl.widgetId].setParams(ctrl.shiptorShipping.WidgetConfigParams);
                                            window.JCShiptorWidgetPvz[ctrl.widgetId].refresh();
                                            ctrl.beforeWidgetConfigParams = angular.copy(ctrl.shiptorShipping.WidgetConfigParams);
                                        }
                                    } else {
                                        // повторно создаем контейнер, чтобы создать с обновленными атрибутами,
                                        // т.к.модальное окно еще не открывали и виджет не создавался
                                        var divgWidgetContainer = $(
                                            '<div id="' + ctrl.widgetId + '" class="_shiptor_widget" style="height:500px;"></div>',
                                        );
                                        Object.keys(ctrl.shiptorShipping.WidgetConfigData).map(function (objectKey) {
                                            divgWidgetContainer.attr(objectKey, ctrl.shiptorShipping.WidgetConfigData[objectKey]);
                                        });

                                        if (yandexMapsService.isLoadedYandexMap()) {
                                            // в доке как признак не грузить яндекс.карты
                                            // только все равно грузит (мож поправят)
                                            // в момент инициализации эти атрибуты пропадают
                                            divgWidgetContainer.attr('data-link-maps', '0');
                                            divgWidgetContainer.attr('data-link-ymaps', '0'); // из кода widget-pvz.js
                                        }

                                        $('#' + ctrl.modalId)
                                            .find('#' + ctrl.widgetId)
                                            .replaceWith(divgWidgetContainer.prop('outerHTML'));

                                        if (!window.JCShiptorWidgetPvz) {
                                            window.JCShiptorWidgetPvz = {};
                                        }

                                        window.JCShiptorWidgetPvz[ctrl.widgetId] = new ShiptorWidgetPvz({
                                            id: ctrl.widgetId,
                                        });
                                        window.JCShiptorWidgetPvz[ctrl.widgetId].init();
                                    }
                                    $('#' + ctrl.modalId)
                                        .find('#' + ctrl.widgetId)
                                        .on('onPvzSelect', ctrl.setShiptorPvz);
                                },
                                ShiptorWidgetCloseModal: function () {
                                    $('#' + ctrl.modalId)
                                        .find('#' + ctrl.widgetId)
                                        .off('onPvzSelect');
                                },
                            },
                        },
                    );
                };

                ctrl.setShiptorPvz = function ($e) {
                    var detail = $e.originalEvent.detail;
                    var additionalData = {
                        Id: detail.id,
                        Code: detail.code,
                        Courier: detail.courier,
                        Cod: detail.cod,
                        Card: detail.card,
                        KladrId: detail.kladr_id,
                        ShippingMethod: detail.shipping_method,
                        Type: detail.type,
                    };

                    ctrl.shiptorShipping.PickpointId = detail.id;
                    ctrl.shiptorShipping.PickpointAddress = detail.prepare_address
                        ? [
                              detail.prepare_address.administrative_area,
                              detail.prepare_address.settlement,
                              detail.prepare_address.street,
                              detail.prepare_address.house + ' ' + (detail.prepare_address.block || ''),
                          ].join(', ')
                        : detail.address;
                    ctrl.shiptorShipping.PickpointAdditionalData = JSON.stringify(additionalData);
                    ctrl.shiptorShipping.PickpointAdditionalDataObj = additionalData;

                    ctrl.shiptorCallback({
                        event: 'shiptorWidget',
                        field: ctrl.shiptorShipping.PickpointId || 0,
                        shipping: ctrl.shiptorShipping,
                    });

                    modalService.close(ctrl.modalId);

                    $scope.$digest();
                };
            },
        ])
        .directive('shiptor', [
            'urlHelper',
            function (urlHelper) {
                return {
                    scope: {
                        shiptorShipping: '=',
                        shiptorCallback: '&',
                        shiptorIsSelected: '=',
                        shiptorContact: '=',
                    },
                    controller: 'ShiptorCtrl',
                    controllerAs: 'shiptor',
                    bindToController: true,
                    templateUrl: shiptorTemplate,
                    link: function (scope, element, attrs, ctrl) {
                        ctrl.init();
                    },
                };
            },
        ]);
})(window.angular);
