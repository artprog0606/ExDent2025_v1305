import appDependency from '../../../../../scripts/appDependency.js';

import 'ng-file-upload';
appDependency.addItem(`ngFileUpload`);

import '../../../Content/src/_shared/file-uploader/fileUploader.js';
import '../../../Content/src/_shared/file-uploader/fileUploader.component.js';
import '../../../Content/src/_shared/file-uploader/modal/fileUploaderModal.js';

import '../Content/vendors/ng-sortable-custom/ng-sortable.module.js';

import '../Content/styles/views/settings.scss';
import '../Content/styles/_shared/product-item/product-item.scss';
import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/views/settings-list.scss';
import '../Content/src/_shared/swipe-line/swipe-line.module.js';

import 'tinycolor2';
import '../../../Content/vendors/angular-color-picker/angularjs-color-picker.cjs';
import '../../../Content/vendors/angular-color-picker/angularjs-color-picker.css';
import '../../../Content/vendors/angular-color-picker/themes/angularjs-color-picker-bootstrap.css';

import '../Content/styles/_shared/color-picker/color-picker.scss';

import '../../../Content/src/settingsCrm/settingsCrm.js';
import '../../../Content/src/settingsCrm/settingsCrm.service.js';
import '../../../Content/src/settingsCrm/components/facebookAuth/facebookAuth.js';
import '../../../Content/src/settingsCrm/components/facebookAuth/facebookAuthService.js';
import '../../../Content/src/settingsCrm/components/salesFunnels/salesFunnels.js';
import '../../../Content/src/settingsCrm/components/salesFunnels/modals/addEditSalesFunnel/ModalAddEditSalesFunnelCtrl.js';
import '../../../Content/src/settingsCrm/components/dealStatuses/dealStatuses.js';
import '../../../Content/src/settingsCrm/components/dealStatuses/modals/editDealStatus/ModalEditDealStatusCtrl.js';
import '../../../Content/src/settingsCrm/components/integrationsLimit/integrationsLimit.js';
import '../../../Content/src/settingsCrm/components/leadFieldsList/leadFieldsList.js';
import '../../../Content/src/settingsCrm/components/leadFieldsList/leadFields.service.js';
import '../../../Content/src/settingsCrm/components/leadFieldsList/modals/addEditLeadField/ModalAddEditLeadFieldCtrl.js';

import '../../../Content/src/lead/lead.js';
import '../../../Content/src/lead/lead.service.js';
import '../../../Content/src/lead/components/leadItemsSummary/leadItemsSummary.js';
import '../../../Content/src/_partials/leadEvents/leadEvents.js';
import '../../../Content/src/_partials/leadEvents/modals/addEditCallComent/ModalAddEditCallComentCtrl.js';
import '../../../Content/src/_partials/leadEvents/modals/showEmail/ModalShowEmailCtrl.js';
import '../../../Content/src/lead/modal/shippingsCity/ModalShippingsCityCtrl.js';
import '../../../Content/src/lead/modal/completeLead/ModalCompleteLeadCtrl.js';
import '../../../Content/src/leads/leads.js';
import '../../../Content/src/leads/leads.components.js';
import '../../../Content/src/leads/leadsListController.js';
import '../../../Content/src/leads/components/leadsListSources/leadsListSourcesController.js';
import '../../../Content/src/leads/components/leadsListChart/leadsListChartController.js';
import '../../../Content/src/leads/leadsListService.js';
import '../../../Content/src/leads/modal/changeLeadManager/ModalChangeLeadManagerCtrl.js';
import '../../../Content/src/leads/modal/changeLeadSalesFunnel/ModalChangeLeadSalesFunnelCtrl.js';

import '../../../Content/src/calls/components/callRecord/callRecord.js';

import '../../../Content/vendors/angular-timeago/angular-timeago-core.js';
import '../../../Content/vendors/angular-timeago/angular-timeago.langs.js';

import '../../../Content/src/import/import.js';
import '../../../Content/src/import/import.service.js';
import '../../../Content/src/import/import.module.js';

import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.component.js';
import '../../../Content/src/_shared/modal/products-selectvizr/ModalProductsSelectvizrCtrl.js';
import '../../../Content/src/_shared/modal/export-products-selectvizr/ModalExportProductsSelectvizrCtrl.js';
import '../../../Content/src/_shared/modal/shipping-products-selectvizr/ModalShippingProductsSelectvizrCtrl.js';
import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';

appDependency.addItem(`swipeLine`);
appDependency.addItem('settingsCrm');
