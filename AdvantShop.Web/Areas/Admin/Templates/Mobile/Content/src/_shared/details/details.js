(function () {
    'use strict';

    angular
        .module('details', [])
        .directive('detailsBlock', [
            '$parse',
            function () {
                return {
                    scope: true,
                    bindToController: true,
                    controllerAs: 'detailsCtrl',
                    controller: [
                        '$attrs',
                        '$scope',
                        '$parse',
                        '$element',
                        function ($attrs, $scope, $parse, $element) {
                            var ctrl = this;

                            ctrl.$onInit = function () {
                                ctrl.changeState($attrs.startValue != null ? $parse($attrs.startValue)($scope) : false);

                                if (ctrl.isOpen) {
                                    $element.attr('open', 'open');
                                } else {
                                    $element.removeAttr('open');
                                }
                            };

                            ctrl.changeState = function (value) {
                                ctrl.isOpen = value != null ? value : !ctrl.isOpen;
                            };
                        },
                    ],
                };
            },
        ])
        .directive('detailsBlockSummary', function () {
            return {
                require: '^detailsBlock',
                link: function (scope, element, attrs, detailsBlock) {
                    element.on('click', function () {
                        detailsBlock.changeState();
                        scope.$apply();
                    });
                },
            };
        });
})();
