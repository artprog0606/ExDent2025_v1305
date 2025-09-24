//#region module

(function (ng) {
    'use strict';

    angular.module('mobileOverlap', ['ngCookies', 'urlHelper']);
})(window.angular);

//#endregion

//#region controller

(function (ng) {
    'use strict';

    var mobileOverlapCtrl = function ($location, $cookies, $timeout, $http, urlHelper) {
        var ctrl = this;

        ctrl.goToDesktop = function (name, value) {
            // $cookies.remove('deviceMode');
            // $cookies.put('deviceMode', 'desktop');
            if (name != null && value != null) {
                $cookies.remove(name);
                $cookies.put(name, value);
                ctrl.resetLastModified()
                    .then((data) => {
                        // window.location = $location.absUrl();
                        location.reload(true);
                    })
                    .catch((error) => {
                        console.error(error);
                    });
            }
        };

        ctrl.goToMobile = function (name, value) {
            // $cookies.remove('deviceMode');
            // $cookies.put('deviceMode', 'mobile');
            $cookies.remove(name);
            $cookies.put(name, value);
            ctrl.resetLastModified()
                .then((data) => {
                    location.reload(true);
                    // window.location = $location.absUrl();
                })
                .catch((error) => {
                    console.error(error);
                });
        };

        ctrl.stayOnDesktop = function (name, value) {
            $cookies.put(name, value);
            document.documentElement.classList.remove('mobile-redirect-panel');
            //$element.remove();
        };

        ctrl.stayOnMobile = function (name, value) {
            $cookies.put(name, value);
            document.documentElement.classList.remove('desktop-redirect-panel');
            //$element.remove();
        };

        ctrl.resetLastModified = function () {
            //чистим cache

            const url = urlHelper.getAbsUrl('/common/resetLastModified', true);
            return $http.post(url);
        };
    };

    angular.module('mobileOverlap').controller('mobileOverlapCtrl', mobileOverlapCtrl);

    mobileOverlapCtrl.$inject = ['$location', '$cookies', '$timeout', '$http', 'urlHelper'];
})(window.angular);

//#endregion
