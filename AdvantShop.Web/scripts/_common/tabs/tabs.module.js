import './styles/tabs.scss';

import tabsService from './services/tabsService.js';
import { tabsDirective, tabHeaderDirective, tabContentDirective, tabsGotoDirective } from './directives/tabsDirectives.js';

import TabsCtrl from './controllers/tabsController.js';
import TabHeaderCtrl from './controllers/tabHeaderController.js';
import TabContentCtrl from './controllers/tabContentController.js';

const moduleName = 'tabs';

angular
    .module('tabs', [])
    .service('tabsService', tabsService)
    .controller('TabsCtrl', TabsCtrl)
    .controller('TabHeaderCtrl', TabHeaderCtrl)
    .controller('TabContentCtrl', TabContentCtrl)
    .directive('tabs', tabsDirective)
    .directive('tabHeader', tabHeaderDirective)
    .directive('tabContent', tabContentDirective)
    .directive('tabsGoto', tabsGotoDirective);

export default moduleName;
