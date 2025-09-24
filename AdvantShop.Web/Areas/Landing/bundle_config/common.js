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

import '../../../node_modules/oclazyload/dist/ocLazyLoad.js';
import '../../../vendors/ocLazyLoad/ocLazyLoad.decorate.js';
appDependency.addItem('oc.lazyLoad');

import '../../../vendors/sweetalert/sweetalert2.default.js';
import '../../../vendors/ng-sweet-alert/ng-sweet-alert.js';
appDependency.addItem('ng-sweet-alert');

import '../../../node_modules/angularjs-toaster/toaster.js';
import '../../../node_modules/angularjs-toaster/toaster.min.css';
appDependency.addItem('toaster');

import '../../../scripts/_common/modal/modal.module.js';
appDependency.addItem('modal');

import zoomerModule from '../../../scripts/_common/zoomer/zoomer.module.js';
appDependency.addItem(zoomerModule);

import flatpickrModule from '../../../vendors/flatpickr/flatpickr.module.js';
appDependency.addItem(flatpickrModule);

import rangeSliderModule from '../../../vendors/rangeSlider/rangeSlider.module.js';
appDependency.addItem(rangeSliderModule);

import '../../../styles/common/validation.scss';
import '../../../scripts/_common/validation/validation.module.js';
appDependency.addItem('validation');

import '../../../scripts/_common/popover/popover.module.js';
appDependency.addItem('popover');

import '../../../scripts/_common/readmore/readmore.module.js';
appDependency.addItem('readmore');

import buyOneClickModule from '../../../scripts/_partials/buy-one-click/buyOneClick.module.js';
appDependency.addItem(buyOneClickModule);

import preOrderModule from '../../../scripts/_partials/pre-order/preOrder.module.js';
appDependency.addItem(preOrderModule);

import productViewModule from '../../../scripts/_partials/product-view/productView.module.js';
appDependency.addItem(productViewModule);

import '../frontend/blocks/product-landing/productPhotosLanding.js';
appDependency.addItem('productPhotosLanding');

import * as Ladda from '../../../node_modules/ladda/dist/ladda-themeless.min.css';

import '../../../node_modules/ladda/js/ladda.js';
import '../../../node_modules/angular-ladda/dist/angular-ladda.js';
appDependency.addItem('angular-ladda');

import '../frontend/_common/advBaguetteBox/advBaguetteBox.js';
import '../frontend/_common/advBaguetteBox/advBaguetteBox.ctrl.js';
import '../frontend/_common/advBaguetteBox/advBaguetteBox.directive.js';

import catalogFilterModule from '../frontend/_common/catalog-filter/catalogFilter.module.js';
appDependency.addItem(catalogFilterModule);

import photoViewListModule from '../../../scripts/_partials/photo-view-list/photoViewList.module.js';
appDependency.addItem(photoViewListModule);

import '../frontend/_common/modalVideo/modalVideo.scss';
import '../frontend/_common/modalVideo/modalVideo.js';
import '../frontend/_common/modalVideo/modalVideo.component.js';
import '../frontend/_common/modalVideo/modalVideo.ctrl.js';
import '../frontend/_common/modalVideo/modalVideo.service.js';

import '../frontend/_common/lp-modal.scss';

import '../frontend/_common/scroll-to-top/scroll-to-top.scss';

import '../frontend/_common/lp.scss';

import '../frontend/_common/atomic.scss';

import '../frontend/_common/font-size-adaptive.scss';

import '../../../styles/spinner.scss';
import '../../../styles/partials/captcha.scss';

import '../frontend/vendors/fontello/css/animation.css';

import ngFileUploadModule from '../../../node_modules/ng-file-upload/index.js';
appDependency.addItem(ngFileUploadModule);

import '../../Admin/Content/src/_shared/is-mobile/is-mobile.js';
appDependency.addItem('isMobile');

import checklistModelModule from '../../../node_modules/checklist-model/checklist-model.js';
appDependency.addItem(checklistModelModule);

import '../../../scripts/_common/input/input.module.js';
appDependency.addItem('input');

import '../../../scripts/_common/select/select.module.js';
appDependency.addItem('select');

import authModule from '../../../scripts/auth/auth.module.js';
appDependency.addItem(authModule);

import smsConfirmationModule from '../../../scripts/sms/smsConfirmation.module.js';
appDependency.addItem(smsConfirmationModule);

import forgotPasswordModule from '../../../scripts/forgotPassword/forgotPassword.module.js';
appDependency.addItem(forgotPasswordModule);

import registrationModule from '../../../scripts/registration/registration.module.js';
appDependency.addItem(registrationModule);

import '../../../scripts/_partials/submenu/submenu.module.js';
appDependency.addItem('submenu');

import '../../../vendors/angular-bind-html-compile/angular-bind-html-compile.js';
appDependency.addItem('angular-bind-html-compile');

import maskModule from '../../../scripts/_common/mask/mask.module.js';
appDependency.addItem(maskModule);

import '../../../vendors/qazy/qazyOpt.directive.js';
appDependency.addItem('qazy');

import '../../../scripts/_common/dom/dom.module.js';
appDependency.addItem('dom');

import '../../../scripts/_common/urlHelper/urlHelperService.module.js';
appDependency.addItem('urlHelper');

import '../../../scripts/_common/window/window.module.js';
appDependency.addItem('windowExt');

import '../../../scripts/_common/module/module.module.js';
appDependency.addItem('module');

import '../../../scripts/_common/harmonica/harmonica.module.js';
appDependency.addItem('harmonica');

