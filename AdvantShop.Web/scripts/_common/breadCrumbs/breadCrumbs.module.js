import './breadcrumbs.scss';
import { breadCrumbsDirective } from './breadCrumbs.component.js';
import BreadCrumbsCtrl from './breadCrumbs.ctrl.js';

const moduleName = 'breadCrumbs';

angular.module(moduleName, []).controller('BreadCrumbsCtrl', BreadCrumbsCtrl).directive('breadCrumbs', breadCrumbsDirective);

export default moduleName;
