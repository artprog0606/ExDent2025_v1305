import cardsFormTemplate from '../templates/cardsForm.html';
function cardsFormDirective() {
    return {
        restrict: 'A',
        scope: {
            applyFn: '&',
            btnClasses: '<?',
        },
        controller: 'CardsFormCtrl',
        controllerAs: 'cardsForm',
        bindToController: true,
        replace: true,
        templateUrl: cardsFormTemplate,
    };
}

function cardsRemoveDirective() {
    return {
        restrict: 'A',
        scope: {
            applyFn: '&',
            type: '@',
        },
        controller: 'CardsRemoveCtrl',
        controllerAs: 'cardsRemove',
        bindToController: true,
        replace: true,
        template: '<a class="icon-cancel-before link-text-decoration-none" data-ng-click="cardsRemove.remove(cardsRemove.type)"></a>',
    };
}

export { cardsFormDirective, cardsRemoveDirective };
