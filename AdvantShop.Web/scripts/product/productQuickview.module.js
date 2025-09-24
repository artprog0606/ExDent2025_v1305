import carouselModule from '../_common/carousel/carousel.module.js';
import ratingModule from '../_common/rating/rating.module.js';
import rotateModule from '../_common/rotate/rotate.module.js';
import videosModule from '../_partials/videos/videos.module.js';
import zoomerModule from '../_common/zoomer/zoomer.module.js';

import customOptionsModule from '../_partials/custom-options/customOptions.module.js';
import colorsViewerModule from '../_partials/colors-viewer/colorsViewer.module.js';
import sizesViewerModule from '../_partials/sizes-viewer/sizesViewer.module.js';
import buyOneClickModule from '../_partials/buy-one-click/buyOneClick.module.js';
import preOrderModule from '../_partials/pre-order/preOrder.module.js';

import '../../styles/partials/gallery.scss';
import '../../styles/partials/product-color.scss';

import '../../styles/views/product.scss';

import ProductCtrl from './controllers/productController.js';
import productService from './services/productService.js';

const moduleName = 'productQuickView';

const deps = [
    ratingModule,
    carouselModule,
    zoomerModule,
    rotateModule,
    customOptionsModule,
    colorsViewerModule,
    sizesViewerModule,
    buyOneClickModule,
    videosModule,
    preOrderModule,
];

angular.module(moduleName, deps).controller('ProductCtrl', ProductCtrl).service('productService', productService);

export default moduleName;
