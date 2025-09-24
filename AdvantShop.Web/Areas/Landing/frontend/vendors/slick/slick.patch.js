/*
 * скрипт переопредел¤ет метод карусели http://kenwheeler.github.io/slick/, а именно закоментировал вызов event.stopImmediatePropagation();
 * так как из-за него не срабатывает inplace-редактирование, сделанное с помощью ckeditor'
 */
(function ($) {
    'use strict';

    var focusHandler = function() {

        var _ = this;

        _.$slider
            .off('focus.slick blur.slick')
            .on('focus.slick blur.slick', '*', function(event) {

            //event.stopImmediatePropagation();

            var $sf = $(this);

            setTimeout(function() {

                if( _.options.pauseOnFocus ) {
                    _.focussed = $sf.is(':focus');
                    _.autoPlay();
                }

            }, 0);

        });
    };




    var oldFn = $.fn.slick;

    var oldDestroy;

    $.fn.slick = function () {
        var result = oldFn.apply(this, arguments);

        if (!Array.isArray(result)) return result;

        return result.each(function (index, item) {

            item.slick.focusHandler = focusHandler.bind(item.slick);
            item.slick.$slider.off('focus.slick blur.slick', '*');
            item.slick.focusHandler();
        });
    };

})(window.jQuery);
