(function (ng) {
    'use strict';

    var ModalVideoCtrl = function (modalService, $element, $window, modalVideoService, $timeout) {
        var ctrl = this;

        ctrl.$onInit = function () {
            modalVideoService.setMQ('(max-width: 768px)');
            ctrl.onChangeMQHandler(modalVideoService.getMQState());
            modalVideoService.addCallbackOnChangeMQ(ctrl.onChangeMQHandler);
        };

        ctrl.$postLink = function () {
            $element[0].addEventListener('click', ctrl.showModal);
        };

        ctrl.showModal = function () {
            if (modalService.hasModal('modalIframeVideo') === false) {
                modalService.renderModal(
                    'modalIframeVideo',
                    null,
                    '<div class="modal-iframe-video-inner"><iframe-responsive in-modal="true" data-from-upload="' +
                        ctrl.fromUpload +
                        '" src="' +
                        ctrl.src +
                        '" data-autoplay="true"></iframe-responsive></div>',
                    null,
                    { modalClass: 'modal-iframe-modal', destroyOnClose: true },
                    null,
                );
            }

            modalService.getModal('modalIframeVideo').then(function (modal) {
                modal.modalScope.open();
            });
        };

        ctrl.onChangeMQHandler = function (mqState) {
            $timeout(function () {
                ctrl.isMobile = mqState;
            }, 0);
        };
    };

    ng.module('modalVideo').controller('ModalVideoCtrl', ModalVideoCtrl);

    ModalVideoCtrl.$inject = ['modalService', '$element', '$window', 'modalVideoService', '$timeout'];
})(window.angular);
