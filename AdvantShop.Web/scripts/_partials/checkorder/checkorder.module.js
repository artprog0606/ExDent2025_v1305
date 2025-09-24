import './styles/check-order.scss';

import CheckOrderCtrl from './controllers/checkorderController.js';
import CheckOrderModalCtrl from './controllers/checkorderModalController.js';
import checkOrderService from './services/checkorderService.js';

const moduleName = 'checkOrder';

angular
    .module(moduleName, [])
    .controller('CheckOrderCtrl', CheckOrderCtrl)
    .controller('CheckOrderModalCtrl', CheckOrderModalCtrl)
    .service('checkOrderService', checkOrderService);

export default moduleName;
