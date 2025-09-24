(function (ng) {
    'use strict';

    var SubBlockBookingConstructorController = function ($http, $filter, blocksConstructorService, toaster) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.getAffiliates().then(function (data) {
                if (ctrl.affiliates && ctrl.affiliates.length) {
                    ctrl.affiliate = ctrl.affiliates[0];
                    ctrl.getResources(ctrl.affiliate.value);
                }
            });
        };

        ctrl.onInit = function (settingsResources) {
            ctrl.settingsResources = settingsResources || [];
        };

        ctrl.getAffiliates = function () {
            return $http.get('adminv3/bookingAffiliate/getAffiliates').then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    ctrl.affiliates = data.obj.Affiliates;
                }
            });
        };

        ctrl.getResources = function () {
            if (!ctrl.affiliate) return;
            return $http.get('landingInplace/getReservationResources', { params: { affiliateId: ctrl.affiliate.value } }).then(function (response) {
                var resources = response.data || [];
                // без уже добавленных ресурсов
                ctrl.resources = resources.filter(function (item) {
                    return !ctrl.settingsResources.some(function (sr) {
                        return sr.id == item.Id && sr.affiliateId == ctrl.affiliate.value;
                    });
                });
            });
        };

        ctrl.addResource = function (data, modal) {
            if (!ctrl.affiliate) return;
            if (!ctrl.resource) {
                toaster.error('', 'Выберите ресурс');
                return;
            }

            data.settings.resources = data.settings.resources || [];
            if (
                data.settings.resources.some(function (item) {
                    return item.id == ctrl.resource.Id && item.affiliateId == ctrl.affiliate.value;
                })
            ) {
                toaster.error('', 'Ресурс уже добавлен');
                return;
            }

            modal.close();

            if (ctrl.resources != null) {
                ctrl.resources = ctrl.resources.filter(function (item) {
                    return item.Id != ctrl.resource.Id;
                });
            }

            data.settings.resources.push({
                id: ctrl.resource.Id,
                name: ctrl.resource.Name,
                description: ctrl.resource.Description,
                affiliateId: ctrl.affiliate.value,
                affiliate: ctrl.affiliate.label,
            });
        };
    };

    SubBlockBookingConstructorController.$inject = ['$http', '$filter', 'blocksConstructorService', 'toaster'];

    ng.module('blocksConstructor').controller('SubBlockBookingConstructorController', SubBlockBookingConstructorController);
})(window.angular);
