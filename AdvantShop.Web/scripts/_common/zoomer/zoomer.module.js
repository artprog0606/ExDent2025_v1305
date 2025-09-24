import './styles/zoomer.scss';

import ZoomerCtrl from './controllers/zoomerController.js';
import { zoomerDirective, zoomerLensDirective, zoomerWindowDirective } from './directives/zoomerDirectives.js';

const moduleName = 'zoomer';

angular
    .module(moduleName, [])
    .controller('ZoomerCtrl', ZoomerCtrl)
    .directive('zoomer', zoomerDirective)
    .directive('zoomerLens', zoomerLensDirective)
    .directive('zoomerWindow', zoomerWindowDirective)
    .constant('zoomerConfig', {
        zoomWidth: 350,
        zoomHeight: 350,
        type: 'right', // right/inner
    });

export default moduleName;
