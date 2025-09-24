import ouibounce from '../../../../../node_modules/ouibounce/build/ouibounce.js';

(function (ng) {
    'use strict';

    ng.module('modalOuibounce', [])
        .controller('ModalOuibounceCtrl', function () {
            var ctrl = this;

            ctrl.modalOuibounceClose = function () {
                ctrl.modalControl.close();
            };
        })
        .directive('modalOuibounce', [
            '$parse',
            function ($parse) {
                return {
                    scope: true,
                    require: {
                        modalControl: 'modalControl',
                    },
                    controller: 'ModalOuibounceCtrl',
                    controllerAs: 'modalOuibounce',
                    bindToController: true,
                    link: function (scope, element, attrs, ctrl) {
                        var disabled = attrs.modalOuibounceDisabled != null && $parse(attrs.modalOuibounceDisabled)(scope) === true;
                        var optionsCustom = $parse(attrs.modalOuibounceOptions)(scope) || {};
                        var options = ng.extend(
                            {},
                            {
                                aggressive: true,
                                callback: function () {
                                    ctrl.modalControl.open();
                                    scope.$apply();
                                },
                            },
                            optionsCustom,
                        );

                        if (disabled === false) {
                            ouibounce(element[0], options);
                        }
                    },
                };
            },
        ]);
})(window.angular);
