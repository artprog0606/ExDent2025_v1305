import './styles/payment.scss';

import PaymentListCtrl from './controllers/paymentListController.js';
import PaymentTemplateCtrl from './controllers/paymentTemplateController.js';
import { paymentListDirective, paymentTemplateDirective } from './directives/paymentDirectives.js';

const moduleName = 'payment';

angular
    .module(moduleName, [])
    .controller('PaymentListCtrl', PaymentListCtrl)
    .controller('PaymentTemplateCtrl', PaymentTemplateCtrl)
    .directive('paymentList', paymentListDirective)
    .directive('paymentTemplate', paymentTemplateDirective);

export default moduleName;
