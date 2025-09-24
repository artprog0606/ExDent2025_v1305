import './styles.scss';
import warehousesMapCtrl from '../../../../scripts/warehouses/controllers/warehousesMap.ctrl.js';

const moduleName = 'shops';

angular.module(moduleName, []).controller('warehousesMap', warehousesMapCtrl);

export default moduleName;
