(function (ng) {
    'use strict';

    var PartnersReportCtrl = function () {
        this.formSubmit = function () {
            document.getElementById('GetPartnersReport').submit();
        };
    };

    PartnersReportCtrl.$inject = [];

    ng.module('partnersReport', []).controller('PartnersReportCtrl', PartnersReportCtrl);
})(window.angular);
