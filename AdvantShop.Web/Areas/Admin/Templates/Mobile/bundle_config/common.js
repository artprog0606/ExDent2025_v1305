import '../../../../../fonts/fa.scss';

//JQuery
// import 'jquery';
import '../../../../../vendors/jquery/jquery.passive.js';

//Angular
import '../../../../../node_modules/angular/angular.js';
import '../../../../../vendors/angular/i18n/angular-locale_ru-ru.js';
import '../../../../../vendors/stop-angular-overrides/stop-angular-overrides.js';
import appDependency from '../../../../../scripts/appDependency.js';
import '../../../../../node_modules/angular-cookies/angular-cookies.js';
appDependency.addItem(`ngCookies`);
import '../../../../../node_modules/angular-sanitize/angular-sanitize.js';
appDependency.addItem(`ngSanitize`);

//IWC  SignalR
import '../../../../../vendors/signalr/jquery.signalR.js';
import '../../../Content/vendors/iwc/iwc.module.js';
import '../../../Content/vendors/iwc-signalr/signalr-patch.js';
import '../../../Content/vendors/iwc-signalr/iwc-signalr.js';

import '../../../Content/vendors/angular-web-notification/desktop-notify.module.js';
import '../../../Content/vendors/angular-web-notification/angular-web-notification.js';
appDependency.addItem(`angular-web-notification`);
import adminWebNotificationsModule from '../../../Content/src/_shared/admin-web-notifications/adminWebNotifications.module.js';
import '../Content/src/_shared/admin-web-notifications/styles/admin-web-notifications.scss';
appDependency.addItem(adminWebNotificationsModule);

import '../../../../../node_modules/angular-translate/dist/angular-translate.js';
appDependency.addItem(`pascalprecht.translate`);
import '../../../../../node_modules/angularjs-toaster/toaster.js';
import '../../../../../node_modules/angularjs-toaster/toaster.min.css';
import '../Content/styles/_shared/toast/toast.scss';
appDependency.addItem(`toaster`);
import '../../../../../vendors/angular-bind-html-compile/angular-bind-html-compile.js';
appDependency.addItem(`angular-bind-html-compile`);
import flatpickrModule from '../../../../../vendors/flatpickr/flatpickr.module.js';
import '../Content/styles/_shared/flatpickr/flatpickr.scss';
appDependency.addItem(flatpickrModule);
//import '../../../Content/vendors/angular-locale/angular-locale_ru-ru'; подключать в footerScripts.cshtml

//ocLazyLoad
import '../../../../../node_modules/oclazyload/dist/ocLazyLoad.js';
import '../../../../../vendors/ocLazyLoad/ocLazyLoad.decorate.js';
appDependency.addItem(`oc.lazyLoad`);

//sweetalert2
import '../../../../../vendors/sweetalert/sweetalert2.default.js';
import '../../../../../vendors/ng-sweet-alert/ng-sweet-alert.js';
appDependency.addItem(`ng-sweet-alert`);

import '../../../../../scripts/_common/dom/dom.module.js';
appDependency.addItem('dom');
import '../../../../../vendors/autofocus/autofocus.js';
appDependency.addItem(`autofocus`);

import fullHeightMobileModule from '../../../../../scripts/_mobile/full-height-mobile/full-height-mobile.module.js';
appDependency.addItem(fullHeightMobileModule);

import '../../../Content/src/_shared/is-mobile/is-mobile.js';
appDependency.addItem('isMobile');

import '../../../../../scripts/_common/setCssCustomProps/setCssCustomProps.module.js';
import urlHelperModule from '../../../../../scripts/_common/urlHelper/urlHelperService.module.js';
appDependency.addItem(urlHelperModule);

import '../../../Content/vendors/jsTree.directive/jsTree.directive.custom.js';
import '../Content/styles/_shared/jstree/jstree.scss';
appDependency.addItem(`jsTree.directive`);
import '../../../Content/src/_shared/switch-on-off/switchOnOff.js';
import '../Content/styles/_shared/onoffswitch/onoffswitch.scss';

appDependency.addItem(`switchOnOff`);
import '../../../Content/src/_shared/switcher-state/switcherState.module.js';

//import '../../../Content/src/_shared/switcher-state/switcherState.js';
//import '../../../Content/src/_shared/switcher-state/switcherState.component.js';
appDependency.addItem(`switcherState`);
import '../../../Content/src/_shared/statistics/statistics.js';
import '../../../Content/src/_shared/statistics/statistics.service.js';
appDependency.addItem(`statistics`);
import sidebarsContainerModule from '../../../../../Areas/Mobile/scripts/_common/sidebarsContainer/sidebarsContainer.module.js';
appDependency.addItem(sidebarsContainerModule);
import setCssCustomPropsModule from '../../../../../scripts/_common/setCssCustomProps/setCssCustomProps.module.js';
appDependency.addItem(setCssCustomPropsModule);
// import '../Content/src/_shared/details/details.js';

