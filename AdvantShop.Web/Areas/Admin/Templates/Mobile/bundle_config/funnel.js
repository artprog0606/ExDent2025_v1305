import appDependency from '../../../../../scripts/appDependency.js';

import '../Content/styles/views/domains.scss';
import '../../../Content/src/landing/landings.js';
import '../../../Content/src/landing/modal/ModalAddLandingCtrl.js';
import '../../../Content/src/landing/landings.service.js';
import '../../../Content/src/landingSite/landingSite.js';
import '../../../Content/src/landingSite/modal/editFunnelNameModalCtrl.js';

import '../../../Content/src/booking/booking.js';
import '../../../Content/src/booking/booking.service.js';
appDependency.addItem('booking');

import '../../../Content/src/bookingCategories/components/bookingCategoriesTreeview/bookingCategoriesTreeview.js';
import '../../../Content/src/bookingCategories/components/bookingCategoriesTreeview/bookingCategoriesTreeview.component.js';

appDependency.addItem('bookingCategoriesTreeview');

import '../../../Content/src/landingSite/components/funnelEmailSequences/funnelEmailSequences.js';
import '../../../Content/src/landingSite/components/funnelBookings/funnelBookings.js';
import '../../../Content/src/landingSite/components/funnelLeads/funnelLeads.js';
import '../../../Content/src/landingSite/components/funnelOrders/funnelOrders.js';

import '../../../Content/src/coupons/coupons.js';
import '../../../Content/src/coupons/modal/addEditCoupon/ModalAddEditCouponCtrl.js';

import '../../../Content/src/triggers/triggers.js';
import '../../../Content/src/triggers/triggers.service.js';
import '../../../Content/src/triggers/components/triggerEdit/triggerEdit.component.js';
import '../../../Content/src/triggers/components/triggerEdit/triggerEditCtrl.js';
import '../../../Content/src/triggers/modal/addEditFilterRule/ModalAddEditTriggerFilterRuleCtrl.js';
import '../../../Content/src/triggers/modal/addTrigger/ModalAddTriggerCtrl.js';
import '../../../Content/src/triggers/modal/addEditCategory/modalAddEditCategoryCtrl.js';
import '../../../Content/src/triggers/components/triggerActionEditField/triggerActionEditField.component.js';
import '../../../Content/src/triggers/components/triggerActionEditField/triggerActionEditFieldCtrl.js';
import '../../../Content/src/triggers/components/triggerActionSendRequest/triggerActionSendRequest.component.js';
import '../../../Content/src/triggers/components/triggerActionSendRequest/triggerActionSendRequestCtrl.js';
import '../../../Content/src/triggers/components/triggerActionSendNotification/triggerActionSendNotification.component.js';
import '../../../Content/src/triggers/components/triggerActionSendNotification/triggerActionSendNotificationCtrl.js';

import '../../../Content/src/order/order.js';
import '../../../Content/src/order/modal/getBillingLink/ModalGetBillingLinkCtrl.js';
import '../../../Content/src/order/modal/sendBillingLink/ModalSendBillingLinkCtrl.js';
import '../../../Content/src/order/components/orderItemsSummary/orderItemsSummary.js';
import '../../../Content/src/order/components/orderStatusHistory/orderStatusHistory.js';
import '../../../Content/src/order/components/orderHistory/orderHistory.js';
import '../../../Content/src/order/modal/shippingsTime/ModalShippingsTimeCtrl.js';
import '../../../Content/src/order/modal/shippings/ModalShippingsCtrl.js';

import '../../../../../scripts/_common/spinbox/spinbox.js';
import '../../../../../scripts/_common/spinbox/controllers/spinboxController.js';
import '../../../../../scripts/_common/spinbox/directives/spinboxDirectives.js';

import '../../../Content/src/_shared/modal/selectCustomer/ModalSelectCustomerCtrl.js';
import '../../../Content/src/_shared/modal/sendLetterToCustomer/ModalSendLetterToCustomerCtrl.js';
import '../../../Content/src/_shared/modal/sendSms/ModalSendSmsAdvCtrl.js';

import shippingModule from '../../../../../scripts/_partials/shipping/shipping.module.js';
appDependency.addItem(shippingModule);

import '../../../Content/src/lead/lead.js';
import '../../../Content/src/lead/lead.service.js';
import '../../../Content/src/lead/components/leadItemsSummary/leadItemsSummary.js';

