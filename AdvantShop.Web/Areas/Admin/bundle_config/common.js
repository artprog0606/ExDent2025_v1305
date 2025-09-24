import '../../../fonts/fa.scss';

import './templates.js';

import appDependency from '../../../scripts/appDependency.js';

//import '../../../node_modules/jquery/dist/jquery.js';
//import '../../../vendors/jquery/jquery.passive.js';

import '../../../node_modules/angular/angular.js';

import '../../../vendors/stop-angular-overrides/stop-angular-overrides.js';

import '../../../node_modules/angular-cookies/angular-cookies.js';
appDependency.addItem('ngCookies');

import '../../../node_modules/angular-sanitize/angular-sanitize.js';
appDependency.addItem('ngSanitize');

import '../../../node_modules/angular-translate/dist/angular-translate.js';
appDependency.addItem('pascalprecht.translate');

import '../../../node_modules/oclazyload/dist/ocLazyLoad.js';
import '../../../vendors/ocLazyLoad/ocLazyLoad.decorate.js';
appDependency.addItem('oc.lazyLoad');

import '../../../scripts/_partials/zone/zone.js';
import zoneModule from '../../../scripts/_partials/zone/zone.module.ts';
appDependency.addItem(zoneModule);

import '../../../vendors/sweetalert/sweetalert2.default.js';
import '../../../vendors/ng-sweet-alert/ng-sweet-alert.js';
appDependency.addItem('ng-sweet-alert');

import '../../../node_modules/angularjs-toaster/toaster.js';
import '../../../node_modules/angularjs-toaster/toaster.min.css';
appDependency.addItem('toaster');
import '../Content/src/_shared/toaster-ext/toaster-ext.css';

//IWC  SignalR
import '../../../vendors/signalr/jquery.signalR.js';
import '../Content/vendors/iwc/iwc.module.js';
import '../Content/vendors/iwc-signalr/signalr-patch.js';
import '../Content/vendors/iwc-signalr/iwc-signalr.js';

import '../Content/vendors/angular-web-notification/desktop-notify.module.js';
import 'angular-web-notification/angular-web-notification.js';
appDependency.addItem(`angular-web-notification`);

import adminWebNotificationsModule from '../Content/src/_shared/admin-web-notifications/adminWebNotifications.module.js';
appDependency.addItem(adminWebNotificationsModule);

/*---*/
import '../../../fonts/fonts.admin.css';

import carouselModule from '../../../scripts/_common/carousel/carousel.module.js';
appDependency.addItem(carouselModule);

import flatpickrModule from '../../../vendors/flatpickr/flatpickr.module.js';
appDependency.addItem(flatpickrModule);

import '../Content/src/_shared/modal/styles/uimodal.scss';
import '../Content/src/_shared/modal/styles/simpleUiModal.scss';
import '../Content/src/_shared/modal/uiModal.js';
import '../Content/src/_shared/modal/uiModalDecorator.js';
import '../Content/src/_shared/modal/uiModal.component.js';
appDependency.addItem('uiModal');

import '../Content/src/menus/modal/addEditMenuItem/ModalAddEditMenuItemCtrl.js';
import '../Content/src/menus/modal/changeParentMenuItem/ModalChangeParentMenuItemCtrl.js';

import '../Content/styles/design_style.css';
import '../Content/styles/panels.scss';

import 'animate.css/animate.min.css';

import '../Content/styles/bootstrap.min.css';
import '../Content/styles/bootstrap.overwrite.css';

import '../Content/styles/flexboxgrid/flexboxgrid.css';
import '../Content/styles/flexboxgrid/flexboxgrid-ext.css';

import '../Content/styles/grid-classic.scss';
import '../Content/styles/custom-fields.scss';
import '../Content/styles/funnel.scss';
import '../Content/styles/breadcrumbs.scss';

import '../Content/styles/headers.scss';
//import '../Content/styles/style.scss';
import '../Content/styles/paginations.scss';
import '../Content/styles/link.scss';
import '../Content/styles/select-main-site.scss';
//import '../Content/styles/theme.scss';
import '../Content/styles/buttons.scss';

import '../Content/src/_shared/ui-grid-custom/uiGridCustom.module.js';
import '../../../node_modules/angular-ui-grid/ui-grid.css';
import '../Content/src/_shared/ui-grid-custom/styles/ui-grid.custom.scss';
import '../Content/src/_shared/ui-grid-custom/styles/ui-grid.custom.pagination.scss';
import '../Content/src/_shared/ui-grid-custom/styles/ui-grid.custom.selection.scss';
import '../Content/src/_shared/ui-grid-custom/styles/ui-grid.custom.edit.scss';
import '../Content/src/_shared/ui-grid-custom/styles/ui-grid.custom.filter.scss';

import kanbanModule from '../Content/src/_shared/kanban/kanban.module.js';
appDependency.addItem(kanbanModule);

import '../../../styles/common/validation.scss';

import '../Content/src/_shared/autocompleter/autocompleter.js';
appDependency.addItem('autocompleter');

import '../../../scripts/_common/spinbox/spinbox.module.js';
appDependency.addItem('spinbox');

import '../../../scripts/_common/transformer/transformer.module.js';
appDependency.addItem('transformer');

import shippingModule from '../../../scripts/_partials/shipping/shipping.module.js';
appDependency.addItem(shippingModule);

import paymentModule from '../../../scripts/_partials/payment/payment.module.js';
appDependency.addItem(paymentModule);

// import customOptionsModule from '../../../scripts/_partials/custom-options/customOptions.module.js';
// appDependency.addItem(customOptionsModule);

import '../../../styles/views/errors.scss';

import '../../../node_modules/jstree/dist/themes/default/style.css';
import '../Content/vendors/jsTree.directive/themes/advantshop/style.css';
import '../Content/vendors/jsTree.directive/jsTree.directive.custom.js';
appDependency.addItem('jsTree.directive');

import '../Content/vendors/ng-sortable-custom/ng-sortable.module.js';
appDependency.addItem(`as.sortable`);

import 'tinycolor2';

import '../Content/vendors/angular-color-picker/angularjs-color-picker.cjs';
import '../Content/vendors/angular-color-picker/angularjs-color-picker.css';
import '../Content/vendors/angular-color-picker/themes/angularjs-color-picker-bootstrap.css';
appDependency.addItem(`color.picker`);

import * as Ladda from '../../../node_modules/ladda/dist/ladda-themeless.min.css';

import '../../../node_modules/ladda/js/ladda.js';
import '../../../node_modules/angular-ladda/dist/angular-ladda.js';
appDependency.addItem('angular-ladda');

import photoViewerModule from '../../../scripts/_common/photoViewer/photoViewer.module.js';
appDependency.addItem(photoViewerModule);

import uiSelectModule from '../Content/vendors/ui-select/ui-select.module.js';
appDependency.addItem(uiSelectModule);

import '../Content/styles/progress-overlay.scss';

import '../Content/vendors/cropper/cropper.min.js';
import '../Content/vendors/cropper/cropper.css';

import '../Content/vendors/cropper/ngCropper.js';
import '../Content/src/_shared/modal/cropImage/ModalCropImageCtrl.js';
appDependency.addItem(`ngCropper`);

import '../Content/vendors/ng-ckeditor/ng-ckeditor.module.js';
appDependency.addItem(`ngCkeditor`);

import '../Content/vendors/jquery.rcrumbs/jquery.rcrumbs.css';
import '../Content/vendors/jquery.rcrumbs/jquery.rcrumbs.js';
import '../Content/vendors/jquery.rcrumbs/angular.rcrumbs.js';
appDependency.addItem(`rcrumbs`);

import '../Content/src/_shared/audio-player/styles/audio-player.css';
import '../Content/src/_shared/audio-player/audioPlayer.js';
import '../Content/src/_shared/audio-player/audioPlayer.component.js';
appDependency.addItem('audioPlayer');

import '../Content/src/_shared/emailingAnalytics/styles/emailing-analytics.css';
import '../Content/src/_shared/emailingAnalytics/emailingAnalytics.js';
appDependency.addItem('emailingAnalytics');

import '../Content/src/_shared/input-ghost/styles/inputGhost.scss';
import '../Content/src/_shared/input-ghost/inputGhost.js';
import '../Content/src/_shared/input-ghost/inputGhost.directive.js';
appDependency.addItem('inputGhost');

import uiAceTextarea from '../../Admin/Content/src/_shared/ui-ace-textarea/uiAceTextarea.module.js';
appDependency.addItem(uiAceTextarea);

import productPickerModule from '../Content/src/_shared/product-picker/productPicker.module.js';
appDependency.addItem(productPickerModule);

import '../Content/src/tariffs/styles/tariffs.scss';
import '../Content/src/tariffs/tariffs.js';
appDependency.addItem('tariffs');

import '../Content/src/_shared/search/styles/search.scss';
import '../Content/src/_shared/search/searchBlockController.js';
appDependency.addItem('search');

import '../Content/src/_shared/picture-uploader/styles/picture-uploader.scss';
import '../Content/src/_shared/picture-uploader/pictureUploader.js';
import '../Content/src/_shared/picture-uploader/pictureUploader.component.js';
import '../Content/src/_shared/picture-uploader/modal/pictureUploaderModal.js';
appDependency.addItem('pictureUploader');

import '../Content/src/_shared/file-uploader/styles/file-uploader.scss';
import '../Content/src/_shared/file-uploader/fileUploader.js';
import '../Content/src/_shared/file-uploader/fileUploader.component.js';
import '../Content/src/_shared/file-uploader/modal/fileUploaderModal.js';
appDependency.addItem('fileUploader');

import '../Content/src/_shared/simple-edit/simple-edit.scss';
import '../Content/src/_shared/simple-edit/simpleEdit.js';
import '../Content/src/_shared/simple-edit/simpleEdit.ctrl.js';
import '../Content/src/_shared/simple-edit/simpleEdit.component.js';
appDependency.addItem('simpleEdit');

