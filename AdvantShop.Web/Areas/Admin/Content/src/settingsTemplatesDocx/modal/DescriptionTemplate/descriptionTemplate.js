import './descriptionTemplate.html';

(function (ng) {
    'use strict';

    var DescriptionTemplateCtrl = function ($http, $uibModalInstance, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.type = params.type;
            ctrl.profiMode = params.profiMode;

            ctrl.getTemplateDescription();
        };

        ctrl.getTemplateDescription = function () {
            return $http.post('settingsTemplatesDocx/getDescription', { type: ctrl.type }).then(function (response) {
                var data = response.data;

                if (data.result === true) {
                    ctrl.Childs = data.obj.Fields;
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

        ctrl.dismiss = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    DescriptionTemplateCtrl.$inject = ['$http', '$uibModalInstance', 'toaster', '$translate'];

    ng.module('uiModal').controller('DescriptionTemplateCtrl', DescriptionTemplateCtrl);
})(window.angular);
