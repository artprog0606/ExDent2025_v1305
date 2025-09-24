import './styles/logoGenerator.scss';

import 'tinycolor2';
import colorpicker from '../../../node_modules/angularjs-color-picker/dist/angularjs-color-picker.js';
import '../../../node_modules/angularjs-color-picker/dist/angularjs-color-picker.css';

import tabsModule from '../../_common/tabs/tabs.module.js';

import LogoGeneratorCtrl from './controllers/logoGeneratorController.js';
import LogoGeneratorFontsCtrl from './controllers/logoGeneratorFontsController.js';
import LogoGeneratorModalCtrl from './controllers/logoGeneratorModalController.js';
import LogoGeneratorPreviewCtrl from './controllers/logoGeneratorPreviewController.js';
import LogoGeneratorTriggerCtrl from './controllers/logoGeneratorTriggerController.js';

import logoGeneratorService from './services/logoGeneratorService.js';

import logoGeneratorFontSupport from './filters/logoGeneratorFilter.js';

import {
    logoGeneratorStartDirective,
    logoGeneratorPreviewLogoDirective,
    logoGeneratorPreviewSloganDirective,
    logoGeneratorComponent,
    logoGeneratorFontsComponent,
    logoGeneratorPreviewComponent,
    logoGeneratorTriggerComponent,
} from './components/logoGeneratorComponent.js';

const moduleName = 'logoGenerator';

angular
    .module(moduleName, [colorpicker.name, tabsModule])
    .controller('LogoGeneratorCtrl', LogoGeneratorCtrl)
    .controller('LogoGeneratorFontsCtrl', LogoGeneratorFontsCtrl)
    .controller('LogoGeneratorModalCtrl', LogoGeneratorModalCtrl)
    .controller('LogoGeneratorPreviewCtrl', LogoGeneratorPreviewCtrl)
    .controller('LogoGeneratorTriggerCtrl', LogoGeneratorTriggerCtrl)
    .service('logoGeneratorService', logoGeneratorService)
    .filter('logoGeneratorFontSupport', logoGeneratorFontSupport)
    .directive('logoGeneratorStart', logoGeneratorStartDirective)
    .directive('logoGeneratorPreviewLogo', logoGeneratorPreviewLogoDirective)
    .directive('logoGeneratorPreviewSlogan', logoGeneratorPreviewSloganDirective)
    .component('logoGenerator', logoGeneratorComponent)
    .component('logoGeneratorFonts', logoGeneratorFontsComponent)
    .component('logoGeneratorPreview', logoGeneratorPreviewComponent)
    .component('logoGeneratorTrigger', logoGeneratorTriggerComponent);

export default moduleName;
