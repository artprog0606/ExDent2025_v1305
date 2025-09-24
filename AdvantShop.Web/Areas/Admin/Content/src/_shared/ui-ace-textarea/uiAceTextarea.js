import uiAceTemplate from './templates/ui-ace-textarea.html';

(function (ng) {
    'use strict';

    var UiAceTextareaCtrl = function (uiAceDefaultOptions, urlHelper) {
        var ctrl = this,
            callbackOnLoad,
            callbackOnChange;

        ctrl.$onInit = function () {
            ctrl._uiAceOptions = ng.extend({}, uiAceDefaultOptions, ctrl.uiAceOptions);

            if (ctrl._uiAceOptions.onLoad != null) {
                callbackOnLoad = ctrl._uiAceOptions.onLoad;
            }

            if (ctrl._uiAceOptions.onChange != null) {
                callbackOnChange = ctrl._uiAceOptions.onChange;
            }

            ctrl._uiAceOptions.onLoad = ctrl._onInitUiAce;
            ctrl._uiAceOptions.onChange = ctrl._onChangeUiAce;
        };

        ctrl._onInitUiAce = function (editor) {
            editor.setShowPrintMargin(false);

            if (callbackOnLoad != null) {
                callbackOnLoad.bind(ctrl, editor);
            }

            if (ctrl.ngModelCtrl != null) {
                if (ctrl.ngModelCtrl.$viewValue != null && ng.isString(ctrl.ngModelCtrl.$viewValue) === true) {
                    editor.setValue(ctrl.ngModelCtrl.$viewValue, -1);
                }

                ctrl.ngModelCtrl.$render = function () {
                    if (ctrl.ngModelCtrl.$viewValue != null && ng.isString(ctrl.ngModelCtrl.$viewValue) === true) {
                        editor.setValue(ctrl.ngModelCtrl.$viewValue, -1);
                    }
                };
            }
        };

        ctrl._onChangeUiAce = function (e) {
            var editor = e[1];

            if (ctrl.ngModelCtrl != null) {
                ctrl.ngModelCtrl.$setViewValue(editor.getValue());
            }

            if (callbackOnChange != null) {
                callbackOnChange.bind(ctrl, editor);
            }
        };

        ctrl.prepereLazyLoadUrl = function (params) {
            for (var i = 0, len = params.length; i < len; i++) {
                params[i] = urlHelper.getAbsUrl(params[i], true);
            }
            return params;
        };
    };

    UiAceTextareaCtrl.$inject = ['uiAceDefaultOptions', 'urlHelper'];

    ng.module('uiAceTextarea', [])
        .controller('UiAceTextareaCtrl', UiAceTextareaCtrl)
        .component('uiAceTextarea', {
            require: {
                ngModelCtrl: '?ngModel',
            },
            templateUrl: uiAceTemplate,
            controller: 'UiAceTextareaCtrl',
            transclude: true,
            bindings: {
                uiAceTextareaOptions: '<?',
                uiAceOptions: '<?',
            },
        });
})(window.angular);
