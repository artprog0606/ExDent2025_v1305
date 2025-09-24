import '../../styles/common/tables.scss';
import '../../scripts/_partials/cart/styles/cart.scss';

import bonusModule from '../_partials/bonus/bonus.module.js';
import addressModule from '../_partials/address/address.module.js';
import autocompleterModule from '../_common/autocompleter/autocompleter.module.js';
import orderModule from '../_partials/order/order.module.js';
import wishlistPageFunnelModule from '../wishlistPage/wishlistPageFunnel.module.js';

import '../../styles/views/myAccount.scss';

import MyAccountCtrl from './controllers/myaccountController.js';

const moduleName = 'myaccount';

angular
    .module(moduleName, [autocompleterModule, bonusModule, addressModule, orderModule, wishlistPageFunnelModule])
    .controller('MyAccountCtrl', MyAccountCtrl);

export default moduleName;
