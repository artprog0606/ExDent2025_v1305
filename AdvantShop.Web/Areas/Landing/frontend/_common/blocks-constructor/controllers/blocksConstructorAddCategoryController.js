(function (ng) {
    'use strict';

    var BlocksConstructorAddCategoryCtrl = function () {
        var ctrl = this;

        ctrl.$onInit = function () {
            if (ctrl.onInit != null) {
                ctrl.onInit({ blocksConstructorAddCategory: ctrl });
            }
        };

        ctrl.init = function (modalData) {
            ctrl.modalData = modalData;
        };
    };

    ng.module('blocksConstructor').controller('BlocksConstructorAddCategoryCtrl', BlocksConstructorAddCategoryCtrl);

    BlocksConstructorAddCategoryCtrl.$inject = [];
})(window.angular);
