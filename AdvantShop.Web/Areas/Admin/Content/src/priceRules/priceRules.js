import addEditPriceRuleTemplate from './modal/addEditPriceRule/addEditPriceRule.html';
(function (ng) {
    'use strict';

    var PriceRulesCtrl = function (uiGridConstants, uiGridCustomConfig, toaster, SweetAlert, $http, $q, $translate) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.PriceRules.Name'),
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.PriceRules.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },
                {
                    name: 'TypeStr',
                    displayName: $translate.instant('Admin.Js.PriceRules.Rule'),
                    enableCellEdit: false,
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.Colors.Order'),
                    width: 100,
                    enableCellEdit: true,
                },
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.PriceRules.Enabled'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row"></ui-grid-custom-switch>',
                    width: 100,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.PriceRules.Enabled'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'Enabled',
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.News.Yes'),
                                value: true,
                            },
                            { label: $translate.instant('Admin.Js.News.No'), value: false },
                        ],
                    },
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 80,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<ui-modal-trigger data-controller="\'ModalAddEditPriceRuleCtrl\'" controller-as="ctrl" ' +
                        'template-url="' +
                        addEditPriceRuleTemplate +
                        '" ' +
                        'data-resolve="{\'Id\': row.entity.Id}" ' +
                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                        '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></button> ' +
                        '</ui-modal-trigger>' +
                        '<ui-grid-custom-delete url="priceRules/forcedelete" params="{\'id\': row.entity.Id}" ' +
                        'confirm-text="{{!row.entity.IsUsed ? null : \'Данный тип цен используется в товарах. При удалении вы потеряете информацию о ценах с данным типом. Вы уверены что хотите удалить?\'}}"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="priceRules/forcedelete" params="{\'id\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Colors.DeleteSelected'),
                        url: 'priceRules/deletePriceRules',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.Deleting'),
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.delete = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('priceRules/delete', { Id: id }).then(function (response) {
                        ctrl.grid.fetchData();
                    });
                }
            });
        };
    };

    PriceRulesCtrl.$inject = ['uiGridConstants', 'uiGridCustomConfig', 'toaster', 'SweetAlert', '$http', '$q', '$translate'];

    ng.module('priceRules', ['uiGridCustom', 'urlHelper']).controller('PriceRulesCtrl', PriceRulesCtrl);
})(window.angular);
