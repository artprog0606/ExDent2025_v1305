import '../../styles/partials/login.scss';
import '../../styles/views/login.scss';

import authModule from '../auth/auth.module.js';
import smsConfirmationModule from '../sms/smsConfirmation.module.js';

const moduleName = 'login';

angular.module('login', [authModule, smsConfirmationModule]).controller('LoginCtrl', function () {});

export default moduleName;
