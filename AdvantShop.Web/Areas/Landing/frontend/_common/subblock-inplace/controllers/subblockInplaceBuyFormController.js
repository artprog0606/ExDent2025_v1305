(function (ng) {
    'use strict';

    var SubblockInplaceBuyFormCtrl = function ($attrs, $window, $http, subblockInplaceService, toaster) {
        var ctrl = this;

        ctrl.modalSettingsBlockData = {
            data: {
                modalData: {},
            },
            backup: {},
        };

        ctrl.showModal = function (modalId) {
            $http.get('landingInplace/getFormSettings', { params: { id: $attrs.formId } }).then(function (response) {
                var data = response.data;

                ctrl.modalSettingsBlockData.data.modalData = ng.extend(ctrl.modalSettingsBlockData.data.modalData, {
                    onApply: ctrl.onApply,
                    onUpdate: ctrl.onUpdate,
                    onCancel: ctrl.onCancel,
                    settings: data,
                    templateUrlByType: 'areas/landing/frontend/blocks/subblock-inplace/templates/subblockModal_buyForm.html',
                });

                subblockInplaceService
                    .showModal(
                        modalId,
                        '<span class="icon-lp-cog-1 block-constructor--modal-header-icon"></span><span class=" block-constructor--modal-header-text">Настройки формы<span>',
                        {
                            modalClass: 'blocks-constructor-modal',
                            modalOverlayClass: 'blocks-constructor-modal-floating-wrap blocks-constructor-modal--settings',
                            isFloating: true,
                            backgroundEnable: false,
                            destroyOnClose: true,
                        },
                        ctrl.modalSettingsBlockData.data,
                    )
                    .then(function () {
                        ctrl.modalSettingsBlockData.backup = ng.copy(ctrl.modalSettingsBlockData.data);
                    });
            });
        };

        ctrl.onUpdate = function (settings) {};

        ctrl.onApply = function (subblockId, settings) {
            $http.post('landingInplace/saveFormSettings', settings.form).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', 'Настройки сохранены');
                    $window.location.reload();
                } else {
                    data.errors.forEach(function (err) {
                        toaster.pop('error', '', err);
                    });
                }
            });
        };

        ctrl.onCancel = function (subblockId, settings) {
            //alert('To do cancel');
            //ctrl.modalSettingsBlockData.data.modalData.settings = ng.extend(ctrl.modalSettingsBlockData.data.modalData.settings, ctrl.modalSettingsBlockData.backup.modalData.settings);
        };
    };

    ng.module('subblockInplace').controller('SubblockInplaceBuyFormCtrl', SubblockInplaceBuyFormCtrl);

    SubblockInplaceBuyFormCtrl.$inject = ['$attrs', '$window', '$http', 'subblockInplaceService', 'toaster'];
})(window.angular);
