import appDependency from '../../../../../scripts/appDependency.js';

import '../../../Content/src/customer/customerView.js';
import '../../../Content/src/customer/customer.js';
import '../../../Content/src/customer/components/customerOrders/customerOrders.js';
import '../../../Content/src/customer/components/customerLeads/customerLeads.js';
import '../../../Content/src/customer/components/customerBookings/customerBookings.js';
import '../../../Content/src/customer/modals/changePassword/ModalChangePasswordCtrl.js';
appDependency.addItem(`customer`);
appDependency.addItem(`customerView`);
import '../Content/styles/_shared/card/card.scss';
import '../../../Content/src/customer/styles/customer.scss';
import '../Content/styles/_partials/customer-data/customer-data.scss';
import '../Content/styles/_partials/view-info/view-info.scss';
import '../Content/styles/_partials/view-social/view-social.scss';
import '../Content/styles/atomic.scss';
import ngFileUpload from 'ng-file-upload';
import '../../../Content/src/booking/booking.js';
import '../../../Content/src/booking/booking.service.js';
appDependency.addItem(ngFileUpload);
import '../../../Content/src/_shared/modal/sendSocialMessage/ModalSendSocialMessageCtrl.js';
import '../../../Content/src/_partials/leadEvents/modals/addEditCallComent/ModalAddEditCallComentCtrl.js';
import '../../../Content/src/calls/components/callRecord/callRecord.js';
import '../../../Content/src/_partials/leadEvents/modals/showEmail/ModalShowEmailCtrl.js';
import '../../../Content/src/_partials/leadEvents/leadEvents.js';
appDependency.addItem(`leadEvents`);

appDependency.addItem(`booking`);
import 'angular-timeago';
appDependency.addItem(`yaru22.angular-timeago`);

import '../Content/styles/_partials/segments/segments.scss';
import '../Content/styles/_partials/bonus-card-client/bonus-card-client.scss';
import '../../../Content/src/_shared/modal/bonus/Cards/ModalAddCardCtrl.js';
import '../../../Content/src/_shared/modal/selectCustomer/ModalSelectCustomerCtrl.js';
import '../../../Content/src/_shared/modal/selectPartner/ModalSelectPartnerCtrl.js';
import '../Content/styles/_partials/view-partner/view-partner.scss';
import '../Content/styles/_shared/view-interests/views-interests.scss';
import '../Content/styles/_shared/view-tags/view-tags.scss';
import '../Content/styles/_shared/customer-orders/customer-orders.scss';
import '../Content/styles/_shared/customer-leads/customer-leads.scss';

import '../../../Content/src/_shared/modal/addLead/ModalAddLeadCtrl.js';
import '../../../Content/src/_shared/modal/sendLetterToCustomer/ModalSendLetterToCustomerCtrl.js';

import '../../../Content/src/_shared/modal/sendSms/ModalSendSmsAdvCtrl.js';
import '../../../Content/src/_shared/modal/selectSmsTemplate/ModalSelectSmsTemplateCtrl.js';

import '../../../Content/src/customer/modals/desktopAppNotification/desktopAppNotification.js';

import '../../../Content/src/customer/modals/sendMobileAppNotification/sendMobileAppNotification.js';

import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.modified.js';
import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.css';
appDependency.addItem(`ngCkeditor`);

import '../../../Content/src/_shared/adv-tracking/advTracking.js';
import '../../../Content/src/_shared/adv-tracking/advTracking.service.js';

import '../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
appDependency.addItem(`ngClickCapture`);

import '../../../Content/src/_shared/modal/editableGridRow/ModalEditableGridRow.js';

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
appDependency.addItem(`productGridItem`);

import offersSelectvizr from '../../../Content/src/_shared/modal/offers-selectvizr/ModalOffersSelectvizr.mobile.module.js';
appDependency.addItem(offersSelectvizr);

import '../../../Content/src/_partials/customer-fields/customerFields.component.js';
appDependency.addItem('customerFields');

import '../../../Content/src/_partials/customer-info/customerInfo.js';
import '../../../Content/src/_partials/customer-info/customer-info.scss';
import '../../../Content/src/_partials/customer-info/customerInfo.component.js';
import '../../../Content/src/_partials/customer-info/customerInfo.service.js';
import '../../../Content/src/_partials/customer-info/customerInfoTrigger.js';
import '../../../Content/src/_partials/customer-info/customerInfoTrigger.component.js';
appDependency.addItem('customerInfo');

import '../Content/styles/views/customer-add-edit-poppup.scss';

