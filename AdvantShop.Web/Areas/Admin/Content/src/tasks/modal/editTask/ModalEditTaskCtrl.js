import finishTaskTemplate from '../finishTask/finishTask.html';
import editTaskObserverTemplate from '../editTaskObserver/editTaskObserver.html';

(function (ng) {
    const ModalEditTaskCtrl = /* @ngInject */ function (
        $document,
        $uibModalInstance,
        $location,
        $uibModal,
        SweetAlert,
        toaster,
        Upload,
        tasksService,
        $q,
        $translate,
        $window,
        $scope,
        $timeout,
        leadService,
        lastStatisticsService,
        isMobileService,
        urlHelper,
    ) {
        const ctrl = this;
        let ckeditorObj;

        const getTaskNumberForToasterMessege = (taskId) => ` <a href="tasks/view/${taskId}">№${taskId}</a> `;

        ctrl.$onInit = function () {
            ctrl.localStorageKey = `adminTaskButtonAction_${urlHelper.transformBaseUriToKey()}`;

            //remove old static key
            localStorage.removeItem('adminTaskButtonAction');

            ctrl.formInited = false;
            ctrl.needRefresh = false;
            ctrl.isMobile = isMobileService.getValue();

            ctrl.ckeditor = {
                height: 150,
                extraPlugins: 'clicklinkbest,ace,lineheight,autolinker,autogrow,pastefromgdocs',
                bodyClass: 'm-n textarea-padding',
                toolbar: {},
                toolbarGroups: {},
                resize_enabled: false,
                toolbar_emptyToolbar: { name: 'empty', items: [] },
                autoGrow_minHeight: 233,
                autoGrow_onStartup: true,
                on: {
                    instanceReady(event) {
                        ckeditorObj = event.editor;
                        $document[0].getElementById(`${event.editor.id}_top`).style.display = 'none';
                    },
                    focus(event) {
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

            //$location.search('grid', null);
            $location.search('viewed', 'true'); // prevent scroll to page top after modal close
            $location.search('modal', ctrl.$resolve.id);
            $location.search('modalOpened', 'true');
            ctrl.managerIds = [];

            const search = $location.search();
            if (search.needRefresh != null) {
                ctrl.needRefresh = true;
            }

            tasksService
                .getFormData(ctrl.$resolve.id)
                .then((data) => {
                    if (data != null) {
                        ctrl.currentManagerId = data.currentManagerId;
                        ctrl.managersAssign = data.managersAssign;
                        ctrl.managersAppoint = data.managersAppoint;
                        ctrl.managersObserve = data.managersObserve;
                        ctrl.taskGroups = data.taskGroups;
                        ctrl.priorities = data.priorities;
                        ctrl.filesHelpText = data.filesHelpText;
                        ctrl.leadDealStatuses = data.leadDealStatuses;
                        ctrl.reminderTypes = data.reminderTypes;
                        ctrl.reminderActive = data.reminderActive;
                        ctrl.projectStatuses = data.projectStatuses;
                        ctrl.statusTypeDict = data.statusTypeDict;

                        ctrl.canAssingToMe = ctrl.managersAssign.some((item) => item.value == ctrl.currentManagerId);
                    }

                    return tasksService.getTask(ctrl.$resolve.id).then((result) => {
                        if (result.result === false) {
                            ctrl.close();
                            if (result.errors != null) {
                                result.errors.forEach((err) => {
                                    toaster.error('', err);
                                });
                            }
                        }

                        ctrl.formInited = true;
                        ctrl.id = result.Id;
                        ctrl.name = result.Name;
                        ctrl.dueDate = result.DueDate;
                        ctrl.description = result.Description;
                        ctrl.managerIds = result.ManagerIds;
                        ctrl.appointedManagerId = result.AppointedManagerId;
                        ctrl.taskGroupId = result.TaskGroupId;
                        ctrl.taskGroupIdInSelect = result.TaskGroupId;
                        ctrl.priority = result.Priority;
                        ctrl.dateAppointedFormatted = result.DateAppointedFormattedFull;
                        ctrl.statusId = result.StatusId;
                        ctrl.statusType = result.StatusType;
                        ctrl.statusName = result.StatusFormatted;
                        ctrl.accepted = result.Accepted;
                        ctrl.acceptedOrCanceled = result.StatusType === 'Accepted' || result.StatusType === 'Canceled';
                        ctrl.orderId = result.OrderId;
                        ctrl.orderNumber = result.OrderNumber;
                        ctrl.leadId = result.LeadId;
                        ctrl.leadTitle = result.LeadTitle;
                        ctrl.leadSalesFunnelId = result.LeadSalesFunnelId;
                        ctrl.leadDealStatusId = result.LeadDealStatusId;
                        ctrl.reviewId = result.ReviewId;
                        ctrl.bindedTaskId = result.BindedTaskId;
                        ctrl.clientCustomerId = result.ClientCustomerId;
                        ctrl.clientName = result.ClientName;
                        ctrl.canDelete = result.CanDelete;
                        ctrl.result = result.ResultFull;
                        ctrl.editTaskForm.$setPristine();
                        ctrl.taskUrl = `${window.location.origin + window.location.pathname.replace('leads', 'tasks')}?modal=${ctrl.id}`;
                        ctrl.commentsType = result.IsPrivateComments ? 'taskHidden' : 'task';
                        ctrl.isAutomatic = result.IsAutomatic;
                        ctrl.isReadonlyTask = result.IsReadonlyTask;
                        ctrl.remind = result.Remind;
                        ctrl.reminder = result.Remind == false ? ctrl.reminderTypes[0].value : result.Reminder;
                        ctrl.observerIds = result.ObserverIds;
                        ctrl.observingTask = ctrl.observerIds.includes(ctrl.currentManagerId);
                    });
                })
                .then(() => {
                    ctrl.buttonActions = {
                        save: {
                            fn: ctrl.chain.bind(ctrl, ctrl.saveTask),
                            text: $translate.instant('Admin.Js.Tasks.EditTask.Save'),
                        },
                        saveAndClose: {
                            fn: ctrl.chain.bind(ctrl, ctrl.saveTask, ctrl.close),
                            text: $translate.instant('Admin.Js.Tasks.EditTask.SaveAndClose'),
                        },
                    };

                    const buttonActionNameDefault = ctrl.getButtonActionNameDefault(ctrl.statusId);
                    ctrl.buttonActionDefault = ctrl.buttonActions[buttonActionNameDefault];
                    ctrl.buttonActionsCurrent = ctrl.getButtonActionsByState(buttonActionNameDefault);
                    ctrl.buttonActionsCurrentCount = Object.keys(ctrl.buttonActionsCurrent).length;
                    setStatusButtons(ctrl.projectStatuses, ctrl.statusId, ctrl.statusType);
                });

            tasksService.getTaskAttachments(ctrl.$resolve.id).then((result) => {
                ctrl.attachments = result;
            });

            $window.onbeforeunload = ctrl.warningMissingData;

            $scope.$on('modal.closing', ($event, reason, closed) => {
                let defer = $q.defer(),
                    promise;

                if (
                    (ctrl.editTaskForm.modified === true && closed === false && (reason == null || reason.notCheck !== true)) ||
                    (ctrl.adminCommentsCtrl.form.text != null &&
                        ctrl.adminCommentsCtrl.form.text.length > 0 &&
                        closed === false &&
                        (reason == null || reason.notCheck !== true))
                ) {
                    $event.preventDefault();
                    promise = $timeout(() =>
                        SweetAlert.confirm(
                            $translate.instant(
                                ctrl.adminCommentsCtrl.form.text != null && ctrl.adminCommentsCtrl.form.text.length > 0
                                    ? 'Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingComment'
                                    : 'Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingData',
                            ),
                            { title: $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingTitle') },
                        ).then((result) => {
                            if (result?.value === true) {
                                if (closed === true) {
                                    ctrl.close();
                                } else {
                                    ctrl.dismiss(true);
                                }
                            }
                            return result;
                        }, 100),
                    );
                } else {
                    promise = defer.promise.then(() => ctrl.clearParams());
                    defer.resolve(null);
                }

                promise.then((result) => {
                    if (result == null || result.value === true) {
                        $location.search('modalOpened', null);
                        $window.onbeforeunload = null;
                    }
                });
            });
        };

        ctrl.warningMissingData = function () {
            if (ctrl.editTaskForm.modified === true || (ctrl.adminCommentsCtrl.form.text != null && ctrl.adminCommentsCtrl.form.text.length > 0)) {
                return $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingData');
            }
        };

        ctrl.getUiSelectCtrl = function (uiSelectCtrl) {
            ctrl.uiSelectCtrl = uiSelectCtrl;
        };

        ctrl.copy = function (data) {
            const input = document.createElement('input');
            input.setAttribute('value', data);
            input.style.opacity = 0;
            document.body.appendChild(input);
            input.select();
            if (document.execCommand('copy')) {
                toaster.success($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.LinkCopiedToClipboard'));
            } else {
                toaster.error($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.FailedToCopyLink'));
            }
            document.body.removeChild(input);
        };

        ctrl.dismiss = function (notCheck) {
            if (notCheck === true) {
                ctrl.clearParams();
            }

            $uibModalInstance.dismiss({ refresh: ctrl.needRefresh, notCheck });
        };

        ctrl.close = function (params) {
            ctrl.clearParams();
            $uibModalInstance.close({ ...params, refresh: true });
            return true;
        };

        ctrl.clearParams = function () {
            $location.search('needRefresh', null);
            $location.search('modal', null);
        };

        ctrl.changeStatus = function (status, withoutMessageComplete) {
            const { id: statusId, statusType } = status;
            return tasksService.changeTaskStatus(ctrl.id, statusId).then((response) => {
                if (withoutMessageComplete !== true) {
                    if (statusId == ctrl.projectStatuses[0].id) {
                        toaster.success(
                            '',
                            `${$translate.instant('Admin.Js.Tasks.Tasks.Task')} <a  href="tasks/view/${ctrl.id}">№${ctrl.id}</a> ${$translate.instant(
                                'Admin.Js.EditTask.Opened',
                            )}`,
                        );
                    } else if (statusId == 'inprogress') {
                        toaster.success(
                            '',
                            `${$translate.instant('Admin.Js.Tasks.Tasks.Task')} <a  href="tasks/view/${ctrl.id}">№${ctrl.id}</a> ${$translate.instant(
                                'Admin.Js.EditTask.Started',
                            )}`,
                        );
                    } else {
                        toaster.success($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.StatusChanged'));
                    }
                }

                if (statusType === ctrl.statusTypeDict.Accepted || statusType === ctrl.statusTypeDict.Canceled) {
                    ctrl.close();

                    if (ctrl.isMobile) {
                        ctrl.popoverIsOpen = false;
                    }

                    return response;
                }
                if (ckeditorObj != null) {
                    ckeditorObj.setReadOnly(false);
                }

                if (ctrl.isMobile) {
                    ctrl.popoverIsOpen = false;
                }

                return ctrl.refreshTask(response);
            });
        };

        ctrl.changeStatusByList = function (statusList, withoutMessageComplete) {
            return $q.when(statusList.length === 1 ? statusList[0] : tasksService.selectStatusByList(statusList)).then((status) => {
                if (status != null) {
                    return ctrl.changeStatus(status, withoutMessageComplete);
                }
            });
        };

        ctrl.finishTask = function (taskId, status) {
            let _status;

            if (status.data != null) {
                _status = status.data;
            } else {
                _status = status;
            }

            const isArrayStatus = Array.isArray(_status);
            if (status == null || (isArrayStatus && _status.length > 1)) {
                $uibModal
                    .open({
                        bindToController: true,
                        controller: 'ModalFinishTaskCtrl',
                        controllerAs: 'ctrl',
                        templateUrl: finishTaskTemplate,
                        resolve: { id: taskId },
                    })
                    .result.then((result) => {
                        ctrl.close();
                    });
            } else {
                const { statusType } = isArrayStatus ? _status[0] : _status;
                if (statusType === 3 || statusType === 4) {
                    $q.when(statusType === 3 ? tasksService.acceptTask(taskId) : statusType === 4 ? tasksService.cancelTask(taskId) : null).then(
                        (response) => {
                            if (response.result === true) {
                                toaster.pop('success', '', 'Изменения сохранены');
                                lastStatisticsService.getLastStatistics();
                                ctrl.close();
                            } else {
                                toaster.pop('error', '', 'Невозможно завершить задачу');
                            }
                        },
                    );
                }
            }
        };

        ctrl.loadTask = function (taskId) {
            return tasksService.getTask(taskId).then((result) => $q.resolve(ctrl.refreshTask(result)));
        };

        ctrl.refreshTask = function (result) {
            if (result.result === false) {
                ctrl.close();
                if (result.errors != null) {
                    result.errors.forEach((err) => {
                        toaster.error('', err);
                    });
                }
            }

            ctrl.statusId = result.StatusId;
            ctrl.statusType = result.StatusType;

            if (result.TaskGroupId != null) {
                ctrl.taskGroupId = ctrl.taskGroupIdInSelect = result.TaskGroupId;
            }

            ctrl.acceptedOrCanceled = ctrl.statusType === 'Accepted' || ctrl.statusType === 'Canceled';
            ctrl.needRefresh = true;
            ctrl.statusName = result.StatusFormatted;

            const buttonActionNameDefault = ctrl.getButtonActionNameDefault(ctrl.statusId);
            ctrl.buttonActionDefault = ctrl.buttonActions[buttonActionNameDefault];
            ctrl.buttonActionsCurrent = ctrl.getButtonActionsByState(buttonActionNameDefault);

            ctrl.result = result.ResultFull;
            setStatusButtons(ctrl.projectStatuses, ctrl.statusId, ctrl.statusType);
        };

        ctrl.changeAssignedManager = function (managerId, single) {
            if (single) {
                ctrl.managerIds = [managerId];
            }
            tasksService
                .changeAssignedManager(ctrl.id, ctrl.managerIds)
                .then((response) => {
                    ctrl.needRefresh = true;
                    toaster.success(
                        '',
                        $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.ExecutorChanged', {
                            number: getTaskNumberForToasterMessege(ctrl.id),
                        }),
                    );
                })
                .finally((response) => {
                    ctrl.getManagers();
                });
        };

        ctrl.changeAppointedManager = function (appointedManagerId) {
            tasksService.changeAppointedManager(ctrl.id, appointedManagerId).then((response) => {
                ctrl.needRefresh = true;
                toaster.success(
                    '',
                    $translate.instant('Admin.Js.Tasks.Tasks.TaskChangesApplied', {
                        number: getTaskNumberForToasterMessege(ctrl.id),
                        field: $translate.instant('Admin.Js.Tasks.EditTask.TaskManager'),
                    }),
                );
            });
        };

        let timerChangeDueDate;

        ctrl.changeDueDate = function (dateObj, dateString) {
            if (timerChangeDueDate != null) {
                clearTimeout(timerChangeDueDate);
            }

            timerChangeDueDate = setTimeout(() => {
                tasksService.changeDueDate(ctrl.id, dateString).then((response) => {
                    ctrl.needRefresh = true;
                    toaster.success(
                        '',
                        $translate.instant('Admin.Js.Tasks.Tasks.TaskChangesApplied', {
                            number: getTaskNumberForToasterMessege(ctrl.id),
                            field: $translate.instant('Admin.Js.Tasks.EditTask.PeriodOfExecution'),
                        }),
                    );
                });
            }, 2000);
        };

        ctrl.changeReminder = function (reminder) {
            const result = reminder != null ? tasksService.changeReminder(ctrl.id, reminder) : tasksService.removeReminder(ctrl.id);

            result.then((response) => {
                ctrl.needRefresh = true;
                toaster.success(
                    '',
                    $translate.instant('Admin.Js.Tasks.Tasks.TaskChangesApplied', {
                        number: getTaskNumberForToasterMessege(ctrl.id),
                        field: $translate.instant('Admin.Js.Tasks.EditTask.Reminder'),
                    }),
                );
            });
        };

        ctrl.changePriority = function (priority) {
            tasksService.changePriority(ctrl.id, priority).then((response) => {
                ctrl.needRefresh = true;
                toaster.success(
                    '',
                    $translate.instant('Admin.Js.Tasks.Tasks.TaskChangesApplied', {
                        number: getTaskNumberForToasterMessege(ctrl.id),
                        field: $translate.instant('Admin.Js.Tasks.EditTask.Priority'),
                    }),
                );
            });
        };

        ctrl.changeTaskGroup = async function (taskGroupId) {
            const oldStatusType = ctrl.statusTypeDict[ctrl.statusType];
            const { result: isSuccessOperation, obj: availableStatuses } = await tasksService.getAvailableStatusesByTaskGroupId(taskGroupId);

            if (!isSuccessOperation) {
                ctrl.taskGroupIdInSelect = ctrl.taskGroupId;
                toaster.error(``, $translate.instant('Admin.Js.ErrorWhileSaving'));
                return;
            }

            const listStatusByType = availableStatuses.filter((x) => x.statusType === oldStatusType);
            let equalStatus = null;
            let isFoundedAvalableStatuses = false;

            if (listStatusByType.length > 0) {
                equalStatus = listStatusByType.find((x) => x.name === ctrl.statusName);
                isFoundedAvalableStatuses = true;
            }
            const statusSelected =
                equalStatus || (await tasksService.selectStatusByList(isFoundedAvalableStatuses ? listStatusByType : availableStatuses, true));

            //отменили смену статуса
            if (statusSelected == null) {
                ctrl.taskGroupIdInSelect = ctrl.taskGroupId;
                toaster.error($translate.instant('Admin.Js.ErrorWhileSaving'));
                return;
            }

            tasksService
                .changeTaskGroup({ taskId: ctrl.id, taskGroupId, taskGroupStatusId: statusSelected.id })
                .then((response) => (response.result ? $q.resolve(response) : $q.reject(response)))
                .then(() => ctrl.setModelVariables(ctrl.taskGroupIdInSelect, statusSelected.statusId, statusSelected.statusType, availableStatuses))
                .then((response) => ctrl.loadTask(ctrl.id))
                .then((response) => {
                    ctrl.needRefresh = true;

                    //setStatusButtons(ctrl.projectStatuses, ctrl.statusId, ctrl.statusType);
                    toaster.success(
                        '',
                        $translate.instant('Admin.Js.Tasks.Tasks.TaskChangesApplied', {
                            number: getTaskNumberForToasterMessege(ctrl.id),
                            field: $translate.instant('Admin.Js.Tasks.EditTask.Project'),
                        }),
                    );

                    return response;
                })
                .catch((response) => {
                    response.errors.forEach((error) => {
                        toaster.error(error);
                    });
                    ctrl.taskGroupIdInSelect = ctrl.taskGroupId;
                });
        };

        ctrl.setModelVariables = function (taskGroupId, statusId, statusType, projectStatuses) {
            ctrl.taskGroupId = taskGroupId;
            ctrl.statusId = statusId;
            ctrl.statusType = statusType;
            ctrl.projectStatuses = projectStatuses;
        };

        ctrl.changeObserver = function (managerId, single) {
            if (single) {
                if (ctrl.observingTask) ctrl.observerIds.splice(ctrl.observerIds.indexOf(managerId), 1);
                else ctrl.observerIds.push(managerId);
            }
            tasksService
                .changeObserver(ctrl.id, ctrl.observerIds)
                .then((response) => {
                    ctrl.needRefresh = true;
                    ctrl.observingTask = ctrl.observerIds.includes(ctrl.currentManagerId);
                    toaster.success(
                        '',
                        `${$translate.instant('Admin.Js.Tasks.Tasks.Task')} <a  href="tasks/view/${ctrl.id}">№${ctrl.id}</a> ${$translate.instant(
                            'Admin.Js.Tasks.ModalEditTaskCtrl.ObserverChanged',
                        )}`,
                    );
                })
                .finally((response) => {
                    ctrl.getManagers();
                });
        };

        ctrl.completeTask = function (statusList) {
            return tasksService
                .completeTaskShowModal({
                    id: ctrl.id,
                    name: ctrl.name,
                    leadId: ctrl.leadId,
                    orderId: ctrl.orderId,
                    taskGroupId: ctrl.taskGroupId,
                    taskResult: ctrl.result,
                    statusList,
                })
                .then((result) => {
                    if (result !== false) {
                        return ctrl.loadTask(ctrl.id);
                    }
                    return result;
                })
                .catch((result) => $q.reject(result))
                .finally(() => (ctrl.popoverIsOpen = false));
        };

        ctrl.acceptTask = function () {
            return tasksService.acceptTask(ctrl.id).then((response) => {
                toaster.success(
                    '',
                    `${$translate.instant('Admin.Js.Tasks.Tasks.Task')} <a  href="tasks/view/${ctrl.id}">№${ctrl.id}</a> ${$translate.instant(
                        'Admin.Js.EditTask.Accepted',
                    )}`,
                );
                return response;
            });
        };

        ctrl.deleteTask = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.Deleting'),
            }).then((result) => {
                if (result === true || result.value) {
                    tasksService.deleteTask(ctrl.id).then((data) => {
                        if (data.result === true) {
                            ctrl.close();
                            toaster.success('', $translate.instant('Admin.Js.Tasks.Tasks.HasBeenDeleted', { number: ` №${ctrl.id}` }));
                        } else {
                            data.errors.forEach((error) => {
                                toaster.error(error);
                            });
                        }
                    });
                }
            });
        };

        ctrl.copyTask = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.AreYouSureCopy'), {
                title: $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.Copy'),
            }).then((result) => {
                if (result === true || result.value) {
                    tasksService.copyTask(ctrl.id).then((data) => {
                        if (data.result === true) {
                            toaster.success(
                                '',
                                $translate.instant('Admin.Js.Tasks.Tasks.TaskHasBeenCopied', {
                                    source: `<a href="tasks/view/${ctrl.id}">№${ctrl.id}</a> `,
                                    dest: `<a href="tasks/view/${data.taskId}">№${data.taskId}</a> `,
                                }),
                            );
                            ctrl.needRefresh = true;
                            ctrl.close({
                                modal: data.taskId,
                            });
                        } else {
                            data.errors.forEach((error) => {
                                toaster.error(error);
                            });
                        }
                    });
                }
            });
        };

        ctrl.saveTask = function () {
            ctrl.btnSleep = true;

            return ctrl
                .validateTaskData()
                .then((result) => {
                    if (!result) {
                        ctrl.btnSleep = false;
                        return $q.reject();
                    }
                })
                .then(() => ctrl.validateTaskGroupManager())
                .then((result) => {
                    if (!result) {
                        ctrl.btnSleep = false;
                        return $q.reject();
                    }
                })
                .then(() => ctrl.validateTaskGroupObserver())
                .then((result) =>
                    $timeout(
                        () =>
                            // задержка нужна для ckeditor чтоб проинициализировались данные
                            ({
                                id: ctrl.id,
                                name: ctrl.name,
                                managerIds: ctrl.managerIds,
                                appointedManagerId: ctrl.appointedManagerId,
                                dueDate: ctrl.dueDate,
                                description: ctrl.description,
                                taskGroupId: ctrl.taskGroupId,
                                priority: ctrl.priority,
                                statusId: ctrl.statusId,
                                accepted: ctrl.accepted,
                                resultFull: ctrl.result,
                                remind: ctrl.remind,
                                reminder: ctrl.reminder,
                                observerIds: ctrl.observerIds,
                            }),
                        300,
                    ),
                )
                .then((objTask) => tasksService.editTask(objTask))
                .then((result) => {
                    ctrl.editTaskForm.$setPristine();
                    ctrl.showSaveMessage();
                    ctrl.btnSleep = false;
                    return result;
                });
        };

        ctrl.uploadAttachment = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            ctrl.loadingFiles = true;
            if (($event.type === 'change' || $event.type === 'drop') && $files != null && $files.length > 0) {
                Upload.upload({
                    url: 'tasks/uploadAttachments',
                    data: {
                        taskId: ctrl.$resolve.id,
                    },
                    file: $files,
                }).then((response) => {
                    const { data } = response;
                    for (const i in response.data) {
                        if (data[i].Result === true) {
                            ctrl.attachments.push(data[i].Attachment);
                            toaster.success(
                                $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.File') +
                                    data[i].Attachment.OriginFileName +
                                    $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.WasAdded'),
                            );
                        } else {
                            toaster.error(
                                $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.ErrorLoading'),
                                (data[i].Attachment != null ? `${data[i].Attachment.OriginFileName}: ` : '') + data[i].Error,
                            );
                        }
                    }
                    ctrl.loadingFiles = false;
                });
            } else if ($invalidFiles.length > 0) {
                toaster.error(
                    $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.ErrorLoading'),
                    $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.FileNotMeetRequirements'),
                );
                ctrl.loadingFiles = false;
            } else {
                ctrl.loadingFiles = false;
            }
        };

        ctrl.deleteAttachment = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.AreYouSureDeleteFile'), {
                title: $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.Deleting'),
            }).then((result) => {
                if (result === true || result.value) {
                    tasksService.deleteAttachment(id, ctrl.$resolve.id).then((response) => {
                        tasksService.getTaskAttachments(ctrl.$resolve.id).then((result) => {
                            ctrl.attachments = result;
                        });
                    });
                }
            });
        };

        ctrl.flatpickrOnSetup = function (fpItem) {
            ctrl.flatpickr = fpItem;
        };

        ctrl.flatpickrOnChange = function (selectedDates, dateStr, instance) {
            if (selectedDates.length == 0) return;
            const date = new Date(selectedDates[0]);
            const minutes = Math.floor(date.getMinutes() / 10) * 10;
            date.setMinutes(minutes);
            ctrl.dueDate = `${date.getFullYear()}-${`0${date.getMonth() + 1}`.slice(-2)}-${`0${date.getDate()}`.slice(
                -2,
            )}T${`0${date.getHours()}`.slice(-2)}:${`0${date.getMinutes()}`.slice(-2)}`;
        };

        ctrl.validateTaskGroupManager = function () {
            if (ctrl.taskGroupId == null || ctrl.managerIds == null || ctrl.managerIds.length == 0) {
                return $q.resolve(true);
            }
            return tasksService.validateTaskGroupManager(ctrl.managerIds, ctrl.taskGroupId).then((data) => {
                if (data.errors != null) {
                    data.errors.forEach((err) => {
                        toaster.error('Ошибка сохранения исполнителей', err);
                    });
                    return false;
                }
                return true;
            });
        };

        ctrl.validateTaskGroupObserver = function () {
            if (ctrl.taskGroupId == null || ctrl.observerIds == null || ctrl.observerIds.length == 0) {
                return $q.resolve(true);
            }
            return tasksService.validateTaskGroupManager(ctrl.observerIds, ctrl.taskGroupId).then((data) => {
                if (data.errors != null) {
                    data.errors.forEach((err) => {
                        toaster.warning('Предупреждение при сохранении наблюдателей', err);
                    });
                }
                return true;
            });
        };

        ctrl.validateTaskData = function () {
            return tasksService.validateTaskData(ctrl.appointedManagerId, ctrl.taskGroupId).then((data) => {
                if (data.errors != null) {
                    data.errors.forEach((err) => {
                        toaster.error('', err);
                    });
                    return false;
                }
                return true;
            });
        };

        ctrl.getManagers = function () {
            tasksService.getTaskManagers(ctrl.id, ctrl.taskGroupId).then((result) => {
                if (result != null) {
                    ctrl.managersAssign = result.managersAssign;
                    ctrl.managersAppoint = result.managersAppoint;
                    ctrl.managersObserve = result.managersObserve;

                    ctrl.canAssingToMe = ctrl.managersAssign.some((item) => item.value == ctrl.currentManagerId);
                }
            });
        };

        ctrl.changeLeadDealStatus = function () {
            if (ctrl.leadId == null || ctrl.leadDealStatusId == null) {
                return;
            }
            leadService.changeDealStatus(ctrl.leadId, ctrl.leadDealStatusId).then((data) => {
                if (data.result == true) {
                    toaster.success(
                        $translate.instant('Admin.Js.Leads.TransactionStageChanged'),
                        data.obj != null && data.obj.orderId != null
                            ? `${$translate.instant('Admin.Js.CompleteTask.OrderCreated')} <a href="orders/edit/${data.obj.orderId}">${
                                  data.obj.orderNumber
                              }</a>`
                            : '',
                    );
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };

        ctrl.chain = function () {
            return ctrl.chainProcess(arguments, 0);
        };

        ctrl.chainProcess = function (fnList, index) {
            return $q.when(fnList[index] != null ? fnList[index]() : true).then(() => {
                const newIndex = index + 1;
                return newIndex < fnList.length ? ctrl.chainProcess(fnList, newIndex) : null;
            });
        };

        ctrl.taskOpen = function () {
            return ctrl.changeTaskStatusByStatusType(ctrl.statusTypeDict.Open);
        };
        ctrl.taskInProgress = function () {
            return ctrl.changeTaskStatusByStatusType(ctrl.statusTypeDict.InProgerss);
        };

        ctrl.acceptTask = function () {
            return ctrl.changeTaskStatusByStatusType(ctrl.statusTypeDict.Accepted);
        };

        ctrl.changeTaskStatusByStatusType = function (statusType) {
            const ckeditorReadonly = statusType === ctrl.statusTypeDict.Accepted;
            const task = ctrl.projectStatuses.find((x) => x.statusType === statusType);

            if (ckeditorObj != null) {
                ckeditorObj.setReadOnly(ckeditorReadonly);
            }
            if (task != null) {
                return ctrl.changeStatus(task);
            }

            return null;
        };

        ctrl.callAndSaveButtonAction = function (buttonActionName) {
            if (ctrl.buttonActions[buttonActionName] != null) {
                ctrl.buttonActions[buttonActionName].fn();
                const storageData = JSON.parse($window.localStorage.getItem(ctrl.localStorageKey)) || {};

                storageData[ctrl.statusId] = buttonActionName;

                $window.localStorage.setItem(ctrl.localStorageKey, JSON.stringify(storageData));

                ctrl.buttonActionDefault = ctrl.buttonActions[buttonActionName];
                ctrl.buttonActionsCurrent = ctrl.getButtonActionsByState(buttonActionName);
            } else {
                throw Error('Not found buttonActionName');
            }
        };

        ctrl.getButtonActionsByState = function (actionDefault) {
            const result = {};

            Object.keys(ctrl.buttonActions).forEach((key) => {
                if (key !== actionDefault) {
                    result[key] = ctrl.buttonActions[key];
                }
            });

            return result;
        };

        ctrl.getButtonActionNameDefault = function (status) {
            let result = `save`;
            const storageData = JSON.parse($window.localStorage.getItem(ctrl.localStorageKey));

            if (storageData != null && storageData[status] != null) {
                if (ctrl.buttonActions[storageData[status]] == null) {
                    //remove old actions button's from storage
                    $window.localStorage.removeItem(ctrl.localStorageKey);
                } else {
                    result = storageData[status];
                }
            }
            return result;
        };

        ctrl.addAdminCommentsCtrl = function (adminCommentsCtrl) {
            ctrl.adminCommentsCtrl = adminCommentsCtrl;
        };

        ctrl.saveAndCheckUnsaveData = function (callbacks) {
            if (ctrl.adminCommentsCtrl.form.text != null && ctrl.adminCommentsCtrl.form.text.length > 0) {
                $timeout(() =>
                    SweetAlert.confirm($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingComment'), {
                        title: $translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingTitle'),
                    }).then((result) => {
                        if (result != null && result === true) {
                            callbacks.func(callbacks.arg);
                        }
                    }, 100),
                );
            } else {
                callbacks.func(callbacks.arg);
            }
        };

        ctrl.showSaveMessage = function () {
            toaster.pop(
                'success',
                '',
                `${$translate.instant('Admin.Js.Tasks.Tasks.Task')} <a  href="tasks/view/${ctrl.id}">№${ctrl.id}</a> ${$translate.instant(
                    'Admin.Js.EditTask.Saved',
                )}`,
            );

            return $q.resolve();
        };

        function setStatusButtons(statusList, statusId, statusType) {
            if (statusList === null) {
                return;
            }

            const statusGroup = [];
            switch (statusType) {
                case 'Open':
                    statusGroup.push({
                        text: 'Следующий этап',
                        data: statusList.filter((x) => x.statusType === ctrl.statusTypeDict.Open && x.id !== statusId),
                    });
                    statusGroup.push({
                        text: $translate.instant('Admin.Js.Tasks.EditTask.StartExecution'),
                        data: statusList.filter((x) => x.statusType === ctrl.statusTypeDict.InProgress && x.id !== statusId),
                    });
                    statusGroup.push({
                        text: $translate.instant('Admin.Js.Tasks.EditTask.Complete'),
                        data: statusList.filter((x) => x.statusType === ctrl.statusTypeDict.Completed && x.id !== statusId),
                        fn: async (statusListItem, selectedIndex) => {
                            await ctrl.completeTask(selectedIndex != null ? [statusListItem.data[selectedIndex]] : statusListItem.data);
                        },
                    });
                    break;
                case 'InProgress':
                    statusGroup.push({
                        text: $translate.instant('Admin.Js.Tasks.EditTask.Pause'),
                        data: statusList.filter((x) => x.statusType === ctrl.statusTypeDict.Open && x.id !== statusId),
                    });
                    statusGroup.push({
                        text: 'Продолжить выполнение',
                        data: statusList.filter((x) => x.statusType === ctrl.statusTypeDict.InProgress && x.id !== statusId),
                    });
                    statusGroup.push({
                        text: $translate.instant('Admin.Js.Tasks.EditTask.Complete'),
                        data: statusList.filter((x) => x.statusType === ctrl.statusTypeDict.Completed && x.id !== statusId),
                        fn: async (statusListItem, selectedIndex) => {
                            await ctrl.completeTask(selectedIndex != null ? [statusListItem.data[selectedIndex]] : statusListItem.data);
                        },
                    });
                    break;
                case 'Completed':
                    statusGroup.push({
                        text: $translate.instant('Admin.Js.Tasks.EditTask.Resume'),
                        data: statusList.filter((x) => x.statusType === ctrl.statusTypeDict.Open && x.id !== statusId),
                    });
                    statusGroup.push({
                        text: 'Следующий этап',
                        data: statusList.filter((x) => x.statusType === ctrl.statusTypeDict.Completed && x.id !== statusId),
                        fn: async (statusListItem, selectedIndex) => {
                            await ctrl.completeTask(selectedIndex != null ? [statusListItem.data[selectedIndex]] : statusListItem.data);
                        },
                    });
                    statusGroup.push({
                        text: $translate.instant('Admin.Js.Tasks.EditTask.Accept'),
                        data: statusList.filter((x) => x.statusType >= ctrl.statusTypeDict.Accepted && x.id !== statusId),
                        actionButton: true,
                    });
                    break;
                case 'Accepted':
                    statusGroup.push({
                        text: $translate.instant('Admin.Js.Tasks.EditTask.Resume'),
                        data: statusList.filter((x) => x.statusType === ctrl.statusTypeDict.Open && x.id !== statusId),
                    });
                    statusGroup.push({
                        text: 'Отклонить',
                        data: statusList.filter((x) => x.statusType >= ctrl.statusTypeDict.Accepted && x.id !== statusId),
                        actionButton: true,
                    });
                    break;
                case 'Canceled':
                    statusGroup.push({
                        text: $translate.instant('Admin.Js.Tasks.EditTask.Resume'),
                        data: statusList.filter((x) => x.statusType === ctrl.statusTypeDict.Open && x.id !== statusId),
                    });
                    statusGroup.push({
                        text: $translate.instant('Admin.Js.Tasks.EditTask.Accept'),
                        data: statusList.filter((x) => x.statusType >= ctrl.statusTypeDict.Accepted && x.id !== statusId),
                        actionButton: true,
                    });
                    break;
            }

            ctrl.statusGroup = statusGroup;
        }

        ctrl.pressButtonChangeStatus = function (statusListItem, selectedIndex) {
            if (statusListItem.fn != null) {
                statusListItem.fn(statusListItem, selectedIndex);
            } else {
                ctrl.changeStatusByList(selectedIndex != null ? [statusListItem.data[selectedIndex]] : statusListItem.data);
            }
        };

        ctrl.edittaskObservers = function () {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalEditTaskObserverCtrl',
                    controllerAs: 'ctrl',
                    size: 'lg',
                    templateUrl: editTaskObserverTemplate,
                    resolve: {
                        params: {
                            parent: ctrl,
                        },
                    },
                })
                .result.then((result) => {
                    if (result != null) {
                        ctrl.changeObserver(result);
                    }
                });
        };
    };

    ng.module('uiModal').controller('ModalEditTaskCtrl', ModalEditTaskCtrl);
})(window.angular);
