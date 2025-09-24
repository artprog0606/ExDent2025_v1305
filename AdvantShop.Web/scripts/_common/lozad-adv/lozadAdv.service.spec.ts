/**
 * @jest-environment jsdom
 */

import angular from 'angular';
import 'angular-mocks';
import {fn} from 'jest-mock';
import lozadAdvModule from './lozadAdv.module.js';
import {ILozadAdvService} from "./lozadAdv.service";
import {LozadAdvObserverParams} from "./types";
import {LOZAD_OBSERVER_MODE} from "./lozadAdv.constants";
import {mockIntersectionObserverInit} from "./__mocks__/lozadAdv";

describe('LozadAdvService', function () {
    let lozadAdvService: ILozadAdvService,
        lozadAdvDefault;

    const {instanceList, mockIntersectionObserver} = mockIntersectionObserverInit();
    document.body.innerHTML = `
        <div id="target" style="width:100px; height:100px"></div>
        <div id="target2" style="width:100px; height:100px"></div>
    `;



    const getDefaultParams = (params?: Partial<LozadAdvObserverParams>): LozadAdvObserverParams => {
        const target = document.querySelector<HTMLElement>('#target')!;

        return Object.assign({
            lozadAdvKey: "init",
            options: lozadAdvDefault,
            element: target,
            lozadAdvDebounce: 500,
            lozadAdv: () => null,
            lozadObserverMode: LOZAD_OBSERVER_MODE.default,

        }, params)
    }

    beforeEach(() => {
        angular.mock.module(lozadAdvModule);
        angular.mock.inject(($injector) => {
            lozadAdvDefault = $injector.get('lozadAdvDefault');
            lozadAdvService = $injector.get('lozadAdvService');
        });
    });

    afterEach(() => {
        instanceList.length = 0;
    });

    describe('initObserver', function () {
        it('should init observer and return instance', function () {
            mockIntersectionObserver();
            const target = document.querySelector<HTMLElement>('#target')!;
            const callback = fn();
            const instance = lozadAdvService.initObserver(getDefaultParams());
            instance.observeWrapper(target, callback)
            expect(instance.observer).toBe(instanceList[0]);
            expect(instanceList[0].observe).toHaveBeenCalled();
        });

        it('should return exist observer by key', function () {
            mockIntersectionObserver();
            const target = document.querySelector<HTMLElement>('#target')!;
            const callback = fn();

            const instance = lozadAdvService.initObserver(getDefaultParams());
            instance.observeWrapper(target, callback)

            expect(instance.observer).toBe(instanceList[0]);
            expect(instanceList[0].observe).toHaveBeenCalledTimes(1);

            const instanceEqual = lozadAdvService.initObserver(getDefaultParams());
            instanceEqual.observeWrapper(target, callback)

            expect(instanceEqual.observer).toBe(instance.observer);
            expect(instanceList[0].observe).toHaveBeenCalledTimes(2);

            const instanceOther = lozadAdvService.initObserver(getDefaultParams({lozadAdvKey: 'initOther'}));
            instanceOther.observeWrapper(target, callback)

            expect(instanceOther.observer).toBe(instanceList[1]);
            expect(instanceOther.observer).not.toBe(instance.observer);
        });

        it('should return exist observer by options', function () {
            mockIntersectionObserver();
            const target = document.querySelector<HTMLElement>('#target')!;
            const callback = fn();

            const instance = lozadAdvService.initObserver(getDefaultParams({lozadAdvKey: undefined}));

            const instanceEqual = lozadAdvService.initObserver(getDefaultParams({lozadAdvKey: undefined, options: {...lozadAdvDefault}}));
            expect(instance.observerId).toBe(instanceEqual.observerId);

            const instanceOther = lozadAdvService.initObserver(getDefaultParams({
                lozadAdvKey: undefined,
                options: {...lozadAdvDefault, customProp: 'test'}
            }));
            expect(instance.observerId).not.toBe(instanceOther.observerId);
        });
    });
    describe('getHash', () => {
        const options = {
            test: 1,
            prop2: 'asdasd',
            obj: {
                init: true,
                arr: ['aaa', 'bbb', 'ccc'],
            },
        };

        const options2 = {
            prop2: 'asdasd',
            obj: {
                init: true,
                arr: ['aaa', 'bbb', 'ccc'],
            },
        };

        const options3 = {
            prop2: 'asdasd',
            obj: {
                init: true,
                arr: ['ccc', 'aaa', 'bbb'],
            },
        };

        it('should return hash by options', function () {
            const hash = lozadAdvService.getHash(options);
            expect(hash.length).toBe(10);
        });

        it('should return equals hash by options', function () {
            const hash1 = lozadAdvService.getHash(options);
            const hash2 = lozadAdvService.getHash({...options});
            expect(hash1).toBe(hash2);
        });

        it('should return different hash by options', function () {
            const hash1 = lozadAdvService.getHash(options);
            const hash2 = lozadAdvService.getHash(options2);
            const hash3 = lozadAdvService.getHash(options3);

            expect(hash1).not.toBe(hash2);
            expect(hash1).not.toBe(hash3);
            expect(hash2).not.toBe(hash3);
        });
    });
    describe('reObserve', function () {
        it('should re-observe element', function () {
            mockIntersectionObserver();
            const target = document.querySelector<HTMLElement>('#target')!;
            const callback = fn();
            const instance = lozadAdvService.initObserver(getDefaultParams());
            instance.observeWrapper(target, callback)

            expect(instanceList[0].observe).toHaveBeenCalledTimes(1);

            const result = lozadAdvService.reObserve('init', target);
            expect(result).toBeTruthy();

            expect(instanceList[0].unobserve).toHaveBeenCalledTimes(1);
            expect(instanceList[0].observe).toHaveBeenCalledTimes(2);
        });

        it('should return falsy if observe not found', function () {
            mockIntersectionObserver();
            const target = document.querySelector<HTMLElement>('#target')!;
            expect(lozadAdvService.reObserve('other', target)).toBeFalsy();
        });
    });

    describe('onIntersection', function () {
        it('should return callback for intersection with custom load', async function () {
            mockIntersectionObserver();
            const target = document.querySelector<HTMLElement>('#target')!;
            const callbackMock = fn();
            const callback = lozadAdvService.onIntersection(callbackMock, 1);

            callback({isIntersecting: true} as IntersectionObserverEntry, {} as IntersectionObserver);
            await new Promise((resolve, reject) => {
                setTimeout(() => {
                    resolve({});
                }, 1);
            });

            expect(callbackMock).toHaveBeenCalledWith({isIntersecting: true}, {});
        });
        it('should skip debounce, when false', function () {
            mockIntersectionObserver();
            const target = document.querySelector<HTMLElement>('#target')!;
            const callbackMock = fn();
            const callback = lozadAdvService.onIntersection(callbackMock, false);

            callback({isIntersecting: true} as IntersectionObserverEntry, {} as IntersectionObserver);
            expect(callbackMock).toHaveBeenCalledWith({isIntersecting: true}, {});
        });
    });

    describe('isElementInViewport', function () {
        beforeEach(() => {
            Object.defineProperty(window, 'innerHeight', {
                writable: true,
                configurable: true,
                value: 1000,
            });

        })
        it('should return true, when element in viewport', function () {
            const target = document.querySelector<HTMLElement>('#target')!;

            expect(
                lozadAdvService.isElementInViewport(target, {
                    boundingClientRect: {
                        top: 0,
                        height: 100,
                    },
                } as IntersectionObserverEntry),
            ).toBeTruthy();
        });
        it('should return false, when element is not in viewport', function () {
            const target = document.querySelector<HTMLElement>('#target')!;

            expect(
                lozadAdvService.isElementInViewport(target, {
                    boundingClientRect: {
                        top: 1100,
                        height: 100,
                    },
                } as IntersectionObserverEntry),
            ).toBeFalsy();
        });
    });
});