import '../Content/src/_shared/input/styles/input.scss';
//import '../../../styles/common/custom-input.scss';
import '../Content/src/_shared/custom-input/custom-input.scss';

import '../Content/src/_shared/switch-on-off/switchOnOff.js';
appDependency.addItem('switchOnOff');

import switcherStateModule from '../Content/src/_shared/switcher-state/switcherState.module.js';
appDependency.addItem(switcherStateModule);

import '../Content/src/_shared/icon-move/styles/icon-move.css';
import '../Content/src/_shared/icon-move/iconMove.js';
appDependency.addItem('iconMove');

import '../Content/src/_partials/admin-comments/styles/admin-comments.scss';
import '../Content/src/_partials/admin-comments/adminComments.js';
import '../Content/src/_partials/admin-comments/adminComments.component.js';
import '../Content/src/_partials/admin-comments/adminComments.service.js';
appDependency.addItem('adminComments');

import '../Content/src/_partials/admin-comments/adminCommentsItem.js';
appDependency.addItem('adminCommentsItem');

import '../Content/src/_partials/admin-comments/adminCommentsForm.js';
appDependency.addItem('adminCommentsForm');

import '../Content/src/_partials/person-avatar/styles/person-avatar.scss';
import '../Content/src/_partials/person-avatar/personAvatar.js';
import '../Content/src/_partials/person-avatar/personAvatar.component.js';
import '../Content/src/_partials/person-avatar/personAvatarImage.directive.js';
appDependency.addItem('personAvatar');

import '../Content/src/_partials/categories-block/styles/categories-block.scss';
import '../Content/src/_partials/categories-block/categoriesBlock.js';
import '../Content/src/_partials/categories-block/categoriesBlock.component.js';
appDependency.addItem('categoriesBlock');

import helpTriggerModule from '../Content/src/_partials/help-trigger/helpTrigger.module.js';
appDependency.addItem(helpTriggerModule);

import leadInfoModule from '../Content/src/_partials/lead-info/leadInfo.module.js';
appDependency.addItem(leadInfoModule);

import customerInfoModule from '../Content/src/_partials/customer-info/customerInfo.module.js';
appDependency.addItem(customerInfoModule);

import sidebarUserModule from '../Content/src/_partials/sidebar-user/sidebarUser.module.js';
appDependency.addItem(sidebarUserModule);

import timesOfWorkModule from '../Content/src/_partials/times-of-work/times-of-work.module.js';
appDependency.addItem(timesOfWorkModule);

//import '../Content/src/_partials/user-info-popup/userInfoPopup.js';
//import '../Content/src/_partials/user-info-popup/userInfoPopup.component.js';
//appDependency.addItem('userInfoPopup');

//import '../Content/src/_partials/user-info-popup/modals/styles/user-info-popup.scss';
//import '../Content/src/_partials/user-info-popup/modals/ModalUserInfoPopupCtrl.js';
//import '../Content/src/_partials/user-info-popup/modals/ModalUserInfoPopupInviteCtrl.js';

import '../Content/src/catalog/styles/catalog.scss';
import '../Content/src/catalog/catalog.js';
import '../Content/src/catalog/catalog.service.js';
import '../Content/src/catalog/components/catalog-treeview/catalogTreeview.js';
import '../Content/src/catalog/components/catalog-treeview/catalogTreeview.component.js';
import '../Content/src/catalog/components/catalog-left-menu/catalogLeftMenu.js';
import '../Content/src/catalog/components/catalog-left-menu/catalogLeftMenu.directive.js';
appDependency.addItem('catalog');

import '../Content/src/customer/styles/customer.scss';
import '../Content/src/customer/customer.js';
appDependency.addItem('customer');

import '../Content/src/customer/customerView.js';
appDependency.addItem('customerView');

import '../Content/src/customer/components/customerBookings/customerBookings.js';
appDependency.addItem('customerBookings');

import '../Content/src/customer/components/customerLeads/customerLeads.js';
appDependency.addItem('customerLeads');

import '../Content/src/customer/components/customerOrders/customerOrders.js';
appDependency.addItem('customerOrders');

import '../Content/src/customer/modals/changePassword/ModalChangePasswordCtrl.js';
import '../Content/src/customer/modals/desktopAppNotification/desktopAppNotification.js';
import '../Content/src/customer/modals/sendMobileAppNotification/sendMobileAppNotification.js';

import '../Content/src/home/home.js';
import '../Content/src/home/components/salesChannelsGraph/sales-channels-graph.scss';
import '../Content/src/home/components/salesChannelsGraph/salesChannelsGraph.js';
appDependency.addItem('home');

import '../Content/src/module/styles/module.css';
import '../Content/src/module/module.js';
appDependency.addItem('module');

import '../Content/src/modules/styles/modules.scss';
import '../Content/src/modules/modules.js';
import '../Content/src/modules/modules.service.js';
appDependency.addItem('modules');

import '../Content/src/partner/styles/partner.css';
import '../Content/src/partner/partner.js';
appDependency.addItem('partner');

import '../Content/src/partner/partnerView.js';
appDependency.addItem('partnerView');

import '../Content/src/partner/modals/changePassword/ModalChangePartnerPasswordCtrl.js';
import '../Content/src/partner/modals/addPartnerMoney/modalAddPartnerMoneyCtrl.js';
import '../Content/src/partner/modals/addPartnerCouponFromTpl/modalAddPartnerCouponFromTplCtrl.js';
import '../Content/src/partner/modals/partnerRewardPayout/modalPartnerRewardPayoutCtrl.js';
import '../Content/src/partner/modals/subtractPartnerMoney/modalSubtractPartnerMoneyCtrl.js';

import '../Content/src/partner/components/partnerTransactions/partnerTransactions.js';
appDependency.addItem('partnerTransactions');

import '../Content/src/partner/components/partnerCustomers/partnerCustomers.js';
appDependency.addItem('partnerCustomers');

import '../Content/src/partner/components/partnerActReports/partnerActReports.js';
appDependency.addItem('partnerActReports');

import productModule from '../Content/src/product/product.module.js';
import '../Content/src/product/styles/product.scss';
appDependency.addItem(productModule);

import '../Content/src/_partials/leadEvents/styles/lead-events.scss';
import '../Content/src/_partials/leadEvents/leadEvents.js';
appDependency.addItem('leadEvents');

import '../Content/src/_partials/leadEvents/modals/answer/ModalAnswerCtrl.js';
import '../Content/src/_partials/leadEvents/modals/editComment/ModalEditCommentCtrl.js';
import '../Content/src/_partials/leadEvents/modals/addEditCallComent/ModalAddEditCallComentCtrl.js';
import '../Content/src/_partials/leadEvents/modals/showEmail/ModalShowEmailCtrl.js';

import '../Content/src/lead/styles/lead.css';
import '../Content/src/lead/lead.js';
import '../Content/src/lead/lead.service.js';
appDependency.addItem('lead');

import '../Content/src/lead/modal/desktopAppNotification/desktopAppNotification.js';
import '../Content/src/lead/modal/completeLead/ModalCompleteLeadCtrl.js';
import '../Content/src/lead/modal/shippingsCity/ModalShippingsCityCtrl.js';

import '../Content/src/lead/components/leadItemsSummary/leadItemsSummary.js';
appDependency.addItem('leadItemsSummary');

import '../Content/src/leads/leads.js';
import '../Content/src/leads/leads.components.js';
import '../Content/src/leads/leadsListController.js';
import '../Content/src/leads/components/leadsListSources/leadsListSourcesController.js';
import '../Content/src/leads/components/leadsListChart/leadsListChartController.js';
import '../Content/src/leads/leadsListService.js';
appDependency.addItem('leads');

import '../Content/src/leads/modal/changeLeadManager/ModalChangeLeadManagerCtrl.js';
import '../Content/src/leads/modal/changeLeadSalesFunnel/ModalChangeLeadSalesFunnelCtrl.js';

import '../Content/src/triggers/styles/triggers.css';
import '../Content/src/triggers/triggers.js';
import '../Content/src/triggers/triggers.service.js';
appDependency.addItem('triggers');

import '../Content/src/triggers/modal/addTrigger/ModalAddTriggerCtrl.js';
import '../Content/src/triggers/modal/addEditCategory/modalAddEditCategoryCtrl.js';
import '../Content/src/triggers/modal/addEditFilterRule/ModalAddEditTriggerFilterRuleCtrl.js';
import '../Content/src/triggers/modal/changeTriggerName/ModalChangeTriggerName.js';
import '../Content/src/triggers/modal/triggerLetterKeys/ModalTriggerLetterKeysCtrl.js';

import '../Content/src/triggers/components/triggerEdit/triggerEditCtrl.js';
import '../Content/src/triggers/components/triggerEdit/triggerEdit.component.js';

import '../Content/src/triggers/components/triggerActionSendRequest/triggerActionSendRequestCtrl.js';
import '../Content/src/triggers/components/triggerActionSendRequest/triggerActionSendRequest.component.js';

import '../Content/src/triggers/components/triggerActionSendNotification/triggerActionSendNotificationCtrl.js';
import '../Content/src/triggers/components/triggerActionSendNotification/triggerActionSendNotification.component.js';

import '../Content/src/triggers/components/triggerActionEditField/triggerActionEditFieldCtrl.js';
import '../Content/src/triggers/components/triggerActionEditField/triggerActionEditField.component.js';

import '../Content/src/design/styles/design.scss';
import '../Content/src/design/design.js';
import '../Content/src/design/design.service.js';
appDependency.addItem('design');

import '../Content/src/settings/styles/settings.scss';
import '../Content/src/settings/settings.js';
appDependency.addItem('settings');

