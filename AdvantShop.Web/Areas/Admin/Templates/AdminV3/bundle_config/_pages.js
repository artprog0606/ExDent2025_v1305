import PagesStorage from '../../../../../node_scripts/pagesStorage.js';
import { getDirname } from '../../../../../node_scripts/shopPath.js';
let obj = new PagesStorage(getDirname(import.meta.url));

// obj.addItem('chart', './Areas/Admin/bundle_config/chart.js');
// obj.addItem('avito', './Areas/Admin/Content/src/avito/avito.js');
// obj.addItem('jqueryUiSelectable', './Areas/Admin/Content/vendors/jquery-ui.selectable/jquery-ui.selectable.js');
// obj.addItem('uiAce', './Areas/Admin/bundle_config/uiAce.js');

obj.addItem('commonV3', 'commonV3.js');

export default obj;
