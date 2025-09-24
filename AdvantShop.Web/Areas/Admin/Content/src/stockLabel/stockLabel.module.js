import StockLabelListCtrl from './stockLabelList.controller.js';
import stockLabelService from './stockLabel.service.js';

const MODULE_NAME = 'stockLabel';

angular.module(MODULE_NAME, ['uiGridCustom']).controller('StockLabelListCtrl', StockLabelListCtrl).service('stockLabelService', stockLabelService);

export default MODULE_NAME;
