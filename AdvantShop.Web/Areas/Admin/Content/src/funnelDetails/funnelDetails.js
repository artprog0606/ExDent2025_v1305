(function (ng) {
    'use strict';

    var FunnelDetailsCtrl = function ($http, urlHelper, toaster, advTrackingService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            //focus();
            //var listener = window.addEventListener('blur', function () {
            //    if (document.activeElement === document.getElementById('funnelVideo')) {
            //        advTrackingService.trackEvent('Shop_Funnels_WatchVideo');
            //    }
            //    window.removeEventListener('blur', listener);
            //});
        };
    };

    FunnelDetailsCtrl.$inject = ['$http', 'urlHelper', 'toaster', 'advTrackingService'];

    ng.module('funnelDetails', []).controller('FunnelDetailsCtrl', FunnelDetailsCtrl);
})(window.angular);
