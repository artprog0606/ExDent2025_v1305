import popover from 'angular-ui-bootstrap/src/popover/index.js';
import '../../../../vendors/ui-bootstrap-custom/styles/ui-popover.css';
import ratingModule from '../../../../scripts/_common/rating/rating.module.js';
import zoomerModule from '../../../../scripts/_common/zoomer/zoomer.module.js';
import tabsModule from '../../../../scripts/_common/tabs/tabs.module.js';

import productViewModule from '../../../../scripts/_partials/product-view/productView.module.js';
import customOptionsModule from '../../../../scripts/_partials/custom-options/customOptions.module.js';
import colorsViewerModule from '../../../../scripts/_partials/colors-viewer/colorsViewer.module.js';
import sizesViewerModule from '../../../../scripts/_partials/sizes-viewer/sizesViewer.module.js';
import buyOneClickModule from '../../../../scripts/_partials/buy-one-click/buyOneClick.module.js';
import preOrderModule from '../../../../scripts/_partials/pre-order/preOrder.module.js';

import '../../../../styles/partials/gallery.scss';
import '../../../../styles/partials/properties.scss';
import '../../../../styles/views/product.scss';

import ProductCtrl from '../../../../scripts/product/controllers/productController.js';
import productService from '../../../../scripts/product/services/productService.js';

import '../../../../scripts/_common/urlHelper/urlHelperService.module.js';

const moduleName = 'product';

const deps = [
    popover,
    ratingModule,
    zoomerModule,
    tabsModule,
    productViewModule,
    customOptionsModule,
    colorsViewerModule,
    sizesViewerModule,
    buyOneClickModule,
    preOrderModule,
    'urlHelper',
];

angular.module(moduleName, deps).controller('ProductCtrl', ProductCtrl).service('productService', productService);

export default moduleName;
