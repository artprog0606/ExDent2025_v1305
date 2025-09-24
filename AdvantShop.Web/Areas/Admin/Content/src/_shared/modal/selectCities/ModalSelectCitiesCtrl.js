import './selectCities.html';

/* @ngInject */
const ModalSelectCitiesCtrl = function ($uibModalInstance, $http, uiGridConstants, uiGridCustomConfig, $translate, $location, $q) {
    const ctrl = this;

    ctrl.$onInit = function () {
        ctrl.showGrid = false;

        let params = ctrl.$resolve.params;

        $location.search('selectCity', null);

        ctrl.countryId = params.countryId;
        ctrl.regionId = params.regionId;

        return $q.when(ctrl.countryId == null && ctrl.regionId == null ? ctrl.getDefaultGeoData() : $q.resolve()).then(function () {
            ctrl.gridParams = { CountryId: ctrl.countryId, RegionId: ctrl.regionId };
            ctrl.showGrid = true;
        });
    };

    ctrl.getDefaultGeoData = function () {
        return $http.get('cities/getDefaultGeoData').then(function (response) {
            let data = response.data;
            ctrl.countryId = data.CountryId;
            ctrl.regionId = data.RegionId;
        });
    };

    ctrl.gridOptions = angular.extend({}, uiGridCustomConfig, {
        enableFullRowSelection: true,
        columnDefs: [
            {
                name: 'CityName',
                displayName: $translate.instant('Admin.Js.SettingsSystem.City'),
                enableCellEdit: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsSystem.City'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'CityName',
                },
            },
            {
                visible: false,
                name: 'CountryId',
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsSystem.Country'),
                    type: uiGridConstants.filter.SELECT,
                    name: 'CountryId',
                    fetch: 'countries/getCountriesList',
                    dynamicSearch: false,
                },
            },
            {
                name: 'RegionName',
                displayName: $translate.instant('Admin.Js.SettingsSystem.Region'),
                enableCellEdit: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsSystem.Region'),
                    type: uiGridConstants.filter.SELECT,
                    name: 'RegionId',
                    fetch: 'regions/getRegionsList',
                    dynamicSearch: false,
                },
            },
        ],
    });

    ctrl.gridOnInit = function (grid) {
        ctrl.grid = grid;
    };

    ctrl.close = function () {
        $uibModalInstance.dismiss('cancel');
    };

    ctrl.select = function () {
        let params = ctrl.selectionCustom.getSelectedParams('CityId');

        if (params.selectMode === 'all') {
            $http.get('cities/getCitiesInSelectCitiesModal', { params }).then(function (response) {
                $uibModalInstance.close(response.data.DataItems.length > 0 ? { cities: ctrl.prepareResult(response.data.DataItems) } : null);
            });
        } else {
            $uibModalInstance.close({
                cities: ctrl.prepareResult(ctrl.selectionCustom.getRowsFromStorage()),
            });
        }
    };

    ctrl.gridSelectionOnInit = function (selectionCustom) {
        ctrl.selectionCustom = selectionCustom;
    };

    ctrl.prepareResult = (data) => {
        return data.map((item) => {
            return { CityId: item.CityId, CityName: item.CityName };
        });
    };
};

angular.module('uiModal').controller('ModalSelectCitiesCtrl', ModalSelectCitiesCtrl);
