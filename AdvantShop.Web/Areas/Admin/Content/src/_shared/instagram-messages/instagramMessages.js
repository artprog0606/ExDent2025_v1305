import instagramMessagesTemplate from './instagramMessages.html';
(function (ng) {
    'use strict';

    var instagramMessagesCtrl = function ($http, toaster, $translate) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.getMessages();
        };
        ctrl.getMessages = function () {
            $http
                .get('instagram/getCustomerMessages', {
                    params: {
                        customerId: ctrl.customerId,
                    },
                })
                .then(function (response) {
                    ctrl.messages = response.data;
                });
        };
        ctrl.sendInstagramMessage = function () {
            $http
                .post('instagram/sendInstagramMessage', {
                    messageId: 0,
                    customerId: ctrl.customerId,
                    message: ctrl.answerText,
                })
                .then(function (response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.InstagramMessages.MessageSent'));
                        ctrl.answerText = '';
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.InstagramMessages.MessageNotSent'));
                    }
                    ctrl.getMessages();
                });
        };
    };
    instagramMessagesCtrl.$inject = ['$http', 'toaster', '$translate'];
    ng.module('instagramMessages', []).component('instagramMessages', {
        templateUrl: instagramMessagesTemplate,
        controller: instagramMessagesCtrl,
        bindings: {
            customerId: '<?',
        },
    });
})(window.angular);
