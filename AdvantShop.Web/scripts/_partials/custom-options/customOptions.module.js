import './styles/customOptions.scss';

import CustomOptionsCtrl from './controllers/customOptionsController.js';
import { customOptionsDirective } from './directives/customOptionsDirectives.js';
import customOptionsService from './services/customOptionsService.js';

const moduleName = 'customOptions';

angular
    .module(moduleName, [])
    .controller('CustomOptionsCtrl', CustomOptionsCtrl)
    .directive('customOptions', customOptionsDirective)
    .service('customOptionsService', customOptionsService);

export default moduleName;
