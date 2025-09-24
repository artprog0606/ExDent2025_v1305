(function (ng) {
    'use strict';

    var BlocksConstructorButtonSettingsModalCtrl = function ($element, $q, modalService) {
        var ctrl = this;

        ctrl.$postLink = function () {
            $element.on('click', ctrl.open.bind(ctrl));
        };

        ctrl.open = function () {
            var modalId = 'modalButtonSettings';

            var parentData = {
                modalData: {
                    options: {
                        lpId: ctrl.lpId,
                        commonOptions: ng.copy(ctrl.commonOptions),
                        buttonOptions: ng.copy(ctrl.buttonOptions),
                        isVisibility: ctrl.isVisibility,
                        formExclude: ctrl.formExclude,
                        paymentExclude: ctrl.paymentExclude,
                        linkMode: ctrl.linkMode,
                        hideFormSettings: ctrl.hideFormSettings,
                    },
                    onApply: function (result) {
                        if (ctrl.onApply != null) {
                            ctrl.onApply({ result: result });
                        }

                        modalService.close(modalId);
                    },
                },
            };

            modalService.renderModal(
                modalId,
                'Настройка кнопки',
                '<blocks-constructor-button-settings lp-id="modalData.options.lpId" link-mode="' +
                    ctrl.linkMode +
                    '" button-options="modalData.options.buttonOptions" common-options="modalData.options.commonOptions" is-visibility="modalData.options.isVisibility" form-exclude="modalData.options.formExclude" hide-form-settings="modalData.options.hideFormSettings" payment-exclude="modalData.options.paymentExclude" form-settings="modalData.buttonFormSettings"></blocks-constructor-button-settings>',
                '<button type="button" class="blocks-constructor-btn-confirm" ng-click="modalData.onApply(modalData.options)">Сохранить</button><input type="button" class="blocks-constructor-btn-cancel blocks-constructor-btn-mar" data-modal-close="" value="Отмена" />',
                { destroyOnClose: true, modalClass: 'blocks-constructor-button-settings-modal' },
                parentData,
            );

            modalService.getModal(modalId).then(function (modal) {
                modal.modalScope.open();
            });
        };
    };

    ng.module('blocksConstructor').controller('BlocksConstructorButtonSettingsModalCtrl', BlocksConstructorButtonSettingsModalCtrl);

    BlocksConstructorButtonSettingsModalCtrl.$inject = ['$element', '$q', 'modalService'];
})(window.angular);
