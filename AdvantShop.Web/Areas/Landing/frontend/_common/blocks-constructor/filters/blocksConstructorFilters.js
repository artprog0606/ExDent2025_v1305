(function (ng) {
    'use strict';

    ng.module('blocksConstructor').filter('blocksConstructorPictureAsObj', function () {
        return function (value, pictureType) {
            pictureType = pictureType || 'picture';
            if (value != null) {
                if (ng.isArray(value) === true) {
                    for (var i = 0, len = value.length; i < len; i++) {
                        value[i] = convert(value[i], pictureType);
                    }
                } else {
                    value = convert(value, pictureType);
                }
            }

            return value;
        };
    });

    function convert(item, pictureType) {
        if (ng.isString(item) === true) {
            item = {};
            item[pictureType] = {
                src: item,
            };
        } else if (ng.isObject(item) === true) {
            if (item.src != null) {
                item[pictureType] = {
                    src: item.src,
                };
            }
        }
        return item;
    }
})(window.angular);
