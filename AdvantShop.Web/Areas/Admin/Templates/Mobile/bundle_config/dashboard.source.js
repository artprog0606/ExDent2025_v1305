import '../../../Content/src/dashboardSites/dashboardSites.js';
import '../../../Content/src/dashboardSites/components/createSite/createSite.js';
import '../../../Content/src/dashboardSites/modals/qrCodeGenerator/ModalQrCodeGeneratorCtrl.js';
import '../../../Content/src/_partials/change-admin-shop-name/changeAdminShopName.js';
import '../Content/styles/views/funnel.scss';
import '../Content/styles/views/dashboard.scss';
import '../../../Content/styles/select-main-site.scss';
import '../../../Content/src/achievements/achievements.js';

import lozadAdvModule from '../../../../../scripts/_common/lozad-adv/lozadAdv.module.js';

import appDependency from '../../../../../scripts/appDependency.js';

appDependency.addItem(lozadAdvModule);

export default `dashboardSites`;