import '../Content/src/settings/settings.Users.js';
appDependency.addItem('settingsUsers');

import '../Content/src/settings/settings.ApiWebhooks.js';
appDependency.addItem('settingsApiWebhooks');

import '../Content/src/settings/modal/addEdit301Red/ModalAddEdit301RedCtrl.js';
import '../Content/src/settings/modal/addEditApiWebhook/addEditApiWebhook.js';
import '../Content/src/settings/modal/addEditDepartment/ModalAddEditDepartmentCtrl.js';
import '../Content/src/settings/modal/addEditManagerRole/ModalAddEditManagerRoleCtrl.js';
import '../Content/src/settings/modal/addEditUser/ModalAddEditUserCtrl.js';
import '../Content/src/settings/modal/changeUserPassword/ModalChangeUserPasswordCtrl.js';
import '../Content/src/settings/modal/clearData/ModalClearDataCtrl.js';
import '../Content/src/settings/modal/editUserRoleActions/ModalEditUserRoleActionsCtrl.js';
import '../Content/src/settings/modal/import301Red/ModalImport301RedCtrl.js';

import '../Content/src/category/styles/category.scss';
import '../Content/src/category/category.js';
appDependency.addItem('category');

import '../Content/src/warehouse/warehouse.js';
appDependency.addItem('warehouse');

import warehouseCitiesModule from '../Content/src/warehouse/components/warehouseCities/warehouseCities.module.js';
appDependency.addItem(warehouseCitiesModule);

import '../Content/src/warehouse/modal/selectWarehouse/modalSelectWarehouse.ctrl.js';

import '../Content/src/category/components/catProductRecommendations.js';
appDependency.addItem('catProductRecommendations');

import '../Content/src/category/modal/changeParentCategory/ModalChangeParentCategoryCtrl.js';
import '../Content/src/category/modal/addRecomProperty/ModalAddRecomPropertyCtrl.js';
import '../Content/src/category/modal/addPropertyGroup/ModalAddPropertyGroupCtrl.js';

// import '../Content/src/order/styles/orders.scss';
// import '../Content/src/order/order.js';
// appDependency.addItem('order');

// import '../Content/src/order/components/orderHistory/orderHistory.js';
// appDependency.addItem('orderHistory');
//
// import '../Content/src/order/components/orderStatusHistory/orderStatusHistory.js';
// appDependency.addItem('orderStatusHistory');

import '../Content/src/tasks/styles/tasks.scss';
import '../Content/src/tasks/tasks.js';
import '../Content/src/tasks/tasks.service.js';
import '../Content/src/tasks/task-create.ctrl.js';
import '../Content/src/tasks/task-create.component.js';
appDependency.addItem('tasks');

import '../Content/src/tasks/modal/completeTask/ModalCompleteTaskCtrl.js';
import '../Content/src/tasks/modal/editTask/ModalEditTaskCtrl.js';
import '../Content/src/tasks/modal/changeTaskGroup/ModalChangeTaskGroupCtrl.js';
import '../Content/src/tasks/modal/changeTaskStatuses/ModalChangeTaskStatusesCtrl.js';
import '../Content/src/tasks/modal/finishTask/ModalFinishTaskCtrl.js';
import '../Content/src/tasks/modal/changeTaskStatus/ModalChangeTaskStatusCtrl.js';

import '../Content/src/warehouseTypes/warehouseTypes.module.js';
appDependency.addItem('warehouseTypes');

import '../Content/src/warehouseTypes/modal/AddEditTypeWarehouse/modalAddEditTypeWarehouse.ctrl.js';
import '../Content/src/warehouseTypes/modal/selectTypeWarehouse/modalSelectTypeWarehouse.ctrl.js';

import stockLabelModule from '../Content/src/stockLabel/stockLabel.module.js';
appDependency.addItem(stockLabelModule);

import '../Content/src/stockLabel/modal/AddEditStockLabel/modalAddEditStockLabel.ctrl.js';
import '../Content/src/domainGeoLocation/modal/AddEditDomainGeoLocation/modalAddEditDomainGeoLocation.ctrl.js';

import domainGeoLocationModule from '../Content/src/domainGeoLocation/domainGeoLocation.module.js';
appDependency.addItem(domainGeoLocationModule);

import '../Content/src/analyticsReport/styles/analytics-report.scss';
import '../Content/src/analyticsReport/analyticsReport.js';
appDependency.addItem('analyticsReport');

import '../Content/src/analyticsReport/components/abcxyzAnalysis/abcxyzAnalysis.js';
import '../Content/src/analyticsReport/components/avgcheck/avgcheck.js';
import '../Content/src/analyticsReport/components/bonus/bonus.js';
import '../Content/src/analyticsReport/components/emailingsWith/emailingsWith.js';
import '../Content/src/analyticsReport/components/emailingsWithout/emailingsWithout.js';
import '../Content/src/analyticsReport/components/export-products/exportProducts.js';
import '../Content/src/analyticsReport/components/export-orders/exportOrders.js';
import '../Content/src/analyticsReport/components/managers-report/managersReport.js';
import '../Content/src/analyticsReport/components/ordersAnalysis/ordersAnalysis.js';
import '../Content/src/analyticsReport/components/productReport/productReport.js';
import '../Content/src/analyticsReport/components/offerReport/offerReport.js';
import '../Content/src/analyticsReport/components/profit/profit.js';
import '../Content/src/analyticsReport/components/rfm/rfm.js';
import '../Content/src/analyticsReport/components/searchRequests/searchRequests.js';
import '../Content/src/analyticsReport/components/telephony/telephony.js';
import '../Content/src/analyticsReport/components/telephonyCallLog/telephonyCallLog.js';
import '../Content/src/analyticsReport/components/vortex/vortex.js';

import '../Content/src/booking/styles/booking.scss';
import '../Content/src/booking/booking.js';
import '../Content/src/booking/booking.service.js';
appDependency.addItem('booking');

import '../Content/src/bookingJournal/bookingJournal.js';
appDependency.addItem('bookingJournal');

import '../Content/src/bookingJournal/modal/addUpdateBooking/ModalAddUpdateBookingCtrl.js';

import '../Content/src/bookingJournal/modal/addUpdateBooking/components/bookingInfo/bookingInfo.js';
appDependency.addItem('bookingInfo');

import '../Content/src/bookingJournal/modal/addUpdateBooking/components/customer/customer.js';
appDependency.addItem('bookingCustomer');

import '../Content/src/bookingJournal/modal/addUpdateBooking/components/itemsSummary/itemsSummary.js';
appDependency.addItem('bookingItemsSummary');

import '../Content/src/bookingJournal/modal/addUpdateBooking/modal/payments/payments.js';
appDependency.addItem('bookingItemsSummary');

import '../Content/src/bookingJournal/modal/reservationResourceSheduler/styles/reservation-resource-sheduler.css';
import '../Content/src/bookingJournal/modal/reservationResourceSheduler/ModalReservationResourceShedulerCtrl.js';
import '../Content/src/bookingJournal/modal/bookingsViewGrid/ModalBookingsViewGrid.js';

import '../Content/src/_shared/booking-sheduler/styles/booking-sheduler.css';
import '../Content/src/_shared/booking-sheduler/booking-sheduler.js';
import '../Content/src/_shared/booking-sheduler/booking-sheduler.ctrl.js';
import '../Content/src/_shared/booking-sheduler/booking-sheduler.component.js';
appDependency.addItem('bookingSheduler');

import '../Content/src/_shared/booking-sheduler-days/booking-sheduler-days.js';
import '../Content/src/_shared/booking-sheduler-days/booking-sheduler-days.ctrl.js';
import '../Content/src/_shared/booking-sheduler-days/booking-sheduler-days.component.js';
appDependency.addItem('bookingShedulerDays');

import '../Content/src/bookingAnalytics/styles/booking-analytics.scss';
import '../Content/src/bookingAnalytics/bookingAnalytics.js';
import '../Content/src/bookingAnalytics/components/common/common.js';
import '../Content/src/bookingAnalytics/components/services/services.js';
import '../Content/src/bookingAnalytics/components/paymentMethods/paymentMethods.js';
import '../Content/src/bookingAnalytics/components/sources/sources.js';
import '../Content/src/bookingAnalytics/components/turnover/turnover.js';
import '../Content/src/bookingAnalytics/components/reservationResources/reservationResources.js';
appDependency.addItem('bookingAnalytics');

import '../Content/src/bookingAnalytics/modal/services/services.js';

import '../Content/src/menus/menus.js';
import '../Content/src/menus/components/menu-item-actions/styles/menu-item-actions.scss';
import '../Content/src/menus/components/menu-item-actions/menuItemActions.js';

import '../Content/src/menus/components/menu-treeview/menuTreeview.js';
import '../Content/src/menus/components/menu-treeview/menuTreeview.component.js';

appDependency.addItem('menus');

import '../Content/src/dashboardSites/styles/dashboardSites.css';
import '../Content/src/dashboardSites/dashboardSites.js';
appDependency.addItem('dashboardSites');

import '../Content/src/dashboardSites/components/createSite/createSite.js';
appDependency.addItem('createSite');

import '../Content/src/dashboardSites/modals/qrCodeGenerator/ModalQrCodeGeneratorCtrl.js';

import '../../../scripts/_partials/submenu/submenu.module.js';
appDependency.addItem('submenu');

import '../../../scripts/_common/countdown/countdown.module.js';

import '../../../scripts/_common/modal/modal.module.js';
appDependency.addItem('modal');

import '../Content/src/_shared/notification-message/styles/styles.css';
import '../Content/src/_shared/notification-message/notificationMessage.js';
import '../Content/src/_shared/notification-message/directives/notificationMessage.directive.js';
appDependency.addItem('notificationMessage');

