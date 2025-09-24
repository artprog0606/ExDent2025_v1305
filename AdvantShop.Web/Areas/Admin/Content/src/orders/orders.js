import changeOrderStatusesTemplate from './modal/ChangeOrderStatuses.html';

(function (ng) {
    'use strict';

    var OrdersCtrl = function (
        $location,
        $window,
        uiGridConstants,
        uiGridCustomConfig,
        uiGridCustomParamsConfig,
        uiGridCustomService,
        urlHelper,
        $q,
        SweetAlert,
        lastStatisticsService,
        adminWebNotificationsService,
        adminWebNotificationsEvents,
        $translate,
        $scope,
        isMobileService,
        toaster,
        $http,
    ) {
        var ctrl = this;

        ctrl.init = function (showManagers) {
            const locationSearch = $location.search();
            ctrl.gridParams = { filterBy: locationSearch.filterBy || 'None' };
            adminWebNotificationsService.addListener(adminWebNotificationsEvents.updateOrders, function () {
                ctrl.gridUpdate();
            });

            ctrl.showManagers = showManagers;

            var columnDefs = [
                {
                    name: '_noopColumnNumber',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.NumberOfOrder'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Number',
                    },
                },
                {
                    name: '_noopColumnStatuses',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.Status'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'OrderStatusId',
                        fetch: 'orders/getorderstatuses',
                    },
                },
                {
                    name: '_noopColumnSum',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.Cost'),
                        type: 'range',
                        rangeOptions: {
                            from: {
                                name: 'PriceFrom',
                            },
                            to: {
                                name: 'PriceTo',
                            },
                        },
                    },
                },
                {
                    name: '_noopColumnIsPaid',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.Payment'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'IsPaid',
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.Orders.Yes'),
                                value: true,
                            },
                            {
                                label: $translate.instant('Admin.Js.Orders.No'),
                                value: false,
                            },
                        ],
                    },
                },
                {
                    name: '_noopColumnName',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.CustomersFullName'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'BuyerName',
                    },
                },
                {
                    name: '_noopColumnPhone',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.CustomersPhone'),
                        type: 'phone',
                        name: 'BuyerPhone',
                    },
                },
                {
                    name: '_noopColumnEmail',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.CustomersEmail'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'BuyerEmail',
                    },
                },
                {
                    name: '_noopColumnProduct',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.NameOrVendorCodeOfProduct'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'ProductNameArtNo',
                    },
                },
            ];

            if (ctrl.showManagers) {
                columnDefs.push({
                    name: '_noopColumnManager',
                    visible: false,
                    enableHiding: false,
                    enabled: ctrl.showManagers,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.Manager'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'ManagerId',
                        fetch: 'managers/getManagersSelectOptions?includeEmpty=true',
                    },
                });
            }

            columnDefs = columnDefs.concat([
                {
                    name: '_noopColumnShippings',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.ShippingMethods'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'ShippingMethod',
                        //fetch: 'orders/getordershippingmethods'
                    },
                },
                {
                    name: '_noopColumnPayments',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.PaymentMethod'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'PaymentMethod',
                        fetch: 'orders/getorderpaymentmethods',
                    },
                },
                {
                    name: '_noopColumnSources',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.OrderSource'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'OrderSourceId',
                        fetch: 'orders/getordersources',
                    },
                },
                {
                    name: 'Number',
                    displayName: $translate.instant('Admin.Js.Orders.Number'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><a ng-href="orders/edit/{{row.entity.OrderId}}">{{COL_FIELD}}</a></div>',
                    width: 90,
                },
                {
                    name: 'StatusName',
                    displayName: $translate.instant('Admin.Js.Orders.Status'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><i class="fa fa-circle m-r-xs" ng-style="{color:\'#\' + row.entity.Color}"></i> {{row.entity.StatusName}}</div>',
                },
                {
                    name: 'BuyerName',
                    displayName: $translate.instant('Admin.Js.Orders.Customer'),
                },
                {
                    name: 'GroupName',
                    displayName: $translate.instant('Admin.Js.Orders.GroupOfCustomers'),
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.GroupOfCustomers'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'GroupName',
                        fetch: 'orders/GetOrderCustomerGroupNames',
                    },
                },
                {
                    name: 'City',
                    displayName: $translate.instant('Admin.Js.Orders.CustomersCity'),
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.CustomersCity'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'BuyerCity',
                    },
                },
                {
                    name: 'DeliveryAddress',
                    displayName: $translate.instant('Admin.Js.Orders.DeliveryAddress'),
                    visible: false,
                },
                {
                    name: 'OrderItems',
                    displayName: $translate.instant('Admin.Js.Orders.OrderItems'),
                    width: 230,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div ng-repeat="item in row.entity.OrderItems | limitTo:5 track by $index">' +
                        '<div class="m-b-xs" ng-bind="item"></div>' +
                        '</div><div ng-if="row.entity.OrderItems.length > 5">и другие</div></div>',
                    visible: false,
                },
                {
                    name: 'SumFormatted',
                    displayName: $translate.instant('Admin.Js.Orders.Amount'),
                    width: 100,
                },
                {
                    name: 'IsPaid',
                    displayName: $translate.instant('Admin.Js.Orders.Payment'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div class="adv-checkbox-label">' +
                        '<input type="checkbox" ng-model="row.entity.IsPaid" readonly class="adv-checkbox-input control-checkbox pointer-events-none" data-e2e="switchOnOffSelect" />' +
                        '<span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span>' +
                        '</div></div>',
                    width: 65,
                    headerCellClass: 'ui-grid-text-center',
                    cellClass: 'ui-grid-text-center',
                },
                {
                    name: 'PaymentMethod',
                    displayName: $translate.instant('Admin.Js.Orders.PaymentMethod'),
                    visible: 1367,
                },
                {
                    name: 'ShippingMethod',
                    displayName: $translate.instant('Admin.Js.Orders.ShippingMethods'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">{{row.entity.ShippingMethod != null && row.entity.ShippingMethod.length > 0 ? row.entity.ShippingMethod : row.entity.ShippingMethodName}}</div>',
                    visible: 1367,
                },
                {
                    name: '_noopColumnBuyerApartment',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.BuyerApartment'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'BuyerApartment',
                    },
                },
                {
                    name: '_noopColumnBuyerZip',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.BuyerZip'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'BuyerZip',
                    },
                },
                {
                    name: '_noopColumnBuyerStreet',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.BuyerStreet'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'BuyerStreet',
                    },
                },
                {
                    name: '_noopColumnBuyerHouse',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.BuyerHouse'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'BuyerHouse',
                    },
                },
                {
                    name: '_noopColumnBuyerStructure',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.BuyerStructure'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'BuyerStructure',
                    },
                },
                {
                    name: 'ManagerName',
                    displayName: $translate.instant('Admin.Js.Orders.Manager'),
                    visible: ctrl.showManagers && 1441,
                    enableHiding: ctrl.showManagers,
                },
                {
                    name: 'AdminOrderComment',
                    displayName: $translate.instant('Admin.Js.Orders.AdminOrderComment'),
                    visible: ctrl.showManagers && 1601,
                    enableHiding: ctrl.showManagers,
                },
                {
                    name: 'OrderDateFormatted',
                    displayName: $translate.instant('Admin.Js.Orders.DateAndTime'),
                    width: 114,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.DateAndTime'),
                        type: 'datetime',
                        term: {
                            from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                            to: new Date(),
                        },
                        datetimeOptions: {
                            from: {
                                name: 'OrderDateFrom',
                            },
                            to: {
                                name: 'OrderDateTo',
                            },
                        },
                    },
                },
                {
                    name: '_noopColumnCouponCode',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.CouponCode'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'CouponCode',
                    },
                },
                {
                    name: 'DeliveryDateTimeFormatted',
                    displayName: $translate.instant('Admin.Js.Orders.DeliveryDate'),
                    cellTemplate: '<div class="ui-grid-cell-contents" ng-bind-html="row.entity.DeliveryDateTimeFormatted"></div>',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Orders.DeliveryDate'),
                        type: 'date',
                        term: {
                            from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                            to: new Date(),
                        },
                        dateOptions: {
                            from: { name: 'DeliveryDateFrom' },
                            to: { name: 'DeliveryDateTo' },
                        },
                    },
                },
                {
                    name: '_serviceColumnEdit',
                    displayName: '',
                    enableHiding: false,
                    width: 40,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    visible: isMobileService.getValue() !== true,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div class="js-grid-not-clicked"><a ng-if="!grid.appScope.$ctrl.isMobile" ng-href="orders/edit/{{row.entity.OrderId}}" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></a></div></div>',
                },
                {
                    name: '_serviceColumnDelete',
                    displayName: '',
                    enableHiding: false,
                    width: 40,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate: uiGridCustomService.getTemplateCellDelete('orders/deleteorder', '{OrderId: row.entity.OrderId }'),
                },
            ]);

            ctrl.fetchGridActions().then((data) => {
                data.forEach(function (item) {
                    if (item.urlAfterAction)
                        item.after = function (data) {
                            var form = document.createElement('form');
                            form.method = 'POST';
                            form.name = 'test';
                            form.action = item.urlAfterAction;
                            form.target = '_blank';
                            var el = document.createElement('input');
                            el.type = 'hidden';
                            el.name = 'dataStr';
                            if (data) {
                                if (typeof data == 'object') {
                                    form.appendChild(el).value = JSON.stringify(data);
                                } else {
                                    form.appendChild(el).value = data;
                                }
                            } else {
                                form.appendChild(el).value = '';
                            }
                            document.body.appendChild(form);
                            form.submit();
                            document.body.removeChild(form);
                        };
                });
                ctrl.gridOptions.uiGridCustom.selectionOptions = ctrl.gridOptions.uiGridCustom.selectionOptions.concat(data);
            });

            ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
                enableGridMenu: !isMobileService.getValue(),
                columnDefs: columnDefs,
                uiGridCustom: {
                    rowUrl: 'orders/edit/{{row.entity.OrderId}}',
                    selectionOptions: [
                        {
                            text: $translate.instant('Admin.Js.Orders.DeleteSelected'),
                            url: 'orders/deleteorders',
                            field: 'OrderId',
                            before: function () {
                                return SweetAlert.confirm($translate.instant('Admin.Js.Orders.AreYouSureDelete'), {
                                    title: $translate.instant('Admin.Js.Orders.Deleting'),
                                }).then(function (result) {
                                    if (result === true || result.value) {
                                        return $q.resolve('sweetAlertConfirm');
                                    }

                                    return $q.reject('sweetAlertCancel');
                                });
                            },
                            after: function () {
                                lastStatisticsService.getLastStatistics();
                            },
                        },
                        {
                            template:
                                '<ui-modal-trigger data-controller="\'ModalChangeOrderStatusesCtrl\'" controller-as="ctrl" data-resolve="{params:$ctrl.getSelectedParams(\'OrderId\')}" ' +
                                'template-url="' +
                                changeOrderStatusesTemplate +
                                '" ' +
                                'data-on-close="$ctrl.gridOnAction()">' +
                                $translate.instant('Admin.Js.Orders.ChangeStatusToSelected') +
                                '</ui-modal-trigger>',
                        },
                        {
                            text: $translate.instant('Admin.Js.Orders.MarkAsPaid'),
                            url: 'orders/markpaid',
                            field: 'OrderId',
                        },
                        {
                            text: $translate.instant('Admin.Js.Orders.MarkAsUnpaid'),
                            url: 'orders/marknotpaid',
                            field: 'OrderId',
                        },
                    ],
                },
            });
        };

        ctrl.fetchGridActions = function () {
            return $http.get('Orders/GetGridSelectionOptions').then((response) => response.data);
        };

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.changeStatusParam = function (filterBy, statusId) {
            $location.search({ filterBy });
            ctrl.gridParams['filterBy'] = filterBy;
            ctrl.gridParams['statusId'] = statusId;
            ctrl.grid.setParams(ctrl.gridParams);
            ctrl.grid.fetchData();
        };

        ctrl.gridUpdate = function () {
            ctrl.grid.fetchData(true);
        };
    };

    OrdersCtrl.$inject = [
        '$location',
        '$window',
        'uiGridConstants',
        'uiGridCustomConfig',
        'uiGridCustomParamsConfig',
        'uiGridCustomService',
        'urlHelper',
        '$q',
        'SweetAlert',
        'lastStatisticsService',
        'adminWebNotificationsService',
        'adminWebNotificationsEvents',
        '$translate',
        '$scope',
        'isMobileService',
        'toaster',
        '$http',
    ];

    ng.module('orders', ['uiGridCustom', 'urlHelper', 'isMobile']).controller('OrdersCtrl', OrdersCtrl);
})(window.angular);
