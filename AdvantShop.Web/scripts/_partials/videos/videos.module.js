import iframeResponsive from '../../_common/iframe-responsive/iframeResponsive.module.js';

import VideosCtrl from './controllers/videosController.js';
import { videosDirective } from './directives/videosDirective.js';

const moduleName = 'videos';

const deps = [iframeResponsive];

angular.module(moduleName, deps).controller('VideosCtrl', VideosCtrl).directive('videos', videosDirective);

export default moduleName;
