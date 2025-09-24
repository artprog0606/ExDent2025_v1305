import catalogModule from './catalog.source.js';
import appDependency from '../../../../../scripts/appDependency.js';

import '../../../../../scripts/_common/spinbox/spinbox.module.js';
appDependency.addItem(`spinbox`);

appDependency.addItem(catalogModule);
appDependency.addItem(`swipeLine`);
appDependency.addItem(`recalc`);
appDependency.addItem(`ngClickCapture`);
appDependency.addItem(`productGridItem`);
