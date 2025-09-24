import emailingsWithoutTemplate from './emailingsWithout.html';
(function (ng) {
    'use strict';

    var EmailingsWithoutCtrl = function ($http, uiGridConstants, uiGridCustomConfig, uiGridCustomParamsConfig, uiGridCustomService, $translate) {
        var ctrl = this;
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
                    '<a ng-click="grid.appScope.$ctrl.gridExtendCtrl.changeView(\'details\', row.entity.FormatId)" href="" class="link-invert ui-grid-custom-service-icon fas fa-eye"></a> ' +
                    '</div></div>',
            },
        ];
        ctrl.gridWithousOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsWithous,
            uiGridCustom: {
                //rowUrl: 'emailings/manualWithoutEmailing/{{row.entity.FormatId}}',
                rowClick: function ($event, row) {
                    ctrl.changeView('details', row.entity.FormatId);
                },
            },
        });
        ctrl.gridWithousOnInit = function (gridWithous) {
            ctrl.gridWithous = gridWithous;
        };
        ctrl.$onInit = function () {
            ctrl.pageType = 'default';
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    emailingsWithout: ctrl,
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
    EmailingsWithoutCtrl.$inject = [
        '$http',
        'uiGridConstants',
        'uiGridCustomConfig',
        'uiGridCustomParamsConfig',
        'uiGridCustomService',
        '$translate',
    ];
    ng.module('analyticsReport')
        .controller('EmailingsWithoutCtrl', EmailingsWithoutCtrl)
        .component('emailingsWithout', {
            templateUrl: emailingsWithoutTemplate,
            controller: EmailingsWithoutCtrl,
            bindings: {
                onInit: '&',
            },
        });
})(window.angular);
