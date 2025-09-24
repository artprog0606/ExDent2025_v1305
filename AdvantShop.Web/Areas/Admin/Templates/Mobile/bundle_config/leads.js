import appDependency from '../../../../../scripts/appDependency.js';

import 'angular-timeago';
import 'ng-file-upload';
import 'tinycolor2';
import '../../../Content/vendors/angular-color-picker/angularjs-color-picker.cjs';
import '../../../Content/vendors/angular-color-picker/angularjs-color-picker.css';
import '../../../Content/vendors/angular-color-picker/themes/angularjs-color-picker-bootstrap.css';

import '../Content/styles/_shared/color-picker/color-picker.scss';
import '../Content/styles/views/settings.scss';

import '../../../Content/src/vk/components/vkAuth/vkAuth.js';

import '../../../Content/src/customers/customers.js';

import '../../../Content/src/customergroups/customergroups.js';
import '../../../Content/src/customergroups/modal/addCustomerGroup/ModalAddCustomerGroupCtrl.js';

import '../../../Content/src/customerSegments/customerSegments.js';

import '../../../Content/src/subscription/subscription.js';

import '../../../Content/src/_shared/file-uploader/fileUploader.js';
import '../../../Content/src/_shared/file-uploader/fileUploader.component.js';
import '../../../Content/src/_shared/file-uploader/modal/fileUploaderModal.js';

import '../../../Content/src/import/import.js';
import '../../../Content/src/import/import.service.js';

import '../../../Content/src/customerTags/customerTags.js';
import '../../../Content/src/customerTags/customerTagsModalCtrl.js';

import '../../../Content/src/_partials/lead-fields/leadFields.component.js';
appDependency.addItem('leadFields');

import '../../../Content/src/order/modal/shippings/ModalShippingsCtrl.js';
import '../../../Content/src/order/modal/shippingsTime/ModalShippingsTimeCtrl.js';
import '../../../Content/src/order/modal/sendOrderGrastin/ModalSendOrderGrastinCtrl.js';
import '../../../Content/src/order/modal/sendOrderGrastin/ModalSendRequestGrastinForIntakeCtrl.js';
import '../../../Content/src/order/modal/sendOrderGrastin/ModalSendRequestGrastinForActCtrl.js';
import '../../../Content/src/order/modal/payments/ModalPaymentsCtrl.js';
import '../../../Content/src/order/modal/changeOrderStatus/ModalChangeOrderStatusCtrl.js';
import '../../../Content/src/order/modal/sendBillingLink/ModalSendBillingLinkCtrl.js';
import '../../../Content/src/order/modal/getBillingLink/ModalGetBillingLinkCtrl.js';
import '../../../Content/src/order/modal/changeOrderCustomer/changeOrderCustomer.js';
import '../../../Content/src/order/modal/changeOrderCustomerAddress/changeOrderCustomerAddress.js';
import '../../../Content/src/order/modal/desktopAppNotification/desktopAppNotification.js';
import '../../../Content/src/order/modal/shippings/russianPost/customsDeclarationProductData/modalCustomsDeclarationProductDataCtrl.js';
import '../../../Content/src/order/modal/shippings/sdek/downloadBarCodeOrder/modalSdekDownloadBarCodeOrderCtrl.js';
import '../../../Content/src/order/modal/shippings/yandex/changeDeliveryDate/modalYandexChangeDeliveryDateCtrl.js';
import '../../../Content/src/order/modal/editCustomOptions/ModalEditCustomOptionsCtrl.js';
import '../../../Content/src/order/modal/changeMarking/ModalChangeMarkingCtrl.js';
import '../../../Content/src/order/modal/changeAddress/ModalChangeOrderAddressCtrl.js';

import '../../../Content/src/settingsCrm/settingsCrm.js';
import '../../../Content/src/settingsCrm/components/facebookAuth/facebookAuth.js';
import '../../../Content/src/settingsCrm/components/facebookAuth/facebookAuthService.js';
import '../../../Content/src/settingsCrm/components/salesFunnels/salesFunnels.js';
import '../../../Content/src/settingsCrm/components/salesFunnels/modals/addEditSalesFunnel/ModalAddEditSalesFunnelCtrl.js';
import '../../../Content/src/settingsCrm/components/dealStatuses/dealStatuses.js';
import '../../../Content/src/settingsCrm/components/dealStatuses/modals/editDealStatus/ModalEditDealStatusCtrl.js';
import '../../../Content/src/settingsCrm/components/integrationsLimit/integrationsLimit.js';
appDependency.addItem('settingsCrm');
appDependency.addItem('dealStatuses');
import '../../../Content/src/settingsCrm/components/leadFieldsList/leadFieldsList.js';
import '../../../Content/src/settingsCrm/components/leadFieldsList/leadFields.service.js';
import '../../../Content/src/settingsCrm/components/leadFieldsList/modals/addEditLeadField/ModalAddEditLeadFieldCtrl.js';
appDependency.addItem('leadFieldsList');

