(function (ng) {
    'use strict';

    var SubblockInplaceVideoCtrl = function ($window, subblockInplaceService) {
        var ctrl = this;

        ctrl.modalSettingsBlockData = {};

        ctrl.showModal = function (modalId) {
            ctrl.modalSettingsBlockData.data = {
                modalData: {
                    subblockId: ctrl.subblockId,
                    onApply: ctrl.onApply,
                    onUpdate: ctrl.onUpdate,
                    onCancel: ctrl.onCancel,
                    settings: ctrl.settings,
                    templateUrlByType: 'areas/landing/frontend/blocks/subblock-inplace/templates/subblockModal_video.html',
                },
            };

            subblockInplaceService.showModal(modalId, 'Настройки видео', null, ctrl.modalSettingsBlockData.data).then(function () {
                ctrl.modalSettingsBlockData.backup = ng.copy(ctrl.modalSettingsBlockData.data);
            });
        };

        ctrl.onUpdate = function (settings) {
            ctrl.settings = settings;
        };

        ctrl.onApply = function (subblockId, settings) {
            subblockInplaceService.updateSubBlockSettings(subblockId, settings).then(function (response) {
                if (response.result === true) {
                    $window.location.reload();
                } else {
                    alert('Ошибка при сохранении настроек видео');
                }
            });
        };

        ctrl.onCancel = function (subblockId, settings) {
            ctrl.modalSettingsBlockData.data.modalData.settings = ng.extend(
                ctrl.modalSettingsBlockData.data.modalData.settings,
                ctrl.modalSettingsBlockData.backup.modalData.settings,
            );
        };
    };

    ng.module('subblockInplace').controller('SubblockInplaceVideoCtrl', SubblockInplaceVideoCtrl);

    SubblockInplaceVideoCtrl.$inject = ['$window', 'subblockInplaceService'];
})(window.angular);
