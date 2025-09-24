import '../../../node_modules/@glidejs/glide/dist/css/glide.core.css';
import '../../../node_modules/@glidejs/glide/dist/css/glide.theme.css';

import { carouselExtRootComponent } from './carouselExtRoot.component.js';
import { carouselExtTrackComponent } from './track/carouselExtTrack.component.js';
import { carouselExtDotComponent } from './dot/carouselExtDot.component.js';
import { carouselExtNavComponent } from './nav/carouselExtNav.component.js';
import { carouselExtSlidesComponent } from './slides/carouselExtSlides.component.js';
import { carouselExtItemComponent } from './item/carouselExtItem.component.js';
import { carouselExtDefault } from './carouselExt.constant.js';
import './carousel-ext.scss';

const MODULE_NAME = 'carouselExt';

angular
    .module(MODULE_NAME, [])
    .component('carouselExtRoot', carouselExtRootComponent)
    .component('carouselExtTrack', carouselExtTrackComponent)
    .component('carouselExtDot', carouselExtDotComponent)
    .component('carouselExtNav', carouselExtNavComponent)
    .component('carouselExtSlides', carouselExtSlidesComponent)
    .component('carouselExtItem', carouselExtItemComponent)
    .constant('carouselExtDefault', carouselExtDefault);

export default MODULE_NAME;
