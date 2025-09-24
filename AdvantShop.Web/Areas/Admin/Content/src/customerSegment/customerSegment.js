(function (ng) {
    'use strict';

    var CustomerSegmentCtrl = function ($http, $window, SweetAlert, uiGridCustomConfig, $translate, $scope, $timeout, $q) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.CustomerSegments.Customer'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                        '<a ng-href="customers/view/{{row.entity.CustomerId}}">' +
                        '{{row.entity.Organization != null && row.entity.Organization.length > 0 ? row.entity.Organization : row.entity.Name }}' +
                        '</a>' +
                        '</div>',
                },
                {
                    name: 'Phone',
                    displayName: $translate.instant('Admin.Js.CustomerSegment.Phone'),
                },
                {
                    name: 'Email',
                    displayName: 'Email',
                },
                {
                    name: 'OrdersCount',
                    displayName: $translate.instant('Admin.Js.CustomerSegment.NumberOfPaidOrders'),
                    width: 150,
                    type: 'number',
                },
                {
                    name: 'RegistrationDateTimeFormatted',
                    displayName: $translate.instant('Admin.Js.CustomerSegment.DateOfRegistration'),
                    width: 150,
                },
            ],
            timerProcessCompanyName;

        ctrl.$onInit = function () {
            ctrl.$scope = $scope;
        };

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowUrl: 'customers/view/{{row.entity.CustomerId}}',
            },
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.deleteSegment = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.CustomerSegments.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.CustomerSegments.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('customerSegments/deleteSegment', { id: id }).then(function (response) {
                        $window.location.assign('customers?customersTab=segments');
                    });
                }
            });
        };

        ctrl.initCategories = function (selectedCategories) {
            ctrl.selectedCategories = selectedCategories === null || selectedCategories == '' ? null : JSON.parse(selectedCategories);
            ctrl.getCategories();
        };

        ctrl.getCategories = function () {
            $http.get('customerSegments/getCategories').then(function (response) {
                ctrl.Categories = response.data.categories;
            });
        };

        ctrl.initCities = function (selectedCities) {
            ctrl.selectedCities = selectedCities === null || selectedCities == '' ? null : JSON.parse(selectedCities);
            ctrl.getCities();
        };

        ctrl.getCities = function () {
            $http.get('customerSegments/getCities').then(function (response) {
                ctrl.Cities = response.data.cities;
            });
        };

        ctrl.initCountries = function (selectedCountries) {
            ctrl.selectedCountries = selectedCountries === null || selectedCountries == '' ? null : JSON.parse(selectedCountries);
            ctrl.getCountries();
        };

        ctrl.getCountries = function () {
            $http.get('customerSegments/getCountries').then(function (response) {
                ctrl.Countries = response.data.countries;
            });
        };

        ctrl.export = function () {
            ctrl.grid.export();
        };

        ctrl.getCustomerIds = function (segmentId) {
            $http.get('customerSegments/getCustomerIdsBySegment', { params: { id: segmentId, itemsPerPage: 1000000 } }).then(function (response) {
                var data = response.data;
                if (data != null && data.DataItems != null) {
                    ctrl.customerIds = data.DataItems.map(function (x) {
                        return x.CustomerId;
                    });
                }
            });
        };

        ctrl.sendSmsNotEnabled = function () {
            SweetAlert.confirm(
                $translate.instant('Admin.Js.CustomerSegment.SmsModuleIsNotConnected') +
                    '<br/>' +
                    $translate.instant('Admin.Js.CustomerSegments.YouCan') +
                    '<a href="modules/market" target="_blank">' +
                    $translate.instant('Admin.Js.CustomerSegment.ConnectTheModule') +
                    '</a>' +
                    $translate.instant('Admin.Js.CustomerSegment.SmsInforming'),
                { title: '' },
            ).then(function (result) {});
        };

        ctrl.getCustomerFields = function () {
            return ctrl.getFunctionGetCustomerFields().then(function () {
                return ctrl.getCustomerFieldsFn();
            });
        };

        ctrl.getFunctionGetCustomerFields = function () {
            if (ctrl.getCustomerFieldsFn) {
                return $q.resolve();
            } else {
                ctrl.functionGetCustomerFieldsPromise = ctrl.functionGetCustomerFieldsPromise ? ctrl.functionGetCustomerFieldsPromise : $q.defer();
                return ctrl.functionGetCustomerFieldsPromise.promise;
            }
        };

        ctrl.onCustomerFieldsInit = function (reloadFn) {
            ctrl.getCustomerFieldsFn = reloadFn || function () {};
            if (ctrl.functionGetCustomerFieldsPromise) {
                ctrl.functionGetCustomerFieldsPromise.resolve();
            }
        };

        ctrl.initCustomerFields = function (customerFields) {
            return (ctrl.customerFields = !customerFields ? [] : customerFields);
        };

        ctrl.processCompanyName = function (item) {
            if (timerProcessCompanyName != null) {
                $timeout.cancel(timerProcessCompanyName);
            }

            return (timerProcessCompanyName = $timeout(
                function () {
                    if (item != null && item.CompanyData) {
                        ctrl.customerfieldsJs.forEach(function (field) {
                            if (field.FieldAssignment == 1) field.Value = item.CompanyData.CompanyName;
                            else if (field.FieldAssignment == 2) field.Value = item.CompanyData.LegalAddress;
                            else if (field.FieldAssignment == 3) field.Value = item.CompanyData.INN;
                            else if (field.FieldAssignment == 4) field.Value = item.CompanyData.KPP;
                            else if (field.FieldAssignment == 5) field.Value = item.CompanyData.OGRN;
                            else if (field.FieldAssignment == 6) field.Value = item.CompanyData.OKPO;
                        });
                    } else if (item != null && item.BankData) {
                        ctrl.customerfieldsJs.forEach(function (field) {
                            if (field.FieldAssignment == 7) field.Value = item.BankData.BIK;
                            else if (field.FieldAssignment == 8) field.Value = item.BankData.BankName;
                            else if (field.FieldAssignment == 9) field.Value = item.BankData.CorrespondentAccount;
                        });
                    }
                },
                item != null ? 0 : 700,
            ));
        };
    };

    CustomerSegmentCtrl.$inject = ['$http', '$window', 'SweetAlert', 'uiGridCustomConfig', '$translate', '$scope', '$timeout', '$q'];

    ng.module('customerSegment', ['uiGridCustom']).controller('CustomerSegmentCtrl', CustomerSegmentCtrl);
})(window.angular);
