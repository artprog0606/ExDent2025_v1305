import addEditSmsTemplateOnOrderChangingTpl from './modal/addEditSmsTemplateOnOrderChanging/AddEditSmsTemplateOnOrderChanging.html';
(function (ng) {
    'use strict';

    var SettingsSmsCtrl = function ($location, $window, uiGridConstants, uiGridCustomConfig, $q, SweetAlert, $http, $uibModal, $translate, toaster) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'OrderStatusName',
                    displayName: 'Статус заказа',
                    width: 230,
                    filter: {
                        placeholder: 'Статус заказа',
                        type: uiGridConstants.filter.SELECT,
                        name: 'OrderStatusId',
                        fetch: 'orders/getOrderStatuses',
                    },
                },
                {
                    name: 'SmsText',
                    displayName: 'Текст sms',
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.News.Title'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'SmsText',
                    },
                },
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.News.SheActive'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row" field-name="Enabled" class="js-grid-not-clicked"></ui-grid-custom-switch>',
                    width: 90,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.News.Activity'),
                        name: 'Enabled',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.News.TheyActive'),
                                value: true,
                            },
                            {
                                label: $translate.instant('Admin.Js.News.Inactive'),
                                value: false,
                            },
                        ],
                    },
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 80,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<ui-modal-trigger data-controller="\'ModalAddEditSmsTemplateOnOrderChangingCtrl\'" controller-as="ctrl" size="lg" ' +
                        'template-url="' +
                        addEditSmsTemplateOnOrderChangingTpl +
                        '" ' +
                        'data-resolve="{value:{\'id\': row.entity.Id }}"' +
                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                        '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></button> ' +
                        '</ui-modal-trigger>' +
                        '<ui-grid-custom-delete url="settingsSms/deleteTemplate" params="{\'id\': row.entity.Id}"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="settingsSms/deleteTemplate" params="{\'id\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.News.DeleteSelected'),
                        url: 'settingsSms/deleteTemplates',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.News.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.News.Deleting'),
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

        ctrl.sendTestSms = function (phone, text) {
            $http.post('settingsSms/sendTestSms', { phone: phone, text: text }).then(function (response) {
                var data = response.data;
                if (data.result) {
                    toaster.pop('success', '', 'Собщение отправлено');
                } else {
                    data.errors.forEach(function (e) {
                        toaster.pop('error', '', e);
                    });
                }
            });
        };
    };

    SettingsSmsCtrl.$inject = [
        '$location',
        '$window',
        'uiGridConstants',
        'uiGridCustomConfig',
        '$q',
        'SweetAlert',
        '$http',
        '$uibModal',
        '$translate',
        'toaster',
    ];

    ng.module('settingsSms', ['uiGridCustom', 'urlHelper'])
        .controller('SettingsSmsCtrl', SettingsSmsCtrl)
        .component('countSms', {
            bindings: {
                smsText: '<',
            },
            template: `
                    <span>
                        {{'Admin.Js.SmsCount.Characters'|translate}}: {{ $ctrl.LengthText }} ({{ $ctrl.SmsCount }} SMS)
                    </span>`,
            controller: function CountSmsCtrl() {
                this.$onInit = function () {
                    this.setCountSms();
                };
                this.$onChanges = function () {
                    this.setCountSms();
                };
                this.setCountSms = function () {
                    var text = this.smsText;
                    var textLength = text == null || text == undefined ? 0 : text.length;
                    var CountSms = 0;
                    /* eslint-disable no-control-regex*/
                    var patternASCII = /^[\x00-\x7F]*$/;
                    var pattern = /[{}\[\]~^\\|]/;
                    if ((!patternASCII.test(text) || pattern.test(text)) && textLength > 70) CountSms = Math.ceil(textLength / 67);
                    else if (patternASCII.test(text) && textLength > 160) CountSms = Math.ceil(textLength / 153);
                    else if (textLength > 0) CountSms = 1;

                    this.SmsCount = CountSms;
                    this.LengthText = textLength;
                };
            },
        });
})(window.angular);
