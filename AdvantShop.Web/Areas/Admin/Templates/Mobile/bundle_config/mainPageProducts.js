import appDependency from '../../../../../scripts/appDependency.js';
import '../../../Content/src/mainpageproducts/mainpageproducts.js';
import '../../../Content/src/mainpageproducts/mainpageproducts.service.js';
import '../../../Content/src/mainpageproducts/components/productListsMenu/productListsMenu.js';
import '../../../Content/src/mainpageproducts/modal/editMainPageList/ModalEditMainPageListCtrl.js';
appDependency.addItem(`mainpageproducts`);

import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.component.js';
// import 'ng-sortable';

import '../../../Content/src/productlists/productlists.js';
import '../../../Content/src/productlists/modal/addEditProductList/ModalAddEditProductListCtrl.js';
appDependency.addItem(`productlists`);

import '../Content/styles/views/settings.scss';
import '../Content/styles/_shared/navigation-item/navigation-item.scss';
import '../Content/styles/_shared/card/card.scss';

import '../Content/vendors/ng-sortable-custom/ng-sortable.module.js';
// appDependency.addItem(`as.sortable`);

import '../../../Content/src/_shared/icon-move/iconMove.js';
import '../../../Content/src/_shared/icon-move/styles/icon-move.css';
appDependency.addItem(`iconMove`);

import '../../../Content/src/_shared/modal/moveProductInOtherCategory/ModalMoveProductInOtherCategoryCtrl.js';

import '../../../Content/src/_shared/modal/products-selectvizr/ModalProductsSelectvizr.mobile.module.js';

import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';

import '../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
appDependency.addItem(`ngClickCapture`);

import '../../../Content/src/_shared/modal/editableGridRow/ModalEditableGridRow.js';

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
appDependency.addItem(`productGridItem`);

import '../../../Content/src/product/modal/copyProduct/ModalCopyProductCtrl.js';

import '../../../../../scripts/_common/spinbox/spinbox.module.js';
appDependency.addItem(`spinbox`);

import '../../../Content/src/_shared/modal/addRemovePropertyToProducts/ModalAddRemovePropertyToProductsCtrl.js';
