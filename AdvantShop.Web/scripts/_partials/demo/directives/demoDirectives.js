import demoModalTemplate from '../templates/demoModal.html';
function demoModalDirective() {
    return {
        restrict: 'A',
        scope: {
            demoModalUrl: '@',
            demoModalId: '@',
        },
        controller: 'DemoCtrl',
        controllerAs: 'demoModal',
        bindToController: true,
        templateUrl: demoModalTemplate,
    };
}

export { demoModalDirective };
