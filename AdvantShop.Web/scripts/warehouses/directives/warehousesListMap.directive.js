import warehousesListMapTemplate from '../templates/warehousesListMap.html';

export default function WarehousesListMapDirective() {
    return {
        scope: {
            mapData: '<',
            apiKeyMap: '@',
            showDetailsWarehouse: '<?',
            compactMode: '<?',
        },
        controller: 'warehousesMapCtrl',
        controllerAs: 'warehousesMap',
        bindToController: true,
        templateUrl: warehousesListMapTemplate,
    };
}
