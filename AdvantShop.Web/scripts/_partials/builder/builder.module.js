import './styles/builder.scss';
import './styles/new-builder.scss';
import './styles/new-builder-theme.scss';
import '../../../styles/common/sidebar.scss';
import '../../../styles/common/progressbar-css.scss';
import '../../../styles/common/spinner.scss';

import './templates/newBuilder/body.html';
import './templates/newBuilder/footer.html';
import './templates/newBuilder/sectionBrands.html';
import './templates/newBuilder/sectionNews.html';
import './templates/newBuilder/sectionCategory.html';
import './templates/newBuilder/sectionCheckout.html';
import './templates/newBuilder/sectionDesign.html';
import './templates/newBuilder/sectionMainPage.html';
import './templates/newBuilder/sectionOtherItems.html';
import './templates/newBuilder/sectionProduct.html';

import '../../../Areas/Admin/Content/src/_shared/switch-on-off/switchOnOff.js';

import cmStatModule from '../../../Areas/Admin/Content/src/_shared/cm-stat/cmStat.module.js';
import helpTriggerModule from '../../../Areas/Admin/Content/src/_partials/help-trigger/helpTrigger.module.js';
import '../../../Areas/Admin/Content/src/_shared/is-mobile/is-mobile.js';
import uiAceTextareaModule from '../../../Areas/Admin/Content/src/_shared/ui-ace-textarea/uiAceTextarea.module.js';
import colorsViewerModule from '../colors-viewer/colorsViewer.module.js';
import '../../../Areas/Admin/Content/src/csseditor/csseditor.js';

import tabsModule from '../../_common/tabs/tabs.module.js';

import { NewBuilderController, BuilderOtherSettingsController } from './controllers/NewBuilderController.js';
import {
    builderTriggerDirective,
    builderStylesheetDirective,
    newBuilderTriggerDirective,
    builderTriggerOtherSettingsDirective,
} from './directives/builderDirectives.js';
import builderService from './services/builderService.js';

const moduleName = 'builder';

angular
    .module(moduleName, ['sidebarsContainer', tabsModule, cmStatModule, helpTriggerModule, uiAceTextareaModule, 'csseditor', colorsViewerModule])
    .constant('builderTypes', {
        colorScheme: 'colorScheme',
        theme: 'theme',
        background: 'background',
    })
    .service('builderService', builderService)
    .controller('NewBuilderCtrl', NewBuilderController)
    .controller('BuilderOtherSettingsCtrl', BuilderOtherSettingsController)
    .directive('builderTrigger', builderTriggerDirective)
    .directive('builderStylesheet', builderStylesheetDirective)
    .directive('newBuilderTrigger', newBuilderTriggerDirective)
    .directive('builderTriggerOtherSettings', builderTriggerOtherSettingsDirective);

export default moduleName;
