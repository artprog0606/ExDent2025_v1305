import '../Content/styles/_shared/chips/chips.scss';
import './design.js';
import '../../../Content/src/csseditor/csseditor.js';
import appDependency from '../../../../../scripts/appDependency.js';
import '../../../Content/src/settingsTemplate/settingsTemplate.js';
appDependency.addItem('settingsTemplate');

import '../Content/styles/views/settings.scss';

import '../../../Content/styles/design_style.css';

import '../../../../../scripts/_partials/zone/zone.js';
import zoneModule from '../../../../../scripts/_partials/zone/zone.module.ts';
appDependency.addItem(zoneModule);

import '../../../../../scripts/_common/modal/modal.module.js';
appDependency.addItem('modal');

import cmStat from '../../../Content/src/_shared/cm-stat/cmStat.module.js';
appDependency.addItem(cmStat);

import uiAceTextarea from '../../../Content/src/_shared/ui-ace-textarea/uiAceTextarea.module.js';
appDependency.addItem(uiAceTextarea);
