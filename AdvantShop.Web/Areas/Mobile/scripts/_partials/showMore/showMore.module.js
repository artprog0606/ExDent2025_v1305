import ShowMoreCtrl from './showMoreController.js';
import { showMoreDirective, showMoreInitHtmlDirective } from './showMoreDirective.js';

const moduleName = 'showMore';

angular
    .module(moduleName, [])
    .controller('ShowMoreCtrl', ShowMoreCtrl)
    .directive('showMoreInitHtml', showMoreInitHtmlDirective)
    .directive('showMore', showMoreDirective);

export default moduleName;
