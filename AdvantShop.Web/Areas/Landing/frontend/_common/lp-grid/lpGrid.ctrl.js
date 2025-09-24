(function (ng) {
    'use strict';

    var globalIncrement = 0;

    var LpGridCtrl = function ($element, $q, lpGridService, toaster, $scope, modalService, $window) {
        var ctrl = this;
        var lpGridChild;

        ctrl.$onInit = function () {
            ctrl.isMobile = $window.matchMedia('(max-width: 768px)').matches;

            globalIncrement += 1;

            ctrl.increment = globalIncrement;

            ctrl.source = ctrl.source || [];

            ctrl.dataItems = [];

            ctrl.columns = [];

            ctrl.editMode = ctrl.editMode || 'runtime';

            ctrl.activeRow = null;

            ctrl.rowSelection = null;

            ctrl.NO_PHOTO_PATH = 'areas/landing/frontend/images/nophoto.jpg';

            ctrl.openedGroups = [];

            ctrl.newElementStorage = new Array(ctrl.source.length);

            ctrl.addButtonText = ctrl.addButtonText || 'Добавить новый элемент';

            ctrl.firstDataNotNull = false;

            ctrl.pictureFields = [];

            $scope.$watchCollection('$ctrl.source', function (newValue) {
                ctrl.mergeDataSources(ctrl.dataItems, newValue);

                if (
                    ctrl.selectionStartIndex != null &&
                    newValue != null &&
                    newValue != null &&
                    newValue.length > 0 &&
                    ctrl.firstDataNotNull === false
                ) {
                    ctrl.rowSelection = ctrl.dataItems[ctrl.selectionStartIndex];
                    ctrl.firstDataNotNull = true;
                }

                if (ctrl.ngRequired === true && ctrl.parentForm != null) {
                    ctrl.parentForm.$setDirty();
                    ctrl.parentForm.$setValidity('lpGridRequired', ctrl.dataItems != null && ctrl.dataItems.length > 0, null);
                }
            });
            //начинаем очистку только когда удаляется рутовый грид
            //иначе данные дочерних гридов теряются при скрытии потомков и добавлении нового рутового грида
            if (ctrl.lpGridParent == null) {
                $element.on('$destroy', function () {
                    ctrl.clear();
                    ctrl.columns.length = 0;
                });
            } else {
                ctrl.lpGridParent.addChild(ctrl);
            }
        };

        ctrl.$postLink = function () {
            ctrl.sortableOptions = {
                containment: '#lpGridSortable_' + ctrl.increment,
                scrollableContainer: '#lpGridSortable_' + ctrl.increment,
                containerPositioning: 'relative',
                orderChanged: function (event) {
                    var dest = ctrl.newElementStorage[event.dest.index];
                    var source = ctrl.newElementStorage[event.source.index];

                    ctrl.newElementStorage[event.source.index] = dest;

                    ctrl.newElementStorage[event.dest.index] = source;

                    var movedElement = ctrl.source.splice(event.source.index, 1)[0];

                    ctrl.source.splice(event.dest.index, 0, movedElement);

                    if (ctrl.onOrderChanged) {
                        ctrl.onOrderChanged({ event: event });
                    }
                },
                accept: function (sourceItemHandleScope, destSortableScope, destItemScope) {
                    return sourceItemHandleScope.sortableScope.$id === destSortableScope.$id;
                },
            };
        };

        ctrl.mergeDataSources = function (lpDataItems, source) {
            if (source == null || source.length === 0) {
                lpDataItems.length = 0;
            } else {
                lpDataItems.length = 0;

                for (var i = 0, len = source.length; i < len; i++) {
                    lpDataItems.push({
                        entity: source[i],
                    });
                }
            }
        };

        ctrl.addGroup = function (group) {
            ctrl.group = group;
        };

        ctrl.addColumn = function (column) {
            if (column.type === 'picture') {
                ctrl.pictureFields.push(column);
            }
            ctrl.columns.push(column);
        };

        ctrl.change = function (row, col, index, saveFromChildModal = false) {
            if (col.onChange != null) {
                col.onChange({ row: row.entity, col: col, index: index });
            }

            if (modalService.hasModal('modalEditLpGridItem') !== false && saveFromChildModal) {
                modalService.getModal('modalEditLpGridItem').then(function (modal) {
                    modal.modalScope.close();
                });
            }
        };

        ctrl.deleteAll = function () {
            var rowsWillDelete = ng.copy(ctrl.dataItems);

            ctrl.clear();

            for (var i = 0, len = rowsWillDelete.length; i < len; i++) {
                if (ctrl.onDelete != null) {
                    ctrl.onDelete({ row: rowsWillDelete[i].entity, index: i });
                }
            }
        };

        ctrl.delete = function (row, index) {
            ctrl.source.splice(index, 1);
            ctrl.dataItems.splice(index, 1);

            ctrl.clearStorageState(index);

            if (ctrl.onDelete != null) {
                ctrl.onDelete({ row: row.entity, index: index });
            }
        };

        ctrl.clear = function () {
            ctrl.source.length = 0;
            ctrl.dataItems.length = 0;
            ctrl.clearStorageState();
        };

        ctrl.clearStorageState = function (index) {
            if (index != null) {
                ctrl.openedGroups.splice(index, 1);
                ctrl.newElementStorage.splice(index, 1);
            } else {
                ctrl.openedGroups.length = 0;
                ctrl.newElementStorage.length = 0;
            }
        };

        ctrl.addNewElement = function () {
            var newElement = {};

            if (ctrl.pictureFields.length > 0) {
                ctrl.pictureFields.forEach(function (item) {
                    lpGridService.getObjectFromProperties(newElement, item.field, { src: null });
                });
            }

            if (ctrl.onBeforeAddNewElement != null) {
                newElement = ctrl.onBeforeAddNewElement({ entity: newElement }) || newElement;
            }

            ctrl.newElementStorage.push(true);

            ctrl.source.push(newElement);

            ctrl.dataItems.push({
                _isNew: true,
                entity: newElement,
            });
        };

        ctrl.uploadPicture = function (result, row, column, index) {
            var entity = lpGridService.resolveObjectFromPath(row.entity, column.field);

            if (column.previewField) {
                row.entity[column.field][column.previewField] = result.processedPictures.preview;
            }
            //row.entity[column.field] = row.entity[column.field] || {};
            //row.entity[column.field].type = result.type || 'image';
            //row.entity[column.field].src = result.picture;
            entity = entity || {};
            entity.type = result.type || 'image';
            entity.src = result.picture;
        };

        ctrl.resizePicture = function (result, row, column, index) {
            var entity = lpGridService.resolveObjectFromPath(row.entity, column.field);
            entity = entity || {};

            if (result.width != null && result.width > 0) {
                entity.width = result.width;
            } else {
                delete entity.width;
            }

            if (result.height != null && result.height > 0) {
                entity.height = result.height;
            } else {
                delete entity.height;
            }

            entity.src = result.picture;
        };

        ctrl.deletePicture = function (result, row, column, index, getsetModel) {
            if (column.previewField) {
                row.entity[column.previewField] = null;
            }
            getsetModel.src = null;

            //row.entity[column.field] = row.entity[column.field] || {};
            //row.entity[column.field].src = null;
        };

        ctrl.getPictureLoaderParameters = function (column) {
            return column.previewWidth || column.previewHeight
                ? [
                      {
                          maxWidth: column.previewWidth,
                          maxHeight: column.previewHeight,
                          postfix: 'preview',
                      },
                  ]
                : null;
        };

        ctrl.activateEditMode = function (row, rowIndex) {
            if (ctrl.activeRow != null) {
                ctrl.cancelEdit(ctrl.activeRow, rowIndex);
            }

            ctrl.activeRow = row;
            ctrl.activeRowBackup = ng.copy(row);
        };

        ctrl.applyEdit = function (row) {
            ctrl.activeRow = null;
            ctrl.activeRowBackup = null;
        };

        ctrl.cancelEdit = function (row, rowIndex) {
            var empty = {};

            if (
                ctrl.dataItems[rowIndex]._isNew === true &&
                (ng.equals(row.entity, ctrl.activeRowBackup.entity) === true || ng.equals(empty, ctrl.activeRowBackup.entity) === true)
            ) {
                ctrl.dataItems[rowIndex].entity = empty;
                ctrl.source[rowIndex].entity = empty;
            } else {
                // row.entity = ng.extend(row.entity, ctrl.activeRowBackup.entity);
                ctrl.dataItems[rowIndex].entity = ctrl.activeRowBackup.entity;
                ctrl.source[rowIndex] = ctrl.activeRowBackup.entity;
            }

            ctrl.activeRow = null;
            ctrl.activeRowBackup = null;
        };

        ctrl.clickCol = function (row, col) {
            if (col.onClick != null) {
                col.onClick({
                    lpGridRow: row,
                    lpGridColumn: col,
                });
            }
        };

        ctrl.ckeditorModalOnApply = function (value, row, col) {
            row.entity[col.field] = value;
        };

        ctrl.onLazyLoadChange = function (result, row, column, index) {
            row.entity[column.field] = row.entity[column.field] || {};
            row.entity[column.field].lazyLoadEnabled = result;
        };

        ctrl.addChild = function (lpGrid) {
            lpGridChild = lpGrid;
        };

        ctrl.removeChilds = function () {
            if (lpGridChild != null) {
                lpGridChild.clear();
                lpGridChild.columns.length = 0;
            }
        };

        ctrl.editDataItems = function (row, rowIndex, index, col) {
            let MODAL_ID = 'modalEditLpGridItem';
            if (modalService.hasModal(MODAL_ID) === false) {
                modalService.renderModal(
                    MODAL_ID,
                    '<div>Редактирование элемента</div>',
                    '<lp-grid-columns parent-ctrl="parentCtrl" row="row" row-index="rowIndex"></lp-grid-columns>',
                    '<div class="row middle-xs">' +
                        '<input type="button" class="blocks-constructor-btn-confirm flex-grow" data-ng-click="modalData.change(row, col, index,true)" value="Сохранить" />' +
                        '<input type="button" class="blocks-constructor-btn-cancel blocks-constructor-btn-mar flex-grow" data-modal-close="" value="Отмена" />' +
                        '</div>',
                    {
                        destroyOnClose: true,
                        modalClass: 'lp-modal-edit-grid-item',
                        backgroundEnable: true,
                    },
                    {
                        parentCtrl: ctrl,
                        row,
                        rowIndex,
                        modalData: ctrl,
                        index,
                        col,
                    },
                );
            }

            modalService.getModal(MODAL_ID).then(function (modal) {
                modal.modalScope.open(modal);
            });
        };
    };

    ng.module('lpGrid').controller('LpGridCtrl', LpGridCtrl);

    LpGridCtrl.$inject = ['$element', '$q', 'lpGridService', 'toaster', '$scope', 'modalService', '$window'];
})(window.angular);
