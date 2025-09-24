import dynamicFormTemplate from './dynamicForm.html';
(function (ng) {
    'use strict';

    //example fields
    // [
    //     { label: 'Title', value: 'mr.', disabled: false, controlType: 'dropdown', model: { key: 'master', value: 'Master'}, options: [
    //             { key: 'mr.',  value: 'Mr.'},
    //             { key: 'miss.',  value: 'Miss.'},
    //         ],
    //     },
    //     { label: 'First Name', attributes: [{'validation-input-float': ''}, {'validation-input-min': 0}, {'validation-input-text': 'Вес'], value:'text', disabled: false, controlType: 'input', type: 'text', model: 'model' },
    //     { label: 'radio', value:true, disabled: false, controlType: 'radio', name: 'radio', checked: false},
    //     { key: 'checkbox', label: 'checkbox', value:true, disabled: false, controlType: 'checkbox', name: 'checkbox', checked: false, model: true},
    //
    // ]
    let increment = 1;
    ng.module('dynamicForm', []).directive('dynamicForm', [
        function () {
            return {
                templateUrl: dynamicFormTemplate,
                scope: {
                    fields: '<',
                    formName: '<?',
                },
                bindToController: true,
                controllerAs: '$ctrl',
                controller: [
                    '$parse',
                    '$scope',
                    function ($parse, $scope) {
                        this.$onInit = function () {
                            if (this.formName == null) {
                                this.formName = 'dynamicForm' + increment;
                                increment++;
                            }
                        };
                        this.getScope = function () {
                            return this;
                        };
                        this.getForm = function (name) {
                            return $parse(name)($scope);
                        };
                    },
                ],
            };
        },
    ]);
})(window.angular);
