(function (ng) {
    'use strict';

    var lpGridService = function ($http, $q) {
        var service = this;

        service.resolveObjectFromPath = function (object, path) {
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

        service.getObjectFromProperties = function (obj, path, val) {
            var stringToPath = function (path) {
                if (typeof path !== 'string') return path;
                var output = [];
                path.split('.').forEach(function (item, index) {
                    item.split(/\[([^}]+)\]/g).forEach(function (key) {
                        if (key.length > 0) {
                            output.push(key);
                        }
                    });
                });

                return output;
            };

            path = stringToPath(path);

            var length = path.length;
            var current = obj;

            path.forEach(function (key, index) {
                if (index === length - 1) {
                    current[key] = val;
                } else {
                    if (!current[key]) {
                        current[key] = {};
                    }
                    current = current[key];
                }
            });
        };
    };

    ng.module('lpGrid').service('lpGridService', lpGridService);

    lpGridService.$inject = ['$http', '$q'];
})(window.angular);