//ниже стиль для страницы settingStoreDashboard
import '../Content/styles/views/settingStoreDashboard.scss';

import '../Content/styles/views/settingSeoRobots.scss';

/*-------js------*/
import '../../../vendors/autofocus/autofocus.js';
appDependency.addItem('autofocus');

import '../../../scripts/_common/urlHelper/urlHelperService.module.js';
appDependency.addItem('urlHelper');

import '../../../scripts/_partials/zone/zone.js';
appDependency.addItem('zone');

import '../../../scripts/_common/input/input.module.js';
appDependency.addItem('input');

import '../../../scripts/_common/dom/dom.module.js';
appDependency.addItem('dom');

import '../../../scripts/_common/select/select.module.js';
appDependency.addItem('select');

import '../../../scripts/_common/window/window.module.js';
appDependency.addItem('windowExt');

import yandexMapsModule from '../../../scripts/_common/yandexMaps/yandexMaps.module.js';
appDependency.addItem(yandexMapsModule);

import '../Content/vendors/ngSelectable/ngSelectable.js';
appDependency.addItem('ngSelectable');

import checklistModelModule from 'checklist-model';
appDependency.addItem(checklistModelModule);

import ngFileUploadModule from 'ng-file-upload';
appDependency.addItem(ngFileUploadModule);

import angularInviewModule from 'angular-inview';
appDependency.addItem(angularInviewModule);

import '../Content/src/_shared/validation/validation.js';
import '../Content/src/_shared/validation/directives/validationDirectives.js';
import '../Content/src/_shared/validation/filters/validationFilters.js';
appDependency.addItem('validation');

import '../Content/src/_shared/adv-tracking/advTracking.js';
import '../Content/src/_shared/adv-tracking/advTracking.service.js';
appDependency.addItem('advTracking');

import angularScrollModule from 'angular-scroll';
appDependency.addItem(angularScrollModule);

import '../Content/vendors/angular-sticky/angular-sticky.custom.js';
appDependency.addItem('sticky');

import '../../../vendors/angular-bind-html-compile/angular-bind-html-compile.js';
appDependency.addItem('angular-bind-html-compile');

import { Dropdown } from 'bootstrap';

import popoverModule from 'angular-ui-bootstrap/src/popover/index.js';
appDependency.addItem(popoverModule);
import tabsModule from 'angular-ui-bootstrap/src/tabs/index.js';
appDependency.addItem(tabsModule);
import paginationModule from 'angular-ui-bootstrap/src/pagination/index.js';
appDependency.addItem(paginationModule);
import modalModule from 'angular-ui-bootstrap/src/modal/index.js';
appDependency.addItem(modalModule);
import typeaheadModule from 'angular-ui-bootstrap/src/typeahead/index.js';
appDependency.addItem(typeaheadModule);
import dropdownModule from 'angular-ui-bootstrap/src/dropdown/index.js';
appDependency.addItem(dropdownModule);
import progressbarModule from 'angular-ui-bootstrap/src/progressbar/index.js';
appDependency.addItem(progressbarModule);
import '../Content/vendors/ui-bootstrap/angular-tab-decorator/angular-tab-decorator.js';
import '../Content/vendors/ui-bootstrap/angular-typehead-decorator/angular-typehead-decorator.js';

import '../Content/vendors/year-calendar/yearCalendar.directive.js';
appDependency.addItem('ngYearCalendar');

import '../Content/vendors/fullcalendar/ng-fullcalendar.directive.custom.js';
appDependency.addItem('angular-fullcalendar');

import '../Content/vendors/ng-textcomplete/ng-textcomplete.js';
import '../Content/vendors/ng-textcomplete/ng-textcomplete.ext.directive.js';
appDependency.addItem('ngTextcomplete');

import '../Content/vendors/angular-input-modified/angular-input-modified.custom.js';
appDependency.addItem('ngInputModified');

import 'angular-timeago/dist/angular-timeago.min.js';
appDependency.addItem('yaru22.angular-timeago');

import '../Content/vendors/dragscroll/dragscroll.js';

import '../Content/vendors/highlight/highlight.angular.js';
appDependency.addItem('highlight');

import '../Content/src/_shared/cm-stat/cmStat.js';
import '../Content/src/_shared/cm-stat/cmStat.service.js';
import '../Content/src/_shared/cm-stat/cmStat.component.js';
appDependency.addItem('cmStat');

import '../Content/src/_shared/saas-stat/saasStat.js';
import '../Content/src/_shared/saas-stat/saasStat.service.js';
import '../Content/src/_shared/saas-stat/saasStat.component.js';
appDependency.addItem('saasStat');

import lozadAdvModule from '../../../scripts/_common/lozad-adv/lozadAdv.module.js';
appDependency.addItem(lozadAdvModule);

import '../Content/src/_shared/modal/addLandingSite/addProductLanding.directive.js';
appDependency.addItem('addProductLanding');

import '../Content/src/_shared/modal/addLandingSite/addOfferLanding.directive.js';
appDependency.addItem('addOfferLanding');

import '../Content/src/_shared/modal/salesChannels/ModalSalesChannelsCtrl.js';
appDependency.addItem('modalSalesChannels');

import '../Content/src/_shared/modal/salesChannelExcluded/ModalSalesChannelExcludedCtrl.js';
import '../Content/src/_shared/modal/addCategory/ModalAddCategoryCtrl.js';
import '../Content/src/_shared/modal/addCategoryList/ModalAddCategoryListCtrl.js';
import '../Content/src/_shared/modal/addProduct/ModalAddProductCtrl.js';
import '../Content/src/_shared/modal/addProductList/ModalAddProductListCtrl.js';
import '../Content/src/_shared/modal/addTask/ModalAddTaskCtrl.js';
import '../Content/src/_shared/modal/addLead/ModalAddLeadCtrl.js';
import '../Content/src/_shared/modal/addBrand/ModalAddBrandCtrl.js';
import '../Content/src/_shared/modal/addAvatar/ModalAddAvatarCtrl.js';
import '../Content/src/_shared/modal/cropImage/ModalCropImageCtrl.js';
import '../Content/src/_shared/modal/products-selectvizr/ModalProductsSelectvizrCtrl.js';
import '../Content/src/_shared/modal/export-products-selectvizr/ModalExportProductsSelectvizrCtrl.js';
import '../Content/src/_shared/modal/shipping-products-selectvizr/ModalShippingProductsSelectvizrCtrl.js';
import '../Content/src/_shared/modal/offers-selectvizr/ModalOffersSelectvizrCtrl.js';
import '../Content/src/_shared/modal/moveProductInOtherCategory/ModalMoveProductInOtherCategoryCtrl.js';
import '../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';
import '../Content/src/_shared/modal/selectCustomer/ModalSelectCustomerCtrl.js';
import '../Content/src/_shared/modal/selectCity/ModalSelectCityCtrl.js';
import '../Content/src/_shared/modal/selectCities/ModalSelectCitiesCtrl.js';
import '../Content/src/_shared/modal/selectPartner/ModalSelectPartnerCtrl.js';
import '../Content/src/_shared/modal/sendLetterToCustomer/ModalSendLetterToCustomerCtrl.js';
import '../Content/src/_shared/modal/selectNews/ModalSelectNewsCtrl.js';
import '../Content/src/_shared/modal/selectStaticPage/ModalSelectStaticPageCtrl.js';
import '../Content/src/_shared/modal/search/ModalSearchCtrl.js';
import '../Content/src/_shared/modal/bonus/ModalBonus/ModalAddBonusCtrl.js';
import '../Content/src/_shared/modal/bonus/ModalBonus/ModalSubtractBonusCtrl.js';
import '../Content/src/_shared/modal/bonus/NotificationTemplate/ModalAddEditNotificationTemplateCtrl.js';
import '../Content/src/_shared/modal/bonus/Rule/ModalListRulesCtrl.js';
import '../Content/src/_shared/modal/bonus/Cards/ModalAddCardCtrl.js';
import '../Content/src/_shared/modal/bonus/Cards/ModalImportCardsCtrl.js';
import '../Content/src/_shared/modal/changeAdminShopName/ModalChangeAdminShopNameCtrl.js';
import '../Content/src/_shared/modal/sendSocialMessage/ModalSendSocialMessageCtrl.js';
import '../Content/src/_shared/modal/sendSms/ModalSendSmsAdvCtrl.js';
import '../Content/src/_shared/modal/selectSmsTemplate/ModalSelectSmsTemplateCtrl.js';
import '../Content/src/_shared/modal/searchImages/ModalSearchImagesCtrl.js';
import '../Content/src/_shared/modal/addLandingSite/ModalAddLandingSiteCtrl.js';
import '../Content/src/_shared/modal/excludedExportProducts/ModalExcludedExportProductsCtrl.js';
import '../Content/src/_shared/modal/testShippingCalculation/ModalTestShippingCalculationCtrl.js';

import maskModule from '../../../scripts/_common/mask/mask.module.js';
appDependency.addItem(maskModule);

import '../Content/src/_shared/activity/activity.component.js';
import '../Content/src/_shared/activity/controllers/activityEmails.js';
import '../Content/src/_shared/activity/controllers/activitySmses.js';
import '../Content/src/_shared/activity/controllers/activityCalls.js';
import '../Content/src/_shared/activity/controllers/activityActions.js';
appDependency.addItem('activity');

import '../Content/src/_shared/elastic-text/elasticText.directive.js';
appDependency.addItem('elasticText');

import '../Content/src/_shared/autosave-text/autosaveText.component.js';
appDependency.addItem('autosaveText');

import '../Content/src/_shared/statistics/statistics.js';
import '../Content/src/_shared/statistics/statistics.service.js';
appDependency.addItem('statistics');

import '../Content/src/_shared/recalc/recalc.js';
appDependency.addItem('recalc');

