import appDependency from '../../../../../scripts/appDependency.js';

import 'ng-file-upload';

import '../../../Content/src/_shared/file-uploader/fileUploader.js';
import '../../../Content/src/_shared/file-uploader/fileUploader.component.js';
import '../../../Content/src/_shared/file-uploader/modal/fileUploaderModal.js';

import '../Content/vendors/ng-sortable-custom/ng-sortable.module.js';

import '../Content/styles/views/settings.scss';
import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/views/settings-list.scss';
import '../Content/src/_shared/swipe-line/swipe-line.module.js';

import '../../../Content/src/settingsSeo/settingsSeo.js';
import '../../../Content/styles/views/settingSeoRobots.scss';

import '../../../Content/src/paymentMethods/paymentMethod.js';
import '../../../Content/src/paymentMethods/components/paymentMethodsList/paymentMethodsList.js';

import '../../../Content/src/settings/modal/addEdit301Red/ModalAddEdit301RedCtrl.js';
import '../../../Content/src/settings/modal/import301Red/ModalImport301RedCtrl.js';

import '../../../Content/src/import/import.js';
import '../../../Content/src/import/import.service.js';
import '../../../Content/src/import/import.module.js';

import '../../../Content/src/_shared/ui-ace-textarea/uiAceTextarea.js';
import '../../../Content/src/_shared/ui-ace-textarea/uiAceTextarea.constant.js';
import '../../../Content/src/_shared/ui-ace-textarea/uiAceTextarea.module.js';

appDependency.addItem(`swipeLine`);
appDependency.addItem('settingsSeo');
