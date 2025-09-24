/* @ngInject */
export default function StockLabelListCtrl(
    $q,
    uiGridConstants,
    uiGridCustomConfig,
    uiGridCustomParamsConfig,
    uiGridCustomService,
    SweetAlert,
    stockLabelService,
    $translate,
) {
    var ctrl = this;

    ctrl.gridOptions = angular.extend({}, uiGridCustomConfig, {
        columnDefs: [
            {
                name: 'Color',
                displayName: '',
                cellTemplate: '<div class="ui-grid-cell-contents"><i class="fa fa-circle" ng-style="{color:\'#\' + COL_FIELD}"></i></div>',
                width: 40,
            },
            {
                name: 'Name',
                displayName: $translate.instant('Admin.Js.StockLabel.Name'),
                enableCellEdit: true,
                filter: {
                    placeholder: $translate.instant('Admin.Js.StockLabel.NameFilter'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Name',
                },
            },
            {
                name: 'AmountUpTo',
                displayName: $translate.instant('Admin.Js.StockLabel.AmountUpTo'),
                width: 150,
                enableCellEdit: true,
                filter: {
                    placeholder: $translate.instant('Admin.Js.StockLabel.AmountUpToFilter'),
                    type: 'range',
                    rangeOptions: {
                        from: {
                            name: 'AmountUpToFrom',
                        },
                        to: {
                            name: 'AmountUpToTo',
                        },
                    },
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
                    '<button type="button" ng-click="grid.appScope.$ctrl.gridExtendCtrl.showStockLabel(row.entity.LabelId); $event.preventDefault();" class="btn-icon ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="{{\'Admin.Js.StockLabel.Edit\' | translate}}"></button>' +
                    '<ui-grid-custom-delete url="warehouse/deletestocklabel" params="{\'StockLabelId\': row.entity.LabelId}"></ui-grid-custom-delete></div></div>' +
                    '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="warehouse/deletestocklabel" params="{\'StockLabelId\': row.entity.LabelId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">{{\'Admin.Js.StockLabel.Delete\' | translate}}</ui-grid-custom-delete>',
            },
        ],
        uiGridCustom: {
            selectionOptions: [
                {
                    text: $translate.instant('Admin.Js.StockLabel.DeleteSelected'),
                    url: 'warehouse/deletestocklabels',
                    field: 'LabelId',
                    before: function () {
                        return SweetAlert.confirm($translate.instant('Admin.Js.StockLabel.DeleteSelected.Notification'), {
                            title: $translate.instant('Admin.Js.StockLabel.DeleteSelected.Title'),
                            confirmButtonText: $translate.instant('Admin.Js.StockLabel.DeleteSelected.Confirm'),
                            cancelButtonText: $translate.instant('Admin.Js.StockLabel.DeleteSelected.Cancel'),
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

    ctrl.showStockLabel = function (stockLabelId) {
        stockLabelService.showStockLabel(stockLabelId).result.then(
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
