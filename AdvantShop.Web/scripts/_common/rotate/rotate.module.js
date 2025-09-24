import './screenfull.cjs';
import './threesixty.js';
import './styles/rotate.scss';

import RotateCtrl from './controllers/rotateController.js';
import { rotateDirective } from './directives/rotateDirectives.js';

const moduleName = 'rotate';

angular.module(moduleName, []).controller('RotateCtrl', RotateCtrl).directive('rotate', rotateDirective);

export default moduleName;
