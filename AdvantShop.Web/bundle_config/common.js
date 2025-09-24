import appDependency from '../scripts/appDependency.js';

//import '../fonts/fonts.store.css';
//import '../fonts/fonts.icons.css';

import '../vendors/flexboxgrid/flexboxgrid.scss';
import '../vendors/flexboxgrid/ext/flexboxgridExt.scss';
import '../node_modules/normalize.css/normalize.css';

import '../styles/general.scss';

import '../styles/common/buttons.scss';
import '../styles/common/button-group.scss';
import '../styles/common/headers.scss';
import '../styles/common/icons.scss';
import '../styles/common/custom-input.scss';
import '../styles/common/inputs.scss';
import '../styles/common/block.scss';
import '../styles/common/forms.scss';
import '../styles/common/links.scss';
import '../styles/common/tables.scss';
import '../styles/common/validation.scss';
import '../styles/common/connector.scss';
import '../styles/common/social.scss';
import '../styles/common/social-widgets.scss';
import '../styles/common/accordion-css.scss';
import '../styles/common/sidebar.scss';

import '../styles/partials/bonus-card.scss';
import '../styles/partials/captcha.scss';
import '../styles/partials/header.scss';
import '../styles/partials/menu-dropdown.scss';
import '../styles/partials/menu-header.scss';
import '../styles/partials/menu-general.scss';
import '../styles/partials/price.scss';
import '../styles/partials/toolbar-top.scss';
import '../styles/partials/footer.scss';
import '../styles/partials/footer-menu.scss';
import '../styles/partials/recentlyView.scss';
import '../styles/partials/gift.scss';
import '../styles/partials/toolbar-bottom.scss';
import '../styles/partials/stickers.scss';
import '../styles/partials/mobile-app-links.scss';

import '../vendors/jquery/jquery.passive.js';

import '../node_modules/angular/angular.js';

import '../vendors/stop-angular-overrides/stop-angular-overrides.js';

import '../node_modules/angular-cookies/angular-cookies.js';
appDependency.addItem('ngCookies');

import '../node_modules/angular-sanitize/angular-sanitize.js';
appDependency.addItem('ngSanitize');

import '../node_modules/angular-translate/dist/angular-translate.js';
appDependency.addItem('pascalprecht.translate');

import '../vendors/qazy/qazyOpt.directive.js';
appDependency.addItem('qazy');

import '../node_modules/angularjs-toaster/toaster.js';
import '../node_modules/angularjs-toaster/toaster.min.css';
appDependency.addItem('toaster');

import '../vendors/sweetalert/sweetalert2.default.js';
import '../vendors/ng-sweet-alert/ng-sweet-alert.js';
appDependency.addItem('ng-sweet-alert');

import '../node_modules/oclazyload/dist/ocLazyLoad.js';
import '../vendors/ocLazyLoad/ocLazyLoad.decorate.js';
appDependency.addItem('oc.lazyLoad');

import '../vendors/autofocus/autofocus.js';
appDependency.addItem('autofocus');

import maskModule from '../scripts/_common/mask/mask.module.js';
appDependency.addItem(maskModule);

import '../vendors/angular-bind-html-compile/angular-bind-html-compile.js';
appDependency.addItem('angular-bind-html-compile');

import photoViewerModule from '../scripts/_common/photoViewer/photoViewer.module.js';
appDependency.addItem(photoViewerModule);

import * as Ladda from '../node_modules/ladda/dist/ladda-themeless.min.css';

import '../node_modules/ladda/js/ladda.js';
import '../node_modules/angular-ladda/dist/angular-ladda.js';
appDependency.addItem('angular-ladda');

import '../scripts/search/search.module.js';
appDependency.addItem('search');

import '../scripts/_partials/submenu/submenu.module.js';
appDependency.addItem('submenu');

import '../scripts/_partials/rootMenu/rootMenu.module.js';
appDependency.addItem('rootMenu');

import '../scripts/_partials/cart/cart.module.js';
appDependency.addItem('cart');

import '../scripts/_partials/zone/zone.js';
import '../scripts/_partials/zone/zone.module.ts';
appDependency.addItem('zone');

import '../scripts/_partials/client-code/client-code.module.js';
appDependency.addItem('clientCode');

import '../scripts/_common/dom/dom.module.js';
appDependency.addItem('dom');

