(function (ng) {
    'use strict';

    var PARAM_KEY = 'partnerIdInfo';

    var partnerInfoService = function ($location, $http, $q, $window, $uibModal, $timeout) {
        var service = this,
            arrayDefers = [];

        service.initContainer = function (container) {
            arrayDefers.forEach(function (defer) {
                defer.resolve(container);
            });
        };

        service.addEditPartner = function (params, options) {
            var scrollTop = $window.pageYOffset;
            return $uibModal
                .open({
                    component: 'partnerInfo',
                    controllerAs: '$ctrl',
                    windowClass: 'lead-info',
                    openedClass: 'modal-open lead-info-modal--open',
                    resolve: { params: params },
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
                    service.removeUrlParam(params);
                    $timeout(function () {
                        $window.scrollTo(0, scrollTop);
                    }, 0);
                });
        };

        service.setUrlParam = function (partnerid) {
            $location.search(PARAM_KEY, partnerid);
        };

        service.removeUrlParam = function () {
            $location.search(PARAM_KEY, null);
        };

        service.getUrlParam = function () {
            var search = $location.search();
            return search != null && search[PARAM_KEY] != null ? search[PARAM_KEY] : null;
        };
    };

    partnerInfoService.$inject = ['$location', '$http', '$q', '$window', '$uibModal', '$timeout'];

    ng.module('partnerInfo').service('partnerInfoService', partnerInfoService);
})(window.angular);
