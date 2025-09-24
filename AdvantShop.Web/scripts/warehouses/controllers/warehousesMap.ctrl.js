/* @ngInject */
export default function WarehousesMapCtrl(warehousesService, $location, $window, WarehousesDisplayStatus, $scope, $translate) {
    const ZOOM_SELECTED_BALLOON = 17;
    const ctrl = this;
    const NATIVE_COMPACT_MODE_BALLOON_PANEL_OPTIONS = {
        panelMaxMapArea: Infinity,
        autoPan: false,
        panelMaxHeightRatio: 0.3,
    };
    const defaultFilterType = $translate.instant('Js.WarehousesMap.All');
    let warehousesMapDataListBackup;

    ctrl.$onInit = () => {
        ctrl.isLoadedMap = false;
        ctrl.isShowMap = false;
        ctrl.isShowList = true;
        ctrl.activeBalloon = null;
        ctrl.activePin = null;
        ctrl.mapControls = 'zoomControl';
        if (ctrl.compactMode) {
            ctrl.setWarehousesDisplayStatus();
        }

        warehousesService.getWarehousesInfo().then((data) => {
            if (data.result === true) {
                ctrl.warehousesList = data.obj;
                ctrl.filterTypes = warehousesService.getFilterTypes(ctrl.warehousesList);
                if (ctrl.filterTypes.length > 0) {
                    ctrl.filterTypes.unshift(defaultFilterType);
                    ctrl.warehouseFilterType = defaultFilterType;
                }
                ctrl.warehousesMapDataList = warehousesMapDataListBackup = warehousesService.getWarehousesMapData(data.obj);
            } else {
                data.errors.forEach(function (error) {
                    console.error(error);
                });
            }
        });

        ctrl.mapOptions = {
            suppressMapOpenBlock: true,
        };
        ctrl.zoom = 10;

        warehousesService.setOnChangeHashListener();

        warehousesService.addHashChangeCallback(() => {
            ctrl.setWarehousesDisplayStatus();
            $scope.$apply();
        });
    };

    ctrl.setWarehousesDisplayStatus = function () {
        const hash = $location.hash();
        if (hash === WarehousesDisplayStatus.map) {
            ctrl.showMapWarehouses();
        } else {
            ctrl.showListWarehouses();
        }
    };

    ctrl.showBalloonPlace = function (e, currentBalloon) {
        if (e.get && ctrl._map) {
            if (ctrl.activePin) {
                ctrl.activePin.options.unset('preset');
            }
            ctrl.setActivePin(e.get('target'));
            ctrl.changeActiveColorPin(ctrl.activePin);
            ctrl._map.setCenter(ctrl.activePin.geometry._coordinates, ZOOM_SELECTED_BALLOON);
            ctrl.setActiveBalloon(currentBalloon);
        }
    };

    ctrl.getContentBalloon = function (data) {
        return `<div class="warehouses-list">
                    <div class="warehouses-list__item">
                        ${data.address ? `<address class="warehouses-list__shop-address">${data.address}</address>` : ``}
                        ${data.name ? `<strong class="warehouses-list__shop-name">${data.name}</strong>` : ``}
                        ${data.workTime ? `<div class="warehouses-list__work-time">${data.workTime}</div>` : ``}
                    </div>
                </div>`;
    };

    ctrl.showBalloonPlace = function (e, balloon) {
        if (e.get && ctrl._map) {
            if (ctrl.compactMode) {
                ctrl.openNativePanelBalloon(balloon);
            }
            if (ctrl.activePin) {
                ctrl.activePin.options.unset('preset');
            }
            ctrl.setActiveBalloon(balloon);
            ctrl.setActivePin(e.get('target'));
            ctrl.changeActiveColorPin(ctrl.activePin);
            ctrl._map.setCenter(ctrl.activePin.geometry._coordinates, ctrl.ZOOM_SELECTED_BALLOON);
        }
    };

    ctrl.runBalloon = (balloon, index) => {
        ctrl._map?.setCenter(balloon.geometry.coordinates, ZOOM_SELECTED_BALLOON);

        if (ctrl.compactMode) {
            ctrl.openNativePanelBalloon(balloon);
        }

        const pin = ctrl.collection?.get(index);
        if (ctrl.activePin != null && ctrl.activePin !== pin) {
            ctrl.activePin.options.unset('preset');
        }
        if (pin) {
            ctrl.setActivePin(pin);
            ctrl.changeActiveColorPin(ctrl.activePin);
        }
        ctrl.setActiveBalloon(balloon);
        ctrl.showMap();
        ctrl.hideList();
    };

    ctrl.openNativePanelBalloon = function (balloon) {
        ctrl._map?.balloon.open(balloon.geometry.coordinates, ctrl.getContentBalloon(balloon.store), NATIVE_COMPACT_MODE_BALLOON_PANEL_OPTIONS);
    };

    ctrl.setActivePin = (pin) => {
        ctrl.activePin = pin;
    };

    ctrl.setActiveBalloon = (balloon) => {
        ctrl.activeBalloon = balloon;
    };

    ctrl.resetActiveBalloon = () => {
        ctrl.activeBalloon = null;
    };

    ctrl.backToList = () => {
        ctrl.resetActiveBalloon();
    };

    ctrl.changeActiveColorPin = (pin) => {
        pin.options.set('preset', 'islands#redIcon');
    };

    ctrl.handleAfterInitMap = function ($target) {
        ctrl._map = $target;
        ctrl.isLoadedMap = true;
    };

    ctrl.setCollection = function (target) {
        ctrl.collection = target;
    };

    ctrl.showMap = function () {
        ctrl.isShowMap = true;
    };

    ctrl.hideMap = function () {
        ctrl.isShowMap = false;
    };

    ctrl.showList = function () {
        ctrl.isShowList = true;
    };

    ctrl.hideList = function () {
        ctrl.isShowList = false;
    };

    ctrl.resetActiveBalloon = function () {
        ctrl.activeBalloon = null;
    };

    ctrl.filterBy = function (type, list) {
        ctrl.resetActiveBalloon();
        if (type === defaultFilterType) {
            ctrl.warehousesMapDataList = warehousesMapDataListBackup;
        } else {
            ctrl.warehousesMapDataList = warehousesService.getWarehousesByType(type, warehousesMapDataListBackup);
        }
    };

    ctrl.showListWarehouses = function () {
        $location.hash(WarehousesDisplayStatus.list);
        ctrl.hideMap();
        ctrl.showList();
    };

    ctrl.showMapWarehouses = function () {
        $location.hash(WarehousesDisplayStatus.map);
        ctrl.hideList();
        ctrl.showMap();
    };
}
