import '../../styles/views/news.scss';
import '../../styles/partials/pagenumberer.scss';

import productViewModule from '../_partials/product-view/productView.module.js';
import subscribeModule from '../_partials/subscribe/subscribe.module.js';
import carouselModule from '../_common/carousel/carousel.module.js';

const moduleName = 'news';

angular.module(moduleName, [productViewModule, subscribeModule, carouselModule]).controller('NewsCtrl', function () {});

export default moduleName;
