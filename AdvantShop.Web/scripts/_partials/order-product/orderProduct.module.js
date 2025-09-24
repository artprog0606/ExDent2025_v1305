import './orderProduct.scss';
const moduleName = 'orderProduct';

import orderProductDirective from './directives/orderProductDirective.js';
import orderProductService from './services/orderProductService.js';
import OrderProductCtrl from './controllers/orderProductController.js';

angular
    .module(moduleName, [])
    .service('orderProductService', orderProductService)
    .controller('OrderProductCtrl', OrderProductCtrl)
    .directive('orderProduct', orderProductDirective);

export default moduleName;
