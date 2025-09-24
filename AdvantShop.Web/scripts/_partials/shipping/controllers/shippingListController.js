/* @ngInject */
function ShippingListCtrl($anchorScroll, $location, shippingService) {
    var ctrl = this;
    var watchersFn = [];
    //ctrl.isProgress = null;

    $anchorScroll.yOffset = 50;

    //ctrl.selectedItemIndex = 0;
    //ctrl.collapsed = true;

    ctrl.$onInit = function () {
        ctrl.customClassesByItemId = {};
        ctrl.visibleItems = Number.POSITIVE_INFINITY;
    };

    ctrl.changeShipping = function (shipping, index) {
        if (index != null) {
            ctrl.selectedItemIndex = index;
        }

        ctrl.change({
            shipping: shipping,
            newShipping: ctrl.newShipping,
        });
    };

    ctrl.changeShippingControl = function (shipping) {
        for (var i = ctrl.items.length - 1; i >= 0; i--) {
            if (ctrl.items[i] === shipping) {
                ctrl.selectShipping = shipping;
                ctrl.selectedItemIndex = i;
                break;
            }
        }

        ctrl.change({
            shipping: shipping,
            customShipping: ctrl.customShipping,
        });
    };

    ctrl.focusEditPrice = function (shipping, index) {
        ctrl.selectShipping = shipping;
        ctrl.selectedItemIndex = index;

        ctrl.focus({
            shipping: shipping,
            customShipping: ctrl.customShipping,
        });
    };

    ctrl.calc = function (index) {
        var selectItemPos = index + 1;

        ctrl.selectedItemIndex = index;

        ctrl.visibleItems = selectItemPos > ctrl.countVisibleItems ? selectItemPos : ctrl.countVisibleItems;

        return selectItemPos;
    };

    ctrl.toggleVisible = function () {
        var selectItemPos = ctrl.calc(ctrl.selectedItemIndex);

        if (ctrl.collapsed === true) {
            ctrl.visibleItems = ctrl.items.length;
            ctrl.collapsed = false;
        } else {
            if (selectItemPos === ctrl.items.length) {
                return;
            }

            ctrl.visibleItems = selectItemPos > ctrl.countVisibleItems ? selectItemPos : ctrl.countVisibleItems;
            ctrl.collapsed = true;

            $location.hash(ctrl.anchor);
            $anchorScroll();
        }
    };

    ctrl.setSelectedIndex = function (index) {
        var selectItemPos = ctrl.calc(index);

        if (selectItemPos === ctrl.items.length) {
            ctrl.collapsed = false;
        } else {
            ctrl.collapsed = true;
        }
    };

    ctrl.addCallbackOnLoad = function (fn) {
        watchersFn.push(fn);
    };

    ctrl.processCallbacks = function () {
        var params = arguments;
        watchersFn.forEach(function (fn) {
            fn(params);
        });
    };

    ctrl.showProgressForItem = function (item) {
        return ctrl.isProgress !== true && item.Template && shippingService.isTemplateReady(item.Template) !== true;
    };

    //#region deliveryInterval

    ctrl.changeDeliveryInterval = function (item) {
        if (ctrl.selectShipping.TimeOfDelivery) {
            ctrl.updateDeliveryInterval({
                shipping: item,
            });
        }
    };

    ctrl.changeSoonest = function (item) {
        if (!ctrl.selectShipping.Asap) return;

        ctrl.selectShipping.TimeOfDelivery = null;
        ctrl.updateDeliveryInterval({
            shipping: item,
        });
    };

    ctrl.initIntervals = function (item, index) {
        if (!ctrl.selectShipping.DateOfDeliveryStr) {
            ctrl.updateDeliveryInterval({
                shipping: item || ctrl.selectShipping,
            });
            return;
        }
        var dateArr = ctrl.selectShipping.DateOfDeliveryStr.split('.');
        var selectedDate = new Date(dateArr[2], dateArr[1] - 1, dateArr[0]);
        var dayOfWeek = selectedDate.getDay();
        if (!ctrl.deliveryIntervals || !Object.prototype.hasOwnProperty.call(ctrl.deliveryIntervals, dayOfWeek)) {
            ctrl.intervalsOnSelectedDay = null;
            return;
        }
        var dateNow = new Date(ctrl.selectShipping.StartDateTime);
        if (
            dateNow.getDay() != dayOfWeek ||
            dateNow.getDate() != selectedDate.getDate() ||
            dateNow.getMonth() != selectedDate.getMonth() ||
            dateNow.getFullYear() != selectedDate.getFullYear()
        ) {
            ctrl.intervalsOnSelectedDay = ctrl.deliveryIntervals[dayOfWeek];
            if (ctrl.intervalsOnSelectedDay.length == 1) {
                ctrl.selectShipping.TimeOfDelivery = ctrl.intervalsOnSelectedDay[0];
                ctrl.changeShipping(item || ctrl.selectShipping, index);
            }
            return;
        }
        var countInvalidIntervals = 0;
        var minutes = dateNow.getUTCMinutes() + (ctrl.selectShipping.TimeZoneOffset % 1) * 60;
        var hours = dateNow.getUTCHours() + Math.trunc(ctrl.selectShipping.TimeZoneOffset);
        if (minutes >= 60) {
            minutes = minutes - 60;
            hours++;
        }
        if (hours >= 24) hours = hours - 24;
        ctrl.deliveryIntervals[dayOfWeek].forEach(function (interval) {
            var timeArr = interval.split('-')[0].split(':');
            if (hours < timeArr[0] || (hours == timeArr[0] && minutes < timeArr[1])) return;
            if (interval == ctrl.selectShipping.TimeOfDelivery)
                //выбранный интервал уже недоступен
                ctrl.selectShipping.TimeOfDelivery = null;
            countInvalidIntervals++;
        });
        ctrl.intervalsOnSelectedDay = ctrl.deliveryIntervals[dayOfWeek].slice(countInvalidIntervals);
        if (ctrl.intervalsOnSelectedDay.length == 1) {
            ctrl.selectShipping.TimeOfDelivery = ctrl.intervalsOnSelectedDay[0];
            ctrl.changeShipping(item || ctrl.selectShipping, index);
        }
    };

    ctrl.parseDeliveryInterval = function () {
        ctrl.deliveryIntervals = {};
        if (ctrl.selectShipping.DeliveryIntervalsStr) {
            ctrl.selectShipping.DeliveryIntervalsStr.split('|').forEach(function (dayStr) {
                var arr = dayStr.split('!');
                var day = arr[0];
                var intervals = arr[1].split('&').filter((x) => !!x && x.length != 0);
                ctrl.deliveryIntervals[day] = intervals || [];
            });
        }
        ctrl.initIntervals();
        ctrl.disabledDates = [
            function (date) {
                return !ctrl.deliveryIntervals || !Object.prototype.hasOwnProperty.call(ctrl.deliveryIntervals, date.getDay());
            },
        ];
    };

    //#endregion
}

export default ShippingListCtrl;
