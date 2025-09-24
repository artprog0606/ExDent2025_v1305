(function (ng) {
    'use strict';

    var RulesCtrl = function (
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
        $translate,
    ) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'RuleTypeStr',
                    displayName: $translate.instant('Admin.Js.Rules.Type'),
                },
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Rules.Name'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><a ng-href="rules/edit/{{row.entity.RuleType}}">{{COL_FIELD}}</a></div>',
                    enableCellEdit: false,
                },
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.Rules.Active'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><label class="adv-checkbox-label"><input disabled class="adv-checkbox-input" type="checkbox" ng-model="row.entity.Enabled"><span class="adv-checkbox-emul"></span></label></div>',
                    enableCellEdit: false,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<a href="rules/edit/{{row.entity.RuleType}}" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></a> ' +
                        '<ui-grid-custom-delete url="rules/deleteRule" params="{\'id\': row.entity.RuleType}"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="rules/deleteRule" params="{\'id\': row.entity.RuleType}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            enableSorting: false,
            uiGridCustom: {
                rowUrl: 'rules/edit/{{row.entity.RuleType}}',
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Rules.DeleteSelected'),
                        url: 'rules/DeleteRuleMass',
                        field: 'RuleType',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Rules.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.Rules.Deleting'),
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

        ctrl.deleteRule = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.Rules.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Rules.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('rules/deleteRule', { id: id }).then(function (response) {
                        $window.location.assign('rules');
                    });
                }
            });
        };
    };

    RulesCtrl.$inject = [
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
        '$translate',
    ];

    ng.module('rules', ['uiGridCustom', 'urlHelper']).controller('RulesCtrl', RulesCtrl);
})(window.angular);
