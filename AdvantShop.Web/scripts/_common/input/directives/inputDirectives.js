(function (ng) {
    'use strict';

    angular.module('input').directive('input', [
        '$parse',
        '$window',
        function ($parse, $window) {
            return {
                restrict: 'E',
                require: '?ngModel',
                priority: 200,
                link: function (scope, element, attrs, ctrl) {
                    var valueDirty = attrs.value,
                        el,
                        type,
                        value,
                        valueScope;

                    if (ctrl != null && (ctrl.$modelValue === undefined || Number.isNaN(ctrl.$modelValue))) {
                        el = element[0];
                        type = el.type;
                        valueScope = $parse(attrs.ngModel)(scope);

                        switch (type) {
                            case 'radio':
                                if (attrs.checked) {
                                    value = attrs.ngValue ? $parse(attrs.ngValue)(scope) : valueDirty;
                                }
                                break;
                            case 'checkbox':
                                if (attrs.checked) {
                                    value = attrs.ngTrueValue ? $parse(attrs.ngTrueValue)(scope) : true;
                                } else if (attrs.ngFalseValue) {
                                    value = $parse(attrs.ngFalseValue)(scope);
                                }
                                break;
                            case 'number':
                                if (valueDirty != null && valueDirty.length > 0) {
                                    var numberParsed = Number(valueDirty.replace(',', '.').replace(/\s*/, ''));
                                    value = isNaN(numberParsed) === false ? numberParsed : null;
                                }
                                if (valueScope != null && angular.isNumber(valueScope) === false) {
                                    valueScope = Number(valueScope.replace(',', '.').replace(/\s*/, ''));
                                }
                                break;
                            case 'datetime-local':
                            case 'date':
                                if (valueDirty != null && valueDirty.length > 0) {
                                    var dateObj = new Date(valueDirty);
                                    value = isNaN(dateObj) === false ? dateObj : null;
                                }
                                break;
                            default:
                                if (valueDirty != null && valueDirty.length > 0) {
                                    value = valueDirty;
                                }
                                break;
                        }

                        if (value != null) {
                            const valAssign = valueScope != null && Number.isNaN(valueScope) === false ? valueScope : value;
                            $parse(attrs.ngModel).assign(scope, valAssign);
                        }

                        if (type === 'text') {
                            var callbackPaste = function (event) {
                                var content;

                                if ($window.clipboardData && $window.clipboardData.getData) {
                                    // IE
                                    content = $window.clipboardData.getData('Text');
                                } else {
                                    // others
                                    content = event.clipboardData.getData('text/plain');
                                }

                                if (content != null) {
                                    ctrl.$setViewValue(content);
                                }
                            };

                            el.addEventListener('paste', callbackPaste);

                            scope.$on('$destroy', function () {
                                el.removeEventListener('paste', callbackPaste);
                            });
                        }
                    }
                },
            };
        },
    ]);

    angular.module('input').directive('textarea', [
        '$parse',
        function ($parse) {
            return {
                restrict: 'E',
                bindToController: true,
                require: {
                    ngModelCtrl: '?ngModel',
                },
                controller: [
                    '$attrs',
                    '$element',
                    '$parse',
                    '$scope',
                    '$window',
                    function ($attrs, $element, $parse, $scope, $window) {
                        var ctrl = this;

                        ctrl.$onInit = function () {
                            var val = $element.val();
                            var isNotEmpty = val.replaceAll(/\s/g, ``).length > 0;

                            if (ctrl.ngModelCtrl != null && isNotEmpty) {
                                $parse($attrs.ngModel).assign($scope, val);
                            }

                            var callbackPaste = function (event) {
                                var content;

                                if ($window.clipboardData && $window.clipboardData.getData) {
                                    // IE
                                    content = $window.clipboardData.getData('Text');
                                } else {
                                    // others
                                    content = event.clipboardData.getData('text/plain');
                                }

                                if (content != null && ctrl.ngModelCtrl != null) {
                                    ctrl.ngModelCtrl.$setViewValue(content);
                                }
                            };

                            $element[0].addEventListener('paste', callbackPaste);

                            $scope.$on('$destroy', function () {
                                $element[0].removeEventListener('paste', callbackPaste);
                            });
                        };
                    },
                ],
                //link: function (scope, element, attrs, ctrl) {
                //    if (element[0].value && ctrl != null) {
                //        //ctrl.$setViewValue(element[0].value);
                //        $parse(attrs.ngModel).assign(scope, element[0].value);
                //    }
                //}
            };
        },
    ]);
})(window.angular);
