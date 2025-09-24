import addOfferLandingTemplate from './addOfferLanding.html';
(function (ng) {
    'use strict';

    var AddOfferLandingCtrl = function () {
        var ctrl = this;
        ctrl.removeItem = function (item) {
            var index = ctrl.offers != null && ctrl.offers.length > 0 ? ctrl.offers.indexOf(item) : null;
            if (index != null && index !== -1) {
                ctrl.offers.splice(index, 1);
                if (ctrl.onClose != null) {
                    ctrl.onClose({
                        result: {
                            ids: ctrl.offers.map(function (item) {
                                return item.OfferId;
                            }),
                        },
                    });
                }
            }
        };
    };
    AddOfferLandingCtrl.$inject = [];
    ng.module('addOfferLanding', [])
        .controller('AddOfferLandingCtrl', AddOfferLandingCtrl)
        .component('addOfferLanding', {
            templateUrl: addOfferLandingTemplate,
            controller: 'AddOfferLandingCtrl',
            bindings: {
                onClose: '&',
                offers: '<',
                settingsSelectvizr: '<?',
            },
        });
})(window.angular);
