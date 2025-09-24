import templates from './templates/filter-types/filterTypesTemplates.js';

(function (ng) {
    'use strict';

    /* @ngInject */
    var UiGridCustomFilterBlockCtrl = function ($timeout, $filter, $http, $location) {
        var ctrl = this,
            timer;

        ctrl.$onInit = function () {
            ctrl.blockUrl = templates.get(ctrl.blockType);
        };

        ctrl.$postLink = function () {
            if (ctrl.blockType === 'datetime') {
                ctrl.applyDatetime(ctrl.item, 'Y-m-dTH:i');
            } else if (ctrl.blockType === 'date') {
                ctrl.applyDate(ctrl.item, 'Y-m-d');
            } else if (ctrl.blockType === 'time') {
                ctrl.applyTime(ctrl.item, 'H:i');
            } else if (ctrl.blockType === 'range' && ctrl.item.filter.rangeOptions && ctrl.item.filter.term != null) {
                ctrl.apply(
                    [
                        { name: ctrl.item.filter.rangeOptions.from.name, value: ctrl.item.filter.term.from },
                        { name: ctrl.item.filter.rangeOptions.to.name, value: ctrl.item.filter.term.to },
                    ],
                    ctrl.item,
                    true,
                );
            } else if ((ctrl.blockType === 'input' || ctrl.blockType === 'number' || ctrl.blockType === 'phone') && ctrl.item.filter.term != null) {
                ctrl.apply([{ name: ctrl.item.filter.name, value: ctrl.item.filter.term }], ctrl.item, true);
            }
        };

        ctrl.close = function (name, item) {
            ctrl.onClose({ name: name, item: item });
        };

        ctrl.apply = function (params, item, debounce) {
            if (debounce === true) {
                if (timer != null) {
                    $timeout.cancel(timer);
                }

                timer = $timeout(function () {
                    ctrl.onApply({ params: params, item: item });
                }, 300);
            } else {
                ctrl.onApply({ params: params, item: item });
            }
        };

        ctrl.applyDate = function (item, format) {
            var fromValue = typeof item.filter.term.from != 'string' ? $filter('ngFlatpickr')(item.filter.term.from, format) : item.filter.term.from;
            var toValue = typeof item.filter.term.to != 'string' ? $filter('ngFlatpickr')(item.filter.term.to, format) : item.filter.term.to;

            ctrl.apply(
                [
                    { name: item.filter.dateOptions.from.name, value: fromValue },
                    { name: item.filter.dateOptions.to.name, value: toValue },
                ],
                item,
                true,
            );
        };

        ctrl.applyDatetime = function (item, format) {
            var fromValue = typeof item.filter.term.from != 'string' ? $filter('ngFlatpickr')(item.filter.term.from, format) : item.filter.term.from;
            var toValue = typeof item.filter.term.to != 'string' ? $filter('ngFlatpickr')(item.filter.term.to, format) : item.filter.term.to;

            ctrl.apply(
                [
                    { name: item.filter.datetimeOptions.from.name, value: fromValue },
                    { name: item.filter.datetimeOptions.to.name, value: toValue },
                ],
                item,
                true,
            );
        };

        ctrl.applyTime = function (item, format) {
            var fromValue = typeof item.filter.term.from != 'string' ? $filter('ngFlatpickr')(item.filter.term.from, format) : item.filter.term.from;
            var toValue = typeof item.filter.term.to != 'string' ? $filter('ngFlatpickr')(item.filter.term.to, format) : item.filter.term.to;

            ctrl.apply(
                [
                    { name: item.filter.timeOptions.from.name, value: fromValue },
                    { name: item.filter.timeOptions.to.name, value: toValue },
                ],
                item,
                true,
            );
        };

        ctrl.filterSearch = function (search, item) {
            if (!item.filter.dynamicSearch || search == null || item.filter.fetch == null) {
                return;
            }

            if (item.filter.prevSearch == null || item.filter.prevSearch === search) {
                item.filter.prevSearch = search;
                return;
            }

            let params = {
                q: search,
            };
            params[item.filter.name || item.name] = item.filter.term;

            if (item.filter.dynamicSearchRelations != null && item.filter.dynamicSearchRelations.length > 0) {
                for (let i = 0; i < item.filter.dynamicSearchRelations.length; i++) {
                    let key = item.filter.dynamicSearchRelations[i];
                    params[key] = ctrl.getGridValue(key);
                }
            }

            $http.get(item.filter.fetch, { params: params }).then(function (response) {
                if (response.data != null) {
                    item.filter.selectOptions = response.data;
                    item.filter.prevSearch = search;
                }
            });
        };

        ctrl.getGridValue = function (key) {
            const _key = key.toLowerCase();
            const { grid } = $location.search();

            if (grid == null) {
                return null;
            }

            return grid[_key];
        };

        ctrl.eventListen = function (event) {
            if (event.key == 'Enter') {
                event.target.blur();
            }
        };
    };

    ng.module('uiGridCustomFilter').controller('UiGridCustomFilterBlockCtrl', UiGridCustomFilterBlockCtrl);
})(window.angular);
