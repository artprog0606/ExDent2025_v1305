import addEditRuleTemplate from './modal/addEditRule/AddEditRule.html';
(function (ng) {
    'use strict';

    var SettingsTasksCtrl = function ($http, $q, $uibModal, uiGridCustomConfig, $translate, $location, isMobileService) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'EventName',
                    displayName: $translate.instant('Admin.Js.SettingsTasks.Event'),
                    enableSorting: false,
                },
                {
                    name: 'ManagerFilterHTML',
                    displayName: $translate.instant('Admin.Js.SettingsTasks.Employee'),
                    width: 250,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                        '<div ng-if="row.entity.ManagerFilter.Comparers.length > 0" ng-repeat="comparer in row.entity.ManagerFilter.Comparers | limitTo : row.entity.ManagerFilter.Comparers.length === 3 ? 3 : 2 ">' +
                        '<span ng-if="row.entity.ManagerFilter.Comparers.length > 1" ng-bind="($index + 1) + \'.\'"></span> <span ng-bind-html="grid.appScope.$ctrl.gridExtendCtrl.getManagerFilterHtml(comparer, row.entity.EventTypeString)"></span>' +
                        '</div><div ng-if="row.entity.ManagerFilter.Comparers.length > 3"><span class="span-link">' +
                        $translate.instant('Admin.Js.SettingsTasks.More') +
                        '</span></div></div>',
                },
                {
                    name: 'TaskDueDateIntervalFormatted',
                    displayName: $translate.instant('Admin.Js.SettingsTasks.TimeForExecution'),
                    enableSorting: false,
                },
                {
                    name: 'TaskCreateIntervalFormatted',
                    displayName: $translate.instant('Admin.Js.SettingsTasks.TermOfSettingTask'),
                    enableSorting: false,
                },
                {
                    name: 'Priority',
                    displayName: $translate.instant('Admin.Js.SettingsTasks.RulePriority'),
                    enableSorting: false,
                },
                //{
                //    name: '_taskServiceColumn',
                //    displayName: 'Текст задачи',
                //    enableSorting: false,
                //    cellTemplate: '<div class="ui-grid-cell-contents"><a href="">Настроить</a></div>'
                //},
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                        '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadRule(row.entity.Id, row.entity.EventTypeString)" aria-label="Редактировать"></button> ' +
                        '<ui-grid-custom-delete url="bizprocessrules/delete" params="{\'Id\': row.entity.Id}"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="bizprocessrules/delete" params="{\'Id\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadRule(row.entity.Id, row.entity.EventTypeString);
                },
            },
        });
        ctrl.gridOrderCreatedRulesOptions = angular.copy(ctrl.gridOptions);
        ctrl.gridOrderStatusChangedRulesOptions = angular.copy(ctrl.gridOptions);
        ctrl.gridLeadCreatedRulesOptions = angular.copy(ctrl.gridOptions);
        ctrl.gridLeadStatusChangedRulesOptions = angular.copy(ctrl.gridOptions);
        ctrl.gridCallMissedRulesOptions = angular.copy(ctrl.gridOptions);
        ctrl.gridReviewAddedRulesOptions = angular.copy(ctrl.gridOptions);
        ctrl.gridMessageReplyRulesOptions = angular.copy(ctrl.gridOptions);
        ctrl.gridTaskCreatedRulesOptions = angular.copy(ctrl.gridOptions);
        ctrl.gridTaskStatusChangedRulesOptions = angular.copy(ctrl.gridOptions);
        ctrl.loadRule = function (id, eventType) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddEditRuleCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: addEditRuleTemplate,
                    resolve: {
                        id: function () {
                            return id;
                        },
                        event: {
                            type: eventType,
                        },
                    },
                    size: 'lg',
                    backdrop: 'static',
                })
                .result.then(
                    function (result) {
                        ctrl.modalClose(result);
                        return result;
                    },
                    function (result) {
                        return result;
                    },
                );
        };
        ctrl.modalClose = function (result) {
            switch (result.eventType) {
                case 'OrderCreated':
                    ctrl.gridOrderCreatedRules.fetchData();
                    break;
                case 'OrderStatusChanged':
                    ctrl.gridOrderStatusChangedRules.fetchData();
                    break;
                case 'LeadCreated':
                    ctrl.gridLeadCreatedRules.fetchData();
                    break;
                case 'LeadStatusChanged':
                    ctrl.gridLeadStatusChangedRules.fetchData();
                    break;
                case 'CallMissed':
                    ctrl.gridCallMissedRules.fetchData();
                    break;
                case 'ReviewAdded':
                    ctrl.gridReviewAddedRules.fetchData();
                    break;
                case 'MessageReply':
                    ctrl.gridMessageReplyRules.fetchData();
                    break;
                case 'TaskCreated':
                    ctrl.gridTaskCreatedRules.fetchData();
                    break;
                case 'TaskStatusChanged':
                    ctrl.gridTaskStatusChangedRules.fetchData();
                    break;
            }
        };
        ctrl.getManagerFilterHtml = function (comparer, eventType) {
            var result;
            if (comparer.FilterType == 2) result = '<span>' + comparer.CustomerName + '</span>';
            else if (comparer.FilterType == 3) {
                switch (eventType) {
                    case 'OrderCreated':
                    case 'OrderStatusChanged':
                        result = '<span>' + $translate.instant('Admin.Js.SettingsTasks.OrderManager') + '</span>';
                        break;
                    case 'LeadCreated':
                    case 'LeadStatusChanged':
                        result = '<span>' + $translate.instant('Admin.Js.SettingsTasks.LeadManager') + '</span>';
                        break;
                    case 'MessageReply':
                        result = '<span>' + $translate.instant('Admin.Js.SettingsTasks.BuyersManager') + '</span>';
                        break;
                    case 'TaskCreated':
                    case 'TaskStatusChanged':
                        result = '<span>' + $translate.instant('Admin.Js.SettingsTasks.TaskManager') + '</span>';
                        break;
                }
            } else {
                result = '<span>' + comparer.FilterTypeName + '</span>';
                var rules = [];
                if (comparer.ManagerRoleId != null) rules.push($translate.instant('Admin.Js.SettingsTasks.Role') + comparer.ManagerRoleName);
                if (comparer.City) rules.push($translate.instant('Admin.Js.SettingsTasks.City') + comparer.City);
                if (rules.length > 0) {
                    result += ' <span class="setting--grid-span-grey">(' + rules.join(', ') + ')</span>';
                }
            }
            return result;
        };
        ctrl.updateGrid = function (name) {
            ctrl['grid' + name + 'Rules'].fetchData();
        };
        ctrl.onChangeStateOffOn = function (checked) {
            ctrl.TasksActive = checked;
        };
        ctrl.onChangeReminderStateOffOn = function (cheched) {
            ctrl.ReminderActive = cheched;
        };
        ctrl.onSelectTab = function (indexTab) {
            ctrl.tabActiveIndex = indexTab;
        };
    };
    SettingsTasksCtrl.$inject = ['$http', '$q', '$uibModal', 'uiGridCustomConfig', '$translate', '$location', 'isMobileService'];
    ng.module('settingsTasks', ['taskgroups', 'tasks', 'adminComments', 'isMobile', 'color.picker', 'as.sortable']).controller(
        'SettingsTasksCtrl',
        SettingsTasksCtrl,
    );
})(window.angular);
