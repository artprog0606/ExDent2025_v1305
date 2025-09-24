import addEditCertificatesTemplate from './modal/addEditCertificates/addEditCertificates.html';
(function (ng) {
    'use strict';

    var CertificatesCtrl = function (
        $location,
        $window,
        uiGridConstants,
        uiGridCustomConfig,
        $q,
        SweetAlert,
        $http,
        $uibModal,
        $translate,
        isMobileService,
    ) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'CertificateCode',
                    displayName: $translate.instant('Admin.Js.Certificates.CertificateCode'),
                    enableCellEdit: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div style="text-align:left">' +
                        '<ui-modal-trigger data-controller="\'ModalAddEditCertificatesCtrl\'" controller-as="ctrl" ' +
                        'template-url="' +
                        addEditCertificatesTemplate +
                        '" ' +
                        'data-resolve="{\'id\': row.entity.CertificateId}" ' +
                        'data-on-close="grid.appScope.$ctrl.fetchData()" ' +
                        'data-size="xs-9"> ' +
                        '<a href="">{{COL_FIELD}}</a>' +
                        '</ui-modal-trigger></div></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Certificates.CertificateCode'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'CertificateCode',
                    },
                },
                {
                    name: 'OrderId',
                    displayName: $translate.instant('Admin.Js.Certificates.OrderNumber'),
                    enableCellEdit: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><a class="link-invert" ng-href="orders/edit/{{row.entity.OrderId}}">{{COL_FIELD}}</a></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Certificates.OrderNumber'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'OrderId',
                    },
                },
                {
                    name: 'ApplyOrderNumber',
                    displayName: $translate.instant('Admin.Js.Certificates.OrderNumberUsed'),
                    enableCellEdit: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><a class="link-invert" ng-href="orders/edit/{{row.entity.ApplyOrderNumber}}">{{COL_FIELD}}</a></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Certificates.OrderNumberUsed'),
                        name: 'ApplyOrderNumber',
                        type: uiGridConstants.filter.INPUT,
                    },
                },
                {
                    name: 'FullSum',
                    displayName: $translate.instant('Admin.Js.Certificates.Sum'),
                    enableCellEdit: false,
                    width: 100,
                    cellTemplate: '<div class="ui-grid-cell-contents"><div style="text-align:left">{{COL_FIELD}}</div></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Certificates.Sum'),
                        name: 'Sum',
                        type: uiGridConstants.filter.INPUT,
                    },
                },
                {
                    name: 'OrderCertificatePaid',
                    displayName: $translate.instant('Admin.Js.Certificates.Paid'),
                    enableCellEdit: false,
                    width: 80,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div style="pointer-events: none;text-align:center;"><label class="adv-checkbox-label">' +
                        '<input type="checkbox" ng-model="row.entity.OrderCertificatePaid" class="adv-checkbox-input control-checkbox" />' +
                        '<span class="adv-checkbox-emul"></span></label></div></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Certificates.Paid'),
                        name: 'Paid',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.Certificates.Yes'),
                                value: true,
                            },
                            {
                                label: $translate.instant('Admin.Js.Certificates.No'),
                                value: false,
                            },
                        ],
                    },
                },
                {
                    name: 'Enable',
                    displayName: $translate.instant('Admin.Js.Certificates.Available'),
                    enableCellEdit: false,
                    width: 80,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div style="pointer-events: none;text-align:center;"><label class="adv-checkbox-label">' +
                        '<input type="checkbox" ng-model="row.entity.Enable" class="adv-checkbox-input control-checkbox" />' +
                        '<span class="adv-checkbox-emul"></span></label></div></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Certificates.Available'),
                        name: 'Enable',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.Certificates.Yes'),
                                value: true,
                            },
                            {
                                label: $translate.instant('Admin.Js.Certificates.No'),
                                value: false,
                            },
                        ],
                    },
                },
                {
                    name: 'Used',
                    displayName: $translate.instant('Admin.Js.Certificates.Used'),
                    enableCellEdit: false,
                    width: 110,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div style="pointer-events: none;text-align:center;"><label class="adv-checkbox-label">' +
                        '<input type="checkbox" ng-model="row.entity.Used" class="adv-checkbox-input control-checkbox" />' +
                        '<span class="adv-checkbox-emul"></span></label></div></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Certificates.Used'),
                        name: 'Used',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.Certificates.Yes'),
                                value: true,
                            },
                            {
                                label: $translate.instant('Admin.Js.Certificates.No'),
                                value: false,
                            },
                        ],
                    },
                },
                {
                    name: 'CreationDates',
                    displayName: $translate.instant('Admin.Js.Certificates.DateOfCreation'),
                    width: 140,
                    visible: 1441,
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Certificates.DateOfCreation'),
                        type: 'datetime',
                        term: {
                            from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                            to: new Date(),
                        },
                        datetimeOptions: {
                            from: {
                                name: 'CreationDateFrom',
                            },
                            to: {
                                name: 'CreationDateTo',
                            },
                        },
                    },
                },
                {
                    name: '_serviceColumn',
                    enableHiding: false,
                    enableSorting: false,
                    displayName: '',
                    useInSwipeBlock: true,
                    width: 90,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<ui-modal-trigger data-controller="\'ModalAddEditCertificatesCtrl\'" controller-as="ctrl" ' +
                        'template-url="' +
                        addEditCertificatesTemplate +
                        '" ' +
                        'data-resolve="{\'id\': row.entity.CertificateId}" ' +
                        'data-on-close="grid.appScope.$ctrl.fetchData()" ' +
                        'data-size="xs-9"> ' +
                        '<button type="button" class="btn-icon ui-grid-custom-service-icon fas fa-pencil-alt" style="cursor:pointer" aria-label="Редактировать"></button>' +
                        '</ui-modal-trigger>' +
                        '<a class="link-invert" target="_blank" ng-href="../giftcertificate/print?code={{row.entity.CertificateCode}}" style="cursor:pointer"><span class="fa fa-print"></span></a>' +
                        '<ui-grid-custom-delete url="Certificates/DeleteCertificates" params="{\'Ids\': row.entity.CertificateId}"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        //'<div ng-if="grid.appScope.$ctrl.isMobile">' +
                        //'<ui-modal-trigger data-controller="\'ModalAddEditCertificatesCtrl\'" controller-as="ctrl" ' +
                        //    'template-url="../areas/admin/content/src/certificates/modal/addEditCertificates/addEditCertificates.html" ' +
                        //    'data-resolve="{\'id\': row.entity.CertificateId}" ' +
                        //    'data-on-close="grid.appScope.$ctrl.fetchData()" ' +
                        //    'data-size="xs-9"> ' +
                        //    '<a class="btn btn-sm btn-success flex center-xs middle-xs" style="height:100%;">Редактировать</a>' +
                        //'</ui-modal-trigger></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="Certificates/DeleteCertificates" params="{\'Ids\': row.entity.CertificateId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            enableGridMenu: !isMobileService.getValue(),
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Certificates.DeleteSelected'),
                        url: 'Certificates/DeleteCertificates',
                        field: 'CertificateId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Certificates.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.Certificates.Delete'),
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
    };

    CertificatesCtrl.$inject = [
        '$location',
        '$window',
        'uiGridConstants',
        'uiGridCustomConfig',
        '$q',
        'SweetAlert',
        '$http',
        '$uibModal',
        '$translate',
        'isMobileService',
    ];

    ng.module('certificates', ['uiGridCustom', 'urlHelper', 'isMobile']).controller('CertificatesCtrl', CertificatesCtrl);
})(window.angular);
