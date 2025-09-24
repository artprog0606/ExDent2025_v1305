import appDependency from '../../../scripts/appDependency.js';

//import '../../../fonts/fonts.mobile.css';
//import '../../../fonts/fonts.icons.css';
import '../../../vendors/flexboxgrid/flexboxgrid.scss';
import '../../../vendors/flexboxgrid/ext/flexboxgridExt.scss';
import '../../../node_modules/normalize.css/normalize.css';

//import '../../../styles/general.scss';

import '../../../styles/common/buttons.scss';
import '../../../styles/common/button-group.scss';
import '../../../styles/common/icons.scss';
import '../../../styles/common/inputs.scss';
import '../../../styles/common/custom-input.scss';
//import '../../../styles/common/block.scss';
import '../../../styles/common/forms.scss';
import '../../../styles/common/links.scss';
import '../../../styles/common/tables.scss';
import '../../../styles/common/validation.scss';
import '../../../styles/common/connector.scss';
import '../../../styles/common/social.scss';
import '../../../styles/common/social-widgets.scss';
import '../../../styles/common/accordion-css.scss';

import '../../../styles/partials/gift.scss';
import '../../../styles/partials/price.scss';
import '../../../styles/partials/mobile-app-links.scss';

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

import '../../../vendors/qazy/qazyOpt.js';
import '../../../vendors/qazy/qazyOpt.directive.js';
appDependency.addItem('qazy');

import '../../../node_modules/angularjs-toaster/toaster.js';
import '../../../node_modules/angularjs-toaster/toaster.min.css';
appDependency.addItem('toaster');

import '../../../vendors/sweetalert/sweetalert2.default.js';
import '../../../vendors/ng-sweet-alert/ng-sweet-alert.js';
appDependency.addItem('ng-sweet-alert');

import '../../../node_modules/oclazyload/dist/ocLazyLoad.js';
import '../../../vendors/ocLazyLoad/ocLazyLoad.decorate.js';
appDependency.addItem('oc.lazyLoad');

import '../../../vendors/autofocus/autofocus.js';
appDependency.addItem('autofocus');

import carouselExtModule from '../../../scripts/_common/carousel-ext/carouselExt.js';
appDependency.addItem(carouselExtModule);

import '../../../vendors/angular-bind-html-compile/angular-bind-html-compile.js';
appDependency.addItem('angular-bind-html-compile');

//import photoViewerModule from '../../../scripts/_common/photoViewer/photoViewer.module.js';
//appDependency.addItem(photoViewerModule);

//import '../../../node_modules/ladda/dist/ladda-themeless.min.css';
//import '../../../node_modules/ladda/js/ladda.js';
//import '../../../node_modules/angular-ladda/dist/angular-ladda.js';
//appDependency.addItem('angular-ladda');

//import '../../../scripts/search/search.module.js';
//appDependency.addItem('search');

//import '../../../scripts/_partials/submenu/submenu.module.js';
//appDependency.addItem('submenu');

//import '../../../scripts/_partials/rootMenu/rootMenu.module.js';
//appDependency.addItem('rootMenu');

import '../../../scripts/_partials/cart/cart.module.js';
appDependency.addItem('cart');

import '../../../scripts/_partials/zone/zone.js';
import '../../../scripts/_partials/zone/zone.module.ts';
appDependency.addItem('zone');

import '../../../scripts/_partials/client-code/client-code.module.js';
appDependency.addItem('clientCode');

import '../../../scripts/_common/dom/dom.module.js';
appDependency.addItem('dom');

import '../../../scripts/_common/window/window.module.js';
appDependency.addItem('windowExt');

import '../../../scripts/_common/autocompleter/autocompleter.module.js';
appDependency.addItem('autocompleter');

import lozadAdvModule from '../../../scripts/_common/lozad-adv/lozadAdv.module.js';
appDependency.addItem(lozadAdvModule);
//import compareModule from '../../../scripts/_partials/compare/compare.module.js';
//appDependency.addItem(compareModule);

//import '../../../scripts/_common/harmonica/harmonica.module.js';
//appDependency.addItem('harmonica');

import '../../../scripts/_common/modal/modal.module.js';
appDependency.addItem('modal');

import '../../../scripts/_common/popover/popover.module.js';
appDependency.addItem('popover');

import '../../../scripts/_common/readmore/readmore.module.js';
appDependency.addItem('readmore');

import '../../../scripts/_common/spinbox/spinbox.module.js';
appDependency.addItem('spinbox');

//import '../../../scripts/_common/scrollToTop/scrollToTop.module.js';
//appDependency.addItem('scrollToTop');

import '../../../scripts/_common/input/input.module.js';
appDependency.addItem('input');

import '../../../scripts/_common/select/select.module.js';
appDependency.addItem('select');

import '../../../scripts/_common/module/module.module.js';
appDependency.addItem('module');

import '../../../scripts/_common/validation/validation.module.js';
appDependency.addItem('validation');

