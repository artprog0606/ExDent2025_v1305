import carouselModule from '../_common/carousel/carousel.module.js';
import productViewModule from '../_partials/product-view/productView.module.js';
import checkOrderModule from '../_partials/checkorder/checkorder.module.js';
import subscribeModule from '../_partials/subscribe/subscribe.module.js';

import '../../styles/views/home.scss';
import '../../styles/partials/banners.scss';
import '../../styles/partials/brands-carousel.scss';
import '../../styles/partials/products-specials.scss';
import '../../styles/partials/product-categories.scss';
import '../../styles/partials/news-block.scss';
import '../../styles/views/giftcertificate.scss';

import HomeCtrl from './controllers/homeController.js';

const moduleName = 'home';

angular.module(moduleName, [carouselModule, productViewModule, checkOrderModule, subscribeModule]).controller('HomeCtrl', HomeCtrl);

export default moduleName;
