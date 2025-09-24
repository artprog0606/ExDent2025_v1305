(function (ng) {
    'use strict';

    var SpinboxCtrl = /* @ngInject */ function ($element, $timeout, spinboxKeyCodeAllow, spinboxTooltipTextType, $translate, $attrs, $scope) {
        var ctrl = this,
            timeoutTooltip = null,
            TOOLTIP_TIMER = 3000,
            MAX_DEFAULT = Infinity,
            MIN_DEFAULT = 0,
            callbackTimerId;

        //ctrl.min = ctrl.min() || 0;
        //ctrl.max = ctrl.max() || Number.POSITIVE_INFINITY;
        //ctrl.step = ctrl.step() || 1;

        var callbackCall = function (value) {
            if (callbackTimerId != null) {
                $timeout.cancel(callbackTimerId);
            }

            callbackTimerId = $timeout(
                function () {
                    // && ctrl.form.$valid
                    if (ctrl.updateFn != null) {
                        ctrl.updateFn({ value: value, proxy: ctrl.proxy });
                    }
                },
                ctrl.debounce ? 700 : 0,
            );
        };

        ctrl.numberToFloat = (num) => {
            return parseFloat(num?.toString().replace(/\s*/g, '').replace(/,/g, '.'));
        };
        ctrl.formatterNumber = (num) => {
            if (ctrl.needComma === false) return num;
            return num?.toString().replace(/\s*/g, '').replace(/\./g, ',');
        };
        ctrl.processValue = (num, fn, needFormatter = false) => {
            const result = fn(ctrl.numberToFloat(num));
            return needFormatter ? ctrl.formatterNumber(result) : result;
        };
        ctrl.$onInit = function () {
            ctrl.needComma = ctrl.needComma != null ? ctrl.needComma : false;
            ctrl.isRequired = ctrl.isRequired != null ? ctrl.isRequired : true;
            ctrl.debounce = ctrl.debounce != null ? ctrl.debounce : true;
            ctrl.onlyValidation = ctrl.onlyValidation != null ? ctrl.onlyValidation : false;
            ctrl.tooltipText = null;
            ctrl.isVisibleArrows = ctrl.isVisibleArrows != null ? ctrl.isVisibleArrows : true;
            ctrl.inputName = ctrl.inputName != null ? ctrl.inputName : undefined;

            ctrl.step = ctrl.numberToFloat(ctrl.step);
            ctrl.max = ctrl.max != null ? ctrl.processValue(ctrl.max, ctrl.getNearValue) : MAX_DEFAULT;
            ctrl.min = ctrl.min != null ? ctrl.processValue(ctrl.min, ctrl.getNearValue) : MIN_DEFAULT;
            if (ctrl.value == null) {
                return;
            }
            ctrl.value = ctrl.processValue(
                ctrl.value,
                (num) => {
                    return ctrl.getNearestMoreMultiple(num, ctrl.step);
                },
                true,
            );
            ctrl.checkButtons(ctrl.numberToFloat(ctrl.value));

            $scope.$watch('spinbox.max', (newVal) => {
                if (newVal === undefined) return;
                ctrl.max = newVal;
                ctrl.checkButtons(ctrl.value);
            });

            $scope.$watch('spinbox.min', (newVal) => {
                if (newVal === undefined) return;
                ctrl.min = newVal;
                ctrl.checkButtons(ctrl.value);
            });
        };

        ctrl.less = function () {
            var newValue = ctrl.numberToFloat(ctrl.value) - ctrl.step;

            if (ctrl.onlyValidation === false) {
                newValue = ctrl.checkRange(newValue) === true ? newValue : ctrl.min != null ? ctrl.min : newValue;
            }

            const finishNewValue = ctrl.numberRound(newValue);

            if ($attrs.validationBeforeUpdateFn && !ctrl.validationBeforeUpdateFn({ value: finishNewValue, proxy: ctrl.proxy })) {
                return;
            }

            ctrl.form.$setDirty();
            ctrl.value = ctrl.formatterNumber(finishNewValue);

            ctrl.checkButtons(ctrl.value);

            ctrl.beforeUpdate();

            callbackCall(ctrl.numberToFloat(ctrl.value));
        };

        ctrl.more = function () {
            var newValue = ctrl.numberToFloat(ctrl.value) + ctrl.step;

            if (ctrl.onlyValidation === false) {
                newValue = ctrl.checkRange(newValue) === true ? newValue : ctrl.max != null ? ctrl.max : newValue;
            }

            const finishNewValue = ctrl.numberRound(newValue);

            if ($attrs.validationBeforeUpdateFn && !ctrl.validationBeforeUpdateFn({ value: finishNewValue, proxy: ctrl.proxy })) {
                return;
            }

            ctrl.form.$setDirty();
            ctrl.value = ctrl.formatterNumber(finishNewValue);

            ctrl.checkButtons(ctrl.value);

            ctrl.beforeUpdate();

            callbackCall(ctrl.numberToFloat(ctrl.value));
        };

        ctrl.checkRange = function (newValue) {
            return newValue <= ctrl.numberToFloat(ctrl.max) && newValue >= ctrl.numberToFloat(ctrl.min);
        };

        ctrl.checkRegex = function (char) {
            return /\d/g.test(char);
        };

        ctrl.keydown = function (event) {
            var symbol;

            if (event.altKey || event.ctrlKey || event.shiftKey) {
                event.preventDefault();
                return;
            }

            var code = ctrl.prepareNumpad(event.keyCode);

            if (!ctrl.isExistKeyCodeAllow(code)) {
                symbol = Number(String.fromCharCode(code));

                if (Number.isNaN(symbol) === true) {
                    event.preventDefault();
                }
            } else {
                switch (code) {
                    case 40:
                        // down arrow
                        ctrl.less();
                        event.preventDefault();
                        break;
                    case 38:
                        // up arrow
                        ctrl.more();
                        event.preventDefault();
                        break;
                }
            }
        };

        ctrl.keyup = function (event) {
            var code = ctrl.prepareNumpad(event.keyCode),
                symbol = Number(String.fromCharCode(code));
            // ctrl.value = parseFloat($element[0].querySelector('.spinbox-input').value);
            //update if number

            // callbackCall(ctrl.normalizeNumberInput(ctrl.value));

            // if (Number.isNaN(symbol) === false) {
            //
            //
            //     // if(ctrl.form.$valid){
            //     //     callbackCall(ctrl.normalizeNumberInput(ctrl.value));

            //     // }
            // } else if ([8, 49, 110, 188].indexOf(event.keyCode) !== -1 && ctrl.value > 0) {
            //     //'backspace': 8,
            //     //'delete': 46,
            //     //'decimalPoint': 110,
            //     //'comma': 188,
            //     callbackCall(ctrl.normalizeNumberInput(ctrl.value));

            // }
        };

        ctrl.prepareNumpad = function (keycode) {
            return keycode > 95 && keycode < 106 ? keycode - 48 : keycode;
        };

        ctrl.isExistKeyCodeAllow = function (keycode) {
            var result = false;

            for (var key in spinboxKeyCodeAllow) {
                if (spinboxKeyCodeAllow[key] == keycode) {
                    result = true;
                    break;
                }
            }

            return result;
        };

        ctrl.checkButtons = function (newValue) {
            if (ctrl.onlyValidation) return;

            ctrl.lessBtnDisabled = newValue <= ctrl.min;
            ctrl.moreBtnDisabled = newValue >= ctrl.max;
        };

        ctrl.checkMinMax = function (value) {
            let result = value;
            if (result > ctrl.max) {
                result = ctrl.formatterNumber(ctrl.max);
                ctrl.tooltipText = ctrl.getTooltipText(spinboxTooltipTextType.max);
            } else if (Number.isNaN(result) || result < ctrl.numberToFloat(ctrl.min)) {
                result = ctrl.formatterNumber(ctrl.min);
                ctrl.tooltipText = ctrl.getTooltipText(spinboxTooltipTextType.min);
            } else {
                result = ctrl.formatterNumber(result);
            }
            return result;
        };
        ctrl.correctByStep = function (value) {
            var whole,
                newValue = value;

            const normalizedValue = ctrl.numberToFloat(newValue);
            const step = ctrl.numberToFloat(ctrl.step);

            if (step === 0) return newValue;

            if (!ctrl.isShareWhole(normalizedValue, step) || normalizedValue === 0) {
                whole = normalizedValue - ctrl.getRemainder(normalizedValue, step);

                newValue = whole + step;

                newValue = ctrl.checkRange(newValue) === true ? newValue : ctrl.max != null ? ctrl.max : newValue;

                newValue = ctrl.numberRound(newValue);
                ctrl.checkButtons(newValue);
                ctrl.tooltipText = ctrl.getTooltipText(spinboxTooltipTextType.multiplicity);
            }
            return newValue;
        };
        ctrl.valueFoldStep = function () {
            let value = ctrl.numberToFloat($element[0].querySelector('.spinbox-input').value);

            if (ctrl.isRequired === false && Number.isNaN(value)) return;

            if (
                $attrs.validationBeforeUpdateFn &&
                !ctrl.validationBeforeUpdateFn({ value: value || ctrl.numberToFloat(ctrl.min), proxy: ctrl.proxy })
            ) {
                return;
            }

            if ((value === 0 || Number.isNaN(value)) && ctrl.min === MIN_DEFAULT) {
                ctrl.value = ctrl.formatterNumber(MIN_DEFAULT);
                return;
            }

            value = ctrl.onlyValidation ? value : ctrl.checkMinMax(value);

            ctrl.value = ctrl.formatterNumber(ctrl.onlyValidation ? value : ctrl.correctByStep(value));

            if (ctrl.tooltipText != null) {
                if (timeoutTooltip) {
                    ctrl.hideTooltip();
                    $timeout.cancel(timeoutTooltip);
                }
                ctrl.showTooltip();
                timeoutTooltip = $timeout(() => {
                    ctrl.tooltipText = null;
                }, TOOLTIP_TIMER);
            }
            ctrl.beforeUpdate();

            callbackCall(ctrl.numberToFloat(ctrl.value));
        };

        ctrl.getLengthAfterSemicolon = (value) => {
            return (value.toString().split('.')[1] ?? '').length;
        };
        ctrl.numberRound = function (newValue) {
            //http://0.30000000000000004.com/
            //http://stackoverflow.com/questions/588004/is-floating-point-math-broken/588014#588014
            return parseFloat(newValue.toPrecision(12));
        };
        ctrl.getRemainder = (num1, num2) => {
            return ctrl.numberRound(num1 % num2);
        };
        ctrl.isShareWhole = (num1, num2) => {
            const maxLengthAfterSemicolon = Math.max(ctrl.getLengthAfterSemicolon(num1), ctrl.getLengthAfterSemicolon(num2));
            const multiple = Math.pow(10, maxLengthAfterSemicolon || 1);

            const _num1 = multiple * num1;
            const _num2 = multiple * num2;

            const remainder = ctrl.getRemainder(_num1, _num2);

            return remainder === 0;
        };
        ctrl.getNearestLessMultiple = function (number, multiple) {
            return multiple === 0 ? number : ctrl.numberRound(Math.floor(number / multiple) * multiple);
        };
        ctrl.getNearestMoreMultiple = function (number, multiple) {
            return multiple === 0 ? number : ctrl.numberRound(Math.ceil(number / multiple) * multiple);
        };
        ctrl.getNearValue = function (value) {
            return ctrl.processValue(value, (num) => {
                const normalizeMax = ctrl.numberToFloat(ctrl.max ?? MAX_DEFAULT);
                const normalizeMin = ctrl.numberToFloat(ctrl.min ?? MIN_DEFAULT);

                if (num >= normalizeMax) {
                    return ctrl.isShareWhole(normalizeMax, ctrl.step) ? normalizeMax : ctrl.getNearestLessMultiple(normalizeMax, ctrl.step);
                } else if (normalizeMin >= num) {
                    return ctrl.isShareWhole(normalizeMin, ctrl.step) ? normalizeMin : ctrl.getNearestMoreMultiple(normalizeMin, ctrl.step);
                }

                return num;
            });
        };

        ctrl.showTooltip = function () {
            ctrl.isOpenTooltip = true;
        };

        ctrl.hideTooltip = function () {
            ctrl.isOpenTooltip = false;
        };

        ctrl.getTooltipText = function (type) {
            if (type === spinboxTooltipTextType.min) {
                return `${$translate.instant('Js.Spinbox.MinTextNote')} ${ctrl.min}`;
            }
            if (type === spinboxTooltipTextType.max) {
                return `${$translate.instant('Js.Spinbox.MaxTextNote')} ${ctrl.max}`;
            }
            if (type === spinboxTooltipTextType.multiplicity) {
                return `${$translate.instant('Js.Spinbox.MultiplicityTextNote')} ${ctrl.step}`;
            }
        };
        ctrl.setValidationText = function (text) {
            ctrl.validationText = text;
        };
    };

    angular.module('spinbox').controller('SpinboxCtrl', SpinboxCtrl);
})(window.angular);
