import apiMapService from './apiMap.service.js';
import { apiMapKeyDirective } from './apiMap.directives.js';

const moduleName = 'apiMap';

angular.module(moduleName, []).service('apiMapService', apiMapService).directive('apiMapKey', apiMapKeyDirective);

export default moduleName;
