import { maskDirective, maskConfigDirective } from './mask.js';
import maskControlService from './mask.service.js';

const moduleName = 'mask';

angular
    .module(moduleName, [])
    .service('maskControlService', maskControlService)
    .directive('maskControl', maskDirective)
    .directive('maskConfig', maskConfigDirective);

export default moduleName;
