(function (ng) {
    'use strict';

    var BlocksConstructorNewBlockCtrl = function ($document, $window, blocksConstructorService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            blocksConstructorService.getBlocks(ctrl.modalData.landingpageId).then(function (data) {
                ctrl.data = data;
                ctrl.categorySelected = ctrl.data[0];
            });
        };

        ctrl.selectCategory = function (category) {
            if (category !== ctrl.categorySelected) {
                ctrl.categorySelected = category;

                $document[0].querySelector('.js-blocks-constructor-modal-block-wrap').scrollTo(0, 0);
            }
        };
    };

    ng.module('blocksConstructor').controller('BlocksConstructorNewBlockCtrl', BlocksConstructorNewBlockCtrl);

    BlocksConstructorNewBlockCtrl.$inject = ['$document', '$window', 'blocksConstructorService'];
})(window.angular);
