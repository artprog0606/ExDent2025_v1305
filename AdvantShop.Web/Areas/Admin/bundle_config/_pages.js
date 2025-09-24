import PagesStorage from '../../../node_scripts/pagesStorage.js';
import { getDirname } from '../../../node_scripts/shopPath.js';
import shippingMethods from '../../../bundle_config/_shippingMethods.js';

let obj = new PagesStorage(getDirname(import.meta.url));

obj.assign(shippingMethods);

obj.addItem('common', 'common.js');
obj.addItem('login', 'login.js');

obj.addItem('chart', 'chart.js');
obj.addItem('avito', '../Content/src/avito/avito.js');
obj.addItem('jqueryUiSelectable', '../Content/vendors/jquery-ui.selectable/jquery-ui.selectable.js');
obj.addItem('uiAce', 'uiAce.js');

obj.addItem('mokka', '../../../scripts/_partials/payment/widgets/mokka/mokkaCtrl.js');

obj.addItem('firstTimeCreate', 'firstTimeCreate.js');
obj.addItem('funnelsScheme', 'funnelsScheme.js');
obj.addItem('mobileOverlap', 'mobileOverlap.js');
obj.addItem('userInfoPopupModule', 'userInfoPopupModule.js');

export default obj;
