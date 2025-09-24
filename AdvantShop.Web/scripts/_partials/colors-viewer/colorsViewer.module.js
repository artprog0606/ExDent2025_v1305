import './styles/colors-viewer.scss';

import ColorsViewerCtrl from './controllers/colorsViewerController.js';
import { colorsViewerDirective, colorsViewerItemBeforeComponent } from './directives/colorsViewerDirectives.js';

const moduleName = 'colorsViewer';

angular
    .module(moduleName, [])
    .controller('ColorsViewerCtrl', ColorsViewerCtrl)
    .directive('colorsViewer', colorsViewerDirective)
    .directive('colorsViewerItemBefore', colorsViewerItemBeforeComponent);

export default moduleName;
