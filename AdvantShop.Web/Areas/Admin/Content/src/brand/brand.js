(function (ng) {
    'use strict';

    var BrandCtrl = function ($http, $window, SweetAlert, $translate) {
        var ctrl = this;
        ctrl.PhotoId = 0;

        ctrl.updateImage = function (result) {
            ctrl.PhotoId = result.pictureId;
        };

        ctrl.deleteBrand = function (brandId) {
            SweetAlert.confirm($translate.instant('Admin.Js.Brand.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Brand.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('brands/deleteBrand', { brandId: brandId }).then(function (response) {
                        //$window.location.assign('brands');
                        $window.location.assign('settingscatalog?catalogTab=brand');
                    });
                }
            });
        };
    };

    BrandCtrl.$inject = ['$http', '$window', 'SweetAlert', '$translate'];

    ng.module('brand', ['uiGridCustom', 'urlGenerator']).controller('BrandCtrl', BrandCtrl);
})(window.angular);
