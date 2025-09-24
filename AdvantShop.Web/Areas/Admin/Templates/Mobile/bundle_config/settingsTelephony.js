import appDependency from '../../../../../scripts/appDependency.js';

import '../Content/styles/views/settings.scss';
import '../Content/styles/_shared/card/card.scss';
import '../Content/src/_shared/swipe-line/swipe-line.module.js';
appDependency.addItem(`swipeLine`);

import '../../../Content/src/settingsTelephony/settingsTelephony.js';
import '../../../Content/src/settingsTelephony/settingsTelephonyService.js';
import '../../../Content/src/settingsTelephony/modals/ModalAddPhoneCtrl.js';
appDependency.addItem('settingsTelephony');
