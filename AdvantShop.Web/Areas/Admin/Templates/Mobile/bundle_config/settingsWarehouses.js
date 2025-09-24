import appDependency from '../../../../../scripts/appDependency.js';

import 'tinycolor2';
import 'ng-file-upload';

import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/views/settings.scss';
import '../Content/styles/views/settings-list.scss';
import '../../../Content/src/settingsWarehouses/settingsWarehouses.js';
import '../Content/src/_shared/swipe-line/swipe-line.module.js';

import '../../../Content/src/stockLabel/stockLabel.module.js';
import '../../../Content/src/stockLabel/modal/AddEditStockLabel/modalAddEditStockLabel.ctrl.js';

import domainGeoLocationModule from '../../../Content/src/domainGeoLocation/domainGeoLocation.module.js';
import '../../../Content/src/domainGeoLocation/modal/AddEditDomainGeoLocation/modalAddEditDomainGeoLocation.ctrl.js';

import '../../../Content/vendors/angular-color-picker/angularjs-color-picker.cjs';
import '../../../Content/vendors/angular-color-picker/angularjs-color-picker.css';
import '../../../Content/vendors/angular-color-picker/themes/angularjs-color-picker-bootstrap.css';
import '../Content/styles/_shared/color-picker/color-picker.scss';

import '../../../Content/src/_shared/modal/selectCustomer/ModalSelectCustomerCtrl.js';
import '../../../Content/src/_shared/modal/selectCities/ModalSelectCitiesCtrl.js';

appDependency.addItem('settingsWarehouses');
appDependency.addItem(`swipeLine`);
appDependency.addItem(`stockLabel`);
appDependency.addItem(domainGeoLocationModule);
appDependency.addItem(`ngFileUpload`);
appDependency.addItem('color.picker');
