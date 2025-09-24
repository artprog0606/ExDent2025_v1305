(function (ng) {
    'use strict';

    var PARAM_KEY = 'customerIdInfo';
    /* @ngInject */
    var customerInfoService = function ($location, $uibModal, $timeout, $window) {
        var service = this;

        service.addInstance = function (params, options) {
            var scrollTop = $window.pageYOffset;
            return $uibModal
                .open({
                    component: 'customerInfo',
                    controllerAs: '$ctrl',
                    windowClass: 'lead-info',
                    openedClass: 'modal-open lead-info-modal--open',
                    resolve: { instance: { customerId: params.customerId, partnerId: params.partnerId } },
                })
                .result.then(
                    function () {},
                    function (dismissResult) {
                        if (options != null && options.onClose != null) {
                            options.onClose({ result: dismissResult });
                        }
                    },
                )
                .finally(function () {
                    service.removeUrlParam();
                    $timeout(function () {
                        $window.scrollTo(0, scrollTop);
                    }, 0);
                });
        };

        service.setUrlParam = function (customerid) {
            $location.search(PARAM_KEY, customerid);
        };

        service.removeUrlParam = function () {
            $location.search(PARAM_KEY, null);
        };

        service.getUrlParam = function () {
            var search = $location.search();
            return search != null && search[PARAM_KEY] != null ? search[PARAM_KEY] : null;
        };
    };

    ng.module('customerInfo').service('customerInfoService', customerInfoService);
})(window.angular);
