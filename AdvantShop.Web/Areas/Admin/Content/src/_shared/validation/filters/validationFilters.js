(function (ng) {
    'use strict';

    ng.module('validation').filter('validationUnique', function () {
        return function (errors) {
            return unboxing(Object.assign({}, errors));
        };
    });

    function unboxing(collection) {
        var result = {};

        Object.keys(collection).forEach(function (key) {
            const item = collection[key];
            if (Array.isArray(item)) {
                collection[key].forEach(function (item) {
                    if (item.constructor.name === 'FormController') {
                        result = merge(result, unboxing(item.$error));
                    } else {
                        result[key] = result[key] || [];
                        if (item.$error != null && item.validationInputText == null) {
                            const resultChild = unboxing(item.$error);

                            result = Object.assign({}, result, resultChild);
                        } else {
                            result[key].push(Object.assign({}, item));
                        }
                    }
                });
            }
        });

        return result;
    }

    function merge(source, dest) {
        var cloneDest = ng.copy(dest);

        Object.keys(source).forEach(function (key) {
            if (cloneDest[key] != null) {
                source[key] = source[key].concat(cloneDest[key]);
                delete cloneDest[key];
            }
        });

        Object.keys(cloneDest).forEach(function (key) {
            source[key] = cloneDest[key];
        });

        return source;
    }
})(window.angular);
