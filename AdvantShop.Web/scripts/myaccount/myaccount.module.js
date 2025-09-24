import flatpickrModule from '../../vendors/flatpickr/flatpickr.module.js';
import tabsModule from '../_common/tabs/tabs.module.js';
import bonusModule from '../_partials/bonus/bonus.module.js';
import addressModule from '../_partials/address/address.module.js';
import orderModule from '../_partials/order/order.module.js';
import wishlistPageModule from '../wishlistPage/wishlistPage.module.js';
import ratingModule from '../_common/rating/rating.module.js';
import orderProductModule from '../_partials/order-product/orderProduct.module.js';

import '../../styles/views/myAccount.scss';

import MyAccountCtrl from './controllers/myaccountController.js';

const moduleName = 'myaccount';

angular
    .module(moduleName, [flatpickrModule, tabsModule, bonusModule, addressModule, orderModule, wishlistPageModule, ratingModule, orderProductModule])
    .controller('MyAccountCtrl', MyAccountCtrl);

export default moduleName;
