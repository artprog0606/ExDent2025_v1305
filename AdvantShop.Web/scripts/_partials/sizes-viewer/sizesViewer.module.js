import './styles/sizes-viewer.scss';

import SizesViewerCtrl from './controllers/sizesViewerController.js';
import { sizesViewerDirective } from './directives/sizesViewerDirectives.js';

const moduleName = 'sizesViewer';

angular
    .module(moduleName, [])
    .constant('sizesViewerConfig', {
        isEnableSlider: 'true',
        visibleItems: 7,
        width: '35px',
        height: '35px',
    })
    .controller('SizesViewerCtrl', SizesViewerCtrl)
    .directive('sizesViewer', sizesViewerDirective);

export default moduleName;
