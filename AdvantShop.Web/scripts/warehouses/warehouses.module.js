import warehousesService from './services/warehousesService.js';
import cartStockInWarehousesCtrl from './controllers/cartStockInWarehousesController.js';
import { cartStockInWarehousesDirective } from './directives/warehousesDirectives.js';

import '../_common/yandexMaps/ya-map-2.1.directive.js';
import '../_common/yandexMaps/styles.scss';
import './styles/warehousesListMap.scss';
import './styles/warehousesList.scss';
import './styles/cartStockInWareHouses.scss';

import WarehousesMapCtrl from './controllers/warehousesMap.ctrl.js';
import WarehousesListMapDirective from './directives/warehousesListMap.directive.js';
import WarehousesListDirective from './directives/warehousesListDirective.js';

const moduleName = 'warehouses';

angular
    .module(moduleName, ['yaMap'])
    .service('warehousesService', warehousesService)
    .controller('cartStockInWarehousesCtrl', cartStockInWarehousesCtrl)
    .controller('warehousesMapCtrl', WarehousesMapCtrl)
    .directive('warehousesListMap', WarehousesListMapDirective)
    .directive('warehousesList', WarehousesListDirective)
    .directive('cartStockInWarehouses', cartStockInWarehousesDirective)
    .constant('WarehousesDisplayStatus', {
        list: 'shops-list',
        map: 'shops-map',
    });

export default moduleName;
