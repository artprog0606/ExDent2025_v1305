(function (ng) {
    'use strict';

    var PropertyValuesCtrl = function (
        $location,
        $window,
        uiGridConstants,
        uiGridCustomConfig,
        uiGridCustomParamsConfig,
        uiGridCustomService,
        toaster,
        $q,
        SweetAlert,
        $translate,
        $http,
    ) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.PropertyValues.Name'),
                    enableCellEdit: true,
                    uiGridCustomEdit: {
                        replaceNullable: false,
                    },
                    filter: {
                        placeholder: $translate.instant('Admin.Js.PropertyValues.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },
                {
                    name: 'Value',
                    displayName: $translate.instant('Admin.Js.PropertyValues.Value'),
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.PropertyValues.Value'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Search',
                    },
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.PropertyValues.SortOrder'),
                    width: 80,
                    enableCellEdit: true,
                },
                {
                    name: 'ProductsCount',
                    displayName: $translate.instant('Admin.Js.PropertyValues.UseWithProducts'),
                    width: 80,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"> <a ng-href="catalog?showMethod=AllProducts&grid=%7B%22PropertyId%22:{{row.entity.PropertyId}},%22PropertyValueId%22:{{row.entity.PropertyValueId}}%7D">{{ COL_FIELD }}</a></div>',
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 50,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate: uiGridCustomService.getTemplateCellDelete(
                        'propertyvalues/deletePropertyValue',
                        "{'propertyValueId': row.entity.PropertyValueId }",
                    ),
                    //cellTemplate:
                    //    '<div class="ui-grid-cell-contents"><div>' +
                    //        '<ui-grid-custom-delete url="propertyvalues/deletePropertyValue" params="{\'propertyValueId\': row.entity.PropertyValueId}"></ui-grid-custom-delete>' +
                    //    '</div></div>'
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.PropertyValues.DeleteSelected'),
                        url: 'propertyvalues/deletePropertyValues',
                        field: 'PropertyValueId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.PropertyValues.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.PropertyValues.Deleting'),
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

        ctrl.initPropertyGroups = function (propertyGroups) {
            ctrl.propertyGroups = propertyGroups;
        };

        ctrl.updatePropertyGroups = function (result) {
            toaster.pop('success', '', $translate.instant('Admin.Js.PropertyValues.PropertyValueAdded'));
            ctrl.propertyGroups.fetch();
        };

        ctrl.getPropertyValue = function (propertyId) {
            return $http.get('properties/GetProperty?propertyId=' + propertyId).then(function (response) {
                var data = response.data;
                if (data != null) {
                    ctrl.selectedProperty = data;
                }
            });
        };

        ctrl.showPropertyValues = function (propertyId) {
            ctrl.getPropertyValue(propertyId).then(function () {
                //ctrl.grid.setParams({ groupId: ctrl.selectedProperty.PropertyId });
                //ctrl.grid.fetchData();
            });
        };

        ctrl.updateSelectedPropertyName = function (result) {
            ctrl.selectedProperty.Name = result;
        };
    };

    PropertyValuesCtrl.$inject = [
        '$location',
        '$window',
        'uiGridConstants',
        'uiGridCustomConfig',
        'uiGridCustomParamsConfig',
        'uiGridCustomService',
        'toaster',
        '$q',
        'SweetAlert',
        '$translate',
        '$http',
    ];

    ng.module('propertyvalues', ['uiGridCustom', 'urlHelper', 'propertyGroups']).controller('PropertyValuesCtrl', PropertyValuesCtrl);
})(window.angular);
