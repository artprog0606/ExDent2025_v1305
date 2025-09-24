import './styles/popover.scss';

import PopoverCtrl from './controllers/popoverController.js';
import PopoverControlCtrl from './controllers/popoverControlController.js';
import PopoverOverlayCtrl from './controllers/PopoverOverlayController.js';
import { popoverControlDirective, popoverDirective, popoverOverlayDirective } from './directives/popoverDirectives.js';

import popoverService from './services/popoverService.js';

const moduleName = 'popover';

angular
    .module('popover', [])
    .constant('popoverConfig', {
        popoverTrigger: 'mouseenter',
        popoverTriggerHide: 'mouseleave',
        popoverShowOnLoad: false,
        popoverOverlayEnabled: false,
        popoverPosition: 'top',
        popoverIsFixed: false,
        popoverIsCanHover: false,
        popoverShowOne: false,
        popoverShowDelay: 0,
        popoverShowCross: false,
    })
    .service('popoverService', popoverService)
    .controller('PopoverCtrl', PopoverCtrl)
    .controller('PopoverControlCtrl', PopoverControlCtrl)
    .controller('PopoverOverlayCtrl', PopoverOverlayCtrl)
    .directive('popoverControl', popoverControlDirective)
    .directive('popover', popoverDirective)
    .directive('popoverOverlay', popoverOverlayDirective);

export default moduleName;
