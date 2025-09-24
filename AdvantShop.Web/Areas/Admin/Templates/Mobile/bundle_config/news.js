import appDependency from '../../../../../scripts/appDependency.js';
import '../../../Content/src/newsItem/newsItem.js';
import '../../../Content/src/newsItem/components/newsProducts/newsProducts.js';
appDependency.addItem(`newsItem`);

import '../../../Content/src/_shared/url-generator/urlGenerator.js';
import flatpickrModule from '../../../../../vendors/flatpickr/flatpickr.module.js';
import '../Content/styles/_shared/flatpickr/flatpickr.scss';
appDependency.addItem(flatpickrModule);

import 'ng-file-upload';
import '../../../Content/src/_shared/picture-uploader/pictureUploader.js';
import '../../../Content/src/_shared/picture-uploader/pictureUploader.component.js';
import '../Content/styles/_shared/picture-uploader/picture-uploader.scss';
import '../../../Content/src/_shared/picture-uploader/modal/pictureUploaderModal.js';
appDependency.addItem('pictureUploader');

import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';
import '../../../Content/src/_shared/modal/products-selectvizr/ModalProductsSelectvizr.mobile.module.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.component.js';
appDependency.addItem(`productsSelectvizr`);

import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/views/settings.scss';

import '../../../Content/src/_shared/icon-move/iconMove.js';
import '../../../Content/src/_shared/icon-move/styles/icon-move.css';
appDependency.addItem(`iconMove`);

import '../Content/vendors/ng-sortable-custom/ng-sortable.module.js';

appDependency.addItem(`as.sortable`);

import '../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
appDependency.addItem(`ngClickCapture`);

import '../../../Content/src/_shared/modal/editableGridRow/ModalEditableGridRow.js';

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
appDependency.addItem(`productGridItem`);
