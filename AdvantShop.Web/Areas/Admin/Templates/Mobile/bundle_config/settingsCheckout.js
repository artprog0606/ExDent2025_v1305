import appDependency from '../../../../../scripts/appDependency.js';

import 'ng-file-upload';

import '../../../Content/src/_shared/file-uploader/fileUploader.js';
import '../../../Content/src/_shared/file-uploader/fileUploader.component.js';
import '../../../Content/src/_shared/file-uploader/modal/fileUploaderModal.js';

import '../../../Content/src/settingsCheckout/settingsCheckout.js';
import '../../../Content/src/settingsCheckout/components/thankYouPageProducts/thankYouPageProducts.js';
import '../../../Content/src/settingsCheckout/modal/addEditTax/ModalAddEditTaxCtrl.js';
import '../../../Content/src/settingsCheckout/modal/addEditWorkingTimes/addEditWorkingTimesCtrl.js';
import '../../../Content/src/settingsCheckout/modal/addEditAdditionalWorkingTime/ModalAddEditAdditionalWorkingTimeCtrl.js';

import '../../../Content/src/orderstatuses/orderstatuses.js';
import '../../../Content/src/orderstatuses/modal/ModalAddEditOrderStatusCtrl.js';
import '../../../Content/src/ordersources/ordersources.js';
import '../../../Content/src/ordersources/modal/ModalAddEditOrderSourceCtrl.js';

import '../Content/styles/views/settings.scss';
import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/_shared/product-item/product-item.scss';
import '../Content/styles/_shared/color-picker/color-picker.scss';
import '../Content/src/_shared/swipe-line/swipe-line.module.js';

import 'tinycolor2';
import '../../../Content/vendors/angular-color-picker/angularjs-color-picker.cjs';
import '../../../Content/vendors/angular-color-picker/angularjs-color-picker.css';
import '../../../Content/vendors/angular-color-picker/themes/angularjs-color-picker-bootstrap.css';

import '../../../Content/src/_shared/ui-ace-textarea/uiAceTextarea.js';
import '../../../Content/src/_shared/ui-ace-textarea/uiAceTextarea.constant.js';
import '../../../Content/src/_shared/ui-ace-textarea/uiAceTextarea.module.js';

import '../../../Content/src/_shared/modal/products-selectvizr/ModalProductsSelectvizrCtrl.js';
import '../../../Content/src/_shared/modal/export-products-selectvizr/ModalExportProductsSelectvizrCtrl.js';
import '../../../Content/src/_shared/modal/shipping-products-selectvizr/ModalShippingProductsSelectvizrCtrl.js';

import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.component.js';

import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';

import '../../../Content/src/analyticsReport/analyticsReport.js';
import '../../../Content/src/analyticsReport/components/export-orders/exportOrders.js';
appDependency.addItem(`analyticsReport`);
import '../../../Content/src/adv-analytics/adv-analytics.js';
import '../../../Content/src/adv-analytics/adv-analytics.service.js';
import '../../../Content/src/analyticsFilter/analyticsFilter.js';

import '../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
appDependency.addItem(`ngClickCapture`);

import '../../../Content/src/_shared/modal/editableGridRow/ModalEditableGridRow.js';

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
appDependency.addItem(`productGridItem`);
appDependency.addItem(`swipeLine`);
appDependency.addItem('settingsCheckout');

import '../../../Content/src/import/import.js';
import '../../../Content/src/import/import.service.js';
appDependency.addItem('import');
