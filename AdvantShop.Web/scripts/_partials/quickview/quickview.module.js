import './styles/quickview.scss';

import QuickviewCtrl from './controllers/quickviewController.js';
import { quickviewTriggerDirective } from './directives/quickviewDirectives.js';
import quickviewService from './services/quickviewService.js';
import priceAmountListModule from '../price-amount-list/priceAmountList.module.js';

const moduleName = 'quickview';

angular
    .module(moduleName, [priceAmountListModule])
    .controller('QuickviewCtrl', QuickviewCtrl)
    .directive('quickviewTrigger', quickviewTriggerDirective)
    .service('quickviewService', quickviewService);

export default moduleName;
