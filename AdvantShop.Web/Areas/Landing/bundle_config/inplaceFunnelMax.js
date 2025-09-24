import '../../../scripts/_partials/inplace/styles/inplace.scss';

import {
    inplaceStartDirective,
    inplaceSwitchDirective,
    inplaceProgressDirective,
} from '../../../scripts/_partials/inplace/directives/inplaceDirectivesMinimum.js';

import {
    inplaceRichDirectives,
    inplaceRichButtonsDirective,
    inplacePriceDirective,
    inplacePriceButtonsDirective,
    inplacePricePanelDirective,
    inplaceModalDirective,
    inplaceAutocompleteDirective,
    inplaceAutocompleteButtonsDirective,
    inplacePropertiesNewDirective,
    inplaceImageDirective,
    inplaceImageButtonsDirective,
} from '../../../scripts/_partials/inplace/directives/inplaceDirectives.js';
import InplaceSwitchCtrl from '../../../scripts/_partials/inplace/controllers/inplaceSwitchController.js';
import InplaceRichCtrl from '../../../scripts/_partials/inplace/controllers/inplaceRichController.js';
import InplaceRichButtonsCtrl from '../../../scripts/_partials/inplace/controllers/inplaceRichButtonsController.js';
import InplaceModalCtrl from '../../../scripts/_partials/inplace/controllers/inplaceModalController.js';
import InplaceAutocompleteCtrl from '../../../scripts/_partials/inplace/controllers/inplaceAutocompleteController.js';
import InplaceAutocompleteButtonsCtrl from '../../../scripts/_partials/inplace/controllers/inplaceAutocompleteButtonsController.js';
import InplacePropertiesNewCtrl from '../../../scripts/_partials/inplace/controllers/inplacePropertiesNewController.js';
import InplaceImageCtrl from '../../../scripts/_partials/inplace/controllers/inplaceImageController.js';
import InplaceImageButtonsCtrl from '../../../scripts/_partials/inplace/controllers/inplaceImageButtonsController.js';
import InplacePriceCtrl from '../../../scripts/_partials/inplace/controllers/inplacePriceController.js';
import InplacePriceButtonsCtrl from '../../../scripts/_partials/inplace/controllers/inplacePriceButtonsController.js';
import InplacePricePanelCtrl from '../../../scripts/_partials/inplace/controllers/inplacePricePanelController.js';
import InplaceProgressCtrl from '../../../scripts/_partials/inplace/controllers/inplaceProgressController.js';
import inplaceService from '../../../scripts/_partials/inplace/services/inplaceService.js';
import inplaceRichConfig from '../../../scripts/_partials/inplace/inplaceRichConfig.js';
import inplaceConfig from '../../../scripts/_partials/inplace/inplaceConfig.js';

const moduleName = 'inplace';

angular
    .module(moduleName, [])
    .constant('inplaceConfig', inplaceConfig)
    .constant('inplaceRichConfig', inplaceRichConfig)
    .service('inplaceService', inplaceService)
    .controller('InplaceRichCtrl', InplaceRichCtrl)
    .controller('InplaceRichButtonsCtrl', InplaceRichButtonsCtrl)
    .controller('InplaceModalCtrl', InplaceModalCtrl)
    .controller('InplaceAutocompleteCtrl', InplaceAutocompleteCtrl)
    .controller('InplaceAutocompleteButtonsCtrl', InplaceAutocompleteButtonsCtrl)
    .controller('InplacePropertiesNewCtrl', InplacePropertiesNewCtrl)
    .controller('InplaceImageCtrl', InplaceImageCtrl)
    .controller('InplaceImageButtonsCtrl', InplaceImageButtonsCtrl)
    .controller('InplacePriceCtrl', InplacePriceCtrl)
    .controller('InplacePriceButtonsCtrl', InplacePriceButtonsCtrl)
    .controller('InplacePricePanelCtrl', InplacePricePanelCtrl)
    .controller('InplaceProgressCtrl', InplaceProgressCtrl)
    .controller('InplaceSwitchCtrl', InplaceSwitchCtrl)
    .directive('inplaceStart', inplaceStartDirective)
    .directive('inplaceSwitch', inplaceSwitchDirective)
    .directive('inplaceProgress', inplaceProgressDirective)
    .directive('inplaceRich', inplaceRichDirectives)
    .directive('inplaceRichButtons', inplaceRichButtonsDirective)
    .directive('inplacePrice', inplacePriceDirective)
    .directive('inplacePriceButtons', inplacePriceButtonsDirective)
    .directive('inplacePricePanel', inplacePricePanelDirective)
    .directive('inplaceModal', inplaceModalDirective)
    .directive('inplaceAutocomplete', inplaceAutocompleteDirective)
    .directive('inplaceAutocompleteButtons', inplaceAutocompleteButtonsDirective)
    .directive('inplacePropertiesNew', inplacePropertiesNewDirective)
    .directive('inplaceImage', inplaceImageDirective)
    .directive('inplaceImageButtons', inplaceImageButtonsDirective);
