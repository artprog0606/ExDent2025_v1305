import appDependency from '../../../../../scripts/appDependency.js';

import '../Content/styles/views/settings.scss';
import '../Content/styles/_shared/card/card.scss';

import '../../../Content/src/emailingLog/emailingLog.js';
import '../../../Content/src/emailingLog/modal/viewEmail/ModalViewEmailCtrl.js';

import '../../../Content/src/triggerAnalytics/triggerAnalytics.js';
appDependency.addItem(`triggerAnalytics`);

import '../../../Content/src/_shared/emailingAnalytics/emailingAnalytics.js';
appDependency.addItem(`emailingAnalytics`);
