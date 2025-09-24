import IMask from 'imask';

function getMaskValue(mask, maskControlPreset) {
    return mask[(maskControlPreset != null && maskControlPreset.modelValueProp) || 'unmaskedValue'];
}

/*@ngInject*/
function maskDirective($filter, $parse, $timeout, $q, maskControlService) {
    return {
        restrict: 'A',
        require: {
            ngModel: 'ngModel',
            ngFlatpickr: '?^ngFlatpickr',
        },
        bindToController: true,
        controllerAs: 'mask',
        priority: 100,
        controller: [
            '$scope',
            '$element',
            '$attrs',
            function (scope, element, attrs) {
                const ctrl = this;

                ctrl.$onInit = function () {
                    const validationDisabled = attrs.validationDisabled != null ? $parse(attrs.validationDisabled)(scope) : false;
                    const presetList = {
                        date: {
                            mask: Date,
                            lazy: false,
                            parse (str) {
                                let year, month, day;

                                if (str.includes('.')) {
                                    const _str = str.split('.');
                                    (day = _str[0]), (month = _str[1]), (year = _str[2]);
                                } else if (str.includes('-')) {
                                    const _str = str.split('-');
                                    (day = _str[2]), (month = _str[1]), (year = _str[0]);
                                }

                                return new Date(year, month - 1, day);
                            },
                        },
                        datetime: {
                            mask: Date,
                            lazy: false,
                            pattern: 'd.m.Y H:i',
                            modelValueProp: 'value',
                            blocks: {
                                d: {
                                    mask: IMask.MaskedRange,
                                    from: 1,
                                    to: 31,
                                    maxLength: 2,
                                },
                                m: {
                                    mask: IMask.MaskedRange,
                                    from: 1,
                                    to: 12,
                                    maxLength: 2,
                                },
                                Y: {
                                    mask: IMask.MaskedRange,
                                    from: 1900,
                                    to: 2099,
                                },
                                H: {
                                    mask: IMask.MaskedRange,
                                    from: 0,
                                    to: 23,
                                },
                                i: {
                                    mask: IMask.MaskedRange,
                                    from: 0,
                                    to: 59,
                                },
                            },
                            parse (str) {
                                const isServerView = str.includes(`-`);
                                const delimiterCommon = str.includes(`T`) ? `T` : ` `;
                                const delimiterDate = str.includes(`-`) ? `-` : `.`;

                                const [date, time] = str.split(delimiterCommon);
                                const [day, month, year] = isServerView ? date.split(delimiterDate).reverse() : date.split(delimiterDate);
                                const [hours, minutes] = time.split(`:`);

                                return new Date(parseFloat(year), parseFloat(month - 1), parseFloat(day), parseFloat(hours), parseFloat(minutes));
                            },
                            format (date) {
                                return $filter(`date`)(date, `dd.MM.yyyy HH:mm`);
                            },
                        },
                        time: {
                            modelValueProp: 'value',
                            mask: 'HH:mm',
                            blocks: {
                                HH: {
                                    mask: IMask.MaskedRange,
                                    from: 0,
                                    to: 23,
                                },
                                mm: {
                                    mask: IMask.MaskedRange,
                                    from: 0,
                                    to: 59,
                                },
                            },
                        },
                        phone: {
                            modelValueProp: 'value',
                            mask: [
                                {
                                    mask: '+0(000)000-00-00', //Россия
                                    startsWith: '7',
                                    lazy: false,
                                },
                                {
                                    mask: '+000(00)000-00-00', //Украина
                                    startsWith: '380',
                                    lazy: false,
                                },
                                {
                                    mask: '+000(00)000-00-00', //Беларусь
                                    startsWith: '375',
                                    lazy: false,
                                },
                                {
                                    mask: '+000(00)000-00-00', //Узбекистан
                                    startsWith: '998',
                                    lazy: false,
                                },
                                {
                                    mask: '+000(000)00-00-00', //Грузия
                                    startsWith: '995',
                                    lazy: false,
                                },
                                {
                                    mask: '+000(00)00-00-00', //Армения
                                    startsWith: '374',
                                    lazy: false,
                                },
                                {
                                    mask: '+000(000)000-00', //Молдова
                                    startsWith: '373',
                                    lazy: false,
                                },
                                {
                                    mask: '+000(000)00-00-00', //Киргизия
                                    startsWith: '996',
                                    lazy: false,
                                },
                                {
                                    mask: '+000(00)000-00-00', //Сербия
                                    startsWith: '381',
                                    lazy: false,
                                },
                                {
                                    mask: '+000(00)000-00-00', //Азербайджан
                                    startsWith: '994',
                                    lazy: false,
                                },
                                {
                                    mask: '+000(00)00-00-00', //Туркменистан
                                    startsWith: '993',
                                    lazy: false,
                                },
                                {
                                    mask: '+000(000)00-00-00', //Таджикистан
                                    startsWith: '992',
                                    lazy: false,
                                },
                                {
                                    mask: '+000(00)000-00-00', //Израиль
                                    startsWith: '972',
                                    lazy: false,
                                },
                                {
                                    mask: '+0(000)000-00-00', //США
                                    startsWith: '1',
                                    lazy: false,
                                },
                                {
                                    mask: '+00(000)000-000-0[0]', //Германия
                                    startsWith: '49',
                                    lazy: false,
                                },
                                {
                                    mask: '+000(00)000-00-00', //Финляндия
                                    startsWith: '358',
                                    lazy: false,
                                },
                                {
                                    mask: '+00(000)00-00-00', //Франция
                                    startsWith: '33',
                                    lazy: false,
                                },
                                {
                                    mask: '({9}00)000-00-00[0]', //Россия
                                    rusNoCode: true,
                                    lazy: true,
                                },
                                {
                                    mask: '+000(00)000-00-0[0]', //Неизвестно
                                    lazy: true,
                                    unknown: true,
                                },
                            ],
                            parse (str) {
                                return str != null && str.length > 0 ? str.replace(/[\s\+]/g, '') : str;
                            },
                            dispatch (appended, dynamicMasked) {
                                const number = (dynamicMasked.value + appended).replace(/\D/g, '');
                                if (number.length === 0) {
                                    return dynamicMasked.compiledMasks[0];
                                }
                                const result = dynamicMasked.compiledMasks.find((m) => {
                                    if (number.indexOf('9') === 0 && number.length <= 2) {
                                        return m.rusNoCode === true;
                                    }
                                    return number.indexOf(m.startsWith) === 0;
                                });

                                return result || dynamicMasked.compiledMasks.find((item) => item.unknown) || dynamicMasked.compiledMasks[0];
                            },
                        },
                        number: {
                            mask: /^\d+$/,
                        },
                    };

                    let isComplete = false;

                    const modelValueSetter = function (scope, newValue) {
                        return $q.when($parse(attrs.ngModel).assign(scope, newValue));
                    };

                    const config = maskControlService.getMaskControlConfig();
                    ctrl.configGlobal = config;
                    if ((attrs.maskControlPreset === 'phone' && config.enablePhoneMask === false) || $parse(attrs.maskControl)(scope) === false) {
                        return;
                    }

                    const ngModelValue = $parse(attrs.ngModel)(scope);
                    const startValue =
                        ngModelValue != null && (typeof ngModelValue !== 'string' || ngModelValue.length > 0) ? ngModelValue : element.val();
                    if (
                        startValue.length > 0 &&
                        startValue !== ctrl.ngModel.$modelValue &&
                        (ctrl.ngModel.$modelValue == null || isNaN(ctrl.ngModel.$modelValue))
                    ) {
                        ctrl.ngModel.$setViewValue(startValue);
                        ctrl.ngModel.$setPristine();
                    }

                    const preset = attrs.maskControlPreset != null ? presetList[attrs.maskControlPreset] : null;
                    const onAccept = attrs.onAccept ? $parse(attrs.onAccept) : null;
                    const mask = IMask(element[0], { ...preset, ...$parse(attrs.maskControlOptions)(scope)});

                    ctrl.maskOriginal = mask;

                    if (attrs.maskControlPreset === 'phone') {
                        updatePlaceholder();

                        element.on(`keydown`, (event) => {
                            if (event.key != null && event.key.length === 1 && /\D+/.test(event.key) === true && !event.ctrlKey) {
                                event.preventDefault();
                            }
                        });
                    }

                    mask.on('accept', (event) => {
                        if (attrs.maskControlPreset === 'phone') {
                            //если ввели по старой памяти 8 заместо +7 для России
                            if (mask.unmaskedValue.startsWith('89')) {
                                mask.value = `+7${  mask.unmaskedValue.substring(1)}`;
                                setTimeout(() => {
                                    mask.updateCursor(4);
                                }, 100);
                            } else if (mask.unmaskedValue[0] === '9' && mask.unmaskedValue.length > 2) {
                                // есть маска с таким кодом страны
                                const codeMask = mask.masked.compiledMasks.find((m) => mask.unmaskedValue.indexOf(m.startsWith) === 0);
                                // нет маски - номер российский
                                if (codeMask == null) {
                                    mask.value = `+7${  mask.unmaskedValue}`;
                                    setTimeout(() => {
                                        mask.updateCursor(6);
                                    });
                                }
                            }
                        }

                        isComplete = false;
                        modelValueSetter(scope, ctrl.ngModel.$modelValue);

                        if (onAccept != null) {
                            onAccept(scope, { mask });
                        }
                    });

                    mask.on('complete', () => {
                        isComplete = true;
                        const maskValue = getMaskValue(mask, presetList[attrs.maskControlPreset]);
                        modelValueSetter(scope, maskValue)
                            .then(() => ctrl.ngModel.$setViewValue(maskValue))
                            .then(() => ctrl.ngModel.$validate());
                    });

                    ctrl.ngModel.$render = function () {
                        //если ввели по старой памяти 8 заместо +7 для России
                        if (
                            ctrl.ngModel.$modelValue != null &&
                            ctrl.ngModel.$modelValue.length === 11 &&
                            ctrl.ngModel.$modelValue.charAt(0) === '8'
                        ) {
                            ctrl.ngModel.$modelValue =
                                `7${  ctrl.ngModel.$modelValue.substring(ctrl.ngModel.$modelValue.length - (ctrl.ngModel.$modelValue.length - 1))}`;
                        }
                        mask.value = ctrl.ngModel.$modelValue || '';
                    };

                    ctrl.ngModel.$parsers.push((value) => getMaskValue(mask, presetList[attrs.maskControlPreset]));

                    ctrl.ngModel.$formatters.push((value) => value != null ||
                            (mask.masked.currentMask != null ? mask.masked.currentMask.lazy === false : mask.masked.lazy === false)
                            ? mask.value
                            : value);

                    if (validationDisabled !== true) {
                        ctrl.ngModel.$validators.mask = function (modelValue, viewValue) {
                            return (
                                mask.masked.isComplete ||
                                isComplete ||
                                (element[0].getAttribute('required') == null &&
                                    $parse(attrs.ngRequired)(scope) != true &&
                                    mask.masked.rawInputValue.length === 0)
                            );
                        };
                    }

                    const deregObserver = attrs.$observe(attrs.ngModel, updateMaskOnWatch);
                    const deregWatch = scope.$watch(attrs.ngModel, updateMaskOnWatch);

                    function updateMaskOnWatch(newVal, oldVal) {
                        if (newVal != null && (mask.unmaskedValue === '' || mask.unmaskedValue !== newVal)) {
                            mask.value = mask.masked.format(typeof newVal === 'string' ? mask.masked.parse(newVal) : newVal);
                            mask.updateValue();
                        }
                    }

                    function updatePlaceholder() {
                        const placeholderOldValue = element[0].placeholder;
                        if (placeholderOldValue == null || placeholderOldValue.length === 0) {
                            const {currentMask} = ctrl.maskOriginal.masked;
                            const placeholder = currentMask.mask.replaceAll('0', currentMask.placeholderChar);
                            element[0].setAttribute('placeholder', placeholder);
                        }
                    }

                    element.on('$destroy', () => {
                        mask.destroy();
                        deregObserver();
                        deregWatch();
                    });
                };
            },
        ],
    };
}

/*@ngInject*/
function maskConfigDirective($parse, maskControlService) {
    return {
        restrict: 'A',
        bindToController: true,
        priority: 100,
        controllerAs: 'mask',
        controller: [
            '$scope',
            '$element',
            '$attrs',
            '$parse',
            function (scope, element, attrs) {
                const ctrl = this;
                ctrl.$onInit = function () {
                    const config = $parse(attrs.maskConfig)(scope);
                    maskControlService.setMaskControlConfig(config || {});
                };
            },
        ],
    };
}

export { maskDirective, maskConfigDirective };
