(function (ng) {
    'use strict';

    var SubblockInplaceBuyFormModalCtrl = function (subblockInplaceBackgroundColors, subblockInplaceService, $http, toaster) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.NewTitle = 'Новое поле';

            ctrl.menuSortableOptions = {
                containment: '#modalFormList',
                scrollableContainer: '#modalFormList',
                containerPositioning: 'relative',
                orderChanged: function (event) {
                    //blocksConstructorService.saveBlockSettings(ctrl.modalData.blockId, ctrl.modalData.data)
                    //    .then(function (data) {
                    //        if (data.result === true) {
                    //            toaster.pop('success', 'Сортировка пунктов меню успешно сохранена');
                    //        } else {
                    //            throw new Error();
                    //        }
                    //    })
                    //    .catch(function (result) {
                    //        toaster.pop('error', 'Ошибка при сохранении сортировки пунктов меню');
                    //    });
                },
            };

            if (ctrl.settings.form.SalesFunnelId == null && ctrl.settings.salesFunnels != null && ctrl.settings.salesFunnels.length > 0) {
                ctrl.settings.form.SalesFunnelId = ctrl.settings.salesFunnels[0].Id;
            }
        };

        ctrl.removeField = function (item) {
            var index = ctrl.settings.form.Fields.indexOf(item);

            if (index !== -1) {
                ctrl.settings.form.Fields.splice(index, 1);
            }
        };

        ctrl.addField = function (title, item) {
            if (title == null || title.length === 0 || item == null) {
                return;
            }

            var field = {
                Title: title,
                TitleCrm: item.Title,
                FieldType: item.FieldType,
                Type: item.Type,
                CustomFieldId: item.CustomFieldId,
            };

            ctrl.settings.form.Fields.push(field);

            ctrl.NewTitle = 'Новое поле';
            ctrl.NewTitleCrm = ctrl.settings.crmFields[0];
        };
    };

    ng.module('subblockInplace').controller('SubblockInplaceBuyFormModalCtrl', SubblockInplaceBuyFormModalCtrl);

    SubblockInplaceBuyFormModalCtrl.$inject = ['subblockInplaceBackgroundColors', 'subblockInplaceService', '$http', 'toaster'];
})(window.angular);
