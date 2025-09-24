import appDependency from '../../../scripts/appDependency.js';

import '../../../fonts/fonts.admin.css';
//import '../../../node_modules/jquery/dist/jquery.js';
//Ui bootstrap
import popoverModule from 'angular-ui-bootstrap/src/popover/index.js';
appDependency.addItem(popoverModule);
import tabsModule from 'angular-ui-bootstrap/src/tabs/index.js';
appDependency.addItem(tabsModule);
import paginationModule from 'angular-ui-bootstrap/src/pagination/index.js';
appDependency.addItem(paginationModule);
import modalModule from 'angular-ui-bootstrap/src/modal/index.js';
appDependency.addItem(modalModule);
import typeaheadModule from 'angular-ui-bootstrap/src/typeahead/index.js';
appDependency.addItem(typeaheadModule);
import dropdownModule from 'angular-ui-bootstrap/src/dropdown/index.js';
appDependency.addItem(dropdownModule);
import '../../Admin/Content/vendors/ui-bootstrap/angular-tab-decorator/angular-tab-decorator.js';
import '../../Admin/Content/vendors/ui-bootstrap/angular-typehead-decorator/angular-typehead-decorator.js';
import '../../Admin/Content/src/_shared/modal/uiModal.js';
import '../../Admin/Content/src/_shared/modal/uiModal.component.js';
import '../../Admin/Content/src/_shared/modal/uiModalDecorator.js';
appDependency.addItem(`uiModal`);

import '../../Admin/Content/vendors/jsTree.directive/jsTree.directive.custom.js';
import '../../Admin/Content/vendors/jsTree.directive/themes/advantshop/style.css';
appDependency.addItem(`jsTree.directive`);

import '../../Admin/Content/src/_shared/ui-grid-custom/uiGridCustom.module.all.js';
appDependency.addItem(`uiGridCustomSelection`);
appDependency.addItem(`uiGridCustom`);

import '../../../scripts/_common/input/input.module.js';
appDependency.addItem(`input`);

import '../../Admin/Content/src/_shared/input/styles/input.scss';

import '../../Admin/Content/vendors/cropper/cropper.min.js';
import '../../Admin/Content/vendors/cropper/cropper.css';
import '../../Admin/Content/vendors/cropper/ngCropper.js';
import '../../Admin/Content/src/_shared/modal/cropImage/ModalCropImageCtrl.js';
appDependency.addItem(`ngCropper`);

import uiAceTextarea from '../../Admin/Content/src/_shared/ui-ace-textarea/uiAceTextarea.module.js';
appDependency.addItem(uiAceTextarea);

import '../frontend/_common/lp-settings/styles/lp-settings.scss';
import '../frontend/_common/inplace-landing/styles/inplaceLanding.scss';
import '../frontend/_common/blocks-constructor/styles/blocksConstructor.scss';
import '../frontend/_common/background-picker/styles/backgroundPicker.scss';
import '../frontend/_common/subblock-inplace/styles/subblockInplace.scss';
import '../frontend/_common/picture-loader/styles/picture-loader.scss';
import '../frontend/_common/galleryCloud/galleryCloud.scss';
import '../frontend/_common/galleryIcons/galleryIcons.scss';
import '../frontend/_common/lp-grid/lpGrid.scss';
import '../frontend/_common/lp-tabs.scss';

import '../frontend/blocks/gallery/styles/gallery-admin.scss';
import '../frontend/blocks/lp-admin-menu/lp-admin-menu.scss';

import '../frontend/_common/edit-mode.scss';

import '../frontend/_common/video-file-uploader/style/style.scss';

import '../../Admin/Content/src/_shared/switch-on-off/switchOnOff.js';
appDependency.addItem(`switchOnOff`);

import '../../Admin/Content/src/_partials/products-selectvizr/productsSelectvizr.js';
import '../../Admin/Content/src/_partials/products-selectvizr/productsSelectvizr.component.js';
appDependency.addItem(`productsSelectvizr`);

import '../../Admin/Content/src/_partials/offers-selectvizr/offersSelectvizr.js';
import '../../Admin/Content/src/_partials/offers-selectvizr/offersSelectvizr.component.js';
appDependency.addItem(`offersSelectvizr`);

import switcherStateModule from '../../Admin/Content/src/_shared/switcher-state/switcherState.module.js';
appDependency.addItem(switcherStateModule);

