(function (ng) {
    'use strict';

    var PARAM_KEY = 'leadIdInfo';

    var leadInfoService = function ($location, $uibModal, $timeout, $window, urlHelper) {
        var service = this;

        service.addInstance = function (params, options) {
            var scrollTop = $window.pageYOffset;
            return $uibModal
                .open({
                    component: 'leadInfo',
                    controllerAs: '$ctrl',
                    windowClass: 'lead-info',
                    openedClass: 'modal-open lead-info-modal--open',
                    resolve: { params: params },
                })
                .result.then(function (closeResult) {
                    if (options != null && options.onClose != null) {
                        options.onClose(closeResult);
                    }
                })
                .catch(() => {})
                .finally(function () {
                    service.removeUrlParam();
                    $timeout(function () {
                        $window.scrollTo(0, scrollTop);
                    }, 0);
                });
        };

        service.setUrlParam = function (leadId) {
            $location.search(PARAM_KEY, leadId);
        };

        service.removeUrlParam = function () {
            $location.search(PARAM_KEY, null);
        };

        service.getUrlParam = function () {
            return urlHelper.getUrlParamsUniversalAsObject()[PARAM_KEY];
        };
    };

    leadInfoService.$inject = ['$location', '$uibModal', '$timeout', '$window', 'urlHelper'];

    ng.module('leadInfo').service('leadInfoService', leadInfoService);
})(window.angular);
