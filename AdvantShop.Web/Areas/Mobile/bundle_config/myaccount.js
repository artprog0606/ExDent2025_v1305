import myaccountModule from '../../../scripts/myaccount/myaccount.module.js';
import wishlistPageModule from '../../../scripts/wishlistPage/wishlistPage.module.js';

import appDependency from '../../../scripts/appDependency.js';

appDependency.addItem(myaccountModule);
appDependency.addItem(wishlistPageModule);

import '../styles/_partials/tabs.mobile.scss';

import '../scripts/_partials/order/styles/orderHistory.scss';
import '../styles/views/myAccount.scss';
import '../styles/views/wishlistPage.scss';