//Ui bootstrap
import popover from 'angular-ui-bootstrap/src/popover/index.js';
import tabs from 'angular-ui-bootstrap/src/tabs/index.js';
import modal from 'angular-ui-bootstrap/src/modal/index.js';
import pagination from 'angular-ui-bootstrap/src/pagination/index.js';
import typeahead from 'angular-ui-bootstrap/src/typeahead/index.js';
import progressbar from 'angular-ui-bootstrap/src/progressbar/index.js';
appDependency.addList([popover, tabs, modal, pagination, typeahead, progressbar]);

import '../../../Content/vendors/ui-bootstrap/angular-tab-decorator/angular-tab-decorator.js';
import '../../../Content/vendors/ui-bootstrap/angular-typehead-decorator/angular-typehead-decorator.js';
import '../../../Content/src/_shared/modal/uiModal.js';

// import '../../../Content/src/_shared/modal/styles/uimodal.scss';
// import '../../../Content/src/_shared/modal/styles/simpleUiModal.scss';

import '../../../Content/src/_shared/modal/uiModal.component.js';
import '../../../Content/src/_shared/modal/uiModalDecorator.js';
appDependency.addItem(`uiModal`);
//appDependency.addItem(`ui.bootstrap`);

//MainMenu mobile
import mainMenuModule from '../Content/src/_partials/main-menu/mainMenu.module.js';
appDependency.addItem(mainMenuModule);

import maskModule from '../../../../../scripts/_common/mask/mask.module.js';
appDependency.addItem(maskModule);

import uiSelectModule from '../../../Content/vendors/ui-select/ui-select.mobile.module.js';

appDependency.addItem(uiSelectModule);

import '../../../Content/vendors/angular-input-modified/angular-input-modified.custom.js';
appDependency.addItem(`ngInputModified`);

import contentLoaderModule from '../Content/src/_shared/content-loader/content-loader.module.js';
appDependency.addItem(contentLoaderModule);

//bootstrap
import '../../../../../node_modules/bootstrap/js/dist/dropdown.js';
import '../Content/styles/_shared/bootstrap/bootstrap.scss';
import '../Content/styles/_shared/bootstrap/atomic.scss';
import '../Content/styles/_shared/dropdown/dropdown.scss';
import '../../../../../node_modules/bootstrap/js/dist/popover.js';
import '../../../../../vendors/ui-bootstrap-custom/styles/ui-popover.css';

appDependency.addItem(popover);

import '../../../../../scripts/_common/input/input.module.js';
appDependency.addItem(`input`);

//Ui grid
import '../../../Content/src/_shared/ui-grid-custom/uiGridCustom.module.js';
import '../Content/styles/_shared/ui-grid/ui-grid.scss';
import '../Content/styles/_shared/ui-grid/ui-grid-filter.scss';
import '../Content/styles/_shared/ui-grid/ui-grid-selection.scss';
import '../Content/styles/_shared/ui-grid/ui-grid-tree.scss';
import '../Content/src/_shared/ui-grid-custom/styles/ui-grid.custom.scss';
import '../Content/src/_shared/ui-grid-custom/styles/ui-grid.custom.edit.scss';

import swipeLineModule from '../Content/src/_shared/swipe-line/swipe-line.module.js';
appDependency.addItem(swipeLineModule);

appDependency.addItem(`uiGridCustomSelection`);
appDependency.addItem(`uiGridCustom`);

import '../../../../../scripts/_common/select/select.module.js';
appDependency.addItem(`select`);
import '../../../Content/src/_shared/validation/validation.js';
import '../../../Content/src/_shared/validation/filters/validationFilters.js';
import '../../../Content/src/_shared/validation/directives/validationDirectives.js';
import '../../../../../styles/common/validation.scss';
appDependency.addItem(`validation`);
import helpTriggerModule from '../../../Content/src/_partials/help-trigger/helpTrigger.module.js';
appDependency.addItem(helpTriggerModule);
import '../../../../../scripts/_common/input/input.module.js';
appDependency.addItem(`input`);
import '../../../Content/src/_shared/back/back.module.js';
import '../../../Content/src/_shared/back/back.service.js';
import '../../../Content/src/_shared/back/back.directive.js';
appDependency.addItem('back');

import cmStatModule from '../../../Content/src/_shared/cm-stat/cmStat.module.js';
appDependency.addItem(cmStatModule);

import '../../../../../scripts/_common/PubSub/PubSub.js';

import '../../../Content/src/_shared/modal/referralLink/ModalReferralLinkCtrl.js';

//css

import 'normalize.css';
import '../../../Content/src/_shared/input/styles/input.scss';
import '../Content/styles/variables.scss';
import '../Content/styles/_shared/layout/layout.scss';
import '../Content/styles/_shared/navigation-item/navigation-item.scss';
import '../../../Content/styles/headers.scss';
import '../Content/styles/_partials/btn-more.scss';
import '../Content/styles/_partials/page-head.scss';
import '../Content/styles/_partials/block.scss';

