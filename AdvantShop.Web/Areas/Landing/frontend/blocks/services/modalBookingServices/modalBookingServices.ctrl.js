(function (ng) {
    'use strict';

    var ModalBookingServicesCtrl = function ($element, modalService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.modalId = 'modalBookingServices_' + ctrl.blockId + '_' + ctrl.resourceId + '_' + ctrl.affiliateId;
        };

        ctrl.$postLink = function () {
            $element[0].addEventListener('click', ctrl.showModal);
        };

        ctrl.showModal = function () {
            if (modalService.hasModal(ctrl.modalId) === false) {
                modalService.renderModal(
                    ctrl.modalId,
                    null,
                    '<div data-ng-include="\'landing/landing/getBookingServices?blockId=' +
                        ctrl.blockId +
                        '&resourceId=' +
                        ctrl.resourceId +
                        '&affiliateId=' +
                        ctrl.affiliateId +
                        '&showPrice=' +
                        ctrl.showPrice +
                        '\'"></div>',
                    null,
                    {
                        modalClass: 'lp-modal-booking ' + (ctrl.colorScheme || 'color-scheme--light'),
                    },
                    {
                        modalBookingServices: ctrl,
                    },
                );
            }

            modalService.getModal(ctrl.modalId).then(function (modal) {
                modal.modalScope.open();
            });
        };
    };

    ng.module('modalBookingServices').controller('ModalBookingServicesCtrl', ModalBookingServicesCtrl);

    ModalBookingServicesCtrl.$inject = ['$element', 'modalService'];
})(window.angular);
