import '../../../../../scripts/_partials/submenu/submenu.module.js';

import CatalogFilterMobileCtrl from './controllers/catalogFilterMobileController.js';

const moduleName = 'catalogFilterMobile';

angular.module(moduleName, ['submenu']).controller('CatalogFilterMobileCtrl', CatalogFilterMobileCtrl);

export default moduleName;
