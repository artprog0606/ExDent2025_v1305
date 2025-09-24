import warehouseCitiesTemplate from './templates/warehouseCities.html';

/* @ngInject */
const warehouseCitiesComponent = {
    templateUrl: warehouseCitiesTemplate,
    controller: 'WarehouseCitiesCtrl',
    bindings: {
        warehouseId: '<',
        onChangeFn: '&',
    },
};

export { warehouseCitiesComponent };
