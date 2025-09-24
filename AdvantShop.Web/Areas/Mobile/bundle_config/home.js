import appDependency from '../../../scripts/appDependency.js';

import homeModule from '../../../scripts/home/home.module.js';
import '../styles/views/home.scss';

import '../styles/_partials/mainpage-products.scss';

import '../styles/_partials/catalog-root.scss';
import '../styles/_partials/news-block.scss';

import '../../../styles/partials/stickers.scss';

import '../styles/_partials/view-mode.scss';

//import countdownModule from '../../../scripts/_common/countdown/countdown.module.js';
import geoModeModule from '../../../scripts/_common/geoMode/geoMode.module.ts';
appDependency.addItem(homeModule);
appDependency.addItem(geoModeModule);
//appDependency.addItem(countdownModule);
