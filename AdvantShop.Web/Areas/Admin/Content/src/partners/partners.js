(function (ng) {
    'use strict';

    var PartnersCtrl = function (uiGridConstants, uiGridCustomConfig, $q, SweetAlert, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.checkPartnerCouponTpl();
        };

        var columnDefs = [
            {
                name: 'Name',
                displayName: $translate.instant('Admin.Js.Partners.FullName'),
                cellTemplate: '<div class="ui-grid-cell-contents"><a ng-href="partners/view/{{row.entity.Id}}">{{row.entity.Name}}</a></div>',
                filter: {
                    placeholder: $translate.instant('Admin.Js.Partners.FullName'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Name',
                },
            },
            {
                name: 'Phone',
                displayName: $translate.instant('Admin.Js.Partners.Phone'),
                filter: {
                    placeholder: $translate.instant('Admin.Js.Partners.Phone'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Phone',
                },
            },
            {
                name: 'Email',
                displayName: $translate.instant('Admin.Js.Partners.Email'),
                filter: {
                    placeholder: $translate.instant('Admin.Js.Partners.Email'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Email',
                },
            },
            {
                name: 'TypeFormatted',
                displayName: $translate.instant('Admin.Js.Partners.Type'),
                enableSorting: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Partners.Type'),
                    type: uiGridConstants.filter.SELECT,
                    name: 'Type',
                    fetch: 'partners/getTypes',
                },
            },
            {
                name: 'BalanceFormatted',
                displayName: $translate.instant('Admin.Js.Partners.Balance'),
                type: 'number',
                width: 100,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Partners.Balance'),
                    type: 'range',
                    rangeOptions: {
                        from: { name: 'BalanceFrom' },
                        to: { name: 'BalanceTo' },
                    },
                },
            },
            {
                name: 'DateCreatedFormatted',
                displayName: $translate.instant('Admin.Js.Partners.DateCreated'),
                width: 150,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Partners.DateCreated'),
                    type: 'datetime',
                    term: {
                        from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                        to: new Date(),
                    },
                    datetimeOptions: {
                        from: { name: 'DateCreatedFrom' },
                        to: { name: 'DateCreatedTo' },
                    },
                },
            },
            {
                name: 'Enabled',
                displayName: $translate.instant('Admin.Js.Partners.Enabled'),
                cellTemplate:
                    '<div class="ui-grid-cell-contents">' +
                    '<label class="ui-grid-custom-edit-field adv-checkbox-label"> ' +
                    '<input type="checkbox" class="adv-checkbox-input" data-e2e="switchOnOffInput" ng-model="row.entity.Enabled" disabled /> ' +
                    '<span class="adv-checkbox-emul" data-e2e="switchOnOffSelect"></span> ' +
                    '</label>' +
                    '</div>',
                width: 76,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Partners.Activity'),
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: [
                        { label: $translate.instant('Admin.Js.Partners.Active'), value: true },
                        { label: $translate.instant('Admin.Js.Partners.NotActive'), value: false },
                    ],
                },
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 80,
                enableSorting: false,
                useInSwipeBlock: true,
                cellTemplate:
                    '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                    '<partner-info-trigger partner-id="row.entity.Id" on-close="grid.appScope.$ctrl.fetchData()"><button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></button></partner-info-trigger>' +
                    '<ui-grid-custom-delete url="partners/deletePartner" params="{\'Id\': row.entity.Id}"></ui-grid-custom-delete>' +
                    '</div></div>' +
                    '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="partners/deletePartner" params="{\'Id\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
            },
        ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowUrl: 'partners/view/{{row.entity.Id}}',
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Partners.DeleteSelected'),
                        url: 'partners/deletePartners',
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

        ctrl.checkPartnerCouponTpl = function () {
            return $http.post('partners/checkPartnerCouponTpl').then(function (response) {
                ctrl.couponTplExists = response.data.result;
            });
        };
    };

    PartnersCtrl.$inject = ['uiGridConstants', 'uiGridCustomConfig', '$q', 'SweetAlert', '$http', 'toaster', '$translate'];

    ng.module('partners', [
        'uiGridCustom',
        'urlHelper',
        'customer',
        'customerInfo',
        'customers',
        'settingsCustomers',
        'as.sortable',
        'vkAuth',
        'customergroups',
        'subscription',
        'ngFileUpload',
        'fileUploader',
    ]).controller('PartnersCtrl', PartnersCtrl);
})(window.angular);
