import './styles/price-amount-list.scss';

import PriceAmountListCtrl from './controllers/priceAmountListController.js';
import { priceAmountListDirective } from './directives/priceAmountListDirectives.js';

const moduleName = 'priceAmountList';

angular.module(moduleName, []).controller('PriceAmountListCtrl', PriceAmountListCtrl).directive('priceAmountList', priceAmountListDirective);

export default moduleName;
