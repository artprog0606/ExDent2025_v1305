/* @ngInject */
export default function DomainGeoLocationListCtrl(
    $q,
    uiGridConstants,
    uiGridCustomConfig,
    uiGridCustomParamsConfig,
    uiGridCustomService,
    SweetAlert,
    domainGeoLocationService,
    $translate,
) {
    var ctrl = this;

    ctrl.gridOptions = angular.extend({}, uiGridCustomConfig, {
        columnDefs: [
            {
                name: 'Url',
                displayName: $translate.instant('Admin.Js.DomainGeoLocationList.Url'),
                enableCellEdit: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.DomainGeoLocationList.UrlFilter'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Url',
                },
            },
            {
                name: 'Cities',
                displayName: $translate.instant('Admin.Js.DomainGeoLocationList.Cities'),
                enableCellEdit: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.DomainGeoLocationList.CitiesFilter'),
                    type: uiGridConstants.filter.SELECT,
                    name: 'CityId',
                    fetch: 'cities/getCitiesList',
                    dynamicSearch: true,
                },
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 75,
                enableSorting: false,
                useInSwipeBlock: true,
                cellTemplate:
                    '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div class="js-grid-not-clicked">' +
                    '<button type="button" ng-click="grid.appScope.$ctrl.gridExtendCtrl.showModal(row.entity.Id); $event.preventDefault();" class="btn-icon ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="{{\'Admin.Js.DomainGeoLocationList.Edit\' | translate}}"></button>' +
                    '<ui-grid-custom-delete url="warehouse/deleteDomainGeoLocation" params="{\'Id\': row.entity.Id}"></ui-grid-custom-delete></div></div>' +
                    '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="warehouse/deleteDomainGeoLocation" params="{\'Id\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">{{\'Admin.Js.DomainGeoLocationList.Delete\' | translate}}</ui-grid-custom-delete>',
            },
        ],
        uiGridCustom: {
            selectionOptions: [
                {
                    text: $translate.instant('Admin.Js.DomainGeoLocationList.DeleteSelected'),
                    url: 'warehouse/deleteDomainGeoLocations',
                    field: 'Id',
                    before: function () {
                        return SweetAlert.confirm($translate.instant('Admin.Js.DomainGeoLocationList.DeleteSelected.Notification'), {
                            title: $translate.instant('Admin.Js.DomainGeoLocationList.DeleteSelected.Title'),
                            confirmButtonText: $translate.instant('Admin.Js.DomainGeoLocationList.DeleteSelected.Confirm'),
                            cancelButtonText: $translate.instant('Admin.Js.DomainGeoLocationList.DeleteSelected.Cancel'),
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

    ctrl.showModal = function (id) {
        domainGeoLocationService.showModal(id).result.then(
            function (result) {
                ctrl.grid.fetchData();
                return result;
            },
            function (result) {
                ctrl.grid.fetchData();
                return result;
            },
        );
    };
}