import '../Content/src/_shared/url-generator/urlGenerator.js';
appDependency.addItem('urlGenerator');

import '../Content/src/_shared/autocompleter/autocompleter.js';
appDependency.addItem('autocompleter');

import '../Content/src/_shared/post-message/doPostMessage.js';

import '../Content/src/_shared/vk-messages/vkMessages.js';
appDependency.addItem('vkMessages');

import '../Content/src/_shared/instagram-messages/instagramMessages.js';
appDependency.addItem('instagramMessages');

import '../Content/src/_partials/change-admin-shop-name/changeAdminShopName.js';
appDependency.addItem('changeAdminShopName');

import '../Content/src/_partials/customer-fields/customerFields.component.js';
appDependency.addItem('customerFields');

import '../Content/src/_partials/lead-fields/leadFields.component.js';
appDependency.addItem('leadFields');

import '../Content/src/_partials/products-selectvizr/productsSelectvizr.js';
import '../Content/src/_partials/products-selectvizr/productsSelectvizr.component.js';
appDependency.addItem('productsSelectvizr');

import '../Content/src/_partials/offers-selectvizr/offersSelectvizr.js';
import '../Content/src/_partials/offers-selectvizr/offersSelectvizr.component.js';
appDependency.addItem('offersSelectvizr');

import '../Content/src/_partials/booking-services-selectvizr/bookingServicesSelectvizr.js';
import '../Content/src/_partials/booking-services-selectvizr/bookingServicesSelectvizr.component.js';
appDependency.addItem('bookingServicesSelectvizr');

import '../Content/src/_partials/partner-info/partnerInfo.js';
import '../Content/src/_partials/partner-info/partnerInfo.component.js';
import '../Content/src/_partials/partner-info/partnerInfoTrigger.js';
import '../Content/src/_partials/partner-info/partnerInfoTrigger.component.js';
import '../Content/src/_partials/partner-info/partnerInfoContainer.js';
import '../Content/src/_partials/partner-info/partnerInfo.service.js';
appDependency.addItem('partnerInfo');

import sidebarMenuModule from '../Content/src/_partials/sidebar-menu/sidebarMenu.module.js';
appDependency.addItem(sidebarMenuModule);

import '../Content/src/_partials/tasks-grid/tasksGrid.js';
import '../Content/src/_partials/tasks-grid/tasksGrid.component.js';
appDependency.addItem('tasksGrid');

import '../Content/src/_partials/collapse-tab/collapseTab.js';
import '../Content/src/_partials/collapse-tab/Controllers/collapseTabCtrl.js';
import '../Content/src/_partials/collapse-tab/Directive/collapseTabDirective.js';
appDependency.addItem('collapseTab');

import '../Content/src/_partials/top-panel-user/topPanelUser.js';
appDependency.addItem('topPanelUser');

import '../Content/src/_partials/change-history/changeHistory.js';
import '../Content/src/_partials/change-history/changeHistory.component.js';
appDependency.addItem('changeHistory');

import '../Content/src/_partials/saasWarningMessage/styles.scss';
import '../Content/src/_partials/saasWarningMessage/saasWarningMessage.js';
appDependency.addItem('saasWarningMessage');

import '../Content/src/carousel/carousel.js';
appDependency.addItem('carouselPage');

import '../Content/src/carousel/modal/addEditCarousel/ModalAddEditCarouselCtrl.js';

import '../Content/src/certificates/certificates.js';
appDependency.addItem('certificates');

import '../Content/src/certificates/modal/addEditCertificates/ModalAddEditCertificatesCtrl.js';
import '../Content/src/certificates/modal/certificateSettings/ModalCertificateSettingsCtrl.js';

import '../Content/src/calls/components/callRecord/callRecord.js';
appDependency.addItem('callRecord');

import '../Content/src/files/files.js';
appDependency.addItem('files');

import '../Content/src/mailSettings/mailSettings.js';
appDependency.addItem('mailSettings');

import '../Content/src/mailSettings/modal/addEditMailFormat/ModalAddEditMailFormatCtrl.js';
import '../Content/src/mailSettings/modal/updateAddress/ModalEmailSettingsUpdateAddressCtrl.js';
import '../Content/src/mailSettings/modal/addEditMailAnswerTemplate/ModalAddEditMailAnswerTemplateCtrl.js';
import '../Content/src/mailSettings/modal/addEditSmsAnswerTemplate/ModalAddEditSmsAnswerTemplateCtrl.js';

import '../Content/src/settingsSms/settingsSms.js';
appDependency.addItem('settingsSms');

import '../Content/src/settingsAuthCall/settingsAuthCall.js';
appDependency.addItem('settingsAuthCall');

import '../Content/src/settingsTwoFactorAuth/settingsTwoFactorAuth.js';
appDependency.addItem('settingsTwoFactorAuth');

import '../Content/src/settingsSms/modal/addEditSmsTemplateOnOrderChanging/ModalAddEditSmsTemplateOnOrderChangingCtrl.js';

import '../Content/src/settingsCheckout/settingsCheckout.js';
appDependency.addItem('settingsCheckout');

import '../Content/src/settingsCheckout/modal/addEditTax/ModalAddEditTaxCtrl.js';
import '../Content/src/settingsCheckout/modal/addEditWorkingTimes/addEditWorkingTimesCtrl.js';
import '../Content/src/settingsCheckout/modal/addEditAdditionalWorkingTime/ModalAddEditAdditionalWorkingTimeCtrl.js';

import '../Content/src/settingsCheckout/components/thankYouPageProducts/thankYouPageProducts.js';
appDependency.addItem('thankYouPageProducts');

import '../Content/src/settingsCoupons/settingsCoupons.js';
import '../Content/src/settingsCoupons/settingsCoupons.service.js';
appDependency.addItem('settingsCoupons');

import '../Content/src/settingsCatalog/settingsCatalog.js';
appDependency.addItem('settingsCatalog');

import '../Content/src/_shared/export-customers/exportCustomers.js';
appDependency.addItem('exportCustomers');

import '../Content/src/settingsCatalog/modal/addEditCurrency/ModalAddEditCurrencyCtrl.js';
import '../Content/src/settingsCatalog/modal/resultPriceRegulation/ResultPriceRegulationCtrl.js';
import '../Content/src/settingsCatalog/modal/resultCategoryDiscountRegulation/ResultCategoryDiscountRegulationCtrl.js';
import '../Content/src/settingsCustomers/settingsCustomers.js';
import '../Content/src/settingsCustomers/customerFields.service.js';
import '../Content/src/settingsCustomers/customerFieldValues.service.js';
appDependency.addItem('settingsCustomers');

import '../Content/src/settingsCustomers/modal/addEditCustomerField/ModalAddEditCustomerFieldCtrl.js';
import '../Content/src/settingsCustomers/modal/addEditCustomerFieldValue/ModalAddEditCustomerFieldValueCtrl.js';

import '../Content/src/settingsSeo/settingsSeo.js';
appDependency.addItem('settingsSeo');

import '../Content/src/settingsMobile/settingsMobile.js';
appDependency.addItem('settingsMobile');

import '../Content/src/settingsSystem/settingsSystem.js';

import '../Content/src/settingsSystem/location/settingsSystem.location.js';
import '../Content/src/settingsSystem/location/settingsSystem.location.component.js';
import '../Content/src/settingsSystem/location/settingsSystem.location.country.js';
import '../Content/src/settingsSystem/location/settingsSystem.location.country.component.js';
import '../Content/src/settingsSystem/location/settingsSystem.location.region.js';
import '../Content/src/settingsSystem/location/settingsSystem.location.region.component.js';
import '../Content/src/settingsSystem/location/settingsSystem.location.city.js';
import '../Content/src/settingsSystem/location/settingsSystem.location.city.component.js';

import '../Content/src/settingsSystem/logs/logs.scss';
import '../Content/src/settingsSystem/logs/settingsSystem.logs.js';
import '../Content/src/settingsSystem/logs/settingsSystem.logs.component.js';
import '../Content/src/settingsSystem/logs/settingsSystem.logs.service.js';

appDependency.addItem('settingsSystem');

import '../Content/src/settingsSystem/location/modal/addEditCountry/ModalAddEditCountryCtrl.js';
import '../Content/src/settingsSystem/location/modal/addEditRegion/ModalAddEditRegionsCtrl.js';
import '../Content/src/settingsSystem/location/modal/addEditRegion/modal/ModalAddEditAdditionalSettingsRegionCtrl.js';
import '../Content/src/settingsSystem/location/modal/addEditCitys/ModalAddEditCitysCtrl.js';
import '../Content/src/settingsSystem/location/modal/addEditCitys/modal/ModalAddEditAdditionalSettingsCityCtrl.js';
import '../Content/src/settingsSystem/modal/addEditLocalization/ModalAddEditLocalizationCtrl.js';

import '../Content/src/settingsPartners/settingsPartners.js';
appDependency.addItem('settingsPartners');

import '../Content/src/settingsPartners/modal/editPaymentTypeName/ModalEditPaymentTypeNameCtrl.js';
import '../Content/src/settingsPartners/modal/editRewardPercent/ModalEditRewardPercentCtrl.js';

import '../Content/src/settingsSocial/settingsSocial.js';
appDependency.addItem('settingsSocial');

import '../Content/src/settingsTelephony/settingsTelephony.js';
import '../Content/src/settingsTelephony/settingsTelephonyService.js';
appDependency.addItem('settingsTelephony');

import '../Content/src/settingsWarehouses/settingsWarehouses.js';
appDependency.addItem('settingsWarehouses');

import '../Content/src/settingsCrm/settingsCrm.js';
import '../Content/src/settingsCrm/settingsCrm.service.js';
appDependency.addItem('settingsCrm');

import '../Content/src/settingsCrm/components/facebookAuth/facebookAuth.js';
import '../Content/src/settingsCrm/components/facebookAuth/facebookAuthService.js';
appDependency.addItem('facebookAuth');

