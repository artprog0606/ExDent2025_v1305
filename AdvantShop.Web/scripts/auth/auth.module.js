import '../../fonts/fa.scss';

import AuthCtrl from './controllers/authController.js';
import authService from './services/authService.js';
import loginOpenId from '../../scripts/_partials/login-open-id/loginOpenId.module.js';

const moduleName = 'auth';

angular.module(moduleName, []).controller('AuthCtrl', AuthCtrl).service('authService', authService);

export default moduleName;
