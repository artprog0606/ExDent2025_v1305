import ProductsCarouselCtrl from './controllers/productsCarouselController.js';
import { productsCarouselDirective } from './directives/productsCarouselDirectives.js';
import productsCarouselService from './services/productsCarouselService.js';

const moduleName = 'productsCarousel';

angular
    .module(moduleName, [])
    .directive('productsCarousel', productsCarouselDirective)
    .controller('ProductsCarouselCtrl', ProductsCarouselCtrl)
    .service('productsCarouselService', productsCarouselService);

export default moduleName;
