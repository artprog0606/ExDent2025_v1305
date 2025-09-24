import ProductAvailabilityMapCtrl from './productAvailabilityMap.ctrl.js';
import ProductAvailabilityMapDirective from './productAvailabilityMap.directive.js';
import '../../../_common/yandexMaps/ya-map-2.1.directive.js';
import '../../../_common/yandexMaps/styles.scss';
import './styles.scss';
//тянуть только warehouses-list
import shopsModule from '../../../warehouses/warehouses.module.js';

const moduleName = 'productAvailabilityMap';

angular
    .module(moduleName, ['yaMap', shopsModule])
    .directive('productAvailabilityMap', ProductAvailabilityMapDirective)
    .controller('productAvailabilityMapCtrl', ProductAvailabilityMapCtrl);

export default moduleName;
