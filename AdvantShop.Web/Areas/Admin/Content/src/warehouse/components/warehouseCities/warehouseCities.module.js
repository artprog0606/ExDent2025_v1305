import WarehouseCitiesCtrl from './warehouseCities.controller.js';
import { warehouseCitiesComponent } from './warehouseCities.component.js';

const MODULE_NAME = 'warehouseCities';

angular.module(MODULE_NAME, []).controller('WarehouseCitiesCtrl', WarehouseCitiesCtrl).component('warehouseCities', warehouseCitiesComponent);

export default MODULE_NAME;
