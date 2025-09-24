import { productViewItemControls } from '../productViewItem.controls.js';

/*@ngInject*/
function ProductViewItemCtrl($document, $q, $timeout, productViewService, $translate, $scope, $parse, urlHelper, $attrs) {
    var ctrl = this,
        controls = {},
        needCarouselUpdate = false,
        requestGetPhotosInPending = false,
        isPhotosStorageMutation = false,
        photosStorage;
    var isMobile = $document[0].documentElement.classList.contains('mobile-version');
    const styles = new Map();

    ctrl.$onInit = function () {
        ctrl.photosVisible = false;
        ctrl.photos = [];
        ctrl.picture = {};
        ctrl.promisesGetPhotos = [];
        ctrl.colorSelected = null;
        ctrl.productId = $parse($attrs.productId)($scope);
        ctrl.offer = $parse($attrs.offer)($scope);
    };

    ctrl.getOffersProduct = function (productId) {
        return productViewService.getOfferId(productId).then(function (result) {
            if (result != null) {
                return result.Offers;
            }
        });
    };

    ctrl.getPhotos = function (productId) {
        var defer = $q.defer(),
            promise;
        if (requestGetPhotosInPending === false && (photosStorage == null || needCarouselUpdate === true)) {
            requestGetPhotosInPending = true;
            promise = productViewService.getPhotos(ctrl.productId || productId).then(function (photos) {
                isPhotosStorageMutation = true;

                for (var i = 0, len = ctrl.promisesGetPhotos.length; i < len; i++) {
                    ctrl.promisesGetPhotos[i].resolve(photos);
                }

                ctrl.promisesGetPhotos.length = 0;

                requestGetPhotosInPending = false;

                return (photosStorage = photos);
            });
        } else if (requestGetPhotosInPending === true) {
            promise = defer.promise;
            ctrl.promisesGetPhotos.push(defer);
        } else {
            promise = defer.promise;
            defer.resolve(photosStorage);
        }
        ctrl.gotPhotos = true;
        return promise;
    };

    ctrl.numberals = function (num) {
        if (num <= 0) return (ctrl.textNumberals = num + ' ' + $translate.instant('Js.ProductView.Photos0'));
        num = num % 100;
        var nums = num % 10;
        if (num > 10 && num < 20) return (ctrl.textNumberals = num + ' ' + $translate.instant('Js.ProductView.Photos5'));
        if (nums > 1 && nums < 5) return (ctrl.textNumberals = num + ' ' + $translate.instant('Js.ProductView.Photos2'));
        return nums === 1
            ? (ctrl.textNumberals = num + ' ' + $translate.instant('Js.ProductView.Photos1'))
            : (ctrl.textNumberals = num + ' ' + $translate.instant('Js.ProductView.Photos5'));
    };

    ctrl.fill = function (photos) {
        if (ctrl.getControl('colorsViewer') != null) {
            ctrl.photos = ctrl.filterPhotos(
                photosStorage == null && isPhotosStorageMutation === false ? ctrl.getPhotos() : photos,
                ctrl.getControl('colorsViewer').colorSelected.ColorId,
                ctrl.onlyPhotoWithColor,
            );
        } else {
            ctrl.photos = photos;
        }

        if (ctrl.photos.length === 0) {
            ctrl.photos.length = 0;
            ctrl.photos.push.apply(ctrl.photos, ctrl.getMainPhoto(photos));
        }
        if (ctrl.maxPhotoView != null) {
            ctrl.photos = ctrl.photos.slice(0, ctrl.maxPhotoView);
        }

        ctrl.numberals(ctrl.photos.length);
        return photos;
    };

    ctrl.process = function (productId) {
        return ctrl.getPhotos(productId).then(function (photos) {
            ctrl.fill(photos);

            $timeout(function () {
                ctrl.carouselInit = true;

                if (needCarouselUpdate === true && ctrl.getControl('photosCarousel') != null && ctrl.getControl('photosCarousel').carousel != null) {
                    ctrl.getControl('photosCarousel').carousel.update();
                    needCarouselUpdate = false;
                }
            }, 0);

            return photos;
        });
    };

    ctrl.clearPhotos = function () {
        photosStorage = null;
        needCarouselUpdate = true;
    };

    ctrl.enter = function () {
        if (ctrl.photosVisible === true) {
            return;
        }

        ctrl.photosVisible = true;
        return ctrl.process(ctrl.productId);
    };
    ctrl.leave = function () {
        ctrl.photosVisible = false;
        ctrl.carouselInit = false;
    };

    ctrl.changePhoto = function (photo) {
        ctrl.picture = photo;
    };

    ctrl.initColors = function (colorsViewer) {
        ctrl.addControl('colorsViewer', colorsViewer);

        if (colorsViewer.changeStartSelectedColor != null) {
            setTimeout(function () {
                colorsViewer.selectColorById(colorsViewer.changeStartSelectedColor);
            }, 500);
        }
    };

    ctrl.getSelectedColorId = function () {
        var colorsViewer = ctrl.getControl('colorsViewer'),
            colorId;

        if (colorsViewer != null && colorsViewer.colorSelected != null && colorsViewer.getDirtyState() === true) {
            colorId = colorsViewer.colorSelected.ColorId;
        }

        return colorId;
    };

    ctrl.scrollToStartImages = function () {
        let photosScrollContent = ctrl.getControl('productViewScrollPhotos');

        if (photosScrollContent != null) {
            return $timeout(() => photosScrollContent.scrollToStart(), 100);
        } else {
            return $q.resolve();
        }
    };

    ctrl.initColorsCarousel = function (carousel) {
        ctrl.addControl('colorsViewerCarousel', carousel);
    };

    ctrl.changeColor = function (color) {
        ctrl.setStateLoading(true);
        ctrl.getOffersProduct(ctrl.productId)
            .then(function (result) {
                ctrl.productOffers = result;
                if (ctrl.productOffers != null && ctrl.productOffers.length > 0) {
                    ctrl.selectedProductOffer = ctrl.productOffers.filter(function (offer) {
                        return offer.Color.ColorId === color.ColorId;
                    });
                    if (ctrl.selectedProductOffer != null && ctrl.selectedProductOffer.length > 0) {
                        var o = null;
                        for (var i = 0; i < ctrl.selectedProductOffer.length; i++) {
                            if (ctrl.selectedProductOffer[i].Amount > 0) {
                                o = ctrl.selectedProductOffer[i];
                                break;
                            }
                        }

                        ctrl.offer = o == null ? ctrl.selectedProductOffer[0] : o;
                    }
                }
                var defaultPhoto;
                if (photosStorage == null && isPhotosStorageMutation === false) {
                    ctrl.getPhotos().then(function (result) {
                        ctrl.photos = ctrl.filterPhotos(
                            result,
                            color.ColorId,
                            ctrl.getControl('colorsViewer') != null ? ctrl.onlyPhotoWithColor : false,
                        );
                        defaultPhoto = ctrl.getMainPhoto(ctrl.photos);
                        //if (ctrl.photos.length === 0) {
                        //    ctrl.photos = defaultPhoto;
                        //}
                        //if (ctrl.maxPhotoView != null) {
                        //    ctrl.photos = ctrl.photos.slice(0, ctrl.maxPhotoView);
                        //}
                        ctrl.setColor(defaultPhoto);
                    });
                } else {
                    defaultPhoto = ctrl.getMainPhoto(photosStorage != null && photosStorage.length !== 0 ? photosStorage : ctrl.photos);
                    ctrl.photos = ctrl.filterPhotos(
                        photosStorage,
                        color.ColorId,
                        ctrl.getControl('colorsViewer') != null ? ctrl.onlyPhotoWithColor : false,
                    );

                    ctrl.setColor(defaultPhoto);
                }

                ctrl.getControl(productViewItemControls.productListCtrl)?.filterPhotos({
                    colorId: color.ColorId,
                    onlyColorPhoto: ctrl.getControl('colorsViewer') != null ? ctrl.onlyPhotoWithColor : false,
                });

                if (ctrl.onChangeColor != null) {
                    $parse(ctrl.onChangeColor)($scope);
                }

                if (ctrl.photos.length === 0) {
                    ctrl.photos.length = 0;
                    ctrl.photos.push.apply(ctrl.photos, defaultPhoto);
                    //ctrl.photos = ctrl.photos.concat(defaultPhoto);
                }

                if (ctrl.maxPhotoView != null) {
                    ctrl.photos = ctrl.photos.slice(0, ctrl.maxPhotoView);
                }

                return ctrl.scrollToStartImages();
            })
            .catch(function (error) {
                console.error(error);
            })
            .finally(() => ctrl.setStateLoading(false));
    };

    ctrl.setColor = function (defaultPhoto) {
        ctrl.picture = ctrl.photos.length === 0 && defaultPhoto != null ? defaultPhoto[0] : ctrl.photos[0];

        ctrl.numberals(ctrl.photos.length);
        var photosCarousel = ctrl.getControl('photosCarousel');
        if (photosCarousel != null && photosCarousel.carousel != null) {
            $timeout(function () {
                photosCarousel.carousel.options.indexActive = 0;
                photosCarousel.carousel.update();
            });
        }

        if (ctrl.photoViewer != null) {
            ctrl.photoViewer.reinit();
        }
    };

    ctrl.addControl = function (name, scope) {
        controls[name] = scope;
    };

    ctrl.getControl = function (name) {
        return controls[name];
    };

    ctrl.filterPhotos = function (photos, colorId, onlyColorPhoto) {
        return productViewService.filterPhotos(photos, colorId, onlyColorPhoto);
    };

    ctrl.getUrl = function (url) {
        var result = url,
            colorId = ctrl.getSelectedColorId();

        if (colorId != null) {
            result = urlHelper.updateQueryStringParameter(result, 'color', colorId);
        }

        return result;
    };

    ctrl.addPhotoViewer = function (photoViewer) {
        ctrl.photoViewer = photoViewer;
    };

    ctrl.getPictureByViewMode = function (photosItem, lazyLoadMode, sourceOnlyParameters, photoSize) {
        if ((lazyLoadMode === 'Carousel' && ctrl.isCarouselImgVisible !== true) || (lazyLoadMode === 'Default' && ctrl.isImgVisible !== true)) {
            return null;
        }

        const picture =
            sourceOnlyParameters !== true && ctrl.picture != null && Object.keys(ctrl.picture).length > 0
                ? ctrl.picture
                : photosItem != null
                  ? Array.isArray(photosItem)
                      ? photosItem[0]
                      : photosItem
                  : null;
        let size;

        if (picture == null) {
            return null;
        }

        const photoSizeProp = photoSize != null ? 'Path' + photoSize : null;

        if (ctrl.productViewMode != null) {
            if (['single'].includes(ctrl.productViewMode.viewName)) {
                size = 'PathBig';
            } else {
                size = photoSizeProp ?? (ctrl.productViewMode.isMobile || lazyLoadMode === 'middle' ? 'PathMiddle' : 'PathSmall');
            }
        } else {
            size = photoSizeProp ?? (isMobile || lazyLoadMode === 'middle' ? 'PathMiddle' : 'PathSmall');
        }

        return ctrl.getPictureBySize(size, picture);
    };

    ctrl.getStylePropByViewMode = function (prop, defaultValue, { viewName, value }) {
        const currentViewName = ctrl.productViewMode?.viewName || 'tile';

        let stylesItem = styles.get(prop) || {};

        stylesItem[prop] = viewName == currentViewName ? value : defaultValue;

        styles.set(prop, stylesItem);

        return stylesItem;
    };

    ctrl.lazyLoadImgInCarousel = function () {
        ctrl.isCarouselImgVisible = true;
    };

    ctrl.lazyLoadImg = function () {
        ctrl.isImgVisible = true;
    };

    ctrl.getPictureBySize = function (size, photos) {
        return photos[size];
    };

    ctrl.getMainPhoto = function (photos) {
        return photos.filter(function (photo) {
            return photo.Main === true;
        });
    };

    ctrl.setStateLoading = function (isLoading) {
        ctrl.isLoading = isLoading;
    };

    ctrl.getStateLoading = function () {
        return ctrl.isLoading;
    };
}

export default ProductViewItemCtrl;
