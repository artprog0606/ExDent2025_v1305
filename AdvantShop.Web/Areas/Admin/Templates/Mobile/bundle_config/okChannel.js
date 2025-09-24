import appDependency from '../../../../../scripts/appDependency.js';

import '../../../Content/src/ok/okMain.js';
import '../../../Content/src/ok/components/okChannel/okChannel.js';
import '../../../Content/src/ok/components/okAuth/okAuth.js';
import '../../../Content/src/ok/components/okMarketExport/okMarketExport.js';
import '../../../Content/src/ok/components/okMarketImport/okMarketImport.js';
import '../../../Content/src/ok/services/okMarketService.js';
import '../../../Content/src/ok/services/okService.js';
import '../../../Content/src/ok/components/okMarketExport/modals/modalAddEditOkCatalog/ModalAddEditOkCatalogCtrl.js';
import '../../../Content/src/ok/components/okMarketExport/modals/modalSaveOkMarketExportSettings/ModalSaveOkMarketExportSettingsCtrl.js';

import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';

import '../Content/styles/views/settings.scss';

import '../../../Content/src/_shared/adv-tracking/advTracking.js';
import '../../../Content/src/_shared/adv-tracking/advTracking.service.js';

import '../Content/styles/_shared/card/card.scss';
import '../Content/src/_shared/swipe-line/swipe-line.module.js';

appDependency.addItem(`okMain`);
appDependency.addItem(`okChannel`);
appDependency.addItem(`okAuth`);
appDependency.addItem(`okMarketExport`);
appDependency.addItem(`okMarketImport`);
appDependency.addItem(`swipeLine`);
appDependency.addItem(`advTracking`);
