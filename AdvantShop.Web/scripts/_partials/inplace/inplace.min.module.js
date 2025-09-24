import './styles/inplace.scss';

import InplaceProgressCtrl from './controllers/inplaceProgressController.js';
import InplaceSwitchCtrl from './controllers/inplaceSwitchController.js';
import inplaceService from './services/inplaceService.js';
import { inplaceStartDirective, inplaceSwitchDirective, inplaceProgressDirective } from './directives/inplaceDirectivesMinimum.js';

import inplaceRichConfig from './inplaceRichConfig.js';
import inplaceConfig from './inplaceConfig.js';

const moduleName = 'inplace';

angular
    .module('inplace', [])
    .constant('inplaceConfig', inplaceConfig)
    .constant('inplaceRichConfig', inplaceRichConfig)
    .service('inplaceService', inplaceService)
    .controller('InplaceProgressCtrl', InplaceProgressCtrl)
    .controller('InplaceSwitchCtrl', InplaceSwitchCtrl)
    .directive('inplaceStart', inplaceStartDirective)
    .directive('inplaceSwitch', inplaceSwitchDirective)
    .directive('inplaceProgress', inplaceProgressDirective);

export default moduleName;
