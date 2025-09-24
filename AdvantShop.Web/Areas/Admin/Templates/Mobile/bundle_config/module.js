import appDependency from '../../../../../scripts/appDependency.js';

import '../../../Content/src/_shared/file-uploader/fileUploader.js';
import '../../../Content/src/_shared/file-uploader/fileUploader.component.js';
import '../../../Content/src/_shared/file-uploader/modal/fileUploaderModal.js';

import 'ng-file-upload';
appDependency.addItem(`ngFileUpload`);

import '../../../Content/src/module/module.js';
import '../Content/styles/views/module.scss';
import '../Content/src/product/styles/product.scss';
import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/views/settings-list.scss';
import '../Content/styles/_shared/module-card/module-card.scss';

/*lazy load in module?*/
import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.modified.js';
import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.css';
appDependency.addItem(`ngCkeditor`);

///*для модуля Callback*/
//import sidebarMenu from '../../../Content/src/_partials/sidebar-menu/sidebarMenu.module.js';
//appDependency.addItem(sidebarMenu);

import '../../../Content/src/_partials/sidebar-menu/sidebarMenuState.js';
import '../../../Content/src/_partials/sidebar-menu/sidebarMenu.service.js';
import '../../../Content/src/_partials/sidebar-menu/sidebarMenu.directive.js';
import '../../../Content/src/_partials/sidebar-menu/sidebarMenuTrigger.js';
appDependency.addItem(`sidebarMenu`);

import '../Content/styles/_shared/ui-grid/ui-grid-filter.scss';
/****/

/*для модуля buyMore*/
import '../../../Content/src/_shared/modal/offers-selectvizr/ModalOffersSelectvizr.mobile.module.js';
import '../../../Content/src/_partials/offers-selectvizr/offersSelectvizr.js';
import '../../../Content/src/_partials/offers-selectvizr/offersSelectvizr.component.js';
appDependency.addItem(`offersSelectvizr`);

import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';
/**/

/*для модуля отзывы магазина*/
import 'ng-file-upload';
appDependency.addItem(`ngFileUpload`);
/**/

/*simaland*/
// import '../Content/styles/_shared/chips/chips.scss';

import scrollIntoViewModule from '../../../../../scripts/_common/scroll-into-view/scroll-into-view.js';
appDependency.addItem(scrollIntoViewModule);

import '../Content/styles/_shared/bootstrap/grid.scss';
import '../Content/styles/_shared/onoffswitch/onoffswitch.scss';

import '../Content/vendors/ng-sortable-custom/ng-sortable.module.js';

import '../../../Content/src/exportfeeds/modal/addYandexGlobalDeliveryCost/ModalAddYandexGlobalDeliveryCostCtrl.js';
import '../../../Content/src/_shared/modal/export-products-selectvizr/ModalExportProductsSelectvizrCtrl.js';

import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.component.js';

import '../../../Content/src/_shared/picture-uploader/pictureUploader.js';
import '../../../Content/src/_shared/picture-uploader/pictureUploader.component.js';
import '../Content/styles/_shared/picture-uploader/picture-uploader.scss';

appDependency.addItem('module');

import '../../../Content/src/_shared/modal/products-selectvizr/ModalProductsSelectvizrCtrl.js';

import '../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
appDependency.addItem(`ngClickCapture`);

import '../../../Content/src/_shared/modal/editableGridRow/ModalEditableGridRow.js';

import '../Content/styles/_shared/product-list/product-list.scss';
import '../Content/styles/_shared/product-item/product-item.scss';

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
appDependency.addItem(`productGridItem`);

import '../../../Content/src/_shared/modal/addCategory/ModalAddCategoryCtrl.js';

import '../../../Content/vendors/cropper/cropper.min.js';
import '../../../Content/vendors/cropper/cropper.css';

import '../../../Content/vendors/cropper/ngCropper.js';
import '../../../Content/src/_shared/modal/cropImage/ModalCropImageCtrl.js';
appDependency.addItem(`ngCropper`);

import 'tinycolor2';
import '../../../Content/vendors/angular-color-picker/angularjs-color-picker.cjs';
import '../../../Content/vendors/angular-color-picker/angularjs-color-picker.css';
import '../../../Content/vendors/angular-color-picker/themes/angularjs-color-picker-bootstrap.css';

import '../Content/styles/_shared/color-picker/color-picker.scss';

import '../../../Content/src/_shared/modal/addExportFeed/ModalAddExportFeedCtrl.js';

import '../../../Content/src/_shared/modal/selectCustomer/ModalSelectCustomerCtrl.js';

import '../../../Content/src/product/components/productProperties/productProperties.js';
import '../../../Content/src/product/components/productProperties/productProperties.service.js';
appDependency.addItem('productProperties');
