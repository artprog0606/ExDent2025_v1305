import appDependency from '../../../scripts/appDependency.js';
//import '../../../node_modules/jquery/dist/jquery.js';

import '../../../node_modules/angular/angular.js';

import '../../../vendors/stop-angular-overrides/stop-angular-overrides.js';

import '../../../node_modules/angularjs-toaster/toaster.js';
import '../../../node_modules/angularjs-toaster/toaster.min.css';
appDependency.addItem('toaster');

import '../Content/styles/common/captcha.scss';
import '../Content/src/_shared/toaster-ext/toaster-ext.css';

//IWC  SignalR
import '../../../vendors/signalr/jquery.signalR.js';
import '../Content/vendors/iwc/iwc.module.js';
import '../Content/vendors/iwc-signalr/signalr-patch.js';
import '../Content/vendors/iwc-signalr/iwc-signalr.js';

import fullHeightMobileModule from '../../../scripts/_mobile/full-height-mobile/full-height-mobile.module.js';
appDependency.addItem(fullHeightMobileModule);

import '../Content/src/error/error.js';
appDependency.addItem('error');

import '../Templates/AdminV3/Content/styles/theme.scss';

import '../Content/styles/admin-login.scss';
