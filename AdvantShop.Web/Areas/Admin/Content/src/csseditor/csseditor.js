(function (ng) {
    'use strict';

    var CssEditorCtrl = function ($http, $q, toaster, $translate) {
        var ctrl = this;

        ctrl.save = function () {
            $http.post('design/cssEditor', { value: ctrl.text }).then(function (response) {
                if (response.data.result) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Csseditor.ChangesSaved'));
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.Csseditor.ChangesNotSaved'));
                }
            });
        };

        $(window).bind('keydown', function (event) {
            if (event.ctrlKey || event.metaKey) {
                switch (String.fromCharCode(event.which).toLowerCase()) {
                    case 's':
                        event.preventDefault();
                        //$("form[name='form']").submit();
                        ctrl.save();
                        break;
                }
            }
        });
    };

    CssEditorCtrl.$inject = ['$http', '$q', 'toaster', '$translate'];

    ng.module('csseditor', []).controller('CssEditorCtrl', CssEditorCtrl);
})(window.angular);
