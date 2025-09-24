import appDependency from '../../../../../scripts/appDependency.js';

import '../Content/styles/views/settings.scss';
import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/views/settings-list.scss';
import '../Content/src/_shared/swipe-line/swipe-line.module.js';

import '../../../Content/src/mailSettings/mailSettings.js';
import '../../../Content/src/mailSettings/modal/addEditMailFormat/ModalAddEditMailFormatCtrl.js';
import '../../../Content/src/mailSettings/modal/updateAddress/ModalEmailSettingsUpdateAddressCtrl.js';
import '../../../Content/src/mailSettings/modal/addEditMailAnswerTemplate/ModalAddEditMailAnswerTemplateCtrl.js';
import '../../../Content/src/mailSettings/modal/addEditSmsAnswerTemplate/ModalAddEditSmsAnswerTemplateCtrl.js';

import '../../../Content/src/settingsSms/settingsSms.js';
import '../../../Content/src/settingsAuthCall/settingsAuthCall.js';
import '../../../Content/src/settingsSms/modal/addEditSmsTemplateOnOrderChanging/ModalAddEditSmsTemplateOnOrderChangingCtrl.js';

import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.modified.js';
import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.css';

appDependency.addItem(`ngCkeditor`);
appDependency.addItem(`swipeLine`);
appDependency.addItem('mailSettings');
appDependency.addItem('settingsAuthCall');
