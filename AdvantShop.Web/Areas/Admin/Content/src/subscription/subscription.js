import sendSmsTemplate from './../_shared/modal/sendSms/sendSms.html';
import sendLetterToCustomerTemplate from './../_shared/modal/sendLetterToCustomer/sendLetterToCustomer.html';
(function (ng) {
    'use strict';

    var SubscriptionCtrl = function (
        Upload,
        toaster,
        $location,
        $window,
        uiGridConstants,
        uiGridCustomConfig,
        $q,
        SweetAlert,
        $http,
        $uibModal,
        $translate,
    ) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Email',
                    displayName: 'Email',
                    enableSorting: true,
                    enableCellEdit: false,
                    filter: {
                        placeholder: 'Email',
                        type: uiGridConstants.filter.INPUT,
                        name: 'Email',
                    },
                },
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.Subscription.Active'),
                    enableCellEdit: false,
                    width: 80,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div style="text-align:center"><ui-grid-custom-switch row="row"></ui-grid-custom-switch></div></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Subscription.Active'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'Enabled',
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.Subscription.Actives'),
                                value: true,
                            },
                            {
                                label: $translate.instant('Admin.Js.Subscription.NotActive'),
                                value: false,
                            },
                        ],
                    },
                },
                {
                    name: 'SubscribeDateStr',
                    displayName: $translate.instant('Admin.Js.Subscription.SubscriptionDate'),
                    width: 180,
                    enableCellEdit: false,
                    cellTemplate: '<div class="ui-grid-cell-contents"><div style="pointer-events: none;text-align:center;">{{COL_FIELD}}</div></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Subscription.SubscriptionDate'),
                        type: 'datetime',
                        term: {
                            from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                            to: new Date(),
                        },
                        datetimeOptions: {
                            from: {
                                name: 'SubscribeFrom',
                            },
                            to: {
                                name: 'SubscribeTo',
                            },
                        },
                    },
                },
                {
                    name: 'SubscribeFromPage',
                    displayName: $translate.instant('Admin.Js.Subscription.FromPage'),
                    width: 300,
                    enableSorting: true,
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Subscription.FromPage'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'SubscribeFromPage',
                    },
                },
                {
                    name: 'SubscribeFromIp',
                    displayName: $translate.instant('Admin.Js.Subscription.FromIp'),
                    width: 180,
                    enableSorting: true,
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Subscription.FromIp'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'SubscribeFromIp',
                    },
                },
                {
                    name: 'UnsubscribeDateStr',
                    displayName: $translate.instant('Admin.Js.Subscription.UnsubscriptionDate'),
                    width: 180,
                    enableCellEdit: false,
                    cellTemplate: '<div class="ui-grid-cell-contents"><div style="pointer-events: none;text-align:center;">{{COL_FIELD}}</div></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Subscription.UnsubscriptionDate'),
                        type: 'datetime',
                        term: {
                            from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                            to: new Date(),
                        },
                        datetimeOptions: {
                            from: {
                                name: 'UnSubscribeFrom',
                            },
                            to: {
                                name: 'UnsubscribeTo',
                            },
                        },
                    },
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 50,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<ui-grid-custom-delete url="Subscription/DeleteSubscription" params="{\'Ids\': row.entity.Id}"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="Subscription/DeleteSubscription" params="{\'Ids\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        template:
                            '<span ng-click="$ctrl.grid.gridExtendCtrl.sendEmails($ctrl.getSelectedParams(\'Id\'))" class="block">' +
                            $translate.instant('Admin.Js.Customers.WriteAnEmail') +
                            '</span>',
                    },
                    {
                        template:
                            '<span ng-click="$ctrl.grid.gridExtendCtrl.sendSms($ctrl.getSelectedParams(\'Id\'))" class="block">' +
                            $translate.instant('Admin.Js.Customers.SendSMS') +
                            '</span>',
                    },
                    {
                        text: $translate.instant('Admin.Js.Subscription.DeleteSelected'),
                        url: 'Subscription/DeleteSubscription',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Subscription.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.Subscription.Deleting'),
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
        ctrl.sendEmails = function (filter) {
            $http.post('subscription/getSubscriptionIds', filter).then(function (response) {
                var subscriptionIds = response.data.ids;
                $uibModal.open({
                    animation: false,
                    bindToController: true,
                    size: 'lg',
                    controller: 'ModalSendLetterToCustomerCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: sendLetterToCustomerTemplate,
                    resolve: {
                        params: {
                            subscriptionIds: subscriptionIds,
                            pageType: 'subscription',
                        },
                    },
                });
            });
        };
        ctrl.sendSms = function (filter) {
            $http.post('subscription/getSubscriptionIds', filter).then(function (response) {
                var subscriptionIds = response.data.ids;
                $uibModal.open({
                    animation: false,
                    bindToController: true,
                    controller: 'ModalSendSmsAdvCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: sendSmsTemplate,
                    resolve: {
                        params: {
                            subscriptionIds: subscriptionIds,
                        },
                    },
                });
            });
        };
        ctrl.upload = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                Upload.upload({
                    url: '/Subscription/Import',
                    data: {
                        file: $file,
                        rnd: Math.random(),
                    },
                }).then(function (response) {
                    var data = response.data;
                    if (data.Result === true) {
                        toaster.pop('success', $translate.instant('Admin.Js.Subscription.FileSuccessfullyUploaded'));
                        ctrl.grid.fetchData();
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.Subscription.ErrorLoadingFile'), data.Error);
                    }
                });
            } else if ($invalidFiles.length > 0) {
                toaster.pop(
                    'error',
                    $translate.instant('Admin.Js.Subscription.ErrorLoadingFile'),
                    $translate.instant('Admin.Js.Subscription.FileNotMeetRequirement'),
                );
            }
        };
    };
    SubscriptionCtrl.$inject = [
        'Upload',
        'toaster',
        '$location',
        '$window',
        'uiGridConstants',
        'uiGridCustomConfig',
        '$q',
        'SweetAlert',
        '$http',
        '$uibModal',
        '$translate',
    ];
    ng.module('subscription', ['uiGridCustom', 'urlHelper']).controller('SubscriptionCtrl', SubscriptionCtrl);
})(window.angular);