import '../../../scripts/_common/urlHelper/urlHelperService.module.js';
appDependency.addItem('urlHelper');

//import '../../../scripts/_common/mouseoverClassToggler/mouseoverClassToggler.module.js';
//appDependency.addItem('mouseoverClassToggler');

//import '../../../scripts/_common/hunter/hunter.module.js';

//import '../../../styles/partials/mobileOverlap.scss';
//import '../../../scripts/_mobile/mobileOverlap.js';
//appDependency.addItem('mobileOverlap');

//import '../../../scripts/_partials/cookies-policy/cookiesPolicy.module.js';
//appDependency.addItem('cookiesPolicy');

//import '../../../scripts/_partials/wishlist/wishlist.module.js';
//appDependency.addItem('wishlist');

//import '../../../scripts/_partials/currency/currency.module.js';
//appDependency.addItem('currency');

/*special for mobile*/
import '../scripts/_common/tableResponsive/styles/tableResponsive.scss';
import '../scripts/_common/tableResponsive/tableResponsive.js';

import fullHeightMobileModule from '../../../scripts/_mobile/full-height-mobile/full-height-mobile.module.js';
appDependency.addItem(fullHeightMobileModule);

import '../styles/cssFramework.scss';
import '../styles/general.scss';
import '../styles/_common/carousel.scss';
import '../styles/_common/buttons.scss';
import '../styles/_common/inputs.scss';
import '../styles/_common/forms.scss';
import '../styles/_common/select-custom.scss';
import '../styles/_partials/header.scss';
import '../styles/_partials/phones-number-list.scss';
import '../styles/_partials/sidebar.scss';
import '../styles/_partials/menu.scss';
import '../styles/_partials/cookies-policy.scss';
import '../styles/_partials/footer.scss';
import '../styles/_partials/footer-menu.scss';
import '../styles/_partials/social-links.scss';
import '../styles/_partials/cart-popover.scss';
import '../styles/_partials/product-view.scss';
import '../styles/_partials/cart.scss';
import '../../../styles/common/block.scss';
import '../../../styles/partials/captcha.scss';
import '../../../styles/partials/bonus-card.scss';
import '../styles/_partials/products-by-ids.scss';
import '../styles/_partials/bottom-panel.scss';

//import uiHelperModule from '../scripts/_common/uiHelper/uiHelper.module.js';
//appDependency.addItem(uiHelperModule);

import searchPanelModule from '../scripts/_common/searchPanel/searchPanel.module.js';
appDependency.addItem(searchPanelModule);

import sidebarsContainerModule from '../scripts/_common/sidebarsContainer/sidebarsContainer.module.js';
appDependency.addItem(sidebarsContainerModule);

//mask
import maskModule from '../../../scripts/_common/mask/mask.module.js';
appDependency.addItem(maskModule);

import mobileMenuModule from '../scripts/_common/mobileMenu/mobileMenu.module.js';

appDependency.addItem(mobileMenuModule);

import breadCrumbsModule from '../../../scripts/_common/breadCrumbs/breadCrumbs.module.js';

appDependency.addItem(breadCrumbsModule);

import headerModule from '../scripts/_partials/header/header.module.js';

appDependency.addItem(headerModule);

import wishlistModule from '../../../scripts/_partials/wishlist/wishlist.module.ts';

appDependency.addItem(wishlistModule);

import choicesModule from '../../../scripts/_common/choices/choices.module.js';

appDependency.addItem(choicesModule);

import iframeResponsiveModule from '../../../scripts/_common/iframe-responsive/iframeResponsive.module.js';

appDependency.addItem(iframeResponsiveModule);

import '../../../scripts/_common/transformer/transformer.module.js';

import '../../../styles/snippets.scss';

import '../scripts/_partials/colors-viewer/styles/colors-viewer.scss';

import stickyElementModule from '../../../scripts/_common/stickyElement/stickyElement.module.js';
appDependency.addItem(stickyElementModule);

import scrollToBlockModule from '../../../scripts/_common/scroll-to-block/scrollToBlock.js';
appDependency.addItem(scrollToBlockModule);

import '../../../scripts/_common/transformer/transformer.module.js';

appDependency.addItem('transformer');

import reviewsModule from '../../../scripts/_partials/reviews/reviews.module.js';
appDependency.addItem(reviewsModule);

angular.module(scrollToBlockModule).run(
    /* @ngInject */ (scrollToBlockConfig, transformerService, stickyElementService) => {
        scrollToBlockConfig.calcExtend = (val) => {
            const transformersList = transformerService.getTransformersStorage();
            const transfomersNeedHeight = transformersList.reduce(function (acc, it) {
                if (it._elementStartRect.topWithScroll < val) {
                    return acc + it.getHeightElement();
                }
                return acc;
            }, 0);
            const stickySumHeightAllElementsActive = stickyElementService.getSumTopSticky(val);
            return val - transfomersNeedHeight - stickySumHeightAllElementsActive;
        };
    },
);

import '../../../scripts/app.js';
