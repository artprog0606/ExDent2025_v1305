import modalReservationResourceShedulerTemplate from './../../bookingJournal/modal/reservationResourceSheduler/ModalReservationResourceSheduler.html';
(function (ng) {
    'use strict';

    /* @ngInject */
    var BookingShedulerCtrl = function ($q, $http, $location, $ocLazyLoad, $uibModal, SweetAlert, toaster, $timeout, $translate, bookingService) {
        var ctrl = this;
        ctrl.columnsFullCalendar = {};
        ctrl.calEventStatus = [];
        ctrl.calendarDropCallBack = null;
        ctrl.calendarEventRenderCallBack = null;
        ctrl.calendarEventAfterRenderCallBack = null;
        ctrl.calendarEventReceiveCallBack = null;
        ctrl.calendarEventDropCallBack = null;
        ctrl.calendarEventDragStartCallBack = null;
        ctrl.calendarEventDragStopCallBack = null;
        ctrl.calendarEventResizeCallBack = null;
        ctrl.calendarEventResizeStartCallBack = null;
        ctrl.calendarEventResizeStopCallBack = null;
        ctrl.calendarViewRenderCallBack = null;
        (ctrl._calendarOptions = {
            defaultView: 'agendaDay',
            droppable: ctrl.shedulerDraggable,
            dragRevertDuration: ctrl.shedulerDraggable ? 0 : undefined,
        }),
            (ctrl.$onInit = function () {
                ctrl._params = ctrl.shedulerParams;
                ctrl.optionsFromUrl();
                if (!ctrl._params.date) {
                    var date = new Date();
                    ctrl.date = date.getFullYear() + '-' + ('0' + (date.getMonth() + 1)).slice(-2) + '-' + ('0' + date.getDate()).slice(-2);
                    //ctrl.date = date;
                    ctrl._params.date = ctrl.date;
                } else {
                    ctrl.date = ctrl._params.date;
                }
                if (ctrl.shedulerDraggable === true) {
                    $ocLazyLoad
                        .load(
                            [
                                '../areas/admin/content/vendors/jquery-ui.draggable/jquery-ui.draggable.min.css',
                                '../areas/admin/content/vendors/jquery-ui.draggable/jquery-ui.draggable.min.js',
                            ],
                            {
                                serie: true,
                            },
                        )
                        .then(function () {
                            ctrl._onInit();
                        });
                } else {
                    ctrl._onInit();
                }
            }),
            (ctrl._onInit = function () {
                if (ctrl.shedulerScrollable === true) {
                    ctrl._shedulerScrollable = true;
                }
                ctrl._calendarOptions = ng.extend({}, ctrl.calendarOptions || {}, ctrl._calendarOptions);
                if (ctrl.shedulerDraggable === true || ctrl.shedulerScrollable === true) {
                    if (ctrl._calendarOptions.drop != null) {
                        ctrl.calendarDropCallBack = ctrl._calendarOptions.drop;
                    }
                    ctrl._calendarOptions.drop = ctrl.calendarDrop;
                    if (ctrl._calendarOptions.eventRender != null) {
                        ctrl.calendarEventRenderCallBack = ctrl._calendarOptions.eventRender;
                    }
                    ctrl._calendarOptions.eventRender = ctrl.calendarEventRender;
                    if (ctrl._calendarOptions.eventAfterRender != null) {
                        ctrl.calendarEventAfterRenderCallBack = ctrl._calendarOptions.eventAfterRender;
                    }
                    ctrl._calendarOptions.eventAfterRender = ctrl.calendarEventAfterRender;
                    if (ctrl._calendarOptions.eventReceive != null) {
                        ctrl.calendarEventReceiveCallBack = ctrl._calendarOptions.eventReceive;
                    }
                    ctrl._calendarOptions.eventReceive = ctrl.calendarEventReceive;
                    if (ctrl._calendarOptions.eventDrop != null) {
                        ctrl.calendarEventDropCallBack = ctrl._calendarOptions.eventDrop;
                    }
                    ctrl._calendarOptions.eventDrop = ctrl.calendarEventDrop;
                    if (ctrl._calendarOptions.eventDragStart != null) {
                        ctrl.calendarEventDragStartCallBack = ctrl._calendarOptions.eventDragStart;
                    }
                    ctrl._calendarOptions.eventDragStart = ctrl.calendarEventDragStart;
                    if (ctrl._calendarOptions.eventDragStop != null) {
                        ctrl.calendarEventDragStopCallBack = ctrl._calendarOptions.eventDragStop;
                    }
                    ctrl._calendarOptions.eventDragStop = ctrl.calendarEventDragStop;
                    if (ctrl._calendarOptions.eventResize != null) {
                        ctrl.calendarEventResizeCallBack = ctrl._calendarOptions.eventResize;
                    }
                    ctrl._calendarOptions.eventResize = ctrl.calendarEventResize;
                    if (ctrl._calendarOptions.eventResizeStart != null) {
                        ctrl.calendarEventResizeStartCallBack = ctrl._calendarOptions.eventResizeStart;
                    }
                    ctrl._calendarOptions.eventResizeStart = ctrl.calendarEventResizeStart;
                    if (ctrl._calendarOptions.eventResizeStop != null) {
                        ctrl.calendarEventResizeStopCallBack = ctrl._calendarOptions.eventResizeStop;
                    }
                    ctrl._calendarOptions.eventResizeStop = ctrl.calendarEventResizeStop;
                    if (ctrl._calendarOptions.viewRender != null) {
                        ctrl.calendarViewRenderCallBack = ctrl._calendarOptions.viewRender;
                    }
                    ctrl._calendarOptions.viewRender = ctrl.calendarViewRender;
                }
                ctrl.fetchData().then(function () {
                    if (ctrl.shedulerOnInit != null) {
                        ctrl.shedulerOnInit({
                            sheduler: ctrl,
                        });
                    }
                    if (ctrl.shedulerScrollable === true) {
                        dragscroll.reset();
                    }
                });
            }),
            (ctrl.getCalendarOptions = function (column) {
                return ng.extend({}, ctrl._calendarOptions, {
                    defaultDate: column.Date,
                    slotDuration: column.BookingDuration,
                    slotLabelInterval: column.BookingDuration,
                    minTime: column.MinTime,
                    maxTime: column.MaxTime,
                    events: function (start, end, timezone, callback) {
                        var col = ctrl.shedulerObj.Columns.filter(function (c) {
                            return c.Id === column.Id;
                        })[0];
                        callback(col.Events || []);
                    },
                });
            });
        ctrl.fullCalendarOnInit = function (column, calendar) {
            ctrl.columnsFullCalendar[column.Id] = calendar;
        };
        ctrl.fullCalendarOnDestroy = function (column) {
            ctrl.columnsFullCalendar[column.Id] = null;
        };

        //#region Calendar events

        (ctrl.calendarDrop = function (date, jsEvent, ui) {
            console.log('calendarDrop');
            if (ctrl.shedulerDraggable === true) {
                if (typeof ctrl.calEventStatus['calendar'] != 'undefined') {
                    /*todo changed events
          var event = ctrl.calEventStatus['calendar'].fullCalendar('clientEvents', ctrl.calEventStatus['event_id'])[0];
          ctrl.calEventStatus['calendar'].fullCalendar('removeEvents', ctrl.calEventStatus['event_id']);*/
                    /*todo changed events
          if (event.source) {
              var column = event.source.calendar.el.closest('[data-columnindex]');
              var colIndex = parseInt(column.attr('data-columnindex'), 10);
                if (ctrl.shedulerObj.Columns[colIndex].Events) {
                  ctrl.shedulerObj.Columns[colIndex].Events = ctrl.shedulerObj.Columns[colIndex].Events.filter(function(ev) {
                      return ev.id !== event.id;
                  });
              }
          }*/
                    //$(ctrl.calEventStatus['calendar']).fullCalendar('unselect');
                }
                ctrl.makeEventsDraggable();
            }
            if (ctrl.calendarDropCallBack != null) {
                ctrl.calendarDropCallBack(date, jsEvent, ui);
            }
        }),
            (ctrl.calendarEventRender = function (event, element, view) {
                if (event.description) {
                    element.popover({
                        //title: event.title,
                        content: event.description,
                        trigger: 'hover',
                        placement: 'top',
                        container: 'body',
                    });
                }
                if (ctrl.calendarEventRenderCallBack != null) {
                    ctrl.calendarEventRenderCallBack(event, element, view);
                }
            }),
            (ctrl.calendarEventAfterRender = function (event, element, view) {
                console.log('calendarEventAfterRender');
                if (ctrl.shedulerDraggable === true) {
                    if (event.id && !element.data('event') && event.end) {
                        element.data(
                            'event',
                            ng.extend(
                                {
                                    stick: true,
                                    oldStart: event.start,
                                    oldEnd: event.end,
                                },
                                event,
                            ),
                        );

                        // make the event draggable using jQuery UI
                        element.draggable({
                            duration: moment.utc(event.end.diff(event.start)).format('HH:mm:ss'),
                            zIndex: 999,
                            revert: true,
                            // will cause the event to go back to its
                            revertDuration: 0, //  original position after the drag
                        });
                        console.log('makeEventsDraggable');
                    }
                }
                if (ctrl.calendarEventAfterRenderCallBack != null) {
                    ctrl.calendarEventAfterRenderCallBack(event, element, view);
                }
            }),
            (ctrl.calendarEventReceive = function (event) {
                console.log('calendarEventReceive');
                if (ctrl.shedulerDraggable === true) {
                    if (event.oldEnd) {
                        event.end = moment(event.start).add(event.oldEnd.diff(event.oldStart));
                    }

                    /*todo changed events
        if (event.source) {
        var column = event.source.calendar.el.closest('[data-columnindex]');
        var colIndex = parseInt(column.attr('data-columnindex'), 10);
          if (ctrl.shedulerObj.Columns[colIndex].Events) {
            ctrl.shedulerObj.Columns[colIndex].Events.push(event);
        }
        }*/

                    ctrl.makeEventsDraggable();
                }
                var aftercolIndex = null;
                var beforeColIndex = null;
                for (var i = 0; i < ctrl.shedulerObj.Columns.length; i++) {
                    if (ctrl.shedulerObj.Columns[i].Id == event.reservationResourceId) {
                        beforeColIndex = i;
                        break;
                    }
                }
                var column = event.source.calendar.el.closest('[data-columnindex]');
                aftercolIndex = parseInt(column.attr('data-columnindex'), 10);
                event.reservationResourceId = ctrl.shedulerObj.Columns[aftercolIndex].Id;
                ctrl.updateBookingAfterDrag(
                    event.id,
                    event.reservationResourceId,
                    event.start,
                    event.end,
                    beforeColIndex,
                    aftercolIndex,
                    null,
                    false,
                );
                if (ctrl.calendarEventReceiveCallBack != null) {
                    ctrl.calendarEventReceiveCallBack(event);
                }
            }),
            (ctrl.calendarEventDrop = function (event, delta, revertFunc, jsEvent, ui, view) {
                console.log('calendarEventDrop');
                if (ctrl.shedulerDraggable === true) {
                    ctrl.makeEventsDraggable();
                }
                var colIndex = parseInt($(jsEvent.target).closest('[data-columnindex]').attr('data-columnindex'), 10);
                if (isNaN(colIndex)) {
                    for (var i = 0; i < ctrl.shedulerObj.Columns.length; i++) {
                        if (ctrl.shedulerObj.Columns[i].Id == event.reservationResourceId) {
                            colIndex = i;
                            break;
                        }
                    }
                }
                $q.when(
                    ctrl.dateDragStart && ctrl.dateDragStop && ctrl.dateDragStop - ctrl.dateDragStart >= 100 // время перетаскивания больше 100мс
                        ? true
                        : SweetAlert.confirm('Кажется это действие выполнено случайно. Вы действительно хотите перенести бронь?', {
                              title: 'Перенос брони',
                              confirmButtonText: 'Да, перенести',
                          }),
                )
                    .then(function (result) {
                        if (result === true || result.value) {
                            ctrl.updateBookingAfterDrag(
                                event.id,
                                event.reservationResourceId,
                                event.start,
                                event.end,
                                colIndex,
                                colIndex,
                                revertFunc,
                                false,
                            );
                            if (ctrl.calendarEventDropCallBack != null) {
                                ctrl.calendarEventDropCallBack(event, delta, revertFunc, jsEvent, ui, view);
                            }
                            return true;
                        } else {
                            return $q.reject('Exception');
                        }
                    })
                    .catch(function () {
                        if (revertFunc) {
                            revertFunc();
                        } else {
                            ctrl.fetchColumnData(colIndex);
                        }
                    });
            }),
            (ctrl.calendarEventDragStart = function (event, jsEvent, ui, view) {
                console.log('calendarEventDragStart');
                ctrl.dateDragStart = new Date();
                if (ctrl.shedulerScrollable === true) {
                    ctrl._shedulerScrollable = false;
                    dragscroll.reset();
                }
                if (ctrl.shedulerDraggable === true) {
                    ctrl.calEventStatus['calendar'] = view.el.closest('.fc');
                    ctrl.calEventStatus['event_id'] = event._id;
                }
                if (ctrl.calendarEventDragStartCallBack != null) {
                    ctrl.calendarEventDragStartCallBack(event, jsEvent, ui, view);
                }
            }),
            (ctrl.calendarEventDragStop = function (event, jsEvent, ui, view) {
                console.log('calendarEventDragStop');
                ctrl.dateDragStop = new Date();
                if (ctrl.shedulerScrollable === true) {
                    ctrl._shedulerScrollable = true;
                    dragscroll.reset();
                }
                if (ctrl.shedulerDraggable === true) {
                    ctrl.makeEventsDraggable();
                }
                if (ctrl.calendarEventDragStopCallBack != null) {
                    ctrl.calendarEventDragStopCallBack(event, jsEvent, ui, view);
                }
            }),
            (ctrl.calendarEventResize = function (event, delta, revertFunc, jsEvent, ui, view) {
                console.log('calendarEventResize');
                if (ctrl.shedulerDraggable === true) {
                    ctrl.makeEventsDraggable();
                }
                var colIndex = parseInt($(jsEvent.target).closest('[data-columnindex]').attr('data-columnindex'), 10);
                if (isNaN(colIndex)) {
                    for (var i = 0; i < ctrl.shedulerObj.Columns.length; i++) {
                        if (ctrl.shedulerObj.Columns[i].Id == event.reservationResourceId) {
                            colIndex = i;
                            break;
                        }
                    }
                }
                $q.when(
                    ctrl.dateResizeStart && ctrl.dateResizeStop && ctrl.dateResizeStop - ctrl.dateResizeStart >= 100 // время перетаскивания больше 100мс
                        ? true
                        : SweetAlert.confirm('Кажется это действие выполнено случайно. Вы действительно хотите изменить продолжительность брони?', {
                              title: 'Изменение брони',
                              confirmButtonText: 'Да, изменить',
                          }),
                )
                    .then(function (result) {
                        if (result === true) {
                            ctrl.updateBookingAfterDrag(
                                event.id,
                                event.reservationResourceId,
                                event.start,
                                event.end,
                                colIndex,
                                colIndex,
                                revertFunc,
                                false,
                            );
                            if (ctrl.calendarEventResizeCallBack != null) {
                                ctrl.calendarEventResizeCallBack(event, delta, revertFunc, jsEvent, ui, view);
                            }
                            return true;
                        } else {
                            return $q.reject('Exception');
                        }
                    })
                    .catch(function () {
                        if (revertFunc) {
                            revertFunc();
                        } else {
                            ctrl.fetchColumnData(colIndex);
                        }
                    });
            }),
            (ctrl.calendarEventResizeStart = function (event, jsEvent, ui, view) {
                console.log('calendarEventResizeStart');
                ctrl.dateResizeStart = new Date();
                if (ctrl.shedulerDraggable === true) {
                    $(jsEvent.target).closest('.fc-draggable').draggable('disable');
                }
                if (ctrl.shedulerScrollable === true) {
                    ctrl._shedulerScrollable = false;
                    dragscroll.reset();
                }
                if (ctrl.calendarEventResizeStartCallBack != null) {
                    ctrl.calendarEventResizeStartCallBack(event, jsEvent, ui, view);
                }
            }),
            (ctrl.calendarEventResizeStop = function (event, jsEvent, ui, view) {
                console.log('calendarEventResizeStop');
                ctrl.dateResizeStop = new Date();
                if (ctrl.shedulerDraggable === true) {
                    $(jsEvent.target).closest('.fc-draggable').draggable('enable');
                }
                if (ctrl.shedulerScrollable === true) {
                    ctrl._shedulerScrollable = true;
                    dragscroll.reset();
                }
                if (ctrl.calendarEventResizeStopCallBack != null) {
                    ctrl.calendarEventResizeStopCallBack(event, jsEvent, ui, view);
                }
            }),
            (ctrl.calendarViewRender = function (view, element) {
                console.log('calendarViewRender');
                if (ctrl.shedulerDraggable === true) {
                    ctrl.makeEventsDraggable();
                }
                if (ctrl.calendarViewRenderCallBack != null) {
                    ctrl.calendarViewRenderCallBack(view, element);
                }
            });

        //#endregion

        //#region Filter

        ctrl.onFilterInit = function (filter) {
            ctrl.filter = filter;
            if (ctrl.shedulerOnFilterInit != null) {
                ctrl.shedulerOnFilterInit({
                    filter: filter,
                });
            }
        };
        ctrl.optionsFromUrl = function () {
            var shedulerParamsByUrl = ctrl.getParamsByUrl(ctrl.uid);
            if (shedulerParamsByUrl != null) {
                ng.extend(ctrl._params, shedulerParamsByUrl);
            }
        };
        ctrl.filterApply = function (params, item) {
            if (ng.isArray(params) === false) {
                throw new Error('Parameter "params" should be array');
            }
            for (var i = 0, len = params.length; i < len; i++) {
                ctrl._params[params[i].name] = params[i].value;
            }
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
            ctrl.fetchData().then(function () {
                ctrl.setParamsByUrl(ctrl._params);
            });
        };
        ctrl.setParamsByUrl = function (params) {
            $location.search(ctrl.uid, JSON.stringify(params));
        };
        ctrl.getParamsByUrl = function (uid) {
            return JSON.parse($location.search()[uid] || null);
        };
        ctrl.getRequestParams = function () {
            var params = {};
            ng.extend(params, ctrl._params);
            return params;
        };
        ctrl.fetchColumnData = function (colIndex) {
            console.log('fetchColumnData');
            var params = ctrl.getRequestParams();
            params.ColumnId = ctrl.shedulerObj.Columns[colIndex].Id;
            params.compactView = ctrl.compactView;
            ctrl.shedulerProcessing = true;
            return $http
                .post(ctrl.fetchColumnUrl, {
                    model: params,
                })
                .then(function (response) {
                    var column = ctrl.shedulerObj.Columns[colIndex];
                    column.Events = response.data;
                    if (!column.IsNotWork) {
                        var fullCalendar = ctrl.columnsFullCalendar[column.Id];
                        if (fullCalendar) {
                            fullCalendar.removeEvents(); //удаляем, т.к. перенесенное остается
                            fullCalendar.refetchEvents();
                        } else {
                            toaster.pop('error', 'Не удалось обновись столбец данными, т.к. fullCalendar не доступен');
                        }
                    }
                    ctrl.shedulerProcessing = false;
                });
        };
        ctrl.fetchData = function () {
            console.log('fetchData');
            var params = ctrl.getRequestParams();
            params.compactView = ctrl.compactView;
            ctrl.shedulerProcessing = true;
            return $http
                .post(ctrl.fetchUrl, {
                    model: params,
                })
                .then(function (response) {
                    var isFirstFetch = !ctrl.shedulerObj;
                    if (!ctrl.compactView) {
                        ctrl.calcSlotHeight(response.data);
                        ctrl.showStyle = true;
                    } else {
                        ctrl.showStyle = false;
                    }
                    ctrl.shedulerObj = response.data;
                    ctrl._params = ctrl._params || {};
                    $timeout(function () {
                        if (!isFirstFetch && ctrl.shedulerObj.Columns) {
                            ctrl.shedulerObj.Columns.forEach(function (column) {
                                if (!column.IsNotWork) {
                                    var fullCalendar = ctrl.columnsFullCalendar[column.Id];
                                    if (fullCalendar) {
                                        var options = ctrl.getCalendarOptions(column);
                                        var oldDate = fullCalendar.optionsManager.get('defaultDate') !== options.defaultDate;
                                        fullCalendar.option(options);
                                        fullCalendar.removeEvents();
                                        if (oldDate) {
                                            fullCalendar.gotoDate(fullCalendar.optionsManager.get('defaultDate'));
                                        } else {
                                            fullCalendar.refetchEvents();
                                        }
                                    }
                                }
                            });
                        }
                        ctrl.shedulerProcessing = false;
                    });
                });
        };

        //#endregion

        ctrl.changeCompactView = function (val) {
            var reFetch = ctrl.compactView !== val;
            ctrl.compactView = val;
            if (reFetch) {
                ctrl.fetchData();
            }
        };
        ctrl.calcSlotHeight = function (data) {
            if (data.Columns.length) {
                data.Columns.forEach(function (col) {
                    col.MinTimeInMinutes = ctrl.GetDurationsMinutes(col.MinTime);
                });
                var minStartTime = data.Columns.reduce(function (min, col) {
                    return min != null && min < col.MinTimeInMinutes ? min : col.MinTimeInMinutes;
                }, null);
                var defaultDurationMinutes = ctrl.GetDurationsMinutes(data.DefaultBookingDuration);
                data.Columns.forEach(function (col) {
                    if (data.DefaultBookingDuration !== col.BookingDuration) {
                        col.durationMinutes = ctrl.GetDurationsMinutes(col.BookingDuration);
                        col.coeffSlotHeight = col.durationMinutes / defaultDurationMinutes;
                    } else {
                        col.durationMinutes = defaultDurationMinutes;
                        col.coeffSlotHeight = 1.0;
                    }
                });
                var slotHeight = ctrl.slotHeightPx;
                var minSlotHeight = ctrl.minSlotHeightPx || ctrl.slotHeightPx;
                if (!slotHeight || !minSlotHeight) {
                    slotHeight = minSlotHeight = 1.5;
                }
                data.Columns.forEach(function (col) {
                    col.slotHeight = (slotHeight * col.coeffSlotHeight).toFixed(ctrl.slotHeightPx ? 0 : 2);
                    col.offset = (col.slotHeight / col.durationMinutes) * (col.MinTimeInMinutes - minStartTime);
                });
                var minHeight = Math.min.apply(
                    null,
                    data.Columns.map(function (col) {
                        return col.slotHeight;
                    }),
                );
                if (minHeight < minSlotHeight) {
                    var correctCoeff = minSlotHeight / minHeight;
                    data.Columns.forEach(function (col) {
                        col.coeffSlotHeight *= correctCoeff;
                    });
                    data.Columns.forEach(function (col) {
                        col.slotHeight = (slotHeight * col.coeffSlotHeight).toFixed(ctrl.slotHeightPx ? 0 : 2);
                        col.offset = (col.slotHeight / col.durationMinutes) * (col.MinTimeInMinutes - minStartTime);
                    });
                }
            }
        };
        ctrl.GetDurationsMinutes = function (duration) {
            var durations = duration.split(':').map(function (item) {
                return parseInt(item, 10);
            });
            return durations[0] * 60 + durations[1];
        };
        ctrl.onFlatpickrInit = function (flatpickr) {
            ctrl.flatpickr = flatpickr;
            ctrl.flatpickr.jumpToDate(new Date(ctrl.date));
        };
        ctrl.changeDate = function () {
            //var isNewDate = ctrl._params.date !== ctrl.date;
            ctrl._params.date = ctrl.date;
            ctrl.fetchData().then(function () {
                ctrl.setParamsByUrl(ctrl._params);
                ctrl.selectedEmployeeName = ctrl.shedulerObj.Columns[0];

                /*if (isNewDate) {
            // переводим все FullCalendar на выбранную дату
            for (var colId in ctrl.columnsFullCalendar) {
                if (ctrl.columnsFullCalendar.hasOwnProperty(colId)) {
                    var fullCalendar = ctrl.columnsFullCalendar[colId];
                    if (fullCalendar) {
                        fullCalendar.removeEvents();
                        fullCalendar.gotoDate(ctrl.date);
                    }
                }
            }
        }*/
            });
        };
        ctrl.showReservationResourceSheduler = function (affiliateId, reservationResourceId, date, slotHeightPx, name, bookingDuration) {
            var fnModalShedulerClose = function (result) {
                if (result && result.reservationResourcesChanged) {
                    result.reservationResourcesChanged.forEach(function (item) {
                        for (var index = 0; index < ctrl.shedulerObj.Columns.length; index++) {
                            if (ctrl.shedulerObj.Columns[index].Id == item) {
                                ctrl.fetchColumnData(index);
                                break;
                            }
                        }
                    });
                }
            };
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalReservationResourceShedulerCtrl',
                    controllerAs: 'ctrl',
                    size: 'xs-11',
                    //backdrop: 'static',
                    templateUrl: modalReservationResourceShedulerTemplate,
                    resolve: {
                        params: {
                            affiliateId: affiliateId,
                            reservationResourceId: reservationResourceId,
                            date: date,
                            slotHeightPx: slotHeightPx,
                            name: name,
                            bookingDuration: bookingDuration,
                            mode: 'edit',
                        },
                    },
                })
                .result.then(
                    function (result) {
                        fnModalShedulerClose(result);
                        return result;
                    },
                    function (result) {
                        fnModalShedulerClose(result);
                        return result;
                    },
                );
        };
        ctrl.updateBookingAfterDrag = function (
            id,
            reservationResourceId,
            beginMoment,
            endMoment,
            beforeColIndex,
            aftercolIndex,
            revertFunc,
            userConfirmed,
        ) {
            var beginDate = bookingService.transformDate(beginMoment, true);
            var endDate = bookingService.transformDate(endMoment, true);
            function fetchColumns() {
                ctrl.fetchColumnData(beforeColIndex);
                if (beforeColIndex !== aftercolIndex) {
                    ctrl.fetchColumnData(aftercolIndex);
                }
            }
            return bookingService
                .updateBookingAfterDrag(id, reservationResourceId, beginDate, endDate, userConfirmed)
                .then(function (data) {
                    if (data.result === true) {
                        if (data.obj && data.obj.UserConfirmIsRequired) {
                            SweetAlert.confirm(data.obj.ConfirmMessage, {
                                title: $translate.instant('Admin.Js.BookingJournal.SaveReservation'),
                                confirmButtonText: data.obj.ConfirmButtomText,
                            }).then(
                                function (result) {
                                    if (result === true || result.value) {
                                        ctrl.updateBookingAfterDrag(
                                            id,
                                            reservationResourceId,
                                            beginMoment,
                                            endMoment,
                                            beforeColIndex,
                                            aftercolIndex,
                                            revertFunc,
                                            true,
                                        );
                                    } else {
                                        if (revertFunc) {
                                            revertFunc();
                                        } else {
                                            fetchColumns();
                                        }
                                    }
                                },
                                function () {
                                    if (revertFunc) {
                                        revertFunc();
                                    } else {
                                        fetchColumns();
                                    }
                                },
                            );
                        } else {
                            toaster.pop('success', '', $translate.instant('Admin.Js.BookingJournal.ReservationSuccessfullySaved'));
                            fetchColumns();
                        }
                    } else {
                        if (revertFunc) {
                            revertFunc();
                        } else {
                            fetchColumns();
                        }
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                        if (!data.errors) {
                            toaster.pop('error', $translate.instant('Admin.Js.BookingJournal.ErrorWhileSavingReservation'));
                        }
                    }
                })
                .catch(function () {
                    if (revertFunc) {
                        revertFunc();
                    } else {
                        fetchColumns();
                    }
                    toaster.pop('error', $translate.instant('Admin.Js.BookingJournal.ErrorSavingReservation'));
                });
        };
        ctrl.makeEventsDraggable = function () {
            //console.log('makeEventsDraggable2');
            //$("td").removeClass("fc-highlight");
            return;
            //$('.fc-draggable').each(function () {

            //    if (!$(this).data('event')) {
            //        // store data so the calendar knows to render an event upon drop
            //        $(this).data('event', Object.assign({
            //            //title: $.trim($(this).text()), // use the element's text as the event title
            //            stick: true, // maintain when user navigates (see docs on the renderEvent method)
            //        }, {}));

            //        // make the event draggable using jQuery UI
            //        $(this).draggable({
            //            zIndex: 999,
            //            revert: true, // will cause the event to go back to its
            //            revertDuration: 0 //  original position after the drag
            //        });

            //        console.log('makeEventsDraggable');

            //        // Dirty fix to remove highlighted blue background
            //    }
            //});
            //$("td").removeClass("fc-highlight");
        };
    };
    ng.module('bookingSheduler').controller('BookingShedulerCtrl', BookingShedulerCtrl);
})(window.angular);
