import './styles/orderHistory.scss';
import ngFileUploadModule from '../../../node_modules/ng-file-upload/index.js';
import OrderHistoryCtrl from './controllers/orderHistoryController.js';
import {
    orderHistoryDirective,
    orderHistoryItemsDirective,
    orderHistoryDetailsDirective,
    orderPayDirective,
    orderReviewDirective,
} from './directives/orderDirectives.js';
import orderService from './services/orderService.js';
import OrderPayCtrl from './controllers/orderPayController.js';
import OrderReviewCtrl from './controllers/orderReviewController.js';

const moduleName = 'order';

angular
    .module('order', [ngFileUploadModule])
    .service('orderService', orderService)
    .controller('OrderHistoryCtrl', OrderHistoryCtrl)
    .controller('OrderPayCtrl', OrderPayCtrl)
    .controller('OrderReviewCtrl', OrderReviewCtrl)
    .directive('orderHistory', orderHistoryDirective)
    .directive('orderHistoryItems', orderHistoryItemsDirective)
    .directive('orderHistoryDetails', orderHistoryDetailsDirective)
    .directive('orderPay', orderPayDirective)
    .directive('orderReview', orderReviewDirective);

export default moduleName;
