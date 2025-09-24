import '../../styles/partials/brands-carousel.scss';
import '../../styles/partials/pagenumberer.scss';
import '../../styles/partials/menu-dropdown.scss';

import '../../styles/views/brands.scss';

import productViewModule from '../_partials/product-view/productView.module.js';
import catalogFilterModule from '../_partials/catalog-filter/catalogFilter.module.js';

import BrandCtrl from './controllers/brandController.js';
import brandService from './services/brandService.js';

const moduleName = 'brand';

angular.module(moduleName, [productViewModule, catalogFilterModule]).controller('BrandCtrl', BrandCtrl).service('brandService', brandService);

export default moduleName;
