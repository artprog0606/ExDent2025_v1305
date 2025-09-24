/* @ngInject */
function ProductViewModeCtrl($attrs, $element, productViewService, viewList, viewPrefix, $parse, $scope) {
    const ctrl = this;
    ctrl.$onInit = function () {
        ctrl.isMobile = $attrs.isMobile === 'true';
        ctrl.currentViewList = viewList[$attrs.viewListName || 'desktop'];
        ctrl.currentViewPrefix = viewPrefix[$attrs.viewListName || 'desktop'];
        ctrl.defaultViewMode = $attrs.defaultViewMode;
        ctrl.photoHeightByViewMode = $attrs.photoHeightByViewMode != null ? $parse($attrs.photoHeightByViewMode)($scope) : null;
        ctrl.photoHeightByViewModeDefault = $attrs.photoHeightByViewModeDefault != null ? $parse($attrs.photoHeightByViewModeDefault)($scope) : null;

        if (ctrl.isMobile === true) {
            ctrl.viewName = productViewService.getViewFromCookie('mobile_viewmode', ctrl.currentViewList, ctrl.defaultViewMode);
            $element[0].classList.add('products-view-' + ctrl.currentViewPrefix + ctrl.viewName);
        } else {
            ctrl.viewName = $attrs.current;
        }
        setPhotoSizeByViewMode();
        productViewService.addCallback('setView', onChangeMode);

        function onChangeMode(view) {
            ctrl.viewName = view.viewName;
            view.viewList.forEach(function (item) {
                $element[0].classList.remove('products-view-' + ctrl.currentViewPrefix + item);
            });
            requestAnimationFrame(() => {
                $element[0].classList.add('products-view-' + ctrl.currentViewPrefix + view.viewName);

                setPhotoSizeByViewMode();
            });
        }

        ctrl.getCurrentBlockPhotoHeight = () => {
            return ctrl.photoHeightByViewMode != null && ctrl.photoHeightByViewMode.viewName === ctrl.viewName
                ? ctrl.photoHeightByViewMode.value
                : ctrl.photoHeightByViewModeDefault;
        };

        function setPhotoSizeByViewMode() {
            if (ctrl.photoHeightByViewMode != null && ctrl.photoHeightByViewMode.viewName === ctrl.viewName) {
                $element[0].style.setProperty('--product-view-photo-size', ctrl.photoHeightByViewMode.value);
                return;
            }
            if (ctrl.photoHeightByViewModeDefault != null) {
                $element[0].style.setProperty('--product-view-photo-size', ctrl.photoHeightByViewModeDefault);
                return;
            }
        }
    };
}

export default ProductViewModeCtrl;
