import '../Content/styles/_shared/chips/chips.scss';
import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/_shared/carousel/styles/carousel.scss';
import './design.js';
import '../../../Content/src/dashboardSites/components/createSite/createSite.js';
import appDependency from '../../../../../scripts/appDependency.js';
import '../../../../../scripts/_common/carousel/carousel.module.js';
appDependency.addItem('createSite');
import '../../../Content/src/dashboardSites/modals/qrCodeGenerator/ModalQrCodeGeneratorCtrl.js';

import '../../../Content/src/_shared/modal/addLandingSite/ModalAddLandingSiteCtrl.js';

import productsSelectvizrModalModule from '../../../Content/src/_shared/modal/products-selectvizr/ModalProductsSelectvizr.mobile.module.js';
appDependency.addItem(productsSelectvizrModalModule);

import '../../../Content/src/_shared/modal/addLandingSite/addProductLanding.directive.js';
import '../Content/styles/_shared/product-list/product-list.scss';
appDependency.addItem('addProductLanding');

import '../../../Content/src/_shared/modal/addLandingSite/addOfferLanding.directive.js';
appDependency.addItem('addOfferLanding');

import scrollIntoViewModule from '../../../../../scripts/_common/scroll-into-view/scroll-into-view.js';
appDependency.addItem(scrollIntoViewModule);

import '../Content/styles/views/createsite.scss';

import 'ng-infinite-scroll';

import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';

import '../../../Content/src/_shared/modal/editableGridRow/ModalEditableGridRow.js';
import '../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
appDependency.addItem(`ngClickCapture`);

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
appDependency.addItem(`productGridItem`);
