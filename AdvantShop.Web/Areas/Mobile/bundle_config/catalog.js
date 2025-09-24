import appDependency from '../../../scripts/appDependency.js';

import catalogModule from '../../../scripts/catalog/catalog.module.js';
import catalogFilterMobileModule from '../scripts/_common/catalogFilterMobile/catalogFilterMobile.module.js';
import showMoreModule from '../scripts/_partials/showMore/showMore.module.js';
import photoViewerModule from '../../../scripts/_common/photoViewer/photoViewer.module.js';

appDependency.addItem(showMoreModule);
appDependency.addItem(catalogModule);
appDependency.addItem(catalogFilterMobileModule);
appDependency.addItem(photoViewerModule);

import '../styles/_partials/view-mode.scss';
import '../styles/_partials/catalog-filter.scss';
import '../styles/_partials/pagenumberer.scss';
import '../styles/_partials/product-view.scss';
import '../styles/_partials/catalog-sort.scss';
import '../styles/views/catalog.scss';
import '../styles/views/product.scss';
