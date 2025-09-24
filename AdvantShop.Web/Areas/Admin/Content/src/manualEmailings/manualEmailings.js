(function (ng) {
    'use strict';

    var ManualEmailingsCtrl = function (uiGridConstants, uiGridCustomConfig, toaster, SweetAlert, $q, $translate) {
        var ctrl = this,
            columnDefs = [
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
                            from: { name: 'DateCreatedFrom' },
                            to: { name: 'DateCreatedTo' },
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
                            from: { name: 'TotalCountFrom' },
                            to: { name: 'TotalCountTo' },
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
                rowUrl: 'emailings/manualemailing/{{row.entity.Id}}',
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.ManualEmailings.DeleteSelected'),
                        url: 'emailings/deleteManualEmailings',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.ManualEmailings.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.ManualEmailings.Deleting'),
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
        });

        var columnDefsWithous = [
            {
                name: 'FormatName',
                displayName: $translate.instant('Admin.Js.EmailingLog.FormatName'),
                enableSorting: false,
            },
            {
                name: 'Count',
                displayName: $translate.instant('Admin.Js.EmailingLog.Count'),
                enableSorting: false,
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 50,
                enableSorting: false,
                cellTemplate:
                    '<div class="ui-grid-cell-contents"><div class="">' +
                    '<a ng-href="emailings/manualWithoutEmailing/{{row.entity.FormatId}}" class="link-invert ui-grid-custom-service-icon fas fa-eye"></a> ' +
                    '</div></div>',
            },
        ];

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.gridWithousOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsWithous,
            uiGridCustom: {
                rowUrl: 'emailings/manualWithoutEmailing/{{row.entity.FormatId}}',
                //rowClick: function ($event, row) {
                //    ctrl.viewEmail(row.entity.Id);
                //}
            },
        });

        ctrl.gridWithousOnInit = function (gridWithous) {
            ctrl.gridWithous = gridWithous;
        };
    };

    ManualEmailingsCtrl.$inject = ['uiGridConstants', 'uiGridCustomConfig', 'toaster', 'SweetAlert', '$q', '$translate'];

    ng.module('manualEmailings', ['uiGridCustom', 'urlHelper']).controller('ManualEmailingsCtrl', ManualEmailingsCtrl);
})(window.angular);
