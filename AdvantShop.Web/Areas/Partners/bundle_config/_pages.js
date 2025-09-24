import PagesStorage from '../../../node_scripts/pagesStorage.js';
import { getDirname } from '../../../node_scripts/shopPath.js';
let obj = new PagesStorage(getDirname(import.meta.url));

obj.addItem('head', 'head.js');
obj.addItem('common', 'common.js');
obj.addItem('inplaceMax', 'inplaceMax.js');
obj.addItem('chart', 'chart.js');
export default obj;
