import pointdeliverymapTemplate from './pointdeliverymap.tpl.html';
import './pointdeliverymap.scss';

(function (ng) {
    'use strict';

    // !!! Используется еще в других доставках !!!

    ng.module('pointDeliveryMap', ['yandexMaps'])
        .controller(
            'PointDeliveryMapCtrl',
            /* @ngInject */
            function ($document, $http, $scope, modalService, $timeout, $element, urlHelper, $q, yandexMapsService, shippingService) {
                var ctrl = this;

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
                    ctrl.modalId = 'modalPointDeliveryMap' + ctrl.pointDeliveryMapShipping.Id;
                    ctrl.containerMapId = 'yaMapPointDelivery' + ctrl.pointDeliveryMapShipping.Id;

                    if (!window.pointDeliveryMapLoaded) {
                        window.pointDeliveryMapLoaded = true;

                        //$(document.body).append($('<link rel="stylesheet" type="text/css" />').attr('href', pointdeliverymapCss));
                    }

                    shippingService.fireTemplateReady($scope);
                };

                ctrl.modalOpen = async function (modalId) {
                    await yandexMapsService.loadYandexMap({
                        apikey: ctrl.pointDeliveryMapShipping.MapParams.YandexMapsApikey,
                        lang: ctrl.pointDeliveryMapShipping.MapParams.Lang,
                    });

                    ctrl.initModal();

                    modalService.getModal(modalId).then(({ modalScope }) => {
                        modalScope.open();
                    });
                };

                ctrl.initModal = function () {
                    modalService.renderModal(
                        ctrl.modalId,
                        null,
                        '<div id="' + ctrl.containerMapId + '" class="shipping-point-delivery-map"></div>',
                        null,
                        {
                            modalClass: 'shipping-point-delivery-map__modal',
                            callbackInit: 'pointDeliveryMapModal.InitModal()',
                            callbackOpen: 'pointDeliveryMapModal.OpenModal()',
                            callbackClose: 'pointDeliveryMapModal.CloseModal()',
                        },
                        {
                            pointDeliveryMapModal: {
                                InitModal: function () {},
                                OpenModal: function () {
                                    $timeout(function () {
                                        if (!ctrl.map) {
                                            ctrl.map = new ymaps.Map(ctrl.containerMapId, {
                                                center: [55.76, 37.64], // Москва
                                                zoom: 10,
                                                controls: ['zoomControl'],
                                                options: {
                                                    autoFitToViewport: 'always',
                                                },
                                            });

                                            ctrl.objectManager = new ymaps.ObjectManager({
                                                // Чтобы метки начали кластеризоваться, выставляем опцию.
                                                clusterize: true,
                                                // ObjectManager принимает те же опции, что и кластеризатор.
                                                gridSize: 64,
                                            });

                                            ctrl.map.geoObjects.add(ctrl.objectManager);

                                            ctrl.setDestination();

                                            if (ctrl.pointDeliveryMapShipping.PointParams.PointsByDestination === false) {
                                                var promise = null;

                                                if (ctrl.pointDeliveryMapShipping.PointParams.IsLazyPoints) {
                                                    promise = ctrl.loadPoints();
                                                } else {
                                                    ctrl.objectManager.add(ctrl.pointDeliveryMapShipping.PointParams.Points);
                                                    promise = $q.resolve();
                                                }

                                                promise.then(function () {
                                                    if (ctrl.pointDeliveryMapShipping.YaSelectedPoint) {
                                                        ctrl.objectManager.objects.setObjectOptions(ctrl.pointDeliveryMapShipping.YaSelectedPoint, {
                                                            preset: 'islands#redDotIcon',
                                                        });
                                                    }
                                                });
                                            }

                                            if (ctrl.pointDeliveryMapIsAdmin) {
                                                ctrl.map.controls.add(new ymaps.control.SearchControl());
                                            }
                                        } else {
                                            ctrl.setDestination();
                                            $timeout(function () {
                                                ctrl.map.container.fitToViewport();
                                            }, 300);
                                        }
                                        window.PointDeliveryMap = ctrl.MapSelectedPoint;
                                    });
                                },
                                CloseModal: function () {
                                    //ctrl.map.destroy();
                                    //ctrl.map = null;
                                    window.PointDeliveryMap = null;
                                },
                            },
                        },
                    );
                };

                ctrl.loadPoints = function () {
                    /*var url = window.location.href.split('?')[0].split('#')[0].replace('/checkout/lp', '/checkout');

                if (url.indexOf('/adminv2/') !== -1) {
                    url = url.split('/adminv2/')[0] + '/checkout';
                }

                if (url.indexOf('/adminv3/') !== -1) {
                    url = url.split('/adminv3/')[0] + '/checkout';
                }

                url += '/GetShippingData';*/
                    var url = urlHelper.getAbsUrl('/checkout/GetShippingData', true);

                    return $http
                        .post(url, {
                            methodId: ctrl.pointDeliveryMapShipping.MethodId,
                            data: ctrl.pointDeliveryMapShipping.PointParams.LazyPointsParams,
                        })
                        .then(function (response) {
                            ctrl.objectManager.removeAll();
                            if (response.data) {
                                ctrl.objectManager.add(response.data);
                            }
                        });
                };

                ctrl.setDestination = function () {
                    if (
                        ctrl.pointDeliveryMapShipping.MapParams.Destination &&
                        ctrl.prevDestination !== ctrl.pointDeliveryMapShipping.MapParams.Destination
                    ) {
                        ctrl.prevDestination = ctrl.pointDeliveryMapShipping.MapParams.Destination;
                        //центрируем карту по месту доставки
                        ymaps
                            .geocode(ctrl.pointDeliveryMapShipping.MapParams.Destination, {
                                // Если нужен только один результат, экономим трафик пользователей.
                                results: 1,
                            })
                            .then(function (res) {
                                if (res.geoObjects.getLength()) {
                                    //ctrl.map.options.set({ restrictMapArea: false });
                                    // Выбираем первый результат геокодирования.
                                    var firstGeoObject = res.geoObjects.get(0),
                                        // Область видимости геообъекта.
                                        bounds = firstGeoObject.properties.get('boundedBy');

                                    // Масштабируем карту на область видимости геообъекта.
                                    ctrl.map
                                        .setBounds(bounds, {
                                            // Проверяем наличие тайлов на данном масштабе.
                                            checkZoomRange: true,
                                        })
                                        .then(function () {
                                            //ctrl.map.options.set({ restrictMapArea: true/*bounds*/ });
                                        });

                                    if (ctrl.pointDeliveryMapShipping.PointParams.PointsByDestination) {
                                        var promise = null;

                                        if (ctrl.pointDeliveryMapShipping.PointParams.IsLazyPoints) {
                                            promise = ctrl.loadPoints();
                                        } else {
                                            ctrl.objectManager.removeAll();
                                            ctrl.objectManager.add(ctrl.pointDeliveryMapShipping.PointParams.Points);
                                            promise = $q.resolve();
                                        }

                                        promise.then(function () {
                                            if (ctrl.pointDeliveryMapShipping.YaSelectedPoint) {
                                                ctrl.objectManager.objects.setObjectOptions(ctrl.pointDeliveryMapShipping.YaSelectedPoint, {
                                                    preset: 'islands#redDotIcon',
                                                });
                                            }
                                        });
                                    }
                                }
                            });
                    }
                };

                ctrl.MapSelectedPoint = function (yaPointId, pointId) {
                    if (ctrl.pointDeliveryMapShipping.YaSelectedPoint) {
                        ctrl.objectManager.objects.setObjectOptions(ctrl.pointDeliveryMapShipping.YaSelectedPoint, {
                            preset: 'islands#dotIcon',
                        });
                    }
                    ctrl.objectManager.objects.setObjectOptions(yaPointId, { preset: 'islands#redDotIcon' });

                    ctrl.pointDeliveryMapShipping.YaSelectedPoint = yaPointId;
                    ctrl.pointDeliveryMapShipping.PickpointId = pointId;
                    ctrl.pointDeliveryMapCallback({
                        event: 'pointDeliveryMap',
                        field: ctrl.pointDeliveryMapShipping.PickpointId || 0,
                        shipping: ctrl.pointDeliveryMapShipping,
                    });

                    modalService.close(ctrl.modalId);
                    $scope.$digest();
                };
            },
        )
        .directive('pointDeliveryMap', [
            'urlHelper',
            function (urlHelper) {
                return {
                    scope: {
                        pointDeliveryMapShipping: '=',
                        pointDeliveryMapCallback: '&',
                        pointDeliveryMapIsSelected: '=',
                        pointDeliveryMapContact: '=',
                        pointDeliveryMapIsAdmin: '<?',
                    },
                    controller: 'PointDeliveryMapCtrl',
                    controllerAs: 'pointDeliveryMap',
                    bindToController: true,
                    templateUrl: pointdeliverymapTemplate,
                    link: function (scope, element, attrs, ctrl) {
                        ctrl.init();
                    },
                };
            },
        ]);
})(window.angular);