import '../scripts/_common/window/window.module.js';
appDependency.addItem('windowExt');

import '../scripts/_common/autocompleter/autocompleter.module.js';
appDependency.addItem('autocompleter');

import compareModule from '../scripts/_partials/compare/compare.module.ts';
appDependency.addItem(compareModule);

import '../scripts/_common/harmonica/harmonica.module.js';
appDependency.addItem('harmonica');

import '../scripts/_common/modal/modal.module.js';
appDependency.addItem('modal');

import '../scripts/_common/popover/popover.module.js';
appDependency.addItem('popover');

import '../scripts/_common/readmore/readmore.module.js';
appDependency.addItem('readmore');

import '../scripts/_common/spinbox/spinbox.module.js';
appDependency.addItem('spinbox');

import '../scripts/_common/scrollToTop/scrollToTop.module.js';
appDependency.addItem('scrollToTop');

import '../scripts/_common/transformer/transformer.module.js';
appDependency.addItem('transformer');

import '../scripts/_common/input/input.module.js';
appDependency.addItem('input');

import '../scripts/_common/select/select.module.js';
appDependency.addItem('select');

import '../scripts/_common/module/module.module.js';
appDependency.addItem('module');

import '../scripts/_common/validation/validation.module.js';
appDependency.addItem('validation');

import '../scripts/_common/urlHelper/urlHelperService.module.js';
appDependency.addItem('urlHelper');

import '../scripts/_common/mouseoverClassToggler/mouseoverClassToggler.module.js';
appDependency.addItem('mouseoverClassToggler');

import carouselExtModule from '../scripts/_common/carousel-ext/carouselExt.js';
appDependency.addItem(carouselExtModule);

import '../scripts/_common/hunter/hunter.module.js';

//import '../styles/partials/mobileOverlap.scss';
//import '../scripts/_mobile/mobileOverlap.js';
//appDependency.addItem('mobileOverlap');

//import '../scripts/_partials/cookies-policy/cookiesPolicy.module.js';
//appDependency.addItem('cookiesPolicy');

import '../scripts/_partials/wishlist/wishlist.module.ts';
appDependency.addItem('wishlist');

//import '../scripts/_partials/currency/currency.module.js';
//appDependency.addItem('currency');

//import countdownModule from '../scripts/_common/countdown/countdown.module.js';
//appDependency.addItem(countdownModule);

import lozadAdvModule from '../scripts/_common/lozad-adv/lozadAdv.module.js';
appDependency.addItem(lozadAdvModule);

//to do добавил костыли для модулей
//storereviews page

import reviewsModule from '../scripts/_partials/reviews/reviews.module.js';
appDependency.addItem(reviewsModule);

//import ratingModule from '../scripts/_common/rating/rating.module.js';
//appDependency.addItem(ratingModule);

////shippingpayment
//import checkoutModule from '../scripts/checkout/checkout.module.js';
//appDependency.addItem(checkoutModule);

////blog
//import newsModule from '../scripts/news/news.module.js';
//appDependency.addItem(newsModule);
////end to do

import sidebarsContainerModule from './sidebarsContainer.js';
appDependency.addItem(sidebarsContainerModule); //for templates which use it

import '../styles/snippets.scss';

import '../scripts/app.js';

//Для совместимости с 4.0
import '../styles/theme.scss';
//Для совместимости с 4.0

import breadcrumbsModule from '../scripts/_common/breadCrumbs/breadCrumbs.module.js';

appDependency.addItem(breadcrumbsModule);

//import setCssCustomPropsModule from '../scripts/_common/setCssCustomProps/setCssCustomProps.module.js'; // НЕ УДАЛЯТЬ используется в Modern шаблоне (удалить из шаблона если добавлять в движок)
//appDependency.addItem(setCssCustomPropsModule); // фича добавляет своства элемента в css переменную

import choicesModule from '../scripts/_common/choices/choices.module.js';
appDependency.addItem(choicesModule);

import iframeResponsiveModule from '../scripts/_common/iframe-responsive/iframeResponsive.module.js';
appDependency.addItem(iframeResponsiveModule);

import stickyElementModule from '../scripts/_common/stickyElement/stickyElement.module.js';
appDependency.addItem(stickyElementModule);

import scrollToBlockModule from '../scripts/_common/scroll-to-block/scrollToBlock.js';
appDependency.addItem(scrollToBlockModule);

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
