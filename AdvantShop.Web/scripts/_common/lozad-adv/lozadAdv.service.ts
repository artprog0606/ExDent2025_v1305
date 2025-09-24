import debounceFn from 'debounce-fn';
import {LozadAdvParams, LozadAdvObserverParams, LozadAdvObserverWrap, LozadAdvObservers} from "./types";
import {LOZAD_OBSERVER_MODE} from "./lozadAdv.constants";


export interface ILozadAdvService {
    guardOptions: (params: LozadAdvParams) => void
    onIntersection: (callback: (entry: IntersectionObserverEntry, observer: IntersectionObserver) => void, debounceTime: LozadAdvParams["lozadAdvDebounce"]) => (entry: IntersectionObserverEntry, observer: IntersectionObserver) => void
    isElementInViewport: (element: Element, entry: IntersectionObserverEntry) => boolean
    getHash: (options: Record<string, any>) => string;
    initObserver: (params: LozadAdvObserverParams) => LozadAdvObserverWrap
    reObserve: (key: string, element: HTMLElement) => boolean;
    lozadIntersectionCallback: (key: string) => (entry: IntersectionObserverEntry[], observer: IntersectionObserver) => void,
}
export type ILozadAdvServiceFn = (this: ILozadAdvService) => void


//TODO: если указан только this, то в тестах он превращается в params. указал ngInject у самой функции
const LozadAdvService:ILozadAdvServiceFn  = /*@ngInject*/ function () {

    const service = this;
    let observers: LozadAdvObservers = new Map();

    service.guardOptions = function (params: LozadAdvParams) {
        if (!['number', 'boolean'].includes(typeof params.lozadAdvDebounce)) {
            throw new Error(`[LozadAdv]: lozadAdvDebounce is number or boolean`);
        }

        if (!Object.keys(LOZAD_OBSERVER_MODE).includes(params.lozadObserverMode)) {
            throw new Error(`[LozadAdv]: mode "${params.lozadObserverMode}" not found`);
        }
    };

    service.onIntersection = function onIntersection(callback, debounceTime = 500) {
        return function (entry, observer) {
            if (entry.isIntersecting || service.isElementInViewport(entry.target, entry)) {
                if (debounceTime === false) {
                    callback(entry, observer);
                    return;
                } else if (debounceTime === true) {
                    debounceTime = 500;
                }

                debounceFn(() => callback(entry, observer), {wait: debounceTime})();
            }
        };
    };

    //https://stackoverflow.com/questions/7616461/generate-a-hash-from-string-in-javascript
    service.getHash = function (options) {
        const optionsAsString = JSON.stringify(options);
        let hash = 0;

        for (let i = 0; i < optionsAsString.length; i++) {
            hash = (hash << 5) - hash + optionsAsString.charCodeAt(i);
            hash |= 0;
        }

        const hashString = Math.abs(hash).toString(36);
        const needFill = hashString.length < 10;
        const diffLength = 10 - hashString.length;

        return needFill ? new Array(diffLength).fill('0').join('') + hashString : hashString.slice(0, 10);
    };

    service.isElementInViewport = function isElementInViewport(el, entry) {
        var rect = entry.boundingClientRect;

        var windowHeight = window.innerHeight || document.documentElement.clientHeight;
        // http://stackoverflow.com/questions/325933/determine-whether-two-date-ranges-overlap
        var vertInView = rect.top <= windowHeight && rect.top + rect.height >= 0;
        return vertInView;
    };

    service.lozadIntersectionCallback = (key) => function (entries, observer) {

        const observerWrap = observers.get(key)
        if (observerWrap == null) {
            console.warn(`Observer with key "${key} not found"`)
            return
        }

        entries.forEach((entry) => {
            const handler = observerWrap.params.handlers.find(({element}) => element === entry.target);
            if (handler) {
                handler.callback(entry, observer)
            }
        })
    }

    service.initObserver = (params) => {
        const {lozadAdvKey, options} = params;

        const _key = lozadAdvKey ?? service.getHash(options);
        let observerWrap = observers.get(_key);

        if (!observerWrap) {
            const observer = new IntersectionObserver(service.lozadIntersectionCallback(_key), options);
            observerWrap = {
                observerId: _key,
                observer,
                params: {
                    ...params,
                    handlers:[]
                },

                observeWrapper: function (element, callback) {
                    this.params.handlers.push({callback, element})
                    this.observer.observe(element)
                }
            };

            observers.set(_key, observerWrap);
        }

        return observerWrap;
    };

    service.reObserve = (key, element) => {
        let observerWrap = observers.get(key);

        if (observerWrap) {
            observerWrap.observer.unobserve(element);
            observerWrap.observer.observe(element);
            return true;
        }
        return false;
    };
}

export default LozadAdvService