import '../Content/src/settingsCrm/components/salesFunnels/salesFunnels.js';
appDependency.addItem('salesFunnels');

import '../Content/src/settingsCrm/components/salesFunnels/modals/addEditSalesFunnel/ModalAddEditSalesFunnelCtrl.js';

import '../Content/src/settingsCrm/components/dealStatuses/dealStatuses.js';
appDependency.addItem('dealStatuses');

import '../Content/src/settingsCrm/components/dealStatuses/modals/editDealStatus/ModalEditDealStatusCtrl.js';

import '../Content/src/settingsCrm/components/integrationsLimit/integrationsLimit.js';
appDependency.addItem('integrationsLimit');

import '../Content/src/settingsCrm/components/leadFieldsList/leadFieldsList.js';
import '../Content/src/settingsCrm/components/leadFieldsList/leadFields.service.js';
import '../Content/src/settingsCrm/components/leadFieldsList/modals/addEditLeadField/ModalAddEditLeadFieldCtrl.js';
appDependency.addItem('leadFieldsList');

import '../Content/src/settingsBonus/settingsBonus.js';
appDependency.addItem('settingsBonus');

import '../Content/src/settingsTemplatesDocx/settingsTemplatesDocx.js';
import '../Content/src/settingsTemplatesDocx/modal/addEditTemplate/addEditTemplate.js';
import '../Content/src/settingsTemplatesDocx/modal/DescriptionTemplate/descriptionTemplate.js';
appDependency.addItem('settingsTemplatesDocx');

import '../Content/src/settingsBooking/settingsBooking.js';
appDependency.addItem('settingsBooking');

import '../Content/src/settingsBooking/modal/addEditTag/ModalAddEditTagCtrl.js';

import '../Content/src/shippingReplaceGeo/shippingReplaceGeo.js';
appDependency.addItem('shippingReplaceGeo');

import '../Content/src/shippingReplaceGeo/modal/addEdit/addEdit.js';

import '../Content/src/orders/orders.js';
appDependency.addItem('orders');

import '../Content/src/orders/modal/ModalChangeOrderStatusesCtrl.js';

import '../Content/src/order/styles/orders.scss';
import '../Content/src/order/order.js';
appDependency.addItem('order');

import '../Content/src/order/components/orderStatusHistory/orderStatusHistory.js';
appDependency.addItem('orderStatusHistory');

import '../Content/src/order/components/orderItemsSummary/orderItemsSummary.scss';
import '../Content/src/order/components/orderItemsSummary/orderItemsSummary.js';
appDependency.addItem('orderItemsSummary');

import '../Content/src/order/components/orderHistory/orderHistory.js';
appDependency.addItem('orderHistory');

import '../Content/src/order/modal/shippings/ModalShippingsCtrl.js';
import '../Content/src/order/modal/shippingsTime/ModalShippingsTimeCtrl.js';
import '../Content/src/order/modal/sendOrderGrastin/ModalSendOrderGrastinCtrl.js';
import '../Content/src/order/modal/sendOrderGrastin/ModalSendRequestGrastinForIntakeCtrl.js';
import '../Content/src/order/modal/sendOrderGrastin/ModalSendRequestGrastinForActCtrl.js';
import '../Content/src/order/modal/payments/ModalPaymentsCtrl.js';
import '../Content/src/order/modal/changeOrderStatus/ModalChangeOrderStatusCtrl.js';
import '../Content/src/order/modal/sendBillingLink/ModalSendBillingLinkCtrl.js';
import '../Content/src/order/modal/getBillingLink/ModalGetBillingLinkCtrl.js';
import '../Content/src/order/modal/changeOrderCustomer/changeOrderCustomer.js';
import '../Content/src/order/modal/changeOrderCustomerAddress/changeOrderCustomerAddress.js';
import '../Content/src/order/modal/desktopAppNotification/desktopAppNotification.js';
import '../Content/src/order/modal/shippings/russianPost/customsDeclarationProductData/modalCustomsDeclarationProductDataCtrl.js';
import '../Content/src/order/modal/shippings/yandex/changeDeliveryDate/modalYandexChangeDeliveryDateCtrl.js';
import '../Content/src/order/modal/shippings/sdek/downloadBarCodeOrder/modalSdekDownloadBarCodeOrderCtrl.js';
import '../Content/src/order/modal/editCustomOptions/ModalEditCustomOptionsCtrl.js';
import '../Content/src/order/modal/changeMarking/ModalChangeMarkingCtrl.js';
import '../Content/src/order/modal/changeAddress/ModalChangeOrderAddressCtrl.js';
import '../Content/src/order/modal/changeOrderRecipient/ModalChangeOrderRecipient.js';
import '../Content/src/order/modal/distributionOfOrderItem/modalDistributionOfOrderItemCtrl.js';

import '../Content/src/orderstatuses/orderstatuses.js';
appDependency.addItem('orderstatuses');

import '../Content/src/orderstatuses/modal/ModalAddEditOrderStatusCtrl.js';

import '../Content/src/ordersources/ordersources.js';
appDependency.addItem('ordersources');

import '../Content/src/orderReviews/orderReviews.js';
appDependency.addItem('orderReviews');

import '../Content/src/order/components/orderItemCustomOptions/styles/orderItemCustomOptions.scss';
import '../Content/src/order/components/orderItemCustomOptions/orderItemCustomOptions.module.js';
import '../Content/src/order/components/orderItemCustomOptions/controllers/orderItemCustomOptions.controller.js';
import '../Content/src/order/components/orderItemCustomOptions/directives/orderItemCustomOptions.directive.js';
import '../Content/src/order/components/orderItemCustomOptions/services/orderItemCustomOptions.service.js';
appDependency.addItem('orderItemCustomOptions');

import '../Content/src/ordersources/modal/ModalAddEditOrderSourceCtrl.js';

import '../Content/src/customers/customers.js';
appDependency.addItem('customers');

import '../Content/src/customergroups/customergroups.js';
appDependency.addItem('customergroups');

import '../Content/src/customergroups/modal/addCustomerGroup/ModalAddCustomerGroupCtrl.js';
import '../Content/src/customergroups/modal/addEditCustomerGroupCategoryDiscount/ModalAddEditCustomerGroupCategoryDiscountCtrl.js';

import '../Content/src/customerSegment/customerSegment.js';
appDependency.addItem('customerSegment');

import '../Content/src/customerSegments/customerSegments.js';
appDependency.addItem('customerSegments');

import '../Content/src/category/category.js';
appDependency.addItem('category');

import '../Content/src/category/components/catProductRecommendations.js';
appDependency.addItem('catProductRecommendations');

import '../Content/src/category/modal/addPropertyGroup/ModalAddPropertyGroupCtrl.js';
import '../Content/src/category/modal/changeParentCategory/ModalChangeParentCategoryCtrl.js';
import '../Content/src/category/modal/addRecomProperty/ModalAddRecomPropertyCtrl.js';

import '../Content/src/informers/informers.js';
appDependency.addItem('informers');

import '../Content/src/_shared/change-history-modal/ModalChangeHistoryCtrl.js';

import '../Content/src/settingsTasks/settingsTasks.js';
appDependency.addItem('settingsTasks');

import '../Content/src/settingsTasks/modal/addEditRule/ModalAddEditRuleCtrl.js';
import '../Content/src/settingsTasks/modal/addEditFilterRule/ModalAddEditFilterRuleCtrl.js';
import '../Content/src/settingsTasks/modal/addEditManagerFilterRule/ModalAddEditManagerFilterRuleCtrl.js';

import '../Content/src/settingsSearch/settingsSearch.js';
import '../Content/src/settingsSearch/modal/addEditSettingsSearch/ModalAddEditSettingsSearchCtrl.js';
appDependency.addItem('settingsSearch');

import '../Content/src/subscription/subscription.js';
appDependency.addItem('subscription');

import '../Content/src/taskgroups/taskgroups.js';
import '../Content/src/taskgroups/projectStatuses/projectStatuses.js';
import '../Content/src/taskgroups/projectStatuses/modal/ModalEditProjectStatusCtrl.js';
appDependency.addItem('taskgroups');

import '../Content/src/taskgroups/modal/ModalAddEditTaskGroupCtrl.js';
import '../Content/src/taskgroups/modal/copyTaskGroup/ModalCopyTaskGroupCtrl.js';

import '../Content/src/templateDocx/templateDocx.js';
import '../Content/src/templateDocx/templateDocx.service.js';
appDependency.addItem('templateDocx');

import '../Content/src/bookingAffiliate/bookingAffiliate.js';
import '../Content/src/bookingAffiliate/bookingAffiliate.service.js';
appDependency.addItem('bookingAffiliate');

import '../Content/src/bookingAffiliate/modals/addAffiliate/ModalAddAffiliateCtrl.js';
import '../Content/src/bookingAffiliate/modals/addUpdateAdditionalTime/ModalAddUpdateAdditionalTimeCtrl.js';
import '../Content/src/bookingAffiliate/modals/addUpdateSmsTemplate/addUpdateSmsTemplate.js';

import '../Content/src/bookingAffiliateSettings/bookingAffiliateSettings.js';
appDependency.addItem('bookingAffiliateSettings');

import '../Content/src/bookingReservationResources/bookingReservationResources.js';
appDependency.addItem('bookingReservationResources');

import '../Content/src/bookingReservationResources/components/listOfReservationResourceServices/listOfReservationResourceServices.js';
appDependency.addItem('listOfReservationResourceServices');

import '../Content/src/bookingReservationResources/components/listAdditionalTime/listReservationResourceAdditionalTime.js';
appDependency.addItem('listReservationResourceAdditionalTime');

