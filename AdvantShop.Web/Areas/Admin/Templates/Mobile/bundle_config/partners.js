import appDependency from '../../../../../scripts/appDependency.js';

import '../../../Content/src/partners/partners.js';
import '../../../Content/src/partners/modals/partnersReport/ModalPartnersReportCtrl.js';

import '../../../Content/src/_partials/partner-info/partnerInfo.js';
import '../../../Content/src/_partials/partner-info/partnerInfo.component.js';
import '../../../Content/src/_partials/partner-info/partnerInfoTrigger.js';
import '../../../Content/src/_partials/partner-info/partnerInfoTrigger.component.js';
import '../../../Content/src/_partials/partner-info/partnerInfoContainer.js';
import '../../../Content/src/_partials/partner-info/partnerInfo.service.js';

import '../../../Content/src/partner/partner.js';
import '../../../Content/src/partner/partnerView.js';
import '../../../Content/src/partner/modals/addPartnerCouponFromTpl/modalAddPartnerCouponFromTplCtrl.js';
import '../../../Content/src/partner/modals/addPartnerMoney/modalAddPartnerMoneyCtrl.js';
import '../../../Content/src/partner/modals/changePassword/ModalChangePartnerPasswordCtrl.js';
import '../../../Content/src/partner/modals/partnerRewardPayout/modalPartnerRewardPayoutCtrl.js';
import '../../../Content/src/partner/modals/subtractPartnerMoney/modalSubtractPartnerMoneyCtrl.js';
import '../../../Content/src/partner/components/partnerActReports/partnerActReports.js';
import '../../../Content/src/partner/components/partnerCustomers/partnerCustomers.js';
import '../../../Content/src/partner/components/partnerTransactions/partnerTransactions.js';

import '../Content/styles/views/settings.scss';
import '../Content/styles/views/partner.scss';
import '../Content/styles/_shared/card/card.scss';
import '../Content/src/_partials/lead-info/styles/lead-info.scss';

import '../../../Content/src/customer/customer.js';
import '../../../Content/src/customer/customerView.js';
import '../../../Content/src/customer/components/customerOrders/customerOrders.js';
import '../../../Content/src/customer/components/customerLeads/customerLeads.js';
import '../../../Content/src/customer/components/customerBookings/customerBookings.js';
import '../../../Content/src/customer/modals/changePassword/ModalChangePasswordCtrl.js';
import '../../../Content/src/customer/modals/desktopAppNotification/desktopAppNotification.js';

import '../../../Content/src/customers/customers.js';

import '../../../Content/src/_partials/customer-info/customerInfo.js';
import '../../../Content/src/_partials/customer-info/customerInfo.component.js';
import '../../../Content/src/_partials/customer-info/customerInfoTrigger.js';
import '../../../Content/src/_partials/customer-info/customerInfoTrigger.component.js';
import '../../../Content/src/_partials/customer-info/customerInfo.service.js';

import '../../../Content/src/settingsCustomers/settingsCustomers.js';
import '../../../Content/src/settingsCustomers/customerFields.service.js';

import '../Content/vendors/ng-sortable-custom/ng-sortable.module.js';
import '../../../Content/src/vk/components/vkAuth/vkAuth.js';
import '../../../Content/src/customergroups/customergroups.js';
import '../../../Content/src/customerSegments/customerSegments.js';
import '../../../Content/src/subscription/subscription.js';
import 'ng-file-upload';
import '../../../Content/src/_shared/file-uploader/fileUploader.js';
import '../../../Content/src/import/import.js';
import '../../../Content/src/customerTags/customerTags.js';

import '../../../Content/src/coupons/modal/addEditCoupon/ModalAddEditCouponCtrl.js';
import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';

import '../../../Content/src/partnersReport/partnersReport.js';
import '../../../Content/src/partnersReport/partnersPayoutReports.js';

import '../../../Content/src/_shared/modal/selectCustomer/ModalSelectCustomerCtrl.js';

import '../../../Content/src/coupons/modal/coupon-products-selectvizr/ModalCouponProductsSelectvizrCtrl.js';
import '../../../Content/src/coupons/coupons.js';

import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.component.js';
import '../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';

import '../../../Content/src/_shared/modal/editableGridRow/ModalEditableGridRow.js';

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
import '../Content/styles/_shared/product-item/product-item.scss';

appDependency.addItem(`ngClickCapture`);
appDependency.addItem(`productGridItem`);
appDependency.addItem('productsSelectvizr');
appDependency.addItem('partner');
appDependency.addItem('partnerView');
appDependency.addItem('partners');
appDependency.addItem('partnerInfo');
appDependency.addItem('partnersReport');
appDependency.addItem('partnersPayoutReports');
