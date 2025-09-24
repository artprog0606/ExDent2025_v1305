(function (ng) {
    'use strict';

    var CustomerSegmentsCtrl = function (
        $location,
        $window,
        uiGridConstants,
        uiGridCustomConfig,
        uiGridCustomParamsConfig,
        uiGridCustomService,
        toaster,
        SweetAlert,
        $http,
        $q,
        $translate,
    ) {
        var ctrl = this,
            url = document.location.pathname.toLowerCase().indexOf('customersegmentscrm') >= 0 ? 'customersegmentscrm' : 'customersegments',
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.CustomerSegments.Name'),
                    filter: {
                        placeholder: $translate.instant('Admin.Js.CustomerSegments.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                    cellTemplate: '<div class="ui-grid-cell-contents"><a ng-href="' + url + '/edit/{{row.entity.Id}}">{{COL_FIELD}}</a></div>',
                },
                {
                    name: 'CustomersCount',
                    displayName: $translate.instant('Admin.Js.CustomerSegments.AmountOfCustomers'),
                    width: 120,
                    enableCellEdit: false,
                    cellTemplate: '<div class="ui-grid-cell-contents"><div class="p-l-sm">{{COL_FIELD}}</div></div>',
                },
                {
                    name: 'CreatedDateFormatted',
                    displayName: $translate.instant('Admin.Js.CustomerSegments.DateOfCreation'),
                    width: 150,
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
                        '<a ng-href="' +
                        url +
                        '/edit/{{row.entity.Id}}" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></a>' +
                        '<ui-grid-custom-delete url="' +
                        url +
                        '/deleteSegment" params="{\'id\': row.entity.Id}"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="' +
                        url +
                        '/deleteSegment" params="{\'id\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.CustomerSegments.DeleteSelected'),
                        url: url + '/deleteSegments',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.CustomerSegments.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.CustomerSegments.Deleting'),
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

    CustomerSegmentsCtrl.$inject = [
        '$location',
        '$window',
        'uiGridConstants',
        'uiGridCustomConfig',
        'uiGridCustomParamsConfig',
        'uiGridCustomService',
        'toaster',
        'SweetAlert',
        '$http',
        '$q',
        '$translate',
    ];

    ng.module('customerSegments', ['uiGridCustom', 'urlHelper']).controller('CustomerSegmentsCtrl', CustomerSegmentsCtrl);
})(window.angular);
