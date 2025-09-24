import { scrollSpyService } from './scrollSpy.service.js';
import { scrollSpyDirective } from './scrollSpy.directive.js';

const MODULE_NAME = 'scrollSpy';
angular
    .module(MODULE_NAME, [])
    .constant('scrollSpyConfig', {
        observe: {
            rootMargin: '50px 0px 0px 50px',
        },
        alignHorizontal: true,
    })
    .service('scrollSpyService', scrollSpyService)
    .directive('scrollSpy', scrollSpyDirective);
export default MODULE_NAME;
