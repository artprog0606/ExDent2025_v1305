import './addPointDelivery.html';
(function (ng) {
    'use strict';

    var ModalAddPointDeliveryCtrl = function ($uibModalInstance) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.mode = (ctrl.$resolve ? ctrl.$resolve.point : null) ? 'edit' : 'add';
            ctrl.point = (ctrl.$resolve.point ? ng.extend({}, ctrl.$resolve.point) : null) || {
                Latitude: 0.0,
                Longitude: 0.0,
            };
            ctrl.point.position = ctrl.point.Latitude + ', ' + ctrl.point.Longitude;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function () {
            var coordinats = ctrl.point.position.replace(/\[|\]/g, '').split(',');
            ctrl.point.Latitude = parseFloat(coordinats[0].trim());
            ctrl.point.Longitude = parseFloat(coordinats[1].trim());
            delete ctrl.point.position;

            $uibModalInstance.close(ctrl.point);
        };
    };

    ModalAddPointDeliveryCtrl.$inject = ['$uibModalInstance'];

    ng.module('uiModal').controller('ModalAddPointDeliveryCtrl', ModalAddPointDeliveryCtrl);
})(window.angular);
