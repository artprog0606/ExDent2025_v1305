/**
 * @jest-environment jsdom
 */

import './__mocks__/PubSub.js';

import { fn } from 'jest-mock';

import { PubSub } from './PubSub.js';

describe('PubSub', () => {
    const eventList = ['cart/add', 'product/remove', 'product/add'];

    const Fn1 = fn();
    const Fn2 = fn();
    const Fn3 = fn();

    const callbacks = [Fn1, Fn2, Fn3];

    beforeEach(() => {
        PubSub.clear();
    });

    test('Subscribe', () => {
        for (let index = 0; index < callbacks.length; index++) {
            PubSub.subscribe(eventList[0], callbacks[index]);
        }

        expect(PubSub.getSubscribes(eventList[0])).toEqual([Fn1, Fn2, Fn3]);
    });

    test('Unsubscribe', () => {
        const removeCallback1 = PubSub.subscribe(eventList[0], Fn2);
        PubSub.subscribe(eventList[0], Fn1);
        PubSub.subscribe(eventList[0], Fn3);

        PubSub.subscribe(eventList[1], Fn1);
        const removeCallback2 = PubSub.subscribe(eventList[1], Fn2);
        PubSub.subscribe(eventList[1], Fn3);

        PubSub.subscribe(eventList[2], Fn1);
        PubSub.subscribe(eventList[2], Fn3);
        const removeCallback3 = PubSub.subscribe(eventList[2], Fn2);

        removeCallback1();
        removeCallback2();
        removeCallback3();

        expect(PubSub.getSubscribes(eventList[0])).toEqual([Fn1, Fn3]);

        expect(PubSub.getSubscribes(eventList[1])).toEqual([Fn1, Fn3]);

        expect(PubSub.getSubscribes(eventList[2])).toEqual([Fn1, Fn3]);
    });

    describe('Publish', () => {
        test('should return correct value when one param', () => {
            const spyFn = fn();
            const param = 5;

            PubSub.subscribe(eventList[0], spyFn);

            expect(PubSub.publish(eventList[0], param)).toBeTruthy();
            expect(spyFn).toHaveBeenCalledWith(param);
        });
        test('should return correct value when array param', () => {
            const spyFn = fn();
            const param = [5, 4];

            PubSub.subscribe(eventList[0], spyFn);

            expect(PubSub.publish(eventList[0], param)).toBeTruthy();
            expect(spyFn).toHaveBeenCalledWith(param);
        });
        test('should return correct value when object param', () => {
            const spyFn = fn();
            const param = { arr: [5, 4] };

            PubSub.subscribe(eventList[0], spyFn);

            expect(PubSub.publish(eventList[0], param)).toBeTruthy();
            expect(spyFn).toHaveBeenCalledWith(param);
        });
        test('should return correct value when several params', () => {
            const spyFn = fn();
            const params = [{ arr: [5, 4] }, 10];

            PubSub.subscribe(eventList[0], spyFn);

            expect(PubSub.publish(eventList[0], ...params)).toBeTruthy();
            expect(spyFn).toHaveBeenCalledWith(...params);
        });
        test('should return undefined when null param', () => {
            const spyFn = fn();

            PubSub.subscribe(eventList[0], spyFn);

            expect(PubSub.publish(eventList[0])).toBeTruthy();
            expect(spyFn).toHaveBeenCalled();
        });

        test('should fail publish when the wrong event', () => {
            const spyFn = fn();

            PubSub.subscribe(eventList[0], spyFn);

            expect(PubSub.publish('wrong/event', 'asdas', 49, 15)).toBeFalsy();
            expect(spyFn).not.toHaveBeenCalled();
        });

        test('should do not called removed callback', () => {
            const spyFn = fn();

            const removeCallback = PubSub.subscribe(eventList[0], spyFn);
            removeCallback();

            expect(PubSub.publish(eventList[0], 5)).toBeTruthy();
            expect(spyFn).not.toHaveBeenCalled();
        });
    });
});
