(function (ng) {
    'use strict';

    ng.module('tasks').component('taskCreate', {
        controller: 'TasksCreateCtrl',
        bindings: {
            resolve: '<?',
            onAfter: '&',
        },
    });
})(window.angular);
