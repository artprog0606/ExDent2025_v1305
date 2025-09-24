import appDependency from '../../../../../scripts/appDependency.js';

import 'ng-file-upload';

import '../../../Content/src/_shared/file-uploader/fileUploader.js';
import '../../../Content/src/_shared/file-uploader/fileUploader.component.js';
import '../../../Content/src/_shared/file-uploader/modal/fileUploaderModal.js';

import '../Content/vendors/ng-sortable-custom/ng-sortable.module.js';

import '../../../Content/src/settingsSystem/settingsSystem.js';
import '../../../Content/src/settingsSystem/location/settingsSystem.location.js';
import '../../../Content/src/settingsSystem/location/settingsSystem.location.component.js';
import '../../../Content/src/settingsSystem/location/settingsSystem.location.country.js';
import '../../../Content/src/settingsSystem/location/settingsSystem.location.country.component.js';
import '../../../Content/src/settingsSystem/location/settingsSystem.location.region.js';
import '../../../Content/src/settingsSystem/location/settingsSystem.location.region.component.js';
import '../../../Content/src/settingsSystem/location/settingsSystem.location.city.js';
import '../../../Content/src/settingsSystem/location/settingsSystem.location.city.component.js';
import '../../../Content/src/settingsSystem/location/modal/addEditCountry/ModalAddEditCountryCtrl.js';
import '../../../Content/src/settingsSystem/location/modal/addEditRegion/ModalAddEditRegionsCtrl.js';
import '../../../Content/src/settingsSystem/location/modal/addEditRegion/modal/ModalAddEditAdditionalSettingsRegionCtrl.js';
import '../../../Content/src/settingsSystem/location/modal/addEditCitys/ModalAddEditCitysCtrl.js';
import '../../../Content/src/settingsSystem/location/modal/addEditCitys/modal/ModalAddEditAdditionalSettingsCityCtrl.js';
import '../../../Content/src/settingsSystem/modal/importLocalization/ModalimportLocalizationCtrl.js';
import '../../../Content/src/settingsSystem/modal/addEditLocalization/ModalAddEditLocalizationCtrl.js';
import '../../../Content/src/settingsSystem/logs/settingsSystem.logs.js';
import '../../../Content/src/settingsSystem/logs/logs.scss';
import '../../../Content/src/settingsSystem/logs/settingsSystem.logs.component.js';
import '../../../Content/src/settingsSystem/logs/settingsSystem.logs.service.js';

import '../../../Content/src/_partials/admin-color-scheme/adminColorScheme.directive.js';
import '../../../Content/src/_partials/admin-color-scheme/adminColorScheme.service.js';

import '../../../Content/src/menus/menus.js';
import '../../../Content/src/menus/components/menu-item-actions/menuItemActions.js';
import '../../../Content/src/menus/components/menu-treeview/menuTreeview.component.js';
import '../../../Content/src/menus/components/menu-treeview/menuTreeview.js';
import '../../../Content/src/menus/modal/addEditMenuItem/ModalAddEditMenuItemCtrl.js';
import '../../../Content/src/menus/modal/changeParentMenuItem/ModalChangeParentMenuItemCtrl.js';

import '../../../Content/src/paymentMethods/components/paymentMethodsList/paymentMethodsList.js';

import '../Content/src/_shared/swipe-line/swipe-line.module.js';

import '../Content/styles/views/settings.scss';
import '../Content/styles/_shared/card/card.scss';

import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.modified.js';
import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.css';
appDependency.addItem(`ngCkeditor`);

import '../Content/styles/views/settings.scss';

import '../../../Content/src/import/import.js';
import '../../../Content/src/import/import.service.js';
import '../../../Content/src/import/import.module.js';

import '../Content/styles/_shared/picture-uploader/picture-uploader.scss';

appDependency.addItem(`swipeLine`);
appDependency.addItem('settingsSystem');
