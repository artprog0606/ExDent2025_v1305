import addEditSmsAnswerTemplateTemplate from './modal/addEditSmsAnswerTemplate/AddEditSmsAnswerTemplate.html';
import addEditMailFormatTemplate from './modal/addEditMailFormat/AddEditMailFormat.html';
import addEditMailAnswerTemplateTpl from './modal/addEditMailAnswerTemplate/AddEditMailAnswerTemplate.html';
(function (ng) {
    'use strict';

    var MailSettingsCtrl = function (
        $location,
        $window,
        uiGridConstants,
        uiGridCustomConfig,
        $q,
        SweetAlert,
        $http,
        toaster,
        $translate,
        $uibModal,
        isMobileService,
    ) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'FormatName',
                    displayName: $translate.instant('Admin.Js.MailSettings.Name'),
                    filter: {
                        placeholder: $translate.instant('Admin.Js.MailSettings.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'FormatName',
                    },
                },
                {
                    name: 'TypeName',
                    displayName: $translate.instant('Admin.Js.MailSettings.TypeOfLetter'),
                    width: 200,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.MailSettings.TypeOfLetter'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'MailFormatTypeID',
                        fetch: 'settingsMail/GetMailFormatTypesSelectOptions',
                    },
                },
                {
                    name: 'Enable',
                    displayName: $translate.instant('Admin.Js.MailSettings.Active'),
                    enableCellEdit: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><ui-grid-custom-switch row="row" field-name="Enable" class="js-grid-not-clicked"></ui-grid-custom-switch></div>',
                    width: 90,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.MailSettings.Activity'),
                        name: 'Enable',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.MailSettings.TheyActive'),
                                value: true,
                            },
                            { label: $translate.instant('Admin.Js.MailSettings.Inactive'), value: false },
                        ],
                    },
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.MailSettings.Sorting'),
                    width: 120,
                    enableCellEdit: true,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 80,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<ui-modal-trigger data-controller="\'ModalAddEditMailFormatCtrl\'" controller-as="ctrl" size="lg" ' +
                        'template-url="' +
                        addEditMailFormatTemplate +
                        '" ' +
                        'data-resolve="{value:{\'MailFormatID\': row.entity.MailFormatID }}"' +
                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                        '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></button> ' +
                        '</ui-modal-trigger>' +
                        '<ui-grid-custom-delete url="settingsMail/deleteMailFormat" params="{\'mailFormatID\': row.entity.MailFormatID}"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="settingsMail/deleteMailFormat" params="{\'mailFormatID\': row.entity.MailFormatID}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ],
            templatesColumnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.MailSettings.Name'),
                    filter: {
                        placeholder: $translate.instant('Admin.Js.MailSettings.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },

                {
                    name: 'Active',
                    displayName: $translate.instant('Admin.Js.MailSettings.Active'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row" field-name="Active" class="js-grid-not-clicked"></ui-grid-custom-switch>',
                    width: 90,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.MailSettings.Activity'),
                        name: 'Active',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.MailSettings.TheyActive'),
                                value: true,
                            },
                            { label: $translate.instant('Admin.Js.MailSettings.Inactive'), value: false },
                        ],
                    },
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.MailSettings.Sorting'),
                    width: 120,
                    enableCellEdit: true,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 80,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<ui-modal-trigger data-controller="\'ModalAddEditMailAnswerTemplateCtrl\'" controller-as="ctrl" size="lg" ' +
                        'template-url="' +
                        addEditMailAnswerTemplateTpl +
                        '" ' +
                        'data-resolve="{value:{\'TemplateId\': row.entity.TemplateId }}"' +
                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                        '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></button> ' +
                        '</ui-modal-trigger>' +
                        '<ui-grid-custom-delete url="settingsMail/deleteMailAnswerTemplate" params="{\'TemplateId\': row.entity.TemplateId}"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="settingsMail/deleteMailAnswerTemplate" params="{\'TemplateId\': row.entity.TemplateId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ],
            smsAnswerTemplatesColumnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.MailSettings.Name'),
                    filter: {
                        placeholder: $translate.instant('Admin.Js.MailSettings.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },

                {
                    name: 'Active',
                    displayName: $translate.instant('Admin.Js.MailSettings.Active'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row" field-name="Active" class="js-grid-not-clicked"></ui-grid-custom-switch>',
                    width: 90,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.MailSettings.Activity'),
                        name: 'Active',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.MailSettings.TheyActive'),
                                value: true,
                            },
                            { label: $translate.instant('Admin.Js.MailSettings.Inactive'), value: false },
                        ],
                    },
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.MailSettings.Sorting'),
                    width: 120,
                    enableCellEdit: true,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 80,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<button type="button" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadSmsAnswerTemplate(row.entity.TemplateId); $event.preventDefault();" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></button> ' +
                        '<ui-grid-custom-delete url="settingsMail/deleteSmsAnswerTemplate" params="{\'TemplateId\': row.entity.TemplateId}"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="settingsMail/deleteSmsAnswerTemplate" params="{\'TemplateId\': row.entity.TemplateId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];

        ctrl.$onInit = function () {
            ctrl.emailsDemo = ['@adv-mail'];
        };

        ctrl.isEmailDemo = function (email) {
            return (
                email != null &&
                ctrl.emailsDemo.some(function (item) {
                    return email.indexOf(item) !== -1;
                })
            );
        };

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.MailSettings.DeleteSelected'),
                        url: 'settingsMail/deleteMailFormats',
                        field: 'MailFormatID',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.MailSettings.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.MailSettings.Deleting'),
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

        ctrl.gridTemplatesOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: templatesColumnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.MailSettings.DeleteSelected'),
                        url: 'settingsMail/deleteMailAnswerTemplates',
                        field: 'TemplateId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.MailSettings.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.MailSettings.Deleting'),
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
        });

        ctrl.gridTemplatesOnInit = function (grid) {
            ctrl.gridTemplates = grid;
        };

        ctrl.gridSmsAnswerTemplatesOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: smsAnswerTemplatesColumnDefs,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadSmsAnswerTemplate(row.entity.TemplateId);
                    $event.preventDefault();
                },
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.MailSettings.DeleteSelected'),
                        url: 'settingsMail/deleteSmsAnswerTemplates',
                        field: 'TemplateId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.MailSettings.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.MailSettings.Deleting'),
                            })
                                .then(function (result) {
                                    return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                                })
                                .catch(function (dismiss) {
                                    if (dismiss !== 'sweetAlertCancel') {
                                        throw dismiss;
                                    }
                                });
                        },
                    },
                ],
            },
        });

        ctrl.gridSmsAnswerTemplatesOnInit = function (grid) {
            ctrl.gridSmsAnswerTemplates = grid;
        };

        ctrl.deleteMailFormat = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.MailSettings.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.MailSettings.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('settingsMail/DeleteMailFormat', { mailFormatID: id }).then(function (response) {
                        $window.location.assign('settings/notifyemails?notifyTab=3');
                    });
                }
            });
        };

        ctrl.mailFormat = {};

        ctrl.getTypeDescription = function (mailFormatTypeId) {
            $http.get('settingsMail/getTypeDescription', { params: { mailFormatTypeId: mailFormatTypeId } }).then(function (response) {
                if (response.data.result) {
                    ctrl.mailFormat.Description = response.data.message;
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.MailSettings.Error'), response.data.error);
                }
            });
        };

        ctrl.sendTestMessageTemplate = function (params, index) {
            ctrl.sendingProgressMessage = index;
            $http
                .post('settingsMailTest/sendTestMessageTemplate', params)
                .then(function (response) {
                    var data = response.data;

                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.MailSettings.EmailSuccessfullySent'));
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', $translate.instant('Admin.Js.MailSettings.ErrorSendingEmail'), error);
                        });
                    }
                })
                .finally(function () {
                    ctrl.sendingProgressMessage = false;
                });
        };

        ctrl.sendTestMessage = function (params) {
            ctrl.sendingProgress = true;
            $http
                .post('settingsMailTest/sendTestMessage', params)
                .then(function (response) {
                    var data = response.data;

                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.MailSettings.EmailSuccessfullySent'));
                        ctrl.notifyemails.emailsettings.To = null;
                        ctrl.notifyemails.emailsettings.Subject = null;
                        ctrl.notifyemails.emailsettings.Body = null;
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', $translate.instant('Admin.Js.MailSettings.ErrorSendingEmail'), error);
                        });
                    }
                })
                .finally(function () {
                    ctrl.sendingProgress = false;
                });
        };

        ctrl.updateStatus = function (confirmDateEmail) {
            $http.post('settingsMail/updateStatus').then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    confirmDateEmail.ConfirmDateEmail = data.obj;
                    toaster.pop(
                        data.obj != null ? 'success' : 'error',
                        '',
                        data.obj != null
                            ? $translate.instant('Admin.Js.MailSettings.UpdateStatusConfirmed')
                            : $translate.instant('Admin.Js.MailSettings.UpdateStatusNotConfirmed'),
                    );
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.MailSettings.ErrorSendingEmail'));
                }

                setTimeout(function () {
                    $window.location.reload();
                }, 1000);
            });
        };

        ctrl.sendValidate = function () {
            $http.post('settingsMail/sendValidate').then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', data.obj);
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', $translate.instant('Admin.Js.MailSettings.ErrorSendingEmail'), error);
                    });
                }
            });
        };

        ctrl.testImap = function () {
            ctrl.sendingImapProgress = true;
            $http
                .post('settingsMailTest/testImap')
                .then(function (response) {
                    var data = response.data;

                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.MailSettings.SuccessImapTest'));
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', $translate.instant('Admin.Js.MailSettings.ErrorImapTest'), error);
                        });
                    }
                })
                .finally(function () {
                    ctrl.sendingImapProgress = false;
                });
        };

        ctrl.onUpdateAddress = function (result) {
            ctrl.notifyemails.emailsettings.FromEmail = result.email;
            ctrl.notifyemails.emailsettings.Login = result.email;
            ctrl.notifyemails.emailsettings.FromName = result.name;
            ctrl.notifyemails.emailsettings.Password = null;
            ctrl.notifyemails.emailsettings.ConfirmDateEmail = null;
            ctrl.notifyemails.emailsettings.PasswordCompare = null;
        };

        ctrl.loadSmsAnswerTemplate = function (temlateId) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddEditSmsAnswerTemplateCtrl',
                    controllerAs: 'ctrl',
                    size: 'lg',
                    templateUrl: addEditSmsAnswerTemplateTemplate,
                    resolve: {
                        params: {
                            id: temlateId,
                        },
                    },
                })
                .result.then(function (result) {
                    ctrl.gridSmsAnswerTemplates.fetchData();
                    return result;
                });
        };

        ctrl.onSelectaTab = function (indexTab) {
            ctrl.tabActiveIndex = indexTab;
        };
    };

    MailSettingsCtrl.$inject = [
        '$location',
        '$window',
        'uiGridConstants',
        'uiGridCustomConfig',
        '$q',
        'SweetAlert',
        '$http',
        'toaster',
        '$translate',
        '$uibModal',
        'isMobileService',
    ];

    ng.module('mailSettings', ['uiGridCustom', 'settingsSms']).controller('MailSettingsCtrl', MailSettingsCtrl);
})(window.angular);
