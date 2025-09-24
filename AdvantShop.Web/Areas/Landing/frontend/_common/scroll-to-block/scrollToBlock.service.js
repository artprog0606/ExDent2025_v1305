(function (ng) {
    'use strict';

    const scrollToBlockService = function ($window, transformerService) {
        const service = this;

        service.scrollToBlock = (el, smooth) => {
            const elCoords = el.getBoundingClientRect();
            const elCoordsTopWithScroll = elCoords.top + window.pageYOffset;
            const transformersList = transformerService.getTransformersStorage();
            const transfomersNeedHeight = transformersList.reduce(function (acc, it) {
                if (it._elementStartRect.topWithScroll < elCoordsTopWithScroll) {
                    return acc + it.getHeightElement();
                }
                return acc;
            }, 0);
            const numberScrollToEl = elCoordsTopWithScroll - transfomersNeedHeight;
            $window.scrollTo({
                top: Math.round(numberScrollToEl),
                behavior: smooth ? 'smooth' : 'auto',
            });
        };

        service.nextUntil = function (elem, selector, filter) {
            elem = elem.nextElementSibling;
            while (elem) {
                if (elem.matches(selector)) break;
                if (filter && !elem.matches(filter)) {
                    elem = elem.nextElementSibling;
                    continue;
                }
                elem = elem.nextElementSibling;
            }
            return elem;
        };
    };

    scrollToBlockService.$inject = ['$window', 'transformerService'];

    angular.module('scrollToBlock').service('scrollToBlockService', scrollToBlockService);
})(window.angular);
