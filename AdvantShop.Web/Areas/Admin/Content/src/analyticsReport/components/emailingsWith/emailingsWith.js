import emailingsWithTemplate from './emailingsWith.html';
(function (ng) {
    'use strict';

    /* @ngInject */
    var EmailingsWithCtrl = function (
        $http,
        uiGridConstants,
        uiGridCustomConfig,
        uiGridCustomParamsConfig,
        uiGridCustomService,
        $translate,
        toaster,
        $q,
        SweetAlert,
    ) {
        var ctrl = this;
        ctrl.$onINit = function () {
            ctrl.pageType = 'default';
        };
        var columnDefs = [
            {
                name: 'Subject',
                displayName: $translate.instant('Admin.Js.ManualEmailings.Subject'),
                filter: {
                    placeholder: $translate.instant('Admin.Js.ManualEmailings.Subject'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Subject',
                },
            },
            {
                name: 'DateCreatedFormatted',
                displayName: $translate.instant('Admin.Js.ManualEmailings.DateCreated'),
                width: 100,
                filter: {
                    placeholder: $translate.instant('Admin.Js.ManualEmailings.DateCreated'),
                    type: 'datetime',
                    term: {
                        from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                        to: new Date(),
                    },
                    datetimeOptions: {
                        from: {
                            name: 'DateCreatedFrom',
                        },
                        to: {
                            name: 'DateCreatedTo',
                        },
                    },
                },
            },
            {
                name: 'TotalCount',
                displayName: $translate.instant('Admin.Js.ManualEmailings.TotalCount'),
                width: 90,
                filter: {
                    placeholder: $translate.instant('Admin.Js.ManualEmailings.TotalCount'),
                    type: 'range',
                    rangeOptions: {
                        from: {
                            name: 'TotalCountFrom',
                        },
                        to: {
                            name: 'TotalCountTo',
                        },
                    },
                },
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 50,
                enableSorting: false,
                cellTemplate:
                    '<div class="ui-grid-cell-contents"><div class="js-grid-not-clicked">' +
                    '<ui-grid-custom-delete url="emailings/deleteManualEmailings" params="{\'Ids\': row.entity.Id}"></ui-grid-custom-delete>' +
                    '</div></div>',
            },
        ];
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.changeView('details', row.entity.Id);
                },
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.ManualEmailings.DeleteSelected'),
                        url: 'emailings/deleteManualEmailings',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.ManualEmailings.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.ManualEmailings.Deleting'),
                            }).then(function (result) {
                                if (result === true || result.value) {
                                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                                    return $q.resolve('sweetAlertConfirm');
                                }
                                return $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
        });
        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };
        ctrl.$onInit = function () {
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    emailingsWith: ctrl,
                });
            }
        };
        ctrl.recalc = function (dateFrom, dateTo) {
            //ctrl.fetchCommonData(dateFrom, dateTo);
        };
        ctrl.changeView = function (pageType, id, logGridParams) {
            ctrl.pageType = pageType;
            ctrl.Id = id;
            ctrl.logGridParams = logGridParams;
        };
    };
    EmailingsWithCtrl.$inject = [
        '$http',
        'uiGridConstants',
        'uiGridCustomConfig',
        'uiGridCustomParamsConfig',
        'uiGridCustomService',
        '$translate',
        'toaster',
    ];
    ng.module('analyticsReport')
        .controller('EmailingsWithCtrl', EmailingsWithCtrl)
        .component('emailingsWith', {
            templateUrl: emailingsWithTemplate,
            controller: EmailingsWithCtrl,
            bindings: {
                onInit: '&',
            },
        });
})(window.angular);
