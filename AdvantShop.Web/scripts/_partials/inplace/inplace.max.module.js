import ngFileUploadModule from '../../../node_modules/ng-file-upload/index.js';

import './styles/inplace.scss';

import { inplaceStartDirective, inplaceSwitchDirective, inplaceProgressDirective } from './directives/inplaceDirectivesMinimum.js';

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
} from './directives/inplaceDirectives.js';
import InplaceSwitchCtrl from './controllers/inplaceSwitchController.js';
import InplaceRichCtrl from './controllers/inplaceRichController.js';
import InplaceRichButtonsCtrl from './controllers/inplaceRichButtonsController.js';
import InplaceModalCtrl from './controllers/inplaceModalController.js';
import InplaceAutocompleteCtrl from './controllers/inplaceAutocompleteController.js';
import InplaceAutocompleteButtonsCtrl from './controllers/inplaceAutocompleteButtonsController.js';
import InplacePropertiesNewCtrl from './controllers/inplacePropertiesNewController.js';
import InplaceImageCtrl from './controllers/inplaceImageController.js';
import InplaceImageButtonsCtrl from './controllers/inplaceImageButtonsController.js';
import InplacePriceCtrl from './controllers/inplacePriceController.js';
import InplacePriceButtonsCtrl from './controllers/inplacePriceButtonsController.js';
import InplacePricePanelCtrl from './controllers/inplacePricePanelController.js';
import InplaceProgressCtrl from './controllers/inplaceProgressController.js';
import inplaceService from './services/inplaceService.js';
import inplaceRichConfig from './inplaceRichConfig.js';
import inplaceConfig from './inplaceConfig.js';

const moduleName = 'inplace';

angular
    .module(moduleName, [ngFileUploadModule])
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

export default moduleName;
