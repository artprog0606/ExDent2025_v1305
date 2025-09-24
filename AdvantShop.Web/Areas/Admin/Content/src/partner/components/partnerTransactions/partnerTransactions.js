import partnerTransactionsTemplate from './partnerTransactions.html';
import transactionDetailsTemplate from '../../modals/transactionDetails/transactionDetails.html';
(function (ng) {
    'use strict';

    var PartnerTransactionsCtrl = function ($http, uiGridCustomConfig, $translate, toaster) {
        var ctrl = this;
        ctrl.gridInit = function (grid) {
            ctrl.gridPartnerTransactions = grid;
            if (ctrl.onGridInit) {
                ctrl.onGridInit({
                    grid: grid,
                });
            }
        };
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'Added',
                    displayName: 'Начислено',
                    width: 100,
                },
                {
                    name: 'Subtracted',
                    displayName: 'Списано',
                    width: 100,
                },
                {
                    name: 'BalanceFormatted',
                    displayName: 'Баланс',
                    width: 100,
                },
                {
                    name: 'DateCreatedFormatted',
                    displayName: 'Дата',
                    width: 100,
                },
                {
                    name: 'Basis',
                    displayName: 'Основание',
                },
                {
                    name: 'CustomerName',
                    displayName: 'Покупатель',
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"> ' +
                        '<div class="m-l-xs"><a ng-if="row.entity.CustomerId != null" href="customers/view/{{row.entity.CustomerId}}" target="_blank">{{row.entity.CustomerName}}</a></div> ' +
                        '</div>',
                },
                {
                    name: 'OrderNumber',
                    displayName: 'Заказ',
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"> ' +
                        '<div class="m-l-xs"><a ng-if="row.entity.OrderId != null" href="orders/edit/{{row.entity.OrderId}}" target="_blank">{{row.entity.OrderNumber}}</a></div> ' +
                        '</div>',
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                        '<ui-modal-trigger ng-if="row.entity.OrderId != null" class="dropdown-menu-link js-menu-link" ' +
                        'controller="grid.appScope.$ctrl.gridExtendCtrl.modalTransactionDetails" ' +
                        'data-resolve="{params: {data: row.entity}}" ' +
                        'template-url="' +
                        transactionDetailsTemplate +
                        '">' +
                        '<a href="" title="Подробнее" class="ui-grid-custom-service-icon fa fa-eye link-invert"></a>' +
                        '</ui-modal-trigger>' +
                        '</div></div>',
                },
            ],
        });
        ctrl.modalTransactionDetails = function () {
            var detailsCtrl = this;
            detailsCtrl.$onInit = function () {
                detailsCtrl.data = detailsCtrl.$resolve.params.data;
            };
        };
    };
    PartnerTransactionsCtrl.$inject = ['$http', 'uiGridCustomConfig', '$translate', 'toaster'];
    ng.module('partnerTransactions', ['uiGridCustom'])
        .controller('PartnerTransactionsCtrl', PartnerTransactionsCtrl)
        .component('partnerTransactions', {
            templateUrl: partnerTransactionsTemplate,
            controller: PartnerTransactionsCtrl,
            bindings: {
                partnerId: '<?',
                onGridInit: '&',
            },
        });
})(window.angular);
