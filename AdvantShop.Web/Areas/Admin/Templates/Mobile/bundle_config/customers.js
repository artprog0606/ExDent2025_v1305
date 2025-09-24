import appDependency from '../../../../../scripts/appDependency.js';
import ngFileUploader from 'ng-file-upload';
appDependency.addItem(ngFileUploader);
import '../../../Content/src/_shared/file-uploader/fileUploader.js';
import '../../../Content/src/_shared/file-uploader/fileUploader.component.js';
import '../../../Content/src/_shared/file-uploader/modal/fileUploaderModal.js';

import '../../../Content/src/customers/customers.js';
import '../../../Content/src/settingsCustomers/settingsCustomers.js';
import '../../../Content/src/settingsCustomers/customerFieldValues.service.js';
import '../../../Content/src/settingsCustomers/customerFields.service.js';
import '../../../Content/src/settingsCustomers/modal/addEditCustomerField/ModalAddEditCustomerFieldCtrl.js';
import '../../../Content/src/settingsCustomers/modal/addEditCustomerFieldValue/ModalAddEditCustomerFieldValueCtrl.js';
appDependency.addItem(`settingsCustomers`);
appDependency.addItem(`customers`);

import '../Content/vendors/ng-sortable-custom/ng-sortable.module.js';

import '../../../Content/src/vk/components/vkAuth/vkAuth.js';

import '../../../Content/src/_partials/customer-info/customerInfo.js';
import '../../../Content/src/_partials/customer-info/customerInfo.service.js';
import '../../../Content/src/_partials/customer-info/customerInfoTrigger.js';
import '../../../Content/src/_partials/customer-info/customerInfoTrigger.component.js';

import '../../../Content/src/_partials/customer-info/customerInfo.component.js';

appDependency.addItem(`customerInfo`);

import '../Content/styles/_shared/ui-grid/ui-grid-selection.scss';
import '../Content/styles/_shared/card/card.scss';
import '../Content/src/_shared/swipe-line/swipe-line.module.js';
appDependency.addItem(`swipeLine`);

import '../Content/styles/_shared/bootstrap/grid.scss';
import '../../../Content/src/customergroups/customergroups.js';
import '../../../Content/src/customergroups/modal/addCustomerGroup/ModalAddCustomerGroupCtrl.js';
import '../../../Content/src/customerSegments/customerSegments.js';
appDependency.addItem(`customerSegments`);

import '../../../Content/src/subscription/subscription.js';
import '../../../Content/src/import/import.module.js';
import '../../../Content/src/customerTags/customerTags.js';
import '../../../Content/src/customerTags/customerTagsModalCtrl.js';

import '../../../Content/src/customer/customer.js';
import '../../../Content/src/customer/components/customerOrders/customerOrders.js';
import '../../../Content/src/customer/components/customerLeads/customerLeads.js';
import '../../../Content/src/customer/components/customerBookings/customerBookings.js';
appDependency.addItem(`customer`);

import '../../../Content/src/_shared/modal/sendLetterToCustomer/ModalSendLetterToCustomerCtrl.js';
import '../../../Content/src/_shared/modal/sendSms/ModalSendSmsAdvCtrl.js';
import '../../../Content/src/_shared/modal/sendSocialMessage/ModalSendSocialMessageCtrl.js';
import '../../../Content/src/_shared/modal/addTagsToCustomers/ModalAddTagsToCustomersCtrl.js';

import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.modified.js';
import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.css';

appDependency.addItem(`ngCkeditor`);

import '../Content/src/_partials/lead-info/styles/lead-info.scss';

import '../Content/styles/views/settings.scss';

import '../../../Content/src/_shared/autocompleter/autocompleter.js';
appDependency.addItem(`autocompleter`);
