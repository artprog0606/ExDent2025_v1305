import kanbanTemplate from './template/kanban.html';

(function (ng) {
    'use strict';

    ng.module('kanban')
        .component('kanban', {
            templateUrl: [
                '$element',
                '$attrs',
                function (element, attrs) {
                    return attrs.templatePath || kanbanTemplate;
                },
            ],
            controller: 'KanbanCtrl',
            bindings: {
                kanbanObj: '<',
                sortOptions: '<',
                update: '&',
                fetchUrl: '<?',
                fetchColumnUrl: '<?',
                kanbanOnInit: '&',
                extendCtrl: '<?',
                filterParams: '<?',
                kanbanColumnDefs: '<?',
                kanbanOnFilterInit: '&',
                kanbanParams: '<?',
                modalAddParams: '<?',
                uid: '@',
                cardTemplateUrl: '@',
                noCardsTemplateUrl: '@',
                kanbanScrollable: '<?',
                kanbanColumnClasses: '<?',
                kanbanRowClasses: '<?',
                kanbanColumnWrapClasses: '<?',
                kanbanStickyTop: '<?',
            },
        })
        .component('kanbanCard', {
            require: {
                parent: '^kanban',
            },
            template: '<div data-ng-include="$ctrl.parent.cardTemplateUrl"></div>',
            controller: 'KanbanCardCtrl',
            bindings: {
                card: '<',
                onUpdate: '&',
            },
        })
        .component('kanbanCardEmpty', {
            require: {
                parent: '^kanban',
            },
            template: '<div data-ng-include="$ctrl.parent.noCardsTemplateUrl"></div>',
            bindings: {
                salesFunnelId: '<',
                dealStatusId: '<',
            },
        });
})(window.angular);
