////(function (ng) {
////    ng.module('warehouseTypes', []);
////})(window.angular);

import WarehouseTypesCtrl from './warehouseTypes.controller.js';
import warehouseTypesService from './warehouseTypes.service.js';

const MODULE_NAME = 'warehouseTypes';

angular
    .module(MODULE_NAME, ['uiGridCustom'])
    .controller('WarehouseTypesCtrl', WarehouseTypesCtrl)
    .service('warehouseTypesService', warehouseTypesService);

export default MODULE_NAME;
