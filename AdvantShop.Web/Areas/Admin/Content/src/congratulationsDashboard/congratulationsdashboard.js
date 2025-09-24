(function (ng) {
    'use strict';

    var CongratulationsDashboardCtrl = function (toaster, $http, $translate, $scope, urlHelper) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.getData(true);
        };

        ctrl.getData = function (isFirstLoad) {
            return $http
                .get('home/getCongratulationsDashboardData', {
                    params: { isTest: urlHelper.getUrlParamByName('istest') },
                })
                .then(function (response) {
                    var data = response.data.obj;

                    ctrl.data = data;

                    if (isFirstLoad) {
                        ctrl.storeName = data.StoreName;
                        ctrl.countryId = data.CountryId;
                        ctrl.regionId = data.RegionId;
                        ctrl.regions = data.Regions;
                        ctrl.hasRegions = data.Regions != null && data.Regions.length > 0;
                        ctrl.city = data.City;
                        ctrl.phone = data.Phone;
                    }
                    ctrl.steps = data.Steps;
                    ctrl.currentStepComplited = 0;

                    for (var i = 0; i < ctrl.steps.length; i++) {
                        if (ctrl.steps[i].Activated) {
                            ctrl.currentStepComplited += 1;
                        }
                    }

                    if (!ctrl.loopStarted) {
                        ctrl.loop();
                    }
                });
        };

        ctrl.getRegions = function () {
            $http.post('settings/getRegions', { countryId: ctrl.countryId }).then(function (response) {
                ctrl.regions = response.data.obj;
                if (response.data.obj.length) {
                    ctrl.regionId = response.data.obj[0].Value;
                    ctrl.regions = response.data.obj;
                    ctrl.hasRegions = true;
                } else {
                    ctrl.hasRegions = false;
                }
            });
        };

        ctrl.saveStoreInfo = function () {
            return $http
                .post('home/saveStoreInfo', {
                    storeName: ctrl.storeName,
                    countryId: ctrl.countryId,
                    regionId: ctrl.regionId,
                    city: ctrl.city,
                    phone: ctrl.phone,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result) {
                        ctrl.steps[0].Activated = true;
                        ctrl.currentStepComplited += 1;
                        //$scope.$emit('adminShopNameUpdated', { shopName: ctrl.storeName });
                    } else {
                        toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                });
        };

        ctrl.loop = function () {
            ctrl.loopStarted = true;
            setTimeout(function () {
                ctrl.getData().then(function () {
                    ctrl.loop();
                });
            }, 3000);
        };

        ctrl.trackEvent = function (trackEvent) {
            $http.post('home/trackCongratulationsDashboardEvents', { trackEvent: trackEvent });
        };
    };

    CongratulationsDashboardCtrl.$inject = ['toaster', '$http', '$translate', '$scope', 'urlHelper'];

    ng.module('congratulationsDashboard', []).controller('CongratulationsDashboardCtrl', CongratulationsDashboardCtrl);
})(window.angular);
