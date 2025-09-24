import appDependency from '../../../scripts/appDependency.js';

//import '../../../node_modules/jquery/dist/jquery.js';
import '../../../vendors/jquery/jquery.passive.js';

import '../../../node_modules/angular/angular.js';

import '../../../vendors/stop-angular-overrides/stop-angular-overrides.js';

import '../../../node_modules/angular-cookies/angular-cookies.js';
appDependency.addItem('ngCookies');

import '../../../node_modules/angular-sanitize/angular-sanitize.js';
appDependency.addItem('ngSanitize');

import '../../../node_modules/angular-translate/dist/angular-translate.js';
appDependency.addItem('pascalprecht.translate');

import 'ng-file-upload';
appDependency.addItem('ngFileUpload');

import '../../../node_modules/oclazyload/dist/ocLazyLoad.js';
import '../../../vendors/ocLazyLoad/ocLazyLoad.decorate.js';
appDependency.addItem('oc.lazyLoad');

import flatpickrModule from '../../../vendors/flatpickr/flatpickr.module.js';
appDependency.addItem(flatpickrModule);

import '../../../node_modules/angularjs-toaster/toaster.js';
import '../../../node_modules/angularjs-toaster/toaster.min.css';
appDependency.addItem('toaster');

import rangeSliderModule from '../../../vendors/rangeSlider/rangeSlider.module.js';
appDependency.addItem(rangeSliderModule);

import * as Ladda from '../../../node_modules/ladda/dist/ladda-themeless.min.css';

import '../../../node_modules/ladda/js/ladda.js';
import '../../../node_modules/angular-ladda/dist/angular-ladda.js';
appDependency.addItem('angular-ladda');

import '../../../vendors/sweetalert/sweetalert2.default.js';
import '../../../vendors/ng-sweet-alert/ng-sweet-alert.js';
appDependency.addItem('ng-sweet-alert');

import '../../../styles/common/block.scss';
import '../../../styles/common/forms.scss';
import '../../../styles/common/links.scss';
import '../../../styles/common/tables.scss';

import '../../../styles/partials/pagenumberer.scss';

import '../../../styles/common/validation.scss';
import '../../../scripts/_common/validation/validation.module.js';
appDependency.addItem('validation');

import '../../../scripts/_common/autocompleter/autocompleter.module.js';
appDependency.addItem('autocompleter');

import '../../../scripts/_common/harmonica/harmonica.module.js';
appDependency.addItem('harmonica');

import '../../../scripts/_common/modal/modal.module.js';
appDependency.addItem('modal');

import '../../../scripts/_common/popover/popover.module.js';
appDependency.addItem('popover');

import '../../../scripts/_common/readmore/readmore.module.js';
appDependency.addItem('readmore');

import '../../../scripts/_common/spinbox/spinbox.module.js';
appDependency.addItem('spinbox');

import '../../../scripts/_common/scrollToTop/scrollToTop.module.js';
appDependency.addItem('scrollToTop');

import '../../../scripts/_common/transformer/transformer.module.js';
appDependency.addItem('transformer');

import zoomerModule from '../../../scripts/_common/zoomer/zoomer.module.js';
appDependency.addItem(zoomerModule);

import '../../../scripts/_partials/submenu/submenu.module.js';
appDependency.addItem('submenu');

import '../../../vendors/lozad/lozad.custom.cjs';
import '../../../vendors/qazy/qazyOpt.js';
import '../../../vendors/qazy/qazyOpt.head.js';
import '../../../vendors/qazy/qazyOpt.directive.js';
appDependency.addItem('qazy');

import '../../../vendors/autofocus/autofocus.js';
appDependency.addItem('autofocus');

import popover from 'angular-ui-bootstrap/src/popover/index.js';
import '../../../vendors/ui-bootstrap-custom/styles/ui-popover.css';
appDependency.addItem(popover);

import maskModule from '../../../scripts/_common/mask/mask.module.js';
appDependency.addItem(maskModule);

import '../../../vendors/angular-bind-html-compile/angular-bind-html-compile.js';
appDependency.addItem('angular-bind-html-compile');

import carouselModule from '../../../scripts/_common/carousel/carousel.module.js';
appDependency.addItem(carouselModule);

import '../../../scripts/_common/dom/dom.module.js';
appDependency.addItem('dom');

import '../../../scripts/_common/input/input.module.js';
appDependency.addItem('input');

import '../../../scripts/_common/module/module.module.js';
appDependency.addItem('module');

import '../../../scripts/_common/select/select.module.js';
appDependency.addItem('select');

import tabsModule from '../../../scripts/_common/tabs/tabs.module.js';
appDependency.addItem(tabsModule);

import '../../../scripts/_common/window/window.module.js';
appDependency.addItem('windowExt');

import '../../../scripts/_common/mouseoverClassToggler/mouseoverClassToggler.module.js';
appDependency.addItem('mouseoverClassToggler');

import '../../../scripts/_common/urlHelper/urlHelperService.module.js';
appDependency.addItem('urlHelper');

import photoViewerModule from '../../../scripts/_common/photoViewer/photoViewer.module.js';
appDependency.addItem(photoViewerModule);

import '../../../scripts/_common/default-button/defaultButton.module.js';
appDependency.addItem('defaultButton');

import '../../../scripts/_common/countdown/countdown.module.js';

import '../../../scripts/_common/hunter/hunter.module.js';

import '../../../scripts/_partials/currency/currency.module.js';
appDependency.addItem('currency');

import '../Content/scripts/forgotPassword/forgotPassword.js';
appDependency.addItem('forgotPassword');

import '../../Admin/Content/vendors/angular-input-modified/angular-input-modified.custom.js';
appDependency.addItem('ngInputModified');

import '../Content/scripts/customers/customers.js';
appDependency.addItem('customers');

import '../Content/scripts/home/home.js';
appDependency.addItem('home');

import '../Content/scripts/rewards/rewards.js';
appDependency.addItem('rewards');

import '../Content/scripts/settings/settings.js';
appDependency.addItem('settings');

import scrollToBlockModule from '../../../scripts/_common/scroll-to-block/scrollToBlock.js';
appDependency.addItem(scrollToBlockModule);

import '../../../styles/views/errors.scss';

import '../../../styles/snippets.scss';

import '../Content/scripts/app.js';
