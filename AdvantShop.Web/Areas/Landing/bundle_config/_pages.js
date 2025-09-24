import PagesStorage from '../../../node_scripts/pagesStorage.js';
import { getDirname } from '../../../node_scripts/shopPath.js';
import shippingMethods from '../../../bundle_config/_shippingMethods.js';
let obj = new PagesStorage(getDirname(import.meta.url));

obj.addItem('head', 'head.js');
obj.addItem('common', 'common.js');
obj.addItem('admin', 'admin.js');
obj.addItem('adminMobile', 'adminMobile.js');

obj.addItem('inplaceFunnelMin', 'inplaceFunnelMin.js');
obj.addItem('inplaceFunnelMax', 'inplaceFunnelMax.js');

obj.addItem('uiAce', '../../Admin/bundle_config/uiAce.js');

obj.addItem('myaccountFunnel', 'myaccountFunnel.js');

obj.addItem('logogeneratorFunnel', 'logogeneratorFunnel.js');

obj.addItem('mokka', '../../../scripts/_partials/payment/widgets/mokka/mokkaCtrl.js');

obj.assign(shippingMethods);

export default obj;
