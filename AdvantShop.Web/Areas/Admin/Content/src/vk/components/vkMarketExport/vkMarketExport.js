(function (ng) {
    'use strict';

    var VkMarketExportCtrl = function (toaster, vkMarketService) {
        var ctrl = this;

        ctrl.export = function () {
            vkMarketService.export().then(function (data) {
                if (data.result === true) {
                    toaster.pop('success', '', 'Начался перенос товаров в ВКонткате. Длительность переноса зависит от кол-ва товаров и фотографий.');
                } else {
                    data.errors.forEach(function (e) {
                        toaster.pop('error', '', e);
                    });
                }
            });
        };
    };

    VkMarketExportCtrl.$inject = ['toaster', 'vkMarketService'];

    ng.module('vkMarketExport', []).controller('VkMarketExportCtrl', VkMarketExportCtrl);
})(window.angular);
