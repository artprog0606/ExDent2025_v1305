import './styles/buyOneClick.scss';

import BuyOneClickTriggerCtrl from './controllers/buyOneClickTriggerController.js';
import BuyOneClickFormCtrl from './controllers/buyOneClickFormController.js';
import { buyOneClickFormDirective, buyOneClickTriggerDirective } from './directives/buyOneClickDirectives.js';
import buyOneClickService from './services/buyOneClickService.js';

const moduleName = 'buyOneClick';

angular
    .module(moduleName, [])
    .service('buyOneClickService', buyOneClickService)
    .directive('buyOneClickForm', buyOneClickFormDirective)
    .directive('buyOneClickTrigger', buyOneClickTriggerDirective)
    .controller('BuyOneClickTriggerCtrl', BuyOneClickTriggerCtrl)
    .controller('BuyOneClickFormCtrl', BuyOneClickFormCtrl);

export default moduleName;
