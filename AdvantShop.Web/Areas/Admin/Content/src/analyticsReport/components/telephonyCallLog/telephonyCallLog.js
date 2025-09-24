import telephonyCallLogTemplate from './telephonyCallLog.html';
(function (ng) {
    'use strict';

    var TelephonyCallLogCtrl = function (uiGridConstants, uiGridCustomConfig, $q, SweetAlert, $http, $translate, uiGridCustomService, toaster) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Type',
                    displayName: $translate.instant('Admin.Js.Calls.Type'),
                    enableCellEdit: false,
                    enableSorting: false,
                    width: 50,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                        '<span class="icon-call" ng-class="grid.appScope.$ctrl.gridExtendCtrl.getTypeIcon(row.entity.TypeString, row.entity.CalledBack)" title="{{grid.appScope.$ctrl.gridExtendCtrl.getTypeTitle(row.entity)}}"></span>' +
                        '</div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Calls.Type'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'Type',
                        fetch: 'calls/getTypesSelectOptions',
                    },
                },
                {
                    name: 'CallDateFormatted',
                    displayName: $translate.instant('Admin.Js.Calls.Date'),
                    enableCellEdit: false,
                    width: 90,
                },
                {
                    name: 'SrcNum',
                    displayName: $translate.instant('Admin.Js.Calls.From'),
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Calls.From'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'SrcNum',
                    },
                },
                {
                    name: 'DstNum',
                    displayName: $translate.instant('Admin.Js.Calls.To'),
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Calls.To'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'DstNum',
                    },
                },
                {
                    name: 'Extension',
                    displayName: $translate.instant('Admin.Js.Calls.AdditionalNumber'),
                    enableCellEdit: false,
                    visible: 1601,
                    width: 90,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Calls.AdditionalNumber'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Extension',
                    },
                },
                {
                    name: 'DurationFormatted',
                    displayName: $translate.instant('Admin.Js.Calls.Duration'),
                    enableCellEdit: false,
                    width: 140,
                    visible: 1601,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Call.DurationInSeconds'),
                        type: 'range',
                        rangeOptions: {
                            from: {
                                name: 'DurationFrom',
                            },
                            to: {
                                name: 'DurationTo',
                            },
                        },
                    },
                },
                {
                    name: 'RecordLink',
                    displayName: $translate.instant('Admin.Js.Calls.CallRecording'),
                    enableCellEdit: false,
                    enableSorting: false,
                    width: 180,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div ng-if="row.entity.CallAnswerDate != null">' +
                        '<call-record call-id="row.entity.Id" operator-type="row.entity.OperatorType"></call-record>' +
                        '</div></div>',
                },
                {
                    name: 'Customer',
                    displayName: $translate.instant('Admin.Js.Calls.Customer'),
                    enableCellEdit: false,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div ng-if="row.entity.CustomerId != null">' +
                        '<a ng-href="customers/view/{{row.entity.CustomerId}}">' +
                        '{{row.entity.CustomerName.length ? row.entity.CustomerName : "' +
                        $translate.instant('Admin.Js.Calls.EmptyCustomerName') +
                        '" }}</a>' +
                        '</div></div>',
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 45,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate: uiGridCustomService.getTemplateCellDelete(
                        'calls/deleteCall',
                        '{callId: row.entity.Id}',
                        'on-delete="grid.appScope.$ctrl.gridExtendCtrl.modalClose()"',
                    ),
                    // cellTemplate:
                    //     '<div class="ui-grid-cell-contents"><div>' +
                    //     '<ui-grid-custom-delete url="calls/deleteCall" params="{\'callId\': row.entity.Id}"></ui-grid-custom-delete>' +
                    //     '</div></div>'
                },
            ];
        ctrl.$onInit = function () {
            ctrl.groupFormatString = 'dd';
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    telephony: ctrl,
                });
            }
        };
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Calls.DeleteSelected'),
                        url: 'calls/deleteCalls',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Calls.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.Calls.Deleting'),
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
        ctrl.getTypeIcon = function (type, calledBack) {
            if (type == null) return '';
            switch (type.toLowerCase()) {
                case 'in':
                    return 'calltype-in';
                case 'out':
                    return 'calltype-out';
                case 'missed':
                    if (calledBack === true) return 'calltype-callback';
                    return 'calltype-missed';
            }
        };
        ctrl.getTypeTitle = function (call) {
            if (call.TypeString == null) return '';
            var result = call.TypeFormatted;
            if (call.TypeString.toLowerCase() == 'missed') {
                if (call.CalledBack === false && call.HangupStatus != 0) result += ', ' + call.HangupStatusFormatted;
                else if (call.CalledBack === true) result += $translate.instant('Admin.Js.Calls.CalledBack');
            }
            return result;
        };
        ctrl.recalc = function (dateFrom, dateTo) {
            ctrl.dateFrom = dateFrom;
            ctrl.dateTo = dateTo;
            if (ctrl.grid != null) {
                ctrl.grid.setParams({
                    CallDateFrom: ctrl.dateFrom,
                    CallDateTo: ctrl.dateTo,
                });
                ctrl.grid.fetchData(true);
            }
        };
    };
    TelephonyCallLogCtrl.$inject = [
        'uiGridConstants',
        'uiGridCustomConfig',
        '$q',
        'SweetAlert',
        '$http',
        '$translate',
        'uiGridCustomService',
        'toaster',
    ];
    ng.module('analyticsReport')
        .controller('TelephonyCallLogCtrl', TelephonyCallLogCtrl)
        .component('telephonyCallLog', {
            templateUrl: telephonyCallLogTemplate,
            controller: TelephonyCallLogCtrl,
            bindings: {
                onInit: '&',
            },
        });
})(window.angular);