import '../Content/styles/_shared/sidebar/sidebar.scss';
import '../Content/styles/common.scss';
import '../Content/styles/_shared/chips/chips.scss';
import '../Content/styles/_shared/tabs-navigation/tabs-navigation.scss';
import '../../../../../fonts/fonts.adminmobile.css';
import '../Content/styles/_shared/icons/icons.scss';
import '../Content/styles/_partials/links/link.scss';
import '../../../Content/src/_shared/custom-input/custom-input.scss';
import '../Content/styles/_shared/inputs/custom-input.scss';
import '../Content/styles/_partials/progress/progress.scss';
import '../Content/styles/_partials/top-menu/top-menu.scss';

import '../Content/styles/_shared/modal/modal.scss';
import '../Content/styles/_shared/select/select.scss';
import '../Content/styles/_partials/balance/balance.scss';
import '../Content/styles/_shared/paginations/paginations.scss';
import '../Content/styles/_shared/inputs/inputs.scss';
import '../Content/styles/_shared/buttons/buttons.scss';
// import '../../../Content/styles/flexboxgrid/flexboxgrid.css';
import '../../../../../vendors/flexboxgrid/flexboxgrid.scss';
import '../../../Content/styles/flexboxgrid/flexboxgrid-ext.css';
import '../Content/styles/_shared/link-back/link-back.scss';
import '../Content/styles/_shared/popover/popover.scss';
import '../Content/styles/_shared/help-trigger/help-trigger.scss';
import '../Content/src/_shared/more-button/more-button.scss';

import moreButtonModule from '../Content/src/_shared/more-button/more-button.js';
appDependency.addItem(moreButtonModule);

import '../Content/src/_app/app.js';

import '../Content/vendors/ui-bootstrap/angular-popover-decorator/angular-popover-decorator.js';

import '../Content/styles/_shared/link-academy/link-academy.scss';

import '../../../Content/styles/atomic.scss';
import '../Content/styles/atomic.scss';

import '../../../Content/src/_shared/search/searchBlockController.js';
import '../../../Content/src/_shared/search/styles/search.scss';
import '../../AdminV3/Content/src/_partials/top-panel/top-panel.css';
import '../Content/styles/_shared/panels/panels.scss';

import 'ng-infinite-scroll';

appDependency.addItem('search');

import '../Content/styles/views/errors.scss';

import '../../../../../node_modules/ladda/dist/ladda-themeless.min.css';

import '../../../../../node_modules/ladda/js/ladda.js';
import '../../../../../node_modules/angular-ladda/dist/angular-ladda.js';
appDependency.addItem('angular-ladda');

import '../Content/src/_partials/saasWarningMessage/saasWarningMeassage.scss';
import '../../../Content/src/_partials/saasWarningMessage/saasWarningMessage.js';
appDependency.addItem(`saasWarningMessage`);

import '../Content/styles/_shared/ckeditor/ckeditor.scss';
import '../../../Content/vendors/ng-ckeditor/ng-ckeditor.module.js';
appDependency.addItem(`ngCkeditor`);

import '../../../Content/vendors/cropper/cropper.min.js';
import '../../../Content/vendors/cropper/cropper.css';

import '../../../Content/vendors/cropper/ngCropper.js';
import '../../../Content/src/_shared/modal/cropImage/ModalCropImageCtrl.js';
appDependency.addItem(`ngCropper`);

import '../../../../../styles/common/shimmer.scss';
import '../../../Content/src/_shared/modal/sendLetterToCustomer/ModalSendLetterToCustomerCtrl.js';
import '../../../Content/src/_shared/modal/sendSms/ModalSendSmsAdvCtrl.js';

import timesOfWorkModule from '../../../Content/src/_partials/times-of-work/times-of-work.module.js';
appDependency.addItem(timesOfWorkModule);

import '../../../Content/src/warehouse/warehouse.js';
appDependency.addItem('warehouse');

import '../../../Content/src/warehouse/modal/selectWarehouse/modalSelectWarehouse.ctrl.js';

import '../../../Content/src/warehousesList/warehousesList.js';
appDependency.addItem('warehousesList');

import '../../../Content/src/warehouseTypes/warehouseTypes.module.js';
appDependency.addItem(`warehouseTypes`);

import '../../../Content/src/_shared/modal/selectCity/ModalSelectCityCtrl.js';
import warehouseCitiesModule from '../../../Content/src/warehouse/components/warehouseCities/warehouseCities.module.js';
appDependency.addItem(warehouseCitiesModule);

import '../../../Content/src/warehouseTypes/modal/AddEditTypeWarehouse/modalAddEditTypeWarehouse.ctrl.js';
import '../../../Content/src/warehouseTypes/modal/selectTypeWarehouse/modalSelectTypeWarehouse.ctrl.js';

import '../../../Content/src/_shared/autocompleter/autocompleter.js';
appDependency.addItem('autocompleter');
import '../../../../../scripts/_common/autocompleter/templates/location.html';
import './svg-files.js';

import '../../../Content/styles/flatpickr-custom.scss';

import '../../../Content/vendors/year-calendar/yearCalendar.directive.js';
appDependency.addItem('ngYearCalendar');
