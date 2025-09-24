import { scrollToBlockService } from './scrollToBlock.service.js';
import { scrollToBlockDirective } from './scrollToBlock.directive.js';
const MODULE_NAME = 'scrollToBlock';
angular
    .module(MODULE_NAME, [])
    .constant('scrollToBlockConfig', {
        calcExtend: null,
        offsetTop: 0,
        smooth: true,
    })
    .directive('scrollToBlock', scrollToBlockDirective)
    .service('scrollToBlockService', scrollToBlockService);
export default MODULE_NAME;
