import appDependency from '../../../../../scripts/appDependency.js';

import '../../../Content/src/coupons/coupons.js';
import '../../../Content/src/coupons/modal/addEditCoupon/ModalAddEditCouponCtrl.js';
import '../../../Content/src/coupons/modal/coupon-products-selectvizr/ModalCouponProductsSelectvizrCtrl.js';
appDependency.addItem('coupons');

import '../../../Content/src/discountsPriceRange/discountsPriceRange.js';
import '../../../Content/src/discountsPriceRange/modal/addEditDiscountsPriceRange/ModalAddEditDiscountsPriceRangeCtrl.js';
appDependency.addItem('discountsPriceRange');
import '../../../Content/src/discountsByTime/discountsByTime.js';
import '../../../Content/src/discountsByTime/modal/discountByDatetime/ModalDiscountByDatetimeCtrl.js';
appDependency.addItem('discountsByTime');

import '../../../Content/src/certificates/certificates.js';
import '../../../Content/src/certificates/modal/certificateSettings/ModalCertificateSettingsCtrl.js';
import '../../../Content/src/certificates/modal/addEditCertificates/ModalAddEditCertificatesCtrl.js';
appDependency.addItem('certificates');

import '../../../Content/src/settingsCoupons/settingsCoupons.js';
import '../../../Content/src/settingsCoupons/settingsCoupons.service.js';

import '../Content/styles/views/settings.scss';
import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/_shared/product-item/product-item.scss';
import '../Content/src/_shared/swipe-line/swipe-line.module.js';

import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.component.js';
import '../../../Content/src/_shared/modal/products-selectvizr/ModalProductsSelectvizrCtrl.js';
import '../../../Content/src/_shared/modal/export-products-selectvizr/ModalExportProductsSelectvizrCtrl.js';
import '../../../Content/src/_shared/modal/shipping-products-selectvizr/ModalShippingProductsSelectvizrCtrl.js';
import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';

import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.modified.js';
import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.css';

import '../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
appDependency.addItem(`ngClickCapture`);

import '../../../Content/src/_shared/modal/editableGridRow/ModalEditableGridRow.js';

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
appDependency.addItem(`productGridItem`);

appDependency.addItem(`ngCkeditor`);
appDependency.addItem(`swipeLine`);
appDependency.addItem('settingsCoupons');
import '../../../Content/src/_shared/modal/offers-selectvizr/ModalOffersSelectvizrCtrl.js';
import '../../../Content/src/_partials/offers-selectvizr/offersSelectvizr.js';
import '../../../Content/src/_partials/offers-selectvizr/offersSelectvizr.component.js';
import '../../../Content/src/_shared/modal/offers-selectvizr/ModalOffersSelectvizrCtrl.js';
appDependency.addItem(`offersSelectvizr`);
