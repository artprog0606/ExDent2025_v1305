/* @ngInject */
export default function WarehouseCitiesCtrl($http, toaster, $translate, SweetAlert) {
    const ctrl = this;
    ctrl.cities = [];

    ctrl.init = function () {
        if (ctrl.warehouseId !== 0) {
            ctrl.getWarehouseCities();
        }
    };

    ctrl.onChange = function () {
        if (ctrl.onChangeFn) {
            ctrl.onChangeFn({ warehouseCities: ctrl.cities });
        }
    };

    ctrl.addCities = function (result) {
        if (result == null || result.cities == null || result.cities.length == 0) {
            return;
        }

        if (ctrl.warehouseId === 0) {
            for (let i = 0; i < result.cities.length; i++) {
                if (ctrl.cities.find((x) => x.CityId === result.cities[i].CityId) == null) {
                    ctrl.cities.push({
                        CityId: result.cities[i].CityId,
                        CityName: result.cities[i].CityName,
                    });
                }
            }
            ctrl.onChange();
        } else {
            let warehouseCities = result.cities.map(function (x) {
                return { warehouseId: ctrl.warehouseId, cityId: x.CityId };
            });

            $http.post('warehouse/addWarehouseCities', warehouseCities).then(function (response) {
                let data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', 'Изменения сохранены');
                    ctrl.getWarehouseCities();
                } else {
                    data.errors.forEach(function (err) {
                        toaster.pop('error', '', err);
                    });
                }
            });
        }
    };

    ctrl.removeCity = function (index, item) {
        SweetAlert.confirm('Удалить город?', { title: 'Удаление города' }).then(function (result) {
            if (result === true || result.value) {
                if (ctrl.warehouseId === 0) {
                    ctrl.cities.splice(index, 1);
                    ctrl.onChange();
                } else {
                    $http.post('warehouse/deleteWarehouseCity', { warehouseId: ctrl.warehouseId, cityId: item.CityId }).then(function (response) {
                        let data = response.data;
                        if (data.result === true) {
                            toaster.pop('success', '', 'Изменения сохранены');
                            ctrl.getWarehouseCities();
                        } else {
                            data.errors.forEach(function (err) {
                                toaster.pop('error', '', err);
                            });
                        }
                    });
                }
            }
        });
    };

    ctrl.getWarehouseCities = function () {
        $http.get('warehouse/getCitiesByWarehouse', { params: { warehouseId: ctrl.warehouseId } }).then(function (response) {
            ctrl.cities = response.data || [];
        });
    };
}
