import './styles/price-amount-cart.scss';

import PriceAmountCartCtrl from './controllers/priceAmountCartController.js';
import { priceAmountCartDirective } from './directives/priceAmountCartDirectives.js';

const moduleName = 'priceAmountCart';

angular.module(moduleName, []).controller('PriceAmountCartCtrl', PriceAmountCartCtrl).directive('priceAmountCart', priceAmountCartDirective);

export default moduleName;
