import PhotoViewListCtrl from './photo-view-list.ctrl.js';
import { PhotoViewList, PhotoViewListItem, PhotoViewListNav } from './photo-view-list.directive.js';
import './photo-view-list.scss';

const moduleName = `photoViewList`;

angular
    .module(moduleName, [])
    .directive(`photoViewList`, PhotoViewList)
    .directive(`photoViewListItem`, PhotoViewListItem)
    .directive(`photoViewListNav`, PhotoViewListNav)
    .controller(`PhotoViewListCtrl`, PhotoViewListCtrl);

export default moduleName;
