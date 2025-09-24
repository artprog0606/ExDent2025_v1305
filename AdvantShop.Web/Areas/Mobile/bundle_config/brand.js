import appDependency from '../../../scripts/appDependency.js';

import brandModule from '../../../scripts/brand/brand.module.js';
import showMoreModule from '../scripts/_partials/showMore/showMore.module.js';
import '../../../scripts/_partials/submenu/submenu.module.js';

appDependency.addItem(showMoreModule);
appDependency.addItem(brandModule);
appDependency.addItem('submenu');

import '../styles/views/brands.scss';
