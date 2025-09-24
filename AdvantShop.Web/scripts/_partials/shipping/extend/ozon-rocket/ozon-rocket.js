import ozonRocketTemplate from './ozon-rocket.tpl.html';

(function (ng) {
    'use strict';

    angular
        .module('ozon-rocket', ['modal', 'checkout', 'shipping'])
        .controller('OzonRocketCtrl', [
            '$http',
            '$scope',
            'modalService',
            '$element',
            'checkoutService',
            'shippingService',
            function ($http, $scope, modalService, $element, checkoutService, shippingService) {
                var ctrl = this;

                ctrl.$onInit = function () {
                    $element.on('$destroy', function () {
                        modalService.destroy(ctrl.modalId);
                    });
                };

                ctrl.init = function () {
                    ctrl.modalId = 'modalOzonRocketWidget' + ctrl.ozonRocketShipping.MethodId;
                    ctrl.initModal();
                };

                ctrl.initModal = function () {
                    shippingService.fireTemplateReady($scope);

                    ctrl.city = ctrl.ozonRocketWidgetConfigData['defaultcity'];
                    var divgWidgetContainer = ctrl.createIframe();

                    modalService.renderModal(
                        ctrl.modalId,
                        null,
                        divgWidgetContainer.prop('outerHTML'),
                        null,
                        {
                            modalClass: 'shipping-dialog',
                            callbackInit: 'ozonrocket.OzonRocketWidgetInitModal()',
                            callbackOpen: 'ozonrocket.OzonRocketWidgetOpenModal()',
                            callbackClose: 'ozonrocket.OzonRocketWidgetCloseModal()',
                        },
                        {
                            ozonrocket: {
                                OzonRocketWidgetInitModal: function () {},
                                OzonRocketWidgetOpenModal: function () {
                                    if (ctrl.city !== ctrl.ozonRocketWidgetConfigData['defaultcity']) {
                                        ctrl.city = ctrl.ozonRocketWidgetConfigData['defaultcity'];
                                        $('#' + ctrl.modalId)
                                            .find('.modal-content')
                                            .empty()
                                            .append(ctrl.createIframe().prop('outerHTML'));
                                    }
                                    window.addEventListener('message', ctrl.setOzonRocketPvz, false);
                                },
                                OzonRocketWidgetCloseModal: function () {
                                    window.removeEventListener('message', ctrl.setOzonRocketPvz);
                                },
                            },
                        },
                    );
                };

                ctrl.createIframe = function () {
                    var urlParams = [];
                    Object.keys(ctrl.ozonRocketWidgetConfigData).map(function (objectKey) {
                        if (ctrl.ozonRocketWidgetConfigData[objectKey] === null) {
                            urlParams.push(objectKey);
                        } else {
                            urlParams.push(objectKey + '=' + ctrl.ozonRocketWidgetConfigData[objectKey]);
                        }
                    });
                    var iframeUrl = 'https://rocket.ozon.ru/lk/widget?' + urlParams.join('&');
                    var divgWidgetContainer = $(
                        '<iframe title="Ozon widget" style="width: 100%; height: 100%; min-width: 320px; min-height: 500px; border: none; overflow: hidden" src="' +
                            iframeUrl +
                            '">Браузер не поддерживает iframe</iframe>',
                    );
                    return divgWidgetContainer;
                };

                ctrl.setOzonRocketPvz = function (event) {
                    if (event.origin !== 'https://rocket.ozon.ru') return;

                    var data = event.data;
                    if (typeof data === 'string') {
                        data = new Function('return ' + data)();
                    }

                    if (!data.id || data.pcmPixelPostMessageEvent) return;

                    var selectedPvzInfo = data;
                    var additionalData = {
                        id: selectedPvzInfo.id,
                        type: selectedPvzInfo.type,
                        address: selectedPvzInfo.address,
                        fittingClothesAvailable: selectedPvzInfo.fittingClothesAvailable,
                        fittingShoesAvailable: selectedPvzInfo.fittingShoesAvailable,
                        price: selectedPvzInfo.price,
                        time: selectedPvzInfo.time,
                    };

                    ctrl.ozonRocketShipping.PointId = selectedPvzInfo.id;
                    ctrl.ozonRocketShipping.PickpointAddress = selectedPvzInfo.address;
                    ctrl.ozonRocketShipping.PickpointAdditionalData = JSON.stringify(additionalData);

                    ctrl.ozonRocketCallback({
                        event: 'ozonRocketWidget',
                        field: ctrl.ozonRocketShipping.PointId || 0,
                        shipping: ctrl.ozonRocketShipping,
                    });

                    modalService.close(ctrl.modalId);

                    $scope.$digest();
                };
            },
        ])
        .directive('ozonRocket', [
            'urlHelper',
            function (urlHelper) {
                return {
                    scope: {
                        ozonRocketShipping: '=',
                        ozonRocketCallback: '&',
                        ozonRocketWidgetConfigData: '=',
                        ozonRocketIsSelected: '=',
                        ozonRocketContact: '=',
                        ozonRocketIsAdmin: '<?',
                    },
                    controller: 'OzonRocketCtrl',
                    controllerAs: 'ozonRocket',
                    bindToController: true,
                    templateUrl: ozonRocketTemplate,
                    link: function (scope, element, attrs, ctrl) {
                        ctrl.init();
                    },
                };
            },
        ]);
})(window.angular);
