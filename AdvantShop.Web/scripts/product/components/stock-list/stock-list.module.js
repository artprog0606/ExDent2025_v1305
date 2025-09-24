import StockListDirective from './stock-list.directive.js';
import './styles.scss';

const moduleName = 'stockList';

angular.module(moduleName, []).directive('stockList', StockListDirective);

export default moduleName;
