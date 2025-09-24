import {LozadAdvObserverMode, LozadAdvOptions} from "./types";


export const LOZAD_OBSERVER_MODE: Record<LozadAdvObserverMode, LozadAdvObserverMode> = {
    default: 'default',
    observerAlways: 'observerAlways',
};



const lozadAdvConstants: LozadAdvOptions = {
    rootMargin: '0px',
    threshold: 0,
    afterWindowLoaded: true,
    load: function load(element) {
        const el = <HTMLImageElement>element

        if (el.dataset.src) {
            el.src = el.dataset.src;
        }
        if (el.dataset.srcset) {
            el.srcset = el.dataset.srcset;
        }
        if (el.dataset.backgroundImage) {
            el.style.backgroundImage = 'url(' + el.dataset.backgroundImage + ')';
        }
    },
};

export default lozadAdvConstants;
