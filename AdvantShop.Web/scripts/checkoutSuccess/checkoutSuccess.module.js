import '../../styles/views/checkoutSuccess.scss';

import carouselMopdule from '../_common/carousel/carousel.module.js';
import productsViewModule from '../_partials/product-view/productView.module.js';
import productsCarouselModule from '../_partials/products-carousel/productsCarousel.module.js';

import CheckOutSuccessCtrl from './controllers/checkoutSuccessController.js';
import '../../styles/views/checkout-thank-page.scss';

const moduleName = 'checkoutSuccess';

angular.module(moduleName, [productsViewModule, productsCarouselModule, carouselMopdule]).controller('CheckOutSuccessCtrl', CheckOutSuccessCtrl);

export default moduleName;
