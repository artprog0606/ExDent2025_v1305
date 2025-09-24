(function (ng) {
    'use strict';

    var LpSettingsTriggerCtrl = function ($controller, $window, $http, toaster, modalService) {
        var ctrl = this;

        ctrl.showModal = function (lpId) {
            modalService.renderModal(
                'lpSettings',
                'Общие настройки',
                '<div ng-include="\'areas/landing/frontend/_common/lp-settings/templates/lp-settings.html\'"></div>',
                '<div><button type="button" class="blocks-constructor-btn-confirm" ng-click="lpSettings.saveSettings()">Сохранить</button><button type="button"  class="blocks-constructor-btn-cancel blocks-constructor-btn-mar" data-modal-close="" data-modal-close-callback="modal.close()">Отмена</button></div>',
                {
                    modalClass: 'lp-settings-modal',
                    modalOverlayClass: 'lp-settings-modal-overlay',
                    isFloating: true,
                    backgroundEnable: false,
                    callbackClose: 'lpSettings.callbackClose()',
                },
                { lpId: lpId, lpSettings: $controller('LpSettingsCtrl') },
            );

            modalService.getModal('lpSettings').then(function (modal) {
                modal.modalScope.open();
            });
        };
    };

    ng.module('lpSettings').controller('LpSettingsTriggerCtrl', LpSettingsTriggerCtrl);

    LpSettingsTriggerCtrl.$inject = ['$controller', '$window', '$http', 'toaster', 'modalService'];
})(window.angular);
