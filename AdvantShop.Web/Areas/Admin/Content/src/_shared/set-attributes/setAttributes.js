(function (ng) {
    'use strict';

    ng.module('setAttributes', []).directive('setAttributes', [
        function () {
            return {
                controller: [
                    '$parse',
                    '$scope',
                    '$attrs',
                    '$element',
                    '$compile',
                    function ($parse, $scope, $attrs, $element, $compile) {
                        this.$onInit = function () {
                            var attributes = $parse($attrs.setAttributes)($scope);

                            if (attributes != null) {
                                attributes.forEach((it) => {
                                    const entriesArray = Object.entries(it);
                                    if (entriesArray.length) {
                                        const [key, value] = entriesArray[0];
                                        $element[0].setAttribute(key, value);
                                        $element[0].removeAttribute('set-attributes');
                                        $element[0].removeAttribute('data-set-attributes');
                                        this.compile($element);
                                    }
                                });
                            }
                        };

                        this.compile = function (element) {
                            // $scope = element.scope();
                            // $injector = element.injector();
                            // $injector.invoke(function ($compile) {
                            $compile(element)($scope);
                            // });
                        };
                    },
                ],
            };
        },
    ]);
})(window.angular);
