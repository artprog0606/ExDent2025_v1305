import appDependency from '../../../../../scripts/appDependency.js';
import '../../../Content/src/category/category.js';
import '../../../Content/src/category/modal/changeParentCategory/ModalChangeParentCategoryCtrl.js';
import '../../../Content/src/category/styles/category.scss';
import '../../../Content/src/_shared/url-generator/urlGenerator.js';
import 'angular-inview';
appDependency.addItem(`angular-inview`);
appDependency.addItem(`category`);

import '../Content/src/product/styles/product.scss';
import '../Content/styles/_shared/product-list/product-list.scss';
import '../Content/styles/_shared/product-item/product-item.scss';

import '../../../Content/src/_partials/offers-selectvizr/offersSelectvizr.js';
import '../../../Content/src/_partials/offers-selectvizr/offersSelectvizr.component.js';
import '../../../Content/src/_shared/modal/offers-selectvizr/ModalOffersSelectvizrCtrl.js';
appDependency.addItem(`offersSelectvizr`);

import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.modified.js';
import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.css';
appDependency.addItem(`ngCkeditor`);

import 'ng-file-upload';
appDependency.addItem(`ngFileUpload`);

import '../../../Content/src/_shared/picture-uploader/pictureUploader.js';
import '../../../Content/src/_shared/picture-uploader/pictureUploader.component.js';
import '../Content/styles/_shared/picture-uploader/picture-uploader.scss';
appDependency.addItem(`pictureUploader`);
import '../../../Content/src/_shared/picture-uploader/modal/pictureUploaderModal.js';
import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/views/category.scss';
import '../Content/styles/_shared/bootstrap/grid.scss';

import '../../../Content/src/category/modal/addPropertyGroup/ModalAddPropertyGroupCtrl.js';
import '../../../Content/src/category/components/catProductRecommendations.js';
appDependency.addItem(`catProductRecommendations`);
import '../../../Content/src/_shared/modal/addCategory/ModalAddCategoryCtrl.js';
import '../../../Content/src/category/modal/addRecomProperty/ModalAddRecomPropertyCtrl.js';
import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';
import '../Content/src/_shared/swipe-line/swipe-line.module.js';
appDependency.addItem(`swipeLine`);

import '../../../Content/src/_shared/change-history-modal/ModalChangeHistoryCtrl.js';
import '../../../Content/src/_partials/change-history/changeHistory.js';
import '../../../Content/src/_partials/change-history/changeHistory.component.js';

appDependency.addItem(`changeHistory`);

import '../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
appDependency.addItem(`ngClickCapture`);

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
appDependency.addItem(`productGridItem`);

import '../../../Content/src/sizeChart/modal/addSizeChartProperty/ModalAddSizeChartPropertyCtrl.js';