import '../../Admin/Content/vendors/ng-ckeditor/ng-ckeditor.module.js';
appDependency.addItem(`ngCkeditor`);

import helpTriggerModule from '../../Admin/Content/src/_partials/help-trigger/helpTrigger.module.js';
appDependency.addItem(helpTriggerModule);

import '../frontend/vendors/fontawesome/svg-with-js/js/fontawesome-all.js';

import '../../Admin/Content/vendors/ng-sortable-custom/ng-sortable.module.js';
appDependency.addItem(`as.sortable`);

import 'tinycolor2';
import '../../Admin/Content/vendors/angular-color-picker/angularjs-color-picker.cjs';
import '../../Admin/Content/vendors/angular-color-picker/angularjs-color-picker.css';
import '../../Admin/Content/vendors/angular-color-picker/themes/angularjs-color-picker-bootstrap.css';
appDependency.addItem(`color.picker`);

import '../frontend/_common/inplace-landing/inplaceLanding.js';
import '../frontend/_common/inplace-landing/controllers/inplaceLandingSwitchController.js';
import '../frontend/_common/inplace-landing/directives/inplaceLandingDirectives.js';
appDependency.addItem(`inplaceLanding`);

import '../frontend/_common/blocks-constructor/blocksConstructor.js';
import '../frontend/_common/blocks-constructor/constants/blocksConstructorConstants.js';
import '../frontend/_common/blocks-constructor/controllers/blocksConstructorController.js';
import '../frontend/_common/blocks-constructor/controllers/blocksConstructorMainController.js';
import '../frontend/_common/blocks-constructor/controllers/blocksConstructorContainerController.js';
import '../frontend/_common/blocks-constructor/controllers/blocksConstructorNewBlockController.js';
import '../frontend/_common/blocks-constructor/controllers/blocksConstructorSettingsBlockController.js';
import '../frontend/_common/blocks-constructor/controllers/blocksConstructorAddSubblockController.js';
import '../frontend/_common/blocks-constructor/controllers/blocksConstructorAddCategoryController.js';
import '../frontend/_common/blocks-constructor/controllers/blocksConstructorButtonSettingsController.js';
import '../frontend/_common/blocks-constructor/controllers/blocksConstructorButtonSettingsModalController.js';
import '../frontend/_common/blocks-constructor/controllers/blocksConstructorFormSettingsController.js';
import '../frontend/_common/blocks-constructor/controllers/blocksConstructorFormSettingsTabController.js';
import '../frontend/_common/blocks-constructor/components/blocksConstructorComponent.js';
import '../frontend/_common/blocks-constructor/services/blocksConstructorService.js';
import '../frontend/_common/blocks-constructor/filters/blocksConstructorFilters.js';
import '../frontend/_common/blocks-constructor/controllers/sublocks/SubBlockBookingConstructorController.js';
import '../frontend/_common/blocks-constructor/controllers/sublocks/SubBlockProductsViewConstructorController.js';
import '../frontend/_common/blocks-constructor/controllers/sublocks/SubBlockGalleryConstructorController.js';
import '../frontend/_common/blocks-constructor/controllers/sublocks/SubBlockCountdownViewConstructorController.js';
import '../frontend/_common/blocks-constructor/controllers/sublocks/CategorySelectorConstructorController.js';
appDependency.addItem(`blocksConstructor`);

import '../frontend/_common/lp-settings/lpSettings.js';
import '../frontend/_common/lp-settings/components/lpSettingsTrigger.js';
import '../frontend/_common/lp-settings/controllers/lpSettingsController.js';
import '../frontend/_common/lp-settings/controllers/lpSettingsTriggerController.js';
import '../frontend/_common/lp-settings/services/lpSettingsService.js';
appDependency.addItem(`lpSettings`);

