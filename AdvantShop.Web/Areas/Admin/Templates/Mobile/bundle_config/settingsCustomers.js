import appDependency from '../../../../../scripts/appDependency.js';

import 'ng-file-upload';

import '../../../Content/src/settingsCustomers/settingsCustomers.js';
import '../../../Content/src/settingsCustomers/customerFields.service.js';
import '../../../Content/src/settingsCustomers/customerFieldValues.service.js';
import '../../../Content/src/settingsCustomers/modal/addEditCustomerField/ModalAddEditCustomerFieldCtrl.js';
import '../../../Content/src/settingsCustomers/modal/addEditCustomerFieldValue/ModalAddEditCustomerFieldValueCtrl.js';

import '../../../Content/src/_partials/customer-fields/customerFields.component.js';
appDependency.addItem(`customerFields`);

import '../../../Content/src/_shared/file-uploader/fileUploader.js';
import '../../../Content/src/_shared/file-uploader/fileUploader.component.js';
import '../../../Content/src/_shared/file-uploader/modal/fileUploaderModal.js';

import '../Content/styles/views/settings.scss';
import '../Content/styles/views/settings-list.scss';
import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/_shared/chips/chips.scss';
import '../Content/styles/funnel.scss';
import '../Content/src/_shared/swipe-line/swipe-line.module.js';
import '../Content/vendors/ng-sortable-custom/ng-sortable.module.js';
import '../../../Content/src/vk/components/vkAuth/vkAuth.js';

import '../../../Content/src/customers/customers.js';
import '../../../Content/src/customergroups/customergroups.js';
import '../../../Content/src/customergroups/modal/addCustomerGroup/ModalAddCustomerGroupCtrl.js';
import '../../../Content/src/customergroups/modal/addEditCustomerGroupCategoryDiscount/ModalAddEditCustomerGroupCategoryDiscountCtrl.js';

import '../../../Content/src/customerSegments/customerSegments.js';

import '../../../Content/src/subscription/subscription.js';
import '../../../Content/src/_shared/modal/sendSms/ModalSendSmsAdvCtrl.js';
import '../../../Content/src/_shared/modal/sendLetterToCustomer/ModalSendLetterToCustomerCtrl.js';

import '../../../Content/src/import/import.js';
import '../../../Content/src/import/import.service.js';
import '../../../Content/src/import/import.module.js';

import '../../../Content/src/customerTags/customerTags.js';
import '../../../Content/src/customerTags/customerTagsModalCtrl.js';

appDependency.addItem(`swipeLine`);
appDependency.addItem('settingsCustomers');

import '../../../Content/src/_shared/export-customers/exportCustomers.js';
appDependency.addItem('exportCustomers');
