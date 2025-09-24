(function (ng) {
    'use strict';

    // resolves a string path against the given object
    // shamelessly borrowed from
    // http://stackoverflow.com/questions/6491463/accessing-nested-javascript-objects-with-string-key
    var resolveObjectFromPath = function (object, path) {
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
    };

    var LpGridModel = function ($attrs, $parse, $scope) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl._rowP = $parse($attrs.lpGridModelRow);
            ctrl._colP = $parse($attrs.lpGridModelCol);
        };

        ctrl.getSetModel = function (value) {
            var _rowP = $parse(ctrl._rowP)($scope);
            var _colP = $parse(ctrl._colP)($scope);

            // Note that newName can be undefined for two reasons:
            // 1. Because it is called as a getter and thus called with no arguments
            // 2. Because the property should actually be set to undefined. This happens e.g. if the
            //    input is invalid
            return arguments.length ? ctrl.set(_rowP, _colP.field, value) : ctrl.get(_rowP, _colP.field);
        };

        ctrl.get = function (row, field) {
            return resolveObjectFromPath(row, field);
        };

        ctrl.set = function (row, field, value) {
            var lastProp, obj;

            if (field.indexOf('.') !== -1) {
                var parts = field.split('.');
                lastProp = parts.pop();
                obj = resolveObjectFromPath(row, parts.join('.'));
            } else {
                lastProp = field;
                obj = row;
            }
            if (obj != null) {
                obj[lastProp] = value;
            }

            return obj[lastProp];
        };
    };

    ng.module('lpGrid').controller('LpGridModel', LpGridModel);

    LpGridModel.$inject = ['$attrs', '$parse', '$scope'];
})(window.angular);
