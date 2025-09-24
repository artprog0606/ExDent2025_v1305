import appDependency from '../../../../../scripts/appDependency.js';
import '../../../Content/src/settingsPartners/settingsPartners.js';
import '../../../Content/src/settingsPartners/modal/editPaymentTypeName/ModalEditPaymentTypeNameCtrl.js';
import '../../../Content/src/settingsPartners/modal/editRewardPercent/ModalEditRewardPercentCtrl.js';

import '../Content/styles/views/settings.scss';
import '../Content/styles/_shared/card/card.scss';

import '../../../Content/src/_shared/file-uploader/fileUploader.js';
import '../../../Content/src/_shared/file-uploader/fileUploader.component.js';
import '../../../Content/src/_shared/file-uploader/modal/fileUploaderModal.js';

import 'ng-file-upload';
appDependency.addItem(`ngFileUpload`);

import '../Content/src/_shared/swipe-line/swipe-line.module.js';
import '../Content/styles/views/settings-list.scss';

import '../Content/vendors/ng-sortable-custom/ng-sortable.module.js';

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
appDependency.addItem(`productGridItem`);

import '../../../Content/src/coupons/modal/addEditCoupon/ModalAddEditCouponCtrl.js';
import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.component.js';
import '../../../Content/src/coupons/modal/coupon-products-selectvizr/ModalCouponProductsSelectvizrCtrl.js';

appDependency.addItem(`settingsPartners`);
