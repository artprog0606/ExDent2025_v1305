const positionList = ['top', 'left', 'bottom'];
const positionListReset = {
    top: 'auto',
    left: 'auto',
    bottom: 'auto',
};

const REGEXP_SCROLL_PARENT = /^(visible|hidden)/u;
const getScrollParent = (el) =>
    !(el instanceof HTMLElement) || typeof window.getComputedStyle !== 'function'
        ? null
        : el.scrollHeight >= el.clientHeight && !REGEXP_SCROLL_PARENT.test(window.getComputedStyle(el).overflowY || 'visible')
          ? el
          : getScrollParent(el.parentElement) || document.body;
/*@ngInject*/
function AutocompleterCtrl($scope, autocompleterService, domService, $document, $timeout, $window, $parse, $q) {
    var ctrl = this,
        listScrollable,
        autocompleterInputElement,
        listWrap,
        scrollParent,
        showEmptyResultMessageDirty;

    ctrl.$onInit = function () {
        ctrl.result = null;
        ctrl.activeItem = null;
        ctrl.isVisibleAutocomplete = false;
        ctrl.viewMode = 'default';
        ctrl.itemFromObjects = [];
        ctrl.items = [];
        ctrl.isClickedItem = false;

        showEmptyResultMessageDirty = ctrl.showEmptyResultMessage != null && ctrl.showEmptyResultMessage();

        ctrl.showEmptyResultMessage = showEmptyResultMessageDirty != null ? showEmptyResultMessageDirty : true;

        if (ctrl.onInit != null) {
            ctrl.onInit({ autocompleter: ctrl });
        }
    };

    ctrl.toggleVisible = function (visible) {
        ctrl.onChangeVisibility({ visible });
        if (visible === true) {
            ctrl.activeItem = null;
        }

        if (ctrl.isVisibleAutocomplete !== visible) {
            if (visible === true) {
                listWrap.classList.add('autocompleter-sub--calc');
                listWrap.classList.add('autocompleter-sub--visible');
            } else {
                listWrap.classList.remove('autocompleter-sub--calc');
                listWrap.classList.remove('autocompleter-sub--visible');
            }
            ctrl.isVisibleAutocomplete = visible;
        }
    };

    ctrl.addList = function (listDOM, listController) {
        listWrap = listDOM;
        scrollParent = getScrollParent(listWrap);
        listScrollable = listDOM.querySelector('.js-autocompleter-list');
        ctrl.listCtrl = listController;
    };

    ctrl.addItem = function (item) {
        item.groupIndex = item.groupIndex != null ? item.groupIndex : 0;

        ctrl.items[item.groupIndex] = ctrl.items[item.groupIndex] || [];

        if (item.index == null) {
            ctrl.items[item.groupIndex].push(item);
            item.index = ctrl.items[item.groupIndex].length - 1;
        } else {
            ctrl.items[item.groupIndex][item.index] = item;
        }

        return ctrl.items;
    };

    ctrl.setListPosition = function (pos) {
        if (listWrap != null) {
            ctrl.listPositional = pos;
            for (const name of positionList) {
                if (pos.hasOwnProperty(name)) {
                    listWrap.style.setProperty(name, pos[name] + (typeof pos[name] !== 'string' ? 'px' : ''));
                } else {
                    listWrap.style.removeProperty(name);
                }
            }
        }
    };

    ctrl.request = function (val) {
        if (angular.isDefined(val) && val.length >= ctrl.minLength) {
            autocompleterService.getData(ctrl.requestUrl, val, ctrl.params).then(function (response) {
                ctrl.result = response;
                //ctrl.items.length = 0;

                if (ctrl.result == null) {
                    return;
                }

                if (angular.isArray(ctrl.result)) {
                    ctrl.viewMode = 'default';
                    ctrl.emptyResult = ctrl.result.length === 0;
                } else {
                    ctrl.viewMode = 'additional';
                    ctrl.emptyResult = ctrl.result.Empty === true;
                }
                ctrl.toggleVisible(true);
            });
        } else if (angular.isDefined(val) && val.length < ctrl.minLength) {
            ctrl.toggleVisible(false);
        }
    };

    ctrl.navigate = function (isDown) {
        var indexGroup, indexCurrent, newIndexGroup, newIndex, newActiveItem;

        var currentItem,
            currentGroup,
            navVal = isDown === true ? 1 : -1;

        if (ctrl.items.length === 0) {
            return;
        }

        if (ctrl.activeItem == null) {
            currentGroup = ctrl.getIndexFirstOrDefaultGroup();
            currentItem = ctrl.getIndexFirstOrDefaultItem(currentGroup);
            newActiveItem = currentItem;
        } else {
            currentItem = ctrl.activeItem;
            newActiveItem = ctrl.items[currentItem.groupIndex][currentItem.index + navVal];
        }

        //пытаемся найти элемент в след./пред. группе
        if (newActiveItem == null && ctrl.items[currentItem.groupIndex + navVal] != null && ctrl.items[currentItem.groupIndex + navVal].length > 0) {
            newActiveItem = ctrl.getIndexFirstOrDefaultItem(ctrl.items[currentItem.groupIndex + navVal]);
        }

        if (newActiveItem != null) {
            ctrl.processItems(function (group, item) {
                if (item != null) {
                    item.isActive = false;
                }
            });

            newActiveItem.isActive = true;
            ctrl.activeItem = newActiveItem;

            ctrl.checkScroll(ctrl.activeItem.itemDOM);
        } else {
            ctrl.activeItem = null;
        }
    };

    ctrl.checkScroll = function (element) {
        var topContainer = listScrollable.scrollTop,
            bottomContainer = topContainer + listScrollable.clientHeight,
            topItem = element.offsetTop,
            heightItem = element.clientHeight,
            bottomItem = topItem + heightItem;

        if (bottomContainer < bottomItem) {
            listScrollable.scrollTop += heightItem;
        } else if (topContainer > topItem) {
            listScrollable.scrollTop = topItem;
        }
    };

    ctrl.apply = function (val, event) {
        ctrl.model.$setViewValue(val);
        ctrl.model.$render();

        ctrl.applyFn({ value: val, obj: ctrl.activeItem != null ? ctrl.activeItem.item : null, event: event });

        ctrl.toggleVisible(false);

        ctrl.isDirty = false;
    };

    ctrl.autocompleteKeyup = function ($event, val, element) {
        autocompleterInputElement = element;

        var keyCode = $event.keyCode;

        switch (keyCode) {
            case 38: //arrow up
                $event.stopPropagation();
                $event.preventDefault();
                ctrl.navigate(false);
                break;
            case 40: //arrow down
                $event.stopPropagation();
                $event.preventDefault();
                ctrl.navigate(true);
                break;
            case 13: //enter
                if (ctrl.activeItem != null) {
                    $event.stopPropagation();
                    ctrl.apply(ctrl.activeItem.item[ctrl.field], $event);
                } else {
                    ctrl.apply(val, $event);
                }
                break;
            default:
                $event.stopPropagation();
                ctrl.isDirty = true;
                ctrl.request(val);
                break;
        }
    };

    ctrl.crossClick = function ($event) {
        ctrl.toggleVisible(false);

        $event.stopPropagation();
    };

    ctrl.itemClick = function ($event, item) {
        ctrl.isClickedItem = true;

        var selectedValue = $parse(ctrl.field)(item.item);

        ctrl.apply(selectedValue, $event);

        $event.stopPropagation();
    };

    ctrl.itemActive = function (item) {
        item.isActive = true;
        ctrl.activeItem = item;
    };

    ctrl.itemDeactive = function (item) {
        item.isActive = false;
        ctrl.activeItem = null;
    };

    ctrl.clickOut = function (event) {
        if (ctrl.isVisibleAutocomplete === true && domService.closest(event.target, '.js-autocompleter-sub') == null) {
            $scope.$apply(function () {
                ctrl.toggleVisible(false);
            });
        }
    };

    ctrl.getIndexFirstOrDefaultGroup = function () {
        var group;

        for (var i = 0, len = ctrl.items.length; i < len; i++) {
            if (ctrl.items[i] != null && ctrl.items[i].length > 0) {
                group = ctrl.items[i];
                break;
            }
        }

        return group;
    };

    ctrl.getIndexFirstOrDefaultItem = function (group) {
        var item;

        for (var i = 0, len = group.length; i < len; i++) {
            if (group[i] != null) {
                item = group[i];
                break;
            }
        }

        return item;
    };

    ctrl.processItems = function (func) {
        for (var i = 0, l = ctrl.items.length; i < l; i++) {
            if (ctrl.items[i] != null) {
                for (var k = 0, l2 = ctrl.items[i].length; k < l2; k++) {
                    if (func(ctrl.items[i], ctrl.items[i][k]) === false) {
                        break;
                    }
                }
            }
        }
    };

    ctrl.recalcPositionAutocompleList = function (hideOnCalc = true) {
        const defer = $q.defer();

        if (hideOnCalc !== false) {
            ctrl.setListPosition(positionListReset);
            $timeout(() => {
                defer.resolve();
            }, 100);
        } else {
            defer.resolve();
        }

        defer.promise.then(() => {
            const listWrapCoordinates = listWrap.getBoundingClientRect();
            const inputCoordinates = autocompleterInputElement[0].getBoundingClientRect();
            const position = {};
            position.left =
                scrollParent.clientWidth >= inputCoordinates.left + listWrapCoordinates.width ? autocompleterInputElement[0].offsetLeft : 'auto';

            const listWrapTop = listWrap.offsetTop;
            const listWrapHeight = listWrapCoordinates.height;
            const inputTop = autocompleterInputElement[0].offsetTop;
            const positionVertical = listWrapTop + listWrapHeight > scrollParent.getBoundingClientRect().bottom ? 'bottom' : 'top';
            position[positionVertical] = (inputTop > 50 ? inputTop : 0) + autocompleterInputElement[0].offsetHeight;
            ctrl.setListPosition(position);
            listWrap.classList.remove('autocompleter-sub--calc');
        });
    };
}

export default AutocompleterCtrl;
