import sendSmsTemplate from '../_shared/modal/sendSms/sendSms.html';
import sendLetterToCustomerTemplate from '../_shared/modal/sendLetterToCustomer/sendLetterToCustomer.html';
import sendSocialMessageTemplate from '../_shared/modal/sendSocialMessage/sendSocialMessage.html';
import addTagsToCustomersTemplate from '../_shared/modal/addTagsToCustomers/addTagsToCustomers.html';

(function (ng) {
    'use strict';

    var CustomersCtrl = function (
        $location,
        $window,
        uiGridConstants,
        uiGridCustomConfig,
        uiGridCustomParamsConfig,
        uiGridCustomService,
        $q,
        SweetAlert,
        $http,
        customerFieldsService,
        $uibModal,
        $translate,
        isMobileService,
        toaster,
    ) {
        var ctrl = this,
            columnDefs = [],
            url = document.location.pathname.toLowerCase().indexOf('customerscrm') >= 0 ? 'customerscrm' : 'customers';

        var isMobile = isMobileService.getValue();

        ctrl.gridOptions = {};

        ctrl.customersInit = function (enableManagers, hasBonuses) {
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Customers.Customer'),
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Customers.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><a ng-href="' +
                        url +
                        '/view/{{row.entity.CustomerId}}">{{row.entity.Organization != null && row.entity.Organization.length > 0 ? row.entity.Organization : row.entity.Name }}</a></div>',
                },
                {
                    name: 'Phone',
                    displayName: $translate.instant('Admin.Js.Customers.Phone'),
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Customers.Phone'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Phone',
                    },
                },
                {
                    name: 'Email',
                    displayName: 'Email',
                    filter: {
                        placeholder: 'Email',
                        type: uiGridConstants.filter.INPUT,
                        name: 'Email',
                    },
                },
            ];
            columnDefs.push({
                name: 'CustomerTypeFormatted',
                displayName: $translate.instant('Admin.Js.SettingsCustomers.CustomerType'),
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsCustomers.CustomerType'),
                    type: uiGridConstants.filter.SELECT,
                    name: 'CustomerType',
                    fetch: 'customers/getCustomerTypes',
                },
            });
            columnDefs.push(
                {
                    name: 'RegistrationDateTimeFormatted',
                    displayName: $translate.instant('Admin.Js.Customers.DateOfRegistration'),
                    width: 110,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Customers.DateOfRegistration'),
                        type: 'datetime',
                        term: {
                            from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                            to: new Date(),
                        },
                        datetimeOptions: {
                            from: {
                                name: 'RegistrationDateTimeFrom',
                            },
                            to: { name: 'RegistrationDateTimeTo' },
                        },
                    },
                },
                {
                    name: 'OrdersCount',
                    displayName: $translate.instant('Admin.Js.Customers.AmountOfPaidOrders'),
                    width: 100,
                    type: 'number',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Customers.AmountOfPaidOrders'),
                        type: 'range',
                        term: {
                            from: 0,
                            to: 0,
                        },
                        rangeOptions: {
                            from: { name: 'OrdersCountFrom' },
                            to: { name: 'OrdersCountTo' },
                        },
                    },
                },
                {
                    name: 'LastOrderNumber',
                    displayName: $translate.instant('Admin.Js.Customers.LastOrder'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents js-grid-not-clicked" data-ng-show="row.entity.LastOrderNumber != null"> <a href="orders/edit/{{row.entity.LastOrderId}}" class="link"># {{row.entity.LastOrderNumber}}</a> </div>',
                    width: 100,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Customers.LastOrder'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'LastOrderNumber',
                    },
                },
                {
                    name: '_noopColumnLastOrderDate',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Customers.DateOfLastOrder'),
                        type: 'datetime',
                        term: {
                            from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                            to: new Date(),
                        },
                        datetimeOptions: {
                            from: { name: 'LastOrderDateTimeFrom' },
                            to: { name: 'LastOrderDateTimeTo' },
                        },
                    },
                },
                {
                    name: 'OrdersSum',
                    displayName: $translate.instant('Admin.Js.Customers.AmountOfOrders'),
                    type: 'number',
                    width: 100,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Customers.AmountOfOrders'),
                        type: 'range',
                        rangeOptions: {
                            from: { name: 'OrderSumFrom' },
                            to: { name: 'OrderSumTo' },
                        },
                    },
                },
                {
                    name: '_noopColumnAvgCheck',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Customers.AverageCheck'),
                        type: 'range',
                        rangeOptions: {
                            from: { name: 'AverageCheckFrom' },
                            to: { name: 'AverageCheckTo' },
                        },
                    },
                },
            );

            if (enableManagers) {
                columnDefs.push({
                    name: 'ManagerName',
                    displayName: $translate.instant('Admin.Js.Customers.Manager'),
                    width: 110,
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Customers.Manager'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'ManagerId',
                        fetch: 'managers/getManagersSelectOptions?includeEmpty=true',
                    },
                });
            }
            columnDefs.push({
                name: '_serviceColumnEdit',
                displayName: '',
                width: 40,
                enableSorting: false,
                useInSwipeBlock: true,
                cellTemplate: isMobile
                    ? ''
                    : '<div class="ui-grid-cell-contents"><div class="js-grid-not-clicked"><customer-info-trigger customer-id="row.entity.CustomerId" on-close="grid.appScope.$ctrl.fetchData()"><button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></button></customer-info-trigger></div></div>',
            });

            columnDefs.push({
                name: '_serviceColumnDelete',
                displayName: '',
                width: 40,
                enableSorting: false,
                useInSwipeBlock: true,
                cellTemplate: uiGridCustomService.getTemplateCellDelete(
                    url + '/deletecustomer',
                    '{CustomerId: row.entity.CustomerId}',
                    'data-ng-show="{{row.entity.CanBeDeleted}}"',
                ),
            });

            columnDefs.push(
                {
                    name: '_noopColumnLocation',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Customers.City'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Location',
                    },
                },
                {
                    name: '_noopColumnCountry',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Customers.Country'),
                        name: 'Country',
                        type: uiGridConstants.filter.SELECT,
                        fetch: 'customers/getCustomerCountries',
                    },
                },
                {
                    name: '_noopColumnSocial',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Customers.HaveAccountOnVKandFB'),
                        name: 'SocialType',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.Customers.VKontakte'),
                                value: 'vk',
                            },
                            { label: 'Instagram', value: 'instagram' },
                            //{ label: 'Facebook', value: 'fb' },
                            { label: 'Telegram', value: 'telegram' },
                            {
                                label: $translate.instant('Admin.Js.Customers.AnySocialNetwork'),
                                value: 'all',
                            },
                        ],
                    },
                },
                {
                    name: '_noopColumnSubscription',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Customers.HaveSubscription'),
                        name: 'Subscription',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.Customers.HaveSubscriptionYes'),
                                value: 'true',
                            },
                            {
                                label: $translate.instant('Admin.Js.Customers.HaveSubscriptionNo'),
                                value: 'false',
                            },
                        ],
                    },
                },
            );

            if (hasBonuses) {
                columnDefs.push({
                    name: '_noopColumnHasBonusCard',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Customers.HasBonusCard'),
                        name: 'HasBonusCard',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.Yes'),
                                value: 'true',
                            },
                            {
                                label: $translate.instant('Admin.Js.No'),
                                value: 'false',
                            },
                        ],
                    },
                });
            }

            columnDefs.push(
                {
                    name: '_GroupId',
                    visible: false,
                    displayName: $translate.instant('Admin.Js.Customers.Group'),
                    width: 110,
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Customers.Group'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'GroupId',
                        fetch: 'customerGroups/getCustomerGroupsSelectOptions',
                    },
                },
                {
                    name: '_noopColumnTag',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Customers.Tag'),
                        name: 'Tags',
                        type: 'selectMultiple',
                        fetch: 'customerTags/getCustomerTagsSelectOptions',
                    },
                },
                {
                    name: '_noopColumnCustomerSegment',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Customers.CustomerSegment'),
                        name: 'CustomerSegment',
                        type: uiGridConstants.filter.SELECT,
                        fetch: 'CustomerSegments/getCustomerSegmentsSelectOptions',
                    },
                },
            );

            ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: columnDefs,
                uiGridCustom: {
                    rowUrl: url + '/view/{{row.entity.CustomerId}}',
                    selectionOptions: [
                        {
                            text: $translate.instant('Admin.Js.Customers.DeleteSelected'),
                            url: url + '/deletecustomers',
                            field: 'CustomerId',
                            before: function () {
                                return SweetAlert.confirm($translate.instant('Admin.Js.Customers.AreYouSureDelete'), {
                                    title: $translate.instant('Admin.Js.Customers.Deleting'),
                                }).then(function (result) {
                                    if (result === true || result.value) {
                                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                                        return $q.resolve('sweetAlertConfirm');
                                    }

                                    return $q.reject('sweetAlertCancel');
                                });
                            },
                        },
                        {
                            template:
                                '<span ng-click="$ctrl.grid.gridExtendCtrl.sendEmails($ctrl.getSelectedParams(\'CustomerId\'))" class="block">' +
                                $translate.instant('Admin.Js.Customers.WriteAnEmail') +
                                '</span>',
                        },
                        {
                            template:
                                '<span ng-click="$ctrl.grid.gridExtendCtrl.sendSms($ctrl.getSelectedParams(\'CustomerId\'))" class="block">' +
                                $translate.instant('Admin.Js.Customers.SendSMS') +
                                '</span>',
                        },
                        {
                            template:
                                '<span ng-click="$ctrl.grid.gridExtendCtrl.sendSocialMessage($ctrl.getSelectedParams(\'CustomerId\'), \'vk\')" class="block">' +
                                $translate.instant('Admin.Js.Customers.SendVKmessage') +
                                '</span>',
                        },
                        {
                            template:
                                '<span ng-click="$ctrl.grid.gridExtendCtrl.sendSocialMessage($ctrl.getSelectedParams(\'CustomerId\'), \'telegram\')" class="block">' +
                                $translate.instant('Admin.Js.Customers.SendTelegramMessage') +
                                '</span>',
                        },
                        {
                            template:
                                '<ui-modal-trigger data-on-close="$ctrl.gridOnAction()" data-controller="\'ModalAddTagsToCustomersCtrl\'" controller-as="ctrl" ' +
                                'data-resolve="{params:$ctrl.getSelectedParams(\'CustomerId\')}" template-url="' +
                                addTagsToCustomersTemplate +
                                '" size="md">' +
                                $translate.instant('Admin.Js.Customers.AddTags') +
                                '</ui-modal-trigger>',
                        },
                    ],
                },
            });
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

        ctrl.sendSocialMessage = function (filter, type) {
            $http.post(url + '/getCustomerIds', filter).then(function (response) {
                var customerIds = response.data.customerIds;

                $uibModal.open({
                    animation: false,
                    bindToController: true,
                    controller: 'ModalSendSocialMessageCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: sendSocialMessageTemplate,
                    resolve: {
                        params: {
                            customerIds: customerIds,
                            type: type,
                        },
                    },
                });
            });
        };

        ctrl.sendEmails = function (filter) {
            $http.post(url + '/getCustomerIds', filter).then(function (response) {
                var customerIds = response.data.customerIds;

                $uibModal.open({
                    animation: false,
                    bindToController: true,
                    size: 'lg',
                    controller: 'ModalSendLetterToCustomerCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: sendLetterToCustomerTemplate,
                    resolve: {
                        params: {
                            customerIds: customerIds,
                            pageType: 'customers',
                        },
                    },
                });
            });
        };

        ctrl.sendSms = function (filter) {
            $http.post(url + '/getCustomerIds', filter).then(function (response) {
                var customerIds = response.data.customerIds;

                $uibModal.open({
                    animation: false,
                    bindToController: true,
                    controller: 'ModalSendSmsAdvCtrl',
                    controllerAs: 'ctrl',
                    size: 'middle',
                    templateUrl: sendSmsTemplate,
                    resolve: {
                        params: { customerIds: customerIds },
                    },
                });
            });
        };

        ctrl.sendSmsNotEnabled = function () {
            SweetAlert.confirm(
                $translate.instant('Admin.Js.Customers.SmsModuleIsNotConnected') +
                    '<br/>' +
                    $translate.instant('Admin.Js.Customers.YouCan') +
                    '<a href="modules/market" target="_blank">' +
                    $translate.instant('Admin.Js.Customers.ConnectTheModule') +
                    '</a>' +
                    $translate.instant('Admin.Js.Customers.SMSInforming'),
                { title: '' },
            ).then(function (result) {});
        };
    };

    CustomersCtrl.$inject = [
        '$location',
        '$window',
        'uiGridConstants',
        'uiGridCustomConfig',
        'uiGridCustomParamsConfig',
        'uiGridCustomService',
        '$q',
        'SweetAlert',
        '$http',
        'customerFieldsService',
        '$uibModal',
        '$translate',
        'isMobileService',
        'toaster',
    ];

    ng.module('customers', ['uiGridCustom', 'urlHelper', 'isMobile']).controller('CustomersCtrl', CustomersCtrl);
})(window.angular);
