import { ContentLoaderService } from './content-loader.service.js';
import { ContentLoaderPlaceCtrl } from './content-loader-place.ctrl.js';
import { ContentLoaderPlaceDirective } from './content-loader.components.js';

const moduleName = 'contentLoader';

angular
    .module(moduleName, [])
    .service('contentLoaderService', ContentLoaderService)
    .controller('ContentLoaderPlaceCtrl', ContentLoaderPlaceCtrl)
    .directive('contentLoaderPlace', ContentLoaderPlaceDirective);

export default moduleName;
