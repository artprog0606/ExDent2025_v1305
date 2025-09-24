(function (ng) {
    'use strict';

    angular.module('modal', []).constant('modalDefaultOptions', {
        id: undefined,
        isFloating: false,
        crossEnable: true,
        backgroundEnable: true,
        closeOut: true,
        isOpen: false,
        closeEsc: true,
        isShowFooter: true,
        modalClass: undefined,
        modalOverlayClass: undefined,
        callbackInit: undefined,
        callbackOpen: undefined,
        callbackClose: undefined,
        startOpenDelay: undefined,
        zIndex: 999,
    });
})(angular);
