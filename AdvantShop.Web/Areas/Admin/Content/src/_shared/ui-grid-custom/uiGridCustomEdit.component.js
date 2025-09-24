import uiGridCustomEditTemplate from './templates/ui-grid-custom-edit.html';
(function (ng) {
    'use strict';

    ng.module('uiGridCustomEdit').directive('uiGridCustomEdit', function () {
        return {
            replace: true,
            priority: 0,
            require: '^uiGrid',
            scope: false,
            compile: function () {
                return {
                    pre: function (scope, element, attrs, uiGridCtrl) {
                        uiGridCtrl.grid.api.registerEventsFromObject({
                            uiGridEditCustom: {
                                change: null,
                            },
                        });
                    },
                    post: function ($scope, $elm, $attrs, uiGridCtrl) {},
                };
            },
        };
    });
    ng.module('uiGridCustomEdit')
        .constant('INPLACE_TYPES', {
            SINGLE: 'single',
            MULTIPLE: 'multiple',
        })
        .directive('uiGridCellCustom', [
            '$compile',
            '$parse',
            '$q',
            '$templateRequest',
            'urlHelper',
            '$filter',
            'INPLACE_TYPES',
            function ($compile, $parse, $q, $templateRequest, urlHelper, $filter, INPLACE_TYPES) {
                return {
                    priority: -100,
                    // run after default uiGridCell directive
                    restrict: 'A',
                    // require: ['^uiGrid'],
                    scope: false,
                    compile: function () {
                        return {
                            pre: function (scope, element, attrs) {},
                            post: function (scope, element, attrs, ctrls) {
                                const col = $parse(attrs.col)(scope);
                                const onInitCallback = $parse(attrs.onInit)(scope);
                                // const columnDefs = $parse(attrs.columnDefs)(scope);
                                scope.INPLACE_TYPES = INPLACE_TYPES;
                                const row = $parse(attrs.row)(scope);
                                scope.col = col;
                                scope.row = row;
                                if (onInitCallback) {
                                    onInitCallback(scope);
                                }
                                var uiGridCustomEditOptions = scope.col.colDef.uiGridCustomEdit || {},
                                    uiGridCustomEditAttributes = uiGridCustomEditOptions != null ? uiGridCustomEditOptions.attributes : null,
                                    modelName,
                                    viewValueName,
                                    isHover,
                                    rowElement = element.parent(),
                                    isEditInit = false,
                                    isMobile = $parse(attrs.isMobile)(scope);
                                // проверить
                                var getModelValue = function () {
                                    var result;
                                    if (uiGridCustomEditOptions.customModel != null) {
                                        result = scope.uiGridEditCustom[uiGridCustomEditOptions.customModel];
                                    } else {
                                        result = scope.row.entity[scope.col.colDef.name];
                                    }
                                    return result;
                                };
                                scope.uiGridEditCustom = {
                                    isMobile: isMobile,
                                };
                                if (uiGridCustomEditOptions.customModel != null) {
                                    modelName = 'uiGridEditCustom.' + uiGridCustomEditOptions.customModel;
                                } else if (uiGridCustomEditOptions.modelFromCol != null) {
                                    modelName = "row.entity['" + uiGridCustomEditOptions.modelFromCol + "']"; //кавычки местами поменял, чтобы в верстке не дублировались двойные кавычки
                                } else {
                                    modelName = scope.row.getQualifiedColField(scope.col);
                                }
                                if (uiGridCustomEditOptions.customViewValue != null) {
                                    viewValueName = 'uiGridEditCustom.' + uiGridCustomEditOptions.customViewValue;
                                } else if (uiGridCustomEditOptions.viewValueFromCol != null) {
                                    viewValueName = "row.entity['" + uiGridCustomEditOptions.viewValueFromCol + "']"; //кавычки местами поменял, чтобы в верстке не дублировались двойные кавычки
                                } else {
                                    // viewValueName = scope.row.getQualifiedColField(
                                    //     scope.col,
                                    // );
                                    viewValueName = row.getQualifiedColField(scope.col);
                                }
                                function shouldEdit(col, row) {
                                    const isEditable = $filter('isEditable');
                                    return isEditable(col, row, scope);
                                    // return (
                                    //     !row.isSaving &&
                                    //     col.colDef.enableCellEdit &&
                                    //     (ng.isFunction(
                                    //         col.colDef.cellEditableCondition,
                                    //     )
                                    //         ? col.colDef.cellEditableCondition(
                                    //               scope,
                                    //           )
                                    //         : true)
                                    // );
                                }
                                function setEditable() {
                                    if (shouldEdit(scope.col, scope.row)) {
                                        scope.isFocus = false;
                                        var timerHover;
                                        if (!isMobile) {
                                            scope.$on('uiGridCustomRowMouseEnter', function ($event, row) {
                                                isHover = true;
                                                if (timerHover != null) {
                                                    clearTimeout(timerHover);
                                                }
                                                timerHover = setTimeout(function () {
                                                    if (isHover === true) {
                                                        $q.when(isEditInit === false ? editInit() : true).then(function () {
                                                            if (uiGridCustomEditOptions.onActive != null) {
                                                                if (
                                                                    scope.isFocus === false &&
                                                                    scope.uiGridEditCustom.form != null &&
                                                                    scope.uiGridEditCustom.form.$valid === true
                                                                ) {
                                                                    uiGridCustomEditOptions.onActive(
                                                                        row.entity,
                                                                        scope.col.colDef,
                                                                        getModelValue(),
                                                                        scope.uiGridEditCustom,
                                                                    );
                                                                }
                                                            }
                                                        });
                                                        isEditInit = true;
                                                    }
                                                }, 200);
                                            });
                                            scope.$on('uiGridCustomRowMouseLeave', function ($event, row) {
                                                isHover = false;
                                                if (uiGridCustomEditOptions.onDeactive != null) {
                                                    if (
                                                        scope.isFocus === false &&
                                                        scope.uiGridEditCustom.form != null &&
                                                        scope.uiGridEditCustom.form.$valid === true
                                                    ) {
                                                        uiGridCustomEditOptions.onDeactive(
                                                            row.entity,
                                                            scope.col.colDef,
                                                            getModelValue(),
                                                            scope.uiGridEditCustom,
                                                        );
                                                    }
                                                    scope.uiGridEditCustom.state = 'default';
                                                }
                                                scope.$digest();
                                            });
                                        }
                                        if (isMobile === true) {
                                            editInit();
                                        }
                                    }
                                }
                                function getSelectOptions() {
                                    var promise;
                                    if (uiGridCustomEditOptions.editDropdownOptionsFunction) {
                                        promise = $q
                                            .when(uiGridCustomEditOptions.editDropdownOptionsFunction(scope.row.entity, scope.col.colDef))
                                            .then(function (result) {
                                                scope.uiGridEditCustom.editDropdownOptionsArray = result;
                                            });
                                    } else if (uiGridCustomEditOptions.editDropdownRowEntityOptionsArrayPath) {
                                        promise = scope.uiGridEditCustom.editDropdownOptionsArray = resolveObjectFromPath(
                                            scope.row.entity,
                                            uiGridCustomEditOptions.editDropdownRowEntityOptionsArrayPath,
                                        );
                                    } else if (uiGridCustomEditOptions.editDropdownOptionsArray != null) {
                                        promise = scope.uiGridEditCustom.editDropdownOptionsArray = uiGridCustomEditOptions.editDropdownOptionsArray;
                                    } else {
                                        promise = true;
                                    }
                                    return $q.when(promise);
                                }
                                function editInit() {
                                    scope.uiGridEditCustom.focus = function ($event, value) {
                                        scope.isFocus = true;
                                        scope.uiGridEditCustom.editOldValue = value;
                                        if (uiGridCustomEditOptions.onActive != null && scope.uiGridEditCustom.form.$valid === true) {
                                            uiGridCustomEditOptions.onActive(scope.row.entity, scope.col.colDef, value, scope.uiGridEditCustom);
                                        }
                                    };
                                    scope.uiGridEditCustom.change = function (rowEntity, colDef, newVal, eventType, uiGridCustom) {
                                        var deferred = $q.defer();
                                        var gridAPI = null;
                                        if (scope.grid) {
                                            gridAPI = scope.grid.api;
                                        } else if (uiGridCustom) {
                                            gridAPI = uiGridCustom.gridApi.grid.api;
                                        }
                                        scope.isFocus = false;
                                        if (newVal !== scope.uiGridEditCustom.editOldValue) {
                                            if (
                                                (newVal == null || newVal.length === 0) &&
                                                (uiGridCustomEditOptions.replaceNullable === true || uiGridCustomEditOptions.replaceNullable == null)
                                            ) {
                                                //rowEntity[colDef.name] = scope.uiGridEditCustom.editOldValue;
                                                uiGridCustomEditOptions.customModel != null
                                                    ? (scope.uiGridEditCustom[uiGridCustomEditOptions.customModel] =
                                                          scope.uiGridEditCustom.editOldValue)
                                                    : (rowEntity[colDef.name] = scope.uiGridEditCustom.editOldValue);
                                                if (uiGridCustomEditOptions.onDeactive != null) {
                                                    deferred.resolve(
                                                        uiGridCustomEditOptions.onDeactive(
                                                            rowEntity,
                                                            colDef,
                                                            scope.uiGridEditCustom.editOldValue,
                                                            scope.uiGridEditCustom,
                                                        ),
                                                    );
                                                }
                                            } else {
                                                //rowEntity[colDef.name] = newVal;

                                                uiGridCustomEditOptions.customModel != null
                                                    ? (scope.uiGridEditCustom[uiGridCustomEditOptions.customModel] = newVal)
                                                    : (rowEntity[colDef.name] = newVal);
                                                if (uiGridCustomEditOptions.onChange != null) {
                                                    uiGridCustomEditOptions.onChange(rowEntity, colDef, newVal, scope.uiGridEditCustom);
                                                }
                                                if (gridAPI) {
                                                    gridAPI.uiGridEditCustom.raise.change(
                                                        rowEntity,
                                                        colDef,
                                                        newVal,
                                                        scope.uiGridEditCustom.editOldValue,
                                                        function (rowEntity, colDef, newVal, oldValue) {
                                                            if (uiGridCustomEditOptions.onDeactive != null) {
                                                                uiGridCustomEditOptions.onDeactive(rowEntity, colDef, newVal, scope.uiGridEditCustom);
                                                            }
                                                        },
                                                        eventType,
                                                        deferred,
                                                    );
                                                }
                                            }
                                        } else {
                                            if (uiGridCustomEditOptions.onDeactive != null) {
                                                uiGridCustomEditOptions.onDeactive(rowEntity, colDef, newVal, scope.uiGridEditCustom);
                                                deferred.resolve();
                                            }
                                        }
                                        return deferred.promise;
                                        //scope.grid.api.core.notifyDataChange(uiGridConstants.dataChange.EDIT);
                                    };
                                    scope.uiGridEditCustom.keyup = function (rowEntity, colDef, newVal, event) {
                                        switch (event.keyCode) {
                                            case 13:
                                                document.activeElement.blur();
                                                break;
                                            case 27:
                                                newVal = scope.uiGridEditCustom.editOldValue;
                                                break;
                                        }
                                    };
                                    scope.uiGridEditCustom.selectToggle = function ($event, isOpen, value) {
                                        if (isOpen === true) {
                                            $q.when(getSelectOptions()).then(function () {
                                                scope.uiGridEditCustom.focus($event, value);
                                            });
                                        }
                                    };
                                    scope.uiGridEditCustom.isShowInput = function () {
                                        return (isHover || scope.isFocus || scope.uiGridEditCustom.isMobile) && shouldEdit(scope.col, scope.row);
                                    };
                                    return $templateRequest(uiGridCustomEditTemplate).then(function (template) {
                                        var tpl = template
                                            .replace(/MODEL_CUSTOM_EDIT/g, modelName)
                                            .replace(/COL_DEF_CUSTOM_EDIT/g, viewValueName)
                                            .replace(/INPUT_ATTRIBUTES/g, attributesToString(uiGridCustomEditAttributes));
                                        var cellElement = ng.element(tpl);
                                        var preloadSelectItem = {};
                                        if (scope.col.colDef.type === 'select' || scope.col.colDef.type === 'ui-select') {
                                            scope.uiGridEditCustom.editDropdownIdLabel = uiGridCustomEditOptions.editDropdownIdLabel || 'value';
                                            scope.uiGridEditCustom.editDropdownValueLabel = uiGridCustomEditOptions.editDropdownValueLabel || 'label';
                                            if (uiGridCustomEditOptions.editDropdownOptionsArray == null) {
                                                scope.uiGridEditCustom.editDropdownOptionsArray = [];
                                                preloadSelectItem[scope.uiGridEditCustom.editDropdownIdLabel] = $parse(modelName)(scope);
                                                preloadSelectItem[scope.uiGridEditCustom.editDropdownValueLabel] = $parse(viewValueName)(scope);
                                                scope.uiGridEditCustom.editDropdownOptionsArray.push(preloadSelectItem);
                                            } else {
                                                scope.uiGridEditCustom.editDropdownOptionsArray = uiGridCustomEditOptions.editDropdownOptionsArray;
                                            }
                                        }
                                        element.html(cellElement);
                                        $compile(cellElement)(scope);
                                        element.off('dblclick');
                                    });
                                }
                                var rowInit = false;
                                var rowWatchDereg = scope.$watch('row', function (newValue, oldValue) {
                                    if (uiGridCustomEditOptions.onInit != null && rowInit === false) {
                                        uiGridCustomEditOptions.onInit(scope.row.entity, scope.col.colDef, getModelValue(), scope.uiGridEditCustom);
                                    }
                                    rowInit = true;
                                    setEditable();
                                });
                                scope.$on('$destroy', function destroyEvents() {
                                    rowWatchDereg();
                                    // unbind all jquery events in order to avoid memory leaks
                                    element.off();
                                    rowElement.off('mouseenter');
                                    rowElement.off('mouseleave');
                                });
                            },
                        };
                    },
                };
            },
        ]);
    function attributesToString(attributes) {
        var result = '';
        if (attributes != null) {
            for (var key in attributes) {
                if (Object.prototype.hasOwnProperty.call(attributes, key) === true) {
                    result += [' ', key, '=', attributes[key]].join('');
                }
            }
        }
        return result;
    }

    // resolves a string path against the given object
    // shamelessly borrowed from
    // http://stackoverflow.com/questions/6491463/accessing-nested-javascript-objects-with-string-key
    function resolveObjectFromPath(object, path) {
        path = path.replace(/\[(\w+)\]/g, '.$1'); // convert indexes to properties
        path = path.replace(/^\./, ''); // strip a leading dot
        var a = path.split('.');
        while (a.length) {
            var n = a.shift();
            if (n in object) {
                object = object[n];
            } else {
                return;
            }
        }
        return object;
    }
})(window.angular);
