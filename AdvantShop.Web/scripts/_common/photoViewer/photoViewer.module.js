import './photo-viewer.scss';
import { pluginDirective, magnificPopupDirective, photoViewerDirective } from './photoViewer.directive.js';

const moduleName = 'photoViewer';

angular
    .module(moduleName, ['oc.lazyLoad'])
    .constant('photoViewerDefaultOptions', {
        navbar: false,
        transition: false,
        inheritedAttributes: ['crossOrigin', 'decoding', 'isMap', 'loading', 'referrerPolicy', 'useMap'],
        toolbar: {
            zoomIn: false,
            zoomOut: false,
            oneToOne: false,
            reset: false,
            prev: { show: true, size: 'large' },
            play: false,
            next: { show: true, size: 'large' },
            rotateLeft: false,
            rotateRight: false,
            flipHorizontal: false,
            flipVertical: false,
        },
    })
    .directive('plugin', pluginDirective)
    .directive('magnificPopup', magnificPopupDirective)
    .directive('photoViewer', photoViewerDirective);

export default moduleName;
