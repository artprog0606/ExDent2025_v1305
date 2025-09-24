import appDependency from '../../../scripts/appDependency.js';

import '../../Admin/Content/src/_shared/modal/offers-selectvizr/ModalOffersSelectvizrCtrl.js';
import '../../Admin/Content/src/_shared/modal/products-selectvizr/ModalProductsSelectvizrCtrl.js';
import '../../Admin/Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';

import '../../Admin/Templates/Mobile/Content/styles/_shared/inputs/inputs.scss';

import '../../Admin/Templates/Mobile/Content/src/product/styles/product.scss';
import '../../Admin/Templates/Mobile/Content/styles/_shared/product-list/product-list.scss';
import '../../Admin/Templates/Mobile/Content/styles/_shared/product-item/product-item.scss';
import '../../Admin/Templates/Mobile/Content/styles/_shared/card/card.scss';

import '../../Admin/Templates/Mobile/Content/styles/_shared/ui-grid/ui-grid.scss';
import '../../Admin/Templates/Mobile/Content/styles/_shared/ui-grid/ui-grid-tree.scss';
import '../../Admin/Templates/Mobile/Content/styles/_shared/ui-grid/ui-grid-selection.scss';
import '../../Admin/Templates/Mobile/Content/styles/_shared/ui-grid/ui-grid-filter.scss';

import '../../Admin/Templates/Mobile/Content/src/_shared/ui-grid-custom/styles/ui-grid.custom.scss';
import '../../Admin/Templates/Mobile/Content/src/_shared/ui-grid-custom/styles/ui-grid-custom.mobile.scss';
import '../../Admin/Templates/Mobile/Content/src/_shared/ui-grid-custom/styles/ui-grid.custom.edit.scss';

import '../../Admin/Templates/Mobile/Content/styles/_shared/bootstrap/atomic.scss';
import '../../Admin/Templates/Mobile/Content/styles/_shared/bootstrap/grid.scss';
//import '../../Admin/Templates/Mobile/Content/styles/_shared/bootstrap/labels.scss';
import '../../Admin/Templates/Mobile/Content/styles/_shared/bootstrap/bootstrap.scss';
import '../../Admin/Templates/Mobile/Content/styles/_shared/paginations/paginations.scss';

//import '../../Admin/Templates/Mobile/Content/styles/common.scss';
import '../../Admin/Templates/Mobile/Content/styles/_shared/jstree/jstree.scss';
import '../../Admin/Templates/Mobile/Content/styles/_shared/modal/modal.scss';
import '../../Admin/Templates/Mobile/Content/styles/_shared/buttons/buttons.scss';

import swipeLineModule from '../../Admin/Templates/Mobile/Content/src/_shared/swipe-line/swipe-line.module.js';
appDependency.addItem(swipeLineModule);

import '../../Admin/Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
appDependency.addItem(`ngClickCapture`);

import '../../Admin/Templates/Mobile/Content/src/_shared/product-grid-item/productGridItem.directive.js';
appDependency.addItem(`productGridItem`);
