import './selectCity.html';

(function (ng) {
    'use strict';

    var ModalSelectCityCtrl = function ($uibModalInstance, $http, uiGridConstants, uiGridCustomConfig, $translate, $location, $q) {
        const ctrl = this;

        ctrl.showGrid = false;

        ctrl.$onInit = function () {
            let params = ctrl.$resolve.params;

            $location.search('selectCity', null);

            ctrl.countryId = params.countryId;
            ctrl.regionId = params.regionId;

            let defer = $q.defer(),
                promise;

            if (ctrl.countryId == null && ctrl.regionId == null) {
                promise = ctrl.getDefaultGeoData();
            } else {
                promise = defer.promise;
                defer.resolve();
            }

            return promise.then(function () {
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

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            multiSelect: false,
            modifierKeysToMultiSelect: false,
            enableRowSelection: true,
            enableRowHeaderSelection: false,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.selectCity(row.entity);
                },
            },
            columnDefs: [
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 80,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                        '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.selectCity(row.entity)">' +
                        $translate.instant('Admin.Js.SelectCustomer.Select') +
                        '</a>' +
                        '</div></div>',
                },
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

        ctrl.selectCity = function (city) {
            $uibModalInstance.close({ city: city });
        };

        ctrl.gridOnInit = function (grid) {};

        ctrl.onGridPreinit = function (grid) {};

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalSelectCityCtrl.$inject = ['$uibModalInstance', '$http', 'uiGridConstants', 'uiGridCustomConfig', '$translate', '$location', '$q'];

    ng.module('uiModal').controller('ModalSelectCityCtrl', ModalSelectCityCtrl);
})(window.angular);
