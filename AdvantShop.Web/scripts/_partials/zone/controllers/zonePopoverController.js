(function (ng) {
    'use strict';

    var ZonePopoverCtrl = function ($cookies, zoneService, popoverService, modalService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            zoneService.zonePopoverShown();
        };

        ctrl.modalsStartWorking = function () {
            modalService.startWorking();
        };

        ctrl.zoneOk = function () {
            popoverService.getPopoverScope('zonePopover').then(function (popoverScope) {
                popoverScope.deactive();
                ctrl.modalsStartWorking();
            });
            zoneService.approveZone();
        };

        ctrl.zoneNo = function () {
            popoverService.getPopoverScope('zonePopover').then(function (popoverScope) {
                popoverScope.deactive();
                zoneService.zoneDialogOpen();
            });
        };
    };

    angular.module('zone').controller('ZonePopoverCtrl', ZonePopoverCtrl);

    ZonePopoverCtrl.$inject = ['$cookies', 'zoneService', 'popoverService', 'modalService'];
})(window.angular);
