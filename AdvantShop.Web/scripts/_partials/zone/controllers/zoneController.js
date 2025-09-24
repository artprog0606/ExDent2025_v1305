(function (ng) {
    'use strict';

    var ZoneCtrl = function (zoneService, $timeout, zoneEvents, $window) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.minCountInColumns = 6;
            ctrl.zoneCity = '';
            ctrl.isProgress = true;

            zoneService
                .getDataForPopup()
                .then(function (data) {
                    ctrl.data = data;
                    ctrl.countrySelected = ctrl.data[0];

                    for (var i = ctrl.data.length - 1; i >= 0; i--) {
                        if (ctrl.data[i].Cities.length > ctrl.minCountInColumns) {
                            ctrl.data[i].Columns = zoneService.sliceCitiesForDialog(ctrl.data[i].Cities);
                        } else {
                            ctrl.data[i].Columns = [ctrl.data[i].Cities];
                        }
                    }

                    return data;
                })
                .finally(function () {
                    ctrl.isProgress = false;
                });
        };

        ctrl.changeCity = function (city, obj, countryId, region, event) {
            if (!city.length || (event != null && event.type === 'blur')) return;
            if (!region && obj != null) region = obj.Region;
            var zip = obj != null ? obj.Zip : null;
            var country = obj != null ? obj.Country : null;
            var district = obj != null ? obj.District : null;

            zoneService.setCurrentZone(city, obj, countryId, region, country, zip, district).then(function (data) {
                if (data.ReloadPage) {
                    if (data.ReloadUrl != null) {
                        $window.location.href = $window.location.href.replace($window.location.origin, data.ReloadUrl);
                    } else {
                        $window.location.reload(true);
                    }
                }

                if (!data.Region) {
                    ctrl.showRegion = true;
                    ctrl.autocompleter.toggleVisible(false);
                } else {
                    zoneService.zoneDialogClose();
                    ctrl.zoneCity = ctrl.zoneRegion = '';
                    ctrl.showRegion = false;
                }
                $timeout(function () {
                    zoneService.processCallback(zoneEvents.changeCity, data);
                }, 0);
            });
        };

        ctrl.keyup = function ($event, val) {
            $event.stopPropagation();
            var keyCode = $event.keyCode;

            switch (keyCode) {
                case 13: //enter
                    ctrl.changeCity(ctrl.zoneCity, null, ctrl.countrySelected.CountryId, ctrl.zoneRegion);
                    break;
            }
        };

        ctrl.autocompleterOnInit = function (autocompleter) {
            ctrl.autocompleter = autocompleter;
        };
    };

    angular.module('zone').controller('ZoneCtrl', ZoneCtrl);

    ZoneCtrl.$inject = ['zoneService', '$timeout', 'zoneEvents', '$window'];
})(window.angular);
