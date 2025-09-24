import warehousesListTemplate from '../templates/warehousesList.html';

export default function ShopsListDirective() {
    return {
        scope: {
            warehouses: '<',
            onClickWarehouse: '&',
            activeWarehouse: '=',
            showDetailsWarehouse: '<?',
            onlyActiveWarehouse: '<?',
        },
        bindToController: true,
        controller: function () {
            const ctrl = this;

            ctrl.backToList = () => {
                ctrl.activeWarehouse = null;
            };

            ctrl.handleClickWarehouse = (warehouse, index) => {
                ctrl.onClickWarehouse({ warehouse, index });
            };
        },
        controllerAs: '$ctrl',
        templateUrl: warehousesListTemplate,
    };
}
