(function (ng) {
    'use strict';

    angular
        .module('select', [])
        .directive('select', [
            '$parse',
            '$timeout',
            function ($parse, $timeout) {
                return {
                    require: '?ngModel',
                    restrict: 'E',
                    link: function (scope, element, attrs, ctrl) {
                        var selectValue;

                        if (
                            ctrl != null &&
                            !attrs.ngOptions &&
                            attrs.disabledAutobind == null &&
                            (ctrl.$modelValue === undefined || isNaN(ctrl.$modelValue))
                        ) {
                            selectValue = element.val();

                            $parse(attrs.ngModel).assign(
                                scope,
                                attrs.convertToNumber != null
                                    ? parseFloat(selectValue, 10)
                                    : attrs.convertToBool != null
                                      ? selectValue === 'True'
                                      : selectValue,
                            );
                        }

                        if (attrs.onChange != null && attrs.onChange.length > 0) {
                            var onChangeCallback = $parse(attrs.onChange),
                                onChangeHandler = function (event) {
                                    onChangeCallback(scope, { event: event });
                                };

                            element.on('change', onChangeHandler);

                            element.on('$destroy', function () {
                                element.off('change', onChangeHandler);
                            });
                        }
                    },
                };
            },
        ])
        .directive('convertToNumber', function () {
            return {
                require: 'ngModel',
                link: function (scope, element, attrs, ngModel) {
                    ngModel.$parsers.push(function (val) {
                        return angular.isArray(val)
                            ? val.map(function (item) {
                                  return parseFloat(item, 10);
                              })
                            : parseFloat(val, 10);
                    });
                    ngModel.$formatters.push(function (val) {
                        return angular.isArray(val)
                            ? val.map(function (item) {
                                  return item != null ? '' + item : item;
                              })
                            : val != null
                              ? '' + val
                              : val;
                    });
                },
            };
        })
        .directive('convertToBool', function () {
            return {
                require: 'ngModel',
                link: function (scope, element, attrs, ngModel) {
                    ngModel.$parsers.push(function (val) {
                        return val != null ? val.toLowerCase() === 'true' : val;
                    });
                    ngModel.$formatters.push(function (val) {
                        return val != null ? (val === true ? 'True' : 'False') : val;
                    });
                },
            };
        });
})(window.angular);
