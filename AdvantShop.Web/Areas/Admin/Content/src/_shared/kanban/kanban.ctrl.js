(function (ng) {
    'use strict';

    var KanbanCtrl = function ($http, $location, $window, $timeout) {
        var ctrl = this;

        var pageYOffset;

        ctrl.$onInit = function () {
            ctrl._params = ctrl.kanbanParams;

            ctrl.optionsFromUrl();

            ctrl.fetchData().then(function () {
                if (ctrl.kanbanOnInit != null) {
                    ctrl.kanbanOnInit({ kanban: ctrl });
                }

                if (ctrl.kanbanScrollable === true) {
                    dragscroll.reset();
                }
            });

            ctrl._sortOptions = ng.extend({}, ctrl.sortOptions);

            var mq = $window.matchMedia('(max-width: 1024px)');

            ctrl.isSortableDisabled = mq.matches;

            mq.addListener(function () {
                ctrl.isSortableDisabled = mq.matches;
            });

            if (ctrl.kanbanScrollable === true) {
                var interval,
                    SCROLL_STEP = 20, //на сколько пикселей переместить скролл
                    DISTANCE_DIFF = 50; //растояние от края, когда нужно начать скролить

                ctrl._sortOptions.dragEnd = function () {
                    if (interval != null) {
                        $window.clearInterval(interval);
                    }
                };

                ctrl._sortOptions.dragMove = function (itemPosition, containment, eventObj) {
                    var containerScrollable =
                        ctrl.containerScrollable || (ctrl.containerScrollable = containment[0].querySelector('.kanban-scrollable'));

                    var containerScrollableOffset = containerScrollable.getBoundingClientRect();

                    var belowX = eventObj.clientX - containerScrollableOffset.left > containerScrollable.clientWidth - DISTANCE_DIFF;
                    var belowY = eventObj.clientY - containerScrollableOffset.top > containerScrollable.clientHeight - DISTANCE_DIFF;

                    //https://github.com/a5hik/ng-sortable/issues/13#issuecomment-120388981
                    //*35 is the vertical offset I needed (the distance between top of the page to the dragging handle)
                    var aboveX = eventObj.clientX < containerScrollableOffset.left + DISTANCE_DIFF;
                    var aboveY = eventObj.clientY < containerScrollableOffset.top + DISTANCE_DIFF;

                    if (belowX || aboveX || belowY || aboveY) {
                        if (!interval) {
                            interval = $window.setInterval(function () {
                                containerScrollable.scrollBy(
                                    belowX || aboveX ? (belowX ? SCROLL_STEP : -SCROLL_STEP) : 0,
                                    belowY || aboveY ? (belowY ? SCROLL_STEP : -SCROLL_STEP) : 0,
                                );
                            }, 25);
                        }
                    } else {
                        $window.clearInterval(interval);
                        interval = null;
                    }
                };
            }

            ctrl.kanbanStickyTop = ctrl.kanbanStickyTop || 70;
        };

        ctrl.filterInit = function (filter) {
            ctrl.filter = filter;
            if (ctrl.kanbanOnFilterInit != null) {
                ctrl.kanbanOnFilterInit({ filter: filter });
            }
        };

        ctrl.optionsFromUrl = function () {
            var kanbanParamsByUrl = ctrl.getParamsByUrl(ctrl.uid);

            if (kanbanParamsByUrl != null) {
                ng.extend(ctrl._params, kanbanParamsByUrl);
            }
        };

        ctrl.filterApply = function (params) {
            if (ng.isArray(params) === false) {
                throw new Error('Parameter "params" should be array');
            }

            for (var i = 0, len = params.length; i < len; i++) {
                ctrl._params[params[i].name] = params[i].value;
            }

            ctrl.resetColumnsData();

            ctrl.fetchData().then(function () {
                ctrl.setParamsByUrl(ctrl._params);
            });
        };

        ctrl.filterRemove = function (name, item) {
            if (item.filter.type === 'range') {
                delete ctrl._params[item.filter.rangeOptions.from.name];
                delete ctrl._params[item.filter.rangeOptions.to.name];
            }
            if (item.filter.type === 'datetime') {
                delete ctrl._params[item.filter.datetimeOptions.from.name];
                delete ctrl._params[item.filter.datetimeOptions.to.name];
            } else if (item.filter.type === 'date') {
                delete ctrl._params[item.filter.dateOptions.from.name];
                delete ctrl._params[item.filter.dateOptions.to.name];
            } else if (item.filter.type === 'time') {
                delete ctrl._params[item.filter.timeOptions.from.name];
                delete ctrl._params[item.filter.timeOptions.to.name];
            } else {
                delete ctrl._params[name];
            }

            ctrl.resetColumnsData();

            ctrl.fetchData().then(function () {
                ctrl.setParamsByUrl(ctrl._params);
            });
        };

        ctrl.setParamsByUrl = function (params) {
            $location.search(ctrl.uid, JSON.stringify(params));
        };

        ctrl.getParamsByUrl = function (uid) {
            const search = $location.search();
            return search != null && search[uid] != null ? JSON.parse(search[uid]) : null;
        };

        ctrl.resetColumnsData = function () {
            if (ctrl.kanbanObj) {
                for (var i = 0; i < ctrl.kanbanObj.Columns.length; i++) {
                    ctrl.kanbanObj.Columns[i].Page = 1;
                }
            }
        };

        ctrl.getRequestParams = function () {
            var params = { columns: [] };
            if (ctrl.kanbanObj) {
                ctrl.kanbanObj.Columns.forEach(function (column) {
                    params.columns.push({
                        id: column.Id,
                        page: column.Page,
                        cardsPerColumn: column.CardsPerColumn,
                    });
                });
            }
            ng.extend(params, ctrl._params);
            return params;
        };

        ctrl.fetchData = function () {
            return $http.post(ctrl.fetchUrl, { model: ctrl.getRequestParams() }).then(function (response) {
                ctrl.kanbanObj = response.data;
                ctrl._params = ctrl._params || {};
            });
        };

        ctrl.fetchColumnData = function (colIndex) {
            pageYOffset = window.pageYOffset;
            ctrl.kanbanObj.Columns[colIndex].Page += 1;
            var params = ctrl.getRequestParams();
            params.ColumnId = ctrl.kanbanObj.Columns[colIndex].Id;

            return $http
                .post(ctrl.fetchColumnUrl, { model: params })
                .then(function (response) {
                    ctrl.kanbanObj.Columns[colIndex].Cards.push.apply(ctrl.kanbanObj.Columns[colIndex].Cards, response.data);
                    document.body.classList.add('overflow-hidden');
                    $timeout(function () {
                        ctrl.setScrollAfterFetchColumnData();
                        document.body.classList.remove('overflow-hidden');
                    }, 0);
                })
                .catch(function () {
                    document.body.classList.remove('overflow-hidden');
                });
        };

        ctrl.onUpdateCard = function () {
            return $http.post(ctrl.fetchUrl, { model: ctrl.getRequestParams() }).then(function (response) {
                ctrl.kanbanObj.Columns = response.data.Columns;
            });
        };

        ctrl.setScrollAfterFetchColumnData = function () {
            window.scroll(window.pageXOffset, pageYOffset === window.pageYOffset ? pageYOffset + window.pageYOffset - pageYOffset : pageYOffset);
        };
    };

    KanbanCtrl.$inject = ['$http', '$location', '$window', '$timeout'];

    ng.module('kanban').controller('KanbanCtrl', KanbanCtrl);
})(window.angular);
