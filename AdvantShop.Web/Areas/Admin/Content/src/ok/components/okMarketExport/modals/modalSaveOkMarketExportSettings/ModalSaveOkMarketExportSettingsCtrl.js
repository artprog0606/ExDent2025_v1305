import './modalSaveOkMarketExportSettings.html';

(function (ng) {
    'use strict';

    var ModalSaveOkMarketExportSettingsCtrl = function (toaster, okMarketService, $uibModalInstance) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.getSettings();
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getSettings = function () {
            okMarketService.getExportSettings().then(function (data) {
                ctrl.settings = data;
            });
        };

        ctrl.save = function () {
            return okMarketService.saveExportSettings(ctrl.settings).then(function (data) {
                if (data.result === true) {
                    toaster.pop('success', '', 'Настройки сохранены');
                    $uibModalInstance.close();
                } else {
                    toaster.pop('error', '', 'Ошибка при сохранении настроек');
                }
            });
        };
    };

    ModalSaveOkMarketExportSettingsCtrl.$inject = ['toaster', 'okMarketService', '$uibModalInstance'];

    ng.module('uiModal').controller('ModalSaveOkMarketExportSettingsCtrl', ModalSaveOkMarketExportSettingsCtrl);
})(window.angular);
