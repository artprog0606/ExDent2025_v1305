import carouselExtRootCtrl from './carouselExtRoot.ctrl.js';

export const carouselExtRootComponent = {
    controller: carouselExtRootCtrl,
    bindings: {
        carouselExtType: '@?',
        carouselExtStartAt: '<?',
        carouselExtPerView: '<?',
        carouselExtFocusAt: '@?',
        carouselExtGap: '<?',
        carouselExtAutoplay: '<?',
        carouselExtHoverPause: '<?',
        carouselExtKeyBoard: '<?',
        carouselExtBound: '<?',
        carouselExtSwipeThreshold: '<?',
        carouselExtDragThreshold: '<?',
        carouselExtPerTouch: '<?',
        carouselExtTouchRatio: '<?',
        carouselExtTouchAngle: '<?',
        carouselExtAnimationDuration: '<?',
        carouselExtRewind: '<?',
        carouselExtRewindDuration: '<?',
        carouselExtAnimationTimingFunc: '@?',
        carouselExtDirection: '@?',
        carouselExtPeek: '<?',
        carouselExtBreakPoints: '<?',
        carouselExtClasses: '<?',
        carouselExtThrottle: '<?',
        //опции именно для этого компонента
        carouselExtEvents: '<?', //https://glidejs.com/docs/events/
        carouselExtEventsCallback: '<?',
        carouselExtDuration: '<?',
        carouselExtAutoPlay: '<?',
    },
};
