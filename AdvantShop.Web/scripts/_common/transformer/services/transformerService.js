(function (ng) {
    'use strict';

    var transformerService = function () {
        var service = this;
        var scrollWidth;
        var storage = [];

        service.getTransformersStorage = function () {
            return storage;
        };

        service.addInStorage = function (transformer) {
            var sortOrder = transformer.sortOrder || 0;
            var isPasted = false;

            if (storage.length > 0) {
                for (var i = 0, len = storage.length; i < len; i++) {
                    if (sortOrder > storage[i].sortOrder) {
                        storage.splice(i, 0, transformer);
                        isPasted = true;
                        break;
                    }
                }
            }

            if (isPasted === false) {
                storage.push(transformer);
            }
        };

        service.getOffsetByParents = function (self) {
            var result = 0;
            for (var i = 0, len = storage.length; i < len; i++) {
                if (storage[i] === self) {
                    break;
                } else if (storage[i].scrollOver === true || storage[i].freeze === true) {
                    result += storage[i].getBottomPoint();
                }
            }

            return result;
        };

        service.getSummHeightTransformers = function (self) {
            var result = 0;
            for (var i = 0, len = storage.length; i < len; i++) {
                if (storage[i] === self) {
                    break;
                } else if (storage[i].scrollOver === true || storage[i].freeze === true) {
                    result += storage[i].getHeightElement();
                }
            }

            return result;
        };

        service.getWidthScroll = function () {
            var div = document.createElement('div');
            div.style.overflowY = 'scroll';
            div.style.width = '50px';
            div.style.height = '50px';
            document.body.append(div);
            scrollWidth = div.offsetWidth - div.clientWidth;
            div.remove();
            return scrollWidth;
        };

        service.deleteFromStorageDestroyedCtrl = function (ctrl) {
            storage = storage.filter(function (it) {
                return it !== ctrl;
            });
        };
    };

    angular.module('transformer').service('transformerService', transformerService);

    transformerService.$inject = [];
})(window.angular);