import '../../../Content/src/lead/lead.js';
import '../../../Content/src/lead/lead.service.js';
import '../../../Content/src/lead/components/leadItemsSummary/leadItemsSummary.js';
import '../../../Content/src/lead/modal/shippingsCity/ModalShippingsCityCtrl.js';
import '../../../Content/src/lead/modal/completeLead/ModalCompleteLeadCtrl.js';
import '../../../Content/src/lead/modal/desktopAppNotification/desktopAppNotification.js';
appDependency.addItem('lead');

import '../../../Content/src/_partials/lead-info/leadInfo.js';
import '../../../Content/src/_partials/lead-info/leadInfo.component.js';
import '../../../Content/src/_partials/lead-info/leadInfoTrigger.js';
import '../../../Content/src/_partials/lead-info/leadInfoTrigger.component.js';
import '../../../Content/src/_partials/lead-info/leadInfo.service.js';
import '../Content/src/_partials/lead-info/styles/lead-info.scss';

import '../../../Content/src/_partials/leadEvents/leadEvents.js';
import '../../../Content/src/_partials/leadEvents/modals/addEditCallComent/ModalAddEditCallComentCtrl.js';
import '../../../Content/src/_partials/leadEvents/modals/showEmail/ModalShowEmailCtrl.js';
import '../../../Content/src/_partials/leadEvents/modals/editComment/ModalEditCommentCtrl.js';
import '../../../Content/src/_partials/leadEvents/modals/answer/ModalAnswerCtrl.js';

import '../Content/src/_partials/lead-info/styles/lead-info.scss';
import '../Content/src/_partials/leadEvents/styles/lead-events.scss';

appDependency.addItem('leadInfo');

import '../../../../../scripts/_partials/shipping/shipping.module.js';
import '../Content/styles/_partials/shipping/shipping.scss';
appDependency.addItem('shipping');

import '../../../Content/src/tasks/tasks.js';
import '../../../Content/src/tasks/tasks.service.js';
import '../../../Content/src/tasks/modal/editTask/ModalEditTaskCtrl.js';
import '../../../Content/src/tasks/modal/changeTaskStatuses/ModalChangeTaskStatusesCtrl.js';
import '../../../Content/src/tasks/modal/completeTask/ModalCompleteTaskCtrl.js';
import '../../../Content/src/tasks/modal/changeTaskGroup/ModalChangeTaskGroupCtrl.js';
import '../../../Content/src/tasks/task-create.ctrl.js';
import '../../../Content/src/tasks/task-create.component.js';
import '../../../Content/src/_shared/modal/addTask/ModalAddTaskCtrl.js';
appDependency.addItem(`tasks`);

import '../../../Content/src/_partials/admin-comments/adminComments.js';
import '../../../Content/src/_partials/admin-comments/adminCommentsForm.js';
import '../../../Content/src/_partials/admin-comments/adminCommentsItem.js';
import '../../../Content/src/_partials/admin-comments/adminComments.component.js';
import '../../../Content/src/_partials/admin-comments/adminComments.service.js';

import '../../../Content/styles/common/delimiter.scss';
import '../Content/styles/views/settings.scss';
import '../Content/styles/_shared/order-items-summary/order-items-summary.scss';

import '../../../Content/src/_partials/sidebar-user/sidebarUser.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUser.service.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUser.component.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUserTrigger.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUserTrigger.component.js';
import '../Content/src/_partials/sidebar-user/styles/sidebar-user.scss';
appDependency.addItem('sidebarUser');

import '../../../Content/src/order/order.js';
import '../../../Content/src/order/modal/getBillingLink/ModalGetBillingLinkCtrl.js';
import '../../../Content/src/order/modal/sendBillingLink/ModalSendBillingLinkCtrl.js';
import '../../../Content/src/order/components/orderItemsSummary/orderItemsSummary.js';
import '../../../Content/src/order/components/orderStatusHistory/orderStatusHistory.js';
import '../../../Content/src/order/components/orderHistory/orderHistory.js';
import '../../../Content/src/order/modal/shippingsTime/ModalShippingsTimeCtrl.js';
import '../../../Content/src/order/modal/shippings/ModalShippingsCtrl.js';

import '../../../Content/src/_shared/picture-uploader/pictureUploader.js';
import '../../../Content/src/_shared/picture-uploader/pictureUploader.component.js';
import '../../../Content/src/_shared/picture-uploader/modal/pictureUploaderModal.js';

import 'ng-file-upload';

import '../../../Content/src/_shared/autocompleter/autocompleter.js';
appDependency.addItem(`autocompleter`);

import addressModule from '../Content/src/_partials/address/address.module.js';
appDependency.addItem(addressModule);

import '../../../Content/src/order/modal/changeAddress/ModalChangeOrderAddressCtrl.js';
