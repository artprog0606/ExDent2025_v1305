(function (ng) {
    'use strict';

    var ChangeHistoryCtrl = function ($http, uiGridCustomConfig, $translate) {
        var ctrl = this;

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'ModificationTimeFormatted',
                    displayName: $translate.instant('Admin.Js.ChangeHistory.Date'),
                    width: 150,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents word-break" ng-if="grid.appScope.$ctrl.gridExtendCtrl.show(row.entity.ModificationTimeFormatted, rowRenderIndex)">' +
                        '{{row.entity.ModificationTimeFormatted}}' +
                        '</div>',
                    enableSorting: false,
                },
                {
                    name: 'ParameterName',
                    displayName: $translate.instant('Admin.Js.ChangeHistory.WhatChanged'),
                    enableSorting: false,
                },
                {
                    name: 'OldValue',
                    displayName: $translate.instant('Admin.Js.ChangeHistory.OldValue'),
                    cellTemplate: '<div class="ui-grid-cell-contents word-break" style="overflow-y: auto;" ng-bind-html="row.entity.OldValue"></div>',
                    enableSorting: false,
                },
                {
                    name: 'NewValue',
                    displayName: $translate.instant('Admin.Js.ChangeHistory.NewValue'),
                    cellTemplate: '<div class="ui-grid-cell-contents word-break" style="overflow-y: auto;" ng-bind-html="row.entity.NewValue"></div>',
                    enableSorting: false,
                },
                {
                    name: 'ChangedByName',
                    displayName: $translate.instant('Admin.Js.ChangeHistory.WhoChanged'),
                    enableSorting: false,
                },
            ],
            paginationPageSize: 50,
            paginationPageSizes: [20, 50, 100],
        });

        var prevDate, prevDate2;

        ctrl.show = function (date, rowRenderIndex) {
            if (rowRenderIndex == 0) prevDate = null;

            if (prevDate == date) return false;

            prevDate = date;
            return true;
        };

        ctrl.show2 = function (date, rowRenderIndex) {
            if (rowRenderIndex == 0) prevDate2 = null;

            if (prevDate2 == date) return false;

            prevDate2 = date;
            return true;
        };
    };

    ChangeHistoryCtrl.$inject = ['$http', 'uiGridCustomConfig', '$translate'];

    ng.module('changeHistory', []).controller('ChangeHistoryCtrl', ChangeHistoryCtrl);
})(window.angular);
