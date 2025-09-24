import { zoneMapDirective } from './directives/zoneMapDirectives';
import zoneSuggestionModalCtrl from './controllers/zoneSuggestionModalController';
import YaMapModule from '../../_common/yandexMaps/yandexMaps.module.js';
import autocompleterModule from '../../_common/autocompleter/autocompleter.module.js';
import addressModule from '../../_partials/address/address.module.js';
import checkoutModule from '../../../scripts/checkout/checkout.module.js';
import apiMapModule from '../../_common/apiMap/apiMap.module.js';
import zoneMapCtrl from './controllers/zoneMapController';
import zoneMapService from './services/zoneMapService';
import './styles/zoneMap.scss';
import './styles/zoneSuggestionPopover.scss';

const moduleName: string = 'zoneMap';

angular
    .module(moduleName, [YaMapModule, autocompleterModule, addressModule, checkoutModule, apiMapModule, 'modal'])
    .service('zoneMapService', zoneMapService)
    .controller('zoneMapCtrl', zoneMapCtrl)
    .controller('zoneSuggestionModalCtrl', zoneSuggestionModalCtrl)
    .directive('zoneMap', zoneMapDirective);
// .directive('zoneSuggestionModal', zoneSuggestionModalDirective);

export default moduleName;