import '../../../Content/src/lead/modal/shippingsCity/ModalShippingsCityCtrl.js';
import '../../../Content/src/lead/modal/completeLead/ModalCompleteLeadCtrl.js';
import '../../../Content/src/lead/modal/desktopAppNotification/desktopAppNotification.js';
import '../../../Content/src/leads/leads.js';
import '../../../Content/src/leads/leads.components.js';
import '../../../Content/src/leads/leadsListController.js';
import '../../../Content/src/leads/components/leadsListSources/leadsListSourcesController.js';
import '../../../Content/src/leads/components/leadsListChart/leadsListChartController.js';
import '../../../Content/src/leads/leadsListService.js';
import '../../../Content/src/leads/modal/changeLeadManager/ModalChangeLeadManagerCtrl.js';
import '../../../Content/src/leads/modal/changeLeadSalesFunnel/ModalChangeLeadSalesFunnelCtrl.js';

import '../../../Content/src/_partials/lead-info/leadInfo.js';
import '../../../Content/src/_partials/lead-info/leadInfo.component.js';
import '../../../Content/src/_partials/lead-info/leadInfoTrigger.js';
import '../../../Content/src/_partials/lead-info/leadInfoTrigger.component.js';
import '../../../Content/src/_partials/lead-info/leadInfo.service.js';

import '../../../Content/src/_partials/leadEvents/leadEvents.js';
import '../../../Content/src/_partials/leadEvents/modals/addEditCallComent/ModalAddEditCallComentCtrl.js';
import '../../../Content/src/_partials/leadEvents/modals/showEmail/ModalShowEmailCtrl.js';
import '../../../Content/src/_partials/leadEvents/modals/editComment/ModalEditCommentCtrl.js';
import '../../../Content/src/_partials/leadEvents/modals/answer/ModalAnswerCtrl.js';

import '../../../Content/src/calls/components/callRecord/callRecord.js';
import 'angular-timeago';
import 'ng-file-upload';

import '../Content/src/_partials/lead-info/styles/lead-info.scss';
import '../Content/src/_partials/leadEvents/styles/lead-events.scss';

appDependency.addItem('landingSite');

import '../Content/styles/funnel.scss';
import '../Content/styles/views/funnel.scss';
import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/_shared/chips/chips.scss';
import '../Content/styles/_shared/order-items-summary/order-items-summary.scss';

import '../../../Content/src/_shared/ui-ace-textarea/uiAceTextarea.js';
import '../../../Content/src/_shared/ui-ace-textarea/uiAceTextarea.constant.js';
import '../../../Content/src/_shared/ui-ace-textarea/uiAceTextarea.module.js';

import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.component.js';

import '../../../Content/src/_shared/modal/products-selectvizr/ModalProductsSelectvizrCtrl.js';
import '../../../Content/src/_shared/modal/export-products-selectvizr/ModalExportProductsSelectvizrCtrl.js';
import '../../../Content/src/_shared/modal/shipping-products-selectvizr/ModalShippingProductsSelectvizrCtrl.js';
import '../../../Content/src/coupons/modal/coupon-products-selectvizr/ModalCouponProductsSelectvizrCtrl.js';

import ModalOffersSelectvizrModule from '../../../Content/src/_shared/modal/offers-selectvizr/ModalOffersSelectvizr.mobile.module.js';
import '../../../Content/src/_shared/modal/products-selectvizr/ModalProductsSelectvizr.mobile.module.js';
appDependency.addItem(ModalOffersSelectvizrModule);

import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';

import '../../../Content/src/_shared/picture-uploader/pictureUploader.js';
import '../../../Content/src/_shared/picture-uploader/pictureUploader.component.js';
import '../../../Content/src/_shared/picture-uploader/modal/pictureUploaderModal.js';

import '../../../Content/src/coupons/coupons.js';
import '../../../Content/src/coupons/modal/addEditCoupon/ModalAddEditCouponCtrl.js';

import '../../../Content/src/_shared/adv-tracking/advTracking.js';
import '../../../Content/src/_shared/adv-tracking/advTracking.service.js';

import '../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
appDependency.addItem(`ngClickCapture`);

import '../../../Content/src/_shared/modal/editableGridRow/ModalEditableGridRow.js';

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
appDependency.addItem(`productGridItem`);

import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.modified.js';
import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.css';
appDependency.addItem(`ngCkeditor`);

import '../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
appDependency.addItem(`ngClickCapture`);

import '../../../Content/src/_shared/modal/editableGridRow/ModalEditableGridRow.js';

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
appDependency.addItem(`productGridItem`);

