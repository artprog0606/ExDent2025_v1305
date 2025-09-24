import {ILozadAdvService} from "./lozadAdv.service";
import {IAttributes, IController, IDocumentService, IParseService, IScope, IWindowService} from "angular";
import {LozadAdvOptions, LozadAdvParams} from "./types";
import {LOZAD_OBSERVER_MODE} from "./lozadAdv.constants";


export type ILozadAdvCtrlFn = (this: ILozadAdvCtrl,
                               $window: IWindowService,
                               $scope: IScope,
                               $element: JQLite,
                               $parse: IParseService,
                               $attrs: IAttributes,
                               $document: IDocumentService,
                               lozadAdvDefault: LozadAdvOptions,
                               lozadAdvService: ILozadAdvService
) => void;

export interface ILozadAdvCtrl extends IController {
    scroll: (entry: IntersectionObserverEntry, observer: IntersectionObserver) => void;
    lozad: (element: Element) => void;
    loadFn: () => void
}


/*@ngInject*/
const LozadAdvCtrl: ILozadAdvCtrlFn = function ($window, $scope, $element, $parse, $attrs, $document, lozadAdvDefault, lozadAdvService) {
    const ctrl = this;

    let elementNative;
    const ctrlParams: LozadAdvParams = {} as LozadAdvParams

    ctrl.$postLink = function () {
        elementNative = $element[0];

        ctrlParams.options = angular.extend({}, lozadAdvDefault, $parse($attrs.lozadAdvOptions)($scope));
        ctrlParams.lozadAdv = $parse($attrs.lozadAdv);
        ctrlParams.lozadObserverMode = $attrs.lozadObserverMode != null ? $parse($attrs.lozadObserverMode)($scope) : LOZAD_OBSERVER_MODE.default;
        ctrlParams.lozadAdvDebounce = $attrs.lozadAdvDebounce != null ? $parse($attrs.lozadAdvDebounce)($scope) : 500;
        ctrlParams.lozadAdvKey = $parse($attrs.lozadAdvKey)($scope);

        lozadAdvService.guardOptions(ctrlParams);


        if (ctrlParams.options.afterWindowLoaded &&
            // @ts-ignore: TODO: fix conflict type IDocumentService for angularjs
            $document[0].readyState !== 'complete') {
            $window.addEventListener('load', ctrl.loadFn);
        } else {
            ctrl.lozad(elementNative);
        }
    };

    ctrl.scroll = function (entry, observer) {
        if (lozadAdvService.isElementInViewport(entry.target, entry)) {
            if (ctrlParams.lozadObserverMode !== LOZAD_OBSERVER_MODE.observerAlways) {
                observer.unobserve(entry.target);
            }
            ctrlParams.options.load(entry.target);
        }
        ctrlParams.lozadAdv($scope, {entry, isVisible: lozadAdvService.isElementInViewport(entry.target, entry)});
        // timeout нужен, чтобы поправить "$digest already in progress" в тестах.
        setTimeout(() => {
            $scope.$digest()
        }, 0)
    };

    ctrl.lozad = function lozad(element) {
        const wrapper = lozadAdvService.initObserver({
            lozadObserverMode: ctrlParams.lozadObserverMode,
            lozadAdvKey: ctrlParams.lozadAdvKey,
            options: ctrlParams.options,
        });

        const callback = ctrlParams.lozadObserverMode !== "observerAlways" ? lozadAdvService.onIntersection((entry, observer) => {
                ctrl.scroll.bind(ctrl, entry, observer)()
            }, ctrlParams.lozadAdvDebounce) :
            ctrl.scroll;

        wrapper.observeWrapper(element, callback)

    };

    ctrl.loadFn = function loadFn() {
        $window.removeEventListener('load', ctrl.loadFn);

        ctrl.lozad(elementNative);
    };
}
export default LozadAdvCtrl
