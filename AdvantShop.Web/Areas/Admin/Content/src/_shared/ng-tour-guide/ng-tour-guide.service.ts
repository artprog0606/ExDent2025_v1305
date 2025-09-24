import {TourGuideClient} from '@sjmc11/tourguidejs';
import {TourGuideStep} from '@sjmc11/tourguidejs/src/types/TourGuideStep';
import {IDocumentService, IQService, IWindowService, ILocationService, IRootScopeService, IDeferred} from 'angular';
import {TourGuideOptions} from '@sjmc11/tourguidejs/src/core/options';

export interface INgTourGuideService {
    addStepsItem(steps: TourGuideStep): void;

    addWatchVisibility(element: Element): Promise<unknown>;

    isVisibleElement(element: Element): boolean;

    isTourGuideEnabled(group: string): boolean;

    finishTour(group: string): void;

    isFinished(group: string): boolean;
}

export class NgTourGuideService implements INgTourGuideService {
    private tg: TourGuideClient;
    private isRunnig: boolean;
    private mapListVisibility: Map<HTMLElement, (value: unknown) => void>;
    private mapListGroups: Map<string, IDeferred<boolean>>;
    private mutationObserver?: MutationObserver | null;
    private activeGroup?: string;
    private storageKey: string;

    /* @ngInject */
    constructor(
        private readonly $document: IDocumentService,
        private readonly $window: IWindowService,
        private readonly ngTourGuideOptions: TourGuideOptions,
        private readonly $q: IQService,
        private readonly $location: ILocationService,
        private readonly $rootScope: IRootScopeService,
        private readonly urlHelper
    ) {
        this.storageKey = this.getKeyStorage();
        this.isRunnig = false;
        this.mapListVisibility = new Map();
        this.mapListGroups = new Map();
        this.tg = new TourGuideClient(this.ngTourGuideOptions);
        this.tg.onAfterExit(() => {
            this.finishTour(this.activeGroup);
            this.activeGroup = undefined;
        });

        this.$rootScope.$on('$locationChangeSuccess', () => {
            const tour = this.$location.search()?.tour;

            if (typeof tour !== 'undefined' && this.mapListGroups.size > 0) {
                const defer = this.mapListGroups.get(tour);
                if (defer) {
                    defer.resolve(true);
                }
            }
        });
    }

    initObserver() {
        if (typeof this.mutationObserver !== 'undefined' && this.mutationObserver !== null) {
            return this.mutationObserver;
        }
        let timerDebounce;
        this.mutationObserver = new MutationObserver(() => {
            if (timerDebounce) {
                clearTimeout(timerDebounce);
            }

            timerDebounce = setTimeout(() => {
                this.checkElements();
            }, 500);
        });

        this.mutationObserver.observe(document.documentElement, {subtree: true, childList: true, attributes: true});

        return this.mutationObserver;
    }

    checkElements() {
        for (const [element, resolve] of Array.from(this.mapListVisibility.entries())) {
            if (element.clientWidth > 0 && element.clientHeight > 0) {
                resolve(true);
                this.mapListVisibility.delete(element);
            }
        }

        if (this.mapListVisibility.size === 0 && typeof this.mutationObserver !== 'undefined' && this.mutationObserver !== null) {
            this.mutationObserver.disconnect();
            this.mutationObserver = null;
        }
    }

    isVisibleElement(element: HTMLElement): boolean {
        return element.clientWidth > 0 && element.clientHeight > 0;
    }

    isTourGuideEnabled(group = 'tour'): boolean {
        return this.$location.search().tour === group;
    }

    addWatchLocation(group = 'tour') {
        const defer = this.$q.defer<boolean>();
        this.mapListGroups.set(group, defer);
        return defer.promise;
    }

    startLazy(group?: string) {
        return new Promise((resolve) => {
            if ((this.$document[0] as Document).readyState === 'complete') {
                resolve(true);
            } else {
                this.$window.addEventListener('load', () => resolve(true));
            }
        }).then(() => this.start(group));
    }

    start(group?: string) {
        this.activeGroup = group;
        this.tg.start(group);
    }

    addWatchVisibility(element: HTMLElement) {
        return new Promise((resolve) => {
            this.mapListVisibility.set(element, resolve);
        });
    }

    addStepsItem(stepsItem: TourGuideStep) {
        this.initObserver();
        const elementDOM =
            typeof stepsItem.target === 'string'
                ? (this.$document[0] as Document).querySelector<HTMLElement>(stepsItem.target)
                : (stepsItem.target as HTMLElement);

        if (typeof elementDOM === 'undefined' || elementDOM === null) {
            return;
        }

        this.$q
            .when(!this.isTourGuideEnabled(stepsItem.group) ? this.addWatchLocation(stepsItem.group) : true)
            .then(() => (!this.isVisibleElement(elementDOM) ? this.addWatchVisibility(elementDOM) : true))
            .then(() => {
                this.tg.addSteps([stepsItem]);
                if (!this.isRunnig && !this.isFinished(stepsItem.group)) {
                    this.isRunnig = true;
                    this.startLazy(stepsItem.group);
                }
            });
    }

    getKeyStorage() {
        return `tourcomplte_${this.urlHelper.transformBaseUriToKey()}`
    }

    finishTour(group = 'tour') {
        const value = localStorage.getItem(this.storageKey);
        let listComppleted: string[] = [];
        if (value !== null) {
            listComppleted = value.split(',');
        }
        if (listComppleted.includes(group)) {
            throw new Error(`Group "${group}" already completed in tour`)
        }
        listComppleted.push(group)
        localStorage.setItem(this.storageKey, listComppleted.join(','))
    }

    isFinished(group = 'tour') {
        const value = localStorage.getItem(this.storageKey);
        return value !== null && value.split(',').includes(group);
    }
}
