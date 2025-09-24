import appDependency from '../../../../../../scripts/appDependency.js';

import popover from 'angular-ui-bootstrap/src/popover/index.js';
import '../../../../../../vendors/ui-bootstrap-custom/styles/ui-popover.css';

appDependency.addItem(popover);

import './styles/help-trigger.scss';

import './helpTrigger.js';
import './helpTrigger.service.js';
import './helpTrigger.component.js';

export default 'helpTrigger';
