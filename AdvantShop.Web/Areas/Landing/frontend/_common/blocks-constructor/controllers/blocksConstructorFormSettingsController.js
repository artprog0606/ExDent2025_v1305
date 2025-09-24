(function (ng) {
    'use strict';

    var BlocksConstructorFormSettingsCtrl = function (modalService, $q, $http) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.tabs = [];

            ctrl.isMobile = window.matchMedia('(max-width: 768px)').matches;
        };

        ctrl.onChangeCrm = function (row, col, index) {
            var itemCrm;

            for (var i = 0, len = col.selectData.length; i < len; i++) {
                if (row.TitleCrm === col.selectData[i].Title) {
                    itemCrm = col.selectData[i];
                    break;
                }
            }
            if (!itemCrm) itemCrm = {};

            row.FieldType = itemCrm.FieldType;
            row.Type = itemCrm.Type;
            row.CustomFieldId = itemCrm.CustomFieldId;

            return row;
        };

        ctrl.addEditLandingEmailTemplate = function (id) {
            var defer = $q.defer(),
                promise;

            if (id != null) {
                promise = ctrl.getLandingEmailTemplate(id);
            } else {
                promise = defer.promise;
                defer.resolve();
            }

            promise.then(function (data) {
                data = data || { SendingTime: 15 };

                var parentData = {
                    modalData: {
                        save: ctrl.addNewLandingEmailTemplate,
                        emailData: {
                            id: id,
                            blockId: ctrl.commonOptions.BlockId,
                            subject: data.Subject,
                            body: data.Body,
                            sendingTime: data.SendingTime,
                        },
                    },
                };

                modalService.renderModal(
                    'modalAddLandingEmailTemplate',
                    id == null ? 'Новое письмо' : 'Редактирование письма',
                    '<div ng-include="\'areas/landing/frontend/_common/blocks-constructor/templates/formLandingEmailTemplate.html\'"></div>',
                    '<input type="button" class="blocks-constructor-btn-confirm" data-ng-click="modalData.save(modalData.emailData)" value="Сохранить" />' +
                        '<input type="button" class="blocks-constructor-btn-cancel blocks-constructor-btn-mar" data-modal-close="modalAddLandingEmailTemplate" value="Отмена" />',
                    {
                        modalClass: 'blocks-constructor-modal',
                        modalOverlayClass: 'blocks-constructor-modal-floating-wrap blocks-constructor-modal--settings',
                        isFloating: true,
                        backgroundEnable: false,
                        destroyOnClose: true,
                    },
                    parentData,
                );

                modalService.getModal('modalAddLandingEmailTemplate').then(function (modal) {
                    modal.modalScope.open();
                });
            });
        };

        ctrl.addNewLandingEmailTemplate = function (modalData) {
            $http.post('landingInplace/saveLandingEmailTemplate', modalData).then(function (response) {
                var data = response.data;
                if (data.result) {
                    modalService.close('modalAddLandingEmailTemplate');
                } else {
                    // show errors
                }
                ctrl.getLandingEmailTemplates();
            });
        };

        ctrl.getLandingEmailTemplate = function (id) {
            return $http.get('landingInplace/getLandingEmailTemplate', { params: { id: id } }).then(function (response) {
                return response.data;
            });
        };

        ctrl.deleteLandingEmailTemplate = function (item) {
            $http.post('landingInplace/deleteLandingEmailTemplate', { id: item.Id }).then(function (response) {
                ctrl.getLandingEmailTemplates();
            });
        };

        ctrl.getLandingEmailTemplates = function () {
            $http.get('landingInplace/getLandingEmailTemplates', { params: { blockId: ctrl.commonOptions.BlockId } }).then(function (response) {
                ctrl.landingEmailTemplates = response.data;
            });
        };

        ctrl.addTab = function (header, content, scope) {
            ctrl.tabs.push({
                header: header,
                content: content,
                scope: scope,
            });
        };

        ctrl.onUploadPicture = function (picture, result) {
            picture.Src = result.picture;
            picture.Type = result.type != null ? result.type : 'image';
        };
        ctrl.onResizePicture = function (picture, result) {
            ng.extend(picture, result);
        };
        ctrl.onDeletePicture = function (picture, result) {
            if (result.result) {
                picture = result.picture;
            } else {
                console.error(result.error);
            }
        };

        ctrl.getCrmFields = function () {
            $http
                .get('landingInplace/getCrmFields', {
                    params: { salesFunnelId: ctrl.commonOptions.Form.SalesFunnelId },
                })
                .then(function (response) {
                    ctrl.commonOptions.CrmFields = response.data;
                });
        };
    };

    ng.module('blocksConstructor').controller('BlocksConstructorFormSettingsCtrl', BlocksConstructorFormSettingsCtrl);

    BlocksConstructorFormSettingsCtrl.$inject = ['modalService', '$q', '$http'];
})(window.angular);
