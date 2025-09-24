import '../../../scripts/_partials/inplace/styles/inplace.scss';

import InplaceProgressCtrl from '../../../scripts/_partials/inplace/controllers/inplaceProgressController.js';
import InplaceSwitchCtrl from '../../../scripts/_partials/inplace/controllers/inplaceSwitchController.js';
import inplaceService from '../../../scripts/_partials/inplace/services/inplaceService.js';
import {
    inplaceStartDirective,
    inplaceSwitchDirective,
    inplaceProgressDirective,
} from '../../../scripts/_partials/inplace/directives/inplaceDirectivesMinimum.js';

import inplaceRichConfig from '../../../scripts/_partials/inplace/inplaceRichConfig.js';
import inplaceConfig from '../../../scripts/_partials/inplace/inplaceConfig.js';

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
