(function (ng) {
    'use strict';

    var KanbanCardCtrl = function () {
        var ctrl = this;

        ctrl.$onInit = function () {};
        ctrl.getFullDate = () => {
            return new Date(ctrl.card.DateAppointed).toLocaleString('ru-RU');
        };
    };

    KanbanCardCtrl.$inject = [];

    ng.module('kanban').controller('KanbanCardCtrl', KanbanCardCtrl);
})(window.angular);
