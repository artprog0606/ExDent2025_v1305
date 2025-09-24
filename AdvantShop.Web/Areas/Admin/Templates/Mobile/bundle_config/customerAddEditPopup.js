import appDependency from '../../../../../scripts/appDependency.js';

import '../../../Content/src/customer/customerView.js';
import '../../../Content/src/customer/customer.js';
import '../../../Content/src/customer/components/customerOrders/customerOrders.js';
import '../../../Content/src/customer/components/customerLeads/customerLeads.js';
import '../../../Content/src/customer/components/customerBookings/customerBookings.js';
import '../Content/styles/_shared/flatpickr/flatpickr.scss';
import '../../../Content/src/category/styles/category.scss';
import '../../../Content/src/_shared/modal/sendLetterToCustomer/ModalSendLetterToCustomerCtrl.js';
import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.modified.js';
import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.css';
appDependency.addItem(`ngCkeditor`);
import '../../../Content/src/_shared/modal/sendSms/ModalSendSmsAdvCtrl.js';
import '../../../Content/src/customer/modals/changePassword/ModalChangePasswordCtrl.js';

appDependency.addItem(`customer`);

import '../Content/styles/_shared/card/card.scss';
