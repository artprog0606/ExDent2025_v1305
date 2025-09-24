import deliveryByZonesTemplate from './deliveryByZones.tpl.html';
import './deliveryByZones.scss';

(function (ng) {
    'use strict';

    ng.module('deliveryByZones', ['yandexMaps'])
        .controller(
            'DeliveryByZonesCtrl',
            /* @ngInject */
            function ($document, $http, $scope, modalService, $timeout, $element, urlHelper, $q, yandexMapsService, shippingService) {
                var ctrl = this;
                //deferReadyMap = $q.defer();

                ctrl.$onInit = function () {
                    $element.on('$destroy', function () {
                        modalService.destroy(ctrl.modalId);
                        if (ctrl.map) {
                            ctrl.map.destroy();
                            ctrl.map = null;
                        }
                    });
                };

                ctrl.init = function () {
                    ctrl.modalId = 'modalDeliveryByZones' + ctrl.deliveryByZonesShipping.Id;
                    ctrl.containerMapId = 'yaMapDeliveryByZones' + ctrl.deliveryByZonesShipping.Id;

                    var loadYaMapPromise = yandexMapsService.loadYandexMap({
                        apikey: ctrl.deliveryByZonesShipping.MapParams.YandexMapsApikey,
                        lang: ctrl.deliveryByZonesShipping.MapParams.Lang,
                    });

                    // if (!window.deliveryByZonesLoaded) {
                    //     window.deliveryByZonesLoaded = true;
                    //
                    //     $(document.body).append($('<link rel="stylesheet" type="text/css" />').attr('href', pointdeliverymapCss));
                    // }

                    ctrl.initModal(loadYaMapPromise);
                };

                ctrl.initModal = function (loadYaMapPromise) {
                    shippingService.fireTemplateReady($scope);

                    modalService.renderModal(
                        ctrl.modalId,
                        null,
                        '<div id="' + ctrl.containerMapId + '" class="delivery-by-zones-widget"></div>',
                        null,
                        {
                            modalClass: 'shipping-dialog',
                            callbackInit: 'deliveryByZonesModal.InitModal()',
                            callbackOpen: 'deliveryByZonesModal.OpenModal()',
                            callbackClose: 'deliveryByZonesModal.CloseModal()',
                        },
                        {
                            deliveryByZonesModal: {
                                InitModal: function () {},
                                OpenModal: function () {
                                    $timeout(() => {
                                        loadYaMapPromise.then(async function () {
                                            if (!ctrl.map) {
                                                ctrl.map = new ymaps.Map(ctrl.containerMapId, {
                                                    center: [55.76, 37.64], // Москва
                                                    zoom: 12,
                                                    controls: ['zoomControl'],
                                                    options: {
                                                        autoFitToViewport: 'always',
                                                    },
                                                });

                                                // ctrl.objectManager = new ymaps.ObjectManager({
                                                //     // Чтобы метки начали кластеризоваться, выставляем опцию.
                                                //     clusterize: true,
                                                //     // ObjectManager принимает те же опции, что и кластеризатор.
                                                //     gridSize: 64
                                                // });
                                                //
                                                // ctrl.map.geoObjects.add(ctrl.objectManager);

                                                // ctrl.setDestination();

                                                if (ctrl.pointDeliveryMapIsAdmin) {
                                                    ctrl.map.controls.add(new ymaps.control.SearchControl());
                                                }

                                                // ctrl.map.events.add('sizechange', function (event) {
                                                //     if (event.get('oldSize') != event.get('newSize')) {
                                                //         // срабатывает при первом открытии окна
                                                //     }
                                                // });

                                                await ctrl.loadPolygons();
                                            } else {
                                                // ctrl.setDestination();
                                                // $timeout(function () {
                                                //     // уже выставляется в autoFitToViewport
                                                //     ctrl.map.container.fitToViewport();
                                                // }, 300);
                                            }

                                            if (!ctrl.deliveryPoint) {
                                                ctrl.deliveryPoint = new ymaps.GeoObject(
                                                    {
                                                        geometry: { type: 'Point' },
                                                        properties: { iconCaption: 'Доставим сюда' },
                                                    },
                                                    {
                                                        preset: 'islands#blueDeliveryIcon',
                                                        // draggable: true,
                                                        iconCaptionMaxWidth: '215',
                                                    },
                                                );
                                                ctrl.map.geoObjects.add(ctrl.deliveryPoint);
                                            }

                                            ctrl.deliveryPoint.geometry.setCoordinates(ctrl.deliveryByZonesShipping.Point);
                                            ctrl.updatePointInfo();

                                            ctrl.applyBoundsMap();
                                        });
                                    });
                                },
                                CloseModal: function () {},
                            },
                        },
                    );
                };

                // ctrl.readyMap = function () {
                //     return deferReadyMap.promise;
                // };

                ctrl.applyBoundsMap = function () {
                    if (ctrl.deliveryByZonesShipping.BoundedBy) {
                        // Масштабируем карту на область видимости геообъекта.
                        ctrl.map
                            .setBounds(ctrl.deliveryByZonesShipping.BoundedBy, {
                                // Проверяем наличие тайлов на данном масштабе.
                                checkZoomRange: true,
                            })
                            .then(function () {
                                //ctrl.map.options.set({ restrictMapArea: true/*bounds*/ });
                                // ctrl.map.setCenter(ctrl.deliveryByZonesShipping.Point);
                            });
                    } else {
                        if (ctrl.deliveryByZonesShipping.Point) {
                            // позиционируем карту на точку
                            ymaps.getZoomRange('yandex#map', ctrl.deliveryByZonesShipping.Point).then(function (result) {
                                ctrl.map.setCenter(ctrl.deliveryByZonesShipping.Point, result[1] < 16 ? result[1] : 16); // 16 - оптимальное для просмотра дома
                            });
                        } else {
                            if (ctrl.deliveryZones) {
                                // позиционируем карту на зоны
                                ctrl.deliveryZones.applyBoundsToMap(ctrl.map);
                            }
                        }
                    }
                };

                ctrl.loadPolygons = function () {
                    var url = urlHelper.getAbsUrl('/checkout/GetShippingData', true);

                    return $http
                        .post(url, {
                            methodId: ctrl.deliveryByZonesShipping.MethodId,
                            data: {},
                        })
                        .then(function (response) {
                            // ctrl.objectManager.removeAll();
                            if (response.data) {
                                ctrl.deliveryZones = ymaps.geoQuery(response.data).addToMap(ctrl.map);
                                ctrl.deliveryZones.each(function (obj) {
                                    obj.options.set({
                                        fillColor: obj.properties.get('fill'),
                                        fillOpacity: obj.properties.get('fill-opacity'),
                                        strokeColor: obj.properties.get('stroke'),
                                        strokeWidth: obj.properties.get('stroke-width'),
                                        strokeOpacity: obj.properties.get('stroke-opacity'),
                                    });
                                    obj.properties.set('balloonContent', obj.properties.get('description'));
                                });
                                //ctrl.deliveryZones.applyBoundsToMap(ctrl.map);
                            }

                            return response.data;
                        });
                };

                ctrl.updatePointInfo = function () {
                    if (!ctrl.deliveryZones) {
                        return;
                    }
                    if (!ctrl.deliveryPoint) {
                        return;
                    }

                    var coords = ctrl.deliveryPoint.geometry.getCoordinates();
                    if (!coords) {
                        return;
                    }
                    // Находим полигон, в который входят переданные координаты.
                    var polygon = ctrl.deliveryZones.searchContaining(coords).get(0);

                    if (polygon) {
                        ctrl.deliveryPoint.options.set('iconColor', polygon.properties.get('fill'));
                        ctrl.deliveryPoint.properties.set({ iconCaption: 'Доставим сюда' });
                    } else {
                        ctrl.deliveryPoint.options.set('iconColor', 'black');
                        ctrl.deliveryPoint.properties.set({ iconCaption: 'Сюда не доставляем' });
                    }
                };
            },
        )
        .directive(
            'deliveryByZones',
            /* @ngInject */ function (urlHelper) {
                return {
                    scope: {
                        deliveryByZonesShipping: '=',
                        deliveryByZonesCallback: '&',
                        deliveryByZonesIsSelected: '=',
                        deliveryByZonesContact: '=',
                        deliveryByZonesIsAdmin: '<?',
                    },
                    controller: 'DeliveryByZonesCtrl',
                    controllerAs: 'deliveryByZones',
                    bindToController: true,
                    templateUrl: deliveryByZonesTemplate,
                    link: function (scope, element, attrs, ctrl) {
                        ctrl.init();
                    },
                };
            },
        );
})(window.angular);
