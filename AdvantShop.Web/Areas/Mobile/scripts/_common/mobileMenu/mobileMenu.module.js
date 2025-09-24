import MobileMenuCtrl from './mobileMenu.ctrl.js';
import MobileMenuItemCtrl from './mobileMenuItem.ctrl.js';
import MobileMenuRootCtrl from './mobileMenuRoot.ctrl.js';
import { mobileMenuRootDirective, mobileMenuDirective, mobileMenuItemDirective } from './mobileMenu.component.js';

const moduleName = 'mobileMenu';

angular
    .module(moduleName, [])
    .controller('MobileMenuCtrl', MobileMenuCtrl)
    .controller('MobileMenuItemCtrl', MobileMenuItemCtrl)
    .controller('MobileMenuRootCtrl', MobileMenuRootCtrl)
    .directive('mobileMenuRoot', mobileMenuRootDirective)
    .directive('mobileMenu', mobileMenuDirective)
    .directive('mobileMenuItem', mobileMenuItemDirective);

export default moduleName;
