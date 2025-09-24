(function (ng) {
    'use strict';

    angular
        .module('mokkaPay', [])
        .controller('MokkaPayCtrl', [
            '$scope',
            'modalService',
            '$element',
            function ($scope, modalService, $element) {
                var ctrl = this;

                ctrl.$onInit = function () {
                    $element.on('$destroy', function () {
                        modalService.destroy(ctrl.modalId);
                    });
                };

                ctrl.init = function () {
                    ctrl.modalId = 'modalMokkaPay-' + ctrl.mokkaOrderCode;
                    ctrl.initModal();
                    modalService.getModal(ctrl.modalId).then(function (modal) {
                        modal.modalScope.open();
                    });
                };

                ctrl.initModal = function () {
                    modalService.renderModal(
                        ctrl.modalId,
                        null,
                        '',
                        null,
                        {
                            modalClass: 'shipping-dialog',
                            callbackOpen: 'mokkapay.MokkaPayWidgetOpenModal()',
                        },
                        {
                            mokkapay: {
                                MokkaPayWidgetOpenModal: function () {
                                    var modalContent = $('#' + ctrl.modalId).find('.modal-content');
                                    if (!modalContent.find('iframe').length) {
                                        modalContent.empty().append(ctrl.createIframe().prop('outerHTML'));
                                    }
                                },
                            },
                        },
                    );
                };

                ctrl.createIframe = function () {
                    var iframeUrl = ctrl.mokkaPayUrl;
                    var divgWidgetContainer = $(
                        '<iframe title="Mokka widget" style="width: 100%; height: 100%; min-width: 35vw; min-height: 85vh; border: none; overflow: hidden" src="' +
                            iframeUrl +
                            '">Браузер не поддерживает iframe</iframe>',
                    );
                    return divgWidgetContainer;
                };
            },
        ])
        .directive('mokkaOrderPay', [
            function () {
                return {
                    scope: {
                        mokkaOrderCode: '<',
                        mokkaPayUrl: '<',
                    },
                    controller: 'MokkaPayCtrl',
                    controllerAs: 'mokkaPay',
                    bindToController: true,
                    link: function (scope, element, attrs, ctrl) {
                        element.on('click', function () {
                            ctrl.init();
                            scope.$apply();
                        });
                    },
                };
            },
        ]);
})(window.angular);
