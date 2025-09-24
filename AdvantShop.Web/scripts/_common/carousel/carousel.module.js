import './styles/carousel.scss';

import './carouselNative.js';

import carouselService from './services/carouselService.js';
import { carouselDirective, carouselImgDirective } from './directives/carouselDirectives.js';
import CarouselCtrl from './controllers/carouselController.js';

const moduleName = 'carousel';

angular
    .module(moduleName, [])
    .directive('carousel', carouselDirective)
    .directive('carouselImg', carouselImgDirective)
    .service('carouselService', carouselService)
    .controller('CarouselCtrl', CarouselCtrl)
    .constant('carouselDefault', {
        isVertical: false,
        scrollCount: 1,
        nav: true,
        dots: false,
        speed: 600,
        auto: false,
        autoPause: 5000,
        indexActive: 0,
        prevIcon: 'icon-left-open-after',
        nextIcon: 'icon-right-open-after',
        prevIconVertical: 'icon-up-open-after',
        nextIconVertical: 'icon-down-open-after',
        prevClass: 'cs-l-1-interactive',
        nextClass: 'cs-l-1-interactive',
        dotsClass: undefined,
        dotsItemClass: 'cs-bg-i-1',
        dotsItemSelectedClass: null,
        dotsItemInnerSelectedClass: null,
        visibleMax: null,
        visibleMin: null,
        itemSelectClass: null,
        carouselClass: null,
        stretch: true,
        navPosition: 'inside', //inside or outside
        responsive: null,
    });

export default moduleName;
