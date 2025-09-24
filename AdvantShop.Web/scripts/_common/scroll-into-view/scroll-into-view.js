const moduleName = `scrollIntoView`;

const scrollToElement = (element) => {
    //element.scrollIntoView({ inline: 'center', block: 'nearest', behavior: 'smooth' })
    const parent = element.parentElement;
    const elWidth = element.offsetWidth;
    const parentCenter = parent.offsetWidth / 2;
    parent.scroll({ behavior: `smooth`, left: element.offsetLeft - parentCenter + elWidth / 2 });
};

class ScrollIntoView {
    constructor($parse) {
        this.scope = true;
        this.compile = (cElement, cAttrs) => {
            return (scope, element, attrs) => {
                scope.$watch(
                    (scope) => $parse(attrs.scrollIntoView)(scope) === true,
                    (newVal, oldVal) => {
                        if (newVal === true) {
                            scrollToElement(element[0]);
                        }
                    },
                );
            };
        };
    }
}

angular.module(moduleName, []).directive(`scrollIntoView`, /* @ngInject */ ($parse) => new ScrollIntoView($parse));

export default moduleName;