import '../frontend/_common/subblock-inplace/subblockInplace.js';
import '../frontend/_common/subblock-inplace/constants/subblockInplaceColors.js';
import '../frontend/_common/subblock-inplace/controllers/subblockInplaceController.js';
import '../frontend/_common/subblock-inplace/controllers/subblockInplaceButtonController.js';
import '../frontend/_common/subblock-inplace/controllers/SubblockInplaceButtonModalController.js';
import '../frontend/_common/subblock-inplace/controllers/subblockInplacePriceController.js';
import '../frontend/_common/subblock-inplace/controllers/subblockInplacePriceModalController.js';
import '../frontend/_common/subblock-inplace/controllers/subblockInplaceVideoController.js';
import '../frontend/_common/subblock-inplace/controllers/subblockInplaceVideoModalController.js';
import '../frontend/_common/subblock-inplace/controllers/subblockInplaceBuyFormController.js';
import '../frontend/_common/subblock-inplace/controllers/subblockInplaceBuyFormModalController.js';
import '../frontend/_common/subblock-inplace/directives/subblockInplaceDirective.js';
import '../frontend/_common/subblock-inplace/services/subblockInplaceService.js';
appDependency.addItem(`subblockInplace`);

import '../frontend/_common/gradient-picker/gradientPicker.js';
import '../frontend/_common/gradient-picker/controllers/gradientPickerController.js';
import '../frontend/_common/gradient-picker/components/gradientPickerComponent.js';
import '../frontend/_common/gradient-picker/services/gradientPickerService.js';
appDependency.addItem(`gradientPicker`);

import '../frontend/_common/background-picker/backgroundPicker.js';
import '../frontend/_common/background-picker/controllers/backgroundPickerController.js';
import '../frontend/_common/background-picker/components/backgroundPickerComponent.js';
import '../frontend/_common/background-picker/services/backgroundPickerService.js';
appDependency.addItem(`backgroundPicker`);

import '../frontend/_common/picture-loader/picture-loader.js';
import '../frontend/_common/picture-loader/picture-loader.constants.js';
import '../frontend/_common/picture-loader/controllers/pictureLoaderController.js';
import '../frontend/_common/picture-loader/controllers/pictureLoaderTriggerController.js';
import '../frontend/_common/picture-loader/components/pictureLoaderComponent.js';
import '../frontend/_common/picture-loader/services/pictureLoaderService.js';
appDependency.addItem(`pictureLoader`);

import '../frontend/_common/color-scheme-settings/colorSchemeSettings.js';
import '../frontend/_common/color-scheme-settings/colorSchemeSettingsController.js';
import '../frontend/_common/color-scheme-settings/colorSchemeSettings.component.js';
appDependency.addItem(`colorSchemeSettings`);

import '../frontend/_common/galleryCloud/galleryCloud.js';
import '../frontend/_common/galleryCloud/galleryCloud.ctrl.js';
import '../frontend/_common/galleryCloud/galleryCloud.service.js';
import '../frontend/_common/galleryCloud/galleryCloud.component.js';
appDependency.addItem(`galleryCloud`);

import '../frontend/_common/galleryIcons/galleryIcons.js';
import '../frontend/_common/galleryIcons/galleryIcons.ctrl.js';
import '../frontend/_common/galleryIcons/galleryIcons.service.js';
import '../frontend/_common/galleryIcons/galleryIcons.component.js';
import '../frontend/_common/galleryIcons/galleryIcons.filter.js';
appDependency.addItem(`galleryIcons`);

import '../frontend/_common/lp-grid/lpGrid.js';
import '../frontend/_common/lp-grid/lpGrid.constant.js';
import '../frontend/_common/lp-grid/lpGrid.ctrl.js';
import '../frontend/_common/lp-grid/lpGridColumn.ctrl.js';
import '../frontend/_common/lp-grid/lpGridGroup.ctrl.js';
import '../frontend/_common/lp-grid/lpAddGroupGrid.ctrl.js';
import '../frontend/_common/lp-grid/lpGridColumnTemplate.ctrl.js';
import '../frontend/_common/lp-grid/lpGridColumns.ctrl.js';
import '../frontend/_common/lp-grid/lpGridCkeditorModal.ctrl.js';
import '../frontend/_common/lp-grid/lpGridModel.ctrl.js';
import '../frontend/_common/lp-grid/lpGrid.service.js';
import '../frontend/_common/lp-grid/lpGrid.component.js';
appDependency.addItem(`lpGrid`);

import '../frontend/_common/video-file-uploader/videoFileUploader.js';
import '../frontend/_common/video-file-uploader/videoFileUploader.ctrl.js';
import '../frontend/_common/video-file-uploader/videoFileUploader.directive.js';
appDependency.addItem(`videoFileUploader`);
