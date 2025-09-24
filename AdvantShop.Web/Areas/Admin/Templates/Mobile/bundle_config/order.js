//import orderModule from './order.source.js';
//import appDependency from '../../../../../scripts/appDependency.js';
//appDependency.addItem(orderModule);

import appDependency from '../../../../../scripts/appDependency.js';

/* хз зачем но просит*/
import 'ng-file-upload';
import '../../../../../scripts/_common/lozad-adv/lozadAdv.module.js';
/**************/
import 'angular-inview';
appDependency.addItem(`angular-inview`);

import '../Content/vendors/ng-sortable-custom/ng-sortable.module.js';

import '../../../Content/src/product/components/productGifts/productGifts.js';
import '../../../Content/src/product/components/productPhotos360/productPhotos360.js';
import '../../../Content/src/product/components/productProperties/productProperties.js';
import '../../../Content/src/product/components/productProperties/productProperties.service.js';
import '../../../Content/src/product/components/productReviews/productReviews.js';
import '../../../Content/src/product/components/productVideos/productVideos.js';
import '../../../Content/src/product/components/relatedProducts/relatedProducts.js';

import '../../../Content/src/_partials/offers-selectvizr/offersSelectvizr.js';
import '../../../Content/src/_partials/offers-selectvizr/offersSelectvizr.component.js';

import '../../../Content/src/product/components/productPhotos/productPhotos.js';
import '../../../Content/src/product/components/productPhotos/productPhotos.service.js';
appDependency.addItem(`productPhotos`);

import '../../../Content/src/product/product.js';
import '../../../Content/src/product/product.service.js';
appDependency.addItem(`product`);

import '../../../../../scripts/_common/spinbox/spinbox.module.js';
appDependency.addItem(`spinbox`);

import paymentModule from '../../../../../scripts/_partials/payment/payment.module.js';
appDependency.addItem(paymentModule);
import shippingModule from '../../../../../scripts/_partials/shipping/shipping.module.js';
appDependency.addItem(shippingModule);
import '../../../Content/src/order/components/orderStatusHistory/orderStatusHistory.js';
appDependency.addItem(`orderStatusHistory`);
import '../../../Content/src/order/components/orderHistory/orderHistory.js';
appDependency.addItem(`orderHistory`);
import '../../../Content/src/order/components/orderItemsSummary/orderItemsSummary.js';
import '../../../Content/src/order/order.js';
import '../../../Content/src/order/styles/orders.scss';
appDependency.addItem(`order`);
appDependency.addItem(`orderItemsSummary`);

import '../../../Content/src/order/components/orderItemCustomOptions/styles/orderItemCustomOptions.scss';
import '../../../Content/src/order/components/orderItemCustomOptions/orderItemCustomOptions.module.js';
import '../../../Content/src/order/components/orderItemCustomOptions/controllers/orderItemCustomOptions.controller.js';
import '../../../Content/src/order/components/orderItemCustomOptions/directives/orderItemCustomOptions.directive.js';
import '../../../Content/src/order/components/orderItemCustomOptions/services/orderItemCustomOptions.service.js';
appDependency.addItem('orderItemCustomOptions');

// import '../../../Content/src/order/styles/orders.scss';
import '../../../Content/src/_shared/modal/offers-selectvizr/ModalOffersSelectvizrCtrl.js';
import '../../../Content/src/order/modal/changeOrderCustomer/changeOrderCustomer.js';
import '../../../Content/src/order/modal/changeOrderCustomerAddress/changeOrderCustomerAddress.js';
import '../../../Content/src/order/modal/changeOrderRecipient/ModalChangeOrderRecipient.js';
import 'checklist-model';
appDependency.addItem(`checklist-model`);

import '../../../Content/src/order/modal/changeOrderStatus/ModalChangeOrderStatusCtrl.js';
import '../../../Content/src/order/modal/desktopAppNotification/desktopAppNotification.js';
import '../../../Content/src/order/modal/editCustomOptions/ModalEditCustomOptionsCtrl.js';

