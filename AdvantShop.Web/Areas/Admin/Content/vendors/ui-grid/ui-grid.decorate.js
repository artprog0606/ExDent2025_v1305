; (function (ng) {
    'use strict';


    ng.module('ui.grid')
        .config(['$provide', function ($provide) {
            $provide.decorator('Grid', ['$delegate', '$q', '$compile', '$parse', 'gridUtil', 'uiGridConstants', 'GridOptions', 'GridColumn', 'GridRow', 'GridApi', 'rowSorter', 'rowSearcher', 'GridRenderContainer', '$timeout', 'ScrollEvent', function ($delegate, $q, $compile, $parse, gridUtil, uiGridConstants, GridOptions, GridColumn, GridRow, GridApi, rowSorter, rowSearcher, GridRenderContainer, $timeout, ScrollEvent) {

                $delegate.prototype.addRowHeaderColumn = function addRowHeaderColumn(colDef, order, stopColumnBuild) {
                    var self = this;

                    // default order
                    if (order === undefined) {
                        order = 0;
                    }

                    var rowHeaderCol = new GridColumn(colDef, gridUtil.nextUid(), self);
                    rowHeaderCol.isRowHeader = true;

                    if (colDef.renderContainer != null) {
                        rowHeaderCol.renderContainer = colDef.renderContainer;
                    } else if (self.isRTL()) {
                        self.createRightContainer();
                        rowHeaderCol.renderContainer = 'right';
                    }
                    else {
                        self.createLeftContainer();
                        rowHeaderCol.renderContainer = 'left';
                    }

                    // relies on the default column builder being first in array, as it is instantiated
                    // as part of grid creation
                    self.columnBuilders[0](colDef, rowHeaderCol, self.options)
                        .then(function () {
                            rowHeaderCol.enableFiltering = false;
                            rowHeaderCol.enableSorting = false;
                            rowHeaderCol.enableHiding = false;
                            rowHeaderCol.headerPriority = order;
                            self.rowHeaderColumns.push(rowHeaderCol);
                            self.rowHeaderColumns = self.rowHeaderColumns.sort(function (a, b) {
                                return a.headerPriority - b.headerPriority;
                            });

                            if (!stopColumnBuild) {
                                self.buildColumns()
                                    .then(function () {
                                        self.preCompileCellTemplates();
                                        self.queueGridRefresh();
                                    }).catch(angular.noop);
                            }
                        }).catch(angular.noop);
                };

                return $delegate;
            }]);
        }]);

    ng.module('ui.grid.selection')
        .config(['$provide', function ($provide) {
            $provide.decorator('uiGridSelectionDirective', ['$delegate', 'uiGridSelectionConstants', 'uiGridSelectionService', 'uiGridConstants', function ($delegate, uiGridSelectionConstants, uiGridSelectionService, uiGridConstants) {
                $delegate[0].compile = function () {
                    return {
                        pre: function ($scope, $elm, $attrs, uiGridCtrl) {
                            uiGridSelectionService.initializeGrid(uiGridCtrl.grid);
                            if (uiGridCtrl.grid.options.enableRowHeaderSelection) {
                                var selectionRowHeaderDef = {
                                    name: uiGridSelectionConstants.selectionRowHeaderColName,
                                    displayName: '',
                                    width: uiGridCtrl.grid.options.selectionRowHeaderWidth,
                                    minWidth: 10,
                                    cellTemplate: 'ui-grid/selectionRowHeader',
                                    headerCellTemplate: 'ui-grid/selectionHeaderCell',
                                    enableColumnResizing: false,
                                    enableColumnMenu: false,
                                    exporterSuppressExport: true,
                                    allowCellFocus: true,
                                    //куда рендерить колонку
                                    renderContainer: 'body',
                                    //классы
                                    cellClass: 'ui-grid-custom-selection__cell',
                                    headerCellClass: 'ui-grid-custom-selection__headercell'
                                };

                                uiGridCtrl.grid.addRowHeaderColumn(selectionRowHeaderDef, 0);
                            }

                            var processorSet = false;

                            var processSelectableRows = function (rows) {
                                rows.forEach(function (row) {
                                    row.enableSelection = uiGridCtrl.grid.options.isRowSelectable(row);
                                });
                                return rows;
                            };

                            var updateOptions = function () {
                                if (uiGridCtrl.grid.options.isRowSelectable !== angular.noop && processorSet !== true) {
                                    uiGridCtrl.grid.registerRowsProcessor(processSelectableRows, 500);
                                    processorSet = true;
                                }
                            };

                            updateOptions();

                            var dataChangeDereg = uiGridCtrl.grid.registerDataChangeCallback(updateOptions, [uiGridConstants.dataChange.OPTIONS]);

                            $scope.$on('$destroy', dataChangeDereg);
                        },
                        post: function ($scope, $elm, $attrs, uiGridCtrl) {

                        }
                    };
                };

                return $delegate;
            }]);

            $provide.decorator('uiGridRowDirective', ['$delegate', function ($delegate) {
                var compileOriginal = $delegate[0].compile;

                $delegate[0].compile = function () {
                    var compileResultOriginal = compileOriginal();
                    var postOriginal = compileResultOriginal.post;

                    return Object.assign({}, compileResultOriginal, {
                        post: function (scope, elm) {

                            postOriginal(scope, elm);

                            var mouseenter = function () {
                                scope.$broadcast('uiGridCustomRowMouseEnter', scope.row);
                            };

                            var mouseleave = function () {
                                scope.$broadcast('uiGridCustomRowMouseLeave', scope.row);
                            };

                            elm.on('mouseenter', mouseenter);

                            elm.on('mouseleave', mouseleave);

                            elm.on('$destroy', function () {
                                elm.off();
                            });
                        }
                    })
                };

                return $delegate;
            }]);

        }]);

    ng.module('ui.grid.treeBase')
        .config(['$provide', function ($provide) {
            $provide.decorator('uiGridTreeBaseService', ['$delegate', 'uiGridTreeBaseConstants', function ($delegate, uiGridTreeBaseConstants) {

                $delegate.createRowHeader = function (grid) {
                    var rowHeaderColumnDef = {
                        name: uiGridTreeBaseConstants.rowHeaderColName,
                        displayName: '',
                        width: 100 || grid.options.treeRowHeaderBaseWidth,
                        minWidth: 10,
                        cellTemplate: 'ui-grid/treeBaseRowHeader',
                        headerCellTemplate: 'ui-grid/treeBaseHeaderCell',
                        enableColumnResizing: false,
                        enableColumnMenu: false,
                        exporterSuppressExport: true,
                        allowCellFocus: true,
                        //куда рендерить колонку
                        renderContainer: 'body',
                        //классы
                        cellClass: 'ui-grid-custom-treeview__cell',
                        headerCellClass: 'ui-grid-custom-treeview__headercell'
                    };

                    rowHeaderColumnDef.visible = grid.options.treeRowHeaderAlwaysVisible;
                    grid.addRowHeaderColumn(rowHeaderColumnDef, -100);
                };

                return $delegate;
            }]);
        }]);
})(window.angular);