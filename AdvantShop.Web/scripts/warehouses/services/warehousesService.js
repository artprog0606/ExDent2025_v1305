/* @ngInject */
function warehousesService($http, $window, $cookies) {
    var service = this;
    const isUseHashListener = false;
    const hashChangeCallbacks = new Set();

    service.getCartStockInWarehouses = function (warehousesId) {
        return $http.post('warehouse/getCartStockInWarehouses', { warehousesId: warehousesId, rnd: Math.random() }).then(function (response) {
            return response.data;
        });
    };

    service.getWarehousesInfo = function () {
        return $http.get('warehouse/getwarehousesinfo').then(function (response) {
            return response.data;
        });
    };

    service.getWarehousesMapData = function (warehouses) {
        return warehouses.map((it, index) => {
            return {
                id: index,
                type: 'Feature',
                geometry: {
                    type: 'Point',
                    coordinates: [it.Longitude, it.Latitude],
                },
                properties: {},
                store: {
                    address: it.Address,
                    name: it.Name,
                    addressComment: it.AddressComment,
                    stockColor: it.StockColor,
                    stock: it.Stock,
                    workTime: it.TimeOfWorkList,
                    type: it.Type,
                },
            };
        });
    };

    service.getFilterTypes = function (warehousesList) {
        const filterType = new Set();
        warehousesList.forEach((it) => {
            it.Type ? filterType.add(it.Type) : null;
        });
        return Array.from(filterType);
    };

    service.getWarehousesByType = function (type, list) {
        return list.filter((it) => {
            return it.store.type === type;
        });
    };

    service.setOnChangeHashListener = function () {
        if (!isUseHashListener) {
            $window.addEventListener('hashchange', (e) => {
                for (let cb of hashChangeCallbacks) {
                    cb(e);
                }
            });
        }
    };

    service.addHashChangeCallback = function (cb) {
        hashChangeCallbacks.add(cb);
    };

    service.removeHashChangeCallback = function (cb) {
        hashChangeCallbacks.delete(cb);
    };

    service.setWarehouseCookie = function (warehouseIds) {
        return $http.post('warehouse/setWarehouses', warehouseIds).then(function (response) {
            return response.data;
        });
    };

    service.removeWarehouseCookie = function () {
        return service.setWarehouseCookie(null);
    };
}

export default warehousesService;
