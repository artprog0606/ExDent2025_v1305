import sizesTemplate from '../templates/sizes.html';
import sizesSelectTemplate from '../templates/sizesSelect.html';
function sizesViewerDirective() {
    return {
        restrict: 'A',
        replace: true,
        templateUrl: (element, attrs) => (attrs?.controlType === 'select' ? sizesSelectTemplate : sizesTemplate),
        controller: 'SizesViewerCtrl',
        controllerAs: 'sizesViewer',
        bindToController: true,
        scope: {
            sizes: '<?',
            sizeSelected: '=?',
            initSizes: '&',
            changeSize: '&',
            startSelectedSizes: '<?',
            controlType: '@',
        },
    };
}

export { sizesViewerDirective };
