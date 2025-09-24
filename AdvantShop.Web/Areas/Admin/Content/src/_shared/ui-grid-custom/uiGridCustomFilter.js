import uiGridCustomFilterTemplate from './templates/ui-grid-custom-filter.html';

(function (ng) {
    'use strict';

    /* @ngInject */
    var UiGridCustomFilterCtrl = function ($attrs, $http, $q, $timeout, uiGridConstants, $translate, uiGridCustomService) {
        var ctrl = this;

        ctrl.blocks = [];
        ctrl.menuItems = [];
        ctrl.columns = [];
        ctrl.searchTimer = null;
        ctrl.currentSortDirection = 'asc';
        ctrl.$onInit = function () {
            ctrl.searchAutofocus = ctrl.searchAutofocus === true || ctrl.searchAutofocus == null;

            ctrl._templateUrl = ctrl.templateUrl != null ? ctrl.templateUrl : uiGridCustomFilterTemplate;

            ctrl.gridColumnDefs.forEach(function (value) {
                if (value.filter != null) {
                    ctrl.columns.push(value);

                    ctrl.addFilterBlockOnLoad(value, ctrl.gridParams);
                }
            });

            ctrl.gridSearchText = ctrl.gridParams.search || ctrl.gridSearchText;

            ctrl.gridSearchPlaceholder = ctrl.gridSearchPlaceholder || $translate.instant('Admin.Js.GridCustomFilter.EnterTextToSearch');
            ctrl.gridSearchVisible = ctrl.gridSearchVisible != null ? ctrl.gridSearchVisible : true;

            if (ctrl.onInit != null) {
                ctrl.onInit({ filter: ctrl });
            }
        };

        ctrl.updateColumns = function () {
            ctrl.blocks.length = 0;
            ctrl.columns.length = 0;

            ctrl.gridColumnDefs.forEach(function (value) {
                if (value.filter != null) {
                    ctrl.columns.push(value);
                    ctrl.addFilterBlockOnLoad(value, ctrl.gridParams);
                }
            });
        };

        ctrl.addFilterBlock = function (item, selectedValue, itemPropName, notFetch) {
            if (item == null) {
                return;
            }

            item.filterMaster = {};

            if (item.filterMaster != null) {
                ng.copy(item.filter, item.filterMaster);

                ctrl.fill(item.filter.type, item, selectedValue, itemPropName, notFetch);

                if (ctrl.blocks.indexOf(item) === -1) {
                    ctrl.blocks.push(item);
                }
            }
        };

        ctrl.sortByColumn = function (item, sortingDirection) {
            if (ctrl.uiGridCustom == null && item == null) {
                return;
            }

            const grid = ctrl.uiGridCustom.gridApi.grid;
            ctrl.currentSortDirection = sortingDirection;

            ctrl.selectedColumn = uiGridCustomService.getColByFieldName(grid.columns, item.name);

            grid.sortColumn(ctrl.selectedColumn, sortingDirection);
            ctrl.sortPopoverIsOpen = false;
        };

        ctrl.getDataForFilter = function (item, notFetch) {
            var defer = $q.defer(),
                promise;

            if (item.filter.fetch != null && (!notFetch || item.filter.type === 'select' || item.filter.type === 'selectMultiple')) {
                if (ng.isString(item.filter.fetch) === true) {
                    promise = $http.get(item.filter.fetch, { params: ctrl.gridParams }).then(function (response) {
                        return response.data;
                    });
                } else {
                    promise = item.filter.fetch;
                }
            } else if (notFetch !== true) {
                defer.resolve(item.filter.type === uiGridConstants.filter.SELECT ? item.filter.selectOptions : null);
                promise = defer.promise;
            }

            return promise;
        };

        ctrl.checkVisibleMenuItem = function (items) {
            var result = items.filter(function (value) {
                return ctrl.blocks.indexOf(value) === -1;
            });

            return result;
        };
        ctrl.checkVisibleMenuItemSort = function () {
            return ctrl.gridColumnDefs.filter(function (value) {
                return (value.enableSorting == null || value.enableSorting === true) && value.displayName?.trim().length > 0;
            });
        };

        ctrl.onApplySearch = function (params, item, event) {
            if (
                event.keyCode === 13 &&
                event.keyCode !== 8 &&
                event.keyCode !== 46 &&
                params[0].value != null &&
                params[0].value.length > 0 &&
                params[0].value.length < 2
            ) {
                return;
            }

            if (ctrl.searchTimer != null) {
                $timeout.cancel(ctrl.searchTimer);
            }

            ctrl.searchTimer = $timeout(function () {
                ctrl.onApplyBlock(params, item, event.keyCode);
            }, 300);
        };

        ctrl.onApplyBlock = function (params, item) {
            if (item != null && item.filter.change != null) {
                item.filter.change(params, item, ctrl);
            }

            ctrl.onChange({ params: params, item: item });
        };

        ctrl.onClose = function (name, item) {
            var index = ctrl.blocks.indexOf(item);

            if (index !== -1) {
                ctrl.blocks.splice(index, 1);
            }

            item.filter = item.filterMaster;

            ctrl.onRemove({ name: name, item: item });
        };

        ctrl.fill = function (type, item, selectedValue, itemPropName, notFetch) {
            return $q.when(ctrl.getDataForFilter(item, notFetch)).then(function (data) {
                if (ctrl.handlerTypes[type] != null) {
                    ctrl.handlerTypes[type].fill(item, data, selectedValue, itemPropName);
                } else {
                    new Error('Not found type ' + item.filter.type + ' for filter in grid');
                }
            });
        };

        ctrl.addFilterBlockOnLoad = function (item, params) {
            var nameFilter, urlValue;

            for (var key in params) {
                if (Object.prototype.hasOwnProperty.call(params, key)) {
                    urlValue = params[key];

                    item.showOnPageLoad = ctrl.handlerTypes[item.filter.type].check(key, item);

                    if (item.showOnPageLoad === true) {
                        ctrl.addFilterBlock(item, urlValue, key, true);
                    }
                }
            }
        };

        ctrl.handlerTypes = {
            range: {
                check: function (key, item) {
                    return key === item.filter.rangeOptions.to.name || key === item.filter.rangeOptions.from.name;
                },
                fill: function (item, data, selectedValue, itemPropName) {
                    if (itemPropName === item.filter.rangeOptions.to.name) {
                        item.filter.term = item.filter.term || {};
                        item.filter.term.to = selectedValue;
                    }

                    if (itemPropName === item.filter.rangeOptions.from.name) {
                        item.filter.term = item.filter.term || {};
                        item.filter.term.from = selectedValue;
                    }

                    if (data != null) {
                        item.filter.term = data;
                    }

                    return item;
                },
            },
            select: {
                check: function (key, item) {
                    return key === item.name || key === item.filter.name;
                },
                fill: function (item, data, selectedValue, itemPropName) {
                    if (data != null) {
                        item.filter.selectOptions = data;
                    }
                    if (selectedValue != null) {
                        item.filter.term = selectedValue;
                    }
                },
            },
            selectMultiple: {
                check: function (key, item) {
                    return key === item.name || key === item.filter.name;
                },
                fill: function (item, data, selectedValue, itemPropName) {
                    if (data != null) {
                        item.filter.selectOptions = data;
                    }
                    if (selectedValue != null) {
                        item.filter.term = selectedValue;
                    }
                },
            },
            time: {
                check: function (key, item) {
                    return key === item.filter.timeOptions.to.name || key === item.filter.timeOptions.from.name;
                },
                fill: function (item, data, selectedValue, itemPropName) {
                    if (itemPropName === item.filter.timeOptions.to.name) {
                        item.filter.term = item.filter.term || {};
                        item.filter.term.to = selectedValue;
                        item.showOnPageLoad = true;
                    }

                    if (itemPropName === item.filter.timeOptions.from.name) {
                        item.filter.term = item.filter.term || {};
                        item.filter.term.from = selectedValue;
                        item.showOnPageLoad = true;
                    }

                    if (data != null) {
                        item.filter.term = data;
                    }
                },
            },
            datetime: {
                check: function (key, item) {
                    return key === item.filter.datetimeOptions.to.name || key === item.filter.datetimeOptions.from.name;
                },
                fill: function (item, data, selectedValue, itemPropName) {
                    if (itemPropName === item.filter.datetimeOptions.to.name) {
                        item.filter.term = item.filter.term || {};
                        item.filter.term.to = selectedValue;
                        item.showOnPageLoad = true;
                    }

                    if (itemPropName === item.filter.datetimeOptions.from.name) {
                        item.filter.term = item.filter.term || {};
                        item.filter.term.from = selectedValue;
                        item.showOnPageLoad = true;
                    }

                    if (data != null) {
                        item.filter.term = data;
                    }
                },
            },
            date: {
                check: function (key, item) {
                    return key === item.filter.dateOptions.to.name || key === item.filter.dateOptions.from.name;
                },
                fill: function (item, data, selectedValue, itemPropName) {
                    if (itemPropName === item.filter.dateOptions.to.name) {
                        item.filter.term = item.filter.term || {};
                        item.filter.term.to = selectedValue;
                        item.showOnPageLoad = true;
                    }

                    if (itemPropName === item.filter.dateOptions.from.name) {
                        item.filter.term = item.filter.term || {};
                        item.filter.term.from = selectedValue;
                        item.showOnPageLoad = true;
                    }

                    if (data != null) {
                        item.filter.term = data;
                    }
                },
            },
            input: {
                check: function (key, item) {
                    return key === item.filter.name;
                },
                fill: function (item, data, selectedValue, itemPropName) {
                    item.filter.term = selectedValue;

                    if (data != null) {
                        item.filter.term = data;
                    }
                },
            },
            number: {
                check: function (key, item) {
                    return key === item.filter.name;
                },
                fill: function (item, data, selectedValue, itemPropName) {
                    item.filter.term = selectedValue;

                    if (data != null) {
                        item.filter.term = data;
                    }
                },
            },
            phone: {
                check: function (key, item) {
                    return key === item.filter.name;
                },
                fill: function (item, data, selectedValue, itemPropName) {
                    item.filter.term = selectedValue;

                    if (data != null) {
                        item.filter.term = data;
                    }
                },
            },
        };
    };

    ng.module('uiGridCustomFilter', []).controller('UiGridCustomFilterCtrl', UiGridCustomFilterCtrl);
})(window.angular);
