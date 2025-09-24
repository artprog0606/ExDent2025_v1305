import './styles/searchPanel.scss';
import SearchPanelCtrl from './controllers/searchPanelController.js';
import { searchIconDirective, searchPanelDirective } from './directives/searchPanelDirectives.js';

const moduleName = 'searchPanel';

angular
    .module(moduleName, [])
    .controller('searchPanelCtrl', SearchPanelCtrl)
    .directive('searchIcon', searchIconDirective)
    .directive('searchPanel', searchPanelDirective);

export default moduleName;
