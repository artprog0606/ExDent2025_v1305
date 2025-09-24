import appDependency from '../../../../../scripts/appDependency.js';

import '../Content/vendors/ng-sortable-custom/ng-sortable.module.js';

import 'ng-file-upload';

import angularInview from 'angular-inview';
appDependency.addItem(angularInview);

//appDependency.addItem(ngFileUpload);

import toaster from 'angularjs-toaster';
appDependency.addItem(toaster);

import paymentMethodModule from '../../../Content/src/paymentMethods/paymentMethod.module.js';
appDependency.addItem(paymentMethodModule);

import '../../../Content/src/shippingMethods/components/shippingMethodsList/shippingMethodsList.js';
import '../Content/src/shippingMethods/components/shippingMethodsList/styles/shippingsMethodsList.scss';

import '../../../Content/src/settingsTwoFactorAuth/settingsTwoFactorAuth.js';
appDependency.addItem('settingsTwoFactorAuth');
import '../../../Content/src/shippingMethods/shippingMethod.js';
import '../../../Content/src/shippingMethods/components/shippingMethodsList/shippingMethodsList.js';
import '../../../Content/src/shippingMethods/components/shippingByOrderPrice/shippingByOrderPriceMethod.js';
import '../../../Content/src/shippingMethods/components/shippingByRangePriceAndDistance/shippingByPriceLimit.js';
import '../../../Content/src/shippingMethods/components/shippingByRangeWeightAndDistance/shippingByWeightLimit.js';
import '../../../Content/src/shippingMethods/components/shippingByRangeWeightAndDistance/shippingByDistanceLimit.js';
import '../../../Content/src/shippingMethods/components/shippingByProductAmount/shippingByProductAmount.js';
import '../../../Content/src/shippingMethods/components/pointDelivery/pointDelivery.js';
import '../../../Content/src/shippingMethods/modal/addShippingMethod/ModalAddShippingMethodCtrl.js';
import '../../../Content/src/shippingMethods/modal/addPointDelivery/addPointDelivery.js';
import '../../../Content/src/shippingMethods/components/shippingSdekSelectCity/shippingSdekSelectCity.js';
import '../../../Content/src/shippingMethods/components/shippingPecSelectCity/shippingPecSelectCity.js';
import '../../../Content/src/shippingMethods/components/deliveryByZonesList/deliveryByZonesList.js';
import '../../../Content/src/shippingMethods/modal/addDeliveryZone/addDeliveryZone.js';
import '../../../Content/src/shippingMethods/components/shippingWithInterval/shippingWithInterval.js';
import '../../../Content/src/shippingMethods/components/shippingWithInterval/modals/addEditShippingIntervals/addEditShippingIntervals.js';
import '../../../Content/src/shippingMethods/components/shippingWithInterval/modals/intervalsByRange/intervalsByRange.js';
import '../../../Content/src/shippingMethods/components/fivePostWarehouseDeliveryTypeReference/fivePostWarehouseDeliveryTypeReference.js';
import '../../../Content/src/shippingMethods/components/fivePostRateDeliverySLReference/fivePostRateDeliverySLReferenceCtrl.js';

import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.component.js';
import '../../../Content/src/_shared/modal/products-selectvizr/ModalProductsSelectvizrCtrl.js';
import '../../../Content/src/_shared/modal/export-products-selectvizr/ModalExportProductsSelectvizrCtrl.js';
import '../../../Content/src/_shared/modal/shipping-products-selectvizr/ModalShippingProductsSelectvizrCtrl.js';
import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';

appDependency.addItem('shippingMethodsList');

import '../../../Content/src/settings/settings.js';
import '../../../Content/src/settings/settings.ApiWebhooks.js';
import '../../../Content/src/settings/modal/addEditApiWebhook/addEditApiWebhook.js';
appDependency.addItem('settings');

/* о магазине*/
import '../Content/styles/views/settings.scss';
import '../Content/styles/views/settings-list.scss';
import '../Content/styles/_shared/logo-generator/logo-generator.scss';
import '../../../Content/src/_shared/picture-uploader/pictureUploader.js';
import '../../../Content/src/_shared/picture-uploader/pictureUploader.component.js';
import '../Content/styles/_shared/picture-uploader/picture-uploader.scss';
appDependency.addItem('pictureUploader');
import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/_shared/product-item/product-item.scss';
import '../../../Content/src/_shared/picture-uploader/modal/pictureUploaderModal.js';

import '../../../Content/src/_shared/icon-move/iconMove.js';
import '../../../Content/src/_shared/icon-move/styles/icon-move.css';
appDependency.addItem('iconMove');

import '../../../Content/src/settings/settings.Users.js';
import '../../../Content/src/settings/modal/addEditDepartment/ModalAddEditDepartmentCtrl.js';
import '../../../Content/src/settings/modal/addEditManagerRole/ModalAddEditManagerRoleCtrl.js';
import '../../../Content/src/settings/modal/editUserRoleActions/ModalEditUserRoleActionsCtrl.js';

import '../../../Content/vendors/cropper/cropper.min.js';
import '../../../Content/vendors/cropper/cropper.css';

import '../../../Content/vendors/cropper/ngCropper.js';
import '../../../Content/src/_shared/modal/cropImage/ModalCropImageCtrl.js';

import '../../../../../vendors/signalr/jquery.signalR.js';
import '../../../Content/vendors/iwc/iwc-all.cjs';
import '../../../Content/vendors/iwc-signalr/iwc-signalr.js';
import '../../../Content/vendors/iwc-signalr/signalr-patch.js';

import '../../../Content/src/settings/modal/changeUserPassword/ModalChangeUserPasswordCtrl.js';

import '../Content/src/paymentMethods/styles/paymentMethods.scss';

import '../../../Content/src/paymentMethods/modal/addPaymentMethod/ModalAddPaymentMethodCtrl.js';
import '../../../Content/src/paymentMethods/modal/robokassaRegistration/ModalRobokassaRegistrationCtrl.js';
import '../../../Content/src/paymentMethods/modal/tBank/ModalTBankRegistrationCtrl.js';

import '../../../Content/src/_shared/autocompleter/autocompleter.js';

import '../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
appDependency.addItem(`ngClickCapture`);

import '../../../Content/src/_shared/modal/editableGridRow/ModalEditableGridRow.js';

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
appDependency.addItem(`productGridItem`);

import '../../../Content/src/_shared/modal/testShippingCalculation/ModalTestShippingCalculationCtrl.js';

import tourguidejsModule from '../../../Content/src/_shared/ng-tour-guide/ng-tour-guide.module.js';
appDependency.addItem(tourguidejsModule);
