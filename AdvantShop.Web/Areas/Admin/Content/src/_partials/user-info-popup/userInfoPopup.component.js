(function (ng) {
    'use strict';

    ng.module('userInfoPopup').component('userInfoPopup', {
        controller: 'userInfoPopupCtrl',
        bindings: {
            onClose: '&',
            onFinish: '&',
        },
    });
})(window.angular);
