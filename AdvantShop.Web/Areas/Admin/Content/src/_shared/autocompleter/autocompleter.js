import autocompleteAddressTemplate from './templates/autocompleteAddress.html';
import autocompleteVkMarketCategoriesTemplate from './templates/autocompleteVkMarketCategories.html';
import './templates/location-admin.html';
import './templates/vkMarketCategory.html';

(function (ng) {
    'use strict';

    var AutocompleterCustomCtrl = function ($http, $scope, autocompleterUrls) {
        var ctrl = this;
        ctrl.find = function (viewValue) {
            if (viewValue == null || viewValue.length < ctrl.minLengthFind) return;
            var url = ctrl.autocompleterUrl || autocompleterUrls[ctrl.onType];
            if (url == null) {
                throw Error('Not find url by onType for autocompleter');
            }
            var params = ng.extend($scope.$eval(ctrl.params) || {}, {
                q: viewValue,
            });
            return $http
                .get(url, {
                    params: params,
                })
                .then(function (response) {
                    return response.data;
                });
        };
        ctrl.typeaheadOnSelect = function (item) {
            if (ctrl.onSelect != null) {
                ctrl.onSelect($scope, {
                    item: item,
                });
            }
        };
    };
    AutocompleterCustomCtrl.$inject = ['$http', '$scope', 'autocompleterUrls'];
    ng.module('autocompleter', [])
        .constant('autocompleterUrls', {
            country: 'countries/getCountriesAutocomplete',
            region: 'regions/getRegionsAutocomplete',
            city: 'cities/getCitiesAutocomplete',
        })
        .controller('AutocompleterCustomCtrl', AutocompleterCustomCtrl)
        .directive('autocompleter', function () {
            return {
                require: ['autocompleter', 'ngModel'],
                template:
                    '<input autocomplete="new-password" uib-typeahead="item for item in autocompleter.find(autocompleter.ngModel.$viewValue)" typeahead-focus-first="false">',
                replace: true,
                controller: 'AutocompleterCustomCtrl',
                controllerAs: 'autocompleter',
                bindToController: true,
                //scope: {
                //    onType: '@',
                //    minLengthFind: '<?'
                //},
                scope: true,
                link: function (scope, element, attrs, ctrls) {
                    var autocompleterCtrl = ctrls[0],
                        ngModelCtrl = ctrls[1];
                    autocompleterCtrl.minLengthFind = 1;
                    autocompleterCtrl.onType = attrs.onType;
                    autocompleterCtrl.ngModel = ngModelCtrl;
                },
            };
        })
        .directive('autocompleterAddress', [
            '$parse',
            function ($parse) {
                return {
                    require: ['autocompleterAddress', 'ngModel'],
                    templateUrl: autocompleteAddressTemplate,
                    replace: true,
                    controller: 'AutocompleterCustomCtrl',
                    controllerAs: 'autocompleter',
                    bindToController: true,
                    scope: true,
                    link: function (scope, element, attrs, ctrls) {
                        if (attrs.autocompleterAddress == 'false') {
                            return;
                        }
                        var autocompleterCtrl = ctrls[0],
                            ngModelCtrl = ctrls[1];
                        autocompleterCtrl.ngModel = ngModelCtrl;
                        autocompleterCtrl.minLengthFind = 1;
                        autocompleterCtrl.autocompleterUrl = attrs.autocompleterUrl || 'cities/GetCitiesSuggestions';
                        autocompleterCtrl.onSelect = attrs.onSelect != null ? $parse(attrs.onSelect) : null;
                        autocompleterCtrl.params = attrs.autocompleterParams != null ? $parse(attrs.autocompleterParams) : null;
                    },
                };
            },
        ])
        .directive('autocompleterSuggest', function () {
            return {
                require: ['autocompleterSuggest', 'ngModel', 'uibTypeahead'],
                controller: 'AutocompleterCustomCtrl',
                controllerAs: 'autocompleter',
                bindToController: true,
                scope: true,
                link: function (scope, element, attrs, ctrls) {
                    if (attrs.autocompleterUrl == null) {
                        console.error('missing "autocompleterUrl" attribute');
                        return;
                    }
                    var autocompleterCtrl = ctrls[0],
                        ngModelCtrl = ctrls[1];
                    autocompleterCtrl.ngModel = ngModelCtrl;
                    autocompleterCtrl.minLengthFind = 1;
                    autocompleterCtrl.autocompleterUrl = attrs.autocompleterUrl;
                },
            };
        })
        .directive('autocompleterVkMarketCategories', [
            '$parse',
            function ($parse) {
                return {
                    require: ['autocompleterVkMarketCategories', 'ngModel'],
                    templateUrl: autocompleteVkMarketCategoriesTemplate,
                    replace: true,
                    controller: 'AutocompleterCustomCtrl',
                    controllerAs: 'autocompleter',
                    bindToController: true,
                    scope: true,
                    link: function (scope, element, attrs, ctrls) {
                        var autocompleterCtrl = ctrls[0],
                            ngModelCtrl = ctrls[1];
                        autocompleterCtrl.ngModel = ngModelCtrl;
                        autocompleterCtrl.minLengthFind = 3;
                        autocompleterCtrl.autocompleterUrl = attrs.autocompleterUrl;
                        autocompleterCtrl.onSelect = attrs.onSelect != null ? $parse(attrs.onSelect) : null;
                        autocompleterCtrl.params = attrs.autocompleterParams != null ? $parse(attrs.autocompleterParams) : null;
                    },
                };
            },
        ]);
})(window.angular);
