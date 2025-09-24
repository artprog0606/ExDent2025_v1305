import videoTemplate from './../templates/subblocks/videoSubblocks.html';
import buyformTemplate from './../templates/subblocks/buyForm.html';
import priceTemplate from './../templates/subblocks/price.html';
import buttonTemplate from './../templates/subblocks/button.html';
(function (ng) {
    'use strict';

    ng.module('subblockInplace').directive('subblockInplace', function () {
        return {
            restrict: 'AE',
            scope: true,
            controller: 'SubblockInplaceCtrl',
            controllerAs: 'subblockInplace',
            bindToController: true,
            //link: function (scope, element, attrs, ctrl) {
            //    ctrl.sublockId = attrs.sublockId;
            //    ctrl.name = attrs.name;
            //    ctrl.type = attrs.type;
            //    ctrl.sortOrder = attrs.sortOrder;
            //    ctrl.settings = (new Function('return ' + attrs.settings))();
            //}
            compile: function compile(tElement, tAttrs, transclude) {
                return {
                    pre: function preLink(scope, element, attrs, ctrl) {
                        ctrl.sublockId = attrs.sublockId;
                        ctrl.name = attrs.name;
                        ctrl.type = attrs.type;
                        ctrl.sortOrder = attrs.sortOrder;
                        if (attrs.settings != null && attrs.settings.length > 0) {
                            ctrl.settings = new Function('return ' + attrs.settings)();
                        }
                    },
                };
            },
        };
    });
    ng.module('subblockInplace').directive('subblockInplaceButton', function () {
        return {
            restrict: 'AE',
            scope: true,
            controller: 'SubblockInplaceButtonCtrl',
            controllerAs: 'subblockInplaceButton',
            bindToController: true,
        };
    });
    ng.module('subblockInplace').component('subblockButtonModal', {
        templateUrl: buttonTemplate,
        controller: 'SubblockInplaceButtonModalCtrl',
        bindings: {
            onUpdate: '&',
            settings: '<',
        },
    });
    ng.module('subblockInplace').directive('subblockInplacePrice', function () {
        return {
            restrict: 'AE',
            scope: true,
            controller: 'SubblockInplacePriceCtrl',
            controllerAs: 'subblockInplacePrice',
            bindToController: true,
        };
    });
    ng.module('subblockInplace').component('subblockPriceModal', {
        templateUrl: priceTemplate,
        controller: 'SubblockInplacePriceModalCtrl',
        bindings: {
            onUpdate: '&',
            settings: '<',
        },
    });
    ng.module('subblockInplace').directive('subblockInplaceBuyForm', function () {
        return {
            restrict: 'AE',
            scope: true,
            controller: 'SubblockInplaceBuyFormCtrl',
            controllerAs: 'subblockInplaceBuyForm',
            bindToController: true,
        };
    });
    ng.module('subblockInplace').component('subblockBuyFormModal', {
        templateUrl: buyformTemplate,
        controller: 'SubblockInplaceBuyFormModalCtrl',
        bindings: {
            onUpdate: '&',
            settings: '<',
            formId: '<',
        },
    });
    ng.module('subblockInplace').directive('subblockInplaceVideo', function () {
        return {
            restrict: 'AE',
            scope: true,
            controller: 'SubblockInplaceVideoCtrl',
            controllerAs: 'subblockInplaceVideo',
            bindToController: true,
        };
    });
    ng.module('subblockInplace').component('subblockInplaceVideoModal', {
        templateUrl: videoTemplate,
        controller: 'SubblockInplaceVideoModalCtrl',
        bindings: {
            onUpdate: '&',
            settings: '<',
        },
    });
})(window.angular);
