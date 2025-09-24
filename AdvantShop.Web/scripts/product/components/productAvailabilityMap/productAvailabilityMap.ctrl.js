/* @ngInject */
export default function ProductAvailabilityMapCtrl(productService, warehousesService) {
    const ctrl = this;

    ctrl.$onInit = () => {
        ctrl.isLoadingData = true;
        productService
            .getOfferStocks(ctrl.offerId)
            .then((data) => {
                if (data.result === true) {
                    ctrl.stockListData = warehousesService.getWarehousesMapData(data.obj.Stocks);
                    //если только один элемент на карте и использовать параметр ShowAll
                    //то зум на крте будет максимальный
                    ctrl.isShowAll = ctrl.stockListData.length > 1;
                } else {
                    data.errors.forEach(function (error) {
                        console.error(error);
                    });
                }
            })
            .finally(() => {
                ctrl.isLoadingData = false;
            });
        ctrl.activeBalloon = null;
        ctrl.activePin = null;
        ctrl.mapControls = 'zoomControl';
        ctrl.mapOptions = {
            suppressMapOpenBlock: true,
        };
        ctrl.ZOOM_SELECTED_BALLOON = 17;
        ctrl.zoom = ctrl.mobileMode ? 13 : 10;
        ctrl.isLoadedMap = false;
    };

    // возможно вынести в property объекта
    ctrl.getContentBalloon = function (data) {
        return `<div class="warehouses-list">
                    <div class="warehouses-lis__item">
                        ${data.address ? `<address class="warehouses-list__shop-address">${data.address}</address>` : ``}
                        ${data.addressComment ? `<div class="warehouses-list__shop-address-comment">${data.addressComment}</div>` : ``}
                        ${data.name ? `<strong class="warehouses-list__shop-name">${data.name}</strong>` : ``}
                        ${data.workTime.length ? ctrl.getWorkTime(data.workTime) : ``}
                        <div ${
                            data.stockColor != null ? `style="color: #${data.stockColor}" ` : ``
                        }class="warehouses-list__product-availability-count">Доступно: ${data.stock}</div>
                    </div>
                </div>`;
    };

    ctrl.getWorkTime = function (workTimes) {
        return workTimes.map((it) => `<div>${it}</div>`).join('');
    };

    ctrl.showBalloonPlace = function (e, currentBalloon) {
        if (e.get && ctrl._map) {
            if (ctrl.mobileMode) {
                ctrl._map.balloon.open(currentBalloon.geometry.coordinates, ctrl.getContentBalloon(currentBalloon.store), {
                    panelMaxMapArea: Infinity,
                    autoPan: false,
                    panelMaxHeightRatio: 0.3,
                });
            }
            if (ctrl.activePin) {
                ctrl.activePin.options.unset('preset');
            }
            ctrl.setActiveBalloon(currentBalloon);
            ctrl.setActivePin(e.get('target'));
            ctrl.changeActiveColorPin(ctrl.activePin);
            ctrl._map.setCenter(ctrl.activePin.geometry._coordinates, ctrl.ZOOM_SELECTED_BALLOON);
        }
    };

    ctrl.runBalloon = (balloon, index) => {
        ctrl._map?.setCenter(balloon.geometry.coordinates, ctrl.ZOOM_SELECTED_BALLOON);
        const pin = ctrl.collection?.get(index);
        if (ctrl.activePin != null && ctrl.activePin !== pin) {
            ctrl.activePin.options.unset('preset');
        }
        if (pin != null) {
            ctrl.setActiveBalloon(balloon);
            ctrl.setActivePin(pin);
            ctrl.changeActiveColorPin(ctrl.activePin);
        }
    };

    ctrl.setActivePin = (pin) => {
        ctrl.activePin = pin;
    };

    ctrl.changeActiveColorPin = (pin) => {
        pin.options.set('preset', 'islands#redIcon');
    };

    ctrl.setActiveBalloon = (balloon) => {
        ctrl.activeBalloon = balloon;
    };

    ctrl.resetActiveBalloon = () => {
        ctrl.activeBalloon = null;
    };

    ctrl.handleAfterInitMap = function ($target) {
        ctrl._map = $target;
        ctrl.isLoadedMap = true;
        if (!ctrl.isShowAll) {
            ctrl._map.setCenter(ctrl.stockListData[0].geometry.coordinates, ctrl.ZOOM_SELECTED_BALLOON);
        }
    };

    ctrl.setCollection = function (target) {
        ctrl.collection = target;
    };
}
