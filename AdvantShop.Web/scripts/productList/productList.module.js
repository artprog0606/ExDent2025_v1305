import catalogModule from '../catalog/catalog.module.js';

const moduleName = 'productList';

import ProductListCtrl from './controllers/productListController.js';

angular.module(moduleName, [catalogModule]).controller('ProductListCtrl', ProductListCtrl);

export default moduleName;
