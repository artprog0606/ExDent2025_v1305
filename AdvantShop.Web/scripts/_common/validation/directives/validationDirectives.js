(function (ng) {
    'use strict';
    angular.module('validation').directive('buttonValidation', [
        '$parse',
        'domService',
        'scrollToBlockService',
        function ($parse, domService, scrollToBlockService) {
            return {
                restrict: 'A',
                require: ['^form'],

                link: function (scope, element, attrs, ctrls) {
                    var FormCtrl = ctrls[0],
                        customValidFunc = $parse(attrs.buttonValidation),
                        startFunc = $parse(attrs.buttonValidationStart),
                        successFunc = $parse(attrs.buttonValidationSuccess),
                        formNames = $parse(attrs.buttonValidationForms);

                    function validate(event) {
                        scope.clickEvent = event;
                        scope.FormCtrl = FormCtrl;

                        startFunc(scope);

                        if (FormCtrl.$invalid === true || customValidFunc(scope) === false) {
                            FormCtrl.$setSubmitted();
                            FormCtrl.$setDirty();
                            event.preventDefault();
                            event.stopPropagation();

                            var form = findForm(event, formNames(scope));

                            if (form != null) {
                                var invalidElementFocus = form.querySelector('.ng-invalid:not(form):not(ng-form)');

                                if (invalidElementFocus != null) {
                                    invalidElementFocus.focus();
                                    scrollToBlockService.scrollToBlock(invalidElementFocus);
                                }
                            }
                        } else {
                            successFunc(scope);
                        }

                        scope.$apply();
                    }

                    function findForm(event, formNames) {
                        var currentFrom;

                        if (formNames != null) {
                            for (var i = 0, len = formNames.length; i < len; i++) {
                                if (document.forms[formNames[i]] != null && document.forms[formNames[i]].classList.contains('ng-invalid')) {
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

    angular.module('validation').directive('validationCustomFunction', function () {
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
                        var validate = $parse($attrs.validationCustomFunction);

                        ctrl.ngModelCtrl.$validators.validationCustomFunction = function (modelValue, viewValue) {
                            return validate($scope, {
                                modelValue: modelValue,
                                viewValue: viewValue,
                            });
                        };
                    };
                },
            ],
        };
    });
    angular.module('validation').directive('validationInputFloat', function () {
        return {
            require: {
                ngModelCtrl: 'ngModel',
            },
            bindToController: true,
            controller: function () {
                var ctrl = this;
                ctrl.$onInit = function () {
                    ctrl.ngModelCtrl.$formatters.push(function (value) {
                        if (value == null) return value;
                        return value.toString().replace(/\s*/g, '').replace(/\./g, ',');
                    });
                };
            },
        };
    });
})(window.angular);