import '../Content/src/bookingReservationResources/modal/addUpdateReservationResource/ModalAddUpdateReservationResourceCtrl.js';
import '../Content/src/bookingReservationResources/modal/addUpdateAdditionalTime/ModalAddUpdateAdditionalTimeCtrl.js';

import '../Content/src/bookingCategories/bookingCategories.js';
import '../Content/src/bookingCategories/bookingCategories.service.js';
appDependency.addItem('bookingCategories');

import '../Content/src/bookingCategories/components/listBookingCategories/listBookingCategories.js';
appDependency.addItem('listBookingCategories');

import '../Content/src/bookingCategories/components/bookingCategoriesTreeview/bookingCategoriesTreeview.js';
import '../Content/src/bookingCategories/components/bookingCategoriesTreeview/bookingCategoriesTreeview.component.js';
appDependency.addItem('bookingCategoriesTreeview');

import '../Content/src/bookingCategories/modals/addEditCategory/ModalAddEditBookingCategoryCtrl.js';

import '../Content/src/bookingServices/bookingServices.js';
appDependency.addItem('bookingServices');

import '../Content/src/bookingServices/modals/addUpdateBookingService/ModalAddUpdateBookingServiceCtrl.js';
import '../Content/src/_shared/modal/bookingServicesSelectvizr/ModalBookingServicesSelectvizrCtrl.js';

import '../Content/src/mainpageproducts/mainpageproducts.js';
import '../Content/src/mainpageproducts/mainpageproducts.service.js';
import '../Content/src/mainpageproducts/components/productListsMenu/productListsMenu.js';
import '../Content/src/mainpageproducts/modal/editMainPageList/ModalEditMainPageListCtrl.js';
appDependency.addItem('mainpageproducts');

import '../Content/src/productlists/productlists.js';
import '../Content/src/productlists/modal/addEditProductList/ModalAddEditProductListCtrl.js';
appDependency.addItem('productlists');

import '../Content/src/properties/properties.js';
appDependency.addItem('properties');

import '../Content/src/properties/modal/addEditProperty/ModalAddEditPropertyCtrl.js';
import '../Content/src/properties/modal/changeGroups/ModalChangeGroupsCtrl.js';
import '../Content/src/properties/modal/changePropertyGroup/ModalChangePropertyGroupCtrl.js';
import '../Content/src/properties/modal/addGroup/ModalAddGroupCtrl.js';

import '../Content/src/properties/components/propertyGroups/propertyGroups.js';
appDependency.addItem('propertyGroups');

import '../Content/src/propertyvalues/propertyvalues.js';
appDependency.addItem('propertyvalues');

import '../Content/src/propertyvalues/modal/addPropertyValue/ModalAddPropertyValueCtrl.js';

import '../Content/src/landing/landings.js';
import '../Content/src/landing/landings.service.js';
appDependency.addItem('landings');

import '../Content/src/landing/modal/ModalAddLandingCtrl.js';

import '../Content/src/landingSite/landingSite.js';
import '../Content/src/landingSite/components/funnelEmailSequences/funnelEmailSequences.js';
import '../Content/src/landingSite/components/funnelBookings/funnelBookings.js';
import '../Content/src/landingSite/components/funnelLeads/funnelLeads.js';
import '../Content/src/landingSite/components/funnelOrders/funnelOrders.js';
appDependency.addItem('landingSite');

import '../Content/src/landingSite/modal/editFunnelNameModalCtrl.js';

import '../Content/src/funnelDetails/funnelDetails.js';
appDependency.addItem('funnelDetails');

import '../Content/src/createFunnel/createFunnel.js';
appDependency.addItem('createFunnel');

import '../Content/src/brandsList/brandsList.js';
import '../Content/src/brandsList/brandsList.service.js';
appDependency.addItem('brandsList');

import '../Content/src/warehousesList/warehousesList.js';
appDependency.addItem('warehousesList');

import '../Content/src/brand/brand.js';
appDependency.addItem('brand');

import '../Content/src/fake/checkout/checkout.js';
import '../Content/src/fake/checkout/checkoutService.js';
appDependency.addItem('checkout');

import '../Content/src/exportfeeds/exportfeeds.js';
import '../Content/src/exportfeeds/exportfeeds.service.js';
appDependency.addItem('exportfeeds');

import '../Content/src/_shared/modal/addExportFeed/ModalAddExportFeedCtrl.js';

import '../Content/src/exportfeeds/modal/addYandexGlobalDeliveryCost/ModalAddYandexGlobalDeliveryCostCtrl.js';
import '../Content/src/exportfeeds/modal/addEditYandexPromoFlash/ModalAddEditYandexPromoFlashCtrl.js';
import '../Content/src/exportfeeds/modal/addEditYandexPromoGift/ModalAddEditYandexPromoGiftCtrl.js';
import '../Content/src/exportfeeds/modal/addEditYandexPromoNPlusM/ModalAddEditYandexPromoNPlusMCtrl.js';
import '../Content/src/exportfeeds/modal/addEditYandexPromoCode/ModalAddEditYandexPromoCodeCtrl.js';
import '../Content/src/exportfeeds/modal/addEditAdditionalProperties/ModalAddEditAdditionalPropertiesCtrl.js';

import '../Content/src/exportcategories/exportcategories.js';
import '../Content/src/exportcategories/exportcategories.service.js';
appDependency.addItem('exportCategories');

import '../Content/src/congratulationsDashboard/congratulationsdashboard.js';
appDependency.addItem('congratulationsDashboard');

import '../Content/src/achievements/achievements.js';
appDependency.addItem('achievements');

import '../Content/src/adv-analytics/adv-analytics.js';
import '../Content/src/adv-analytics/adv-analytics.service.js';
appDependency.addItem('analytics');

import '../Content/src/analyticsFilter/analyticsFilter.js';
appDependency.addItem('analyticsFilter');

import '../Content/src/import/import.js';
import '../Content/src/import/import.service.js';
appDependency.addItem('import');

import '../Content/src/design/design.js';
import '../Content/src/design/design.service.js';
appDependency.addItem('design');

import '../Content/src/tariffs/tariffs.js';
appDependency.addItem('tariffs');

import '../Content/src/sizes/sizes.js';
import '../Content/src/sizes/modal/addEditSize/ModalAddEditSizeCtrl.js';
import '../Content/src/sizes/modal/addEditSizeNameForCategories/ModalAddEditSizeNameForCategoriesCtrl.js';
appDependency.addItem('sizes');

import '../Content/src/colors/colors.js';
appDependency.addItem('colors');

import '../Content/src/colors/modal/addEditColor/ModalAddEditColorCtrl.js';
import '../Content/src/colors/modal/importColors/ModalImportColorsCtrl.js';

import '../Content/src/units/units.js';
import '../Content/src/units/modal/addEditUnit/ModalAddEditUnitCtrl.js';
appDependency.addItem('units');

import '../Content/src/tags/tags.js';
appDependency.addItem('tags');

import '../Content/src/grades/grades.js';
appDependency.addItem('grades');

import '../Content/src/grades/modal/addEditGrade/ModalAddEditGradeCtrl.js';

import '../Content/src/cards/cards.js';
appDependency.addItem('cards');

import '../Content/src/notificationtemplates/notificationtemplates.js';
appDependency.addItem('notificationtemplates');

import '../Content/src/rules/rules.js';
appDependency.addItem('rules');

import '../Content/src/newsItem/newsItem.js';
import '../Content/src/newsItem/components/newsProducts/newsProducts.js';
appDependency.addItem('newsItem');

import '../Content/src/news/news.js';
appDependency.addItem('news');

import '../Content/src/newsCategory/newsCategory.js';
appDependency.addItem('newsCategory');

import '../Content/src/newsCategory/modal/addEditNewsCategory/ModalAddEditNewsCategoryCtrl.js';

import '../Content/src/staticBlock/staticBlock.js';
appDependency.addItem('staticBlock');
import '../Content/src/staticBlock/modal/addEditStaticBlock/ModalAddEditStaticBlockCtrl.js';

import '../Content/src/staticPage/staticPage.js';
appDependency.addItem('staticPage');

import '../Content/src/staticPages/staticPages.js';
appDependency.addItem('staticPages');

import '../Content/src/reviews/reviews.js';
appDependency.addItem('reviews');

import '../Content/src/reviews/modal/addEditReview/ModalAddEditReviewCtrl.js';

import '../Content/src/discountsPriceRange/discountsPriceRange.js';
appDependency.addItem('discountsPriceRange');

import '../Content/src/discountsPriceRange/modal/addEditDiscountsPriceRange/ModalAddEditDiscountsPriceRangeCtrl.js';

import '../Content/src/discountsByTime/discountsByTime.js';
import '../Content/src/discountsByTime/modal/discountByDatetime/ModalDiscountByDatetimeCtrl.js';
appDependency.addItem('discountsByTime');

import '../Content/src/coupons/coupons.js';
appDependency.addItem('coupons');

import '../Content/src/coupons/modal/addEditCoupon/ModalAddEditCouponCtrl.js';
import '../Content/src/coupons/modal/coupon-products-selectvizr/ModalCouponProductsSelectvizrCtrl.js';

import '../Content/src/partners/partners.js';
appDependency.addItem('partners');

import '../Content/src/partners/modals/partnersReport/ModalPartnersReportCtrl.js';

import '../Content/src/partnersReport/partnersPayoutReports.js';
appDependency.addItem('partnersPayoutReports');

import '../Content/src/partnersReport/partnersReport.js';
appDependency.addItem('partnersReport');

import '../Content/src/paymentMethods/paymentMethod.js';
import '../Content/src/paymentMethods/components/paymentMethodsList/paymentMethodsList.js';
import '../Content/src/paymentMethods/components/billStamp/billStamp.js';
appDependency.addItem('paymentMethod');

