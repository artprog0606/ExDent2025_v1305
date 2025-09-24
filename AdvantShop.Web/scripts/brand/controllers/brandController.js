/* @ngInject */
function BrandCtrl($window, brandService) {
    var ctrl = this;

    ctrl.changeCountyId = function (curSort) {
        brandService.filterRefresh(brandService.buildUrlParams($window.location.search, 'countryId', curSort));
    };

    ctrl.changeLetter = function (curLetter) {
        brandService.filterRefresh(brandService.buildUrlParams($window.location.search, 'letter', curLetter));
    };

    ctrl.changeBrandname = function (event, curName) {
        if (curName != null && ((event.type == 'keypress' && event.keyCode == 13) || event.type == 'click')) {
            brandService.filterRefresh(brandService.buildUrlParams($window.location.search, 'q', curName));
        }
    };
}

export default BrandCtrl;
