import colorsTemplate from '../templates/colors.html';
import colorsSelectTemplate from '../templates/colorsSelect.html';
function colorsViewerDirective() {
    return {
        require: {
            carousel: '?^carousel',
        },
        restrict: 'A',
        replace: true,
        templateUrl: (element, attrs) => (attrs?.controlType === 'select' ? colorsSelectTemplate : colorsTemplate),
        controller: 'ColorsViewerCtrl',
        controllerAs: 'colorsViewer',
        bindToController: true,
        transclude: {
            colorsViewerItemBefore: '?colorsViewerItemBefore',
        },
        scope: {
            colors: '=',
            colorSelected: '=?',
            startSelectedColors: '<?',
            changeStartSelectedColor: '<?',
            colorWidth: '=?',
            colorHeight: '=?',
            initColors: '&',
            changeColor: '&',
            multiselect: '<?',
            imageType: '@',
            viewMode: '@',
            isHiddenColorName: '<?',
            controlType: '@',
            countVisibleItems: '<?',
        },
    };
}

const colorsViewerItemBeforeComponent = () => {
    return {
        controllerAs: 'colorsViewerItemBefore',
        /* @ngInject */
        controller: function ($scope) {
            const ctrl = this;

            ctrl.$onInit = function () {
                $scope.$itemScope = findPropertyInScope($scope, `color`);
            };
        },
    };
};

export { colorsViewerDirective, colorsViewerItemBeforeComponent };

const findPropertyInScope = (scope, propertyName) => {
    let result;
    let item = scope;
    while (item != null) {
        if (item[propertyName] != null) {
            result = item;
            break;
        } else {
            item = item.$parent;
        }
    }
    return result;
};
