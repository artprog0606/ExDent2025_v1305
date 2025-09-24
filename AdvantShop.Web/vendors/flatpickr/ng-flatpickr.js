const tokenRegexCalendar = {
    D: "(\\w+)",
    F: "(\\w+)",
    G: "(\\d\\d|\\d)",
    H: "(\\d\\d|\\d)",
    J: "(\\d\\d|\\d)\\w+",
    K: "",
    M: "(\\w+)",
    S: "(\\d\\d|\\d)",
    U: "(.+)",
    W: "(\\d\\d|\\d)",
    Y: "(\\d{4})",
    Z: "(.+)",
    d: "(\\d\\d|\\d)",
    h: "(\\d\\d|\\d)",
    i: "(\\d\\d|\\d)",
    j: "(\\d\\d|\\d)",
    l: "(\\w+)",
    m: "(\\d\\d|\\d)",
    n: "(\\d\\d|\\d)",
    s: "(\\d\\d|\\d)",
    u: "(.+)",
    w: "(\\d\\d|\\d)",
    y: "(\\d{2})",
};

function checkNeedFormat(date, format) {

    // if (typeof date !== 'string') {
    //     return false;
    // }

    var str = '';

    var symbolList = format.split('');

    symbolList.forEach(symbol => str += tokenRegexCalendar[symbol] != null ? tokenRegexCalendar[symbol] : symbol);

    var regexp = new RegExp('^' + str + '$');

    return !regexp.test(date);
}

var maskIsCompleteOptional = function (maskControl) {
    return maskControl == null || maskControl.maskOriginal.masked.isComplete;
}

var ngModelFormatFn = function (flatpickr, ngFlatpickrFilter, maskControl) {
    return function (value) {

        var result;

        if (value == null || value === '' || (maskControl != null && maskControl.maskOriginal.masked.isComplete === false)) {
            result = value;
        }
        else {
            if (angular.isString(value) === true && flatpickr.config.startDateFormat == null) {
                console.warn('Option "startDateFormat" required');
            }

            result = ngFlatpickrFilter(value, flatpickr.config.dateFormat, flatpickr.config.startDateFormat);
        }

        return result;
    };
};

var ngModelParseFn = function (flatpickr, ngFlatpickrFilter, maskControl) {
    return function (value) {
        return value == null || value.length === 0 || (maskControl != null && maskControl.maskOriginal.masked.isComplete === false) ? value : ngFlatpickrFilter(value, flatpickr.config.startDateFormat, flatpickr.config.dateFormat);
    };
};


var ngFlatpickr = angular.module('angular-flatpickr', []);


ngFlatpickr.constant('ngFlatpickrDefaultOptions', {
    time_24hr: true,
    allowInput: true
});

