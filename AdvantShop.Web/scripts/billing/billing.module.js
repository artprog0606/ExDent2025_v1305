import BillingCtrl from './controllers/billingController.js';

import checkoutModule from '../checkout/checkout.module.js';
import orderModule from '../_partials/order/order.module.js';

const moduleName = 'billing';

angular.module(moduleName, [checkoutModule, orderModule]).controller('BillingCtrl', BillingCtrl);

export default moduleName;
