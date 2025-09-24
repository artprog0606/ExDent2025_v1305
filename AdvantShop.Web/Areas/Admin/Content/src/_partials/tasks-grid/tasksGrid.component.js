import tasksGridTemplate from './templates/tasks-grid.html';
(function (ng) {
    'use strict';

    ng.module('tasksGrid').component('tasksGrid', {
        templateUrl: tasksGridTemplate,
        controller: 'TasksGridCtrl',
        bindings: {
            objId: '<',
            type: '@',
            onInit: '&',
            isAdmin: '<',
            onUpdate: '&',
            readonly: '=',
        },
    });
})(window.angular);
