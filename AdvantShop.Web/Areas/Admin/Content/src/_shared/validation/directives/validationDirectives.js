import validationListItemTemplate from './../templates/validationListItem.html';
import validationListTemplate from './../templates/validationList.html';
import validationOutputTemplate from './../templates/validationOutput.html';

(function (ng) {
    'use strict';

    ng.module('validation').directive('buttonValidation', [
        '$parse',
        'domService',
        'toaster',
        '$translate',
        '$filter',
        function ($parse, domService, toaster, $translate, $filter) {
            return {
                restrict: 'A',
                require: {
                    form: '?^form',
                },
                link: function (scope, element, attrs, ctrls) {
                    if (ctrls.form == null && (attrs.formCtrl == null || attrs.formCtrl.length === 0)) {
                        throw Error('Need parent from or set attribute "form" for buttonValidation directive');
                    }
                    var FormCtrl = ctrls.form || $parse(attrs.formCtrl)(scope),
                        customValidFunc = $parse(attrs.buttonValidation),
                        startFunc = $parse(attrs.buttonValidationStart),
                        successFunc = $parse(attrs.buttonValidationSuccess),
                        formNames = $parse(attrs.buttonValidationForms);

                    function validate(event) {
                        scope.clickEvent = event;
                        scope.FormCtrl = FormCtrl;
                        startFunc(scope);
                        if (FormCtrl.$invalid === true || customValidFunc(scope) === false) {
                            event.preventDefault();
                            FormCtrl.$setSubmitted();
                            FormCtrl.$setDirty();
                            var form = findForm(event, formNames(scope));
                            if (form != null) {
                                var invalidElementFocus = form.querySelector('.ng-invalid:not(form)');
                                if (invalidElementFocus != null) {
                                    invalidElementFocus.focus();
                                }
                            }
                            toaster.pop({
                                type: 'error',
                                title: $translate.instant('Admin.Js.Validation.ErrorEnteringData'),
                                body: 'validation-output',
                                bodyOutputType: 'directive',
                                directiveData: {
                                    errors: $filter('validationUnique')(FormCtrl[attrs.form] ? FormCtrl[attrs.form].$error : FormCtrl.$error),
                                },
                                toasterId: 'toasterContainerAlternative',
                                timeout: 5000,
                            });
                        } else {
                            successFunc(scope);
                        }
                        scope.$apply();
                    }

                    function findForm(event, formNames) {
                        var currentFrom;
                        if (formNames != null) {
                            for (var i = 0, len = formNames.length; i < len; i++) {
                                if (document.forms[formNames[i]].classList.contains('ng-invalid')) {
                                    currentFrom = document.forms[formNames[i]];
                                    break;
                                }
                            }
                        } else {
                            currentFrom =
                                document.getElementById(event.target.getAttribute('form')) ||
                                domService.closest(event.target, 'ng-form') ||
                                domService.closest(event.target, 'form') ||
                                document.querySelector('form');
                        }
                        return currentFrom;
                    }

                    element[0].addEventListener('click', validate);
                },
            };
        },
    ]);
    ng.module('validation').directive('validationTabIndex', [
        '$parse',
        function ($parse) {
            return {
                controller: [
                    '$attrs',
                    '$parse',
                    '$scope',
                    function ($attrs, $parse, $scope) {
                        this.validationTabIndex = $parse($attrs.validationTabIndex)($scope);
                    },
                ],
                controllerAs: 'validationTabIndex',
                bindToController: true,
            };
        },
    ]);
    ng.module('validation').directive('validationInputText', [
        function () {
            return {
                restrict: 'A',
                bindToController: true,
                controllerAs: 'validationInputText',
                controller: [
                    '$attrs',
                    '$interpolate',
                    '$scope',
                    function ($attrs, $interpolate, $scope) {
                        var ctrl = this;
                        ctrl.$onInit = function () {
                            ctrl.ngModelCtrl.validationInputText = $interpolate($attrs.validationInputText)($scope);
                            ctrl.validationOpenTab = function () {
                                ctrl.uibTabsetCtrl.select(ctrl.validationTabIndexCtrl.validationTabIndex);
                            };
                            $attrs.$observe('validationInputText', (value) => {
                                ctrl.ngModelCtrl.validationInputText = $interpolate(value)($scope);
                            });
                        };
                    },
                ],
                require: {
                    ngModelCtrl: 'ngModel',
                    validationTabIndexCtrl: '?^validationTabIndex',
                    uibTabsetCtrl: '?^uibTabset',
                },
                //link: function (scope, element, attrs, ctrls) {
                //    ctrls[0].validationInputText = attrs.validationInputText;

                //    ctrls[0].validationOpenTab = function () {
                //        ctrls[2].select(ctrls[1].validationTabIndex)
                //    };

                //    if (ctrls[1] != null) {
                //        ctrls[0].validationTabIndex = ctrls[1].validationTabIndex;
                //        //ctrls[0].uibTabsetCtrl = ctrls[2];
                //    }
                //}
            };
        },
    ]);
    ng.module('validation').directive('validationErrorsText', [
        function () {
            return {
                restrict: 'A',
                bindToController: true,
                controllerAs: 'validationErrorsText',
                controller: [
                    '$attrs',
                    '$parse',
                    '$scope',
                    function ($attrs, $parse, $scope) {
                        var ctrl = this;
                        ctrl.$onInit = function () {
                            ctrl.ngModelCtrl.validationErrorsText = $parse($attrs.validationErrorsText)($scope);
                        };
                    },
                ],
                require: {
                    ngModelCtrl: 'ngModel',
                },
            };
        },
    ]);
    ng.module('validation').directive('validationOutput', [
        function () {
            return {
                templateUrl: validationOutputTemplate,
            };
        },
    ]);
    ng.module('validation').component('validationList', {
        templateUrl: validationListTemplate,
        bindings: {
            validationType: '<',
            validationErrors: '<',
        },
    });
    ng.module('validation').component('validationListItem', {
        templateUrl: validationListItemTemplate,
        bindings: {
            error: '<?',
        },
        controller: [
            'domService',
            function (domService) {
                var ctrl = this;
                ctrl.goToElement = function (text, error, uiTabset) {
                    var validationInputTextCtrl;

                    var el = document.querySelector(error.$name?.length ? '[name="' + error.$name + '"]' : '[validation-input-text="' + text + '"]');

                    var closeBtn = document.querySelector('.toast-close-button');
                    if (el != null) {
                        if (el.type == null || ['input', 'textarea', 'select'].indexOf(el.tagName.toLowerCase()) === -1) {
                            el = el.querySelector('input:not([type="button"]), textarea');
                        }
                        validationInputTextCtrl = angular.element(el).controller('validationInputText');
                        if (validationInputTextCtrl.uibTabsetCtrl != null && validationInputTextCtrl.validationTabIndexCtrl != null) {
                            validationInputTextCtrl.uibTabsetCtrl.active = validationInputTextCtrl.validationTabIndexCtrl.validationTabIndex;
                        }
                        setTimeout(function () {
                            el.ckEditorInstance ? el.ckEditorInstance.focus() : el.focus();
                            closeBtn.click();
                        }, 0);
                    }
                };
            },
        ],
    });
    ng.module('validation').directive('validationInputFloat', function () {
        return {
            require: {
                ngModelCtrl: 'ngModel',
            },
            bindToController: true,
            controller: function () {
                var ctrl = this;
                ctrl.$onInit = function () {
                    ctrl.ngModelCtrl.$validators.validInputFloat = function (modelValue, viewValue) {
                        return viewValue == null || viewValue.length === 0 || (viewValue.length > 0 && /^-?[\s\d,\.]*$/.test(viewValue));
                    };
                    ctrl.ngModelCtrl.$parsers.push(function (value) {
                        var result = parseFloat(value.replace(/\s*/g, '').replace(/,/g, '.'));
                        return isNaN(result) === false ? result : value;
                    });
                    ctrl.ngModelCtrl.$formatters.push(function (value) {
                        if (value == null) return value;
                        return value.toString().replace(/\s*/g, '').replace(/\./g, ',');
                    });
                };
            },
        };
    });
    ng.module('validation').directive('validationInputNotEmpty', function () {
        return {
            require: {
                ngModelCtrl: 'ngModel',
            },
            bindToController: true,
            controller: [
                '$attrs',
                '$parse',
                '$scope',
                function ($attrs, $parse, $scope) {
                    var ctrl = this;
                    ctrl.$onInit = function () {
                        var valide = $parse($attrs.validationInputNotEmpty);
                        ctrl.ngModelCtrl.$validators.validInputNotEmpty = function (modelValue, viewValue) {
                            return valide($scope, {
                                modelValue: modelValue,
                                viewValue: viewValue,
                            });
                        };
                    };
                },
            ],
        };
    });
    ng.module('validation').directive('validationInputMin', function () {
        return {
            require: {
                ngModelCtrl: 'ngModel',
            },
            bindToController: true,
            controller:
                /* @ngInject */
                function ($attrs, $parse, $scope) {
                    var ctrl = this;
                    ctrl.$onInit = function () {
                        var minValueFn = $parse($attrs.validationInputMin);
                        ctrl.ngModelCtrl.$validators.validInputMin = function (modelValue, viewValue) {
                            var minValueParsed = minValueFn($scope);
                            var valAsNumber = ng.isNumber(modelValue) === false ? convertToNumber(modelValue) : modelValue;
                            return viewValue == null || viewValue.length === 0 || (isNaN(valAsNumber) === false && valAsNumber >= minValueParsed);
                        };
                        ctrl.ngModelCtrl.$parsers.push(convertToNumber);
                    };
                },
        };
    });
    ng.module('validation').directive('validationInputMax', function () {
        return {
            require: {
                ngModelCtrl: 'ngModel',
            },
            bindToController: true,
            controller:
                /* @ngInject */
                function ($attrs, $parse, $scope) {
                    var ctrl = this;

                    ctrl.$onInit = function () {
                        var maxValueFn = $parse($attrs.validationInputMax);

                        ctrl.ngModelCtrl.$validators.validInputMax = function (modelValue, viewValue) {
                            var maxValueParsed = maxValueFn($scope);
                            var valAsNumber = ng.isNumber(modelValue) === false ? convertToNumber(modelValue) : modelValue;

                            return viewValue == null || viewValue.length === 0 || (isNaN(valAsNumber) === false && valAsNumber <= maxValueParsed);
                        };

                        ctrl.ngModelCtrl.$parsers.push(convertToNumber);
                    };
                },
        };
    });

    ng.module('validation').directive('validationInputNotCyrillic', function () {
        return {
            require: {
                ngModelCtrl: 'ngModel',
            },
            bindToController: true,
            controller: function () {
                var ctrl = this;
                ctrl.$onInit = function () {
                    ctrl.ngModelCtrl.$validators.validInputFloat = function (modelValue, viewValue) {
                        return viewValue == null || viewValue.length === 0 || (viewValue.length > 0 && !/[а-яА-ЯЁё]/.test(viewValue));
                    };
                };
            },
        };
    });

    const convertToNumber = function (value) {
        if (value != null) {
            var result = ng.isNumber(value) === false ? parseFloat(value.replace(/\s*/g, '').replace(/,/g, '.')) : value;
            return isNaN(result) === false ? result : value;
        }
        return value;
    };
})(window.angular);
