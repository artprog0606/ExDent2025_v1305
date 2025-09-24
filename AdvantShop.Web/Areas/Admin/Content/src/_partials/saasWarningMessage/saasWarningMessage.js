(function (ng) {
    'use strict';

    var SaasWarningMessageCtrl = function ($http) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.isHidden = false;
        };

        ctrl.close = function () {
            ctrl.isHidden = true;
            $http.post('common/saasWarningMessageClose');
        };
    };

    SaasWarningMessageCtrl.$inject = ['$http'];

    ng.module('saasWarningMessage', []).controller('SaasWarningMessageCtrl', SaasWarningMessageCtrl);
})(window.angular);
