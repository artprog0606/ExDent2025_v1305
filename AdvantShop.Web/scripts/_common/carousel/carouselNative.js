(function (window) {
    'use strict';

    var isTouchDevice = 'ontouchstart' in document.documentElement,
        autoStop = false,
        clonesForCreate = {},
        transformName = 'transform',
        transitionDurationName = 'transitionDuration',
        webkitTransitionDuration = 'webkitTransitionDuration',
        Carousel,
        storage = {},
        idIncrement = 0,
        deferList = {},
        isOverScrollX = false,
        isScrolling = false;

    Carousel = function (element, options) {
        var id = element.getAttribute('id') || 'carousel_' + (idIncrement += 1);

        this.list = element;
        this.items = Array.prototype.slice.call(element.children).filter((x) => x.classList.contains('js-carousel-clone') === false);
        this.options = options;
        this.responsive = options.responsive;
        this.responsiveOption = this.options.responsive != null ? this.checkResponsive() : null;
        this.propName = this.getPropName(this.getIsVerticalOption());
        this.cache = this.items.slice();
        this.id = id;
        this.dots = [];

        storage[id] = {
            state: { callAsNav: false },
            obj: this,
        };

        this.resolveAsNavForReady(this.id);

        return this;
    };

    Carousel.prototype.addToCache = function (item) {
        this.cache.push(item);
    };

    Carousel.prototype.getFromCache = function (item) {
        var index;

        if (typeof item === 'number') {
            index = item;
        } else {
            index = this.cache.indexOf(item);
        }

        return this.cache[index];
    };

    Carousel.prototype.removeFromCache = function (item) {
        var index;

        if (typeof item === 'number') {
            index = item;
        } else {
            index = this.cache.indexOf(item);
        }

        if (index !== -1) {
            this.cache.splice(index, 1);
        }

        return this.cache[index];
    };

    Carousel.prototype.clearCache = function () {
        this.cache.length = 0;
    };

    Carousel.prototype.getSize = function (totalCount, maxWidth, maxHeight, isVertical, diff) {
        var size = {};

        if (isVertical === false) {
            size['width'] = totalCount * maxWidth - (diff || 0);
            size['height'] = maxHeight;
        } else {
            size['width'] = maxWidth;
            size['height'] = totalCount * maxHeight - (diff || 0);
        }

        return size;
    };

    Carousel.prototype.getPropName = function (isVertical) {
        return isVertical === false ? 'width' : 'height';
    };

    Carousel.prototype.getItemsMaxSizes = function (items) {
        var tempWidth = 0,
            tempHeight = 0,
            maxWidth = 0,
            maxHeigth = 0;

        for (var i = items.length - 1; i >= 0; i--) {
            if (items[i].carouselItemData == null) {
                continue;
            }

            tempWidth = items[i].carouselItemData.originalWidth;

            if (tempWidth > maxWidth) {
                maxWidth = tempWidth;
            }

            tempHeight = items[i].carouselItemData.originalHeight;

            if (tempHeight > maxHeigth) {
                maxHeigth = tempHeight;
            }
        }

        return {
            width: maxWidth,
            height: maxHeigth,
        };
    };

    Carousel.prototype.setItemSize = function (item, value) {
        var self = this,
            valueStr = value + 'px';

        item.style[self.propName] = valueStr;
        //item.style['min' + self.propName.charAt(0).toUpperCase() + self.propName.slice(1)] = valueStr;
        item.style['max' + self.propName.charAt(0).toUpperCase() + self.propName.slice(1)] = valueStr;
        item.style['flexBasis'] = valueStr;
        item.style['msFlexPreferredSize'] = valueStr;
        item.style['webkitFlexBasis'] = valueStr;
    };

    Carousel.prototype.processItems = function (items, saveStyleAttribute) {
        var self = this;

        for (var i = 0, len = items.length - 1; i <= len; i++) {
            self.processItem(items[i], i, saveStyleAttribute);
        }
    };

    Carousel.prototype.processItem = function (item, index, saveStyleAttribute) {
        var self = this;
        var itemStylesComputed = getComputedStyle(item),
            itemBorderLeft,
            itemBorderRight,
            itemBorderTop,
            itemBorderBottom;

        itemBorderLeft = parseInt(itemStylesComputed['border-left-width'], 10);
        itemBorderRight = parseInt(itemStylesComputed['border-right-width'], 10);
        itemBorderTop = parseInt(itemStylesComputed['border-top-width'], 10);
        itemBorderBottom = parseInt(itemStylesComputed['border-bottom-width'], 10);

        itemBorderLeft = isNaN(itemBorderLeft) ? 0 : itemBorderLeft;
        itemBorderRight = isNaN(itemBorderRight) ? 0 : itemBorderRight;
        itemBorderTop = isNaN(itemBorderTop) ? 0 : itemBorderTop;
        itemBorderBottom = isNaN(itemBorderBottom) ? 0 : itemBorderBottom;

        /*,
        itemStylesComputed = getComputedStyle(item),
        itemStylesComputed = item.getBoundingClientRect(),
        itemPaddingLeft,
        itemPaddingRight,
        itemPaddingTop,
        itemPaddingBottom;*/

        /*itemPaddingLeft = parseInt(itemStylesComputed['padding-left'], 10);
        itemPaddingRight = parseInt(itemStylesComputed['padding-right'], 10);
        itemPaddingTop = parseInt(itemStylesComputed['padding-top'], 10);
        itemPaddingBottom = parseInt(itemStylesComputed['padding-bottom'], 10);

        itemPaddingLeft = isNaN(itemPaddingLeft) ? 0 : itemPaddingLeft;
        itemPaddingRight = isNaN(itemPaddingRight) ? 0 : itemPaddingRight;
        itemPaddingTop = isNaN(itemPaddingTop) ? 0 : itemPaddingTop;
        itemPaddingBottom = isNaN(itemPaddingBottom) ? 0 : itemPaddingBottom;*/

        item.carouselItemData = item.carouselItemData || {};
        item.carouselItemData.originalWidth = item.getBoundingClientRect().width;
        item.carouselItemData.originalHeight = item.getBoundingClientRect().height;
        item.carouselItemData.index = index != null ? index : self.items.length;
        item.carouselItemData.parameters =
            item.getAttribute('data-parameters') != null ? new Function('return ' + item.getAttribute('data-parameters'))() : null;
        item.carouselItemData.stylesRaw =
            saveStyleAttribute === true ? item.getAttribute('style') : item.carouselItemData != null ? item.carouselItemData.stylesRaw : null;

        item.classList.add('js-carousel-item');
        item.classList.add('carousel-item');

        return item;
    };

    Carousel.prototype.setSizes = function (wrapSize, innerSize, listSize, itemsSizes) {
        var self = this;

        //if (wrapSize != null) {
        //    self.wrap.style[self.propName] = wrapSize[self.propName] + 'px';
        //}

        if (innerSize != null) {
            self.inner.style[self.propName] = innerSize[self.propName] + 'px';
        }

        if (listSize != null) {
            self.list.style[self.propName] = listSize[self.propName] + 'px';
        }

        if (itemsSizes != null) {
            for (var i = self.items.length - 1; i >= 0; i--) {
                self.setItemSize(self.items[i], itemsSizes[self.propName]);
            }
        }
    };

    Carousel.prototype.calc = function (items, options, responsiveOptions) {
        var self = this;

        var result = responsiveOptions != null ? self.calcResponsive(items, options, responsiveOptions) : self.calcAuto(items, options);

        self.countVisible = result.countVisible;
        self.wrapSize = result.wrapSize;
        self.listSize = result.listSize;
        self.innerSize = result.innerSize;
        self.itemsSize = result.itemsSize;
        self.slidesSize = result.slidesSize;

        return result;
    };

    Carousel.prototype.getCarouselSize = function () {
        var self = this,
            carouselStylesComputed,
            carouselPaddingLeft,
            carouselPaddingRight,
            carouselPaddingTop,
            carouselPaddingBottom;

        carouselStylesComputed = getComputedStyle(self.wrap);

        carouselPaddingLeft = parseInt(carouselStylesComputed['padding-left'], 10);
        carouselPaddingRight = parseInt(carouselStylesComputed['padding-right'], 10);
        carouselPaddingTop = parseInt(carouselStylesComputed['padding-top'], 10);
        carouselPaddingBottom = parseInt(carouselStylesComputed['padding-bottom'], 10);

        carouselPaddingLeft = isNaN(carouselPaddingLeft) ? 0 : carouselPaddingLeft;
        carouselPaddingRight = isNaN(carouselPaddingRight) ? 0 : carouselPaddingRight;
        carouselPaddingTop = isNaN(carouselPaddingTop) ? 0 : carouselPaddingTop;
        carouselPaddingBottom = isNaN(carouselPaddingBottom) ? 0 : carouselPaddingBottom;

        return {
            width: Math.floor(self.wrap.clientWidth - carouselPaddingLeft - carouselPaddingRight),
            height: Math.floor(self.wrap.clientHeight - carouselPaddingTop - carouselPaddingBottom),
        };
    };

    Carousel.prototype.calcAuto = function (items, options) {
        var self = this,
            result = {},
            slidesMaxSize,
            countVisibleDirty,
            carouselSizes,
            countVisible,
            dimension,
            propName,
            slidesSize;

        propName = self.propName;

        carouselSizes = self.getCarouselSize();

        slidesMaxSize = self.getItemsMaxSizes(items);
        countVisibleDirty = carouselSizes[propName] / (slidesMaxSize[propName] || 1);

        countVisible = Math.floor(countVisibleDirty); //Math.round

        //dimension число с плавающей точкой
        if (options.visibleMin != null && options.visibleMin > items.length) {
            countVisible = items.length;
            dimension = countVisibleDirty - countVisible;
        } else if (countVisible > items.length) {
            countVisible = items.length;
            dimension = 0;
        } else if (countVisible < 1) {
            countVisible = 1;
            dimension = countVisibleDirty - countVisible;
        } else {
            dimension = countVisibleDirty - countVisible;
        }

        if ((options.visibleMax != null && options.visibleMax < countVisible) || (options.visibleMin != null && options.visibleMin > countVisible)) {
            if (options.visibleMax != null && options.visibleMax < countVisible) {
                countVisible = options.visibleMax;
            } else if (options.visibleMin != null && options.visibleMin > countVisible) {
                countVisible = options.visibleMin;
                slidesMaxSize[propName] = carouselSizes.width / countVisible;
            }

            if (options.stretch) {
                slidesMaxSize[propName] = carouselSizes[propName] / countVisible;
            } else {
                //carouselSizes[propName] = carouselSizes[propName] - (slidesMaxSize[propName] * countVisible);
                var sizeSlides = slidesMaxSize[propName] * countVisible;
                carouselSizes[propName] =
                    sizeSlides >= carouselSizes[propName]
                        ? carouselSizes[propName]
                        : carouselSizes[propName] - (carouselSizes[propName] - sizeSlides);
            }
        } else {
            if (isNaN(dimension) == false && dimension !== 0) {
                if (options.stretch) {
                    slidesMaxSize[propName] += (slidesMaxSize[propName] * dimension) / countVisible;
                } else {
                    if (dimension > 0) {
                        carouselSizes[propName] = carouselSizes[propName] - slidesMaxSize[propName] * dimension;
                    } else {
                        slidesMaxSize[propName] += (slidesMaxSize[propName] * dimension) / countVisible;

                        if (slidesMaxSize[propName] <= 0) {
                            slidesMaxSize[propName] = carouselSizes[propName];
                        }
                    }
                }
            }
        }

        if (countVisible <= 1) {
            countVisible = 1;
            result = slidesMaxSize[propName];
        } else {
            result = slidesMaxSize[propName];
        }

        var diff = countVisible < items.length ? self.getScrollDiff(result, countVisible) : 0;

        if (options.isVertical === false) {
            slidesSize = {
                width: result - diff,
                height: slidesMaxSize.height,
            };
        } else {
            slidesSize = {
                width: slidesMaxSize.width,
                height: result - diff,
            };
        }

        return {
            countVisible: countVisible,
            wrapSize: carouselSizes,
            listSize: self.getSize(self.items.length, slidesMaxSize.width, slidesMaxSize.height, options.isVertical, diff * self.items.length),
            innerSize: self.getSize(countVisible, slidesMaxSize.width, slidesMaxSize.height, options.isVertical),
            itemsSize: slidesSize,
            slidesSize: slidesSize,
        };
    };

    Carousel.prototype.calcResponsive = function (items, options, responsiveOptions) {
        var self = this,
            propName = self.propName,
            carouselSizes,
            slidesSize,
            slidesMaxSize,
            countVisible;

        if (responsiveOptions.slidesToShow != undefined) {
            countVisible = responsiveOptions.slidesToShow;
        } else {
            throw new Error('Count sliders to show is not set');
        }

        carouselSizes = self.getCarouselSize();
        slidesMaxSize = self.getItemsMaxSizes(items);

        if (options.visibleMin != null && options.visibleMin > items.length) {
            countVisible = items.length;
        } else if (countVisible > items.length) {
            countVisible = items.length;
        }

        if (options.stretch) {
            slidesMaxSize[propName] = carouselSizes[propName] / countVisible;
        } else {
            //carouselSizes[propName] = carouselSizes[propName] - (slidesMaxSize[propName] * countVisible);
            var sizeSlides = slidesMaxSize[propName] * countVisible;
            carouselSizes[propName] =
                sizeSlides >= carouselSizes[propName] ? carouselSizes[propName] : carouselSizes[propName] - (carouselSizes[propName] - sizeSlides);
        }

        slidesMaxSize = {
            width: carouselSizes.width / countVisible,
            height: carouselSizes.height / countVisible,
        };

        var isVertical = self.getIsVerticalOption();
        var diff = self.getScrollDiff(slidesMaxSize[self.getPropName(isVertical)], countVisible);

        if (isVertical === false) {
            slidesSize = {
                width: slidesMaxSize.width - diff,
                //height: slidesMaxSize.height
            };
        } else {
            slidesSize = {
                //width: slidesMaxSize.width,
                height: slidesMaxSize.height - diff,
            };
        }

        return {
            countVisible: countVisible,
            wrapSize: carouselSizes,
            listSize: self.getSize(self.items.length, slidesMaxSize.width, slidesMaxSize.height, isVertical, diff * self.items.length),
            innerSize: self.getSize(countVisible, slidesMaxSize.width, slidesMaxSize.height, isVertical),
            itemsSize: slidesSize,
            slidesSize: slidesSize,
        };
    };

    Carousel.prototype.checkDots = function () {
        var self = this;
        var need;
        if (self.options.dots === true) {
            need = self.items.length !== 1 && self.countVisible !== self.items.length;

            if (need === false) {
                if (self.dotsContainer != null && self.dotsContainer.parentNode != null) {
                    self.dotsContainer.parentNode.removeChild(self.dotsContainer);
                }

                self.dotsContainer = null;
                self.dots.length = 0;
            } else {
                self.renderDots();
                self.selectDots(self.options.indexActive);
            }
        }
    };

    Carousel.prototype.renderDots = function () {
        var self = this,
            newCount,
            dot,
            isRenderContaner = false,
            dim,
            dimAbs,
            itemTemp;

        if (self.dotsContainer == null) {
            self.dotsContainer = self.wrap.querySelector('.carousel-dots');

            if (self.dotsContainer != null) {
                Array.prototype.forEach.call(self.dotsContainer.children, function (el) {
                    self.dots.push(el);
                });
            } else {
                self.dotsContainer = createComponent('ul');
                self.dotsContainer.className = 'carousel-dots ' + (self.options.dotsClass || '');
                isRenderContaner = true;
            }
        }

        newCount = self.items.length / self.options.scrollCount;

        dim = self.dots.length - newCount;
        dimAbs = Math.abs(dim);

        if (dim < 0) {
            for (var d = 0, len = dimAbs; d < len; d++) {
                dot = createComponent('li');
                dot.classList.add('carousel-dots-item');
                dot.innerHTML = '<i class="carousel-dots-item-inner ' + (self.options.dotsItemClass || '') + '" />';
                self.dotsContainer.appendChild(dot);
                self.dots.push(dot);
            }

            self.dots.forEach(function (el, index) {
                el.setAttribute('data-index', index);
            });

            if (isRenderContaner === true) {
                self.wrap.appendChild(self.dotsContainer);
            }
        } else {
            for (var r = dimAbs - 1; r >= 0; r--) {
                itemTemp = self.dots.pop();
                itemTemp.parentNode.removeChild(itemTemp);
            }
        }
    };

    Carousel.prototype.renderNav = function () {
        var self = this,
            nav = self.wrap.querySelector('.carousel-nav'),
            navPrev,
            navNext,
            needRenderNav,
            needRenderPrev,
            needRenderNext;

        //#region nav find or create
        if (nav == null || nav.parentNode !== self.wrap) {
            nav = createComponent('div');
            needRenderNav = true;
        }

        nav.className = 'carousel-nav ' + ('carousel-nav-' + self.options.navPosition);

        self.nav = nav;
        //#endregion

        //#region prev find or create
        navPrev = nav.querySelector('.carousel-nav-prev');

        if (navPrev == null) {
            navPrev = createComponent('button', {
                type: 'button',
                role: 'button',
                'aria-label': 'Предыдущий слайд',
            });
            needRenderPrev = true;
        }

        self.navPrev = navPrev;

        //var isVertical = self.getIsVerticalOption();

        //navPrev.className = 'carousel-nav-prev ' + (isVertical ? self.options.prevIconVertical : self.options.prevIcon);

        //if (self.options.prevClass) {
        //    self.options.prevClass.split(' ').forEach(function (item) {
        //        navPrev.classList.add(item);
        //    });
        //}

        self.navPrev = navPrev;
        //#endregion

        //#region next find or create
        navNext = nav.querySelector('.carousel-nav-next');

        if (navNext == null) {
            navNext = createComponent('button', {
                type: 'button',
                role: 'button',
                'aria-label': 'Следующий слайд',
            });
            needRenderNext = true;
        }
        self.navNext = navNext;

        self.addDirectionClassFromNav();

        //navNext.className = 'carousel-nav-next ' + (isVertical ? self.options.nextIconVertical : self.options.nextIcon);
        //if (self.options.nextClass) {
        //    self.options.nextClass.split(' ').forEach(function (item) {
        //        navNext.classList.add(item);
        //    });
        //}

        //#endregion

        if (needRenderPrev === true) {
            nav.appendChild(navPrev);
        }

        if (needRenderNext === true) {
            nav.appendChild(navNext);
        }

        if (needRenderNav === true) {
            self.wrap.appendChild(nav);
        }
    };

    Carousel.prototype.removeDirectionClassFromNav = function () {
        var isVertical = this.getIsVerticalOption();
        var self = this;

        self.navNext.className = isVertical ? self.options.nextIconVertical : self.options.nextIcon;

        if (self.options.nextClass) {
            self.options.nextClass.split(' ').forEach(function (item) {
                self.navNext.classList.remove(item);
            });
        }

        self.navPrev.className = isVertical ? self.options.prevIconVertical : self.options.prevIcon;

        if (self.options.prevClass) {
            self.options.prevClass.split(' ').forEach(function (item) {
                self.navPrev.classList.remove(item);
            });
        }
    };

    Carousel.prototype.addDirectionClassFromNav = function () {
        var isVertical = this.getIsVerticalOption();
        var self = this;

        self.navNext.className = 'carousel-nav-next ' + (isVertical ? self.options.nextIconVertical : self.options.nextIcon);

        if (self.options.nextClass) {
            self.options.nextClass.split(' ').forEach(function (item) {
                self.navNext.classList.add(item);
            });
        }

        self.navPrev.className = 'carousel-nav-prev ' + (isVertical ? self.options.prevIconVertical : self.options.prevIcon);

        if (self.options.prevClass) {
            self.options.prevClass.split(' ').forEach(function (item) {
                self.navPrev.classList.add(item);
            });
        }
    };

    Carousel.prototype.generate = function (element) {
        var self = this,
            wrap,
            inner,
            needRenderInner,
            needRenderWrap;

        element.classList.add('carousel-list');

        if (self.options.itemActiveClass != null && self.options.itemActiveClass.length > 0) {
            self.options.itemActiveClass.split(' ').forEach(function (classNameValue) {
                self.items[self.options.indexActive].classList.add(classNameValue);
            });
        }

        if (self.options.itemSelectClass != null && self.options.itemSelectClass.length > 0) {
            self.options.itemSelectClass.split(' ').forEach(function (classNameValue) {
                self.items[self.options.indexActive].classList.add(classNameValue);
            });
        }

        //#region inner find or create
        if (self.list.parentNode != null && self.list.parentNode.classList.contains('carousel-inner') === true) {
            inner = self.list.parentNode;
        } else {
            inner = createComponent('div');
            needRenderInner = true;
        }

        inner.classList.add('carousel-inner');

        self.inner = inner;
        //#endregion

        //#region wrap find or create
        if (self.inner.parentNode != null && self.inner.parentNode.classList.contains('carousel') === true) {
            wrap = self.inner.parentNode;
        } else {
            wrap = createComponent('div');
            needRenderWrap = true;
        }

        var isVertical = self.getIsVerticalOption();

        wrap.classList.add('carousel');
        wrap.classList.add('carousel-' + (isVertical ? 'vertical' : 'horizontal'));
        wrap.classList.add('carousel-wrap-nav-' + self.options.navPosition);

        if (self.options.carouselClass != null && self.options.carouselClass.length > 0) {
            self.options.carouselClass
                .split(' ')
                .filter((item) => item.length > 0)
                .forEach(function (item) {
                    wrap.classList.add(item);
                });
        }

        if (self.options.scrollNav === true) {
            wrap.classList.add('carousel-scroll-nav');
        }

        self.wrap = wrap;
        //#endregion

        //TODO подумать, можно ли оптимизировать рендеринг
        if (needRenderInner) {
            wrap.appendChild(inner);
        }

        if (needRenderWrap) {
            //element.parentNode.appendChild(wrap);
            element.insertAdjacentElement('beforebegin', wrap);
        }

        if (needRenderInner) {
            inner.appendChild(element);
        }
    };

    Carousel.prototype.selectDots = function (index) {
        var self = this;

        if (self.dots == null || self.dotActive === self.dots[index]) {
            return;
        }

        if (self.dotActive != null) {
            self.dotActive.classList.remove('carousel-dots-selected');

            if (self.options.dotsItemSelectedClass != null && self.options.dotsItemSelectedClass.length > 0) {
                self.options.dotsItemSelectedClass.split(' ').forEach(function (classNameValue) {
                    self.dotActive.classList.remove(classNameValue);
                });
            }

            if (self.options.dotsItemInnerSelectedClass != null && self.options.dotsItemInnerSelectedClass.length > 0) {
                self.options.dotsItemInnerSelectedClass.split(' ').forEach(function (classNameValue) {
                    self.dotActive.children[0].classList.remove(classNameValue);
                });
            }
        }

        var dotSelectedIndex = self.dots.find((x) => parseFloat(x.dataset.index) + self.options.scrollCount - 1 >= index);

        if (dotSelectedIndex != null) {
            self.dotActive = dotSelectedIndex;
            dotSelectedIndex.classList.add('carousel-dots-selected');

            if (self.options.dotsItemSelectedClass != null && self.options.dotsItemSelectedClass.length > 0) {
                self.options.dotsItemSelectedClass.split(' ').forEach(function (classNameValue) {
                    dotSelectedIndex.classList.add(classNameValue);
                });
            }

            if (self.options.dotsItemInnerSelectedClass != null && self.options.dotsItemInnerSelectedClass.length > 0) {
                self.options.dotsItemInnerSelectedClass.split(' ').forEach(function (classNameValue) {
                    dotSelectedIndex.children[0].classList.add(classNameValue);
                });
            }
        }
    };

    Carousel.prototype.doClone = function () {
        var self = this,
            oldClones,
            itemsDuplicate,
            itemsClonePrev,
            itemsCloneNext,
            fragmentPrev,
            fragmentNext,
            clonePrev,
            cloneNext,
            marginLeftValue;

        var clonesNext = [];
        var clonesPrev = [];

        //#region find and delete old clones

        oldClones = self.list.querySelectorAll('.js-carousel-clone');
        for (var c = oldClones.length - 1; c >= 0; c--) {
            oldClones[c].parentNode.removeChild(oldClones[c]);
        }

        for (var i = self.items.length - 1; i >= 0; i--) {
            delete self.items[i].carouselItemData.clone;
        }

        self.list.style.marginLeft = '0px';

        //#endregion

        if (self.countVisible >= self.items.length) {
            return null;
        }

        itemsDuplicate = self.items.slice();

        itemsClonePrev = Array.prototype.slice.call(itemsDuplicate.reverse(), 0, self.countVisible).reverse();
        itemsCloneNext = Array.prototype.slice.call(itemsDuplicate.reverse(), 0, self.countVisible);

        fragmentPrev = document.createDocumentFragment();
        fragmentNext = document.createDocumentFragment();

        for (var p = 0, len = itemsClonePrev.length; p < len; p++) {
            clonePrev = (itemsClonePrev[p].carouselItemData.originalClone || itemsClonePrev[p]).cloneNode(true);
            clonePrev.classList.add('js-carousel-clone');

            self.setItemSize(clonePrev, self.slidesSize[self.propName]);

            fragmentPrev.appendChild(clonePrev);
            clonesPrev.push(clonePrev);

            itemsClonePrev[p].carouselItemData.clone = clonePrev;
        }

        for (var n = 0, l = itemsCloneNext.length; n < l; n++) {
            cloneNext = (itemsCloneNext[n].carouselItemData.originalClone || itemsCloneNext[n]).cloneNode(true);
            cloneNext.classList.add('js-carousel-clone');

            self.setItemSize(cloneNext, self.slidesSize[self.propName]);

            fragmentNext.appendChild(cloneNext);
            clonesNext.push(cloneNext);

            itemsCloneNext[n].carouselItemData.clone = cloneNext;
        }

        //insert for prev
        self.list.insertBefore(fragmentPrev, self.items[0]);

        //insert for next
        self.list.appendChild(fragmentNext);

        marginLeftValue = -itemsClonePrev.length * self.slidesSize[self.propName];

        self.list.style.marginLeft = marginLeftValue + 'px';

        self.hasClones = true;

        self.countClone = itemsClonePrev.length + itemsCloneNext.length;

        self.clonesInOneDirection = (itemsClonePrev.length + itemsCloneNext.length) / 2;

        var result = {
            clonesNext: clonesNext,
            clonesPrev: clonesPrev,
            clonesNextCount: itemsCloneNext.length,
            clonesPrevCount: itemsClonePrev.length,
            marginLeftValue: marginLeftValue,
        };

        if (self.options.onDoClone != null) {
            self.options.onDoClone(result);
        }

        return result;
    };

    Carousel.prototype.getMoveData = function (index) {
        var self = this,
            result;

        if (self.items.length > self.countVisible) {
            result = Math.abs(index) * self.slidesSize[self.propName] * (index < 0 ? 1 : -1);
        } else {
            result = 0;
        }

        return result;
    };

    Carousel.prototype.move = function (transformValue, useAnimate) {
        useAnimate = useAnimate != null ? useAnimate : true;

        var self = this,
            transformObj = {},
            transformStyle;

        var isVertical = self.getIsVerticalOption();

        transformObj[isVertical ? 'top' : 'left'] = transformValue;

        if (self.options.scrollNav === false) {
            //elStyle.webkitTransitionDuration = duration;
            //elStyle.transitionDuration = duration;

            self.list.style[transitionDurationName] = useAnimate === false ? '0ms' : self.options.speed / 1000 + 's';
            self.list.style[webkitTransitionDuration] = useAnimate === false ? '0ms' : self.options.speed / 1000 + 's';
            transformStyle = ['translate3d(', transformObj.left || 0, 'px,', ' ', transformObj.top || 0, 'px, 0px)'].join('');
            self.list.style[transformName] = transformStyle;
        } else {
            var scrollValue = Math.floor(isVertical ? self.inner.scrollTop : self.inner.scrollLeft);
            var scrollValueEnd = Math.floor(Math.abs((isVertical ? transformObj.top : transformObj.left) || 0));

            smoothScroll(self.inner, scrollValue, scrollValueEnd, isVertical);
        }

        self.transformValue = transformValue;
    };

    Carousel.prototype.moveAuto = function () {
        var self = this;

        if (autoStop === true) {
            return;
        }

        clearTimeout(self.timerAuto);

        self.timerAuto = setTimeout(function () {
            if (autoStop === true) {
                return;
            }

            self.next();

            self.moveAuto();
        }, self.options.autoPause);
    };

    Carousel.prototype.stopAuto = function () {
        autoStop = true;

        if (self.timerAuto != null) {
            clearTimeout(self.timerAuto);
        }
    };

    Carousel.prototype.startAuto = function () {
        var self = this;

        autoStop = false;

        self.moveAuto();
    };

    Carousel.prototype.checkNav = function () {
        var self = this,
            itemsCount = self.items.length;

        self.isPrevDisabled = (self.options.auto === false && 0 === self.options.indexActive) || self.countVisible >= itemsCount;
        self.isNextDisabled =
            (self.options.auto === false && self.options.indexActive + self.countVisible === self.items.length) || self.countVisible >= itemsCount;
        self.isNavNotShow = itemsCount <= self.countVisible;

        self.isPrevDisabled ? self.navPrev.setAttribute('disabled', 'disabled') : self.navPrev.removeAttribute('disabled');
        self.isNextDisabled ? self.navNext.setAttribute('disabled', 'disabled') : self.navNext.removeAttribute('disabled');

        self.wrap.classList[self.isNavNotShow === true ? 'add' : 'remove']('carousel-nav-not-show');
    };

    Carousel.prototype.prev = function () {
        var self = this,
            newIndex;
        var carouselAsNavFor = self.getCarouselAsNav();

        if (self.isPrevDisabled === true || self.animationLoop === true) {
            return;
        }

        newIndex = self.options.indexActive - self.options.scrollCount;

        //go to last item
        if (self.options.auto === true && newIndex < 0) {
            self.animationLoop = true;

            var returnFn = function () {
                self.list.removeEventListener('transitionend', returnFn);

                setTimeout(function () {
                    self.animationLoop = false;
                    self.goto(self.items.length - 1, false);

                    if (carouselAsNavFor != null) {
                        carouselAsNavFor.goto(self.items.length - 1, false);
                    }
                }, 0);
            };

            self.list.addEventListener('transitionend', returnFn);
        }

        self.goto(newIndex, true, 'backwards');

        if (carouselAsNavFor != null) {
            carouselAsNavFor.goto(newIndex, true, 'backwards');
        }
    };

    Carousel.prototype.next = function () {
        var self = this,
            newIndex;
        var carouselAsNavFor = self.getCarouselAsNav();

        if (self.isNextDisabled === true || self.animationLoop === true) {
            return;
        }

        newIndex = self.options.indexActive + self.options.scrollCount;
        //go to first item

        if (self.options.auto === true && newIndex > self.items.length) {
            self.goto(0, false);
            newIndex = self.options.scrollCount;
            setTimeout(function () {
                self.goto(newIndex, true, 'forward');
            }, 0);
            return;
        }

        if (self.options.auto === true && newIndex === self.items.length) {
            // newIndex > self.items.length - self.countVisible

            self.animationLoop = true;

            var returnFn = function () {
                self.list.removeEventListener('transitionend', returnFn);
                setTimeout(function () {
                    self.animationLoop = false;
                    self.goto(0, false);

                    if (carouselAsNavFor != null) {
                        carouselAsNavFor.goto(0, false);
                    }
                }, 0);
            };

            self.list.addEventListener('transitionend', returnFn);
        }

        self.goto(newIndex, true, 'forward');

        if (carouselAsNavFor != null) {
            carouselAsNavFor.goto(newIndex, true, 'forward');
        }
    };

    Carousel.prototype.loadImg = function (objForLoad, preload) {
        var self = this;
        var list, img;

        if (objForLoad != null) {
            list = Array.prototype.slice.call(objForLoad instanceof NodeList ? objForLoad : [objForLoad]);
            let dataSetKey;
            for (let i = 0, len = list.length; i < len; i++) {
                img = list[i];
                dataSetKey = img.dataset.src != null ? 'src' : img.dataset.srcset ? 'srcset' : null;
                if (img.dataset.carouselImg != null) {
                    if (self.options.onLazyLoad != null) {
                        img.addEventListener('load', function () {
                            this.classList.remove('carousel-placeholder');
                        });
                        self.options.onLazyLoad(img, img);
                    }
                } else if (dataSetKey != null && img.classList.contains('loaded') === false && img.dataset[dataSetKey].indexOf('{{') === -1) {
                    // {{ - выражение ангуляра

                    img.addEventListener('load', function () {
                        this.classList.remove('carousel-placeholder');
                    });

                    img[dataSetKey] = img.dataset[dataSetKey];
                    img.classList.add('loaded');

                    if (preload === true) {
                        let fakeImg = new Image();
                        fakeImg[dataSetKey] = img.dataset[dataSetKey];
                    }
                }
            }
        }
    };

    Carousel.prototype.loadImgInsideItems = function (start, end) {
        var self = this;
        var list = [];

        if (self.options.auto === true) {
            start = start < 0 ? 0 : start;
            list = list.concat(self.cloneResult.clonesPrev.slice());
            list = list.concat(self.items, self.cloneResult != null ? self.cloneResult.clonesNext : []);
        } else {
            list = list.concat(self.items);
        }

        for (var i = start; i < end; i++) {
            self.loadImg(list[i].querySelectorAll('img'));
        }
    };

    Carousel.prototype.goto = function (index, isAnimate, direction) {
        var self = this;
        var carouselAsNavFor = self.getCarouselAsNav();

        if (self.options.itemActiveClass != null && self.options.itemActiveClass.length > 0) {
            self.options.itemActiveClass.split(' ').forEach(function (classNameValue) {
                self.items[self.options.indexActive].classList.remove(classNameValue);
                self.items[index].classList.add(classNameValue);
            });
        }

        if (self.countVisible === 1) {
            if (self.options.itemSelectClass != null && self.options.itemSelectClass.length > 0) {
                self.options.itemSelectClass.split(' ').forEach(function (classNameValue) {
                    self.items[self.options.indexActive].classList.remove(classNameValue);
                    self.items[index].classList.add(classNameValue);
                });
            }

            if (carouselAsNavFor != null) {
                self.callFnCarouselAsNavFor(self.setItemSelect, [index]);
            }
        }

        self.options.indexActive = index;
        var maxIndex;
        if (self.items.length < self.countVisible) {
            maxIndex = 0;
        } else {
            maxIndex = self.items.length - self.countVisible + (self.options.auto === true ? self.countVisible : 0);
        }

        var minIndex = 0 - (self.options.auto === true ? self.countVisible : 0);

        if (self.options.auto === false) {
            if (index < minIndex) {
                index = minIndex;
                self.options.indexActive = minIndex;
            } else if (index > maxIndex) {
                index = maxIndex;
                self.options.indexActive = maxIndex;
            }
        }

        isAnimate = isAnimate != null ? isAnimate : true;

        var transform = self.getMoveData(self.options.indexActive);

        self.move(transform, isAnimate);

        if (self.options.nav === true) {
            self.checkNav();
        }

        if (self.options.dots) {
            var dotsIndex = self.options.indexActive;
            if (self.options.auto === true && self.options.indexActive === self.items.length) {
                dotsIndex = 0;
            } else if (self.options.auto === true && self.options.indexActive < 0) {
                dotsIndex = self.items.length - -self.options.indexActive;
            }
            self.selectDots(dotsIndex);
        }
    };

    Carousel.prototype.removeItem = function (child, keepInCache) {
        var self = this,
            index,
            clone;

        index = self.items.indexOf(child);

        if (index < 0) {
            return;
        }

        keepInCache = keepInCache != null ? keepInCache : true;

        if (child != null && child.parentNode != null) {
            if (self.options.auto === true && child.carouselItemData.clone != null) {
                clone = child.carouselItemData.clone;
                clone.parentNode.removeChild(clone);
            }

            child.parentNode.removeChild(child);
            self.items.splice(index, 1);
        }

        if (keepInCache === false) {
            self.removeFromCache(child);
        }

        //else {
        //    self.addToCache(child);
        //}

        self.checkDots();

        return child;
    };

    Carousel.prototype.addItem = function (item, positonIndex) {
        var self = this,
            index = self.cache.indexOf(item),
            indexSibling = index - 1;

        if (index == -1 || self.items.length === 0 || self.items[indexSibling] == null || self.items[indexSibling].carouselItemData == null) {
            indexSibling = null;
        }

        if (indexSibling == null && positonIndex == null) {
            self.items.push(item);
            self.list.insertAdjacentElement('beforeend', item);
        } else {
            self.items.splice(positonIndex != null ? positonIndex : indexSibling + 1, 0, item);
            self.items[positonIndex != null ? positonIndex - 1 : indexSibling].insertAdjacentElement('afterend', item);
        }

        if (item.carouselItemData == null) {
            self.processItem(item);
        }

        return item;
    };

    Carousel.prototype.updateItems = function (newItems, keepInCache) {
        var self = this;
        var insertContent = document.createDocumentFragment();

        self.items.length = 0;
        keepInCache = keepInCache != null ? keepInCache : true;

        if (keepInCache === false) {
            self.clearCache();
        }

        for (var i = 0, len = newItems.length; i < len; i++) {
            insertContent.appendChild(newItems[i]);

            if (i < self.countVisible) {
                self.loadImg(newItems[i].querySelectorAll('img'), true);
            }
        }

        self.list.innerHTML = '';
        self.list.appendChild(insertContent);

        self.processItems(newItems, true);

        return newItems;
    };

    Carousel.prototype.getItems = function () {
        return this.items;
    };

    Carousel.prototype.filterItems = function (filterFunction) {
        var self = this,
            arrayAll = self.cache,
            itemsForVisible;

        var carouselAsNavFor = self.getCarouselAsNav();

        if (self.options.filterFn) {
            filterFunction = self.options.filterFn;
        }

        itemsForVisible = arrayAll.filter(filterFunction);

        for (var i = 0, len = arrayAll.length - 1; i <= len; i++) {
            if (self.observer != null) {
                if (arrayAll[i] != null) {
                    var img = arrayAll[i].querySelector('img');
                    if (img != null) {
                        img.classList.remove('loaded');
                        self.observer.unobserve(img);
                    }
                }
            }
        }

        self.items = self.updateItems(itemsForVisible, true);

        for (var j = 0; itemsForVisible.length > j; j++) {
            if (self.observer != null) {
                if (itemsForVisible[j] != null) {
                    const img = itemsForVisible[j].querySelector('img');
                    if (img != null) {
                        self.observer.observe(img);
                    }
                }
            }
        }

        self.options.indexActive = 0;

        self.update();

        if (carouselAsNavFor != null) {
            self.callFnCarouselAsNavFor(self.filterItems, [filterFunction]);
        }

        return self.items;
    };

    Carousel.prototype.clearFilterItems = function () {
        var self = this;

        self.filterItems(function () {
            return true;
        });
    };

    Carousel.prototype.getActiveItem = function () {
        return this.items[this.options.indexActive];
    };

    Carousel.prototype.getSelectedItem = function () {
        return this.itemSelected;
    };

    Carousel.prototype.setItemSelect = function (item) {
        var self = this;
        var itemIndex;
        var carouselAsNavFor = self.getCarouselAsNav();

        if (item == null) {
            return;
        }

        self.itemSelected = null;

        if (typeof item === 'number') {
            itemIndex = item;
            item = self.items[item];

            if (item == null) {
                return;
            }
        } else {
            itemIndex = self.items.indexOf(item);
        }

        for (var j = self.items.length - 1; j >= 0; j--) {
            if (self.options.itemSelectClass != null) {
                self.options.itemSelectClass.split(' ').forEach(function (classNameValue) {
                    self.items[j].classList.remove(classNameValue);
                });
            }

            if (self.items[j].carouselItemData != null) {
                self.items[j].carouselItemData.isSelect = false;
            }
        }

        if (self.options.itemSelectClass != null) {
            self.options.itemSelectClass.split(' ').forEach(function (cssClass) {
                item.classList.add(cssClass);

                if (self.options.auto === true && item.carouselItemData != null && item.carouselItemData.clone != null) {
                    item.carouselItemData.clone.classList.add(cssClass);
                }
            });
        }

        if (item.carouselItemData != null) {
            item.carouselItemData.isSelect = true;
            self.itemSelected = item;
        }

        if (carouselAsNavFor != null) {
            self.callFnCarouselAsNavFor(self.setItemSelect, [itemIndex]);
        }
    };

    Carousel.prototype.dotClick = function (event) {
        var self = this,
            currentDot,
            index;

        if (event.target.tagName.toLowerCase() === 'i') {
            currentDot = event.target.parentNode;
        } else if (event.target.tagName.toLowerCase() === 'li') {
            currentDot = event.target;
        } else {
            return;
        }

        index = parseInt(currentDot.getAttribute('data-index'));

        self.goto(index === 0 ? 0 : index + self.options.scrollCount - 1);
    };

    Carousel.prototype.itemClick = function (item) {
        var self = this;
        var itemIndex;
        var itemObj;
        var carouselAsNavFor = self.getCarouselAsNav();

        if (typeof item === 'number') {
            itemIndex = item;
            itemObj = self.items[itemIndex];
        } else {
            itemIndex = self.items.indexOf(item);
            itemObj = item;
        }

        self.setItemSelect(itemObj);

        if (self.options.itemSelect != null) {
            self.options.itemSelect(self, itemObj, itemIndex);
        }

        if (carouselAsNavFor != null) {
            if (carouselAsNavFor.isVisibleItem(itemIndex) === false) {
                carouselAsNavFor.goto(itemIndex, true);
            }

            self.callFnCarouselAsNavFor(self.itemClick, [itemIndex]);
        }
    };

    Carousel.prototype.touch = function () {
        var self = this;
        var startCoords, movedCoords;
        function touchStart(event) {
            event.stopPropagation();
            startCoords = self.getCoordinates(event);
            movedCoords = startCoords;

            if (self.options.auto === true) {
                self.stopAuto();
            }

            self.list.addEventListener('touchmove', touchMove, { passive: true });

            self.list.addEventListener('touchend', touchEnd, { passive: true });
        }

        function touchStartScroll() {
            var scrollEvent = debounce(function () {
                self.inner.removeEventListener('scroll', scrollEvent);
                self.inner.removeEventListener('touchend', scrollEvent);
                var isVertical = self.getIsVerticalOption();
                var newIndex = Math.ceil(self.inner.scrollLeft / self.itemsSize[self.getPropName(isVertical)]);
                self.goto(newIndex, true);
            }, 700);

            self.inner.addEventListener('scroll', scrollEvent, { passive: true });
            self.inner.addEventListener('touchend', scrollEvent, { passive: true });
        }

        function touchMove(event) {
            var validSwipe;
            var coords = self.getCoordinates(event);
            var dim = coords.main - movedCoords.main;
            var dimAllTime = movedCoords.main - startCoords.main;

            isOverScrollX = self.listSize.width + Math.abs(dimAllTime) - self.slidesSize.width > self.listSize.width;

            if (self.options.auto === true) {
                self.goToFirstInMobile();
            }

            if (!isScrolling) {
                validSwipe = self.validSwipe(startCoords, coords, dim >= 0 ? 1 : -1);
                if (validSwipe === true) {
                    isScrolling = true;
                } else {
                    isScrolling = false;
                }
            }
            if (isScrolling) {
                event.stopPropagation();
                self.move((self.transformValue || 0) + dim, false);
                movedCoords = coords;
            } else {
                if (self.options.scrollNav === false) {
                    self.list.removeEventListener('touchmove', touchMove);
                }

                self.list.removeEventListener('touchend', touchEnd);

                if (self.options.auto === true) {
                    self.startAuto();
                }
            }
        }

        function touchEnd(event) {
            self.list.removeEventListener('touchmove', touchMove);
            self.list.removeEventListener('touchend', touchEnd);
            var dim = movedCoords.main - startCoords.main;

            if (isScrolling) {
                gotoByTouhMove(dim);
            }

            if (self.options.auto === true) {
                self.startAuto();
            }

            isScrolling = false;
        }

        function gotoByTouhMove(dim) {
            var dimAllTime = movedCoords.main - startCoords.main;
            var maxIndex = self.items.length - self.countVisible + (self.options.auto === true ? self.countVisible - 1 : 0);
            var minIndex = 0 - (self.options.auto === true ? self.countVisible - 1 : 0);
            var isVertical = self.getIsVerticalOption();
            var touchMoveItemsCount = Math.abs(Math.round(dimAllTime / self.slidesSize[self.getPropName(isVertical)])) || 1;
            var index = dimAllTime < 0 ? self.options.indexActive + touchMoveItemsCount : self.options.indexActive - touchMoveItemsCount;
            var carouselAsNavFor = self.getCarouselAsNav();
            var direction = dim >= 0 ? 'forward' : 'backward';

            if (self.options.auto === false && index > maxIndex) {
                index = maxIndex;
            } else if (self.options.auto === false && index < minIndex) {
                index = minIndex;
            }

            if (self.options.auto === true && isOverScrollX && direction === 'backward') {
                index = self.items.length - self.countVisible + self.clonesInOneDirection;
            } else if (self.options.auto === true && isOverScrollX && direction === 'forward') {
                index = 0 - self.countVisible;
            }

            if (carouselAsNavFor != null && carouselAsNavFor.isVisibleItem(index) === false) {
                carouselAsNavFor.goto(index, true);
            }

            self.goto(index, true);

            isOverScrollX = false;
        }

        self.list.addEventListener('touchstart', self.options.scrollNav === false ? touchStart : touchStartScroll, {
            passive: true,
        });
    };

    Carousel.prototype.getCoordinates = function (event) {
        var self = this;
        var originalEvent = event.originalEvent || event;
        var touches = originalEvent.touches && originalEvent.touches.length ? originalEvent.touches : [originalEvent];
        var e = (originalEvent.changedTouches && originalEvent.changedTouches[0]) || touches[0];
        var result;
        var isVertical = self.getIsVerticalOption();
        if (isVertical) {
            result = {
                main: e.clientY,
                alt: e.clientX,
            };
        } else {
            result = {
                main: e.clientX,
                alt: e.clientY,
            };
        }

        return result;
    };

    Carousel.prototype.validSwipe = function (startCoords, coords) {
        var deltaAlt = Math.abs(coords.alt - startCoords.alt);
        var deltaMain = Math.abs(coords.main - startCoords.main);
        var self = this;
        var touchAngle = (Math.atan2(Math.abs(deltaAlt), Math.abs(deltaMain)) * 180) / Math.PI;
        var isVertical = self.getIsVerticalOption();
        if (isVertical === false && touchAngle > 45) {
            return false;
        }
        if (isVertical === false && touchAngle <= 45) {
            return true;
        }
        if (isVertical === true && 90 - touchAngle > 45) {
            return true;
        } else {
            return false;
        }
    };

    Carousel.prototype.bindIt = function () {
        var self = this,
            options = self.options;

        if (isTouchDevice === true) {
            self.touch();

            window.addEventListener('orientationchange', self.update.bind(self));
        } else {
            window.addEventListener('resize', function () {
                self.update();
            });
        }

        if (options.auto === true && isTouchDevice === false) {
            //self.wrap.removeEventListener('mouseenter', self.stopAuto);
            self.wrap.addEventListener('mouseenter', function () {
                self.stopAuto();
            });

            //self.wrap.removeEventListener('mouseleave', self.startAuto);
            self.wrap.addEventListener('mouseleave', function () {
                self.startAuto();
            });
        }

        self.wrap.addEventListener('click', function (event) {
            var itemClicked;

            if (options.nav === true) {
                if (event.target === self.navNext) {
                    self.next();
                    return;
                } else if (event.target === self.navPrev) {
                    self.prev();
                    return;
                }
            }

            if (options.dots === true && closest(event.target, self.dotsContainer) !== null) {
                self.dotClick(event);
                return;
            }

            itemClicked = closest(event.target, '.js-carousel-item');

            if (itemClicked !== null) {
                self.itemClick(itemClicked);
            }
        });

        if (self.options.responsive != null) {
            Object.keys(self.options.responsive).forEach(function (mqRule) {
                var mq = getMediaQuery(mqRule);

                mq.addListener(function (event) {
                    if (event.matches === true) {
                        self.update();
                    }
                });
            });
        }
        if (self.options.auto) {
            document.addEventListener('visibilitychange', function () {
                if (document.visibilityState === 'visible') {
                    self.startAuto();
                } else {
                    self.stopAuto();
                }
            });
        }
    };

    Carousel.prototype.init = function () {
        var self = this,
            sizes;

        //self.cache.length = 0;

        self.processItems(self.items, true);

        self.generate(self.list);

        self.sizes = self.calc(self.items, self.options, self.options.responsive != null ? self.responsiveOption : null); //self.wrap, self.inner, self.list,

        self.setSizes(self.sizes.wrapSize, self.sizes.innerSize, self.sizes.listSize, self.sizes.itemsSize);

        self.checkDots();

        if (self.options.nav === true) {
            self.renderNav();
        }

        if (self.options.auto === true && self.countVisible < self.items.length) {
            self.cloneResult = self.doClone();

            self.sizes.listSize[self.propName] += Math.abs(self.cloneResult.marginLeftValue) * 2; //2 - с обеих сторон ширина клонированных слайдов

            self.setSizes(self.sizes.wrapSize, self.sizes.innerSize, self.sizes.listSize, self.sizes.itemsSize);

            self.goto(self.options.indexActive, false);
        }

        if (self.options.nav === true) {
            self.checkNav();
        }

        if (self.options.auto === true) {
            self.startAuto();
        }

        if (self.dots != null) {
            self.selectDots(self.options.indexActive);
        }

        if (self.options.initFn != null) {
            self.options.initFn(self);
        }

        self.bindIt();

        self.setIntersectionObserver();

        self.initilized = true;

        self.wrap.classList.add('carousel-initilized');
        self.addDirectionCarouselClass();

        return self;
    };

    Carousel.prototype.resetSizes = function (callback) {
        var self = this;
        var isVertical = self.getIsVerticalOption();

        self.wrap.style[self.propName] = isVertical ? '100%' : 'auto';
        self.inner.style[self.propName] = isVertical ? '100%' : 'auto';
        self.list.style[self.propName] = isVertical ? '100%' : 'auto';

        self.list.style.marginLeft = '0';

        self.removeDirectionCarouselClass();
        if (self.navPref != null && self.navNext != null) {
            self.removeDirectionClassFromNav();
        }

        var oldClones = self.list.querySelectorAll('.js-carousel-clone');
        for (var c = oldClones.length - 1; c >= 0; c--) {
            clearStyleSlide(oldClones[c], self);
        }

        for (var i = self.items.length - 1; i >= 0; i--) {
            clearStyleSlide(self.items[i], self);
            self.items[i].setAttribute('style', self.items[i].carouselItemData.stylesRaw || '');
        }

        setTimeout(function () {
            callback();
        }, 500);
    };

    function clearStyleSlide(slide, carousel) {
        slide.style[carousel.propName] = 'auto';
        slide.style['flex-basis'] = 'auto';
        slide.style['msFlexPreferredSize'] = 'auto';
        slide.style['webkitFlexBasis'] = 'auto';

        if (self.propName === 'width') {
            slide.style.maxWidth = 'none';
        } else {
            slide.style.maxHeight = 'none';
        }
    }

    Carousel.prototype.update = function () {
        var self = this,
            sizes;

        self.wrap.classList.remove('carousel-nav-not-show');
        self.wrap.classList.add('carousel-update');

        self.resetSizes(function () {
            if (self.list.children != null && self.list.children.length > 0) {
                var childrenWithoutClone = Array.prototype.filter.call(self.list.children, function (child) {
                    return child.classList.contains('js-carousel-clone') === false;
                });
            } else {
                return;
            }
            self.responsiveOption = self.options.responsive != null ? self.checkResponsive() : null;
            var isVertical = self.getIsVerticalOption();
            self.addDirectionCarouselClass();
            if (self.navPref != null && self.navNext != null) {
                self.addDirectionClassFromNav();
            }
            self.propName = self.getPropName(isVertical);

            self.items = Array.prototype.slice.call(childrenWithoutClone);
            self.processItems(self.items);
            sizes = self.calc(self.items, self.options, self.responsiveOption);

            self.setSizes(sizes.wrapSize, sizes.innerSize, sizes.listSize, sizes.itemsSize);

            if (self.options.auto === true) {
                self.cloneResult = self.doClone();

                if (self.cloneResult != null) {
                    sizes.listSize[self.propName] += Math.abs(self.cloneResult.marginLeftValue) * 2; //2 - с обеих сторон ширина клонированных слайдов
                }

                self.setSizes(sizes.wrapSize, sizes.innerSize, sizes.listSize, sizes.itemsSize);

                //self.options.indexActive = 0;
            } else {
                if (self.options.nav === true) {
                    self.checkNav();
                }
            }

            self.goto(self.options.indexActive, false);

            if (self.options.dots === true) {
                self.checkDots();
                self.selectDots(self.options.indexActive);
            }

            self.wrap.classList.remove('carousel-update');

            if (self.options.onUpdate != null) {
                self.options.onUpdate();
            }
        });
    };

    Carousel.prototype.checkResponsive = function () {
        var self = this;
        var mq;
        var mqOptions;
        var mqRules = Object.keys(this.options.responsive);

        for (var i = mqRules.length - 1; i >= 0; i--) {
            mq = getMediaQuery(mqRules[i]);
            mqOptions = self.options.responsive[mqRules[i]];

            if (mq.matches === true) {
                break;
            }
        }

        return mqOptions;
    };

    Carousel.prototype.getCarouselAsNav = function () {
        return storage[this.options.asNavFor] && storage[this.options.asNavFor].obj;
    };

    Carousel.prototype.callFnCarouselAsNavFor = function (fn, params) {
        var self = this;
        if (
            self.options.asNavFor != null &&
            self.options.asNavFor.length > 0 &&
            storage[self.options.asNavFor] &&
            storage[self.options.asNavFor].state.callAsNav !== true
        ) {
            storage[self.options.asNavFor].state.callAsNav = true;
            fn.apply(storage[self.options.asNavFor].obj, params);
            storage[self.options.asNavFor].state.callAsNav = false;
        }
    };

    Carousel.prototype.whenAsNavForReady = function (idAsNavFor, callback) {
        if (storage[idAsNavFor] != null) {
            callback(storage[idAsNavFor]);
        } else {
            deferList[idAsNavFor] = callback;
        }
    };

    Carousel.prototype.resolveAsNavForReady = function (idAsNavFor) {
        if (deferList[idAsNavFor] != null) {
            deferList[idAsNavFor](storage[idAsNavFor]);
        }
    };

    Carousel.prototype.isVisibleItem = function (item) {
        var self = this;
        var itemObj = typeof item === 'number' ? self.items[item] : item;
        var itemIndex = itemObj.carouselItemData.index;
        var isVertical = self.getIsVerticalOption();
        var minIndex =
            (self.options.scrollNav === true ? self.inner.scrollLeft : Math.abs(self.transformValue)) / self.slidesSize[self.getPropName(isVertical)];
        var maxIndex = minIndex + self.countVisible;
        return minIndex < itemIndex && maxIndex > itemIndex;
    };

    Carousel.prototype.getScrollDiff = function (itemSize, countVisible) {
        return this.options.scrollNav === true ? Math.ceil(itemSize / 2 / countVisible) : 0;
    };

    Carousel.prototype.goToFirstInMobile = function () {
        var self = this;
        if (self.options.indexActive >= self.items.length + self.clonesInOneDirection - self.countVisible) {
            self.goto(0, false);
        } else if (self.options.indexActive <= 0 - self.clonesInOneDirection) {
            self.goto(self.items.length - self.countVisible, false);
        }
    };

    Carousel.prototype.setIntersectionObserver = function (objOptions, funcCallback, classElement) {
        var self = this;
        var targetArr = self.inner.querySelectorAll(classElement || 'img');

        if (targetArr != null && targetArr.length > 0) {
            var isVertical = self.getIsVerticalOption();
            var options = objOptions || {
                root: self.inner,
                rootMargin: (isVertical ? self.innerSize.height : self.innerSize.width) + 'px',
                threshold: 0,
            };
            var callback =
                funcCallback ||
                function (entries, observer) {
                    entries.forEach(function (entry) {
                        if (entry.isIntersecting) {
                            var img = entry.target;

                            self.loadImg(img);

                            self.observer.unobserve(img);
                        }
                    });
                };

            if (window.IntersectionObserver) {
                self.observer = new IntersectionObserver(callback, options);

                for (let i = 0; i < targetArr.length; i++) {
                    self.observer.observe(targetArr[i]);
                }
            } else {
                for (let i = 0; i < targetArr.length; i++) {
                    var img = targetArr[i];

                    self.loadImg(img);
                }
            }
        }
    };

    Carousel.prototype.getIsVerticalOption = function () {
        var self = this;
        return self.responsiveOption != null && self.responsiveOption.isVertical != null ? self.responsiveOption.isVertical : self.options.isVertical;
    };

    Carousel.prototype.removeDirectionCarouselClass = function () {
        this.wrap.classList.remove('carousel-vertical');
        this.wrap.classList.remove('carousel-horizontal');
    };

    Carousel.prototype.addDirectionCarouselClass = function () {
        var isVertical = this.getIsVerticalOption();
        this.wrap.classList.add(isVertical ? 'carousel-vertical' : 'carousel-horizontal');
    };

    window.Carousel = Carousel;

    function createComponent(tagName, attrs = {}) {
        if (clonesForCreate[tagName] == null) {
            clonesForCreate[tagName] = document.createElement(tagName);
            for (const [attr, value] of Object.entries(attrs)) {
                clonesForCreate[tagName].setAttribute(attr, value);
            }
        }

        return clonesForCreate[tagName].cloneNode();
    }

    function closest(element, selector) {
        var parent = element,
            matchesSelector;

        if (parent == null) {
            return null;
        }

        matchesSelector = parent.matches || parent.webkitMatchesSelector || parent.mozMatchesSelector || parent.msMatchesSelector;

        while (parent != document.body && parent != document && parent != null) {
            if (typeof selector === 'string') {
                if (matchesSelector.bind(parent)(selector) === true) {
                    return parent;
                }
            } else {
                if (parent == selector) {
                    return parent;
                }
            }

            parent = parent.parentNode;
        }

        return null;
    }

    function getMediaQuery(value) {
        return window.matchMedia('(min-width:' + value + 'px)');
    }

    function smoothScroll(element, scrollValue, scrollEnd, isVertical) {
        var timeLapsed = 0,
            start,
            speed = 700,
            percentage,
            position;
        var distance = scrollEnd - scrollValue;

        function go(timestamp) {
            var finish = true;

            if (!start) {
                start = timestamp;
            }

            timeLapsed += timestamp - start;
            percentage = speed === 0 ? 0 : timeLapsed / speed;
            percentage = percentage > 1 ? 1 : percentage;
            position = Math.floor(scrollValue + distance * animateValue(percentage));

            if (position != scrollEnd) {
                finish = false;
                start = timestamp;
            }

            element.scrollTo(!isVertical && position, isVertical && position);

            if (finish === false) {
                window.requestAnimationFrame(go, element);
            }
        }

        window.requestAnimationFrame(go, element);
    }

    function debounce(func, ms) {
        var timer;

        return function () {
            if (timer != null) {
                clearTimeout(timer);
            }

            var vm = this;
            var args = arguments;

            timer = setTimeout(function () {
                func.apply(vm, args);
            }, ms);
        };
    }

    //easeInOutCubic
    function animateValue(time) {
        return time < 0.5 ? 4 * time * time * time : (time - 1) * (2 * time - 2) * (2 * time - 2) + 1;
    }
})(window);
