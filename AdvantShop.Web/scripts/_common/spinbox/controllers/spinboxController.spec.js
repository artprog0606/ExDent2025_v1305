/**
 * @jest-environment jsdom
 */

import angular from 'angular';
import 'angular-mocks';
import { fn } from 'jest-mock';

import '../../../../node_modules/angular-translate/dist/angular-translate.js';
import '../spinbox.module.js';

const createCtrl = ($controller, name, defaultArgs) => {
    return {
        getNewCtrl: (args) => {
            return $controller(name, angular.merge(defaultArgs, args));
        },
    };
};

describe('SpinboxController', function () {
    let $timeout,
        $controller,
        $rootScope,
        spinboxKeyCodeAllow,
        spinboxTooltipTextType,
        $scope,
        ctrl,
        $translate,
        $attrs,
        getNewCtrl,
        $flushPendingTasks;

    beforeEach(() => {
        angular.mock.module('pascalprecht.translate');
        angular.mock.module('spinbox');
        angular.mock.inject(($injector) => {
            $timeout = $injector.get('$timeout');
            $controller = $injector.get('$controller');

            $rootScope = $injector.get('$rootScope');
            $translate = $injector.get('$translate');
            spinboxKeyCodeAllow = $injector.get('spinboxKeyCodeAllow');
            spinboxTooltipTextType = $injector.get('spinboxTooltipTextType');
            $flushPendingTasks = $injector.get('$flushPendingTasks');
        });

        $scope = $rootScope.$new();

        getNewCtrl = createCtrl($controller, 'SpinboxCtrl', {
            $element: [],
            $timeout,
            spinboxKeyCodeAllow,
            spinboxTooltipTextType,
            $translate,
            $attrs: {},
            $scope,
        }).getNewCtrl;

        ctrl = getNewCtrl({});
        ctrl.form = {
            $setDirty: fn(),
        };
    });
    describe('$onInit', function () {
        it('preprocess initial data', () => {
            ctrl.step = '0,5';
            ctrl.max = '11';
            ctrl.min = 0;
            ctrl.value = '3,4';

            ctrl.$onInit();

            expect(ctrl.step).toEqual(0.5);
            expect(ctrl.max).toEqual(11);
            expect(ctrl.min).toEqual(0);
            expect(ctrl.value).toEqual(3.5);
            expect(ctrl.lessBtnDisabled).toEqual(false);
            expect(ctrl.moreBtnDisabled).toEqual(false);
            expect(ctrl.tooltipText).toEqual(null);
            expect(ctrl.isVisibleArrows).toEqual(true);
            expect(ctrl.needComma).toEqual(false);
            expect(ctrl.inputName).toEqual(undefined);
            expect(ctrl.onlyValidation).toEqual(false);
        });

        it('preprocess initial data when min or max is null', () => {
            ctrl.step = '0,5';
            ctrl.max = null;
            ctrl.min = null;
            ctrl.value = '3,4';

            ctrl.$onInit();

            expect(ctrl.step).toEqual(0.5);
            expect(ctrl.max).toEqual(Infinity);
            expect(ctrl.min).toEqual(0);
            expect(ctrl.value).toEqual(3.5);
            expect(ctrl.lessBtnDisabled).toEqual(false);
            expect(ctrl.moreBtnDisabled).toEqual(false);
        });
    });
    describe('numberRound', function () {
        it('should correct round, when floating point precision error', async function () {
            expect(ctrl.numberRound(0.1 + 0.2)).toBe(0.3);
        });

        it('should correct round, when simple number', async function () {
            expect(ctrl.numberRound(0.2 + 0.2)).toBe(0.4);
        });
    });
    describe('formatterNumber', function () {
        it('should add comma if need', function () {
            ctrl.needComma = true;
            expect(ctrl.formatterNumber(1.3)).toBe('1,3');
        });

        it('should add comma if not need', function () {
            ctrl.needComma = false;
            expect(ctrl.formatterNumber(1.3)).toBe(1.3);
        });
    });

    describe('getRemainder', function () {
        it('should correct return remainder, when floating point precision error', async function () {
            expect(ctrl.getRemainder(9, 0.9)).toBe(0.9);
        });
    });

    describe('isShareWhole', function () {
        it('should correct true, when floating point precision error', async function () {
            expect(ctrl.isShareWhole(9, 0.9)).toBeTruthy();
        });

        it.each([
            { num1: 9, num2: 3, result: true },
            { num1: 8, num2: 2, result: true },
            { num1: 1.5, num2: 0.5, result: true },
            {
                num1: 1.5,
                num2: 0.05,
                result: true,
            },
            { num1: 1.5, num2: 0.02, result: true },
            { num1: 1000, num2: 20, result: true },

            { num1: 10, num2: 3, result: false },
            { num1: 9, num2: 2, result: false },
            { num1: 9, num2: 5, result: false },
            {
                num1: 1.5,
                num2: 0.7,
                result: false,
            },
            { num1: 1.5, num2: 0.04, result: false },
        ])('should correct "$result", when "$num1" and "$num2"', ({ num1, num2, result }) => {
            expect(ctrl.isShareWhole(num1, num2)).toEqual(result);
        });
    });

    describe('getNearValue', function () {
        it.each([
            //check min
            { max: 90, value: 0, step: 0.9, min: 9, result: 9 },
            { max: 90, value: 0, step: 0.8, min: 9, result: 9.6 },
            {
                max: 90,
                value: 0,
                step: 3,
                min: 4,
                result: 6,
            },
            { max: 90, value: 0, step: 0.25, min: 1.5, result: 1.5 },
            { max: 1.33, value: 0, step: 0.03, min: 0.33, result: 0.33 },

            //check max
            { max: 90, value: 100, step: 0.9, min: 9, result: 90 },
            { max: 90, value: 100, step: 0.8, min: 9, result: 89.6 },
            {
                max: 90,
                value: 100,
                step: 3,
                min: 4,
                result: 90,
            },
            { max: 81, value: 100, step: 0.23, min: 1.5, result: 80.96 },
            { max: 1.33, value: 100, step: 0.03, min: 0.33, result: 1.32 },
            {
                max: 1.33,
                value: 1.33,
                step: 0.03,
                min: 0.33,
                result: 1.32,
            },
        ])(
            'should correctly return limit is "$result", when max = $max; value = $value; step = $step; min = $min;',
            ({ max, step, value, min, result }) => {
                ctrl.max = max;
                ctrl.min = min;
                ctrl.step = step;
                expect(ctrl.getNearValue(value)).toEqual(result);
            },
        );
    });
    describe('getNearestLessMultiple', function () {
        it('should correct round, when floating point precision error', async function () {
            expect(ctrl.getNearestLessMultiple(1.2, 0.2)).toBe(1);
        });

        it('should correct round, when simple number', async function () {
            expect(ctrl.getNearestLessMultiple(10, 3)).toBe(9);
        });
    });
    describe('getNearestMoreMultiple', function () {
        it('should correct round, when floating point precision error', async function () {
            expect(ctrl.getNearestMoreMultiple(1.2, 0.2)).toBe(1.2);
        });

        it('should correct round, when simple number', async function () {
            expect(ctrl.getNearestMoreMultiple(10, 3)).toBe(12);
        });
    });

    describe('processValue', function () {
        const diff = 1.1;
        const processFn = (num) => num + diff;

        it.each([
            { value: 1, result: 1 + diff },
            { value: 4.3, result: 4.3 + diff },
        ])('process value $value', ({ value, result }) => {
            expect(ctrl.processValue(value, processFn)).toEqual(result);
        });

        it.each([
            { value: 1, result: '2,1' },
            { value: 4.3, result: '5,4' },
        ])('process value $value and normalize out', ({ value, result }) => {
            expect(ctrl.processValue(value, processFn, true)).toEqual(result);
        });
    });
    describe('valueFoldStep', function () {
        it('should get data from spinbox input', () => {
            const spinboxInputFn = fn(() => {
                return { value: 99 };
            });
            ctrl = getNewCtrl({
                $element: [
                    {
                        querySelector: spinboxInputFn,
                    },
                ],
            });

            ctrl.max = 10;
            ctrl.min = 0;
            ctrl.step = 1;
            ctrl.beforeUpdate = fn();
            ctrl.valueFoldStep();
            expect(spinboxInputFn).toHaveBeenCalledWith('.spinbox-input');
        });
        it('skip correcting if enable onlyValidation', () => {
            ctrl = getNewCtrl({
                $element: [
                    {
                        querySelector: fn(() => {
                            return { value: 5.5 };
                        }),
                    },
                ],
            });

            ctrl.onlyValidation = true;
            ctrl.max = 5;
            ctrl.min = 0;
            ctrl.step = 1;
            ctrl.beforeUpdate = fn();
            ctrl.valueFoldStep();
            expect(ctrl.value).toEqual('5,5');
        });

        it('skip correcting when value is null and not required', () => {
            ctrl = getNewCtrl({
                $element: [
                    {
                        querySelector: fn(() => {
                            return { value: null };
                        }),
                    },
                ],
            });

            ctrl.isRequired = false;
            ctrl.max = 10;
            ctrl.min = 0;
            ctrl.step = 1;
            ctrl.beforeUpdate = fn();
            ctrl.valueFoldStep();
            expect(ctrl.value).toBeUndefined();
        });

        it('should not correcting to value when value and min value is zero', () => {
            ctrl = getNewCtrl({
                $element: [
                    {
                        querySelector: fn(() => {
                            return { value: 0 };
                        }),
                    },
                ],
            });

            ctrl.max = 99;
            ctrl.min = 0;
            ctrl.step = 1.3;
            ctrl.beforeUpdate = fn();
            ctrl.valueFoldStep();
            expect(ctrl.value).toEqual('0');
        });

        it('send min value to validationBeforeUpdateFn when value NaN', () => {
            const validationBeforeUpdateFn = fn(() => {
                return false;
            });
            ctrl = getNewCtrl({
                $attrs: {
                    validationBeforeUpdateFn,
                },
                $element: [
                    {
                        querySelector: fn(() => {
                            return { value: null };
                        }),
                    },
                ],
            });
            ctrl.validationBeforeUpdateFn = validationBeforeUpdateFn;
            ctrl.max = 99;
            ctrl.min = '5,5';
            ctrl.step = 1.3;
            ctrl.beforeUpdate = fn();
            ctrl.valueFoldStep();
            expect(validationBeforeUpdateFn).toHaveBeenCalledWith({
                value: 5.5,
                proxy: undefined,
            });
            expect(ctrl.value).toBeUndefined();
        });

        it('should not correcting, when validationBeforeUpdateFn return false', () => {
            const validationBeforeUpdateFn = fn(() => {
                return false;
            });
            ctrl = getNewCtrl({
                $attrs: {
                    validationBeforeUpdateFn,
                },
                $element: [
                    {
                        querySelector: fn(() => {
                            return { value: -1 };
                        }),
                    },
                ],
            });
            ctrl.value = '5';
            ctrl.validationBeforeUpdateFn = validationBeforeUpdateFn;
            ctrl.max = 99;
            ctrl.min = 0;
            ctrl.step = 1.3;
            ctrl.beforeUpdate = fn();
            ctrl.valueFoldStep();
            expect(validationBeforeUpdateFn).toHaveBeenCalledWith({
                value: -1,
                proxy: undefined,
            });
            expect(ctrl.value).toEqual('5');
        });

        it('should set zero when value is NaN and min value is zero', () => {
            ctrl = getNewCtrl({
                $element: [
                    {
                        querySelector: fn(() => {
                            return { value: null };
                        }),
                    },
                ],
            });

            ctrl.max = 99;
            ctrl.min = 0;
            ctrl.step = 1.3;
            ctrl.beforeUpdate = fn();
            ctrl.valueFoldStep();
            expect(ctrl.value).toEqual('0');
        });

        it('should set min value with step when value is NaN and min value is not zero', () => {
            ctrl = getNewCtrl({
                $element: [
                    {
                        querySelector: fn(() => {
                            return { value: null };
                        }),
                    },
                ],
            });

            ctrl.max = 99;
            ctrl.min = 1;
            ctrl.step = 1.3;
            ctrl.beforeUpdate = fn();
            ctrl.valueFoldStep();
            expect(ctrl.value).toEqual('1,3');
        });

        it('should correcting value when value more max', () => {
            ctrl = getNewCtrl({
                $element: [
                    {
                        querySelector: fn(() => {
                            return { value: 99 };
                        }),
                    },
                ],
            });

            ctrl.max = 10;
            ctrl.min = 0;
            ctrl.step = 1;
            ctrl.beforeUpdate = fn();
            ctrl.valueFoldStep();
            expect(ctrl.value).toEqual('10');
        });

        it('should correcting value when value less min', () => {
            ctrl = getNewCtrl({
                $element: [
                    {
                        querySelector: fn(() => {
                            return { value: 1 };
                        }),
                    },
                ],
            });

            ctrl.max = 99;
            ctrl.min = 10;
            ctrl.step = 1;
            ctrl.beforeUpdate = fn();
            ctrl.valueFoldStep();
            expect(ctrl.value).toEqual('10');
        });

        it('should correcting value when value is not divisible by step', () => {
            ctrl = getNewCtrl({
                $element: [
                    {
                        querySelector: fn(() => {
                            return { value: 2 };
                        }),
                    },
                ],
            });

            ctrl.max = 99;
            ctrl.min = 1;
            ctrl.step = 1.3;
            ctrl.beforeUpdate = fn();
            ctrl.valueFoldStep();
            expect(ctrl.value).toEqual('2,6');
        });

        it('should correcting value when value is not divisible by step and min value is zero', () => {
            ctrl = getNewCtrl({
                $element: [
                    {
                        querySelector: fn(() => {
                            return { value: 1 };
                        }),
                    },
                ],
            });

            ctrl.max = 99;
            ctrl.min = 0;
            ctrl.step = 1.5;
            ctrl.beforeUpdate = fn();
            ctrl.valueFoldStep();
            expect(ctrl.value).toEqual('1,5');
        });

        it('should correcting value when value is zero', () => {
            ctrl = getNewCtrl({
                $element: [
                    {
                        querySelector: fn(() => {
                            return { value: 0 };
                        }),
                    },
                ],
            });

            ctrl.max = 99;
            ctrl.min = 1;
            ctrl.step = 1.3;
            ctrl.beforeUpdate = fn();
            ctrl.valueFoldStep();
            expect(ctrl.value).toEqual('1,3');
        });

        it('validate after adjustment by step', () => {
            ctrl = getNewCtrl({
                $element: [
                    {
                        querySelector: fn(() => {
                            return { value: 10 };
                        }),
                    },
                ],
            });

            ctrl.max = 10;
            ctrl.min = 0;
            ctrl.step = 1.3;
            ctrl.beforeUpdate = fn();
            ctrl.valueFoldStep();
            expect(ctrl.value).toEqual('10');
        });
    });
    describe('less', function () {
        it('substract step from value', () => {
            ctrl.max = 99;
            ctrl.min = 1;
            ctrl.value = '10';
            ctrl.step = 1.3;
            ctrl.beforeUpdate = fn();
            ctrl.$onInit();
            ctrl.less();
            expect(ctrl.value).toEqual(9.1);
        });

        it('validate after adjustment by step', () => {
            ctrl.max = 99;
            ctrl.min = 10;
            ctrl.value = '10';
            ctrl.step = 1.3;
            ctrl.beforeUpdate = fn();
            ctrl.$onInit();

            ctrl.less();
            expect(ctrl.value).toEqual(10.4);
        });
        it('should not correcting, when validationBeforeUpdateFn return false', () => {
            const validationBeforeUpdateFn = fn(() => {
                return false;
            });
            ctrl = getNewCtrl({
                $attrs: {
                    validationBeforeUpdateFn,
                },
            });

            ctrl.validationBeforeUpdateFn = validationBeforeUpdateFn;
            ctrl.max = 99;
            ctrl.min = 10;
            ctrl.value = '10';
            ctrl.step = 1.3;
            ctrl.beforeUpdate = fn();
            ctrl.$onInit();

            ctrl.less();

            expect(validationBeforeUpdateFn).toHaveBeenCalledWith({
                value: 10.4,
                proxy: undefined,
            });
            expect(ctrl.value).toEqual(10.4);
        });
    });

    describe('more', function () {
        it('add step to value', () => {
            ctrl.max = 99;
            ctrl.min = 1;
            ctrl.value = '10';
            ctrl.step = 1.3;
            ctrl.beforeUpdate = fn();
            ctrl.$onInit();

            ctrl.more();
            expect(ctrl.value).toEqual(11.7);
        });

        it('validate after adjustment by step', () => {
            ctrl.max = 10;
            ctrl.min = 1;
            ctrl.value = '10';
            ctrl.step = 1.3;
            ctrl.beforeUpdate = fn();
            ctrl.$onInit();

            ctrl.more();
            expect(ctrl.value).toEqual(9.1);
        });

        it('should not correcting, when validationBeforeUpdateFn return false', () => {
            const validationBeforeUpdateFn = fn(() => {
                return false;
            });
            ctrl = getNewCtrl({
                $attrs: {
                    validationBeforeUpdateFn,
                },
            });

            ctrl.validationBeforeUpdateFn = validationBeforeUpdateFn;
            ctrl.max = 10;
            ctrl.min = 1;
            ctrl.value = '10';
            ctrl.step = 1.3;
            ctrl.beforeUpdate = fn();
            ctrl.$onInit();

            ctrl.more();

            expect(validationBeforeUpdateFn).toHaveBeenCalledWith({
                value: 9.1,
                proxy: undefined,
            });
            expect(ctrl.value).toEqual(10.4);
        });
    });
    describe('callbackCall', function () {
        it('should call after less button', () => {
            ctrl.max = 99;
            ctrl.min = 0;
            ctrl.value = '10';

            ctrl.step = 1;
            ctrl.beforeUpdate = fn();
            ctrl.updateFn = fn();

            expect(ctrl.updateFn).not.toHaveBeenCalled();
            ctrl.less();
            $flushPendingTasks();
            expect(ctrl.updateFn).toHaveBeenCalledTimes(1);
        });

        it('should call after more button', () => {
            ctrl.max = 99;
            ctrl.min = 0;
            ctrl.value = '10';
            ctrl.step = 1;
            ctrl.beforeUpdate = fn();
            ctrl.updateFn = fn();

            expect(ctrl.updateFn).not.toHaveBeenCalled();

            ctrl.more();
            $flushPendingTasks();
            expect(ctrl.updateFn).toHaveBeenCalledTimes(1);
        });

        it('should call after auto update value', () => {
            ctrl = getNewCtrl({
                $element: [
                    {
                        querySelector: fn(() => {
                            return { value: 99 };
                        }),
                    },
                ],
            });
            ctrl.max = 10;
            ctrl.min = 0;
            ctrl.step = 1;
            ctrl.beforeUpdate = fn();
            ctrl.updateFn = fn();

            expect(ctrl.updateFn).not.toHaveBeenCalled();
            ctrl.valueFoldStep();
            $flushPendingTasks();
            expect(ctrl.updateFn).toHaveBeenCalledTimes(1);
        });
    });
});
