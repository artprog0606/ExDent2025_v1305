/* @ngInject */
export default function SetCssCustomProps($parse, setCssCustomPropsService) {
    // use { nameVariable :nameProperty  } || [ { nameVariable :nameProperty  } ]
    return {
        scope: true,
        controller:
            /* @ngInject */
            function ($element, setCssCustomPropsService) {
                const ctrl = this;
                ctrl.sum = (...arg) => {
                    return setCssCustomPropsService.sum($element[0], arg);
                };
            },
        controllerAs: `setCssCustomProps`,
        bindToController: true,
        link: {
            post: function (scope, element, attrs, ctrl) {
                const objMap = $parse(attrs.setCssCustomProps)(scope);
                if (objMap) {
                    const computedStyleDocument = getComputedStyle(document.documentElement);
                    if (Array.isArray(objMap)) {
                        objMap.forEach((it, i, arr) => {
                            setCssCustomPropsService.getValueFromMapAndSetProperty(element[0], it, computedStyleDocument);
                        });
                    } else {
                        setCssCustomPropsService.getValueFromMapAndSetProperty(element[0], objMap, computedStyleDocument);
                    }
                }
            },
        },
    };
}
