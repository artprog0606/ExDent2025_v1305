(function (ng) {
    'use strict';

    var SettingsSystemLocationCtrl = function ($location, $q, $http) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.enumLocation = {
                country: 'country',
                region: 'region',
                city: 'city',
            };

            ctrl.shower = {};
            ctrl.shower[ctrl.enumLocation.country] = ctrl.showCountry;
            ctrl.shower[ctrl.enumLocation.region] = ctrl.showGridRegion;
            ctrl.shower[ctrl.enumLocation.city] = ctrl.showGridCity;

            ctrl.locationGrids = {};

            ctrl.locationGridsParams = {};
            ctrl.locationGridsParams[ctrl.enumLocation.country] = {};
            ctrl.locationGridsParams[ctrl.enumLocation.region] = {};
            ctrl.locationGridsParams[ctrl.enumLocation.city] = {};

            ctrl.lastParamsData = {};

            var urlSearch = $location.search();

            if (urlSearch != null && urlSearch.locationType != null && ctrl.enumLocation[urlSearch.locationType] != null) {
                ctrl.setLocationType(urlSearch.locationType, true);
            }

            if (ctrl.onInit != null) {
                ctrl.onInit({ ctrl: ctrl });
            }

            const locationParams = $location.search();
            if (!ctrl.countryEntity && locationParams.gridRegion != null) {
                const { id } = locationParams.gridRegion != null ? JSON.parse(locationParams.gridRegion) : null;
                if (id != null) {
                    ctrl.getCountryById(id);
                }
            }

            if (!ctrl.regionEntity && locationParams.gridCity?.id != null) {
                const { id } = locationParams.gridCity != null ? JSON.parse(locationParams.gridCity) : null;
                if (id != null) {
                    ctrl.getRegionById(id);
                }
            }
        };

        ctrl.getCountryById = function (id) {
            $http.get('countries/getcountryitem', { params: { countryId: id } }).then(function (response) {
                var data = response.data;

                if (data.result === true) {
                    ctrl.countryEntity = data.obj;
                    Object.assign(ctrl.locationGridsParams[ctrl.enumLocation.region], ctrl.countryEntity);
                }
            });
        };

        ctrl.getRegionById = function (id) {
            $http.get('regions/getregionitem', { params: { RegionID: id } }).then(function (response) {
                var data = response.data;

                if (data) {
                    ctrl.regionEntity = data;
                    Object.assign(ctrl.locationGridsParams[ctrl.enumLocation.region], ctrl.regionEntity);
                }
            });
        };

        ctrl.showCountry = function () {
            ctrl.locationGridsParams[ctrl.enumLocation.country].search = null;
        };

        ctrl.selectCountry = function (id, name, entity) {
            ctrl.lastParamsData[ctrl.enumLocation.region] = {
                id: id,
                name: name,
            };

            ctrl.countryEntity = entity;

            ctrl.showGridRegion(id, name);
        };

        ctrl.showGridRegion = function (id, name) {
            ctrl.setLocationType(ctrl.enumLocation.region);

            ctrl.locationGridsParams[ctrl.enumLocation.region].id = id || ctrl.countryIdFromUrl;
            ctrl.locationGridsParams[ctrl.enumLocation.region].countryName = name || ctrl.countryNameFromUrl;
            ctrl.locationGridsParams[ctrl.enumLocation.region].search = null;
        };

        ctrl.selectRegion = function (id, name) {
            ctrl.lastParamsData[ctrl.enumLocation.city] = {
                id: id,
                name: name,
            };

            ctrl.getRegionById(id);

            ctrl.showGridCity(id, name);
        };

        ctrl.showGridCity = function (id, name) {
            ctrl.setLocationType(ctrl.enumLocation.city);

            ctrl.locationGridsParams[ctrl.enumLocation.city].id = id;
            ctrl.locationGridsParams[ctrl.enumLocation.city].regionName = name;
            ctrl.locationGridsParams[ctrl.enumLocation.city].search = null;
            ctrl.locationGridsParams[ctrl.enumLocation.city].cityCountrys = null;
        };

        ctrl.revertSubject = function (type) {
            var lastParams = ctrl.lastParamsData[type],
                countryId;

            if (
                type === ctrl.enumLocation.region &&
                (lastParams == null || lastParams.id == null || lastParams.id.length === 0) &&
                ctrl.locationGridsParams[ctrl.enumLocation.city].cityCountrys
            ) {
                countryId = ctrl.locationGridsParams[ctrl.enumLocation.city].cityCountrys;
            }

            if (lastParams == null && countryId == null) {
                type = ctrl.enumLocation.country;
            }

            ctrl.setLocationType(type);
            ctrl.shower[type](lastParams != null ? lastParams.id : countryId, lastParams != null ? lastParams.name : null);
        };

        ctrl.viewAllCity = function () {
            ctrl.setLocationType(ctrl.enumLocation.city);

            ctrl.shower[ctrl.enumLocation.city](null, null);

            ctrl.locationGridsParams[ctrl.enumLocation.city].cityCountrys =
                ctrl.lastParamsData[ctrl.enumLocation.region] != null ? ctrl.lastParamsData[ctrl.enumLocation.region].id : ctrl.countryIdFromUrl;
        };

        ctrl.setLocationType = function (type, skipChangeUrl) {
            ctrl.locationTypeSelected = type;

            if (!skipChangeUrl) {
                $location.search('locationType', type);
            }

            //очищаем адресную строку от параметров
            type !== ctrl.enumLocation.country &&
                ctrl.locationGrids[ctrl.enumLocation.country] &&
                $location.search(ctrl.locationGrids[ctrl.enumLocation.country].gridUniqueId, null);
            type !== ctrl.enumLocation.region &&
                ctrl.locationGrids[ctrl.enumLocation.region] &&
                $location.search(ctrl.locationGrids[ctrl.enumLocation.region].gridUniqueId, null);
            type !== ctrl.enumLocation.city &&
                ctrl.locationGrids[ctrl.enumLocation.city] &&
                $location.search(ctrl.locationGrids[ctrl.enumLocation.city].gridUniqueId, null);
        };

        ctrl.gridCountryOnInit = function (grid) {
            ctrl.countryGrid = grid;
            ctrl.locationGrids[ctrl.enumLocation.country] = grid;
            ctrl.locationGridsParams[ctrl.enumLocation.country] = grid._params;
        };

        ctrl.gridRegionOnInit = function (grid) {
            ctrl.regionGrid = grid;
            ctrl.locationGrids[ctrl.enumLocation.region] = grid;
            ctrl.locationGridsParams[ctrl.enumLocation.region] = grid._params;
            grid.setParams();
        };

        ctrl.updateCountry = function (result) {
            Object.assign(ctrl.locationGridsParams[ctrl.enumLocation.region], result);
            ctrl.countryEntity = result;
            ctrl.locationGridsParams[ctrl.enumLocation.region].countryName = result.Name;
        };

        ctrl.updateRegion = function (result) {
            Object.assign(ctrl.locationGridsParams[ctrl.enumLocation.city], result);
            ctrl.regionEntity = result;
            ctrl.locationGridsParams[ctrl.enumLocation.city].regionName = ctrl.regionEntity.Name;
        };

        ctrl.gridCityOnPreinit = function (grid) {
            var defer = $q.defer();

            ctrl.locationGrids[ctrl.enumLocation.city] = grid;
            ctrl.locationGridsParams[ctrl.enumLocation.city] = grid._params;

            if (
                ctrl.locationGridsParams[ctrl.enumLocation.city].regionName == null &&
                ctrl.locationGridsParams[ctrl.enumLocation.city].cityCountrys != null
            ) {
                $http
                    .get('countries/getcountryitem', {
                        params: { countryId: ctrl.locationGridsParams[ctrl.enumLocation.city].cityCountrys },
                    })
                    .then(function (response) {
                        var data = response.data;

                        if (data.result === true) {
                            ctrl.countryIdFromUrl = data.obj.CountryId;
                            ctrl.countryNameFromUrl = data.obj.Name;
                        }

                        grid.setParams();

                        defer.resolve();
                    });
            } else {
                grid.setParams();
                defer.resolve();
            }

            return defer.promise;
        };

        ctrl.back = function (event) {
            switch (ctrl.locationTypeSelected) {
                case 'region':
                    event.preventDefault();
                    ctrl.revertSubject(ctrl.enumLocation.country);
                    return false;
                case 'city':
                    event.preventDefault();
                    ctrl.revertSubject(ctrl.enumLocation.region);
                    return false;
                default:
                    break;
            }
        };
    };

    SettingsSystemLocationCtrl.$inject = ['$location', '$q', '$http'];

    ng.module('settingsSystem').controller('SettingsSystemLocationCtrl', SettingsSystemLocationCtrl);
})(window.angular);