import cardsModule from '../../../scripts/_partials/cards/cards.module.js';
appDependency.addItem(cardsModule);

import '../../../scripts/_partials/cart/cart.module.js';
appDependency.addItem('cart');

import productModule from '../../../scripts/product/product.module.js';
appDependency.addItem(productModule);

import '../frontend/_partials/product-view/styles/product-view.scss';

import '../../../scripts/_common/mouseoverClassToggler/mouseoverClassToggler.module.js';
appDependency.addItem('mouseoverClassToggler');

import fullHeightMobileModule from '../../../scripts/_mobile/full-height-mobile/full-height-mobile.module.js';
appDependency.addItem(fullHeightMobileModule);

import '../../../node_modules/slick-carousel/slick/slick.js';
import '../../../node_modules/slick-carousel/slick/slick.scss';
import '../frontend/vendors/slick/slick.patch.js';

import '../../../node_modules/angular-slick-carousel/dist/angular-slick.js';
appDependency.addItem('slickCarousel');

import '../frontend/vendors/parallax/jquery.enllax.js';

import ngInfinityScrollModule from '../../../node_modules/ng-infinite-scroll/build/ng-infinite-scroll.js';
appDependency.addItem(ngInfinityScrollModule);

import '../frontend/_common/scroll-to-block/scrollToBlock.js';
import '../frontend/_common/scroll-to-block/scrollToBlock.directive.js';
import '../frontend/_common/scroll-to-block/scrollToBlock.service.js';
appDependency.addItem(`scrollToBlock`);

import '../frontend/_common/hunter/hunter.js';

import '../frontend/_common/lp-form/lp-form.js';
import '../frontend/_common/lp-form/controllers/lpFormController.js';
appDependency.addItem(`lp-form`);

import '../frontend/_common/lp-menu/lpMenu.js';
import '../frontend/_common/lp-menu/lpMenuState.ctrl.js';
import '../frontend/_common/lp-menu/lpMenuTrigger.ctrl.js';
import '../frontend/_common/lp-menu/lpMenu.service.js';
import '../frontend/_common/lp-menu/lpMenu.component.js';
appDependency.addItem(`lpMenu`);

import '../frontend/_common/modal-ouibounce/modalOuibounce.js';
appDependency.addItem(`modalOuibounce`);

import '../frontend/blocks/booking/modalBooking.js';
import '../frontend/blocks/booking/modalBooking.ctrl.js';
import '../frontend/blocks/booking/modalBooking.component.js';
appDependency.addItem(`modalBooking`);

import '../frontend/blocks/services/modalBookingServices/modalBookingServices.js';
import '../frontend/blocks/services/modalBookingServices/modalBookingServices.ctrl.js';
import '../frontend/blocks/services/modalBookingServices/modalBookingServices.component.js';
appDependency.addItem(`modalBookingServices`);

import '../frontend/blocks/countdown/countdown.js';
import '../frontend/blocks/countdown/controllers/countdownController.js';
import '../frontend/blocks/countdown/directives/countdownDirectives.js';
appDependency.addItem(`countdown`);

import '../frontend/_common/bookingCart/cart.js';
import '../frontend/_common/bookingCart/controllers/cartCountController.js';
import '../frontend/_common/bookingCart/directives/cartDirectives.js';
import '../frontend/_common/bookingCart/services/cartService.js';
appDependency.addItem(`bookingCart`);

import '../frontend/blocks/lp-cart/lp-cart.js';
appDependency.addItem(`lpCart`);

import '../frontend/blocks/products-by-category/productsByCategory.js';
import '../frontend/blocks/products-by-category/controllers/productsByCategoryController.js';
appDependency.addItem(`productsByCategory`);

import '../frontend/blocks/news/news.js';
import '../frontend/blocks/news/news.ctrl.js';
appDependency.addItem(`news`);

import '../frontend/blocks/product-landing/productPhotosLanding.js';
appDependency.addItem(`productPhotosLanding`);

import '../frontend/blocks/reviews-form-landing/reviewsFormLanding.js';
import '../frontend/blocks/reviews-form-landing/reviewsFormLanding.ctrl.js';
appDependency.addItem(`reviewsFormLanding`);

import '../frontend/utils/utils.service.js';
appDependency.addItem(`utils`);

import '../frontend/blocks/columns/columns.js';
import '../frontend/blocks/columns/columns.ctrl.js';
appDependency.addItem(`columns`);

import lozadAdvModule from '../../../scripts/_common/lozad-adv/lozadAdv.module.js';
appDependency.addItem(lozadAdvModule);

import '../../../Areas/Mobile/scripts/_common/tableResponsive/tableResponsive.js';

import '../../../scripts/_common/scrollToTop/scrollToTop.module.js';
appDependency.addItem(`scrollToTop`);

import '../frontend/_common/modalVideo/modalVideo.js';
import '../frontend/_common/modalVideo/modalVideo.component.js';
import '../frontend/_common/modalVideo/modalVideo.ctrl.js';
import '../frontend/_common/modalVideo/modalVideo.service.js';
import '../frontend/_common/modalVideo/modalVideo.scss';
appDependency.addItem(`modalVideo`);

import choicesModule from '../../../scripts/_common/choices/choices.module.js';
appDependency.addItem(choicesModule);
import '../../../scripts/_partials/zone/zone.js';
import zoneModule from '../../../scripts/_partials/zone/zone.module.ts';
appDependency.addItem(zoneModule);

import '../frontend/_partials/shipping/styles/shipping.scss';

import '../frontend/_app/app.js';
import '../frontend/_app/controllers/appController.js';
