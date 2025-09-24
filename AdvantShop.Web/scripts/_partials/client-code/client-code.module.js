import { clientCodeDirective } from './directives/clientCodeDirective.js';
import clientCodeService from './services/clientCodeService.js';

const moduleName = 'clientCode';
angular.module(moduleName, []).directive('clientCode', clientCodeDirective).service('clientCodeService', clientCodeService);

export default moduleName;
