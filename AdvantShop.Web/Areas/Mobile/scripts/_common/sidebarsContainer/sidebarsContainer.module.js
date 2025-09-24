import sidebarsContainerService from './sidebarsContainer.service.js';
import SidebarsContainerCtrl from './sidebarsContainer.ctrl.js';
import {
    sidebarsContainerDirective,
    sidebarContainerCloseDirective,
    sidebarContainerStateDirective,
    sidebarContentStaticComponent,
    sidebarContainerSaveDirective,
    sidebarHookCloseDirective,
} from './sidebarsContainer.component.js';

const moduleName = 'sidebarsContainer';

angular
    .module(moduleName, [])
    .service('sidebarsContainerService', sidebarsContainerService)
    .controller('SidebarsContainerCtrl', SidebarsContainerCtrl)
    .directive('sidebarsContainer', sidebarsContainerDirective)
    .directive('sidebarContainerClose', sidebarContainerCloseDirective)
    .directive('sidebarContainerState', sidebarContainerStateDirective)
    .component('sidebarContentStatic', sidebarContentStaticComponent)
    .directive('sidebarContainerSave', sidebarContainerSaveDirective)
    .directive('sidebarHookClose', sidebarHookCloseDirective);

export default moduleName;
