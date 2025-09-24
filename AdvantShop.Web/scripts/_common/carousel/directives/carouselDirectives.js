/* @ngInject */
function carouselDirective($compile, $document, $window, carouselDefault) {
    return {
        restrict: 'A',
        scope: {
            isVertical: '&',
            scrollCount: '&',
            nav: '&',
            dots: '&',
            speed: '&',
            auto: '&',
            autoPause: '&',
            indexActive: '=?',
            prevIcon: '@',
            nextIcon: '@',
            filterFn: '&',
            prevIconVertical: '@',
            nextIconVertical: '@',
            prevClass: '@',
            nextClass: '@',
            dotsClass: '@',
            dotsItemClass: '@',
            dotsItemSelectedClass: '@',
            dotsItemInnerSelectedClass: '@',
            visibleMax: '&',
            visibleMin: '&',
            itemSelectClass: '@',
            itemActiveClass: '@',
            carouselClass: '@',
            stretch: '&',
            navPosition: '@',
            initOnLoad: '<?',
            load: '=?',
            initFn: '&',
            itemSelect: '&',
            initilazeTo: '@',
            responsive: '<?', //пример: {768: {slidesToShow : 3}}
            asNavFor: '@', // accept id carousel
            scrollNav: '<?',
        },
        controller: 'CarouselCtrl',
        controllerAs: 'carousel',
        bindToController: true,
        link: function (scope, element, attrs, ctrl, trasclude) {
            var scrollCount = ctrl.scrollCount(),
                isVertical = ctrl.isVertical(),
                nav = ctrl.nav(),
                dots = ctrl.dots(),
                speed = ctrl.speed(),
                auto = ctrl.auto(),
                autoPause = ctrl.autoPause(),
                visibleMax = ctrl.visibleMax(),
                visibleMin = ctrl.visibleMin(),
                stretch = ctrl.stretch();

            ctrl.isVertical = isVertical != null ? isVertical : carouselDefault.isVertical;
            ctrl.scrollCount = scrollCount != null ? scrollCount : carouselDefault.scrollCount;
            ctrl.nav = nav != null ? nav : carouselDefault.nav;
            ctrl.dots = dots != null ? dots : carouselDefault.dots;
            ctrl.speed = speed != null ? speed : carouselDefault.speed;
            ctrl.auto = auto != null ? auto : carouselDefault.auto;
            ctrl.autoPause = autoPause != null ? autoPause : carouselDefault.autoPause;
            ctrl.indexActive = angular.isNumber(ctrl.indexActive) ? ctrl.indexActive : carouselDefault.indexActive;
            ctrl.prevIcon = ctrl.prevIcon != null ? ctrl.prevIcon : carouselDefault.prevIcon;
            ctrl.nextIcon = ctrl.nextIcon != null ? ctrl.nextIcon : carouselDefault.nextIcon;
            ctrl.prevIconVertical = ctrl.prevIconVertical != null ? ctrl.prevIconVertical : carouselDefault.prevIconVertical;
            ctrl.nextIconVertical = ctrl.nextIconVertical != null ? ctrl.nextIconVertical : carouselDefault.nextIconVertical;
            ctrl.prevClass = ctrl.prevClass != null ? ctrl.prevClass : carouselDefault.prevClass;
            ctrl.nextClass = ctrl.nextClass != null ? ctrl.nextClass : carouselDefault.nextClass;
            (ctrl.dotsClass = ctrl.dotsClass != null ? ctrl.dotsClass : carouselDefault.dotsClass),
                (ctrl.dotsItemClass = ctrl.dotsItemClass != null ? ctrl.dotsItemClass : carouselDefault.dotsItemClass);
            ctrl.dotsItemSelectedClass = ctrl.dotsItemSelectedClass != null ? ctrl.dotsItemSelectedClass : carouselDefault.dotsItemSelectedClass;
            ctrl.dotsItemInnerSelectedClass =
                ctrl.dotsItemInnerSelectedClass != null ? ctrl.dotsItemInnerSelectedClass : carouselDefault.dotsItemInnerSelectedClass;
            ctrl.visibleMax = visibleMax != null ? visibleMax : carouselDefault.visibleMax;
            ctrl.visibleMin = visibleMin != null ? visibleMin : carouselDefault.visibleMin;
            ctrl.itemSelectClass = ctrl.itemSelectClass != null ? ctrl.itemSelectClass : carouselDefault.itemSelectClass;
            ctrl.stretch = stretch != null ? stretch : carouselDefault.stretch;
            ctrl.navPosition = ctrl.navPosition != null ? ctrl.navPosition : carouselDefault.navPosition;

            ctrl.carouselOptions = {
                isVertical: ctrl.isVertical,
                scrollCount: ctrl.scrollCount,
                nav: ctrl.nav,
                dots: ctrl.dots,
                speed: ctrl.speed,
                auto: ctrl.auto,
                autoPause: ctrl.autoPause,
                indexActive: ctrl.indexActive,
                prevIcon: ctrl.prevIcon,
                nextIcon: ctrl.nextIcon,
                prevIconVertical: ctrl.prevIconVertical,
                nextIconVertical: ctrl.nextIconVertical,
                prevClass: ctrl.prevClass,
                nextClass: ctrl.nextClass,
                filterFn:
                    attrs.filterFn != null && ctrl.filterFn != null
                        ? function (item, index, array) {
                              return ctrl.filterFn({
                                  item: item,
                                  index: index,
                                  array: array,
                              });
                          }
                        : null,
                dotsClass: ctrl.dotsClass,
                dotsItemClass: ctrl.dotsItemClass,
                dotsItemSelectedClass: ctrl.dotsItemSelectedClass,
                dotsItemInnerSelectedClass: ctrl.dotsItemInnerSelectedClass,
                visibleMax: ctrl.visibleMax,
                visibleMin: ctrl.visibleMin,
                itemSelectClass: ctrl.itemSelectClass,
                itemActiveClass: ctrl.itemActiveClass,
                carouselClass: ctrl.carouselClass,
                stretch: ctrl.stretch,
                navPosition: ctrl.navPosition,
                animateString: ctrl.animateString,
                initFn: function (carousel) {
                    ctrl.initFn({ carousel: carousel });

                    scope.$apply();
                },
                itemSelect: function (carousel, item, index) {
                    ctrl.itemSelect({
                        carousel: carousel,
                        item: item.carouselItemData,
                        index: index,
                    });

                    scope.$apply();
                },
                responsive: ctrl.responsive,
                asNavFor: ctrl.asNavFor,
                scrollNav: ctrl.scrollNav === true,
                onLazyLoad: function (img, carouselItem) {
                    ctrl.callFnFromCarouselImg(img, carouselItem);

                    scope.$apply();
                },
                onDoClone: function (cloneResult) {
                    $compile(cloneResult.clonesPrev)(scope);
                    $compile(cloneResult.clonesNext)(scope);

                    scope.$apply();
                },
                onUpdate: function () {
                    scope.$apply();
                },
            };

            function _initWrap() {
                if (ctrl.initOnLoad === true) {
                    var unbind = scope.$watch('carousel.load', function (newValue, oldValue) {
                        if (newValue) {
                            ctrl.init();
                            unbind();
                        }
                    });
                } else {
                    ctrl.init();
                }
            }

            function memoryItemsAsClone() {
                var children = element[0].children;

                if (children != null && children.length > 0) {
                    if (children.length === 1 && children[0].classList.contains('carousel-inner')) {
                        children = children[0].children;
                    }

                    if (children != null) {
                        for (var i = 0, len = children.length; i < len; i++) {
                            children[i].carouselItemData = children[i].carouselItemData || {};
                            children[i].carouselItemData.originalClone = children[i].cloneNode(true);
                        }
                    }
                }
            }

            memoryItemsAsClone();

            if ($document[0].readyState !== 'complete') {
                $window.addEventListener('load', function () {
                    _initWrap();
                });
            } else {
                _initWrap();
            }
        },
    };
}

/* @ngInject */
function carouselImgDirective($parse) {
    return {
        require: '^?carousel',
        link: function (scope, element, attrs, carouselCtrl) {
            if (carouselCtrl != null) {
                var callbackParsed = $parse(attrs.carouselImg);

                var callback = function (img, carouselItem) {
                    return callbackParsed(scope, {
                        img: img,
                        carouselItem: carouselItem,
                    });
                };

                var carouselImgId = carouselCtrl.addCarouselImg({
                    callback: callback,
                });

                attrs.$set('dataCarouselImgId', carouselImgId);
            }
        },
    };
}

export { carouselDirective, carouselImgDirective };
