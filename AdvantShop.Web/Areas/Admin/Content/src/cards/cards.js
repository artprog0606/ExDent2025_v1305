(function (ng) {
    'use strict';

    var CardsCtrl = function (
        $location,
        $window,
        uiGridConstants,
        uiGridCustomConfig,
        uiGridCustomParamsConfig,
        uiGridCustomService,
        toaster,
        $http,
        $q,
        SweetAlert,
        customerFieldsService,
        $translate,
    ) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'CardNumber',
                    displayName: $translate.instant('Admin.Js.Cards.CardNumber'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><a ng-href="cards/edit/{{row.entity.CardId}}">{{COL_FIELD}}</a></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Cards.CardNumber'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'CardNumber',
                    },
                },
                {
                    name: 'FIO',
                    displayName: $translate.instant('Admin.Js.Cards.FullName'),
                },
                {
                    name: 'GradeName',
                    displayName: $translate.instant('Admin.Js.Cards.Grade'),
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Cards.Grade'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'GradeId',
                        fetch: 'grades/getGradesSelectItems',
                    },
                },
                {
                    name: 'GradePersent',
                    displayName: $translate.instant('Admin.Js.Cards.BonusPercent'),
                },
                {
                    name: 'CreatedFormatted',
                    displayName: $translate.instant('Admin.Js.Cards.CardIssueDate'),
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Cards.CardIssueDate'),
                        type: 'datetime',
                        term: {
                            from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                            to: new Date(),
                        },
                        datetimeOptions: {
                            from: { name: 'CreatedFrom' },
                            to: { name: 'CreatedTo' },
                        },
                    },
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<a href="cards/edit/{{row.entity.CardId}}" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></a> ' +
                        '<ui-grid-custom-delete url="cards/deleteCard" params="{\'cardId\': row.entity.CardId}"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="cards/deleteCard" params="{\'cardId\': row.entity.CardId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];

        // more filters
        columnDefs.push(
            {
                name: '_noopColumnBonusAmount',
                visible: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Cards.QuantityBonuses'),
                    type: 'range',
                    rangeOptions: {
                        from: { name: 'BonusAmountFrom' },
                        to: { name: 'BonusAmountTo' },
                    },
                },
            },
            {
                name: '_noopColumnFIO',
                visible: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Cards.FullName'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'FIO',
                },
            },
            {
                name: '_noopColumnEmail',
                visible: false,
                filter: {
                    placeholder: 'E-mail',
                    type: uiGridConstants.filter.INPUT,
                    name: 'Email',
                },
            },
            {
                name: '_noopColumnMobilePhone',
                visible: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Cards.MobilePhone'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'MobilePhone',
                },
            },
            {
                name: '_noopColumnRegDate',
                visible: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Cards.DateOfRegistration'),
                    type: 'datetime',
                    term: {
                        from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                        to: new Date(),
                    },
                    datetimeOptions: {
                        from: { name: 'RegDateFrom' },
                        to: { name: 'RegDateTo' },
                    },
                },
            },
            {
                name: '_noopColumnLocation',
                visible: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Cards.Location'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Location',
                },
            },
            {
                name: '_noopColumnOrdersCount',
                visible: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Cards.QuantityPaidOrders'),
                    type: 'range',
                    rangeOptions: {
                        from: { name: 'OrdersCountFrom' },
                        to: { name: 'OrdersCountTo' },
                    },
                },
            },
            {
                name: '_noopColumnOrdersSum',
                visible: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Cards.AmountOfOrders'),
                    type: 'range',
                    rangeOptions: {
                        from: { name: 'OrderSumFrom' },
                        to: { name: 'OrderSumTo' },
                    },
                },
            },
            {
                name: '_noopColumnCustomerGroup',
                visible: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Cards.CustomerGroup'),
                    type: uiGridConstants.filter.SELECT,
                    name: 'CustomerGroupId',
                    fetch: 'cards/getCustomerGroupsSelectItems',
                },
            },
        );
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowUrl: 'cards/edit/{{row.entity.CardId}}',
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Cards.DeleteSelected'),
                        url: 'cards/deleteCards',
                        field: 'CardId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Cards.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.Cards.Deleting'),
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
        });

        ctrl.initCard = function (cardId, isEditMode, cardnumber) {
            ctrl.cardId = cardId;
            ctrl.customerId = cardId;
            ctrl.isEditMode = isEditMode;
            ctrl.cardNumber = cardnumber;
        };

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.gridOnFilterInit = function (filter) {
            ctrl.gridFilter = filter;
            customerFieldsService.getFilterColumns().then(function (columns) {
                Array.prototype.push.apply(ctrl.gridOptions.columnDefs, columns);
                ctrl.gridFilter.updateColumns();
            });
        };

        ctrl.deleteCard = function (cardId) {
            SweetAlert.confirm($translate.instant('Admin.Js.Cards.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Cards.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('cards/deleteCard', { cardId: cardId }).then(function (response) {
                        var data = response.data;

                        if (data.result === true || result.value) {
                            $window.location.assign('cards');
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.pop('error', '', error);
                            });
                        }
                    });
                }
            });
        };

        ctrl.startExport = function () {
            $window.location.assign('cards/ExportCards');
        };

        ctrl.gridUpdate = function () {
            ctrl.grid.fetchData();
        };
    };

    CardsCtrl.$inject = [
        '$location',
        '$window',
        'uiGridConstants',
        'uiGridCustomConfig',
        'uiGridCustomParamsConfig',
        'uiGridCustomService',
        'toaster',
        '$http',
        '$q',
        'SweetAlert',
        'customerFieldsService',
        '$translate',
    ];

    ng.module('cards', ['uiGridCustom', 'urlHelper', 'settingsCustomers']).controller('CardsCtrl', CardsCtrl);
})(window.angular);