import '../../../Content/src/settingsCustomers/settingsCustomers.js';
import '../../../Content/src/settingsCustomers/customerFields.service.js';
import '../../../Content/src/settingsCustomers/customerFieldValues.service.js';
import '../../../Content/src/settingsCustomers/modal/addEditCustomerField/ModalAddEditCustomerFieldCtrl.js';
import '../../../Content/src/settingsCustomers/modal/addEditCustomerFieldValue/ModalAddEditCustomerFieldValueCtrl.js';
appDependency.addItem('settingsCustomers');

import '../../../Content/src/_partials/lead-info/leadInfo.js';
import '../../../Content/src/_partials/lead-info/leadInfo.component.js';
import '../../../Content/src/_partials/lead-info/leadInfoTrigger.js';
import '../../../Content/src/_partials/lead-info/leadInfoTrigger.component.js';
import '../../../Content/src/_partials/lead-info/leadInfo.service.js';
import '../Content/src/_partials/lead-info/styles/lead-info.scss';
appDependency.addItem('leadInfo');

import '../../../Content/src/_partials/sidebar-user/sidebarUser.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUser.service.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUser.component.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUserTrigger.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUserTrigger.component.js';
import '../Content/src/_partials/sidebar-user/styles/sidebar-user.scss';
appDependency.addItem('sidebarUser');

import '../../../Content/src/_shared/simple-edit/simpleEdit.js';
import '../../../Content/src/_shared/simple-edit/simpleEdit.ctrl.js';
import '../../../Content/src/_shared/simple-edit/simpleEdit.component.js';
appDependency.addItem('simpleEdit');

import '../../../Content/src/lead/lead.js';
import '../../../Content/src/lead/lead.service.js';
import '../../../Content/src/lead/components/leadItemsSummary/leadItemsSummary.js';
import '../Content/styles/_shared/order-items-summary/order-items-summary.scss';
appDependency.addItem('lead');

import '../../../Content/src/_partials/leadEvents/leadEvents.js';
import '../../../Content/src/_partials/leadEvents/modals/addEditCallComent/ModalAddEditCallComentCtrl.js';
import '../../../Content/src/_partials/leadEvents/modals/showEmail/ModalShowEmailCtrl.js';
appDependency.addItem('leadEvents');

import '../../../Content/src/calls/components/callRecord/callRecord.js';
appDependency.addItem('callRecord');

import '../../../Content/src/_shared/kanban/kanban.module.js';
appDependency.addItem('kanban');

import '../../../Content/src/leads/leads.js';
import '../../../Content/src/leads/leads.components.js';
import '../../../Content/src/leads/leadsListController.js';
import '../../../Content/src/leads/leadsListService.js';
import '../../../Content/src/leads/components/leadsListSources/leadsListSourcesController.js';
import '../../../Content/src/leads/components/leadsListChart/leadsListChartController.js';
import '../../../Content/src/leads/modal/changeLeadManager/ModalChangeLeadManagerCtrl.js';
import '../../../Content/src/leads/modal/changeLeadSalesFunnel/ModalChangeLeadSalesFunnelCtrl.js';

appDependency.addItem('leads');

import '../../../Content/src/_shared/modal/addLead/ModalAddLeadCtrl.js';

import '../../../Content/src/_partials/customer-fields/customerFields.component.js';
appDependency.addItem('customerFields');

import '../Content/vendors/ng-sortable-custom/ng-sortable.module.js';

import '../Content/styles/_shared/chips/chips.scss';

import scrollIntoViewModule from '../../../../../scripts/_common/scroll-into-view/scroll-into-view.js';
appDependency.addItem(scrollIntoViewModule);

