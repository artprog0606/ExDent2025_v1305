import carouselModule from '../../_common/carousel/carousel.module.js';
import ratingModule from '../../_common/rating/rating.module.js';
import quickviewModule from '../quickview/quickview.module.js';
import colorsViewerModule from '../colors-viewer/colorsViewer.module.js';
import productsCarouselModule from '../products-carousel/productsCarousel.module.js';
import photoViewListModule from '../../../scripts/_partials/photo-view-list/photoViewList.module.js';

import './styles/product-view.scss';
import '../price-amount-list/styles/price-amount-list.scss';

import productViewService from './services/productViewService.js';
import {
    productViewItemDirective,
    productViewCarouselPhotosDirective,
    productViewChangeModeDirective,
    productViewModeDirective,
    productViewImageListDirective,
    productViewScrollPhotosDirective,
    productViewImageDirective,
} from './directives/productViewDirectives.js';

import ProductViewCarouselPhotosCtrl from './controllers/productViewCarouselPhotosController.js';
import ProductViewChangeModeCtrl from './controllers/productViewChangeModeController.js';
import ProductViewItemCtrl from './controllers/productViewItemController.js';
import ProductViewModeCtrl from './controllers/productViewModeController.js';

import '../../../styles/partials/modal-video.scss';
import ProductViewImageListCtrl from './controllers/productViewImageListController.js';
import ProductViewImageCtrl from './controllers/productViewImageController.js';

const moduleName = 'productView';

angular
    .module(moduleName, [ratingModule, quickviewModule, colorsViewerModule, productsCarouselModule, photoViewListModule, carouselModule])
    .constant('viewPrefix', {
        desktop: '',
        mobile: 'mobile-',
        mobileModern: 'mobile-modern-',
    })
    .constant('viewList', {
        desktop: ['tile', 'list', 'table'],
        mobile: ['tile', 'list', 'single'],
        mobileModern: ['tile', 'list', 'single'],
    })
    .service('productViewService', productViewService)
    .directive('productViewItem', productViewItemDirective)
    .directive('productViewCarouselPhotos', productViewCarouselPhotosDirective)
    .directive('productViewChangeMode', productViewChangeModeDirective)
    .directive('productViewMode', productViewModeDirective)
    .directive('productViewImageList', productViewImageListDirective)
    .directive('productViewImage', productViewImageDirective)
    .directive('productViewScrollPhotos', productViewScrollPhotosDirective)
    .controller('ProductViewCarouselPhotosCtrl', ProductViewCarouselPhotosCtrl)
    .controller('ProductViewChangeModeCtrl', ProductViewChangeModeCtrl)
    .controller('ProductViewItemCtrl', ProductViewItemCtrl)
    .controller('ProductViewImageListCtrl', ProductViewImageListCtrl)
    .controller('ProductViewImageCtrl', ProductViewImageCtrl)
    .controller('ProductViewModeCtrl', ProductViewModeCtrl);

export default moduleName;