/*модалки доставки */
import '../../../Content/src/order/modal/shippings/ModalShippingsCtrl.js';
import '../../../Content/src/order/modal/shippings/russianPost/customsDeclarationProductData/modalCustomsDeclarationProductDataCtrl.js';
import '../../../Content/src/order/modal/sendOrderGrastin/ModalSendOrderGrastinCtrl.js';
import '../../../Content/src/order/modal/sendOrderGrastin/ModalSendRequestGrastinForActCtrl.js';
import '../../../Content/src/order/modal/sendOrderGrastin/ModalSendRequestGrastinForIntakeCtrl.js';
import '../../../Content/src/order/modal/shippings/sdek/downloadBarCodeOrder/modalSdekDownloadBarCodeOrderCtrl.js';
import '../../../Content/src/order/modal/shippings/yandex/changeDeliveryDate/modalYandexChangeDeliveryDateCtrl.js';
import '../../../Content/src/order/modal/shippingsTime/ModalShippingsTimeCtrl.js';
import '../../../Content/src/order/modal/changeMarking/ModalChangeMarkingCtrl.js';

/*модалки оплаты */
import '../../../Content/src/order/modal/payments/ModalPaymentsCtrl.js';
import '../../../Content/src/order/modal/sendBillingLink/ModalSendBillingLinkCtrl.js';
import '../../../Content/src/order/modal/getBillingLink/ModalGetBillingLinkCtrl.js';

import '../../../Content/src/orders/modal/ModalChangeOrderStatusesCtrl.js';

/*склады*/
import '../../../Content/src/order/modal/distributionOfOrderItem/modalDistributionOfOrderItemCtrl.js';
/**/
import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';
import '../../../Content/src/_partials/leadEvents/leadEvents.js';
import '../../../Content/src/_partials/leadEvents/modals/addEditCallComent/ModalAddEditCallComentCtrl.js';
import '../../../Content/src/_partials/leadEvents/modals/showEmail/ModalShowEmailCtrl.js';
// import '../../../Content/src/_partials/leadEvents/styles/lead-events.scss';

import '../../../Content/src/calls/components/callRecord/callRecord.js';
import '../../../Content/vendors/angular-timeago/angular-timeago-core.js';
import '../../../Content/vendors/angular-timeago/angular-timeago.langs.js';
appDependency.addItem(`leadEvents`);

import '../../../Content/src/_shared/modal/sendLetterToCustomer/ModalSendLetterToCustomerCtrl.js';
import '../Content/styles/_shared/ui-grid/ui-grid-selection.scss';
import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.modified.js';
import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.css';
appDependency.addItem(`ngCkeditor`);

import '../../../Content/src/_shared/modal/sendSms/ModalSendSmsAdvCtrl.js';
import '../../../Content/src/_shared/modal/selectSmsTemplate/ModalSelectSmsTemplateCtrl.js';

import '../../../Content/src/settingsSms/settingsSms.js';
appDependency.addItem(`settingsSms`);

import '../../../Content/src/_shared/modal/bonus/Cards/ModalAddCardCtrl.js';

import '../../../Content/src/_shared/modal/selectCustomer/ModalSelectCustomerCtrl.js';

import '../../../Content/src/_shared/autocompleter/autocompleter.js';
appDependency.addItem(`autocompleter`);

import '../Content/styles/_shared/card/card.scss';
import '../../../Content/src/customer/styles/customer.scss';
import '../Content/styles/_partials/page-head.scss';
import '../Content/styles/_partials/block.scss';
import '../Content/styles/views/order.scss';
import '../Content/styles/_shared/order-customer/order-customer.scss';
import '../Content/styles/_shared/order-customer/order-customer-form.scss';
import '../Content/styles/_shared/inputs/custom-input.scss';
import '../Content/styles/_partials/order-info/order-info.scss';
import '../Content/styles/_partials/order-content/order-content.scss';
import '../Content/styles/_shared/product-item/product-item.scss';
import '../Content/styles/_shared/inputs/form-control-floating.scss';
import '../Content/styles/_partials/client-info/client-info.scss';
import '../Content/styles/_partials/popup-order-customer.scss';
import '../Content/styles/_partials/payment/payment.scss';
import '../Content/styles/_partials/shipping/shipping.scss';
import '../Content/styles/_partials/bonus-card-client/bonus-card-client.scss';
import '../Content/styles/_partials/add-bonus-card-form/add-bonus-card-form.scss';

