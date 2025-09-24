(function (ng) {
    'use strict';

    var ErrorCtrl = function () {
        var ctrl = this;

        ctrl.inputTypeLogin = 'password';
        ctrl.inputTypePassword = 'password';
    };

    ng.module('error', ['toaster', 'fullHeightMobile'])
        .controller('ErrorCtrl', ErrorCtrl)
        .config([
            'toasterConfig',
            function (toasterConfig) {
                //toasterConfig['position-class'] = 'toast-bottom-right';
            },
        ])
        .run([
            '$timeout',
            'toaster',
            function ($timeout, toaster) {
                var toasterContainer = document.querySelector('[data-toaster-container]'),
                    toasterItems;

                $timeout(function () {
                    if (toasterContainer != null) {
                        toasterItems = document.querySelectorAll('[data-toaster-type]');
                        if (toasterItems != null) {
                            for (var i = 0, len = toasterItems.length; i < len; i++) {
                                toaster.pop({
                                    type: toasterItems[i].getAttribute('data-toaster-type'),
                                    body: toasterItems[i].innerHTML,
                                    bodyOutputType: 'trustedHtml',
                                    toasterId: 'toasterContainerAlternative',
                                });
                            }
                        }
                    }
                });
            },
        ]);
})(window.angular);
