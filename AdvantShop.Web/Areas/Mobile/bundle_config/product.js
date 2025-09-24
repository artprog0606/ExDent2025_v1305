import appDependency from '../../../scripts/appDependency.js';

import uiBootstrapModule from 'angular-ui-bootstrap/src/popover/index.js';

import productViewModule from '../../../scripts/_partials/product-view/productView.module.js';

import '../../../vendors/ui-bootstrap-custom/styles/ui-popover.css';

import carouselModule from '../../../scripts/_common/carousel/carousel.module.js';
import ratingModule from '../../../scripts/_common/rating/rating.module.js';
import rotateModule from '../../../scripts/_common/rotate/rotate.module.js';
import videosModule from '../../../scripts/_partials/videos/videos.module.js';

import customOptionsModule from '../../../scripts/_partials/custom-options/customOptions.module.js';
import colorsViewerModule from '../../../scripts/_partials/colors-viewer/colorsViewer.module.js';
import sizesViewerModule from '../../../scripts/_partials/sizes-viewer/sizesViewer.module.js';
import buyOneClickModule from '../../../scripts/_partials/buy-one-click/buyOneClick.module.js';
import preOrderModule from '../../../scripts/_partials/pre-order/preOrder.module.js';

import tabsModule from '../../../scripts/_common/tabs/tabs.module.js';
import compareModule from '../../../scripts/_partials/compare/compare.module.ts';
import reviewsModule from '../../../scripts/_partials/reviews/reviews.module.js';
import shippingModule from '../../../scripts/_partials/shipping/shipping.module.js';
import priceAmountListModule from '../../../scripts/_partials/price-amount-list/priceAmountList.module.js';

import photoViewerModule from '../../../scripts/_common/photoViewer/photoViewer.module.js';

import '../../../styles/partials/gallery.scss';
import '../../../styles/partials/product-color.scss';
import '../../../styles/partials/properties.scss';
import '../../../styles/partials/bonus-card.scss';
import '../styles/views/product.scss';
import stockListModule from '../../../scripts/product/components/stock-list/stock-list.module.js';

import ProductCtrl from '../../../scripts/product/controllers/productController.js';
import productService from '../../../scripts/product/services/productService.js';
import { showStocksDirective } from '../../../scripts/product/directives/showStocksDirective.js';

import '../../../styles/partials/stickers.scss';
import productAvailabilityMapModule from '../../../scripts/product/components/productAvailabilityMap/productAvailabilityMap.module.js';

import '../styles/_common/popover.scss';

import '../scripts/_partials/cutom-options/styles/customOptions.scss';

const moduleName = 'product';

const deps = [
    uiBootstrapModule,
    tabsModule,
    ratingModule,
    carouselModule,
    productViewModule,
    rotateModule,
    compareModule,
    customOptionsModule,
    colorsViewerModule,
    sizesViewerModule,
    reviewsModule,
    shippingModule,
    priceAmountListModule,
    buyOneClickModule,
    videosModule,
    photoViewerModule,
    preOrderModule,
    productAvailabilityMapModule,
    stockListModule,
];

angular
    .module(moduleName, deps)
    .controller('ProductCtrl', ProductCtrl)
    .service('productService', productService)
    .directive('showStocks', showStocksDirective);

appDependency.addItem(moduleName);
//appDependency.addItem(countdownModule);