import '../../../Content/src/tasks/tasks.js';
import '../../../Content/src/tasks/tasks.service.js';
import '../../../Content/src/tasks/modal/editTask/ModalEditTaskCtrl.js';
import '../../../Content/src/tasks/modal/changeTaskStatuses/ModalChangeTaskStatusesCtrl.js';
import '../../../Content/src/tasks/modal/completeTask/ModalCompleteTaskCtrl.js';
import '../../../Content/src/tasks/modal/changeTaskGroup/ModalChangeTaskGroupCtrl.js';
import '../../../Content/src/tasks/task-create.ctrl.js';
import '../../../Content/src/tasks/task-create.component.js';
import '../../../Content/src/_shared/modal/addTask/ModalAddTaskCtrl.js';

import '../../../Content/src/_partials/admin-comments/adminComments.js';
import '../../../Content/src/_partials/admin-comments/adminCommentsForm.js';
import '../../../Content/src/_partials/admin-comments/adminCommentsItem.js';
import '../../../Content/src/_partials/admin-comments/adminComments.component.js';
import '../../../Content/src/_partials/admin-comments/adminComments.service.js';

import '../../../Content/styles/common/delimiter.scss';
import '../Content/styles/views/settings.scss';

import '../../../Content/src/_partials/customer-fields/customerFields.component.js';
appDependency.addItem(`customerFields`);

import '../../../Content/src/_shared/modal/bookingServicesSelectvizr/ModalBookingServicesSelectvizrCtrl.js';

import '../../../Content/src/_partials/booking-services-selectvizr/bookingServicesSelectvizr.js';
import '../../../Content/src/_partials/booking-services-selectvizr/bookingServicesSelectvizr.component.js';
appDependency.addItem(`bookingServicesSelectvizr`);

import '../../../Content/src/bookingJournal/bookingJournal.js';
import '../../../Content/src/bookingJournal/modal/addUpdateBooking/ModalAddUpdateBookingCtrl.js';
import '../../../Content/src/bookingJournal/modal/addUpdateBooking/components/bookingInfo/bookingInfo.js';
import '../../../Content/src/bookingJournal/modal/addUpdateBooking/components/customer/customer.js';
import '../../../Content/src/bookingJournal/modal/addUpdateBooking/components/itemsSummary/itemsSummary.js';
import '../../../Content/src/bookingJournal/modal/addUpdateBooking/modal/payments/payments.js';
import '../../../Content/src/bookingJournal/modal/reservationResourceSheduler/ModalReservationResourceShedulerCtrl.js';
import '../../../Content/src/bookingJournal/modal/bookingsViewGrid/ModalBookingsViewGrid.js';

appDependency.addItem(`bookingInfo`);
appDependency.addItem(`bookingCustomer`);
appDependency.addItem(`bookingItemsSummary`);
appDependency.addItem(`bookingJournal`);

import '../../../Content/vendors/year-calendar/yearCalendar.directive.js';
appDependency.addItem('ngYearCalendar');
import '../../../Content/vendors/fullcalendar/ng-fullcalendar.directive.custom.js';
appDependency.addItem(`angular-fullcalendar`);

import '../../../../../scripts/_partials/shipping/shipping.module.js';
import '../Content/styles/_partials/shipping/shipping.scss';
appDependency.addItem('shipping');

import '../../../Content/src/_partials/sidebar-user/sidebarUser.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUser.service.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUser.component.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUserTrigger.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUserTrigger.component.js';
import '../Content/src/_partials/sidebar-user/styles/sidebar-user.scss';
appDependency.addItem('sidebarUser');

import paymentModule from '../../../../../scripts/_partials/payment/payment.module.js';
import '../../../Content/src/order/modal/payments/ModalPaymentsCtrl.js';
import '../Content/styles/_partials/payment/payment.scss';
appDependency.addItem(paymentModule);

import '../../../Content/src/order/components/orderItemCustomOptions/styles/orderItemCustomOptions.scss';
import '../../../Content/src/order/components/orderItemCustomOptions/orderItemCustomOptions.module.js';
import '../../../Content/src/order/components/orderItemCustomOptions/controllers/orderItemCustomOptions.controller.js';
import '../../../Content/src/order/components/orderItemCustomOptions/directives/orderItemCustomOptions.directive.js';
import '../../../Content/src/order/components/orderItemCustomOptions/services/orderItemCustomOptions.service.js';
appDependency.addItem('orderItemCustomOptions');

import ratingModule from '../../../../../scripts/_common/rating/rating.module.js';
appDependency.addItem(ratingModule);
