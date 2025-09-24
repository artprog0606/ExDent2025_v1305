export const PRODUCT_VIEW_IMAGE_CONTAINER_CLASS = 'js-product-view-image-container';

/* @ngInject */
function ProductViewImageCtrl($element, $attrs, $parse, $scope, $document) {
    const ctrl = this;

    const isMobile = $document[0].documentElement.classList.contains('mobile-version');

    ctrl.$onInit = function () {
        ctrl.photoSize = $attrs.photoSize != null ? $parse($attrs.photoSize)($scope) : null;
        ctrl.startPhotoJson = $attrs.startPhotoJson != null ? $parse($attrs.startPhotoJson)($scope) : null;
        ctrl.containerImage = $element[0].closest(`.${PRODUCT_VIEW_IMAGE_CONTAINER_CLASS}`);
        ctrl.productViewImageList?.addImageCtrl(ctrl);
    };

    ctrl.getPictureBySize = (size, photos) => {
        return photos[size];
    };

    ctrl.viewModeToPhotoSizeMapper = (viewMode, photoSizeProp, isMobile) => {
        switch (viewMode) {
            case 'single':
                return 'PathBig';
            default:
                return photoSizeProp ?? (isMobile ? 'PathMiddle' : 'PathSmall');
        }
    };
    ctrl.getPhotoSrc = (photo, viewMode) => {
        const picture = photo ?? (Array.isArray(ctrl.startPhotoJson) ? ctrl.startPhotoJson[0] : ctrl.startPhotoJson);
        let size;

        if (picture == null) {
            return null;
        }

        const photoSizeProp = ctrl.photoSize != null ? 'Path' + ctrl.photoSize : null;

        size =
            ctrl.productViewMode != null
                ? ctrl.viewModeToPhotoSizeMapper(viewMode ?? ctrl.productViewMode.viewName, photoSizeProp, ctrl.productViewMode.isMobile)
                : ctrl.viewModeToPhotoSizeMapper(null, photoSizeProp, isMobile);

        return ctrl.getPictureBySize(size, picture);
    };

    ctrl.hiddenImage = () => {
        ctrl.containerImage.classList.add('product-view-image-container--hidden');
    };

    ctrl.changeSrc = (photo, viewObj) => {
        ctrl.containerImage.classList.remove('product-view-image-container--hidden');
        ctrl.containerImage.classList.add('product-view-image-container--block');

        const newSrc = ctrl.getPhotoSrc(photo, viewObj?.viewMode);
        if (newSrc === $element[0].src) return;

        $element[0].src = newSrc;
    };
}

export default ProductViewImageCtrl;
