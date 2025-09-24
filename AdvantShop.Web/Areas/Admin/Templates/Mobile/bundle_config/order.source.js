import appDependency from '../../../../../scripts/appDependency.js';

import '../../../Content/vendors/angular-inview/angular-inview.js';

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

import '../../../Content/src/product/product.js';
import '../../../Content/src/product/product.service.js';

import '../../../../../scripts/_common/spinbox/spinbox.module.js';

import '../../../../../scripts/_partials/payment/payment.module.js';

import '../../../../../scripts/_partials/shipping/shipping.module.js';

import '../../../Content/src/order/components/orderStatusHistory/orderStatusHistory.js';

import '../../../Content/src/order/components/orderHistory/orderHistory.js';

import '../../../Content/src/order/components/orderItemsSummary/orderItemsSummary.js';
import '../../../Content/src/order/styles/orders.scss';
import '../../../Content/src/order/order.js';

// import '../../../Content/src/order/styles/orders.scss';
import '../../../Content/src/_shared/modal/offers-selectvizr/ModalOffersSelectvizrCtrl.js';
import '../../../Content/src/order/modal/changeOrderCustomer/changeOrderCustomer.js';
import '../../../Content/src/order/modal/changeOrderCustomerAddress/changeOrderCustomerAddress.js';
import '../../../Content/vendors/checklist-model/checklist-model.js';

import '../../../Content/src/order/modal/changeOrderStatus/ModalChangeOrderStatusCtrl.js';
import '../../../Content/src/order/modal/desktopAppNotification/desktopAppNotification.js';
import '../../../Content/src/order/modal/editCustomOptions/ModalEditCustomOptionsCtrl.js';
import '../../../Content/src/order/modal/changeAddress/ModalChangeOrderAddressCtrl.js';
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

/**/

import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';
import '../../../Content/src/_partials/leadEvents/leadEvents.js';
import '../../../Content/src/_partials/leadEvents/modals/addEditCallComent/ModalAddEditCallComentCtrl.js';
import '../../../Content/src/_partials/leadEvents/modals/showEmail/ModalShowEmailCtrl.js';
// import '../../../Content/src/_partials/leadEvents/styles/lead-events.scss';

import '../../../Content/src/calls/components/callRecord/callRecord.js';
import '../../../Content/vendors/angular-timeago/angular-timeago-core.js';
import '../../../Content/vendors/angular-timeago/angular-timeago.langs.js';
import '../../../Content/src/_shared/modal/sendLetterToCustomer/ModalSendLetterToCustomerCtrl.js';
import '../Content/styles/_shared/ui-grid/ui-grid-selection.scss';
import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.modified.js';
import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.css';

import '../../../Content/src/_shared/modal/sendSms/ModalSendSmsAdvCtrl.js';

import '../../../Content/src/settingsSms/settingsSms.js';

import '../../../Content/src/_shared/modal/bonus/Cards/ModalAddCardCtrl.js';

import '../../../Content/src/_shared/modal/selectCustomer/ModalSelectCustomerCtrl.js';
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
import '../Content/styles/_shared/order-items-summary/order-items-summary.scss';
import '../Content/styles/_shared/inputs/form-control-floating.scss';
import '../Content/styles/_partials/client-info/client-info.scss';
import '../Content/styles/_shared/popup-order-customer/popup-order-customer.scss';
import '../Content/styles/_partials/payment/payment.scss';
import '../Content/styles/_partials/shipping/shipping.scss';
import '../Content/styles/_partials/bonus-card-client/bonus-card-client.scss';
import '../Content/styles/_partials/add-bonus-card-form/add-bonus-card-form.scss';

import addressModule from '../Content/src/_partials/address/address.module.js';
appDependency.addItem(addressModule);

export default `order`;
