import {fn} from "jest-mock";

export const mockIntersectionObserverInit = () => {
    let instanceList: IntersectionObserver[] = [];


    return {
        instanceList,
        mockIntersectionObserver: (entries?: IntersectionObserverEntry[]) => {
            const mockIntersectionObserverFn = fn((callback: IntersectionObserverCallback, options: IntersectionObserverInit) => {

                const instance = {
                    observe: fn((element) => {
                        callback(entries ?? [{isIntersecting: true}] as IntersectionObserverEntry[], {...instance})
                    }),
                    unobserve: fn(() => null),
                    disconnect: fn(() => null),
                } as unknown as IntersectionObserver;

                instanceList.push(instance);
                return instance;
            });

            window["IntersectionObserver"] = mockIntersectionObserverFn as {
                prototype: IntersectionObserver;
                new(callback: IntersectionObserverCallback, options?: IntersectionObserverInit): IntersectionObserver;
            };
        },
    }
};
