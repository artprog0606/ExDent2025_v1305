import customOptionsTemplate from '../templates/customOptions.html';

/* @ngInject */
function customOptionsDirective(urlHelper) {
    return {
        scope: {
            productId: '<',
            initFn: '&',
            changeFn: '&',
            beforeChangeFn: '&',
            customOptionPicture: '<?',
            customOptionComboView: '<?',
        },
        replace: true,
        templateUrl: customOptionsTemplate,
        controller: 'CustomOptionsCtrl',
        controllerAs: 'customOptions',
        bindToController: true,
    };
}

export { customOptionsDirective };
