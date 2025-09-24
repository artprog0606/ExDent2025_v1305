import Glide from '@glidejs/glide';
import { SliderDisableToggle } from './carouselExt.plugin.js';

let nextId = 0;
class carouselExtRootCtrl {
    /*@ngInject*/
    constructor($element, carouselExtDefault, $q) {
        this.carouselExtDefault = carouselExtDefault;
        this.el = $element[0];
        this.$q = $q;
    }

    $onInit() {
        this.massiveSlides = [];
    }

    getSublingIndex(currentIndex) {
        const result = [];
        if (currentIndex - 1 >= 0) {
            result.push(currentIndex - 1);
        } else {
            result.push(this.massiveSlides.length - 1);
        }

        if (currentIndex + 1 > this.massiveSlides.length - 1) {
            result.push(0);
        } else {
            result.push(currentIndex + 1);
        }

        return result;
    }

    //preloadSiblingSlide(currentIndex) {
    //    const observer = lozad();
    //    [currentIndex, ...this.getSublingIndex(currentIndex)].forEach(index => {
    //        if (!this.massiveSlides[index].isLoaded) {
    //            this.massiveSlides[index].slide.el.querySelectorAll("picture source").forEach(source =>{
    //                observer.triggerLoad(source);
    //            })
    //            this.massiveSlides[index].isLoaded = true;
    //        }
    //    })

    //}

    $postLink() {
        const CAROUSEL_CLASS = `glide-${nextId}`;
        ++nextId;

        this.el.classList.add(CAROUSEL_CLASS);

        const defaults = this.carouselExtDefault;

        if (this.carouselExtType === 'carousel' && this.carouselExtFocusAt === 'center' && this.carouselExtKeyBoard === true) {
            throw new Error(' Board works only with slider type and a non-centered focusAt setting.');
        }

        if (this.carouselExtType === 'carousel' && this.carouselExtRewind === true) {
            throw new Error('Rewind works only with slider type.');
        }

        const options = {
            type: this.carouselExtType != null ? this.carouselExtType : defaults.carouselExtType,
            startAt: this.carouselExtStartAt != null ? this.carouselExtStartAt : defaults.carouselExtStartAt,
            perView: this.carouselExtPerView != null ? this.carouselExtPerView : defaults.carouselExtPerView,
            focusAt: this.carouselExtFocusAt != null ? this.carouselExtFocusAt : defaults.carouselExtFocusAt,
            gap: this.carouselExtGap != null ? this.carouselExtGap : defaults.carouselExtGap,
            hoverpause: this.carouselExtHoverPause != null ? this.carouselExtHoverPause : defaults.carouselExtHoverPause,
            keyboard: this.carouselExtKeyBoard != null ? this.carouselExtKeyBoard : defaults.carouselExtKeyBoard,
            bound: this.carouselExtBound != null ? this.carouselExtBound : defaults.carouselExtBound,
            swipeThreshold: this.carouselExtSwipeThreshold != null ? this.carouselExtSwipeThreshold : defaults.carouselExtSwipeThreshold,
            dragThreshold: this.carouselExtDragThreshold != null ? this.carouselExtDragThreshold : defaults.carouselExtDragThreshold,
            perTouch: this.carouselExtPerTouch != null ? this.carouselExtPerTouch : defaults.carouselExtPerTouch,
            touchRatio: this.carouselExtTouchRatio != null ? this.carouselExtTouchRatio : defaults.carouselExtTouchRatio,
            touchAngle: this.carouselExtTouchAngle != null ? this.carouselExtTouchAngle : defaults.carouselExtTouchAngle,
            animationDuration: this.carouselExtAnimationDuration != null ? this.carouselExtAnimationDuration : defaults.carouselExtAnimationDuration,
            rewind: this.carouselExtRewind != null ? this.carouselExtRewind : defaults.carouselExtRewind,
            rewindDuration: this.carouselExtRewindDuration != null ? this.carouselExtRewindDuration : defaults.carouselExtRewindDuration,
            animationTimingFunc:
                this.carouselExtAnimationTimingFunc != null ? this.carouselExtAnimationTimingFunc : defaults.carouselExtAnimationTimingFunc,
            direction: this.carouselExtDirection != null ? this.carouselExtDirection : defaults.carouselExtDirection,
            peek: this.carouselExtPeek != null ? this.carouselExtPeek : defaults.carouselExtPeek,
            breakpoints: this.carouselExtBreakPoints != null ? this.carouselExtBreakPoints : defaults.carouselExtBreakPoints,
            classes: this.carouselExtClasses != null ? this.carouselExtClasses : defaults.carouselExtClasses,
            throttle: this.carouselExtThrottle != null ? this.carouselExtThrottle : defaults.carouselExtThrottle,
            duration: this.carouselExtDuration != null ? this.carouselExtDuration : defaults.carouselExtDuration,
            autoplay: this.carouselExtAutoPlay != null ? this.carouselExtAutoPlay : defaults.carouselExtAutoPlay,
        };

        var glide = new Glide(`.${CAROUSEL_CLASS}`, options);

        if (this.carouselExtEvents != null && this.carouselExtEvents.length > 0 && this.carouselExtEventsCallback != null) {
            glide.on(this.carouselExtEvents, this.carouselExtEventsCallback.bind(this));
        }

        this.$q.all([this.whenReady(), whenAdvantshopStylesLoaded()]).then(() => {
            this.glide = glide.mount({
                SliderDisableToggle: SliderDisableToggle,
            });
        });
    }

    whenReady() {
        const defer = this.$q.defer();
        if (document.readyState === 'complete') {
            defer.resolve();
        } else {
            window.addEventListener('load', function start() {
                window.removeEventListener('load', start);
                defer.resolve();
            });
        }
        return defer.promise;
    }

    addSlide(slide) {
        this.massiveSlides.push({ slide, isLoaded: false });
    }
}

export default carouselExtRootCtrl;
