(function (ng) {
    'use strict';

    var TechDomainCtrl = function () {
        var ctrl = this;

        ctrl.$onInit = function () {
            let today = new Date();

            let storageShowMessageDate = localStorage.getItem('dateWhenShowMessage');

            if (storageShowMessageDate != null) {
                if (new Date(storageShowMessageDate) - today <= 0) {
                    ctrl.isShowMessage = true;
                    localStorage.removeItem('dateWhenShowMessage');
                } else {
                    ctrl.isShowMessage = false;
                }
            } else {
                ctrl.isShowMessage = true;
                localStorage.removeItem('dateWhenShowMessage');
            }
        };

        ctrl.closeDomainInfo = function () {
            ctrl.isShowMessage = false;

            let clickCloseDate = new Date();
            let dateWhenShowMessage = new Date(
                clickCloseDate.getFullYear(),
                clickCloseDate.getMonth(),
                clickCloseDate.getDate() + 3,
                clickCloseDate.getHours(),
                clickCloseDate.getMinutes(),
            );
            localStorage.setItem('dateWhenShowMessage', dateWhenShowMessage);
        };
    };

    angular.module('techDomain', []).controller('TechDomainCtrl', TechDomainCtrl);

    TechDomainCtrl.$inject = [];
})(window.angular);
