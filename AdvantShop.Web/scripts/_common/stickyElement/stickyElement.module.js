import StickyElement from './directives/stickyElementDirective.js';
import StickyElementService from './services/stickyElementService.js';
import './styles/stickyElementDirective.scss';

const moduleName = `stickyElement`;

angular.module(`stickyElement`, []).service(`stickyElementService`, StickyElementService).directive(`stickyElement`, StickyElement);

export default moduleName;
