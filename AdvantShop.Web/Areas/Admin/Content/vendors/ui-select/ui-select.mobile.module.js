import appDependency from '../../../../../scripts/appDependency.js';

// import uiSelectModule from 'ui-select';
import uiSelectModule from './index.js';
import './ui-select.decorate.js';

// import '../../../../../node_modules/ui-select/dist/select.min.css';
import './src/select.css';
import './ui-select-required-mobile.scss';

import 'ui-select-infinity';
appDependency.addItem(`ui-select-infinity`);

//import './ui-select.scss';

export default uiSelectModule;