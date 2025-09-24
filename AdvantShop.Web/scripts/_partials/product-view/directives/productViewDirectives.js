import photosTemplate from '../templates/photos.html';

/* @ngInject */
function productViewItemDirective(productViewService, domService, windowService, $parse) {
    return {
        restrict: 'A',
        require: {
            productViewMode: '?^productViewMode',
            productViewItem: 'productViewItem',
        },
        controller: 'ProductViewItemCtrl',
        controllerAs: 'productViewItem',
        bindToController: true,
        scope: true,
        link: function (scope, element, attrs, ctrls) {
            var productViewItemCtrl = ctrls.productViewItem,
                timerHover;

            if (attrs.offer != null) {
                productViewItemCtrl.offer = $parse(attrs.offer)(scope);
            }

            if (attrs.onChangeColor != null) {
                productViewItemCtrl.onChangeColor = attrs.onChangeColor;
            }

            if (attrs.maxPhotoView != null) {
                productViewItemCtrl.maxPhotoView = parseFloat(attrs.maxPhotoView);
            }

            if (attrs.onlyPhotoWithColor != null) {
                productViewItemCtrl.onlyPhotoWithColor = attrs.onlyPhotoWithColor === 'true';
            }

            productViewItemCtrl.offerId = attrs.offerId != null && attrs.offerId.length > 0 ? $parse(attrs.offerId)(scope) : null;

            //productViewItemCtrl.getOffersProduct(productViewItemCtrl.productId);

            productViewService.addCallback('setView', function (viewMode) {
                productViewItemCtrl.viewName = viewMode.viewName;

                setTimeout(function () {
                    var colorsViewerCarousel = productViewItemCtrl.getControl('colorsViewerCarousel');

                    if (colorsViewerCarousel != null) {
                        colorsViewerCarousel.update();
                    }

                    scope.$digest();
                }, 50);
            });

            element[0].addEventListener('mouseenter', function () {
                if (timerHover != null) {
                    clearTimeout(timerHover);
                }

                timerHover = setTimeout(async function () {
                    await productViewItemCtrl.enter();
                    productViewItemCtrl.isLoad = true;
                    scope.$digest();
                }, 100);
            });

            element[0].addEventListener('mouseleave', function () {
                clearTimeout(timerHover);
                productViewItemCtrl.leave();
                scope.$digest();
            });

            element[0].addEventListener(
                'touchstart',
                async function () {
                    //await productViewItemCtrl.enter();
                    productViewItemCtrl.isLoad = true;
                    scope.$digest();
                },
                { passive: true },
            );

            //windowService.addCallback('touchstart', function (eventObj) {
            //    var isClickedMe = domService.closest(eventObj.event.target, element[0]) != null;

            //    if (isClickedMe === false) {
            //        productViewItemCtrl.leave();

            //        scope.$digest();
            //    }
            //});
        },
    };
}

function productViewCarouselPhotosDirective() {
    return {
        require: ['^productViewCarouselPhotos', '^productViewItem'],
        restrict: 'A',
        scope: {
            photoHeight: '@',
            photoWidth: '@',
            changePhoto: '&',
        },
        replace: true,
        templateUrl: photosTemplate,
        controller: 'ProductViewCarouselPhotosCtrl',
        controllerAs: 'photosCarousel',
        bindToController: true,
        link: function (scope, element, attrs, ctrl) {
            var carouselPhotosCtrl = ctrl[0],
                productViewItemCtrl = ctrl[1];

            carouselPhotosCtrl.parentScope = productViewItemCtrl;

            productViewItemCtrl.addControl('photosCarousel', carouselPhotosCtrl);
        },
    };
}

/* @ngInject */
function productViewChangeModeDirective(productViewService, viewList) {
    return {
        restrict: 'A',
        scope: true,
        controller: 'ProductViewChangeModeCtrl',
        controllerAs: 'changeMode',
        bindToController: true,
        link: function (scope, element, attrs, ctrl) {
            ctrl.name = attrs.name;
            ctrl.currentViewList = viewList[attrs.viewListName || 'desktop'];
            ctrl.isMobile = attrs.isMobile === 'true';
            ctrl.isReadyViewMode = false;
            ctrl.defaultViewMode = attrs.defaultViewMode;

            if (ctrl.isMobile === true) {
                ctrl.current = productViewService.getViewFromCookie('mobile_viewmode', ctrl.currentViewList, ctrl.defaultViewMode);
            } else {
                ctrl.current = attrs.viewMode;
            }

            ctrl.isReadyViewMode = true;
        },
    };
}

/* @ngInject */
function productViewModeDirective() {
    return {
        restrict: 'A',
        scope: true,
        require: {
            productViewMode: 'productViewMode',
        },
        controller: 'ProductViewModeCtrl',
        controllerAs: 'productViewMode',
        bindToController: true,
    };
}

/* @ngInject */
function productViewImageListDirective($q) {
    return {
        restrict: 'A',
        scope: true,
        require: {
            productViewMode: '?^productViewMode',
            productViewImageList: 'productViewImageList',
            productViewItem: '^^productViewItem',
        },
        controller: 'ProductViewImageListCtrl',
        controllerAs: 'productViewImageList',
        bindToController: true,
        link: function (scope, element, attrs, ctrls) {
            var productViewImageList = ctrls.productViewImageList,
                timerHover;

            element[0].addEventListener('mouseenter', function () {
                if (timerHover != null) {
                    clearTimeout(timerHover);
                }

                timerHover = setTimeout(function () {
                    $q.all(productViewImageList.insertHtml(), productViewImageList.getPhotos());
                }, 100);
            });

            element[0].addEventListener('mouseleave', function () {
                clearTimeout(timerHover);
            });

            element[0].addEventListener(
                'touchstart',
                function () {
                    $q.all(productViewImageList.insertHtml(), productViewImageList.getPhotos());
                },
                { passive: true },
            );
        },
    };
}

/* @ngInject */
function productViewImageDirective(productViewService) {
    return {
        restrict: 'A',
        scope: true,
        require: {
            productViewMode: '?^^productViewMode',
            productViewImageList: '?^productViewImageList',
            productViewImage: 'productViewImage',
        },
        // require: ['?^^productViewMode',
        //     '^^productViewImageList',
        //     'productViewImage'],
        controller: 'ProductViewImageCtrl',
        controllerAs: 'productViewImage',
        bindToController: true,
    };
}

function productViewScrollPhotosDirective() {
    return {
        restrict: 'A',
        scope: true,
        require: {
            productViewItem: '^productViewItem',
        },
        controller: [
            '$element',
            function ($element) {
                const ctrl = this;

                ctrl.$onInit = function () {
                    ctrl.productViewItem.addControl('productViewScrollPhotos', ctrl);
                };

                ctrl.scrollToStart = function () {
                    $element[0].scrollTo(0, 0);
                };
            },
        ],
        controllerAs: 'productViewScrollPhotos',
        bindToController: true,
    };
}

export {
    productViewItemDirective,
    productViewCarouselPhotosDirective,
    productViewChangeModeDirective,
    productViewModeDirective,
    productViewScrollPhotosDirective,
    productViewImageListDirective,
    productViewImageDirective,
};
