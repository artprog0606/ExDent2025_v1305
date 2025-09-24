import projectstatusesTemplate from './projectStatuses.html';
(function (ng) {
    'use strict';

    var ProjectStatusesCtrl = function ($http, $filter, $timeout, $q, toaster, SweetAlert, $translate) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.fetch().then(() => {
                // #region colorPicker
                ctrl.colorPickerOptions = {
                    swatchBootstrap: false,
                    format: 'hex',
                    alpha: false,
                    swatchOnly: false,
                    case: 'lower',
                    allowEmpty: true,
                    required: false,
                    preserveInputFormat: false,
                    restrictToFormat: false,
                    inputClass: 'form-control',
                };
                ctrl.colorPickerEventApi = {
                    onBlur: function () {
                        ctrl.colorPickerApi.getScope().AngularColorPickerController.update();
                    },
                };
                // #endregion
            });
        };
        ctrl.fetch = function () {
            var url = ctrl.taskGroupId != 0 ? 'taskgroups/getProjectStatuses' : 'taskgroups/getDefaultProjectStatuses';
            return $http
                .get(url, {
                    params: {
                        taskGroupId: ctrl.taskGroupId,
                    },
                })
                .then(function (response) {
                    var data = response.data;
                    ctrl.items = data.items || [];
                    ctrl.systemItems = data.systemItems || [];
                    ctrl.statusTypeList = data.statusTypeList || [];
                    if (ctrl.onChange != null) {
                        ctrl.onChange({
                            items: data.items,
                            systemItems: data.systemItems,
                            statusTypeList: data.statusTypeList,
                        });
                    }
                    ctrl.addEmpty();
                });
        };
        ctrl.getNextColor = function () {
            return tinycolor.random().toHexString().slice(1);
        };
        ctrl.addEmpty = function () {
            ctrl.newColor = ctrl.getNextColor();
            //$timeout(function () {
            if (ctrl.colorPickerApi) ctrl.colorPickerApi.getScope().AngularColorPickerController.setNgModel(ctrl.newColor);
            //})
        };
        ctrl.sortableOptions = {
            orderChanged: function (event) {
                if (ctrl.taskGroupId === 0) {
                    return;
                }
                var id = event.source.itemScope.item.Id,
                    prevId = event.dest.index - 1,
                    nextId = event.dest.index + 1;
                ctrl.changeStatuSorting(id, prevId, nextId);
            },
        };
        ctrl.changeStatuSorting = function (id, prevIndex, nextIndex, showToaster = true) {
            if (ctrl.taskGroupId === 0) {
                return false;
            }
            var prev = ctrl.items[prevIndex],
                next = ctrl.items[nextIndex];

            // выбираем тип статуса, по карям - ок, после пустого - плохо
            var prevStatusType = prev != null ? (prev.Id != null ? prev.StatusType : 100) : -1;
            var nextStatusType = next != null ? (next.Id != null ? next.StatusType : -1) : 100;
            var currentStatusType = ctrl.items.find((x) => x.Id === id).StatusType;

            // если нельзя поменять местами, просим изначальный список с бека
            if (currentStatusType > nextStatusType || currentStatusType < prevStatusType) {
                return ctrl.fetch();
            } else {
                return $http
                    .post('taskgroups/changeProjectStatusSorting', {
                        statusList: ctrl.items,
                    })
                    .then(function (response) {
                        if (response.data.result === true) {
                            if (showToaster) {
                                toaster.success('', $translate.instant('Admin.Js.TaskStatuses.ChangesSaved'));
                            }
                        }
                        return response.data.result;
                    });
            }
        };
        ctrl.deleteItem = function (item) {
            if (ctrl.taskGroupId === 0) {
                var index = ctrl.items.indexOf(item);
                if (index !== -1) {
                    ctrl.items.splice(index, 1);
                }
                return;
            }
            SweetAlert.confirm($translate.instant('Admin.Js.TaskStatuses.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.SettingsCrm.Delete'),
            }).then(function (result) {
                if (result === true || result.value === true) {
                    $http
                        .post('taskgroups/deleteProjectStatus', {
                            id: item.Id,
                            taskGroupId: ctrl.taskGroupId,
                        })
                        .then(function (response) {
                            if (response.data.result === true) {
                                ctrl.fetch();
                                toaster.success('', $translate.instant('Admin.Js.TaskStatuses.ChangesSaved'));
                            }
                        });
                }
            });
        };
        ctrl.addItem = function () {
            if (!ctrl.newName) return;
            var item = {
                Id: 0,
                Name: ctrl.newName,
                Color: ctrl.newColor,
                StatusType: 1,
                taskGroupId: ctrl.taskGroupId,
            };
            if (ctrl.taskGroupId === 0) {
                ctrl.items.push(item);
                ctrl.addEmpty();
                ctrl.newName = '';
                return;
            }
            $http.post('taskgroups/addProjectStatus', item).then(function (response) {
                if (response.data.result === true) {
                    item.Id = response.data.obj;
                    ctrl.items.push(item);
                    ctrl.newName = '';
                    var index = ctrl.items.findLastIndex((x) => x.StatusType === item.StatusType) + 1;
                    ctrl.changeStatuSorting(response.data.obj, index, index + 1, false).then(function (result) {
                        toaster.success('', $translate.instant('Admin.Js.TaskStatuses.ChangesSaved'));
                    });
                    ctrl.addEmpty();
                }
            });
        };
        ctrl.onEditProjectStatus = function (prev, edited, isSystemStatus) {
            if (ctrl.taskGroupId === 0) {
                var index = (isSystemStatus ? ctrl.systemItems : ctrl.items).indexOf(prev);
                if (index !== -1) {
                    (isSystemStatus ? ctrl.systemItems : ctrl.items)[index] = edited;
                }
                return;
            }
            ctrl.fetch();
        };
        ctrl.inplaceStatus = function (item) {
            $http.post('taskgroups/updateProjectStatus', item).then(function (response) {
                if (response.data.result === true) {
                    var currentIndex = ctrl.items.findIndex((x) => x.Id === item.Id);
                    ctrl.items.splice(currentIndex, 1);
                    var newIndex = ctrl.items.findLastIndex((x) => x.StatusType === item.StatusType) + 1;
                    ctrl.items.splice(newIndex, 0, item);
                    ctrl.changeStatuSorting(item.Id, newIndex, newIndex + 1, false);
                }
            });
        };
    };
    ProjectStatusesCtrl.$inject = ['$http', '$filter', '$timeout', '$q', 'toaster', 'SweetAlert', '$translate'];
    ng.module('taskgroups')
        .controller('ProjectStatusesCtrl', ProjectStatusesCtrl)
        .component('projectStatuses', {
            templateUrl: projectstatusesTemplate,
            controller: 'ProjectStatusesCtrl',
            transclude: true,
            bindings: {
                taskGroupId: '<',
                onChange: '&',
            },
        });
})(window.angular);
