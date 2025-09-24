(function (ng) {
    'use strict';

    var ModalAddEditTemplateCtrl = function ($http, $uibModalInstance, toaster, Upload, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.id = params != null && params.id != null ? params.id : 0;
            ctrl.mode = ctrl.id != 0 ? 'edit' : 'add';

            ctrl.getTemplateForm().then(function (result) {
                if (result && ctrl.mode === 'edit') {
                    ctrl.getTemplate();
                }
            });
        };

        ctrl.getTemplate = function () {
            return $http.post('settingsTemplatesDocx/get', { id: ctrl.id }).then(function (response) {
                var data = response.data;

                if (data.result === true) {
                    ctrl.type = data.obj.Type.toString();
                    ctrl.name = data.obj.Name;
                    ctrl.sortOrder = data.obj.SortOrder;
                    ctrl.debugMode = data.obj.DebugMode;
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });

                    if (!data.errors) {
                        toaster.pop('error', 'Ошибка', 'Ошибка при загрузке данных');
                    }
                    ctrl.dismiss();
                }
            });
        };

        ctrl.getTemplateForm = function () {
            return $http.get('settingsTemplatesDocx/getTemplateForm').then(function (response) {
                var data = response.data;

                if (data.result === true) {
                    ctrl.types = data.obj.Types;
                    ctrl.fileUploadHelpText = data.obj.FileUploadHelpText;

                    return true;
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });

                    if (!data.errors) {
                        toaster.pop('error', 'Ошибка', 'Ошибка при загрузке данных');
                    }
                    ctrl.dismiss();
                }

                return false;
            });
        };

        ctrl.selectFile = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (($event.type === 'change' || $event.type === 'drop') && $files != null && $files.length > 0) {
                ctrl.files = $files;
            } else if ($invalidFiles.length > 0) {
                toaster.pop('error', 'Файл не соответствует требованиям');
            }
        };

        ctrl.save = function () {
            var params = {
                id: ctrl.id,
                type: ctrl.type,
                name: ctrl.name,
                sortOrder: ctrl.sortOrder,
                debugMode: ctrl.debugMode,
            };
            var url = ctrl.mode === 'add' ? 'settingsTemplatesDocx/addTemplate' : 'settingsTemplatesDocx/updateTemplate';

            var promise = null;

            if (ctrl.files === null) {
                promise = $http.post(url, params);
            } else {
                promise = Upload.upload({ url: url, data: params, file: ctrl.files });
            }

            promise.then(function (result) {
                var data = result.data;

                if (data.result === true) {
                    toaster.pop('success', '', 'Изменения сохранены');

                    $uibModalInstance.close('saveTemplateDocx');
                } else {
                    ctrl.btnLoading = false;

                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });

                    if (!data.errors) {
                        toaster.pop('error', 'Ошибка', 'Ошибка при сохранении данных шаблона');
                    }
                }
            });
        };

        ctrl.dismiss = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalAddEditTemplateCtrl.$inject = ['$http', '$uibModalInstance', 'toaster', 'Upload', '$translate'];

    ng.module('uiModal').controller('ModalAddEditTemplateCtrl', ModalAddEditTemplateCtrl);
})(window.angular);
