/**
 * @jest-environment jsdom
 */

import angular from 'angular';
import {fn} from 'jest-mock';
import lozadAdvModule from './lozadAdv.module.js';
import {createTestApp} from 'angularjs-jest';
import {mockIntersectionObserverInit} from "./__mocks__/lozadAdv";

describe('LozadAdv', function () {
    const {instanceList, mockIntersectionObserver} = mockIntersectionObserverInit();


    const getTestApp = () =>
        createTestApp({
            modules: [lozadAdvModule],
            mocks: {},
            access: ['$flushPendingTasks', 'lozadAdvDefault'],
        });


    beforeEach(() => {
        Object.defineProperty(window, 'innerHeight', {
            writable: true,
            configurable: true,
            value: 1000,
        });
    })
    afterEach(() => {
        instanceList.length = 0;
    });

    it('should call callback fn with true, when element in viewport', function () {


        document.body.innerHTML = `
            <div id="target" lozad-adv="callbackScrollFn(isVisible)" lozad-adv-key="'testKey'" lozad-adv-options="{load:loadMock}" lozad-adv-debounce="false"></div>`;

        mockIntersectionObserver([{
            isIntersecting: true,
            boundingClientRect: {
                top: 0,
                height: 100
            },
            target: document.querySelector("#target")!
        } as IntersectionObserverEntry]);
        const app = getTestApp();
        app.$scope.callbackScrollFn = fn(() => null);
        app.$scope.loadMock = fn();

        const el = app.render(document.body, app.$scope);
        app.$scope.$$childHead.lozadAdv.isElementInViewport = fn(() => true);

        app.$scope.$apply();
        expect(app.$scope.callbackScrollFn).toHaveBeenCalledWith(true);
        expect(instanceList[0].unobserve).toHaveBeenCalled();
    });

    it('should not call callback fn, when element is not in viewport and mode "default"', function () {

        document.body.innerHTML =
            `<div id="target" data-lozad-adv="callbackScrollFn(isVisible)" lozad-adv-key="test" data-lozad-adv-options="{load:loadMock}"  lozad-adv-debounce="false"></div>`

        mockIntersectionObserver([
            {
                isIntersecting: false,
                boundingClientRect: {
                    top: 1100,
                    height: 100,
                },
                target: document.querySelector("#target")!
            } as IntersectionObserverEntry]);


        const app = getTestApp();
        app.$scope.callbackScrollFn = fn(($scope) => null);
        app.$scope.loadMock = fn();

        const el = app.render(document.body, app.$scope,);

        app.$scope.$$childHead.lozadAdv.isElementInViewport = fn(() => false);

        app.$scope.$digest();
        expect(app.$scope.callbackScrollFn).not.toHaveBeenCalled();
    });


    it('should call callback fn with false, when element is not in viewport and mode "observerAlways"', function () {

        document.body.innerHTML =
            `<div id="target" data-lozad-adv="callbackScrollFn(isVisible)" lozad-adv-key="test" lozad-observer-mode="'observerAlways'" data-lozad-adv-options="{load:loadMock}"  lozad-adv-debounce="false"></div>`

        mockIntersectionObserver([
            {
                isIntersecting: false,
                boundingClientRect: {
                    top: 1100,
                    height: 100,
                },
                target: document.querySelector("#target")!
            } as IntersectionObserverEntry]);


        const app = getTestApp();
        app.$scope.callbackScrollFn = fn(($scope) => null);
        app.$scope.loadMock = fn();

        const el = app.render(document.body, app.$scope,);

        app.$scope.$$childHead.lozadAdv.isElementInViewport = fn(() => false);

        app.$scope.$digest();
        expect(app.$scope.callbackScrollFn).toHaveBeenCalledWith(false);
    });


    it('should not unobserve, when mode "observerAlways"', function () {

        document.body.innerHTML =
            `<div id="target"  data-lozad-adv="callbackScrollFn(isVisible)" lozad-adv-key="test" lozad-observer-mode="'observerAlways'" data-lozad-adv-options="{load:loadMock}"  lozad-adv-debounce="false"></div>`

        mockIntersectionObserver([
            {
                isIntersecting: false,
                boundingClientRect: {
                    top: 0,
                    height: 100,
                },
                target: document.querySelector("#target")!
            } as IntersectionObserverEntry]);


        const app = getTestApp();
        app.$scope.callbackScrollFn = fn(($scope) => null);
        app.$scope.loadMock = fn();

        const el = app.render(document.body, app.$scope,);

        app.$scope.$$childHead.lozadAdv.isElementInViewport = fn(() => false);

        app.$scope.$digest();
        expect(app.$scope.callbackScrollFn).toHaveBeenCalledWith(true);
        expect(instanceList[0].unobserve).not.toHaveBeenCalled();
    });

    it('should using one instance observer for multiple elements with equal key', async function () {
        mockIntersectionObserver();
        const app = getTestApp();
        app.$scope.callbackScrollFn = fn(($scope) => null);
        app.$scope.loadMock = fn();

        const el = app.render(
            `<div data-lozad-adv="callbackScrollFn" lozad-adv-key="'test'"></div>
            <div data-lozad-adv="callbackScrollFn" lozad-adv-key="'test'"></div>`,
            app.$scope,
        );

        expect(instanceList.length).toBe(1);
    });

    it('should using one instance observer for multiple elements with equal options', async function () {
        mockIntersectionObserver();
        const app = getTestApp();
        app.$scope.callbackScrollFn = fn(($scope) => null);
        app.$scope.loadMock = fn();

        const el = app.render(
            `<div data-lozad-adv="callbackScrollFn()" data-lozad-adv-options="{load:loadMock}"></div>
            <div data-lozad-adv="callbackScrollFn()" data-lozad-adv-options="{load:loadMock}"></div>`,
            app.$scope,
        );

        expect(instanceList.length).toBe(1);
    });

    it('should using different instance observer for multiple elements with different options', async function () {
        mockIntersectionObserver();
        const app = getTestApp();
        app.$scope.callbackScrollFn = fn(($scope) => null);
        app.$scope.loadMock = fn();

        const el = app.render(
            `<div data-lozad-adv="callbackScrollFn()" data-lozad-adv-options="{load:loadMock, customKey:123}"></div>
            <div data-lozad-adv="callbackScrollFn()" data-lozad-adv-options="{load:loadMock}"></div>`,
            app.$scope,
        );

        expect(instanceList.length).toBe(2);
    });


    it('should using one observer for multiple different element and callbacks', async function () {
        document.body.innerHTML = `<div id="target" data-lozad-adv="callbackScrollFn(isVisible)" data-lozad-adv-options="{load:loadMock}"  lozad-adv-debounce="false"></div>
            <div id="target2" data-lozad-adv="callbackScrollFn2(isVisible)" data-lozad-adv-options="{load:loadMock}"  lozad-adv-debounce="false"></div>`

        mockIntersectionObserver([
            {
                isIntersecting: true,
                boundingClientRect: {
                    top: 100,
                    height: 100,
                },
                target: document.querySelector("#target")!
            } as IntersectionObserverEntry,
            {
                isIntersecting: true,
                boundingClientRect: {
                    top: 200,
                    height: 100,
                },
                target: document.querySelector("#target2")!
            } as IntersectionObserverEntry]);


        const app = getTestApp();
        app.$scope.callbackScrollFn = fn(($scope) => null);
        app.$scope.callbackScrollFn2 = fn(($scope) => null);
        app.$scope.loadMock = fn();

        const el = app.render(document.body, app.$scope);
        app.$scope.$$childHead.lozadAdv.isElementInViewport = fn(() => true);

        app.$scope.$digest();
        expect(instanceList.length).toBe(1);
        expect(app.$scope.callbackScrollFn).toHaveBeenCalled();
        expect(app.$scope.callbackScrollFn2).toHaveBeenCalled();

    });
});
