import appDependency from '../../../scripts/appDependency.js';

import productlistModule from '../../../scripts/productList/productList.module.js';
import catalogFilterMobileModule from '../scripts/_common/catalogFilterMobile/catalogFilterMobile.module.js';
import showMoreModule from '../scripts/_partials/showMore/showMore.module.js';

appDependency.addItem(productlistModule);
appDependency.addItem(catalogFilterMobileModule);
appDependency.addItem(showMoreModule);

import '../styles/_partials/view-mode.scss';
import '../styles/_partials/catalog-filter.scss';
import '../styles/_partials/pagenumberer.scss';
import '../styles/_partials/product-view.scss';
import '../styles/_partials/catalog-sort.scss';
import '../styles/views/catalog.scss';
import '../styles/views/product.scss';
