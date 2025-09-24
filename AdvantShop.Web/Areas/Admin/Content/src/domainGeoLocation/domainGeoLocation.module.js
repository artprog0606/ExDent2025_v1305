import DomainGeoLocationListCtrl from './domainGeoLocationList.controller.js';
import domainGeoLocationService from './domainGeoLocation.service.js';

const MODULE_NAME = 'domainOfCity';

angular
    .module(MODULE_NAME, ['uiGridCustom'])
    .controller('DomainGeoLocationListCtrl', DomainGeoLocationListCtrl)
    .service('domainGeoLocationService', domainGeoLocationService);

export default MODULE_NAME;