import '../../../Content/src/_shared/modal/selectCustomer/ModalSelectCustomerCtrl.js';
import '../../../Content/src/_shared/modal/sendLetterToCustomer/ModalSendLetterToCustomerCtrl.js';
import '../../../Content/src/_shared/modal/sendSms/ModalSendSmsAdvCtrl.js';
import '../../../Content/src/_shared/modal/selectSmsTemplate/ModalSelectSmsTemplateCtrl.js';

import '../../../Content/src/order/modal/shippings/ModalShippingsCtrl.js';
import '../../../../../scripts/_partials/shipping/shipping.module.js';
import '../../../../../scripts/_partials/zone/zone.js';
import zoneModule from '../../../../../scripts/_partials/zone/zone.module.ts';
appDependency.addItem(zoneModule);

import '../../../Content/src/fake/checkout/checkout.js';
import '../../../Content/src/fake/checkout/checkoutService.js';

import '../../../../../scripts/_common/modal/modal.module.js';

appDependency.addItem('shipping');

import 'checklist-model';
appDependency.addItem('checklist-model');

import '../../../Content/src/lead/modal/completeLead/ModalCompleteLeadCtrl.js';
import '../../../Content/src/lead/modal/shippingsCity/ModalShippingsCityCtrl.js';
import '../../../Content/src/lead/modal/completeLead/ModalCompleteLeadCtrl.js';
import '../../../Content/src/lead/modal/desktopAppNotification/desktopAppNotification.js';

import '../../../Content/src/_partials/tasks-grid/tasksGrid.js';
import '../../../Content/src/_partials/tasks-grid/tasksGrid.component.js';
appDependency.addItem('tasksGrid');

import '../Content/styles/_shared/order-items-summary/order-items-summary.scss';

import '../../../Content/src/tasks/tasks.js';
import '../../../Content/src/tasks/tasks.service.js';
import '../../../Content/src/tasks/modal/editTask/ModalEditTaskCtrl.js';
import '../../../Content/src/tasks/modal/changeTaskStatuses/ModalChangeTaskStatusesCtrl.js';
import '../../../Content/src/tasks/modal/completeTask/ModalCompleteTaskCtrl.js';
import '../../../Content/src/tasks/modal/changeTaskGroup/ModalChangeTaskGroupCtrl.js';
import '../../../Content/src/tasks/task-create.ctrl.js';
import '../../../Content/src/tasks/task-create.component.js';
import '../Content/styles/_shared/order-items-summary/order-items-summary.scss';
appDependency.addItem('tasks');

import '../../../Content/src/_partials/admin-comments/adminComments.js';
appDependency.addItem('adminComments');
import '../../../Content/src/_partials/admin-comments/adminCommentsForm.js';
appDependency.addItem('adminCommentsForm');
import '../../../Content/src/_partials/admin-comments/adminCommentsItem.js';
appDependency.addItem('adminCommentsItem');
import '../../../Content/src/_partials/admin-comments/adminComments.component.js';
import '../../../Content/src/_partials/admin-comments/adminComments.service.js';

import '../Content/src/_partials/leadEvents/styles/lead-events.scss';

import '../../../Content/src/_partials/leadEvents/modals/editComment/ModalEditCommentCtrl.js';

import '../../../Content/src/_partials/leadEvents/modals/answer/ModalAnswerCtrl.js';

import '../../../Content/src/_shared/modal/addTask/ModalAddTaskCtrl.js';

import '../../../Content/src/_shared/modal/sendSocialMessage/ModalSendSocialMessageCtrl.js';

import '../../../Content/styles/common/delimiter.scss';

import '../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
appDependency.addItem(`ngClickCapture`);

import '../../../Content/src/_shared/modal/editableGridRow/ModalEditableGridRow.js';

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
appDependency.addItem(`productGridItem`);

import '../Content/styles/_partials/messegers.scss';
import '../Content/styles/_partials/shipping/shipping.scss';

import ModalOffersSelectvizrModule from '../../../Content/src/_shared/modal/offers-selectvizr/ModalOffersSelectvizr.mobile.module.js';
import '../../../Content/src/_shared/modal/products-selectvizr/ModalProductsSelectvizr.mobile.module.js';
appDependency.addItem(ModalOffersSelectvizrModule);

import '../../../Content/src/_shared/autocompleter/autocompleter.js';
appDependency.addItem(`autocompleter`);

import '../Content/styles/views/analytics.scss';

import '../Content/styles/_partials/statistics-data.scss';
import '../Content/styles/views/settings-list.scss';

import addressModule from '../Content/src/_partials/address/address.module.js';
appDependency.addItem(addressModule);