import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.component.js';

import '../../../Content/src/_shared/dynamic-form/dynamicForm.js';
appDependency.addItem(`dynamicForm`);

import '../../../Content/src/_shared/modal/editFormField/ModalEditFormFieldCtrl.js';
import '../../../Content/src/_shared/set-attributes/setAttributes.js';
appDependency.addItem(`setAttributes`);

import '../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
appDependency.addItem(`ngClickCapture`);

import '../../../Content/src/_shared/modal/editableGridRow/ModalEditableGridRow.js';

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
appDependency.addItem(`productGridItem`);

import '../../../Content/src/_partials/customer-fields/customerFields.component.js';
appDependency.addItem('customerFields');

import '../Content/styles/views/settings.scss';

import '../Content/src/_partials/leadEvents/styles/lead-events.scss';

import '../../../Content/src/_partials/leadEvents/modals/editComment/ModalEditCommentCtrl.js';

import '../../../Content/src/_partials/leadEvents/modals/answer/ModalAnswerCtrl.js';

import '../../../Content/src/_shared/modal/addTask/ModalAddTaskCtrl.js';

import '../../../Content/src/_shared/modal/sendSocialMessage/ModalSendSocialMessageCtrl.js';

import scrollIntoViewModule from '../../../../../scripts/_common/scroll-into-view/scroll-into-view.js';
appDependency.addItem(scrollIntoViewModule);

import '../Content/src/_partials/admin-comments/styles/admin-comments.scss';
import '../../../Content/src/_partials/admin-comments/adminComments.js';
import '../../../Content/src/_partials/admin-comments/adminCommentsForm.js';
import '../../../Content/src/_partials/admin-comments/adminCommentsItem.js';
import '../../../Content/src/_partials/admin-comments/adminComments.component.js';
import '../../../Content/src/_partials/admin-comments/adminComments.service.js';
appDependency.addItem('adminComments');
appDependency.addItem('adminCommentsForm');
appDependency.addItem('adminCommentsItem');

import '../../../Content/src/_partials/sidebar-user/sidebarUser.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUser.service.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUser.component.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUserTrigger.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUserTrigger.component.js';
import '../Content/src/_partials/sidebar-user/styles/sidebar-user.scss';
appDependency.addItem('sidebarUser');

import '../../../Content/src/_partials/tasks-grid/tasksGrid.js';
import '../../../Content/src/_partials/tasks-grid/tasksGrid.component.js';
appDependency.addItem('tasksGrid');

import '../../../Content/src/tasks/tasks.js';
import '../../../Content/src/tasks/tasks.service.js';
import '../../../Content/src/tasks/modal/editTask/ModalEditTaskCtrl.js';
import '../../../Content/src/tasks/modal/changeTaskStatuses/ModalChangeTaskStatusesCtrl.js';
import '../../../Content/src/tasks/modal/completeTask/ModalCompleteTaskCtrl.js';
import '../../../Content/src/tasks/modal/changeTaskGroup/ModalChangeTaskGroupCtrl.js';
import '../../../Content/src/tasks/task-create.ctrl.js';
import '../../../Content/src/tasks/task-create.component.js';
import '../../../Content/src/tasks/modal/editTaskObserver/ModalEditTaskObserverCtrl.js';
import '../../../Content/src/tasks/modal/finishTask/ModalFinishTaskCtrl.js';
import '../Content/styles/_shared/order-items-summary/order-items-summary.scss';
import '../../../Content/src/tasks/styles/tasks.scss';
appDependency.addItem('tasks');

import '../../../Content/src/lead/lead.js';
import '../../../Content/src/lead/lead.service.js';
import '../../../Content/src/lead/components/leadItemsSummary/leadItemsSummary.js';

appDependency.addItem('lead');

import ratingModule from '../../../../../scripts/_common/rating/rating.module.js';
appDependency.addItem(ratingModule);

import '../../../Content/src/order/modal/changeAddress/ModalChangeOrderAddressCtrl.js';

import addressModule from '../Content/src/_partials/address/address.module.js';
appDependency.addItem(addressModule);

import '../Content/src/order/components/orderItemCustomOptions/styles/styles.scss';
