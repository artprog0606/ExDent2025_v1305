import appDependency from '../../../../../scripts/appDependency.js';

import '../Content/vendors/ng-sortable-custom/ng-sortable.module.js';

import '../../../Content/src/settingsCatalog/settingsCatalog.js';
import '../../../Content/src/settingsCatalog/modal/addEditCurrency/ModalAddEditCurrencyCtrl.js';
import '../../../Content/src/settingsCatalog/modal/resultPriceRegulation/ResultPriceRegulationCtrl.js';
import '../../../Content/src/settingsCatalog/modal/resultCategoryDiscountRegulation/ResultCategoryDiscountRegulationCtrl.js';

import '../../../Content/src/colors/colors.js';
import '../../../Content/src/colors/modal/addEditColor/ModalAddEditColorCtrl.js';
import '../../../Content/src/colors/modal/importColors/ModalImportColorsCtrl.js';

import '../../../Content/src/brandsList/brandsList.js';
import '../../../Content/src/brandsList/brandsList.service.js';

import '../../../Content/src/sizes/sizes.js';
import '../../../Content/src/sizes/modal/addEditSize/ModalAddEditSizeCtrl.js';
import '../../../Content/src/sizes/modal/addEditSizeNameForCategories/ModalAddEditSizeNameForCategoriesCtrl.js';

import '../../../Content/src/properties/properties.js';
import '../../../Content/src/properties/modal/addEditProperty/ModalAddEditPropertyCtrl.js';
import '../../../Content/src/properties/modal/changeGroups/ModalChangeGroupsCtrl.js';
import '../../../Content/src/properties/modal/changePropertyGroup/ModalChangePropertyGroupCtrl.js';
import '../../../Content/src/properties/modal/addGroup/ModalAddGroupCtrl.js';
import '../../../Content/src/properties/components/propertyGroups/propertyGroups.js';

import '../../../Content/src/propertyvalues/propertyvalues.js';
import '../../../Content/src/propertyvalues/modal/addPropertyValue/ModalAddPropertyValueCtrl.js';

import '../../../Content/src/import/import.module.js';
import '../../../Content/src/tags/tags.js';

import '../Content/styles/views/settings.scss';

import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/views/settings-list.scss';
import '../Content/src/_shared/swipe-line/swipe-line.module.js';

import '../Content/styles/_shared/color-picker/color-picker.scss';

import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.modified.js';
import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.css';

import '../../../Content/src/_shared/picture-uploader/pictureUploader.js';
import '../../../Content/src/_shared/picture-uploader/pictureUploader.component.js';
import '../../../Content/src/_shared/picture-uploader/modal/pictureUploaderModal.js';
import '../../../Content/src/_shared/file-uploader/fileUploader.js';
import '../../../Content/src/_shared/file-uploader/fileUploader.component.js';
import '../../../Content/src/_shared/file-uploader/modal/fileUploaderModal.js';

import 'ng-file-upload';
appDependency.addItem(`ngFileUpload`);

import '../../../Content/src/brand/brand.js';
import '../../../Content/src/brandsList/brandsList.js';
import '../../../Content/src/brandsList/brandsList.service.js';
import '../../../Content/src/brand/brand.js';
import '../../../Content/src/_shared/modal/addBrand/ModalAddBrandCtrl.js';

import '../../../Content/src/_shared/url-generator/urlGenerator.js';

import 'tinycolor2';
import '../../../Content/vendors/angular-color-picker/angularjs-color-picker.cjs';
import '../../../Content/vendors/angular-color-picker/angularjs-color-picker.css';
import '../../../Content/vendors/angular-color-picker/themes/angularjs-color-picker-bootstrap.css';

import '../../../Content/src/priceRules/priceRules.js';
import '../../../Content/src/priceRules/modal/addEditPriceRule/ModalAddEditPriceRuleCtrl.js';

import '../../../Content/src/units/units.js';
import '../../../Content/src/units/modal/addEditUnit/ModalAddEditUnitCtrl.js';

import '../../../Content/src/_shared/modal/addTag/ModalAddTagCtrl.js';

import '../../../Content/src/photoCategory/photoCategory.js';
import '../../../Content/src/photoCategory/modal/addEditPhotoCategory/ModalAddEditPhotoCategoryCtrl.js';

import '../../../Content/src/sizeChart/sizeChart.js';
import '../../../Content/src/sizeChart/modal/addEditSizeChart/ModalAddEditSizeChartCtrl.js';

appDependency.addItem(`ngCkeditor`);

appDependency.addItem(`swipeLine`);
appDependency.addItem('settingsCatalog');

import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';
import '../../../Content/src/sizeChart/modal/addEditSizeChart/ModalAddEditSizeChartCtrl.js';
import '../../../Content/src/sizeChart/modal/addSizeChartProperty/ModalAddSizeChartPropertyCtrl.js';
import modalProductsSelectvizrModule from '../../../Content/src/_shared/modal/products-selectvizr/ModalProductsSelectvizr.mobile.module.js';
appDependency.addItem(modalProductsSelectvizrModule);
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.component.js';
appDependency.addItem(`productsSelectvizr`);

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
appDependency.addItem(`productGridItem`);
