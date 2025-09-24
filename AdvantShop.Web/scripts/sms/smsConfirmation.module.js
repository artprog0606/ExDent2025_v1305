import './smsConfirmation.scss';

import SmsAuthCtrl from './controllers/smsAuthCtrl.js';
import SmsConfirmationCtrl from './controllers/smsConfirmationCtrl.js';
import SmsConfirmationInputCtrl from './controllers/smsConfirmationInputCtrl.js';
import smsConfirmationService from './services/smsService.js';
import { smsConfirmationDirective, smsConfirmationInputComponent } from './directives/smsDirectives.js';

const moduleName = 'smsConfirmation';

angular
    .module(moduleName, [])
    .service('smsConfirmationService', smsConfirmationService)
    .controller('SmsAuthCtrl', SmsAuthCtrl)
    .controller('SmsConfirmationCtrl', SmsConfirmationCtrl)
    .controller('SmsConfirmationInputCtrl', SmsConfirmationInputCtrl)
    .directive('smsConfirmation', smsConfirmationDirective)
    .component('smsConfirmationInput', smsConfirmationInputComponent);

export default moduleName;
