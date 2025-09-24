(function (ng) {
    'use strict';

    var BlocksConstructorMainCtrl = function ($element, $q, $interpolate, $rootScope, blocksConstructorService) {
        var ctrl = this,
            defer;

        ctrl.$onInit = function () {
            ctrl.listContainers = {};
            blocksConstructorService.saveMain(ctrl);
        };

        ctrl.addContainer = function (id, container) {
            if (id != null && id.length > 0) {
                ctrl.listContainers[id] = container;
            }
        };

        ctrl.removeContainer = function (id) {
            delete ctrl.listContainers[id];
        };

        ctrl.activateSelectMode = function () {
            ctrl.enabledSelectMode = true;
            defer = $q.defer();

            return defer.promise;
        };

        ctrl.deactivateSelectMode = function () {
            ctrl.enabledSelectMode = false;
        };

        ctrl.selectBlock = function (block) {
            ctrl.deactivateSelectMode();
            defer.resolve(block);
            return block;
        };

        ctrl.saveSettings = function (value, inplaceScope, blockId, data, property) {
            //задаем значение свойству сеттинга
            new Function('settings', 'settings.' + searchInterpolateHierarchy(property, inplaceScope) + '=' + JSON.stringify(value))(data.Settings);
            blocksConstructorService.saveBlockSettings(blockId, data);
        };

        ctrl.saveFormSettings = function (value, blockId, data, property) {
            //задаем значение свойству сеттинга
            new Function('Form', 'Form.' + property + '=' + JSON.stringify(value))(data.Form);
            blocksConstructorService.saveBlockSettings(blockId, data);
        };

        ctrl.hideModals = function () {
            $element.addClass('adv-modals--hide');
        };

        ctrl.showModals = function () {
            $element.removeClass('adv-modals--hide');
        };

        function searchInterpolateHierarchy(property, inplaceScope) {
            var result = $interpolate(property, false, null, true)(inplaceScope);

            while (result == null && inplaceScope !== $rootScope) {
                result = searchInterpolateHierarchy(property, inplaceScope.$parent);
            }

            return result;
        }
    };

    ng.module('blocksConstructor').controller('BlocksConstructorMainCtrl', BlocksConstructorMainCtrl);

    BlocksConstructorMainCtrl.$inject = ['$element', '$q', '$interpolate', '$rootScope', 'blocksConstructorService'];
})(window.angular);
