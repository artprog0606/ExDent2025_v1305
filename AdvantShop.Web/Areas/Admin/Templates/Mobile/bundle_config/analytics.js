import appDependency from '../../../../../scripts/appDependency.js';
import '../../../Content/src/analyticsReport/analyticsReport.js';
import '../../../Content/src/adv-analytics/adv-analytics.js';
import '../../../Content/src/adv-analytics/adv-analytics.service.js';
appDependency.addItem(`analytics`);
// import '../../../Content/src/analyticsReport/styles/analytics-report.scss';
import '../../../Content/src/analyticsReport/components/abcxyzAnalysis/abcxyzAnalysis.js';
import '../../../Content/src/analyticsReport/components/avgcheck/avgcheck.js';
import '../../../Content/src/analyticsReport/components/emailingsWith/emailingsWith.js';
//import '../../../Content/src/analyticsReport/components/export-customers/exportCustomers.js';
import '../../../Content/src/analyticsReport/components/export-products/exportProducts.js';
import '../../../Content/src/analyticsReport/components/managers-report/managersReport.js';
import '../../../Content/src/analyticsReport/components/emailingsWith/emailingsWith.js';
import '../../../Content/src/analyticsReport/components/emailingsWithout/emailingsWithout.js';
import '../../../Content/src/analyticsReport/components/export-orders/exportOrders.js';
import '../../../Content/src/analyticsReport/components/ordersAnalysis/ordersAnalysis.js';
import '../../../Content/src/analyticsReport/components/productReport/productReport.js';
import '../../../Content/src/analyticsReport/components/offerReport/offerReport.js';
import '../../../Content/src/analyticsReport/components/profit/profit.js';
import '../../../Content/src/analyticsReport/components/rfm/rfm.js';
import '../../../Content/src/analyticsReport/components/searchRequests/searchRequests.js';
import '../../../Content/src/analyticsReport/components/bonus/bonus.js';
import '../../../Content/src/analyticsReport/components/telephony/telephony.js';
import '../../../Content/src/analyticsReport/components/telephonyCallLog/telephonyCallLog.js';
import '../../../Content/src/analyticsReport/components/vortex/vortex.js';
appDependency.addItem(`analyticsReport`);

import '../../../Content/src/_shared/export-customers/exportCustomers.js';
appDependency.addItem('exportCustomers');

import '../Content/styles/_shared/chips/chips.scss';
import scrollIntoViewModule from '../../../../../scripts/_common/scroll-into-view/scroll-into-view.js';
appDependency.addItem(scrollIntoViewModule);

import '../../../Content/src/_shared/modal/products-selectvizr/ModalProductsSelectvizr.mobile.module.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.component.js';
appDependency.addItem(`productsSelectvizr`);
import '../../../Content/src/_shared/modal/offers-selectvizr/ModalOffersSelectvizr.mobile.module.js';
import '../../../Content/src/_partials/offers-selectvizr/offersSelectvizr.js';
import '../../../Content/src/_partials/offers-selectvizr/offersSelectvizr.component.js';
appDependency.addItem(`offersSelectvizr`);
import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';
import '../../../Content/src/calls/components/callRecord/callRecord.js';
appDependency.addItem(`callRecord`);
import '../../../Content/src/_shared/audio-player/audioPlayer.js';
import '../../../Content/src/_shared/audio-player/audioPlayer.component.js';
import '../../../Content/src/_shared/audio-player/styles/audio-player.css';
appDependency.addItem(`audioPlayer`);

import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/_shared/bootstrap/grid.scss';
import '../Content/styles/_shared/flatpickr/flatpickr.scss';
import '../Content/styles/_shared/list-group/list-group.scss';
import '../Content/styles/views/analytics.scss';
import '../Content/styles/_shared/reports/reports.scss';
import '../Content/styles/_shared/onoffswitch/onoffswitch.scss';

import swipeLine from '../Content/src/_shared/swipe-line/swipe-line.module.js';
appDependency.addItem(swipeLine);

import '../Content/src/_shared/modal/exportProducts/ModalExportProductsCtrl.js';
import '../Content/src/_shared/modal/exportCustomers/ModalExportCustomersCtrl.js';
import '../Content/src/_shared/modal/exportOrders/ModalExportOrdersCtrl.js';

import '../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
appDependency.addItem(`ngClickCapture`);

import '../../../Content/src/_shared/modal/editableGridRow/ModalEditableGridRow.js';

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
appDependency.addItem(`productGridItem`);

import angularInview from 'angular-inview';

appDependency.addItem(angularInview);
