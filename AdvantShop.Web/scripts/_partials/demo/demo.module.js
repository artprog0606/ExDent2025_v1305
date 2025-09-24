import DemoCtrl from './controllers/demoController.js';
import { demoModalDirective } from './directives/demoDirectives.js';

const moduleName = 'demo';

angular.module('demo', []).controller('DemoCtrl', DemoCtrl).directive('demoModal', demoModalDirective);

export default moduleName;
