import appDependency from '../../../../../scripts/appDependency.js';

import 'ng-file-upload';
import 'angular-timeago';
appDependency.addItem(`ngFileUpload`);

import '../../../Content/src/booking/booking.js';
import '../../../Content/src/booking/booking.service.js';

import '../../../Content/src/bookingAffiliate/bookingAffiliate.js';
import '../../../Content/src/bookingAffiliate/bookingAffiliate.service.js';
import '../../../Content/src/bookingAffiliate/modals/addAffiliate/ModalAddAffiliateCtrl.js';
import '../../../Content/src/bookingAffiliate/modals/addUpdateAdditionalTime/ModalAddUpdateAdditionalTimeCtrl.js';
import '../../../Content/src/bookingAffiliate/modals/addUpdateSmsTemplate/addUpdateSmsTemplate.js';

import '../../../Content/src/bookingAffiliateSettings/bookingAffiliateSettings.js';

import '../../../Content/src/bookingAnalytics/bookingAnalytics.js';
import '../../../Content/src/bookingAnalytics/components/turnover/turnover.js';
import '../../../Content/src/bookingAnalytics/components/sources/sources.js';
import '../../../Content/src/bookingAnalytics/components/reservationResources/reservationResources.js';
import '../../../Content/src/bookingAnalytics/components/paymentMethods/paymentMethods.js';
import '../../../Content/src/bookingAnalytics/components/common/common.js';
import '../../../Content/src/bookingAnalytics/components/services/services.js';
import '../../../Content/src/bookingAnalytics/modal/services/services.js';

import '../../../Content/src/bookingCategories/bookingCategories.js';
import '../../../Content/src/bookingCategories/bookingCategories.service.js';
import '../../../Content/src/bookingCategories/components/listBookingCategories/listBookingCategories.js';
import '../../../Content/src/bookingCategories/components/bookingCategoriesTreeview/bookingCategoriesTreeview.js';
import '../../../Content/src/bookingCategories/components/bookingCategoriesTreeview/bookingCategoriesTreeview.component.js';
import '../../../Content/src/bookingCategories/modals/addEditCategory/ModalAddEditBookingCategoryCtrl.js';

import '../../../Content/src/bookingJournal/bookingJournal.js';
import '../../../Content/src/bookingJournal/modal/addUpdateBooking/ModalAddUpdateBookingCtrl.js';
import '../../../Content/src/bookingJournal/modal/addUpdateBooking/components/bookingInfo/bookingInfo.js';
import '../../../Content/src/bookingJournal/modal/addUpdateBooking/components/customer/customer.js';
import '../../../Content/src/bookingJournal/modal/addUpdateBooking/components/itemsSummary/itemsSummary.js';
import '../../../Content/src/bookingJournal/modal/addUpdateBooking/modal/payments/payments.js';
import '../../../Content/src/bookingJournal/modal/reservationResourceSheduler/ModalReservationResourceShedulerCtrl.js';
import '../../../Content/src/bookingJournal/modal/bookingsViewGrid/ModalBookingsViewGrid.js';

import '../../../Content/src/bookingReservationResources/bookingReservationResources.js';
import '../../../Content/src/bookingReservationResources/components/listOfReservationResourceServices/listOfReservationResourceServices.js';
import '../../../Content/src/bookingReservationResources/components/listAdditionalTime/listReservationResourceAdditionalTime.js';
import '../../../Content/src/bookingReservationResources/modal/addUpdateReservationResource/ModalAddUpdateReservationResourceCtrl.js';
import '../../../Content/src/bookingReservationResources/modal/addUpdateAdditionalTime/ModalAddUpdateAdditionalTimeCtrl.js';

import '../../../Content/src/bookingServices/bookingServices.js';
import '../../../Content/src/bookingServices/modals/addUpdateBookingService/ModalAddUpdateBookingServiceCtrl.js';

import '../../../Content/src/_partials/booking-services-selectvizr/bookingServicesSelectvizr.js';
import '../../../Content/src/_partials/booking-services-selectvizr/bookingServicesSelectvizr.component.js';
import '../../../Content/src/_shared/modal/bookingServicesSelectvizr/ModalBookingServicesSelectvizrCtrl.js';

import '../../../Content/src/_shared/booking-sheduler-days/booking-sheduler-days.js';
import '../../../Content/src/_shared/booking-sheduler-days/booking-sheduler-days.ctrl.js';
import '../../../Content/src/_shared/booking-sheduler-days/booking-sheduler-days.component.js';

