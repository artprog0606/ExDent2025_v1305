(function (ng) {
    'use strict';

    ng.module('uiGridCustomEdit', []).filter('isEditable', function () {
        return function (col, row, scope) {
            return (
                !row.isSaving &&
                col.colDef.enableCellEdit &&
                (ng.isFunction(col.colDef.cellEditableCondition) ? col.colDef.cellEditableCondition(scope) : true)
            );
        };
    });
})(window.angular);
