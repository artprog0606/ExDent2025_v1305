
; (function (window) {
    'use strict';

    var qazy = new window.Qazy();
    var elements = [];
    //start
    (function spy() {
        if (document.readyState !== 'complete') {
            elements = elements.concat(qazy.searchImages());
            setTimeout(spy, 100);
        }
    })();

    window.addEventListener('load', function () {
        setTimeout(function () {
            //qazy.observe(elements);
            var images = document.querySelectorAll('[data-qazy]:not(.js-qazy-loaded), [data-qazy-container]:not([data-inplace-rich]) img:not(.js-qazy-loaded):not([data-inplace-image])');
            // сделал так чтоб при перерисовки DOM были актуальные ссылки на img
            images = Array.prototype.slice.call(images, 0);
            qazy.observe(images);
        }, 200);
    });

})(window);
