import appDependency from '../../../../../scripts/appDependency.js';
import '../../../Content/src/vk/vkMain.js';
import '../../../Content/src/vk/components/vkChannel/vkChannel.js';
import '../../../Content/src/vk/components/vkAuth/vkAuth.js';
import '../../../Content/src/vk/components/vkMarketExport/vkMarketExport.js';
import '../../../Content/src/vk/components/vkMarketCategories/vkMarketCategories.js';
import '../../../Content/src/vk/components/vkMarketCategories/modals/ModalAddEditVkCategory.js';
import '../../../Content/src/vk/components/vkMarketExportSettings/vkMarketExportSettings.js';
import '../../../Content/src/vk/components/vkMarketImportSettings/vkMarketImportSettings.js';
import '../../../Content/src/vk/services/vkMarketService.js';
import '../../../Content/src/vk/services/vkService.js';
import '../../../Content/src/vk/modal/saveVkMarketSettings/ModalSaveVkMarketSettingsCtrl.js';

import '../../../Content/src/_shared/adv-tracking/advTracking.js';
import '../../../Content/src/_shared/adv-tracking/advTracking.service.js';

import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';

import '../../../Content/src/_shared/autocompleter/autocompleter.js';

import '../Content/styles/views/settings.scss';

import '../Content/styles/_shared/card/card.scss';
import '../Content/src/_shared/swipe-line/swipe-line.module.js';

appDependency.addItem(`vkChannel`);
appDependency.addItem(`vkMain`);
appDependency.addItem(`vkAuth`);
appDependency.addItem(`vkMarket`);
appDependency.addItem(`vkMarketExport`);
appDependency.addItem(`vkMarketCategories`);
appDependency.addItem(`vkMarketExportSettings`);
appDependency.addItem(`vkMarketImportSettings`);
appDependency.addItem(`advTracking`);
appDependency.addItem(`swipeLine`);
appDependency.addItem(`autocompleter`);
