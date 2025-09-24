import staticPageModule from '../../../scripts/staticPage/staticPage.module.js';
import photoViewerModule from '../../../scripts/_common/photoViewer/photoViewer.module.js';

import appDependency from '../../../scripts/appDependency.js';

appDependency.addItem(staticPageModule);
appDependency.addItem(photoViewerModule);

import '../styles/views/staticpage.scss';
