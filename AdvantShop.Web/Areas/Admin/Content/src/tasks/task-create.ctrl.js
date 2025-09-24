import addTaskTemplate from './../_shared/modal/addTask/addTask.html';
(function (ng) {
    'use strict';

    var TasksCreateCtrl = function ($element, $scope, $uibModal) {
        var ctrl = this;
        ctrl.$postLink = function () {
            $element.on('click', function () {
                return $uibModal
                    .open({
                        bindToController: true,
                        controller: 'ModalAddTaskCtrl',
                        controllerAs: 'ctrl',
                        templateUrl: addTaskTemplate,
                        resolve: ctrl.resolve,
                        windowClass: 'modal__window--scrollbar-no',
                        size: 'lg',
                        backdrop: 'static',
                    })
                    .result.then(
                        function (result) {
                            if (ctrl.onAfter != null) {
                                ctrl.onAfter({
                                    result: result,
                                });
                            }
                            return result;
                        },
                        function (result) {
                            if (ctrl.onAfter != null) {
                                ctrl.onAfter({
                                    result: result,
                                });
                            }
                            return result;
                        },
                    );
            });
        };
    };
    TasksCreateCtrl.$inject = ['$element', '$scope', '$uibModal'];
    ng.module('tasks').controller('TasksCreateCtrl', TasksCreateCtrl);
})(window.angular);
