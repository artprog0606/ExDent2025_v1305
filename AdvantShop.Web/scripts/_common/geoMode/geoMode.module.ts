import './styles.scss';
import geoModeCtrl from './geoMode.ctrl';
import geoModeDirective, { GeoModeChangeAddressTriggerDirective } from './geoMode.directive';
import GeoModeService from './geoMode.service';
import zoneMapModule from '../../_partials/zone/zoneMap.module';
import zoneModule from '../../_partials/zone/zone.module';
import addressModule from '../../_partials/address/address.module.js';

const moduleName = 'geoMode';

type GeoModeEventsType = 'changeAddress';

export type GeoModeEventsObjType = Record<string, GeoModeEventsType>;

const geoModeEvents: GeoModeEventsObjType = {
    CHANGE_ADDRESS: 'changeAddress',
};

angular
    .module(moduleName, [zoneMapModule, zoneModule, addressModule, 'modal', 'warehouses'])
    .service('geoModeService', GeoModeService)
    .directive('geoMode', geoModeDirective)
    .directive('geoModeChangeAddressTrigger', GeoModeChangeAddressTriggerDirective)
    .controller('geoModeCtrl', geoModeCtrl)
    .constant('geoModeEvents', geoModeEvents);
export default moduleName;
