(function (ng) {
    'use strict';

    var ModalAddEditRuleCtrl = function ($http, $filter, $uibModalInstance, SweetAlert, toaster, $translate) {
        var ctrl = this;
        ctrl.formInited = false;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            if (params.event != null) {
                ctrl.eventType = params.event.type;
            }
            ctrl.id = params.id;
            ctrl.mode = ctrl.id != null ? 'edit' : 'add';

            ctrl.getFormData().then(function () {
                if (ctrl.mode == 'add') {
                    ctrl.priority = 0;
                    ctrl.showEventObject = (ctrl.eventObjects != null && ctrl.eventObjects.length) || ctrl.eventObjectsFetchUrl;
                    ctrl.taskDueDateInterval = ctrl.intervalOrDefault(null, 'taskDueDateIntervalNotSet');
                    ctrl.taskCreateInterval = ctrl.intervalOrDefault(null, 'taskCreateIntervalNotSet');
                    ctrl.taskPriority = 1;
                    ctrl.managerFilter = {
                        Comparers: [],
                    };
                    ctrl.filter = {
                        Comparers: [],
                    };
                    ctrl.formInited = true;
                } else {
                    ctrl.showEventObject = false;
                    ctrl.getRule(ctrl.id);
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getRule = function (id) {
            $http.get('bizprocessrules/get', { params: { id: id, eventType: ctrl.eventType, rnd: Math.random() } }).then(function (response) {
                var data = response.data;
                if (data != null) {
                    ctrl.eventName = data.EventName;
                    ctrl.eventObjId = data.EventObjId;
                    ctrl.taskDueDateInterval = ctrl.intervalOrDefault(data.TaskDueDateInterval, 'taskDueDateIntervalNotSet');
                    ctrl.taskCreateInterval = ctrl.intervalOrDefault(data.TaskCreateInterval, 'taskCreateIntervalNotSet');
                    ctrl.priority = data.Priority;
                    ctrl.taskName = data.TaskName;
                    ctrl.taskDescription = data.TaskDescription;
                    ctrl.taskPriority = data.TaskPriority;
                    ctrl.taskGroupId = data.TaskGroupId;
                    ctrl.managerFilter = data.ManagerFilter || { Comparers: [] };
                    ctrl.filter = data.Filter || { Comparers: [] };
                }
                ctrl.serializeManagerFilter();
                ctrl.serializeFilter();
                ctrl.addEditRuleForm.$setPristine();
                ctrl.formInited = true;
            });
        };

        ctrl.save = function () {
            ctrl.btnSleep = true;
            ctrl.serializeManagerFilter();
            ctrl.serializeFilter();
            var params = {
                id: ctrl.id,
                eventType: ctrl.eventType,
                eventObjId: ctrl.eventObject != null ? ctrl.eventObject.value : ctrl.eventObjId,
                taskDueDateIntervalSerialized:
                    !ctrl.taskDueDateIntervalNotSet && ctrl.taskDueDateInterval.Interval != null ? JSON.stringify(ctrl.taskDueDateInterval) : null,
                taskCreateIntervalSerialized:
                    !ctrl.taskCreateIntervalNotSet && ctrl.taskCreateInterval.Interval != null ? JSON.stringify(ctrl.taskCreateInterval) : null,
                priority: ctrl.priority,
                taskName: ctrl.taskName,
                taskDescription: ctrl.taskDescription,
                taskPriority: ctrl.taskPriority,
                taskGroupId: ctrl.taskGroupId,
                managerFilterSerialized: ctrl.managerFilterSerialized,
                filterSerialized: ctrl.filterSerialized,
                rnd: Math.random(),
            };

            var url = ctrl.mode == 'add' ? 'bizprocessrules/add' : 'bizprocessrules/update';

            $http.post(url, params).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.pop(
                        'success',
                        '',
                        ctrl.mode == 'add'
                            ? $translate.instant('Admin.Js.SettingsTasks.RuleAdded')
                            : $translate.instant('Admin.Js.SettingsTasks.ChangesSaved'),
                    );
                    $uibModalInstance.close({ eventType: ctrl.eventType });
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.SettingsTasks.Error'), data.error);
                    ctrl.btnSleep = false;
                }
            });
        };

        ctrl.processManagerFilterRule = function (prevRule, newRule) {
            if (newRule == null) {
                // remove
                SweetAlert.confirm('Вы уверены, что хотите удалить?', { title: 'Удаление' }).then(function (result) {
                    if (result && !result.isDismissed) {
                        ctrl.managerFilter.Comparers = $filter('filter')(ctrl.managerFilter.Comparers, function (comparer) {
                            return comparer !== prevRule;
                        });

                        ctrl.serializeFilter();
                        toaster.pop('success', '', $translate.instant('Admin.Js.Design.SuccessfullyDeleted'));
                    } else if (result === false) {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Catalog.ErrorWhileDeleting'));
                    }
                });
            } else if (prevRule == null) {
                // add
                ctrl.managerFilter.Comparers.push(newRule);
            } else {
                // update
                var ind = ctrl.managerFilter.Comparers.indexOf(prevRule);
                if (ind != -1) {
                    ctrl.managerFilter.Comparers[ind] = newRule;
                }
            }
            ctrl.serializeManagerFilter();
        };

        ctrl.processFilterRule = function (prevRule, newRule) {
            if (newRule == null) {
                // remove
                SweetAlert.confirm('Вы уверены, что хотите удалить?', { title: 'Удаление' }).then(function (result) {
                    if (result && !result.isDismissed) {
                        ctrl.filter.Comparers = $filter('filter')(ctrl.filter.Comparers, function (comparer) {
                            return comparer !== prevRule;
                        });

                        ctrl.serializeFilter();
                        toaster.pop('success', '', $translate.instant('Admin.Js.Design.SuccessfullyDeleted'));
                    } else if (result === false) {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Catalog.ErrorWhileDeleting'));
                    }
                });
            } else if (prevRule == null) {
                // add
                ctrl.filter.Comparers.push(newRule);
            } else {
                // update
                var ind = ctrl.filter.Comparers.indexOf(prevRule);
                if (ind != -1) {
                    ctrl.filter.Comparers[ind] = newRule;
                }
            }
            ctrl.serializeFilter();
        };

        ctrl.serializeManagerFilter = function () {
            ctrl.managerFilterSerialized = ctrl.managerFilter != null ? JSON.stringify(ctrl.managerFilter) : null;
        };

        ctrl.serializeFilter = function () {
            ctrl.filterSerialized = ctrl.filter != null ? JSON.stringify(ctrl.filter) : null;
        };

        ctrl.intervalOrDefault = function (value, flagNotSet) {
            ctrl[flagNotSet] = value == null;
            return value || { IntervalType: ctrl.intervalTypes[0].value };
        };

        ctrl.getFormData = function () {
            return $http
                .get('bizprocessrules/getruleformdata', { params: { eventType: ctrl.eventType, rnd: Math.random() } })
                .then(function (response) {
                    var data = response.data;
                    if (data != null) {
                        ctrl.eventName = data.eventName;
                        ctrl.eventObjectGroups = data.eventObjectGroups;
                        ctrl.eventObjects = data.eventObjects;
                        ctrl.eventObjectsFetchUrl = data.eventObjectsFetchUrl;
                        ctrl.intervalTypes = data.intervalTypes;
                        ctrl.filterTypeName = data.filterTypeName;
                        ctrl.taskPriorities = data.taskPriorities;
                        ctrl.taskGroups = data.taskGroups;
                        ctrl.availableVariables = data.availableVariables;
                        ctrl.useFilter = data.useFilter;
                        if (ctrl.eventObjectGroups) ctrl.eventObjectGroup = ctrl.eventObjectGroups[0].value;
                    }
                });
        };

        ctrl.getManagerFilterHtml = function (comparer) {
            var result;
            if (comparer.FilterType == 2) result = comparer.CustomerName;
            else if (comparer.FilterType == 3) {
                switch (ctrl.eventType) {
                    case 'OrderCreated':
                    case 'OrderStatusChanged':
                        result = $translate.instant('Admin.Js.SettingsTasks.OrderManager');
                        break;
                    case 'LeadCreated':
                    case 'LeadStatusChanged':
                        result = $translate.instant('Admin.Js.SettingsTasks.LeadManager');
                        break;
                    case 'MessageReply':
                        result = $translate.instant('Admin.Js.SettingsTasks.BuyersManager');
                        break;
                    case 'TaskCreated':
                    case 'TaskStatusChanged':
                        result = $translate.instant('Admin.Js.SettingsTasks.TaskManager');
                        break;
                }
            } else {
                result = comparer.FilterTypeName;
                var rules = [];
                if (comparer.ManagerRoleId != null) rules.push($translate.instant('Admin.Js.SettingsTasks.Role') + comparer.ManagerRoleName);
                if (comparer.City) rules.push($translate.instant('Admin.Js.SettingsTasks.City') + comparer.City);
                if (rules.length > 0) {
                    result += ' (' + rules.join(', ') + ')';
                }
            }
            return result;
        };

        ctrl.getEventObjects = function (evenObjectGroup) {
            if (!ctrl.eventObjectsFetchUrl) return;
            return $http.get(ctrl.eventObjectsFetchUrl + evenObjectGroup, { params: { rnd: Math.random() } }).then(function (response) {
                ctrl.eventObjects = response.data;
            });
        };
    };

    ModalAddEditRuleCtrl.$inject = ['$http', '$filter', '$uibModalInstance', 'SweetAlert', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalAddEditRuleCtrl', ModalAddEditRuleCtrl);
})(window.angular);
