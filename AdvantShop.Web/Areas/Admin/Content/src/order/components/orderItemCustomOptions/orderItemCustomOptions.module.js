import './styles/orderItemCustomOptions.scss';

import CustomOptionsCtrl from './controllers/orderItemCustomOptions.controller.js';
import { orderItemCustomOptionsDirective } from './directives/orderItemCustomOptions.directive.js';
import customOptionsService from './services/orderItemCustomOptions.service.js';

const moduleName = 'orderItemCustomOptions';

angular
    .module(moduleName, [])
    .controller('OrderItemCustomOptionsCtrl', CustomOptionsCtrl)
    .directive('orderItemCustomOptions', orderItemCustomOptionsDirective)
    .service('orderItemCustomOptionsService', customOptionsService);

export default moduleName;