ngFlatpickr.directive('ngFlatpickr', ['$q', '$filter', '$parse', '$timeout', 'ngFlatpickrDefaultOptions', function ($q, $filter, $parse, $timeout, ngFlatpickrDefaultOptions) {
    return {
        require: {
            ngModel: '?ngModel',
            maskControl: '?maskControl'
        },
        restrict: 'A',
        scope: {
            fpOpts: '<?',
            fpOnSetup: '&',
            fpOnChange: '&',
            fpOnOpen: '&'
        },
        bindToController: true,
        controllerAs: 'ngFlatpickr',
        controller: ['$element', '$scope', function ($element, $scope) {
            var ctrl = this;

            ctrl.addNgFlatpickrInput = function (ngFlatpickrInput) {
                ctrl.ngFlatpickrInput = ngFlatpickrInput;
            };

            ctrl.processNgModel = function (flatpickr, ngModel, maskControl) {
                var ngModelParseFnCurrent;
                var ngModelFormatFnCurrent;

                ngModelFormatFnCurrent = ngModelFormatFn(flatpickr, $filter('ngFlatpickr'), maskControl);
                ngModelParseFnCurrent = ngModelParseFn(flatpickr, $filter('ngFlatpickr'), maskControl);

                ngModel.$formatters.push(ngModelFormatFnCurrent);
                ngModel.$parsers.push(ngModelParseFnCurrent);

                ngModel.$render = function () {
                    if (maskIsCompleteOptional(maskControl) && ctrl.ngFlatpickrInput != null) {
                        const valueSource =  ['value', 'ng-value', 'data-ng-value'].every(attrName => ctrl.ngFlatpickrInput.$element.attr(attrName) == null) ? ngModel.$modelValue : ngModel.$viewValue; 
                        flatpickr.setDate(Array.isArray(valueSource) ? valueSource : $filter('ngFlatpickr')(valueSource, flatpickr.config.dateFormat, flatpickr.config.startDateFormat), false);
                    }
                };

                if (ngModel.$viewValue != null && angular.isString(ngModel.$viewValue) === true && ngModel.$viewValue.length > 0 && maskIsCompleteOptional(maskControl)) {
                    flatpickr.setDate($filter('ngFlatpickr')(ngModel.$viewValue, flatpickr.config.dateFormat, flatpickr.config.startDateFormat), false);
                }

                return function () {
                    var indexFormatFn;
                    var indexParseFn;

                    if (ngModelFormatFnCurrent) {
                        indexFormatFn = ngModel.$formatters.indexOf(ngModelFormatFnCurrent);

                        if (indexFormatFn !== -1) {
                            ngModel.$formatters.splice(indexFormatFn, 1);
                        }
                    }

                    if (ngModelParseFnCurrent) {
                        indexParseFn = ngModel.$parsers.indexOf(ngModelParseFnCurrent);

                        if (indexParseFn !== -1) {
                            ngModel.$parsers.splice(indexParseFn, 1);
                        }
                    }
                }
            }

            ctrl.bindElementEvents = function (inputElement, maskControl) {
                if (inputElement != null) {
                    inputElement.on('keyup', function () {
                        ctrl.needUpdate = true;
                    });

                    inputElement.on('blur', function (event) {
                        var el = this;
                        if (ctrl.needUpdate === true && maskIsCompleteOptional(maskControl)) {
                            $timeout(function () {
                                ctrl.flatpickr.setDate(el.value, true);
                            }, 10);
                        } else {
                            //�������� ����� ������� ������ ������ � flatpickr, ��� ��� �� ������� ��-�� ������������ �����
                            event.stopImmediatePropagation();
                        }
                    });
                    return function () {
                        inputElement.off();
                    }
                } else {
                    return function () { };
                }
            }

            function removeMaskPlaceholder(input, ngModel, maskControl, options) {
                var startVal;

                if ((isNaN(ngModel.$modelValue) || ngModel.$modelValue == null) && input != null && maskControl != null) {
                   const valueScope = $parse(input.attr('ng-model') || input.attr('data-ng-model'))($scope.$parent);
                    let valueScopeParsed;
                    if (valueScope != null) {
                        valueScopeParsed = valueScope.replace(/[\.:_]*/g, '');
                    }

                    startVal = valueScopeParsed != null && valueScopeParsed.length > 0 ? $filter('ngFlatpickr')(valueScope, options.dateFormat, options.startDateFormat) : input.val();
                    input.val('');

                    return function () {
                        setTimeout(() => {
                            input.val(startVal);
                        }, 0);
                    }
                } else {
                    return function () { }
                }
            }

            ctrl.$postLink = function () {
                var input = ctrl.ngFlatpickrInput != null ? ctrl.ngFlatpickrInput.$element : $element[0].tagName.toLowerCase() === 'input' ? $element : null;
                var maskControl = ctrl.ngFlatpickrInput != null && ctrl.ngFlatpickrInput.maskControl != null ? ctrl.ngFlatpickrInput.maskControl : ctrl.maskControl;
                var ngModel = ctrl.ngFlatpickrInput != null ? ctrl.ngFlatpickrInput.ngModel : ctrl.ngModel;
                var options = angular.extend({}, ngFlatpickrDefaultOptions, ctrl.fpOpts || {});

                if (typeof options.appendTo === 'string') {
                    options.appendTo = document.querySelector(options.appendTo);

                }

                options.onChange = function (selectedDates, dateStr, instance) {
                    ctrl.needUpdate = false;

                    if (ctrl.fpOnChange != null) {
                        $timeout(function () {
                            ctrl.fpOnChange({ selectedDates: selectedDates, dateStr: dateStr, instance: instance });
                        });
                    }
                };

                options.onValueUpdate = function (selectedDates, dateStr, instance) {
                    //����� ����� ������ � �������� �� ���� ������ ���, �� ����������� ����
                    if (ngModel != null && ngModel.modelValue == null) {

                        if (maskControl != null) {
                            maskControl.maskOriginal.value = dateStr;
                        }
                        ngModel.$setViewValue(dateStr);
                    }

                    //if (maskControl != null) {
                    //    maskControl.maskOriginal.updateValue();
                    //}

                };

                options.onOpen = function (selectedDates, dateStr, instance) {
                    ctrl.needUpdate = false;

                    if (ctrl.fpOnOpen != null) {
                        $timeout(function () {
                            ctrl.fpOnOpen({ selectedDates: selectedDates, dateStr: dateStr, instance: instance });
                        });
                    }
                };

                //������� ����������� �� ����� �� ����� ����� ��������� ����� ��� ��� �������� � ������
                var revertMaskPlaceholder = removeMaskPlaceholder(input, ngModel, maskControl, options);

                ctrl.flatpickr = new flatpickr($element[0], options);

                var destroyNgModel = ctrl.processNgModel(ctrl.flatpickr, ngModel, maskControl);
                var destroyElementEvents = ctrl.bindElementEvents(input, maskControl);

                //���������� �����������
                revertMaskPlaceholder();

                // destroy the flatpickr instance when the dom element is removed
                $element.on('$destroy', function () {
                    destroyNgModel();
                    destroyElementEvents();
                    delete ctrl.ngFlatpickrInput;
                    ctrl.flatpickr.destroy();
                    $scope.$destroy();
                });

                if (ctrl.fpOnSetup) {
                    ctrl.fpOnSetup({
                        fpItem: ctrl.flatpickr
                    });
                }
            };
            
            ctrl.parse = function (str){
                return  ctrl.flatpickr.parseDate(str, ctrl.flatpickr.config.dateFormat);
            }

            ctrl.format = function (date){
                return ctrl.flatpickr.formatDate(date, ctrl.flatpickr.config.dateFormat);
                //return $filter('ngFlatpickr')(date, ctrl.flatpickr.config.dateFormat, ctrl.flatpickr.config.startDateFormat);
            }
        }],
    };
}]);

