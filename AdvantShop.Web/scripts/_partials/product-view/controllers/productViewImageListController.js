import { productViewItemControls } from '../productViewItem.controls.js';

export const productViewImageListMapperRequest = ({
    productId,
    viewMode,
    blockProductPhotoHeight,
    productImageType,
    photoWidth,
    photoHeight,
    isProductPhotoLazy,
    limitPhotoCount,
    colorInitialId,
    renderedPhotoId,
}) => {
    return {
        productId,
        productViewMode: viewMode,
        blockProductPhotoHeight,
        productImageType,
        photoWidth,
        photoHeight,
        isProductPhotoLazy,
        limitPhotoCount: limitPhotoCount - 1,
        colorId: colorInitialId,
        RenderedPhotoId: renderedPhotoId,
    };
};

/* @ngInject */
function ProductViewImageListCtrl($element, $attrs, $parse, $scope, $http, $q, productViewService, $compile) {
    const ctrl = this;

    ctrl.$onInit = function () {
        ctrl.productId = $attrs.productId != null ? $parse($attrs.productId)($scope) : null;
        ctrl.viewMode = $attrs.viewMode != null ? $parse($attrs.viewMode)($scope) : null;
        ctrl.blockProductPhotoHeight = $attrs.blockProductPhotoHeight != null ? $parse($attrs.blockProductPhotoHeight)($scope) : null;
        ctrl.productImageType = $attrs.productImageType != null ? $parse($attrs.productImageType)($scope) : null;
        ctrl.photoWidth = $attrs.photoWidth != null ? $parse($attrs.photoWidth)($scope) : null;
        ctrl.photoHeight = $attrs.photoHeight != null ? $parse($attrs.photoHeight)($scope) : null;
        ctrl.isProductPhotoLazy = $attrs.isProductPhotoLazy != null ? $parse($attrs.isProductPhotoLazy)($scope) : null;
        ctrl.limitPhotoCount = $attrs.limitPhotoCount != null ? $parse($attrs.limitPhotoCount)($scope) : 5;
        ctrl.colorInitialId = $parse($attrs.colorInitialId)($scope);
        ctrl.renderedPhotoId = $parse($attrs.renderedPhotoId)($scope);

        ctrl.isLoadedHtml = false;
        ctrl.photosStorage = null;
        ctrl.currentPhotos = [];
        ctrl.imageCtrls = [];

        ctrl.initial = true;
        ctrl.productViewItem.addControl(productViewItemControls.productListCtrl, ctrl);

        ctrl.currentColorId = ctrl.colorInitialId;

        productViewService.addCallback('setView', function (viewObj) {
            ctrl.viewMode = viewObj.viewName;

            ctrl.filterPhotos({
                isVisible: ctrl.isVisible,
                colorId: ctrl.currentColorId,
            });
        });
    };

    ctrl.addImageCtrl = function (ctrlChild) {
        if (ctrl.imageCtrls.includes(ctrlChild)) return;

        return ctrl.imageCtrls.push(ctrlChild);
    };

    ctrl.removeImageCtrl = function (id) {
        ctrl.imageCtrls.splice(id, 1);
    };

    ctrl.updatePhotos = function () {
        if (ctrl.currentPhotos.length === 0) return;

        ctrl.imageCtrls.forEach((imageCtrl, i) => {
            if (ctrl.checkAvailablePhoto(i)) {
                imageCtrl.changeSrc(ctrl.currentPhotos[i], ctrl.viewMode);
            } else {
                imageCtrl.hiddenImage();
            }
        });
    };

    ctrl.checkAvailablePhoto = function (index) {
        return index >= 0 && index < Math.min(ctrl.limitPhotoCount, ctrl.currentPhotos.length);
    };

    ctrl.getPhoto = function (photoIndex) {
        return ctrl.currentPhotos[photoIndex];
    };

    ctrl.getPhotos = function () {
        var defer = $q.defer();

        if (ctrl.photosStorage != null && ctrl.photosStorage.length) return defer.resolve(ctrl.photosStorage);

        return productViewService.getPhotos(ctrl.productId).then(function (photos) {
            ctrl.photosStorage = photos;
            return photos;
        });
    };

    ctrl.filterPhotos = function ({ isVisible, colorId, onlyColorPhoto }) {
        ctrl.isVisible = typeof isVisible === 'undefined' ? ctrl.isVisible : isVisible;
        ctrl.currentColorId = colorId;

        if (ctrl.isVisible === false) return;

        if (ctrl.photosStorage) {
            ctrl.currentPhotos = productViewService.filterPhotos(ctrl.photosStorage, colorId, onlyColorPhoto).slice(0, ctrl.limitPhotoCount);
            ctrl.updatePhotos();
        } else {
            ctrl.getPhotos().then((photos) => {
                ctrl.currentPhotos = productViewService.filterPhotos(photos, colorId, onlyColorPhoto).slice(0, ctrl.limitPhotoCount);
                ctrl.updatePhotos();
            });
        }
    };

    productViewService.addCallback('setView', () => {
        requestAnimationFrame(() => {
            $element[0].scrollLeft = 0;
        });
    });

    ctrl.getPhotoHtml = function (data) {
        if (data.productId == null) return;

        return $http.post('/mobile/product/ProductViewPhoto', data).then(function (response) {
            return response.data;
        });
    };
    ctrl.insertHtml = function (replaceInner = false) {
        var defer = $q.defer();

        if (ctrl.isLoadedHtml) return defer.promise;

        const data = {
            productId: ctrl.productId,
            productImageType: ctrl.productImageType,
            photoWidth: ctrl.photoWidth,
            photoHeight: ctrl.photoHeight,
            isProductPhotoLazy: ctrl.isProductPhotoLazy,
            limitPhotoCount: ctrl.limitPhotoCount,
            colorInitialId: ctrl.colorInitialId,
            renderedPhotoId: ctrl.renderedPhotoId,
        };

        if (ctrl.productViewMode != null) {
            data.viewMode = ctrl.productViewMode.viewName;
            data.blockProductPhotoHeight = ctrl.productViewMode.getCurrentBlockPhotoHeight();
        } else {
            data.viewMode = ctrl.viewMode;
            data.blockProductPhotoHeight = ctrl.blockProductPhotoHeight;
        }

        data.blockProductPhotoHeight = parseInt(data.blockProductPhotoHeight);

        return ctrl.getPhotoHtml(productViewImageListMapperRequest(data)).then((html) => {
            if (html == null) return;
            ctrl.isLoadedHtml = true;

            $element.find('.js-skeleton-photos').remove();
            $element.append(html);

            $compile($element.children('.mobile-product-view-item-image-inner').slice(1))($scope.$new());
        });
    };
}

export default ProductViewImageListCtrl;
