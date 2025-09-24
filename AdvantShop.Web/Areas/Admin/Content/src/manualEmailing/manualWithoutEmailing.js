(function (ng) {
    'use strict';

    /* @ngInject */
    var ManualWithoutEmailingCtrl = function ($http, $translate, uiGridConstants, uiGridCustomConfig, $q, SweetAlert) {
        var ctrl = this;

        ctrl.init = function (id) {
            ctrl.id = id;
            //ctrl.fetch();
        };

        //ctrl.fetch = function () {
        //    var params = {
        //        id: ctrl.id
        //    };
        //    $http.get('emailings/getManualWithoutEmailingAnalytics', { params: { id: ctrl.id } }).then(function (response) {
        //        ctrl.data = response.data.obj;
        //    });
        //}

        var columnDefs = [
            {
                name: 'Subject',
                displayName: $translate.instant('Admin.Js.ManualEmailings.Subject'),
                filter: {
                    placeholder: $translate.instant('Admin.Js.ManualEmailings.Subject'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Subject',
                },
            },
            {
                name: 'TotalCount',
                displayName: $translate.instant('Admin.Js.ManualEmailings.TotalCount'),
                width: 90,
                filter: {
                    placeholder: $translate.instant('Admin.Js.ManualEmailings.TotalCount'),
                    type: 'range',
                    rangeOptions: {
                        from: { name: 'TotalCountFrom' },
                        to: { name: 'TotalCountTo' },
                    },
                },
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 50,
                enableSorting: false,
                cellTemplate:
                    '<div class="ui-grid-cell-contents"><div class="js-grid-not-clicked">' +
                    '<ui-grid-custom-delete url="emailings/deleteManualEmailings" params="{\'Ids\': row.entity.Id}"></ui-grid-custom-delete>' +
                    '</div></div>',
            },
        ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowUrl: 'emailings/manualemailing/{{row.entity.Id}}',
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.ManualEmailings.DeleteSelected'),
                        url: 'emailings/deleteManualEmailings',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.ManualEmailings.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.ManualEmailings.Deleting'),
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

    ng.module('manualWithoutEmailing', []).controller('ManualWithoutEmailingCtrl', ManualWithoutEmailingCtrl);
})(window.angular);
