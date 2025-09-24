import addEditNotificationTemplateForBonusTpl from '../_shared/modal/bonus/NotificationTemplate/AddEditNotificationTemplate.html';
(function (ng) {
    'use strict';

    var NotificationTemplatesCtrl = function (
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
                    name: 'NotificationTypeName',
                    displayName: $translate.instant('Admin.Js.Smstemplates.TypeOfTemplate'),
                    enableSorting: true,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><ui-modal-trigger data-controller="\'ModalAddEditNotificationTemplateCtrl\'" controller-as="ctrl" on-close="grid.appScope.$ctrl.gridExtendCtrl.gridUpdate();" size="md" backdrop="static"' +
                        'template-url="' +
                        addEditNotificationTemplateForBonusTpl +
                        '"' +
                        'resolve="{params:{ NotificationTypeId:row.entity.NotificationType, BonusNotificationMethod:row.entity.NotificationMethod, NotificationTemplateId:row.entity.NotificationId }}"><div class="ui-grid-cell-contents ui-grid-custom-pointer"><a ng-href="">{{COL_FIELD}}</a><p>{{row.entity.NotificationMethodName}}</p></div></ui-modal-trigger></div>',
                },
                {
                    name: 'NotificationBody',
                    displayName: $translate.instant('Admin.Js.Smstemplates.Messages'),
                    enableSorting: true,
                    enableCellEdit: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><ui-modal-trigger data-controller="\'ModalAddEditNotificationTemplateCtrl\'" controller-as="ctrl" on-close="grid.appScope.$ctrl.gridExtendCtrl.gridUpdate();" size="md" backdrop="static"' +
                        'template-url="' +
                        addEditNotificationTemplateForBonusTpl +
                        '"' +
                        'resolve="{params:{ NotificationTypeId:row.entity.NotificationType, BonusNotificationMethod:row.entity.NotificationMethod, NotificationTemplateId:row.entity.NotificationId }}"><div class="ui-grid-cell-contents ui-grid-custom-pointer">{{COL_FIELD}}' +
                        '</div></ui-modal-trigger></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Smstemplates.Messages'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'NotificationBody',
                    },
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<ui-modal-trigger data-controller="\'ModalAddEditNotificationTemplateCtrl\'" controller-as="ctrl" on-close="grid.appScope.$ctrl.gridExtendCtrl.gridUpdate();" size="md" backdrop="static"' +
                        'template-url="' +
                        addEditNotificationTemplateForBonusTpl +
                        '"' +
                        'resolve="{params:{ NotificationTypeId:row.entity.NotificationType, BonusNotificationMethod:row.entity.NotificationMethod, NotificationTemplateId:row.entity.NotificationId }}"><button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></button></ui-modal-trigger>' +
                        '<ui-grid-custom-delete url="notificationTemplates/deleteNotificationTemplate" params="{\'id\': row.entity.NotificationId }"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="notificationTemplates/deleteNotificationTemplate" params="{\'id\': row.entity.NotificationId }" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            enableSorting: false,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Smstemplates.DeleteSelected'),
                        url: 'notificationtemplates/DeleteNotificationTemplateMass',
                        field: 'NotificationId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Smstemplates.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.Smstemplates.Deleting'),
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
            enableRowSelection: true,
            multiSelect: true,
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.gridUpdate = function () {
            ctrl.grid.fetchData();
        };

        var columnlogDefs = [
            {
                name: 'Contact',
                displayName: $translate.instant('Admin.Js.Smstemplates.Number'),
                enableSorting: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Smstemplates.Number'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Contact',
                },
            },
            {
                name: 'Body',
                displayName: $translate.instant('Admin.Js.Smstemplates.Message'),
                enableCellEdit: false,
                enableSorting: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Smstemplates.Message'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Body',
                },
            },
            {
                name: 'State',
                displayName: $translate.instant('Admin.Js.Smstemplates.Status'),
                enableCellEdit: false,
            },
            {
                name: 'Created_Str',
                displayName: $translate.instant('Admin.Js.Smstemplates.Created'),
                enableCellEdit: false,
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 75,
                enableSorting: false,
                //cellTemplate:
                //    '<div class="ui-grid-cell-contents"><div>' +
                //        '<ui-modal-trigger data-controller="\'ModalAddEditSmsTemplateCtrl\'" controller-as="ctrl" size="md" backdrop="static"' +
                //          'template-url="../areas/admin/content/src/_shared/modal/bonus/smstemplate/addeditsmstemplate.html"' +
                //          'resolve="{params:{ SmsTypeId:row.entity.SmsTypeId}}"><a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt"></a></ui-modal-trigger>' +
                //        '<ui-grid-custom-delete url="smstemplates/deletesmstemplate" params="{\'id\': row.entity.SmsTypeId}"></ui-grid-custom-delete>' +
                //    '</div></div>'
            },
        ];

        ctrl.gridlogOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnlogDefs,
            rowHeight: 100,
        });

        ctrl.gridlogOnInit = function (gridlog) {
            ctrl.gridlog = gridlog;
        };

        ctrl.deleteSmsTemplate = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.Smstemplates.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Smstemplates.Deleting'),
            }).then(function (result) {
                if (result === true) {
                    $http.post('notificationtemplates/deleteNotificationTemplate', { id: id }).then(function (response) {
                        ctrl.gridUpdate();
                    });
                }
            });
        };
    };

    NotificationTemplatesCtrl.$inject = [
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

    ng.module('notificationtemplates', ['uiGridCustom', 'urlHelper']).controller('NotificationTemplatesCtrl', NotificationTemplatesCtrl);
})(window.angular);
