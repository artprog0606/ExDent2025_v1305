import flatpickrModule from '../../vendors/flatpickr/flatpickr.module.js';

import smsModule from '../sms/smsConfirmation.module.js';

import '../../scripts/_partials/login-open-id/loginOpenId.module.js';

import RegistrationPageCtrl from './controllers/registrationController.js';

const moduleName = 'registrationPage';

angular.module(moduleName, [flatpickrModule, smsModule]).controller('RegistrationPageCtrl', RegistrationPageCtrl);

export default moduleName;
