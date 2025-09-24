import socialTemplate from '../templates/partials/_social.html';
import carouselTemplate from '../templates/partials/_carousel.html';
import generalRightTemplate from '../templates/partials/_general-right.html';
import generalTemplate from '../templates/partials/_general.html';
import blocks from '../templates/blocks/blocks.js';
(function (ng) {
    'use strict';
    /* @ngInject */
    var BlocksConstructorCtrl = function (
        $window,
        modalService,
        blocksConstructorService,
        Upload,
        toaster,
        $q,
        $element,
        $transclude,
        $scope,
        tabsService,
        $timeout,
        $location,
    ) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.isMobile = window.matchMedia('(max-width: 768px)').matches;

            ctrl.modalSettingsBlockData = {};
            ctrl.modalAddSubblockData = {};

            blocksConstructorService.getBlockConstructorContainer(ctrl.blockId).then(function (blocksConstructorContainerCtrl) {
                ctrl.blocksConstructorContainerCtrl = blocksConstructorContainerCtrl;
            });

            if (ctrl.templateCustom) {
                $transclude($scope, function (clone, scope) {
                    scope.blocksConstructor = ctrl;
                    $element.html(clone);
                });
            }
        };

        ctrl.addBlock = function (top) {
            var parentData = {
                modalData: {
                    landingpageId: ctrl.landingpageId,
                    blockId: ctrl.blockId,
                    sortOrder: ctrl.sortOrder,
                    onApplyNewBlock: function onApplyNewBlock(blockName, sortOrder, top, blockId) {
                        blocksConstructorService
                            .addBlock(ctrl.landingpageId, blockName, sortOrder, null, top, blockId)
                            .then(function (response) {
                                if (response.result === true) {
                                    $window.location.reload();
                                } else {
                                    return $q.reject('Ошибка при добавлении блока');
                                }
                            })
                            .catch(function (err) {
                                toaster.error('Ошибка при добавлении блока');
                            });
                    },
                    onApplyByCategories: function onApplyByCategories(blocks, sortOrder, top, blockId) {
                        blocksConstructorService
                            .addListBlock(ctrl.landingpageId, blocks, sortOrder, null, top, blockId)
                            .then(function (response) {
                                if (response.result === true) {
                                    $window.location.reload();
                                } else {
                                    return $q.reject('Ошибка при добавлении блоков');
                                }
                            })
                            .catch(function (err) {
                                toaster.error('Ошибка при добавлении блоков');
                            });
                    },
                    onRemoveByCategories: function (category) {
                        blocksConstructorService
                            .removeAllBlockByCategory(ctrl.landingpageId, category)
                            .then(function (response) {
                                if (response.result === true) {
                                    $window.location.reload();
                                } else {
                                    return $q.reject('Ошибка при удалении блоков');
                                }
                            })
                            .catch(function (err) {
                                toaster.error('Ошибка при удалении блоков');
                            });
                    },
                    top: top,
                    experemental: $window.location.search.indexOf('addexp') !== -1,
                },
            };

            if (modalService.hasModal('modalNewBlock') === false) {
                modalService.renderModal(
                    'modalNewBlock',
                    'Добавить новый блок',
                    '<blocks-constructor-modal-new-block data-modal-data="modalData" data-on-apply="modalData.onApplyNewBlock(blockName, sortOrder, top, blockId)" data-on-apply-by-categories="modalData.onApplyByCategories(blocks, sortOrder, top, blockId)" experemental="modalData.experemental" on-remove-by-categories="modalData.onRemoveByCategories(categoryName)" />',
                    null,
                    {
                        modalClass: 'blocks-constructor-modal blocks-constructor-modal-new-item',
                        modalOverlayClass: 'blocks-constructor-modal-new-item-overlay',
                    },
                    parentData,
                );
            }

            modalService.getModal('modalNewBlock').then(function (modal) {
                modal.modalScope.open();
            });
        };

        ctrl.showOptionsBlock = function (tabId) {
            ctrl.blocksConstructorContainerCtrl.getData().then(function (blockOptions) {
                var modalId = 'modalSettingsBlock_' + ctrl.blockId;

                var backup = ng.copy(blockOptions);

                ctrl.addCallback = function (callback) {
                    ctrl.callback = callback;
                };

                ctrl.modalSettingsBlockData.data = {
                    modalData: {
                        disallowSave: false,
                        applySettingsInProcess: false,
                        landingpageId: ctrl.landingpageId,
                        blockId: ctrl.blockId,
                        name: ctrl.name,
                        type: ctrl.type,
                        sortOrder: ctrl.sortOrder,
                        settings: blockOptions.Settings,
                        data: blockOptions,
                        templateUrlByType: blocks.get(ctrl.type),
                        generalOptionsTemplateUrl: generalTemplate,
                        generalRightOptionsTemplateUrl: generalRightTemplate,
                        carouselOptionsTemplateUrl: carouselTemplate,
                        socialTemplateUrl: socialTemplate,
                        uploadFileBackground: ctrl.uploadFileBackground,
                        addCallback: ctrl.addCallback,
                        onApplySettings: function onApplySettings(blockId, data, modalData) {
                            modalData.applySettingsInProcess = true;

                            $q.when(ctrl.callback != null ? ctrl.callback(data) : true)
                                .then(function () {
                                    if (modalData.pictureLoaderSaveFn != null) {
                                        return modalData.pictureLoaderSaveFn();
                                    }

                                    return true;
                                })
                                .then(function () {
                                    return blocksConstructorService.saveBlockSettings(blockId, data);
                                })
                                .then(function (response) {
                                    if (response.result === true) {
                                        $window.location.hash = '#block_' + blockId;
                                        $window.location.reload(true);
                                    } else {
                                        toaster.error('Ошибка при применении настроек');
                                    }

                                    //if (ctrl.blocksConstructorContainerCtrl.callbacks != null) {
                                    //    ctrl.blocksConstructorContainerCtrl.callbacks();
                                    //}
                                })
                                .catch(function () {
                                    toaster.error('Ошибка при применении настроек');
                                })
                                .finally(function () {
                                    modalData.applySettingsInProcess = false;
                                });
                        },
                        onCancel: function onCancel() {
                            //if (ctrl.modalSettingsBlockData.data.modalData.settings.background_settings != null && ctrl.modalSettingsBlockData.data.modalData.settings.background_settings.parallax) {
                            //    $('#block_' + ctrl.blockId + '_inner').enllax('destroy');
                            //}
                            blockOptions = ng.extend(blockOptions, backup);
                        },
                        colorSchemeList: blocksConstructorService.getColorSchemeList(),
                        onModalClose: function () {
                            $timeout(() => $location.hash(''));
                        },
                    },
                };
                //if (modalService.hasModal(modalId) === false) {
                modalService.renderModal(
                    modalId,
                    'Настройки блока',
                    '<blocks-constructor-modal-settings-block data-modal-data="modalData" data-on-apply="modalData.onApplySettings(blockId, data)" data-on-upload-file-background="modalData.onUploadFileBackground($file)" data-on-update-file-background="modalData.onUpdateFileBackground(result, data, file)" data-on-delete-file-background="modalData.onDeleteFileBackground()" data-on-cancel="modalData.onCancel()" in-progress="modalData.disallowSave" />',
                    '<button type="button" ladda="modalData.applySettingsInProcess" class="blocks-constructor-btn-confirm" data-e2e="SaveSettingsBtn" data-button-validation data-button-validation-success="modalData.onApplySettings(modalData.blockId, modalData.data, modalData)" ng-disabled="modalData.disallowSave">Сохранить</button><input type="button" class="blocks-constructor-btn-cancel blocks-constructor-btn-mar" data-e2e="CancelSettingsBtn" data-modal-close="" data-modal-close-callback="modalData.onCancel()" value="Отмена" />',
                    {
                        modalClass: 'blocks-constructor-modal',
                        modalOverlayClass: 'blocks-constructor-modal-floating-wrap blocks-constructor-modal--settings',
                        isFloating: true,
                        backgroundEnable: false,
                        destroyOnClose: true,
                        closeEsc: false,
                        callbackClose: 'modalData.onModalClose()',
                    },
                    ng.extend($scope, ctrl.modalSettingsBlockData.data),
                );
                //}

                modalService.getModal(modalId).then(function (modal) {
                    modal.modalScope.open();

                    if (tabId != null) {
                        $timeout(function () {
                            tabsService.change(tabId);
                        }, 100);
                    }
                });
            });
        };

        ctrl.moveUpBlock = function () {
            blocksConstructorService
                .saveBlockSortOrder(ctrl.blockId, true)
                .then(function (response) {
                    if (response.result === true) {
                        ctrl.blocksConstructorContainerCtrl.moveUpBlock();
                    } else {
                        toaster.error('Ошибка при перемещения блока');
                    }
                })
                .catch(function () {
                    toaster.error('Ошибка при перемещения блока');
                });
        };

        ctrl.moveDownBlock = function () {
            blocksConstructorService
                .saveBlockSortOrder(ctrl.blockId, false)
                .then(function (response) {
                    if (response.result === true) {
                        ctrl.blocksConstructorContainerCtrl.moveDownBlock();
                    } else {
                        alert('Ошибка при перемещении блока вниз');
                    }
                })
                .catch(function () {
                    toaster.error('Ошибка при перемещении блока вниз');
                });
        };

        ctrl.removeBlock = function () {
            blocksConstructorService
                .removeBlock(ctrl.blockId)
                .then(function (response) {
                    if (response.result === true) {
                        ctrl.blocksConstructorContainerCtrl.removeBlock();
                    } else {
                        toaster.error('Ошибка при удалении блока');
                    }
                })
                .catch(function () {
                    toaster.error('Ошибка при удалении блока');
                });
        };
    };

    ng.module('blocksConstructor').controller('BlocksConstructorCtrl', BlocksConstructorCtrl);
})(window.angular);