ngFlatpickr.directive('ngFlatpickrInput', ['$filter', '$timeout', function ($filter, $timeout) {
    return {
        require: {
            ngModel: 'ngModel',
            ngFlatpickr: '^ngFlatpickr',
            maskControl: '?maskControl'
        },
        restrict: 'A',
        bindToController: true,
        controllerAs: 'ngFlatpickrInput',
        controller: ['$element', function ($element) {
            var ctrl = this;

            ctrl.$onInit = function () {
                ctrl.$element = $element;
                ctrl.ngFlatpickr.addNgFlatpickrInput(ctrl);
            };

            ctrl.$postLink = function () {
                $element.attr('data-input', '');
            }
        }]
    };
}]);

ngFlatpickr.filter('ngFlatpickr', function () {
    return function ngFlatpickrFilter (value, format, formatParse) {
        const valueIsString = angular.isString(value);
        let valueParsed;

        if (value == null || (valueIsString && value.length === 0) || checkNeedFormat(value, format) === false) {
            return value;
        }

        if (valueIsString) {
            if (formatParse == null || formatParse.length === 0) {
                console.warn('Missing required filter parameter "formatParse" for parse date string');
            } else {
                valueParsed = flatpickr.parseDate(value, formatParse);
            }
        } else if (angular.isDate(value) === true) {
            valueParsed = value;
        }
        else if(Array.isArray(value)){
            
            return value.map(x => ngFlatpickrFilter(x,format, formatParse)).join(` - `);
        }
        else {
            console.warn('Unsupport variable type for filter ngFlatpickr');
            return value;
        }

        return flatpickr.formatDate(valueParsed || value, format);
    };
});

export default 'ngFlatpickr';
