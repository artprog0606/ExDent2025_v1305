import { PubSub } from '../../../_common/PubSub/PubSub.js';

/* @ngInject */
function SubscribeCtrl($http, toaster, $translate) {
    var ctrl = this;

    ctrl.subscribeSend = function () {
        $http
            .post('newssubscribe', {
                email: ctrl.subscribeEmail,
                agreeForPromotionalNewsletter: ctrl.agreeForPromotionalNewsletter,
                agree: ctrl.agree,
                rnd: Math.random(),
            })
            .then(function (response) {
                const status = response.data.status;
                if (status === 'success') {
                    toaster.pop('success', '', $translate.instant('Js.Subscribe.SuccessMsg'));
                    ctrl.agreeForPromotionalNewsletter = false;
                    ctrl.agree = false;
                    PubSub.publish('subscribe.email', ctrl.subscribeEmail);
                    ctrl.subscribeEmail = '';
                    ctrl.form.$setPristine();
                } else if (
                    (response.data.agreeForPromotionalNewsletter != null && response.data.agreeForPromotionalNewsletter === 'none') ||
                    (response.data.agree != null && response.data.agree === 'none')
                ) {
                    toaster.pop('error', '', $translate.instant('Js.Subscribe.ErrorAgreement'));
                } else {
                    toaster.pop('error', '', $translate.instant('Js.Subscribe.EmailAreadySubscribed'));
                }
            });
    };
}

export default SubscribeCtrl;