import '../Content/src/paymentMethods/modal/addPaymentMethod/ModalAddPaymentMethodCtrl.js';
import '../Content/src/paymentMethods/modal/robokassaRegistration/ModalRobokassaRegistrationCtrl.js';
import '../Content/src/paymentMethods/modal/tBank/ModalTBankRegistrationCtrl.js';

import '../Content/src/shippingMethods/shippingMethod.js';
import '../Content/src/shippingMethods/components/shippingByOrderPrice/shippingByOrderPriceMethod.js';
import '../Content/src/shippingMethods/components/shippingByRangePriceAndDistance/shippingByPriceLimit.js';
import '../Content/src/shippingMethods/components/shippingByRangeWeightAndDistance/shippingByWeightLimit.js';
import '../Content/src/shippingMethods/components/shippingByRangeWeightAndDistance/shippingByDistanceLimit.js';
import '../Content/src/shippingMethods/components/shippingByProductAmount/shippingByProductAmount.js';
import '../Content/src/shippingMethods/components/pointDelivery/pointDelivery.js';
import '../Content/src/shippingMethods/components/shippingSdekSelectCity/shippingSdekSelectCity.js';
import '../Content/src/shippingMethods/components/shippingPecSelectCity/shippingPecSelectCity.js';
import '../Content/src/shippingMethods/components/deliveryByZonesList/deliveryByZonesList.js';
import '../Content/src/shippingMethods/components/fivePostWarehouseDeliveryTypeReference/fivePostWarehouseDeliveryTypeReference.js';
import '../Content/src/shippingMethods/components/fivePostRateDeliverySLReference/fivePostRateDeliverySLReferenceCtrl.js';
appDependency.addItem('shippingMethod');

import '../Content/src/shippingMethods/components/shippingMethodsList/shippingMethodsList.js';
appDependency.addItem('shippingMethodsList');

import '../Content/src/shippingMethods/modal/addShippingMethod/ModalAddShippingMethodCtrl.js';
import '../Content/src/shippingMethods/modal/addPointDelivery/addPointDelivery.js';
import '../Content/src/shippingMethods/modal/addDeliveryZone/addDeliveryZone.js';

import '../Content/src/csseditor/csseditor.js';
appDependency.addItem('csseditor');

import '../Content/src/domainsManage/domainsManage.js';
appDependency.addItem('domainsManage');

import '../Content/src/landingPages/landingPages.js';
appDependency.addItem('landingPages');

import '../Content/src/searchQueries/searchQueries.js';
appDependency.addItem('searchQueries');

import '../Content/src/emailingLog/emailingLog.js';
appDependency.addItem('emailingLog');

import '../Content/src/emailingLog/modal/viewEmail/ModalViewEmailCtrl.js';

import '../Content/src/manualEmailing/manualEmailing.js';
appDependency.addItem('manualEmailing');

import '../Content/src/manualEmailing/manualWithoutEmailing.js';
appDependency.addItem('manualWithoutEmailing');

import '../Content/src/manualEmailings/manualEmailings.js';
appDependency.addItem('manualEmailings');

import '../Content/src/triggerAnalytics/triggerAnalytics.js';
appDependency.addItem('triggerAnalytics');

// import '../Content/src/_shared/iframe-responsive/iframe-responsive.css';
// import '../Content/src/_shared/iframe-responsive/iframeResponsive.js';
// import '../Content/src/_shared/iframe-responsive/iframeResponsive.component.js';
// import '../Content/src/_shared/iframe-responsive/iframeResponsive.service.js';
// import '../Content/src/_shared/iframe-responsive/iframeResponsive.ctrl.js';
import '../../../scripts/_common/iframe-responsive/iframeResponsive.module.js';
appDependency.addItem('iframeResponsive');

import '../Content/src/_shared/modal/referralLink/ModalReferralLinkCtrl.js';

import '../Content/src/_partials/admin-color-scheme/adminColorScheme.directive.js';
import '../Content/src/_partials/admin-color-scheme/adminColorScheme.service.js';
appDependency.addItem('adminColorScheme');

import '../Content/src/settingsTemplate/settingsTemplate.js';
appDependency.addItem('settingsTemplate');

import '../Content/src/vk/vkMain.js';
appDependency.addItem('vkMain');

import '../Content/src/vk/components/vkChannel/vkChannel.js';
appDependency.addItem('vkChannel');

import '../Content/src/vk/components/vkAuth/vkAuth.js';
import '../Content/src/vk/services/vkService.js';
appDependency.addItem('vkAuth');

import '../Content/src/vk/components/vkMarketExport/vkMarketExport.js';
appDependency.addItem('vkMarketExport');

import '../Content/src/vk/components/vkMarketCategories/vkMarketCategories.js';
appDependency.addItem('vkMarketCategories');

import '../Content/src/vk/components/vkMarketCategories/modals/ModalAddEditVkCategory.js';

import '../Content/src/vk/components/vkMarketExportSettings/vkMarketExportSettings.js';
appDependency.addItem('vkMarketExportSettings');

import '../Content/src/vk/components/vkMarketImportSettings/vkMarketImportSettings.js';
appDependency.addItem('vkMarketImportSettings');

import '../Content/src/vk/services/vkMarketService.js';
appDependency.addItem('vkMarket');

import '../Content/src/vk/modal/saveVkMarketSettings/ModalSaveVkMarketSettingsCtrl.js';

import '../Content/src/telegram/components/telegramAuth/telegramAuth.js';
import '../Content/src/telegram/components/telegramAuth/telegramAuthService.js';
appDependency.addItem('telegramAuth');

import '../Content/src/instagram/components/instagramAuth/instagramAuth.js';
import '../Content/src/instagram/components/instagramAuth/instagramAuthService.js';
appDependency.addItem('instagramAuth');

import '../Content/src/_shared/salesChannels/salesChannels.js';
appDependency.addItem('salesChannels');

import '../Content/src/ok/okMain.js';
appDependency.addItem('okMain');

import '../Content/src/ok/components/okAuth/okAuth.js';
appDependency.addItem('okAuth');

import '../Content/src/ok/components/okChannel/okChannel.js';
import '../Content/src/ok/services/okService.js';
appDependency.addItem('okChannel');

import '../Content/src/ok/components/okMarketExport/okMarketExport.js';
import '../Content/src/ok/services/okMarketService.js';
appDependency.addItem('okMarketExport');

import '../Content/src/ok/components/okMarketExport/modals/modalAddEditOkCatalog/ModalAddEditOkCatalogCtrl.js';
import '../Content/src/ok/components/okMarketExport/modals/modalSaveOkMarketExportSettings/ModalSaveOkMarketExportSettingsCtrl.js';

import '../Content/src/ok/components/okMarketImport/okMarketImport.js';
appDependency.addItem('okMarketImport');

import '../Content/src/_shared/modal/addRemovePropertyToProducts/ModalAddRemovePropertyToProductsCtrl.js';
import '../Content/src/_shared/modal/addTagsToProducts/ModalAddTagsToProductsCtrl.js';
import '../Content/src/_shared/modal/removeTagsToProducts/ModalRemoveTagsToProductsCtrl.js';

import '../Content/src/customerTags/customerTags.js';
import '../Content/src/customerTags/customerTagsModalCtrl.js';
appDependency.addItem('customerTags');

import '../Content/src/_shared/modal/addTagsToCustomers/ModalAddTagsToCustomersCtrl.js';

import ngInfiniteScroll from 'ng-infinite-scroll';
appDependency.addItem(ngInfiniteScroll);

import '../Content/src/storePage/storePage.js';
appDependency.addItem('storePage');

import '../Content/src/priceRules/priceRules.js';
appDependency.addItem('priceRules');

import '../Content/src/priceRules/modal/addEditPriceRule/ModalAddEditPriceRuleCtrl.js';

import '../Content/src/photoCategory/photoCategory.js';
appDependency.addItem('photoCategory');
import '../Content/src/photoCategory/modal/addEditPhotoCategory/ModalAddEditPhotoCategoryCtrl.js';

import '../Content/src/sizeChart/sizeChart.js';
appDependency.addItem('sizeChart');
import '../Content/src/sizeChart/modal/addEditSizeChart/ModalAddEditSizeChartCtrl.js';
import '../Content/src/sizeChart/modal/addSizeChartProperty/ModalAddSizeChartPropertyCtrl.js';

import '../Content/src/_shared/back/back.module.js';
import '../Content/src/_shared/back/back.service.js';
import '../Content/src/_shared/back/back.directive.js';
appDependency.addItem('back');

import '../Content/src/_shared/is-mobile/is-mobile.js';
appDependency.addItem('isMobile');

import '../Content/styles/atomic.scss';

import '../Content/src/_app/app.js';
import setCssCustomProps from '../../../scripts/_common/setCssCustomProps/setCssCustomProps.module.js';
appDependency.addItem(setCssCustomProps);
import '../Content/src/_shared/modal/addTag/ModalAddTagCtrl.js';

import '../Content/src/_shared/shop-name/shopName.component.js';
appDependency.addItem('shopName');

import '../Content/src/shippingMethods/components/shippingWithInterval/shippingWithInterval.js';
import '../Content/src/shippingMethods/components/shippingWithInterval/modals/addEditShippingIntervals/addEditShippingIntervals.js';
import '../Content/src/shippingMethods/components/shippingWithInterval/modals/intervalsByRange/intervalsByRange.js';

import ratingModule from '../../../scripts/_common/rating/rating.module.js';
appDependency.addItem(ratingModule);

import addressModule from '../../../scripts/_partials/address/address.module.js';
appDependency.addItem(addressModule);

import '../Content/src/_shared/modal/editFormField/ModalEditFormFieldCtrl.js';

import tourguidejsModule from '../Content/src/_shared/ng-tour-guide/ng-tour-guide.module.js';
appDependency.addItem(tourguidejsModule);