import '../../../Content/src/_shared/booking-sheduler/styles/booking-sheduler.css';
import '../../../Content/src/_shared/booking-sheduler/booking-sheduler.js';
import '../../../Content/src/_shared/booking-sheduler/booking-sheduler.ctrl.js';
import '../../../Content/src/_shared/booking-sheduler/booking-sheduler.component.js';

import '../../../Content/src/_partials/leadEvents/styles/lead-events.scss';
import '../../../Content/src/_partials/leadEvents/leadEvents.js';

import '../../../Content/src/calls/components/callRecord/callRecord.js';

import '../../../Content/src/_shared/modal/selectCustomer/ModalSelectCustomerCtrl.js';

import '../../../Content/src/booking/styles/booking.scss';
import '../../../Content/src/bookingAnalytics/styles/booking-analytics.scss';
import '../../../Content/src/bookingJournal/modal/reservationResourceSheduler/styles/reservation-resource-sheduler.css';

import '../Content/styles/views/settings.scss';
import '../Content/styles/_shared/chips/chips.scss';
import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/_shared/product-list/product-list.scss';
import '../Content/src/_shared/swipe-line/swipe-line.module.js';
import '../Content/src/_partials/lead-info/styles/lead-info.scss';
import '../Content/styles/views/settings-list.scss';
import '../Content/styles/_partials/messegers.scss';
import '../Content/styles/_partials/shipping/shipping.scss';
import '../Content/styles/views/analytics.scss';
import '../../../Content/styles/common/delimiter.scss';
import '../Content/src/_partials/sidebar-user/styles/sidebar-user.scss';
import '../Content/src/_partials/leadEvents/styles/lead-events.scss';
import '../Content/styles/funnel.scss';
import '../Content/styles/_shared/product-item/product-item.scss';
import '../Content/styles/_shared/product-list/product-list.scss';
import '../Content/src/shippingMethods/components/shippingMethodsList/styles/shippingsMethodsList.scss';

import '../../../Content/src/_partials/sidebar-user/sidebarUser.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUser.service.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUser.component.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUserTrigger.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUserTrigger.component.js';
import '../Content/src/_partials/sidebar-user/styles/sidebar-user.scss';
appDependency.addItem('sidebarUser');

import '../Content/vendors/ng-sortable-custom/ng-sortable.module.js';
appDependency.addItem(`as.sortable`);

import '../../../Content/src/_shared/file-uploader/fileUploader.js';
import '../../../Content/src/_shared/file-uploader/fileUploader.component.js';
import '../../../Content/src/_shared/file-uploader/modal/fileUploaderModal.js';

import '../../../Content/vendors/fullcalendar/ng-fullcalendar.directive.custom.js';

import '../../../Content/vendors/dragscroll/dragscroll.js';

import '../../../Content/vendors/cropper/cropper.min.js';
import '../../../Content/vendors/cropper/cropper.css';

import '../../../Content/vendors/cropper/ngCropper.js';
import '../../../Content/src/_shared/modal/cropImage/ModalCropImageCtrl.js';
appDependency.addItem(`ngCropper`);

import '../../../Content/vendors/year-calendar/yearCalendar.directive.js';
appDependency.addItem('ngYearCalendar');

import '../../../Content/src/_shared/modal/editableGridRow/ModalEditableGridRow.js';
import '../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
appDependency.addItem(`ngClickCapture`);

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
appDependency.addItem(`productGridItem`);

import scrollIntoViewModule from '../../../../../scripts/_common/scroll-into-view/scroll-into-view.js';
appDependency.addItem(scrollIntoViewModule);

import checklistModelModule from 'checklist-model';
appDependency.addItem(checklistModelModule);

appDependency.addItem('booking');
appDependency.addItem('bookingAffiliate');
appDependency.addItem('bookingAffiliateSettings');
appDependency.addItem('bookingAnalytics');
appDependency.addItem('bookingCategories');
appDependency.addItem('bookingJournal');
appDependency.addItem('bookingCustomer');
appDependency.addItem('bookingReservationResources');
appDependency.addItem('bookingServices');
appDependency.addItem('bookingServicesSelectvizr');
appDependency.addItem('bookingShedulerDays');
appDependency.addItem('bookingSheduler');
appDependency.addItem('bookingInfo');
appDependency.addItem('leadEvents');
appDependency.addItem('callRecord');
appDependency.addItem('angular-fullcalendar');
