(function () {
    'use strict';
    var media = window.matchMedia('@mq');

    media.addListener(onChangeViewport);

    function onChangeViewport(media) {
        switchClassOnHtml(media.matches);
    }
    function switchClassOnHtml(isMobile) {
        if (isMobile === true) {
            document.documentElement.classList.add('@(isMobile ? "desktop-redirect-panel" : "mobile-redirect-panel")');
        } else {
            document.documentElement.classList.remove('@(isMobile ? "desktop-redirect-panel" : "mobile-redirect-panel")');
        }
    }
})();
