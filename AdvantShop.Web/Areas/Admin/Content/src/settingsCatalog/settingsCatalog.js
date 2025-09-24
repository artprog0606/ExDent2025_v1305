import resultCategoryDiscountRegulationTemplate from './modal/resultCategoryDiscountRegulation/ResultCategoryDiscountRegulation.html';
import resultPriceRegulationTemplate from './modal/resultPriceRegulation/ResultPriceRegulation.html';
import addEditCurrencyTemplate from './modal/addEditCurrency/AddEditCurrency.html';
(function (ng) {
    'use strict';

    var SettingsCatalogCtrl = /* @ngInject */ function (
        $uibModal,
        $http,
        $q,
        uiGridConstants,
        uiGridCustomConfig,
        SweetAlert,
        toaster,
        $translate,
        $location,
        isMobileService,
    ) {
        var ctrl = this;
        ctrl.selectedBrandsList = [];
        ctrl.$onInit = function () {
            ctrl.brandDiscountRegulation = {
                value: 0,
            };
        };

        // #region Currencies
        ctrl.roundNumbersList = {
            '-1': $translate.instant('Admin.Js.SettingsCatalog.NotRoundOff'),
            0.01: $translate.instant('Admin.Js.SettingsCatalog.RoundToKopecks'),
            1: $translate.instant('Admin.Js.SettingsCatalog.RoundToInteger'),
            10: $translate.instant('Admin.Js.SettingsCatalog.RoundToTens'),
            100: $translate.instant('Admin.Js.SettingsCatalog.RoundUpToHundreds'),
            1000: $translate.instant('Admin.Js.SettingsCatalog.RoundUpToThousands'),
        };
        var columnDefsCurrencies = [
            {
                name: 'Name',
                displayName: $translate.instant('Admin.Js.SettingsCatalog.Name'),
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsCatalog.Name'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Name',
                },
                enableCellEdit: true,
            },
            {
                name: 'Symbol',
                displayName: $translate.instant('Admin.Js.SettingsCatalog.Symbol'),
                enableCellEdit: true,
                width: 80,
                uiGridCustomEdit: {
                    attributes: {
                        'ng-trim': false,
                    },
                },
            },
            {
                name: 'Rate',
                displayName: $translate.instant('Admin.Js.SettingsCatalog.Value'),
                type: 'number',
                enableCellEdit: true,
                width: 100,
            },
            {
                name: 'Iso3',
                displayName: $translate.instant('Admin.Js.SettingsCatalog.ISO3code'),
                enableCellEdit: true,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsCatalog.ISO3code'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Iso3',
                },
                uiGridCustomEdit: {
                    attributes: {
                        maxlength: 3,
                        minlength: 3,
                        validation_input_text: $translate.instant('Admin.Js.SettingsCatalog.ISO3code'),
                    },
                },
                width: 80,
            },
            {
                name: 'NumIso3',
                displayName: $translate.instant('Admin.Js.SettingsCatalog.ISO3numericcode'),
                type: 'number',
                enableCellEdit: true,
                uiGridCustomEdit: {
                    attributes: {
                        max: 999,
                    },
                },
                width: 80,
            },
            // {
            //     name: 'IsCodeBefore',
            //     displayName: $translate.instant('Admin.Js.SettingsCatalog.SymbolInFront'),
            //     type: 'checkbox',
            //     cellTemplate:
            //         '<div class="ui-grid-cell-contents"><label class="ui-grid-custom-edit-field adv-checkbox-label" data-e2e="switchOnOffLabel"><input type="checkbox" class="adv-checkbox-input" ng-model="MODEL_COL_FIELD " data-e2e="switchOnOffSelect" /><span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span></label></div>',
            //     enableCellEdit: true,
            //     width: 80,
            // },
            {
                name: 'RoundNumbers',
                displayName: $translate.instant('Admin.Js.SettingsCatalog.RoundingOrder'),
                enableCellEdit: true,
                width: 220,
                type: 'select',
                cellTemplate:
                    '<div class="ui-grid-cell-contents"><span ng-bind="grid.appScope.$ctrl.gridExtendCtrl.getTextForRound(COL_FIELD)"></span></div>',
                uiGridCustomEdit: {
                    customViewValue: 'roundNumbersViewValue',
                    onInit: function (rowEntity, colDef, newValue, uiGridEditCustom) {
                        uiGridEditCustom.roundNumbersViewValue = ctrl.roundNumbersList[rowEntity[colDef.name]];
                    },
                    //onActive: function (rowEntity, colDef, newValue, uiGridEditCustom) {
                    //    uiGridEditCustom.roundNumbersViewValue = ctrl.roundNumbersList[rowEntity[colDef.name]];
                    //},
                    //onDeactive: function (rowEntity, colDef, newValue, uiGridEditCustom) {
                    //    uiGridEditCustom.roundNumbersViewValue = ctrl.roundNumbersList[rowEntity[colDef.name]];
                    //},
                    onChange: function (rowEntity, colDef, newValue, uiGridEditCustom) {
                        uiGridEditCustom.roundNumbersViewValue = ctrl.roundNumbersList[newValue];
                    },
                    editDropdownOptionsArray: Object.keys(ctrl.roundNumbersList)
                        .sort(function (a, b) {
                            return parseInt(a) < parseInt(b) ? -1 : 1;
                        })
                        .map(function (roundValue) {
                            return {
                                label: ctrl.roundNumbersList[roundValue],
                                value: parseFloat(roundValue),
                            };
                        }),
                },
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 80,
                enableSorting: false,
                useInSwipeBlock: true,
                cellTemplate:
                    '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                    '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadCurrency(row.entity.CurrencyId)" aria-label="Редактировать"></button> ' +
                    '<ui-grid-custom-delete ng-if="row.entity.CanDelete" url="settingsCatalog/deleteCurrency" params="{\'Id\': row.entity.CurrencyId}"></ui-grid-custom-delete>' +
                    '</div></div>' +
                    '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="settingsCatalog/deleteCurrency" params="{\'Id\': row.entity.CurrencyId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
            },
        ];
        ctrl.getTextForRound = function (roundValue) {
            return ctrl.roundNumbersList[roundValue.toString()];
        };
        ctrl.gridCurrenciesOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsCurrencies,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadCurrency(row.entity.CurrencyId);
                },
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.SettingsCatalog.DeleteSelected'),
                        url: 'settingsCatalog/deleteItems',
                        field: 'CurrencyId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.SettingsCatalog.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.SettingsCatalog.Deleting'),
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
        });
        ctrl.gridCurrenciesOnInit = function (grid) {
            ctrl.gridCurrencies = grid;
        };
        ctrl.loadCurrency = function (id) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddEditCurrencyCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: addEditCurrencyTemplate,
                    resolve: {
                        id: function () {
                            return id;
                        },
                    },
                })
                .result.then(
                    function (result) {
                        ctrl.gridCurrencies.fetchData();
                        return result;
                    },
                    function (result) {
                        return result;
                    },
                );
        };
        ctrl.getCurrencies = function () {
            return $http.get('settingsCatalog/getCurrencies').then(function (response) {
                return (ctrl.currencies = response.data);
            });
        };
        ctrl.modalProcessResult = function () {
            if (ctrl.gridCurrencies != null) {
                ctrl.gridCurrencies.fetchData();
            }
            ctrl.updateCurrencies();
        };
        ctrl.updateCurrencies = function (rowEntity, colDef, newValue, oldValue) {
            ctrl.getCurrencies().then(function (currencies) {
                if (colDef != null && colDef.name === 'Iso3' && ctrl.DefaultCurrencyIso3.value === oldValue) {
                    ctrl.DefaultCurrencyIso3 = ctrl.findCurrency(currencies, newValue);
                }
            });
        };
        ctrl.initCurrencies = function (deafultCurrency) {
            ctrl.DefaultCurrencyIso3 = deafultCurrency;
            ctrl.getCurrencies();
        };
        ctrl.findCurrency = function (currencies, id) {
            var c = currencies.filter(function (x) {
                    return x.value === id;
                }),
                result;
            if (c != null && c.length > 0) {
                result = c[0];
            }
            return result;
        };

        // #endregion

        ctrl.updateCb = function () {
            var url = 'settingsCatalog/updateCb';
            $http
                .post(url)
                .then(function (response) {
                    var data = response.data;
                    if (data.result == true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCatalog.CurrencyRatesUpdated'));
                        ctrl.gridCurrencies.fetchData();
                    } else {
                        toaster.pop(
                            'error',
                            $translate.instant('Admin.Js.SettingsCatalog.Error'),
                            $translate.instant('Admin.Js.SettingsCatalog.ErrorUpdatingCurrencies'),
                        );
                        ctrl.btnSleep = false;
                    }
                })
                .then(function (res) {
                    if (isMobileService.getValue()) {
                        document.querySelector('.popover-backdrop').click();
                    }
                });
        };
        ctrl.isReindexStart = false;
        ctrl.reindex = function () {
            if (ctrl.isReindexStart) {
                toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCatalog.UpdatingIndexIsAlreadyRunning'));
                return;
            }
            ctrl.isReindexStart = true;
            toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCatalog.UpdatingIndexIsRunning'));
            $http.post('settingsCatalog/reindexLucene').then(function (response) {
                if (response.data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCatalog.IndexUpdateCompleted'));
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.SettingsCatalog.ErrorUpdatingIndex'));
                }
                ctrl.isReindexStart = false;
            });
        };
        ctrl.pricesSettingsInit = function (customerGroups, avalableCustomerGroups) {
            ctrl.CustomerGroups = customerGroups;
            ctrl.AvalableCustomerGroups = avalableCustomerGroups;
        };
        ctrl.updateAvalableCustomerGroups = function (avalableCustomerGroups) {
            return $http.post('settingsCatalog/updateAvalableCustomerGroups', avalableCustomerGroups).then(function (response) {
                toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCatalog.AvalableCustomerGroupsUpdate'));
            });
        };

        //#region PriceRegulation
        ctrl.priceRegulationValue = 0;
        ctrl.priceRegulationChooseProducts = false;
        ctrl.priceRegulationAction = 'Increment';
        ctrl.priceRegulationValueOption = 'Percent';
        ctrl.priceRegulationCategoryIds = [];
        ctrl.priceRegulationTreeCallbacks = {
            select_node: function (event, data) {
                var tree = ng.element(event.target).jstree(true);
                ctrl.priceRegulationCategoryIds = tree.get_selected(false);
            },
            deselect_node: function (event, data) {
                var tree = ng.element(event.target).jstree(true);
                ctrl.priceRegulationCategoryIds = tree.get_selected(false);
            },
        };
        ctrl.changePrices = function () {
            if (ctrl.priceRegulationIsRun) {
                return;
            }
            ctrl.priceRegulationIsRun = true;
            var params = {
                chooseProducts: ctrl.priceRegulationChooseProducts,
                action: ctrl.priceRegulationAction,
                valueOption: ctrl.priceRegulationValueOption,
                value: ctrl.priceRegulationValue,
                categoryIds: ctrl.priceRegulationCategoryIds,
            };
            $http.post('settingsCatalog/changePrices', params).then(function (response) {
                var data = response.data;
                $uibModal.open({
                    bindToController: true,
                    controller: 'ResultPriceRegulationCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: resultPriceRegulationTemplate,
                    resolve: {
                        msg: function () {
                            return data.message;
                        },
                        title: function () {
                            return data.result === true
                                ? $translate.instant('Admin.Js.SettingsCatalog.PriceRegulation.RegulationOfPrices')
                                : $translate.instant('Admin.Js.SettingsCatalog.PriceRegulation.Error');
                        },
                    },
                    backdrop: 'static',
                });
                ctrl.priceRegulationIsRun = false;
            });
        };
        //#endregion
        //#region CategoryDiscountRegulation
        ctrl.categoryDiscountRegulationValue = 0;
        ctrl.categoryDiscountRegulationChooseProducts = false;
        ctrl.categoryDiscountRegulationAction = 'Increment';
        ctrl.categoryDiscountRegulationValueOption = 'Percent';
        ctrl.categoryDiscountRegulationCategoryIds = [];
        ctrl.categoryDiscountRegulationTreeCallbacks = {
            select_node: function (event, data) {
                var tree = ng.element(event.target).jstree(true);
                ctrl.categoryDiscountRegulationCategoryIds = tree.get_selected(false);
            },
            deselect_node: function (event, data) {
                var tree = ng.element(event.target).jstree(true);
                ctrl.categoryDiscountRegulationCategoryIds = tree.get_selected(false);
            },
        };
        ctrl.changeDiscounts = function () {
            if (ctrl.categoryDiscountRegulationIsRun) {
                return;
            }
            ctrl.categoryDiscountRegulationIsRun = true;
            var params = {
                chooseProducts: ctrl.categoryDiscountRegulationChooseProducts,
                action: ctrl.categoryDiscountRegulationAction,
                valueOption: ctrl.categoryDiscountRegulationValueOption,
                value: ctrl.categoryDiscountRegulationValue,
                categoryIds: ctrl.categoryDiscountRegulationCategoryIds,
            };
            $http.post('settingsCatalog/ChangeDiscounts', params).then(function (response) {
                var data = response.data;
                $uibModal.open({
                    bindToController: true,
                    controller: 'ResultCategoryDiscountRegulationCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: resultCategoryDiscountRegulationTemplate,
                    resolve: {
                        msg: function () {
                            return data.message;
                        },
                        title: function () {
                            return data.result === true
                                ? $translate.instant('Admin.Js.SettingsCatalog.CategoryDiscountRegulation.RegulationOfCategoryDiscount')
                                : $translate.instant('Admin.Js.SettingsCatalog.PriceRegulation.Error');
                        },
                    },
                    backdrop: 'static',
                });
                ctrl.categoryDiscountRegulationIsRun = false;
            });
        };
        //#endregion

        //#region BrandDiscountRegulation

        ctrl.brandDiscountRegulationIsRun = false;
        ctrl.deleteBrand = function (brandId) {
            ctrl.selectedBrandsList = ctrl.selectedBrandsList.filter(function (it) {
                return it.BrandId !== brandId;
            });
        };
        ctrl.changeBrand = function (result) {
            ctrl.selectedBrandsList = result;
        };
        ctrl.brandDiscountRegulationChangeDiscounts = function () {
            if (ctrl.brandDiscountRegulationIsRun || ctrl.selectedBrandsList.length === 0) {
                return;
            }
            ctrl.brandDiscountRegulationIsRun = true;
            var brandIds = ctrl.getBrandIdsFromBrandsList(ctrl.selectedBrandsList);
            var params = {
                brandIds: brandIds,
                value: ctrl.brandDiscountRegulation.value,
                valueOption: ctrl.brandDiscountRegulation.valueOption,
            };
            $http.post('settingsCatalog/ChangeDiscountsByBrands', params).then(function (response) {
                var data = response.data;
                $uibModal.open({
                    bindToController: true,
                    controller: 'ResultCategoryDiscountRegulationCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: resultCategoryDiscountRegulationTemplate,
                    resolve: {
                        msg: function () {
                            return data.message;
                        },
                        title: function () {
                            return data.result === true
                                ? $translate.instant('Admin.Js.SettingsCatalog.BrandDiscountRegulation.RegulationOfBrandDiscount')
                                : $translate.instant('Admin.Js.SettingsCatalog.PriceRegulation.Error');
                        },
                    },
                    backdrop: 'static',
                });
                ctrl.brandDiscountRegulationIsRun = false;
            });
        };
        ctrl.getBrandIdsFromBrandsList = function (brandsList) {
            return brandsList.map(function (it) {
                return it.BrandId;
            });
        };

        //#endregion

        ctrl.backInMobile = function () {
            if (Object.keys($location.$$search)[0] || $location.$$hash) {
                let path = $location.$$absUrl;
                if ($location.$$hash) {
                    path = path.split('#%3FcatalogTab')[0];
                } else {
                    path = path.split('?catalogTab')[0];
                }
                history.pushState(null, null, path);
            }
            ctrl.floatHeader = null;
            ctrl.tabActiveIndex = null;
        };
        ctrl.onSelectTab = function (indexTab) {
            ctrl.tabActiveIndex = indexTab;
        };
        ctrl.addChildCtrl = function (name, childCtrl) {
            ctrl.childsCtrlList = ctrl.childsCtrlList || {};
            ctrl.childsCtrlList[name] = childCtrl;
        };
    };
    ng.module('settingsCatalog', [
        'colors',
        'brandsList',
        'sizes',
        'properties',
        'propertyvalues',
        'as.sortable',
        'import',
        'tags',
        'color.picker',
        'fileUploader',
        'pictureUploader',
        'brand',
        'urlGenerator',
        'isMobile',
        'priceRules',
        'units',
        'photoCategory',
        'sizeChart',
    ]).controller('SettingsCatalogCtrl', SettingsCatalogCtrl);
})(window.angular);
