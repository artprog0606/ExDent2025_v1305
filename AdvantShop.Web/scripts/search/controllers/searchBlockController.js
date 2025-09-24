/* @ngInject */
var SearchBlockCtrl = function ($window) {
    var ctrl = this;

    ctrl.submit = function (value, redirectOnObj, q) {
        if (value != null && value.length > 0 && value !== q) {
            var resulUrl = redirectOnObj === true ? value : ctrl.url + '?q=' + encodeURIComponent(value);

            if (resulUrl != null && resulUrl.trim().length > 0) {
                $window.location.assign(resulUrl);
            }
        }
    };

    ctrl.aSubmut = function (value, obj) {
        if (obj != null) {
            ctrl.submit(obj.Url, true);
        }
    };
};

angular.module('search').controller('SearchBlockCtrl', SearchBlockCtrl);
