(function (ng) {
    'use strict';
    /* @ngInject */
    var ModalAddTaskCtrl = function (
        $document,
        $uibModalInstance,
        $filter,
        $q,
        Upload,
        toaster,
        tasksService,
        $window,
        lastStatisticsService,
        $translate,
        $scope,
        $timeout,
        SweetAlert,
        urlHelper,
    ) {
        var ctrl = this;

        ctrl.chain = function () {
            ctrl.btnLoading = true;
            return ctrl.chainProcess(arguments, 0).finally(() => {
                ctrl.btnLoading = false;
            });
        };

        ctrl.chainProcess = function (fnList, index) {
            return $q.when(fnList[index] != null ? fnList[index]() : true).then(function () {
                const newIndex = index + 1;
                return newIndex < fnList.length ? ctrl.chainProcess(fnList, newIndex) : null;
            });
        };

        ctrl.callAndSaveButtonAction = function (buttonActionName) {
            if (ctrl.buttonActions[buttonActionName] != null) {
                ctrl.buttonActions[buttonActionName].fn();
                var storageData = JSON.parse($window.localStorage.getItem(ctrl.localStorageKey)) || {};

                storageData[ctrl.taskGroupId] = buttonActionName;

                $window.localStorage.setItem(ctrl.localStorageKey, JSON.stringify(storageData));
            } else {
                throw Error('Not found buttonActionName');
            }
        };

        ctrl.getButtonActionNameDefault = function () {
            var result;
            var storageData = JSON.parse($window.localStorage.getItem(ctrl.localStorageKey));

            result = storageData != null ? storageData[ctrl.taskGroupId] || 'add' : 'add';

            return result;
        };

        ctrl.getUiSelectCtrl = function (uiSelectCtrl) {
            ctrl.uiSelectCtrl = uiSelectCtrl;
        };

        ctrl.flatpickrOnSetup = function (fpItem) {
            ctrl.flatpickr = fpItem;
        };

        ctrl.flatpickrOnChange = function (selectedDates, dateStr, instance) {
            if (selectedDates.length == 0) {
                return;
            }
            var date = new Date(selectedDates[0]);
            var minutes = Math.floor(date.getMinutes() / 10) * 10;
            date.setMinutes(minutes);
            ctrl.dueDate = dateStr;
            instance.setDate(date);
        };

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.redirectToTasks = params.redirectToTasks || false;
            if (params.bindTo != null) {
                switch (params.bindTo.type.toLowerCase()) {
                    case 'order':
                        ctrl.orderId = params.bindTo.objId;
                        break;
                    case 'lead':
                        ctrl.leadId = params.bindTo.objId;
                        break;
                    case 'customer':
                        ctrl.clientCustomerId = params.bindTo.objId;
                        break;
                }
            }

            tasksService
                .getFormData(null, params.taskGroupId)
                .then(function (data) {
                    if (data != null) {
                        ctrl.managersAssign = data.managersAssign;
                        (ctrl.currentManagerId = data.currentManagerId), (ctrl.taskGroups = data.taskGroups);
                        ctrl.priorities = data.priorities;
                        ctrl.filesHelpText = data.filesHelpText;
                        ctrl.taskGroupId =
                            params.taskGroupId || (data.taskGroups != null && data.taskGroups.length === 1 ? data.taskGroups[0].value : null); //|| data.defaultTaskGroupId || null;
                        ctrl.reminderTypes = data.reminderTypes;
                        ctrl.remind = false;
                        ctrl.reminder = ctrl.reminderTypes[0].value;
                        ctrl.reminderActive = data.reminderActive;

                        //if (ctrl.taskGroupId != null) {
                        //    var items = ctrl.taskGroups.filter(function (x) { return x.value == ctrl.taskGroupId; });
                        //    if (items != null && items.length == 0) {
                        //        ctrl.taskGroupId = '';
                        //    }
                        //}

                        if (ctrl.priorities.length > 1) {
                            ctrl.priority = ctrl.priorities[1].value;
                        } else if (ctrl.priorities.length > 0) {
                            ctrl.priority = ctrl.priorities[0].value;
                        }

                        ctrl.canAssingToMe = ctrl.managersAssign.some(function (item) {
                            return item.value == ctrl.currentManagerId;
                        });
                    }

                    if (params.userData != null) {
                        ctrl.managerIds = [params.userData.assignedManager];
                    }

                    $timeout(function () {
                        ctrl.formAddTask.$setPristine();
                    });
                })
                .then(() => {
                    ctrl.buttonActionNameDefault = ctrl.getButtonActionNameDefault();
                    ctrl.buttonActionDefault = ctrl.buttonActions[ctrl.buttonActionNameDefault];
                });

            ctrl.attachments = [];
            ctrl.managerIds = [];

            $window.onbeforeunload = ctrl.warningMissingData;

            $scope.$on('modal.closing', function ($event, reason, closed) {
                var defer = $q.defer(),
                    promise;

                if (ctrl.formAddTask.modified === true && closed === false && (reason == null || reason.notCheck !== true)) {
                    $event.preventDefault();
                    promise = $timeout(function () {
                        return SweetAlert.confirm($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingData'), {
                            title: $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingTitle'),
                        }).then(function (result) {
                            return result;
                        }, 100);
                    });
                } else {
                    promise = defer.promise;

                    defer.resolve(false);
                }

                promise.then(function (result) {
                    if ((typeof result === 'boolean' && result !== false) || (typeof result !== 'boolean' && result.isDismissed === false)) {
                        if (closed === false) {
                            ctrl.close(true);
                        }
                    }

                    $window.onbeforeunload = null;
                });
            });

            //remove old static key
            localStorage.removeItem('adminAddTaskButtonAction');

            ctrl.localStorageKey = `adminAddTaskButtonAction_${urlHelper.transformBaseUriToKey()}`;

            ctrl.ckeditor = {
                height: 200,
                extraPlugins: 'clicklinkbest,ace,lineheight,autolinker,autogrow',
                bodyClass: 'm-n textarea-padding',
                toolbar: {},
                toolbarGroups: {},
                resize_enabled: false,
                toolbar_emptyToolbar: { name: 'empty', items: [] },
                autoGrow_minHeight: 233,
                autoGrow_onStartup: true,
                on: {
                    instanceReady: function (event) {
                        $document[0].getElementById(event.editor.id + '_top').style.display = 'none';
                    },
                    focus: function () {
                        if (ctrl.uiSelectCtrl) {
                            ctrl.uiSelectCtrl.close();
                        }

                        if (ctrl.flatpickr) {
                            ctrl.flatpickr.close();
                        }
                    },
                },
                disableNativeSpellChecker: false,
                browserContextMenuOnCtrl: false,
                removePlugins: 'language,liststyle,tabletools,scayt,menubutton,contextmenu,tableselection,elementspath',
            };

            ctrl.buttonActions = {
                add: {
                    fn: ctrl.chain.bind(ctrl, ctrl.addTask),
                    text: $translate.instant('Admin.Js.AddTask.Add'),
                },
                addAndSee: {
                    fn: ctrl.chain.bind(ctrl, ctrl.addTask.bind(ctrl, true)),
                    text: $translate.instant('Admin.Js.AddTask.AddAndSee'),
                },
            };

            ctrl.buttonActionsCount = Object.keys(ctrl.buttonActions).length;
        };

        ctrl.warningMissingData = function () {
            if (ctrl.formAddTask.modified === true) {
                return $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingData');
            }
        };

        ctrl.close = function (notCheck) {
            $uibModalInstance.dismiss({ notCheck: notCheck });
        };

        ctrl.addAttachments = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            ctrl.loadingFiles = true;
            if (($event.type === 'change' || $event.type === 'drop') && $files != null && $files.length > 0) {
                for (var i = 0, len = ctrl.attachments.length; i < len; i++) {
                    if ($filter('filter')($files, { name: ctrl.attachments[i].name }, true)[0] != null) {
                        toaster.pop(
                            'error',
                            $translate.instant('Admin.Js.AddTask.Error'),
                            ctrl.attachments[i].name + $translate.instant('Admin.Js.AddTask.FileAlreadyExist'),
                        );
                        $files = $filter('filter')($files, function (file) {
                            return file.name !== ctrl.attachments[i].name;
                        });
                        ctrl.loadingFiles = false;
                        return;
                    }
                }
                Upload.upload({
                    url: 'tasks/validateAttachments',
                    data: {},
                    file: $files,
                }).then(function (response) {
                    var data = response.data;
                    for (var i = 0, len = data.length; i < len; i++) {
                        if (data[i].Result === true) {
                            ctrl.attachments.push($files[i]);
                        } else {
                            toaster.pop(
                                'error',
                                $translate.instant('Admin.Js.AddTask.Error'),
                                (data[i].Attachment != null ? data[i].Attachment.OriginFileName + ': ' : '') + data[i].Error,
                            );
                        }
                    }
                    ctrl.loadingFiles = false;
                });
            } else if ($invalidFiles.length > 0) {
                toaster.pop('error', $translate.instant('Admin.Js.AddTask.ErrorLoading'), $translate.instant('Admin.Js.AddTask.FileDoesNotMeet'));
                ctrl.loadingFiles = false;
            } else {
                ctrl.loadingFiles = false;
            }
        };

        ctrl.deleteAttachment = function (index) {
            ctrl.attachments.splice(index, 1);
        };

        ctrl.addTask = function (seeAfterAdding) {
            return ctrl.validateTaskGroupManager().then(function (result) {
                if (!result) {
                    ctrl.btnLoading = false;
                    return;
                }

                return $timeout(function () {
                    // задержка нужна для ckeditor чтоб проинициализировались данные
                    return Upload.upload({
                        url: 'tasks/addTask',
                        data: {
                            name: ctrl.name,
                            managerIds: ctrl.managerIds,
                            dueDate: ctrl.dueDate,
                            description: ctrl.description,
                            taskGroupId: ctrl.taskGroupId,
                            priority: ctrl.priority,
                            orderId: ctrl.orderId,
                            leadId: ctrl.leadId,
                            clientCustomerId: ctrl.clientCustomerId,
                            remind: ctrl.remind,
                            reminder: ctrl.reminder,
                        },
                        file: ctrl.attachments,
                    }).then(function (response) {
                        var data = response.data;
                        if (data.result === true) {
                            toaster.pop(
                                'success',
                                '',
                                $translate.instant('Admin.Js.AddTask.Task') +
                                    ' <a href="tasks/view/' +
                                    data.id +
                                    '">№' +
                                    data.id +
                                    '</a> ' +
                                    $translate.instant('Admin.Js.AddTask.Added'),
                            );

                            $window.onbeforeunload = null;
                            let resultOnClose;

                            if (ctrl.redirectToTasks && $window.location.pathname.split('/').pop() != 'tasks') {
                                $window.location.assign('tasks');
                            } else if (seeAfterAdding === true) {
                                resultOnClose = {
                                    taskId: data.id,
                                };
                            }

                            $uibModalInstance.close(resultOnClose);

                            lastStatisticsService.getLastStatistics();
                        } else {
                            if (data.errors != null) {
                                data.errors.forEach(function (err) {
                                    toaster.pop('error', '', err);
                                });
                            } else {
                                toaster.pop('error', $translate.instant('Admin.Js.AddTask.Error'), data.Error);
                            }
                        }
                        return data;
                    });
                }, 300);
            });
        };

        ctrl.validateTaskGroupManager = function () {
            if (ctrl.taskGroupId == null || ctrl.managerIds == null || ctrl.managerIds.length == 0) {
                return $q.resolve(true);
            }
            return tasksService.validateTaskGroupManager(ctrl.managerIds, ctrl.taskGroupId).then(function (data) {
                if (data.errors != null) {
                    data.errors.forEach(function (err) {
                        toaster.pop('error', '', err);
                    });
                    return false;
                }
                return true;
            });
        };

        ctrl.getManagers = function () {
            tasksService.getTaskManagers(null, ctrl.taskGroupId).then(function (result) {
                if (result != null) {
                    ctrl.managersAssign = result.managersAssign;
                    ctrl.canAssingToMe = ctrl.managersAssign.some(function (item) {
                        return item.value == ctrl.currentManagerId;
                    });
                }
            });
        };
    };

    ng.module('uiModal').controller('ModalAddTaskCtrl', ModalAddTaskCtrl);
})(window.angular);
