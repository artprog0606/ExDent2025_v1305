import '../../styles/partials/bonus-card.scss';

import cardsModule from '../_partials/cards/cards.module.js';
import buyOneClickModule from '../_partials/buy-one-click/buyOneClick.module.js';
import preOrderModule from '../_partials/pre-order/preOrder.module.js';
import priceAmountCartModule from '../_partials/price-amount-cart/priceAmountCart.module.js';
import '../../styles/views/cart.scss';
//для модуля AdvantShop.Module.RelatedProductsInShoppingCart
import carouselModule from '../_common/carousel/carousel.module.js';
import productsCarouselModule from '../_partials/products-carousel/productsCarousel.module.js';
import productViewModule from '../_partials/product-view/productView.module.js';
//конец зависимости модуля

import CartPageCtrl from './controllers/cartPageController.js';

const moduleName = 'cartPage';

angular
    .module(moduleName, [
        cardsModule,
        buyOneClickModule,
        carouselModule,
        productsCarouselModule,
        productViewModule,
        preOrderModule,
        priceAmountCartModule,
    ])
    .controller('CartPageCtrl', CartPageCtrl);

export default moduleName;
