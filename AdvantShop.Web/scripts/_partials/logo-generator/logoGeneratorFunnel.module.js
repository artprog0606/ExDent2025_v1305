import './styles/logoGenerator.scss';

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
    .module(moduleName, [])
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
