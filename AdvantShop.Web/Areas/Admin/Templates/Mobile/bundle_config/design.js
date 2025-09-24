import photoViewerModule from '../../../../../scripts/_common/photoViewer/photoViewer.module.js';
appDependency.addItem(photoViewerModule);

import 'ng-file-upload';

import '../../../Content/src/design/design.js';
import '../../../Content/src/design/design.service.js';

import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/views/design.scss';
import '../Content/styles/views/settings.scss';
import '../Content/styles/views/settings-list.scss';

import uiAceTextarea from '../../../Content/src/_shared/ui-ace-textarea/uiAceTextarea.module.js';

import appDependency from '../../../../../scripts/appDependency.js';
appDependency.addItem('design');
