import appDependency from '../../../../../scripts/appDependency.js';

import '../Content/vendors/ng-sortable-custom/ng-sortable.module.js';

import sidebarMenuModule from '../../../Content/src/_partials/sidebar-menu/sidebarMenu.module.js';
appDependency.addItem(sidebarMenuModule);

import '../../../Content/src/_shared/modal/addCategory/ModalAddCategoryCtrl.js';
import '../../../Content/src/_shared/modal/addBrand/ModalAddBrandCtrl.js';
import '../../../Content/src/reviews/modal/addEditReview/ModalAddEditReviewCtrl.js';
import '../../../Content/src/_shared/picture-uploader/modal/pictureUploaderModal.js';
import '../../../Content/src/_shared/modal/searchImages/ModalSearchImagesCtrl.js';
import '../../../Content/src/_shared/modal/addLandingSite/ModalAddLandingSiteCtrl.js';
import '../../../Content/src/_shared/modal/products-selectvizr/ModalProductsSelectvizrCtrl.js';
import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.component.js';

import '../../../Content/src/_partials/change-history/changeHistory.js';
import '../../../Content/src/_partials/change-history/changeHistory.component.js';
import '../../../Content/src/_shared/change-history-modal/ModalChangeHistoryCtrl.js';
appDependency.addItem('changeHistory');

import productModule from '../../../Content/src/product/product.module.js';
appDependency.addItem(productModule);

import '../../../../../scripts/_common/spinbox/spinbox.module.js';
appDependency.addItem('spinbox');

import '../Content/styles/_shared/ui-grid/ui-grid-selection.scss';
import '../Content/src/_shared/swipe-line/swipe-line.module.js';
appDependency.addItem(`swipeLine`);

import scrollIntoViewModule from '../../../../../scripts/_common/scroll-into-view/scroll-into-view.js';
appDependency.addItem(scrollIntoViewModule);

import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/_shared/chips/chips.scss';
import '../Content/styles/_shared/product-item/product-item.scss';
import '../Content/styles/_shared/product-list/product-list.scss';
import '../Content/src/product/styles/product.scss';
import '../Content/styles/views/settings-list.scss';

import 'checklist-model';

import '../../../Content/src/_shared/modal/offers-selectvizr/ModalOffersSelectvizrCtrl.js';

import '../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
appDependency.addItem(`ngClickCapture`);

import '../../../Content/src/module/module.js';
import '../../../Content/src/module/styles/module.css';

import '../../../Content/src/_shared/modal/editableGridRow/ModalEditableGridRow.js';

import '../../../Content/src/_shared/picture-uploader/pictureUploader.js';
import '../../../Content/src/_shared/picture-uploader/pictureUploader.component.js';
import '../../../Content/src/_shared/picture-uploader/styles/picture-uploader.scss';
import '../../../Content/src/_shared/picture-uploader/modal/pictureUploaderModal.js';

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
appDependency.addItem(`productGridItem`);

import '../../../Content/src/_shared/modal/addLandingSite/addProductLanding.directive.js';
import '../Content/styles/_shared/product-list/product-list.scss';
import '../Content/styles/views/createsite.scss';
appDependency.addItem('addProductLanding');

/* для модуля Дополнительные маркеры*/
import 'tinycolor2';
import '../../../Content/vendors/angular-color-picker/angularjs-color-picker.cjs';
import '../../../Content/vendors/angular-color-picker/angularjs-color-picker.css';
import '../../../Content/vendors/angular-color-picker/themes/angularjs-color-picker-bootstrap.css';
import '../Content/styles/_shared/color-picker/color-picker.scss';

import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.modified.js';
import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.css';
appDependency.addItem(`ngCkeditor`);

import '../../../../../scripts/_common/iframe-responsive/iframeResponsive.module.js';
appDependency.addItem('iframeResponsive');

angular.module('app').config(
    /*@ngInject*/
    function ($anchorScrollProvider) {
        $anchorScrollProvider.disableAutoScrolling();
    },
);
