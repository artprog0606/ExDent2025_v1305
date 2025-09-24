import './styles/styles.scss';
import PreOrderTriggerCtrl from './controllers/preOrderTriggerController.js';
import PreOrderFormCtrl from './controllers/preOrderFormController.js';
import { preOrderFormDirective, preOrderTriggerDirective } from './directives/preOrderDirectives.js';
import preOrderService from './services/preOrderService.js';

const moduleName = 'preOrder';

angular
    .module(moduleName, [])
    .service('preOrderService', preOrderService)
    .directive('preOrderForm', preOrderFormDirective)
    .directive('preOrderTrigger', preOrderTriggerDirective)
    .controller('PreOrderTriggerCtrl', PreOrderTriggerCtrl)
    .controller('PreOrderFormCtrl', PreOrderFormCtrl);

export default moduleName;
