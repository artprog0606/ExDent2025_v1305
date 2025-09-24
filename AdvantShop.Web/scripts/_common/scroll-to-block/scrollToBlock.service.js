/* @ngInject */
export const scrollToBlockService = function ($window, scrollToBlockConfig) {
    const service = this;
    const scrollendSupport = 'onscrollend' in $window;
    service.scrollToBlock = (el, options = { smooth: true, offsetTop: 0 }, onStartScroll, onEndScroll) => {
        const elCoords = el.getBoundingClientRect();
        const elCoordsTopWithScroll = elCoords.top + window.pageYOffset;
        const standardValue = elCoordsTopWithScroll - options.offsetTop;
        const numberScrollToEl = scrollToBlockConfig.calcExtend != null ? scrollToBlockConfig.calcExtend(standardValue) : standardValue;

        if (onStartScroll != null) {
            onStartScroll();
        }

        if (onEndScroll != null) {
            if (scrollendSupport) {
                $window.addEventListener('scrollend', function scrollendCallback() {
                    $window.removeEventListener('scrollend', scrollendCallback);
                    onEndScroll();
                });
            } else {
                let timerId;
                $window.addEventListener('scroll', function scrollendCallback() {
                    if (timerId != null) {
                        clearTimeout(timerId);
                    }

                    timerId = setTimeout(() => {
                        $window.removeEventListener('scroll', scrollendCallback);
                        onEndScroll();
                    }, 300);
                });
            }
        }

        $window.scrollTo({
            top: Math.round(numberScrollToEl),
            behavior: options.smooth !== false ? 'smooth' : 'auto',
        });
    };
};
