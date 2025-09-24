import customerInfoTemplate from './templates/customer-info.html';
(function (ng) {
    'use strict';

    ng.module('customerInfo').component('customerInfo', {
        templateUrl: customerInfoTemplate,
        controller: 'CustomerInfoCtrl',
        bindings: {
            close: '&',
            dismiss: '&',
            resolve: '<',
        },
    });
})(window.angular);
