(function (ng) {
    'use strict';

    ng.module('simpleEdit')
        .component('simpleEdit', {
            require: {},
            controller: 'SimpleEditCtrl',
            bindings: {
                onChange: '&',
                emptyText: '@',
                timeout: '<?',
                defaultValue: '@',
            },
        })
        .directive('simpleEditContent', function () {
            return {
                require: {
                    simpleEdit: '^simpleEdit',
                },
                bindToController: true,
                controller: [
                    '$element',
                    '$scope',
                    function ($element, $scope) {
                        this.$postLink = function () {
                            var ctrl = this;

                            $element.attr('contenteditable', 'true');

                            ctrl.simpleEdit.addContent($element[0]);

                            $element.on('focus', function () {
                                var value = ctrl.simpleEdit.getValue();

                                if (value === ctrl.simpleEdit.emptyText) {
                                    value = ctrl.simpleEdit.defaultValue || '';

                                    ctrl.simpleEdit.setValue(value);

                                    setTimeout(function () {
                                        ctrl.simpleEdit.setCursorPosition(ctrl.simpleEdit.getContent());
                                    });
                                }

                                ctrl.simpleEdit.saveAsOldValue(value);

                                $scope.$apply();
                            });

                            $element.on('paste', function () {
                                ctrl.simpleEdit.change(ctrl.simpleEdit.getValue());
                                $scope.$apply();
                            });

                            var timer;
                            var time = ctrl.simpleEdit.timeout || 2000;
                            $element.on('input', function () {
                                if (timer != null) {
                                    clearTimeout(timer);
                                }

                                timer = setTimeout(function () {
                                    ctrl.simpleEdit.change(ctrl.simpleEdit.getValue());

                                    setTimeout(function () {
                                        window.getSelection().removeAllRanges();
                                        $element.blur();

                                        $scope.$digest();
                                    }, 0);

                                    $scope.$apply();
                                }, time);
                            });

                            $element.on('blur', function () {
                                var value = ctrl.simpleEdit.getValue();

                                if (value.length === 0) {
                                    ctrl.simpleEdit.setValue(ctrl.simpleEdit.defaultValue || ctrl.simpleEdit.emptyText);
                                    ctrl.simpleEdit.change(ctrl.simpleEdit.defaultValue || ctrl.simpleEdit.emptyText);
                                }
                                $scope.$apply();
                            });

                            $element.on('keydown', function (event) {
                                if (event.keyCode === 13) {
                                    event.preventDefault();
                                } else if (event.keyCode === 27) {
                                    ctrl.simpleEdit.revertValue();
                                }

                                $scope.$apply();
                            });
                        };
                    },
                ],
            };
        })
        .component('simpleEditTrigger', {
            require: {
                simpleEdit: '^simpleEdit',
            },
            controller: [
                '$element',
                function ($element) {
                    var ctrl = this;

                    this.$postLink = function () {
                        ctrl.simpleEdit.addTrigger($element[0]);

                        $element.on('click', function () {
                            ctrl.simpleEdit.setCursorPosition(ctrl.simpleEdit.getContent());
                        });
                    };
                },
            ],
        });
})(window.angular);
