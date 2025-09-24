
/**
  * jQuery.enllax.js v1.1.0
  * https://github.com/mmkjony/enllax.js
  * demo: http://mmkjony.github.io/enllax.js/
  *
  * Copyright 2015, MMK Jony
  * This content is released under the MIT license
 **/

(function ($) {
    'use strict';
    var arrayObj = [];


    $.fn.enllax = function (command, options) {

        var windowHeight = window.innerHeight;

        if (typeof command === 'object') {

            var _options = $.extend({
                ratio: 0,
                type: 'background', //foreground
                direction: 'vertical' //horizontal
            }, options);

            var elem = this;

            init(elem, _options);

            window.addEventListener('scroll', function () {
                var scrollTop = window.pageYOffset;

                window.requestAnimationFrame(updateAll.bind(this, scrollTop));
            });

            window.addEventListener('resize', function () {
                init(elem, _options);
            });


        } else if (typeof command === 'string') {
            if (command === 'destroy') {
                ellaxDestroy(this);
            } else if (command === 'init') {
                init(this, _options);
            }
        }

        function init(elem, options) {
            var _options = $.extend({
                ratio: 0,
                type: 'background', //foreground
                direction: 'vertical' //horizontal
            }, options);


            elem.each(function () {
                var ratio;
                var type;
                var dir;
                var $this = $(this);
                var height = $this.outerHeight();
                var width = $this.outerWidth();
                var dataRat = $this.data('enllax-ratio') || _options.ratio;
                var dataType = $this.data('enllax-type') || _options.type;
                var dataDir = $this.data('enllax-direction') || _options.direction;
                var url = $this.data('enllax-url');

                $this.addClass('parallax');

                if (dataRat) {
                    ratio = dataRat;
                }
                else { ratio = _options.ratio; }

                if (dataType) {
                    type = dataType;
                }
                else { type = _options.type; }

                if (dataDir) {
                    dir = dataDir;
                }
                else { dir = _options.direction; }

                this.ellaxDestroy = ellaxDestroy;

                var scrolling = $(this).scrollTop();

                if (type === 'background') {
                    if (dir === 'vertical') {

                        var image = new Image();

                        image.onload = function () {
                            var bgSizeX, bgSizeY;

                            var coorBlock = $this[0].getBoundingClientRect(),
                                offset = coorBlock.top,
                                bgHeight = Math.round((height * ratio) + height),
                                imageWidth = this.width,
                                bgY = (offset / windowHeight) * (height - bgHeight),
                                ratioImage = this.width / this.height,
                                emptySpaceVertical = coorBlock.height - this.height,
                                emptySpaceHorizontal = coorBlock.width - this.width,

                                heightBgWithEmptySpaceVertical = emptySpaceVertical + this.height + (this.height * ratio),
                                widthBgWithEmptySpaceHorizontal = emptySpaceHorizontal + this.width + (this.width * ratio);


                            //новая ширина относительно новой высоты
                            var newWidth = heightBgWithEmptySpaceVertical * ratioImage;

                            if (newWidth < coorBlock.width) {

                                bgSizeX = widthBgWithEmptySpaceHorizontal;

                                bgSizeY = widthBgWithEmptySpaceHorizontal / ratioImage;

                            } else {

                                bgSizeX = newWidth;

                                bgSizeY = heightBgWithEmptySpaceVertical;

                            }

                            $this.css({
                                'background-size': getBgSizeString(bgSizeX, bgSizeY),
                                'background-position': 'center ' + bgY + 'px'
                            });

                            $this.removeClass('parallax--initialize');

                            arrayObj.push({
                                ratio: ratio,
                                type: type,
                                dir: dir,
                                el: $this,
                                height: height,
                                width: width,
                                dataRat: dataRat,
                                dataType: dataType,
                                dataDir: dataDir,
                                dataId: $this.data('enllax-id'),
                                bgHeight: bgHeight
                            });

                            if (_options.onInit) {
                                _options.onInit(bgY);
                            }
                        };

                        image.src = url;
                    }
                }

            });

        }

        function getBgSizeString(width, height) {
            return width + 'px ' + height + 'px';
        }

        function ellaxDestroy(els) {
            els.removeClass('parallax');

            els.each(function (index, el) {
                $(el).removeClass('parallax');
                arrayObj.forEach(function (item, i) {
                    if (el == item.el[0]) {
                        arrayObj.splice(i, 1);
                    }
                });
            });
        }


        function update(scrolling, item) {
            if (item.type == 'background') {
                var ratio = item.ratio;
                var offset = item.el[0].getBoundingClientRect().top;
                var height = item.height;
                var bgHeight = item.bgHeight;

                var bgY = (offset / windowHeight) * (height - bgHeight);

                if (item.dir == 'vertical') {
                    item.el.css({
                        'background-position': 'center ' + bgY + 'px'
                    });
                }
            }
        }

        function updateAll(scrollTop) {
            arrayObj.forEach(function (item) {
                update(scrollTop, item);
            });
        }
    };


})(jQuery);
