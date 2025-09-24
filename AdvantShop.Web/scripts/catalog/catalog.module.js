import '../../styles/partials/product-categories.scss';
import '../../styles/partials/pagenumberer.scss';

import productViewModule from '../_partials/product-view/productView.module.js';
import catalogFilterModule from '../_partials/catalog-filter/catalogFilter.module.js';

import CatalogCtrl from './contollers/catalogController.js';

const moduleName = 'catalog';

angular.module(moduleName, [productViewModule, catalogFilterModule]).controller('CatalogCtrl', CatalogCtrl);

export default moduleName;
